#if HAS_SPAN
using System.Buffers;
using System.Buffers.Binary;
#endif

using System.Diagnostics;

namespace QRCoder;

public partial class QRCodeGenerator
{
    private sealed class ModuleMatrix : IDisposable
    {
        public byte[] Bytes { get; }
        public int Size { get; }
        public int Stride { get; }
        private int ByteLength { get; }
        private bool Disposed { get; set; }

#if !HAS_SPAN
#if NET40_OR_GREATER
        private static readonly System.Collections.Concurrent.ConcurrentStack<byte[]> _pool = new();
#else
        private static byte[]? _pooledBytes;
#endif
#endif

        public ModuleMatrix(int size)
        {
            Debug.Assert(size > 0);

            Size = size;

            // Round up to number of bytes required per row and add one byte per row.
            // The one byte extra will allow us to copy bit arrays with 4 bits of padding on each side in CopyFrom(QRCodeData data).
            Stride = (size + 15) >> 3;

            ByteLength = Size * Stride;

#if HAS_SPAN
            Bytes = ArrayPool<byte>.Shared.Rent(ByteLength);
            Bytes.AsSpan(0, ByteLength).Clear();
#elif NET40_OR_GREATER
            while (_pool.TryPop(out byte[]? bytes))
            {
                // Test whether the array is large enough.
                // If not, discard it and don't return it to the pool. When this instance is disposed, a larger array will be returned to the pool.
                // Over time, the pool will tend to be filled with enough arrays that are large enough for the workload.
                if (bytes.Length >= ByteLength)
                {
                    Array.Clear(bytes, 0, ByteLength);
                    Bytes = bytes;
                    return;
                }
            }

            Bytes = new byte[ByteLength];
#else
            var bytes = Interlocked.Exchange(ref _pooledBytes, null);
            if (bytes != null)
            {
                if (bytes.Length >= ByteLength)
                {
                    Array.Clear(bytes, 0, ByteLength);
                    Bytes = bytes;
                    return;
                }
                else
                {
                    Interlocked.Exchange(ref _pooledBytes, bytes);
                }
            }

            Bytes = new byte[ByteLength];
#endif
        }

        public bool this[int y, int x]
        {
            get
            {
                Debug.Assert((uint)y < (uint)Size);
                Debug.Assert((uint)x < (uint)Size);

                return (GetByteRef(y, x) & (1 << (x & 7))) != 0;
            }
            set
            {
                Debug.Assert(!Disposed);
                Debug.Assert((uint)y < (uint)Size);
                Debug.Assert((uint)x < (uint)Size);

                int mask = 1 << (x & 7);

                ref byte target = ref GetByteRef(y, x);

                if (value)
                {
                    target |= (byte)mask;
                }
                else
                {
                    target &= (byte)~mask;
                }
            }
        }

        private ref byte GetByteRef(int y, int x) => ref Bytes[y * Stride + (x >> 3)];

        public void CopyFrom(QRCodeData data)
        {
            Debug.Assert(!Disposed);
            Debug.Assert(Size == data.ModuleMatrix.Count - 8);

            var source = data.ModuleMatrix;
            var target = Bytes;

#if NETSTANDARD1_3
            Array.Clear(target, 0, target.Length);
#endif

            for (int y = 0; y < Size; y++)
            {
                var bitArray = source[y + 4];

#if NETSTANDARD1_3
                int startIndex = y * Stride;

                for (int x = 0; x < bitArray.Length - 8; x++)
                {
                    if (bitArray[x + 4])
                    {
                        target[startIndex + (x >> 3)] |= (byte)(1 << (x & 7));
                    }
                }
#else
                bitArray.CopyTo(target, y * Stride);
#endif
            }

#if !NETSTANDARD1_3
            // Move all modules 4 places to the left
#if HAS_SPAN
            if (BitConverter.IsLittleEndian)
            {
                var span = target.AsSpan(0, ByteLength);

                if (Environment.Is64BitProcess)
                {
                    while (span.Length >= 8 + 1)
                    {
                        BinaryPrimitives.WriteUInt64LittleEndian(span, (BinaryPrimitives.ReadUInt64LittleEndian(span) >>> 4) | (((ulong)span[8]) << (64 - 4)));
                        span = span[8..];
                    }
                }
                else
                {
                    while (span.Length >= 4 + 1)
                    {
                        BinaryPrimitives.WriteUInt32LittleEndian(span, (BinaryPrimitives.ReadUInt32LittleEndian(span) >>> 4) | (((uint)span[4]) << (32 - 4)));
                        span = span[4..];
                    }
                }

                while (span.Length >= 2)
                {
                    span[0] = (byte)((span[0] >> 4) | ((span[1] & 15) << (8 - 4)));
                    span = span[1..];
                }

                return;
            }
#endif
            for (int i = 1; i < ByteLength; i++)
            {
                target[i - 1] = (byte)((target[i - 1] >>> 4) | ((target[i] & 15) << (8 - 4)));
            }
#endif
        }

        public void SetModules(Rectangle rectangle)
        {
            Debug.Assert(!Disposed);
            Debug.Assert((uint)rectangle.X < (uint)Size && (uint)rectangle.Y < (uint)Size);
            Debug.Assert((uint)(rectangle.X + rectangle.Width) <= (uint)Size && (uint)(rectangle.Y + rectangle.Height) <= Size);

            for (int x = 0; x < rectangle.Width; x++)
            {
                var column = GetColumn(rectangle.X + x);
                for (int y = 0; y < rectangle.Height; y++)
                {
                    column[rectangle.Y + y] = true;
                }
            }
        }

        public bool HasModulesSet(Rectangle rectangle)
        {
            Debug.Assert((uint)rectangle.X < (uint)Size && (uint)rectangle.Y < (uint)Size);
            Debug.Assert((uint)(rectangle.X + rectangle.Width) <= (uint)Size && (uint)(rectangle.Y + rectangle.Height) <= Size);

            for (int x = 0; x < rectangle.Width; x++)
            {
                var column = GetColumn(rectangle.X + x);
                for (int y = 0; y < rectangle.Height; y++)
                {
                    if (column[rectangle.Y + y])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates a penalty score for a Micro QR code to evaluate the effectiveness of a mask pattern.
        /// A lower score indicates a QR code that is easier for decoders to read accurately.
        /// </summary>
        /// <returns>The total penalty score of the QR code.</returns>
        public int ScoreMicro()
        {
            int size = Size;
            int sum1 = 0;
            int sum2 = 0;
            for (int i = 1; i < size; i++)
            {
                if (this[size - 1, i])
                    sum1++;
                if (this[i, size - 1])
                    sum2++;
            }
            int total = sum1 < sum2 ? sum1 * 16 + sum2 : sum2 * 16 + sum1;
            return -total; // negate so that lower is better
        }

        /// <summary>
        /// Calculates a penalty score for a QR code to evaluate the effectiveness of a mask pattern.
        /// A lower score indicates a QR code that is easier for decoders to read accurately.
        /// The score is the sum of four penalty rules applied to the QR code.
        /// </summary>
        /// <returns>The total penalty score of the QR code.</returns>
        public int Score()
        {
            int score = 0;
            int blackModules = 0;

            int size = Size;

            // Penalty 1: Penalty for groups of five or more same-color modules in a row (or column)
            for (int i = 0; i < size; i++)
            {
                var row = GetRow(i);
                var column = GetColumn(i);

                int modInRow = 0, modInColumn = 0;

                bool lastValRow = false, lastValColumn = false;

                for (int j = 0; j < size; j++)
                {
                    bool current = row[j];

                    if (current)
                        blackModules++;

                    // Check rows for consecutive modules
                    if (current == lastValRow)
                    {
                        modInRow++;
                        if (modInRow == 5)
                            score += 3;
                        else if (modInRow > 5)
                            score++;
                    }
                    else
                    {
                        modInRow = 1;
                    }

                    lastValRow = current;

                    // Check columns for consecutive modules
                    current = column[j];

                    if (current == lastValColumn)
                    {
                        modInColumn++;
                        if (modInColumn == 5)
                            score += 3;
                        else if (modInColumn > 5)
                            score++;
                    }
                    else
                    {
                        modInColumn = 1;
                    }

                    lastValColumn = current;
                }
            }

            // Penalty 4: Penalty for having more than 50% black modules or more than 50% white modules
            int percentDiv5 = blackModules * 20 / (size * size);
            int prevMultipleOf5 = Math.Abs(percentDiv5 - 10);
            int nextMultipleOf5 = Math.Abs(percentDiv5 - 9);
            score += Math.Min(prevMultipleOf5, nextMultipleOf5) * 10;

            // Penalty 2: Penalty for square blocks of four modules in the same color
            for (int y = 0; y < size - 1; y++)
            {
                var currentRow = GetRow(y);
                var nextRow = GetRow(y + 1);

                for (int x = 0; x < size - 1; x++)
                {
                    bool topRightModule = currentRow[x + 1];

                    if (topRightModule != nextRow[x + 1])
                    {
                        // If the right modules don't match, skip the next column.
                        // They wouldn't match being the left modules either.
                        x++;
                    }
                    else if (topRightModule == currentRow[x] && topRightModule == nextRow[x])
                    {
                        score += 3;
                    }
                }
            }

            // Penalty 3: Penalty for specific patterns within the QR code (patterns that should be avoided)
            for (int i = 0; i < size; i++)
            {
                // Horizontal pattern matching
                var r = GetRow(i);

                for (int j = 0; j < size - 10; j++)
                {
                    if (r[j + 6])
                    {
                        if (!r[j + 1] && r[j + 4] && !r[j + 5] && !r[j + 9])
                        {
                            if (r[j] && r[j + 2] && r[j + 3] && !r[j + 7] && !r[j + 8] && !r[j + 10] ||
                                !r[j] && !r[j + 2] && !r[j + 3] && r[j + 7] && r[j + 8] && r[j + 10])
                            {
                                score += 40;
                            }
                        }

                        // If r[j + 6] was set, then r[j + 5] would be set when we move one column.
                        j++;
                    }
                }

                // Vertical pattern matching
                var c = GetColumn(i);

                for (int j = 0; j < size - 10; j++)
                {
                    if (c[j + 6])
                    {
                        if (!c[j + 1] && c[j + 4] && !c[j + 5] && !c[j + 9])
                        {
                            if (c[j] && c[j + 2] && c[j + 3] && !c[j + 7] && !c[j + 8] && !c[j + 10] ||
                                !c[j] && !c[j + 2] && !c[j + 3] && c[j + 7] && c[j + 8] && c[j + 10])
                            {
                                score += 40;
                            }
                        }

                        // If c[j + 6] was set, then c[j + 5] would be set when we move one row.
                        j++;
                    }
                }
            }

            return score;
        }

        public void Dispose()
        {
            Debug.Assert(!Disposed);
            Disposed = true;

#if HAS_SPAN
            // Avoid leaking QR code data into the pool
            Bytes.AsSpan(0, ByteLength).Clear();
            ArrayPool<byte>.Shared.Return(Bytes);
#elif NET40_OR_GREATER
            _pool.Push(Bytes);
#else
            Interlocked.Exchange(ref _pooledBytes, Bytes);
#endif
        }

        public Row GetRow(int y) => new Row(this, y);

        public Column GetColumn(int x) => new Column(this, x);

        public readonly struct Row
        {
            private readonly ModuleMatrix _moduleMatrix;

            private readonly int _startByteIndex;

            public Row(ModuleMatrix moduleMatrix, int y)
            {
                Debug.Assert((uint)y < (uint)moduleMatrix.Size);
                _moduleMatrix = moduleMatrix;
                _startByteIndex = moduleMatrix.Stride * y;
            }

            public bool this[int x]
            {
                get
                {
                    Debug.Assert((uint)x < (uint)_moduleMatrix.Size);

                    return (_moduleMatrix.Bytes[_startByteIndex + (x >> 3)] & (1 << (x & 7))) != 0;
                }
            }
        }

        public readonly struct Column
        {
            private readonly ModuleMatrix _moduleMatrix;
            private readonly int _byteIndex;
            private readonly byte _byteMask;

            public Column(ModuleMatrix moduleMatrix, int x)
            {
                Debug.Assert((uint)x < (uint)moduleMatrix.Size);
                _moduleMatrix = moduleMatrix;
                _byteIndex = x >> 3;
                _byteMask = (byte)(1 << (x & 7));
            }

            public bool this[int y]
            {
                get
                {
                    Debug.Assert((uint)y < (uint)_moduleMatrix.Size);

                    return (_moduleMatrix.Bytes[y * _moduleMatrix.Stride + _byteIndex] & _byteMask) != 0;
                }
                set
                {
                    Debug.Assert(!_moduleMatrix.Disposed);
                    Debug.Assert((uint)y < (uint)_moduleMatrix.Size);

                    ref byte target = ref _moduleMatrix.Bytes[y * _moduleMatrix.Stride + _byteIndex];

                    if (value)
                    {
                        target |= _byteMask;
                    }
                    else
                    {
                        target &= (byte)~_byteMask;
                    }
                }
            }
        }
    }
}

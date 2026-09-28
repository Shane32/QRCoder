using System.Diagnostics;

namespace QRCoder;

public partial class QRCodeGenerator
{
    private static partial class ModulePlacer
    {
        /// <summary>
        /// Provides the data to handle mask patterns used in QR code generation.
        /// Mask patterns are applied to QR codes to break up patterns in the data matrix that might confuse scanners.
        /// </summary>
        private readonly struct MaskPattern
        {
            /// <summary>
            /// An array mapping each mask pattern index to its corresponding pattern that determines whether a given pixel should be masked.
            /// </summary>
            public static readonly MaskPattern[] Patterns =
                [Pattern1, Pattern2, Pattern3, Pattern4, Pattern5, Pattern6, Pattern7, Pattern8];

            /// <summary>
            /// Mask pattern 1: (x + y) % 2 == 0
            /// Applies a checkerboard mask on the QR code.
            /// </summary>
            private static MaskPattern Pattern1 => new MaskPattern((int x, int y) => (x + y) % 2 == 0);

            /// <summary>
            /// Mask pattern 2: y % 2 == 0
            /// Applies a horizontal striping mask on the QR code.
            /// </summary>
            private static MaskPattern Pattern2 => new MaskPattern((int x, int y) => y % 2 == 0);

            /// <summary>
            /// Mask pattern 3: x % 3 == 0
            /// Applies a vertical striping mask on the QR code.
            /// </summary>
            private static MaskPattern Pattern3 => new MaskPattern((int x, int y) => x % 3 == 0);

            /// <summary>
            /// Mask pattern 4: (x + y) % 3 == 0
            /// Applies a diagonal striping mask on the QR code.
            /// </summary>
            private static MaskPattern Pattern4 => new MaskPattern((int x, int y) => (x + y) % 3 == 0);

            /// <summary>
            /// Mask pattern 5: ((y / 2) + (x / 3)) % 2 == 0
            /// Applies a complex pattern mask on the QR code, mixing horizontal and vertical rules.
            /// </summary>
            private static MaskPattern Pattern5 => new MaskPattern((int x, int y) => ((y / 2) + (x / 3)) % 2 == 0);

            /// <summary>
            /// Mask pattern 6: ((x * y) % 2 + (x * y) % 3) == 0
            /// Applies a mask based on the product of x and y coordinates modulo 2 and 3.
            /// </summary>
            private static MaskPattern Pattern6 => new MaskPattern((int x, int y) => ((x * y) % 2) + ((x * y) % 3) == 0);

            /// <summary>
            /// Mask pattern 7: (((x * y) % 2 + (x * y) % 3) % 2) == 0
            /// Applies a mask based on a more complex function involving the product of x and y coordinates.
            /// </summary>
            private static MaskPattern Pattern7 => new MaskPattern((int x, int y) => (((x * y) % 2) + ((x * y) % 3)) % 2 == 0);

            /// <summary>
            /// Mask pattern 8: (((x + y) % 2) + ((x * y) % 3) % 2) == 0
            /// Combines rules of checkers and complex multiplicative masks.
            /// </summary>
            private static MaskPattern Pattern8 => new MaskPattern((int x, int y) => (((x + y) % 2) + ((x * y) % 3)) % 2 == 0);

            private readonly byte[] _bytes;

            public Func<int, int, bool> Generator { get; }

            private const int PATTERN_SIZE = 12;

            public MaskPattern(Func<int, int, bool> generator)
            {
                Generator = generator;

                var bytes = new byte[2 * PATTERN_SIZE * PATTERN_SIZE / 8];
                for (int x = 0; x < PATTERN_SIZE; x++)
                {
                    int byteIndex1 = x >> 3;
                    byte mask1 = (byte)(1 << (x & 7));

                    int byteIndex2 = (PATTERN_SIZE + x) >> 3;
                    byte mask2 = (byte)(1 << ((PATTERN_SIZE + x) & 7));

                    for (int y = 0; y < PATTERN_SIZE; y++)
                    {
                        if (generator(x, y))
                        {
                            bytes[y * (2 * PATTERN_SIZE / 8) + byteIndex1] |= mask1;
                            bytes[y * (2 * PATTERN_SIZE / 8) + byteIndex2] |= mask2;
                        }
                    }
                }

                _bytes = bytes;
            }

            public void Apply(ModuleMatrix moduleMatrix, ModuleMatrix blockedModules)
            {
                Debug.Assert(moduleMatrix.Size == blockedModules.Size);
                Debug.Assert(moduleMatrix.Stride == blockedModules.Stride);

                var pattern = _bytes;
                var target = moduleMatrix.Bytes;
                var blocked = blockedModules.Bytes;

                for (int y = 0; y < moduleMatrix.Size; y++)
                {
                    int rowStartIndex = y * moduleMatrix.Stride;

                    byte byte0 = pattern[y % PATTERN_SIZE * 3 + 0];
                    byte byte1 = pattern[y % PATTERN_SIZE * 3 + 1];
                    byte byte2 = pattern[y % PATTERN_SIZE * 3 + 2];

                    int bx = 0;

                    for (; bx + 3 <= moduleMatrix.Stride; bx += 3)
                    {
                        target[rowStartIndex + bx + 0] ^= (byte)(byte0 & ~blocked[rowStartIndex + bx + 0]);
                        target[rowStartIndex + bx + 1] ^= (byte)(byte1 & ~blocked[rowStartIndex + bx + 1]);
                        target[rowStartIndex + bx + 2] ^= (byte)(byte2 & ~blocked[rowStartIndex + bx + 2]);
                    }

                    if (bx < moduleMatrix.Stride)
                    {
                        target[rowStartIndex + bx] ^= (byte)(byte0 & ~blocked[rowStartIndex + bx]);

                        bx++;

                        if (bx < moduleMatrix.Stride)
                        {
                            target[rowStartIndex + bx] ^= (byte)(byte1 & ~blocked[rowStartIndex + bx]);
                        }
                    }
                }
            }
        }
    }
}

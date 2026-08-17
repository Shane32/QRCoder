using System.Diagnostics;

namespace QRCoder;

public partial class QRCodeGenerator
{
    /// <summary>
    /// Represents a polynomial, which is a sum of polynomial terms.
    /// </summary>
    internal struct Polynom : IDisposable
    {
#if HAS_SPAN
        private static ReadOnlySpan<byte> _generatorPolynomCoefficients =>
#else
        private static readonly byte[] _generatorPolynomCoefficients =
#endif
            [
                0,
                1, 25,
                3, 199, 198,
                6, 78, 249, 75,
                10, 119, 166, 164, 113,
                15, 176, 5, 134, 0, 166,
                21, 102, 238, 149, 146, 229, 87,
                28, 196, 252, 215, 249, 208, 238, 175,
                36, 123, 11, 149, 235, 231, 137, 246, 95,
                45, 32, 94, 64, 70, 118, 61, 46, 67, 251,
                55, 10, 227, 116, 209, 177, 172, 194, 91, 192, 220,
                66, 157, 87, 131, 143, 198, 113, 187, 121, 98, 43, 102,
                78, 140, 206, 218, 130, 104, 106, 100, 86, 100, 176, 152, 74,
                91, 22, 59, 207, 87, 216, 137, 218, 124, 190, 48, 155, 249, 199,
                105, 99, 5, 124, 140, 237, 58, 58, 51, 37, 202, 91, 61, 183, 8,
                120, 225, 194, 182, 169, 147, 191, 91, 3, 76, 161, 102, 109, 107, 104, 120,
                136, 163, 243, 39, 150, 99, 24, 147, 214, 206, 123, 239, 43, 78, 206, 139, 43,
                153, 96, 98, 5, 179, 252, 148, 152, 187, 79, 170, 118, 97, 184, 94, 158, 234, 215,
                171, 220, 138, 222, 252, 133, 153, 128, 44, 159, 150, 17, 83, 90, 52, 153, 105, 3, 67,
                190, 188, 212, 212, 164, 156, 239, 83, 225, 221, 180, 202, 187, 26, 163, 61, 50, 79, 60, 17,
                210, 175, 148, 254, 122, 36, 230, 137, 148, 115, 210, 200, 85, 98, 67, 140, 181, 247, 104, 233, 240,
                231, 165, 105, 160, 134, 219, 80, 98, 172, 8, 74, 200, 53, 221, 109, 14, 230, 93, 242, 247, 171, 210,
                253, 147, 56, 78, 1, 192, 224, 164, 94, 248, 183, 25, 14, 150, 193, 17, 65, 103, 49, 91, 146, 102, 171,
                21, 227, 96, 87, 232, 117, 0, 111, 218, 228, 226, 192, 152, 169, 180, 159, 126, 251, 117, 211, 48, 135, 121, 229,
                45, 252, 178, 129, 243, 95, 182, 144, 167, 99, 208, 237, 66, 54, 201, 148, 15, 59, 12, 26, 170, 39, 156, 181, 231,
                70, 218, 145, 153, 227, 48, 102, 13, 142, 245, 21, 161, 53, 165, 28, 111, 201, 145, 17, 118, 182, 103, 2, 158, 125, 173,
                96, 149, 17, 26, 157, 193, 216, 94, 172, 126, 73, 135, 138, 58, 45, 99, 70, 237, 9, 29, 180, 21, 227, 165, 8, 228, 79,
                123, 9, 37, 242, 119, 212, 195, 42, 87, 245, 43, 21, 201, 232, 27, 205, 147, 195, 190, 110, 180, 108, 234, 224, 104, 200, 223, 168,
                151, 24, 140, 250, 68, 162, 202, 9, 23, 148, 150, 234, 75, 28, 189, 175, 241, 5, 136, 24, 249, 96, 54, 219, 151, 29, 183, 45, 156,
                180, 192, 40, 238, 216, 251, 37, 156, 130, 224, 193, 226, 173, 42, 125, 222, 96, 239, 86, 110, 48, 50, 182, 179, 31, 216, 152, 145, 173, 41,
                210, 200, 187, 117, 183, 123, 105, 225, 1, 55, 248, 248, 144, 119, 118, 137, 122, 73, 44, 39, 113, 83, 115, 31, 225, 75, 63, 93, 252, 37, 20
            ];

        /// <summary>
        /// Creates the generator polynomial used for creating error correction codewords.
        /// </summary>
        /// <param name="numEccWords">The number of error correction codewords to generate.</param>
        /// <returns>A polynomial that can be used to generate ECC codewords.</returns>
        public static Polynom CreateGeneratorPolynom(int numEccWords)
        {
            Debug.Assert(numEccWords < 32);

            int startIndex = (numEccWords - 1) * numEccWords / 2;

            var generatorPolynomial = new Polynom(numEccWords + 1);

            // Return the polynomial terms by exponent in descending order.
            // The highest order coefficient is always 0.
            generatorPolynomial.Add(new PolynomItem(0, numEccWords));

#if HAS_SPAN
            var coefficients = _generatorPolynomCoefficients.Slice(startIndex, numEccWords);
            for (int i = coefficients.Length - 1; i >= 0; i--)
            {
                generatorPolynomial.Add(new PolynomItem(coefficients[i], i));
            }
#else
            for (int i = numEccWords - 1; i >= 0; i--)
            {
                generatorPolynomial.Add(new PolynomItem(_generatorPolynomCoefficients[startIndex + i], i));
            }
#endif

            return generatorPolynomial;
        }

        private PolynomItem[] _polyItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynom"/> struct with a specified number of initial capacity for polynomial terms.
        /// </summary>
        /// <param name="count">The initial capacity of the polynomial items list.</param>
        public Polynom(int count)
        {
            Count = 0;
            _polyItems = RentArray(count);
        }

        /// <summary>
        /// Adds a polynomial term to the polynomial.
        /// </summary>
        public void Add(PolynomItem item)
        {
            AssertCapacity(Count + 1);
            _polyItems[Count++] = item;
        }

        /// <summary>
        /// Removes the polynomial term at the specified index.
        /// </summary>
        public void RemoveAt(int index)
        {
            Debug.Assert((uint)index < (uint)Count);

            if (index < Count - 1)
                Array.Copy(_polyItems, index + 1, _polyItems, index, Count - index - 1);

            Count--;
        }

        /// <summary>
        /// Gets or sets a polynomial term at the specified index.
        /// </summary>
        public PolynomItem this[int index]
        {
            get
            {
                Debug.Assert((uint)index < Count);
                return _polyItems[index];
            }
            set
            {
                Debug.Assert((uint)index < Count);
                _polyItems[index] = value;
            }
        }


        /// <summary>
        /// Gets the number of polynomial terms in the polynomial.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Removes all polynomial terms from the polynomial.
        /// </summary>
        public void Clear() => Count = 0;

        /// <summary>
        /// Clones the polynomial, creating a new instance with the same polynomial terms.
        /// </summary>
        public Polynom Clone()
        {
            var newPolynom = new Polynom(Count);
            Array.Copy(_polyItems, newPolynom._polyItems, Count);
            newPolynom.Count = Count;
            return newPolynom;
        }

        /// <summary>
        /// Returns a string that represents the polynomial in standard algebraic notation.
        /// Example output: "a^2*x^3 + a^5*x^1 + a^3*x^0", which represents the polynomial 2x³ + 5x + 3.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                var polyItem = _polyItems[i];
                sb.Append("a^" + polyItem.Coefficient + "*x^" + polyItem.Exponent + " + ");
            }

            // Remove the trailing " + " if the string builder has added terms
            if (sb.Length > 0)
                sb.Length -= 3;

            return sb.ToString();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReturnArray(_polyItems);
            _polyItems = null!;
        }

        /// <summary>
        /// Ensures that the polynomial has enough capacity to store the specified number of polynomial terms.
        /// </summary>
        private void AssertCapacity(int min)
        {
            // All math by QRCoder should be done with fixed polynomials, so we don't need to grow the capacity.
            Debug.Assert(_polyItems.Length >= min);
        }

#if HAS_SPAN
        /// <summary>
        /// Rents memory for the polynomial terms from the shared memory pool.
        /// </summary>
        private static PolynomItem[] RentArray(int count)
            => System.Buffers.ArrayPool<PolynomItem>.Shared.Rent(count);

        /// <summary>
        /// Returns memory allocated for the polynomial terms back to the shared memory pool.
        /// </summary>
        private static void ReturnArray(PolynomItem[] array)
            => System.Buffers.ArrayPool<PolynomItem>.Shared.Return(array);
#else
        // Implement a poor-man's array pool for .NET Framework
        [ThreadStatic]
        private static List<PolynomItem[]>? _arrayPool;

        /// <summary>
        /// Rents memory for the polynomial terms from a shared memory pool.
        /// </summary>
        private static PolynomItem[] RentArray(int count)
        {
            if (count <= 0)
                ThrowArgumentOutOfRangeException();

            // Search for a suitable array in the thread-local pool, if it has been initialized
            if (_arrayPool != null)
            {
                for (int i = 0; i < _arrayPool.Count; i++)
                {
                    var array = _arrayPool[i];
                    if (array.Length >= count)
                    {
                        _arrayPool.RemoveAt(i);
                        return array;
                    }
                }
            }

            // No suitable buffer found; create a new one
            return new PolynomItem[count];

            void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException(nameof(count), "The count must be a positive number.");
        }

        /// <summary>
        /// Returns memory allocated for the polynomial terms back to a shared memory pool.
        /// </summary>
        private static void ReturnArray(PolynomItem[] array)
        {
            if (array == null)
                return;

            // Initialize the thread-local pool if it's not already done
            _arrayPool ??= new List<PolynomItem[]>(8);

            // Add the buffer back to the pool
            _arrayPool.Add(array);
        }
#endif

        /// <summary>
        /// Returns an enumerator that iterates through the polynomial terms.
        /// </summary>
        public PolynumEnumerator GetEnumerator() => new PolynumEnumerator(this);

        /// <summary>
        /// Value type enumerator for the <see cref="Polynom"/> struct.
        /// </summary>
        public struct PolynumEnumerator
        {
            private Polynom _polynom;
            private int _index;

            public PolynumEnumerator(Polynom polynom)
            {
                _polynom = polynom;
                _index = -1;
            }

            public PolynomItem Current => _polynom[_index];

            public bool MoveNext() => ++_index < _polynom.Count;
        }
    }
}

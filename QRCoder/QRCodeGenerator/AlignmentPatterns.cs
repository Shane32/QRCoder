using System.Diagnostics;

namespace QRCoder;

public partial class QRCodeGenerator
{
    /// <summary>
    /// This class contains the alignment patterns used in QR codes.
    /// </summary>
    private static class AlignmentPatterns
    {
        /// <summary>
        /// A lookup table mapping QR code versions to their corresponding alignment patterns.
        /// </summary>
        /// <remarks>
        /// Versions range from -4 up to and including -1 and 1 up to and including 40.
        /// </remarks>
        private static readonly Point[][] _alignmentPatternTable = CreateAlignmentPatternTable();

        /// <summary>
        /// Offset used to map -4 .. 40 to 0 .. 44
        /// </summary>
        private const int INDEX_OFFSET = 4;

        /// <summary>
        /// Retrieves the alignment pattern for a specific QR code version.
        /// </summary>
        public static Point[] FromVersion(int version) => _alignmentPatternTable[version + INDEX_OFFSET];

        /// <summary>
        /// Creates a lookup table mapping QR code versions to their corresponding alignment patterns.
        /// Alignment patterns are used in QR codes to help scanners accurately read the code at high speeds and when partially obscured.
        /// This table provides the necessary patterns based on the QR code version which dictates the size and complexity of the QR code.
        /// </summary>
        /// <returns>An array where indices are QR code version numbers offset by 4 and values are Point arrays containing the positions of alignment patterns for each version.</returns>
        private static Point[][] CreateAlignmentPatternTable()
        {
            var localAlignmentPatternTable = new Point[4 + 1 + 40][];

            // Micro QR codes do not have alignment patterns.
            Point[] empty = [];

            localAlignmentPatternTable[-4 + INDEX_OFFSET] = empty;
            localAlignmentPatternTable[-3 + INDEX_OFFSET] = empty;
            localAlignmentPatternTable[-2 + INDEX_OFFSET] = empty;
            localAlignmentPatternTable[-1 + INDEX_OFFSET] = empty;

            // A Version 1 QR code does not have alignment patterns.
            localAlignmentPatternTable[1 + INDEX_OFFSET] = empty;

#if HAS_SPAN
            ReadOnlySpan<byte> alignmentPatternBaseValues =
#else
            byte[] alignmentPatternBaseValues =
#endif
                [4, 16, 0, 0, 0, 0, 0, 4, 20, 0, 0, 0, 0, 0, 4, 24, 0, 0, 0, 0, 0, 4, 28, 0, 0, 0, 0, 0, 4, 32, 0, 0, 0, 0, 0, 4, 20, 36, 0, 0, 0, 0, 4, 22, 40, 0, 0, 0, 0, 4, 24, 44, 0, 0, 0, 0, 4, 26, 48, 0, 0, 0, 0, 4, 28, 52, 0, 0, 0, 0, 4, 30, 56, 0, 0, 0, 0, 4, 32, 60, 0, 0, 0, 0, 4, 24, 44, 64, 0, 0, 0, 4, 24, 46, 68, 0, 0, 0, 4, 24, 48, 72, 0, 0, 0, 4, 28, 52, 76, 0, 0, 0, 4, 28, 54, 80, 0, 0, 0, 4, 28, 56, 84, 0, 0, 0, 4, 32, 60, 88, 0, 0, 0, 4, 26, 48, 70, 92, 0, 0, 4, 24, 48, 72, 96, 0, 0, 4, 28, 52, 76, 100, 0, 0, 4, 26, 52, 78, 104, 0, 0, 4, 30, 56, 82, 108, 0, 0, 4, 28, 56, 84, 112, 0, 0, 4, 32, 60, 88, 116, 0, 0, 4, 24, 48, 72, 96, 120, 0, 4, 28, 52, 76, 100, 124, 0, 4, 24, 50, 76, 102, 128, 0, 4, 28, 54, 80, 106, 132, 0, 4, 32, 58, 84, 110, 136, 0, 4, 28, 56, 84, 112, 140, 0, 4, 32, 60, 88, 116, 144, 0, 4, 28, 52, 76, 100, 124, 148, 4, 22, 48, 74, 100, 126, 152, 4, 26, 52, 78, 104, 130, 156, 4, 30, 56, 82, 108, 134, 160, 4, 24, 52, 80, 108, 136, 164, 4, 28, 56, 84, 112, 140, 168];

            var points = new List<Point>(7 * 7);

            for (var version = 2; version <= 40; version++)
            {
                var indexBase = (version - 2) * 7;

                for (var x = 0; x < 7 && alignmentPatternBaseValues[indexBase + x] != 0; x++)
                {
                    for (var y = 0; y < 7 && alignmentPatternBaseValues[indexBase + y] != 0; y++)
                    {
                        points.Add(new Point(alignmentPatternBaseValues[indexBase + x], alignmentPatternBaseValues[indexBase + y]));
                    }
                }

                Debug.Assert(points.Count <= 7 * 7);

                localAlignmentPatternTable[version + INDEX_OFFSET] = points.ToArray();

                points.Clear();
            }

            return localAlignmentPatternTable;
        }
    }
}

namespace QRCoder;

public partial class QRCodeGenerator
{
    /// <summary>
    /// Represents a 2D point with byte coordinates.
    /// </summary>
    private readonly struct Point
    {
        /// <summary>
        /// Gets the X-coordinate of the point.
        /// </summary>
        public byte X { get; }

        /// <summary>
        /// Gets the Y-coordinate of the point.
        /// </summary>
        public byte Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct with specified X and Y coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the point.</param>
        /// <param name="y">The Y-coordinate of the point.</param>
        public Point(byte x, byte y)
        {
            X = x;
            Y = y;
        }
    }
}

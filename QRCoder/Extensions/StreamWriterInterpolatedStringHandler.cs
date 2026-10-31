using System.Runtime.CompilerServices;

namespace QRCoder;

/// <summary>
/// Writes interpolated values directly to a StreamWriter using invariant culture.
/// </summary>
[InterpolatedStringHandler]
internal struct StreamWriterInterpolatedStringHandler
{
    private readonly StreamWriter _writer;

    /// <summary>
    /// Initializes the handler with the writer to write to.
    /// </summary>
    /// <param name="literalLength">The number of literal characters.</param>
    /// <param name="formattedCount">The number of interpolated values.</param>
    /// <param name="writer">The writer to write to.</param>
    public StreamWriterInterpolatedStringHandler(int literalLength, int formattedCount, StreamWriter writer)
        => _writer = writer;

    /// <summary>
    /// Writes a literal string.
    /// </summary>
    /// <param name="value">The literal string.</param>
    public void AppendLiteral(string value) => _writer.Write(value);

    /// <summary>
    /// Writes a string, writing nothing when the value is null.
    /// </summary>
    /// <param name="value">The string to write.</param>
    /// <param name="format">The format string, ignored for strings.</param>
    public void AppendFormatted(string? value, string? format = null) => _writer.Write(value);

    /// <summary>
    /// Writes an integer using invariant culture.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted(int value, string? format = null) => WriteFormatted(value, format);

    /// <summary>
    /// Writes a long integer using invariant culture.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted(long value, string? format = null) => WriteFormatted(value, format);

    /// <summary>
    /// Writes a single-precision number using invariant culture.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted(float value, string? format = null) => WriteFormatted(value, format);

    /// <summary>
    /// Writes a double-precision number using invariant culture.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted(double value, string? format = null) => WriteFormatted(value, format);

    /// <summary>
    /// Writes a decimal number using invariant culture.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted(decimal value, string? format = null) => WriteFormatted(value, format);

    /// <summary>
    /// Writes a value using invariant culture.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted<T>(T value, string? format = null)
        => _writer.Write(value is IFormattable formattable
            ? formattable.ToString(format, CultureInfo.InvariantCulture)
            : value?.ToString());

#if NET6_0_OR_GREATER
    private void WriteFormatted<T>(T value, string? format) where T : ISpanFormattable
#else
    private void WriteFormatted<T>(T value, string? format) where T : IFormattable
#endif
    {
#if NET6_0_OR_GREATER
        Span<char> buffer = stackalloc char[64];
        if (value.TryFormat(buffer, out int charsWritten, format, CultureInfo.InvariantCulture))
        {
            _writer.Write(buffer.Slice(0, charsWritten));
            return;
        }
#endif
        _writer.Write(value.ToString(format, CultureInfo.InvariantCulture));
    }
}

using System.Runtime.CompilerServices;

namespace QRCoder;

/// <summary>
/// Appends interpolated values directly to a StringBuilder using invariant culture.
/// </summary>
[InterpolatedStringHandler]
internal struct InvariantInterpolatedStringHandler
{
#if NET6_0_OR_GREATER
    private StringBuilder.AppendInterpolatedStringHandler _handler;
#else
    private readonly StringBuilder _builder;
#endif

    /// <summary>
    /// Initializes the handler with invariant culture before any values are formatted.
    /// </summary>
    /// <param name="literalLength">The number of literal characters.</param>
    /// <param name="formattedCount">The number of interpolated values.</param>
    /// <param name="builder">The builder to append to.</param>
    public InvariantInterpolatedStringHandler(int literalLength, int formattedCount, StringBuilder builder)
    {
#if NET6_0_OR_GREATER
        _handler = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, builder, CultureInfo.InvariantCulture);
#else
        _builder = builder;
#endif
    }

    /// <summary>
    /// Appends a literal string.
    /// </summary>
    /// <param name="value">The literal string.</param>
    public void AppendLiteral(string value)
    {
#if NET6_0_OR_GREATER
        _handler.AppendLiteral(value);
#else
        _builder.Append(value);
#endif
    }

    /// <summary>
    /// Appends a value using invariant culture.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void AppendFormatted<T>(T value)
    {
#if NET6_0_OR_GREATER
        _handler.AppendFormatted(value);
#else
        AppendFormatted(value, null);
#endif
    }

    /// <summary>
    /// Appends a value using the specified format and invariant culture.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted<T>(T value, string? format)
    {
#if NET6_0_OR_GREATER
        _handler.AppendFormatted(value, format);
#else
        _builder.Append(value is IFormattable formattable
            ? formattable.ToString(format, CultureInfo.InvariantCulture)
            : value?.ToString());
#endif
    }
}

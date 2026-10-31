using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace QRCoder;

internal static class StringExtensions
{
    /// <summary>
    /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <returns>
    ///   <see langword="true"/> if the <paramref name="value"/> is null, empty, or white space; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsNullOrWhiteSpace(
        [NotNullWhen(false)]
        this string? value)
    {
#if NET35
        if (value == null)
            return true;

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i]))
                return false;
        }

        return true;
#else
        return string.IsNullOrWhiteSpace(value);
#endif
    }

#if !NETCOREAPP2_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified character.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="c">The character to compare.</param>
    /// <returns>true if value starts with c; otherwise, false.</returns>
    internal static bool StartsWith(this string value, char c)
        => value.Length > 0 && value[0] == c;
#endif

    /// <summary>
    /// Converts a hex color string to a byte array.
    /// </summary>
    /// <param name="colorString">Color in HEX format like #ffffff.</param>
    /// <returns>Returns the color as a byte array.</returns>
    internal static byte[] HexColorToByteArray(this string colorString)
    {
        var offset = 0;
        if (colorString.StartsWith('#'))
            offset = 1;
        byte[] byteColor = new byte[(colorString.Length - offset) / 2];
        for (int i = 0; i < byteColor.Length; i++)
#if HAS_SPAN
            byteColor[i] = byte.Parse(colorString.AsSpan(i * 2 + offset, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
#else
            byteColor[i] = byte.Parse(colorString.Substring(i * 2 + offset, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
#endif
        return byteColor;
    }

#if NETSTANDARD1_3
    /// <inheritdoc cref="char.ToString()"/>
    internal static string ToString(this char c, CultureInfo _)
        => c.ToString();
#endif

    /// <summary>
    /// Appends an interpolated string using invariant culture formatting.
    /// The handler writes directly to <paramref name="sb"/> during interpolation, without an intermediate string on .NET 6+.
    /// </summary>
    internal static void AppendInvariant(
        StringBuilder sb,
        [InterpolatedStringHandlerArgument(nameof(sb))] ref InvariantInterpolatedStringHandler handler)
    {
        // The handler has already appended the interpolated string to sb.
    }

    /// <summary>
    /// Appends an integer value to the StringBuilder using invariant culture formatting.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="num">The integer value to append.</param>
    internal static void AppendInvariant(this StringBuilder sb, int num)
    {
#if NET6_0_OR_GREATER
        AppendInvariant(sb, $"{num}");
#else
#if HAS_SPAN
        Span<char> buffer = stackalloc char[16];
        if (num.TryFormat(buffer, out int charsWritten, default, CultureInfo.InvariantCulture))
        {
            sb.Append(buffer.Slice(0, charsWritten));
            return;
        }
#endif
        sb.Append(num.ToString(CultureInfo.InvariantCulture));
#endif
    }

    /// <summary>
    /// Appends a float value to the StringBuilder using invariant culture formatting with G7 precision.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <param name="num">The float value to append.</param>
    internal static void AppendInvariant(this StringBuilder sb, float num)
    {
#if NET6_0_OR_GREATER
        AppendInvariant(sb, $"{num:G7}");
#else
#if HAS_SPAN
        Span<char> buffer = stackalloc char[16];
        if (num.TryFormat(buffer, out int charsWritten, "G7", CultureInfo.InvariantCulture))
        {
            sb.Append(buffer.Slice(0, charsWritten));
            return;
        }
#endif
        sb.Append(num.ToString("G7", CultureInfo.InvariantCulture));
#endif
    }
}

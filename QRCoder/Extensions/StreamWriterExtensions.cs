using System.Runtime.CompilerServices;

namespace QRCoder;

internal static class StreamWriterExtensions
{
    /// <summary>
    /// Writes an interpolated string directly to the writer using invariant culture.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="handler">The handler that writes the interpolated string.</param>
    internal static void WriteInvariant(
        this StreamWriter writer,
        [InterpolatedStringHandlerArgument(nameof(writer))] ref StreamWriterInterpolatedStringHandler handler)
    {
        // The handler has already written the interpolated string to writer.
    }
}

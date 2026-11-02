using System.IO;
using System.Threading;

namespace QRCoderTests;

public class StreamWriterExtensionsTests
{
    [Fact]
    public void can_write_numbers_using_invariant_culture()
    {
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var culture = new CultureInfo("de-DE");
        culture.NumberFormat.NegativeSign = "~";
        culture.NumberFormat.PositiveSign = "^";

        try
        {
            Thread.CurrentThread.CurrentCulture = culture;
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);

            writer.WriteInvariant($"{-42}|{-1.2345678f:G7}|{12.5m:F2}|{42:0000000000}|{1.25e20:E2}|{1m:F70}");
            writer.Flush();

            Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("-42|-1.234568|12.50|0000000042|1.25E+020|1." + new string('0', 70));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void preserves_text_and_writer_encoding(bool unicode)
    {
        Encoding encoding = unicode ? new UnicodeEncoding(false, false) : new UTF8Encoding(false);
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, encoding, 128, true);
        string? missing = null;
        var text = "Grüße 漢字 😀";
        var longText = new string('é', 4095) + "😀" + new string('界', 4097);

        writer.WriteInvariant($"prefix|");
        writer.WriteInvariant($"{text}|{missing}|{longText}|suffix");
        writer.Flush();

        stream.ToArray().ShouldBe(encoding.GetBytes("prefix|" + text + "||" + longText + "|suffix"));
    }
}

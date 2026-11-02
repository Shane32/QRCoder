using System.IO;
using System.Threading;

namespace QRCoderTests;

public class PdfByteQRCodeRendererTests
{
    [Fact]
    public void can_render_pdfbyte_qrcode_blackwhite()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        byte[] pdfCodeGfx = new PdfByteQRCode(data).GetGraphic(5);
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    [Fact]
    public void can_render_pdfbyte_qrcode_color()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        byte[] pdfCodeGfx = new PdfByteQRCode(data).GetGraphic(5, "#FF0000", "#0000FF");
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    [Fact]
    public void can_render_pdfbyte_qrcode_custom_dpi()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        byte[] pdfCodeGfx = new PdfByteQRCode(data).GetGraphic(5, "#000000", "#FFFFFF", 300);
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    [Fact]
    public void renders_identical_pdf_bytes_across_cultures()
    {
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode("Culture-independent PDF", QRCodeGenerator.ECCLevel.L);
        using var pdfCode = new PdfByteQRCode(data);

        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            byte[] invariantBytes = pdfCode.GetGraphic(3, "#123456", "#E7F0FA", 137);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            pdfCode.GetGraphic(3, "#123456", "#E7F0FA", 137).ShouldBe(invariantBytes);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void can_instantate_pdfbyte_qrcode_parameterless()
    {
        var pdfCode = new PdfByteQRCode();
        pdfCode.ShouldNotBeNull();
        pdfCode.ShouldBeOfType<PdfByteQRCode>();
    }

    [Fact]
    public void can_render_pdfbyte_qrcode_from_helper()
    {
        byte[] pdfCodeGfx = PdfByteQRCodeHelper.GetQRCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L, 10);
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    [Fact]
    public void can_render_pdfbyte_qrcode_from_helper_2()
    {
        byte[] pdfCodeGfx = PdfByteQRCodeHelper.GetQRCode("This is a quick test! 123#?", 5, "#FF0000", "#0000FF", QRCodeGenerator.ECCLevel.L);
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    private static readonly char[] _lineEndChars = { '\r', '\n' };

    [Fact]
    public void pdf_xref_table_is_valid()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        byte[] pdfBytes = new PdfByteQRCode(data).GetGraphic(5);

        // Parse from the end to find startxref
        string pdfText = Encoding.ASCII.GetString(pdfBytes);

        // Verify no \n line breaks; only \r\n should be used (this test file has no binary image data)
        pdfText.Replace("\r\n", "CRLF").ShouldNotContain('\n', "PDF should not contain LF line breaks; only CRLF should be used");
        pdfText.Replace("\r\n", "CRLF").ShouldNotContain('\r', "PDF should not contain CR line breaks; only CRLF should be used");

        // Find %%EOF at the end, then work backward to find startxref
        int eofIndex = pdfText.LastIndexOf("%%EOF", StringComparison.Ordinal);
        eofIndex.ShouldBeGreaterThan(0, "%%EOF not found");

        int startxrefIndex = pdfText.LastIndexOf("startxref\r\n", eofIndex, StringComparison.Ordinal);
        startxrefIndex.ShouldBeGreaterThan(0, "startxref not found");

        // Read the xref byte offset (the number on the line after "startxref")
        int afterStartxref = startxrefIndex + "startxref\r\n".Length;
        int endOfOffset = pdfText.IndexOf("\r\n", afterStartxref, StringComparison.Ordinal);
        string xrefOffsetStr = pdfText.Substring(afterStartxref, endOfOffset - afterStartxref);
        int xrefOffset = int.Parse(xrefOffsetStr, NumberStyles.None, CultureInfo.InvariantCulture);
        xrefOffset.ShouldBeGreaterThan(0, "xref byte offset should be positive");

        // Seek to xref table and parse it
        using var stream = new MemoryStream(pdfBytes);
        stream.Position = xrefOffset;
        var reader = new StreamReader(stream, Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

        // First line must be "xref"
        string? xrefLine = reader.ReadLine();
        xrefLine.ShouldBe("xref", "xref keyword not found at expected offset");

        // Parse subsections: "firstObjNum count"
        var objectOffsets = new Dictionary<int, long>();
        string? subsectionLine;
        while ((subsectionLine = reader.ReadLine()) != null && subsectionLine != "trailer")
        {
            string[] parts = subsectionLine.Split(' ');
            parts.Length.ShouldBe(2, $"Expected 'firstObj count' but got: {subsectionLine}");
            int firstObj = int.Parse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture);
            firstObj.ShouldBe(0);
            int count = int.Parse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture);

            for (int i = 0; i < count; i++)
            {
                // Each entry: "NNNNNNNNNN GGGGG f\r\n" or "NNNNNNNNNN GGGGG n\r\n"
                string? entry = reader.ReadLine();
                entry.ShouldNotBeNull();
                entry.Length.ShouldBe(18);
                string[] entryParts = entry.Split(' ');
                entryParts.Length.ShouldBe(3, $"Expected 'offset gen type' but got: {entry}");
                long offset = long.Parse(entryParts[0], NumberStyles.None, CultureInfo.InvariantCulture);
                int generation = int.Parse(entryParts[1], NumberStyles.None, CultureInfo.InvariantCulture);
                string type = entryParts[2];
                type.ShouldBeOneOf("n", "f");

                if (type == "n")
                {
                    generation.ShouldBe(0, $"Expected generation 0 for in-use object but got {generation}");
                    objectOffsets[i] = offset;
                }
                else
                {
                    // Free objects should only be listed for the first object in the subsection
                    i.ShouldBe(0);
                    offset.ShouldBe(0);
                    generation.ShouldBe(65535, $"Expected generation 65535 for free object but got {generation}");
                }
            }
        }

        objectOffsets.Count.ShouldBeGreaterThan(0, "No in-use objects found in xref table");

        // Verify each object: seek to its offset and confirm "N 0 obj" is present
        foreach (var kvp in objectOffsets)
        {
            stream.Position = kvp.Value;
            int objNum = kvp.Key;
            long offset = kvp.Value;
            var objReader = new StreamReader(stream, Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            string? objLine = objReader.ReadLine();
            objLine.ShouldNotBeNull($"No content at offset {offset} for object {objNum}");
            objLine.ShouldBe($"{objNum} 0 obj", $"Object {objNum} at offset {offset} did not start with '{objNum} 0 obj'");
        }
    }
}

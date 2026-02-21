using System.IO;

namespace QRCoderTests;

public class PdfByteQRCodeRendererTests
{
    [Fact]
    public void can_render_pdfbyte_qrcode_blackwhite()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        var pdfCodeGfx = new PdfByteQRCode(data).GetGraphic(5);
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    [Fact]
    public void can_render_pdfbyte_qrcode_color()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        var pdfCodeGfx = new PdfByteQRCode(data).GetGraphic(5, "#FF0000", "#0000FF");
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    [Fact]
    public void can_render_pdfbyte_qrcode_custom_dpi()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        var pdfCodeGfx = new PdfByteQRCode(data).GetGraphic(5, "#000000", "#FFFFFF", 300);
        pdfCodeGfx.ShouldMatchApproved("pdf");
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
        var pdfCodeGfx = PdfByteQRCodeHelper.GetQRCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L, 10);
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    [Fact]
    public void can_render_pdfbyte_qrcode_from_helper_2()
    {
        var pdfCodeGfx = PdfByteQRCodeHelper.GetQRCode("This is a quick test! 123#?", 5, "#FF0000", "#0000FF", QRCodeGenerator.ECCLevel.L);
        pdfCodeGfx.ShouldMatchApproved("pdf");
    }

    private static readonly char[] _lineEndChars = { '\r', '\n' };

    [Fact]
    public void pdf_xref_table_is_valid()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.L);
        var pdfBytes = new PdfByteQRCode(data).GetGraphic(5);

        // Parse from the end to find startxref
        var pdfText = Encoding.ASCII.GetString(pdfBytes);

        // Find %%EOF at the end, then work backward to find startxref
        var eofIndex = pdfText.LastIndexOf("%%EOF", StringComparison.Ordinal);
        eofIndex.ShouldBeGreaterThan(0, "%%EOF not found");

        var startxrefIndex = pdfText.LastIndexOf("startxref\r\n", eofIndex, StringComparison.Ordinal);
        startxrefIndex.ShouldBeGreaterThan(0, "startxref not found");

        // Read the xref byte offset (the number on the line after "startxref")
        var afterStartxref = startxrefIndex + "startxref\r\n".Length;
        var endOfOffset = pdfText.IndexOf("\r\n", afterStartxref, StringComparison.Ordinal);
        var xrefOffsetStr = pdfText.Substring(afterStartxref, endOfOffset - afterStartxref);
        var xrefOffset = int.Parse(xrefOffsetStr, NumberStyles.None, CultureInfo.InvariantCulture);
        xrefOffset.ShouldBeGreaterThan(0, "xref byte offset should be positive");

        // Seek to xref table and parse it
        using var stream = new MemoryStream(pdfBytes);
        stream.Position = xrefOffset;
        var reader = new StreamReader(stream, Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

        // First line must be "xref"
        var xrefLine = reader.ReadLine();
        xrefLine.ShouldBe("xref", "xref keyword not found at expected offset");

        // Parse subsections: "firstObjNum count"
        var objectOffsets = new Dictionary<int, long>();
        string? subsectionLine;
        while ((subsectionLine = reader.ReadLine()) != null && subsectionLine != "trailer")
        {
            var parts = subsectionLine.Split(' ');
            parts.Length.ShouldBe(2, $"Expected 'firstObj count' but got: {subsectionLine}");
            var firstObj = int.Parse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture);
            firstObj.ShouldBe(0);
            var count = int.Parse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture);

            for (int i = 0; i < count; i++)
            {
                // Each entry: "NNNNNNNNNN GGGGG f\r\n" or "NNNNNNNNNN GGGGG n\r\n"
                var entry = reader.ReadLine();
                entry.ShouldNotBeNull();
                entry.Length.ShouldBe(18);
                var entryParts = entry.Split(' ');
                entryParts.Length.ShouldBe(3, $"Expected 'offset gen type' but got: {entry}");
                var offset = long.Parse(entryParts[0], NumberStyles.None, CultureInfo.InvariantCulture);
                var generation = int.Parse(entryParts[1], NumberStyles.None, CultureInfo.InvariantCulture);
                var type = entryParts[2];
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
            var objNum = kvp.Key;
            var offset = kvp.Value;
            var objReader = new StreamReader(stream, Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            var objLine = objReader.ReadLine();
            objLine.ShouldNotBeNull($"No content at offset {offset} for object {objNum}");
            objLine.ShouldBe($"{objNum} 0 obj", $"Object {objNum} at offset {offset} did not start with '{objNum} 0 obj'");
        }
    }
}

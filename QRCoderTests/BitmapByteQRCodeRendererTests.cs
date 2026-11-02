namespace QRCoderTests;

public class BitmapByteQRCodeRendererTests
{
    [Fact]
    public void can_render_bitmapbyte_qrcode()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
        var bmp = new BitmapByteQRCode(data).GetGraphic(10);
        bmp.ShouldMatchApprovedImage();
    }


    [Fact]
    public void can_render_bitmapbyte_qrcode_color_bytearray()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
        var bmp = new BitmapByteQRCode(data).GetGraphic(10, new byte[] { 30, 30, 30 }, new byte[] { 255, 0, 0 });
        bmp.ShouldMatchApprovedImage();
    }

    [Theory]
    [InlineData(1, "#123456", "#E7F0FA")]
    [InlineData(3, "#123456", "#E7F0FA")]
    [InlineData(1, "123456", "E7F0FA")]
    [InlineData(3, "123456", "E7F0FA")]
    [InlineData(1, "#123456", "#e7F0fA")]
    [InlineData(3, "#123456", "#e7F0fA")]
    [InlineData(1, "#123456Z", "#E7F0FAZ")]
    [InlineData(3, "#123456Z", "#E7F0FAZ")]
    [InlineData(1, "#12345678", "#E7F0FA90")]
    [InlineData(3, "#12345678", "#E7F0FA90")]
    public void equivalent_hex_and_rgb_colors_render_identical_bitmap_bytes(int pixelsPerModule, string darkColor, string lightColor)
    {
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode("Equivalent bitmap colors", QRCodeGenerator.ECCLevel.L);
        using var bitmapCode = new BitmapByteQRCode(data);

        var expected = bitmapCode.GetGraphic(pixelsPerModule, new byte[] { 0x12, 0x34, 0x56 }, new byte[] { 0xE7, 0xF0, 0xFA });

        bitmapCode.GetGraphic(pixelsPerModule, darkColor, lightColor).ShouldBe(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void default_colors_render_identical_bitmap_bytes_to_explicit_rgb_colors(int pixelsPerModule)
    {
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode("Default bitmap colors", QRCodeGenerator.ECCLevel.L);
        using var bitmapCode = new BitmapByteQRCode(data);

        var expected = bitmapCode.GetGraphic(pixelsPerModule, new byte[] { 0, 0, 0 }, new byte[] { 255, 255, 255 });

        bitmapCode.GetGraphic(pixelsPerModule).ShouldBe(expected);
    }

    [Theory]
    [InlineData("#123456GG", "#E7F0FA")]
    [InlineData("#123456", "#E7F0FAGG")]
    public void rejects_invalid_hex_digits_in_extra_color_bytes(string darkColor, string lightColor)
    {
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode("Invalid extra bitmap color bytes", QRCodeGenerator.ECCLevel.L);
        using var bitmapCode = new BitmapByteQRCode(data);

        Should.Throw<FormatException>(() => bitmapCode.GetGraphic(1, darkColor, lightColor));
    }

    [Fact]
    public void can_render_bitmapbyte_qrcode_drawing_color()
    {
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
        var bmp = new BitmapByteQRCode(data).GetGraphic(10, "#e3e3e3", "#ffffff");
        bmp.ShouldMatchApprovedImage();
    }
}

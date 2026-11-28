using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ECCLevel = QRCoder.QRCodeGenerator.ECCLevel;

namespace QRCoderTests;

public class ReedSolomonDecoderTests
{
    [Theory]
    [InlineData(1200, 39)]
    [InlineData(1250, 40)]
    public void zero_filled_payload_matches_reference_symbol(int byteCount, int version)
    {
        var payload = new byte[byteCount];
        using var qr = QRCodeGenerator.GenerateQrCode(payload, ECCLevel.H);
        qr.Version.ShouldBe(version);

        var matrix = GetSymbol(qr);
        var decoded = AssertDecodes((BitMatrix)matrix.Clone(), payload, 0);
        // Mask selection can differ between encoders. Use the symbol's mask to compare every module.
        var hints = new Dictionary<EncodeHintType, object>
        {
            [EncodeHintType.QR_MASK_PATTERN] = ((QRCodeDecoderMetaData)decoded.Other).DataMask
        };
        var reference = ZXing.QrCode.Internal.Encoder.encode(new string('\0', byteCount), ErrorCorrectionLevel.H, hints).Matrix;
        matrix.Width.ShouldBe(reference.Width);
        for (int y = 0; y < matrix.Height; y++)
        {
            for (int x = 0; x < matrix.Width; x++)
                Assert.Equal(reference[x, y] == 1, matrix[x, y]);
        }
    }

    [Fact]
    public void zero_filled_payload_recovers_two_damaged_data_codewords()
    {
        var payload = new byte[1250];
        using var qr = QRCodeGenerator.GenerateQrCode(payload, ECCLevel.H);
        qr.Version.ShouldBe(40);

        var matrix = GetSymbol(qr);
        // The first two data codewords of block 1 are 81 codewords apart after interleaving.
        matrix.flip(176, 176);
        matrix.flip(174, 165);
        AssertDecodes(matrix, payload, 2);
    }

    [Fact]
    public void nonzero_data_after_long_zero_prefix_decodes_without_corrections()
    {
        var payload = new byte[120];
        for (int i = 88; i < payload.Length; i++)
            payload[i] = (byte)'A';

        using var qr = QRCodeGenerator.GenerateQrCode(payload, ECCLevel.L);
        qr.Version.ShouldBe(6);
        AssertDecodes(GetSymbol(qr), payload, 0);
    }

    private static BitMatrix GetSymbol(QRCodeData qr)
    {
        const int QuietZone = 4;
        int size = qr.ModuleMatrix.Count - 2 * QuietZone;
        var matrix = new BitMatrix(size);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
                matrix[x, y] = qr.ModuleMatrix[y + QuietZone][x + QuietZone];
        }
        return matrix;
    }

    private static DecoderResult AssertDecodes(BitMatrix matrix, byte[] payload, int errorsCorrected)
    {
        var result = new ZXing.QrCode.Internal.Decoder().decode(matrix, null);
        result.ShouldNotBeNull();
        result.ByteSegments.ShouldHaveSingleItem().ShouldBe(payload);
        result.ErrorsCorrected.ShouldBe(errorsCorrected);
        return result;
    }
}

namespace QRCoderTests.PayloadGeneratorTests;

public class IbanTests
{
    [Fact]
    public void iban_validator_validate_german_iban()
    {
        string iban = "DE15268500010154131577";

        bool result = PayloadGenerator.IsValidIban(iban);

        result.ShouldBe<bool>(true);
    }

    [Fact]
    public void iban_validator_validate_swiss_iban()
    {
        string iban = "CH1900767000U00121977";

        bool result = PayloadGenerator.IsValidIban(iban);

        result.ShouldBe<bool>(true);
    }

    [Fact]
    public void iban_validator_invalidates_iban()
    {
        string iban = "DE29268500010154131577";

        bool result = PayloadGenerator.IsValidIban(iban);

        result.ShouldBe<bool>(false);
    }

    [Fact]
    public void qriban_validator_validates_iban()
    {
        string iban = "CH2430043000000789012";

        bool result = PayloadGenerator.IsValidQRIban(iban);

        result.ShouldBe<bool>(true);
    }

    [Fact]
    public void qriban_validator_invalidates_iban()
    {
        string iban = "CH3908704016075473007";

        bool result = PayloadGenerator.IsValidQRIban(iban);

        result.ShouldBe<bool>(false);
    }
}

using static QRCoder.QRCodeGenerator;

namespace QRCoderTests.PayloadGeneratorTests;

public class GirocodeTests
{
    [Fact]
    public void girocode_generator_can_generate_payload_minimal()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00\n\n\n\n");
    }

    [Fact]
    public void girocode_generator_can_generate_payload_full()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser,
            PayloadGenerator.Girocode.GirocodeVersion.Version1,
            PayloadGenerator.Girocode.GirocodeEncoding.ISO_8859_1);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }

    [Fact]
    public void girocode_generator_should_handle_version()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser,
            PayloadGenerator.Girocode.GirocodeVersion.Version2,
            PayloadGenerator.Girocode.GirocodeEncoding.ISO_8859_1);

        generator
            .ToString()
            .ShouldBe("BCD\n002\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }


    [Fact]
    public void girocode_generator_should_handle_iban_whitespaces()
    {
        var iban = "DE33 1002 0500 0001 1947 00";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }

    [Fact]
    public void girocode_generator_should_handle_bic_whitespaces()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSW DE 33 BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }


    [Fact]
    public void girocode_generator_should_fill_amount_decimals()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSW DE 33 BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 12m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR12.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }


    [Fact]
    public void girocode_generator_should_throw_iban_exception()
    {
        var iban = "33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("The IBAN entered isn't valid.");
    }


    [Fact]
    public void girocode_generator_should_throw_bic_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "DWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("The BIC entered isn't valid.");
    }


    [Fact]
    public void girocode_generator_should_throw_name_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "A company with a name which is exactly 71 chars - and for that to long.";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("(Payee-)Name must be shorter than 71 chars.");
    }


    [Fact]
    public void girocode_generator_should_throw_amount_decimals_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.521m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Amount must have less than 3 digits after decimal point.");
    }

    [Fact]
    public void girocode_generator_should_throw_amount_min_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 0.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Amount has to be at least 0.01 and must be smaller or equal to 999999999.99.");
    }



    [Fact]
    public void girocode_generator_should_throw_amount_max_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 1999999999.99m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Amount has to be at least 0.01 and must be smaller or equal to 999999999.99.");
    }


    [Fact]
    public void girocode_generator_should_throw_purpose_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "12345";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Purpose of credit transfer can only have 4 chars at maximum.");
    }

    [Fact]
    public void girocode_generator_should_throw_remittance_unstructured_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "An unstructured remittance information which is longer than a tweet. This means that this unstructures remittance info has more than 140 chars.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Unstructured reference texts have to be shorter than 141 chars.");
    }

    [Fact]
    public void girocode_generator_should_throw_remittance_structured_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Structured remittance infos have to be shorter than 36 chars.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "Thanks for using Girocode";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Structured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Structured reference texts have to be shorter than 36 chars.");
    }

    [Fact]
    public void girocode_generator_should_throw_usermessage_exception()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;
        var remittanceInformation = "Donation to Wikipedia.";
        var purposeOfCreditTransfer = "1234";
        var messageToGirocodeUser = "The usermessage is shown to the user which scans the Girocode. It has to be shorter than 71 chars.";


        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Message to the Girocode-User reader texts have to be shorter than 71 chars.");
    }

    [Fact]
    public void girocode_generator_sets_encoding_parameters()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Wikimedia Fördergesellschaft";
        var amount = 10.00m;

        var payload = new PayloadGenerator.Girocode(iban, bic, name, amount);

        payload.EccLevel.ShouldBe<ECCLevel>(ECCLevel.M);
        payload.EciMode.ShouldBe<EciMode>(EciMode.Default);
        payload.Version.ShouldBe(-1);
    }

    [Fact]
    public void girocode_generator_version2_with_null_bic_should_succeed()
    {
        var iban = "NL86INGB0002445588";
        var name = "a name";
        var remittanceInformation = "some info";
        var amount = 1337.99m;

        var payload = new PayloadGenerator.Girocode(
            iban: iban,
            bic: null,
            name: name,
            amount: amount,
            remittanceInformation: remittanceInformation,
            version: PayloadGenerator.Girocode.GirocodeVersion.Version2,
            encoding: PayloadGenerator.Girocode.GirocodeEncoding.UTF_8);

        payload
            .ToString()
            .ShouldBe("BCD\n002\n1\nSCT\n\na name\nNL86INGB0002445588\nEUR1337.99\n\n\nsome info\n");
    }

    [Fact]
    public void girocode_generator_version2_with_empty_bic_should_succeed()
    {
        var iban = "NL86INGB0002445588";
        var name = "a name";
        var remittanceInformation = "some info";
        var amount = 1337.99m;

        var payload = new PayloadGenerator.Girocode(
            iban: iban,
            bic: string.Empty,
            name: name,
            amount: amount,
            remittanceInformation: remittanceInformation,
            version: PayloadGenerator.Girocode.GirocodeVersion.Version2,
            encoding: PayloadGenerator.Girocode.GirocodeEncoding.UTF_8);

        payload
            .ToString()
            .ShouldBe("BCD\n002\n1\nSCT\n\na name\nNL86INGB0002445588\nEUR1337.99\n\n\nsome info\n");
    }

    [Fact]
    public void girocode_generator_version2_with_valid_bic_should_succeed()
    {
        var iban = "NL86INGB0002445588";
        var bic = "INGBNL2A";
        var name = "a name";
        var remittanceInformation = "some info";
        var amount = 1337.99m;

        var payload = new PayloadGenerator.Girocode(
            iban: iban,
            bic: bic,
            name: name,
            amount: amount,
            remittanceInformation: remittanceInformation,
            version: PayloadGenerator.Girocode.GirocodeVersion.Version2,
            encoding: PayloadGenerator.Girocode.GirocodeEncoding.UTF_8);

        payload
            .ToString()
            .ShouldBe("BCD\n002\n1\nSCT\nINGBNL2A\na name\nNL86INGB0002445588\nEUR1337.99\n\n\nsome info\n");
    }

    [Fact]
    public void girocode_generator_version2_with_invalid_bic_should_throw_exception()
    {
        var iban = "NL86INGB0002445588";
        var bic = "INVALID";
        var name = "a name";
        var remittanceInformation = "some info";
        var amount = 1337.99m;

        var exception = Record.Exception(() => new PayloadGenerator.Girocode(
            iban: iban,
            bic: bic,
            name: name,
            amount: amount,
            remittanceInformation: remittanceInformation,
            version: PayloadGenerator.Girocode.GirocodeVersion.Version2,
            encoding: PayloadGenerator.Girocode.GirocodeEncoding.UTF_8));

        Assert.NotNull(exception);
        Assert.IsType<PayloadGenerator.Girocode.GirocodeException>(exception);
        exception.Message.ShouldBe("The BIC entered isn't valid.");
    }

    [Fact]
    public void girocode_generator_version1_with_null_bic_should_throw_exception()
    {
        var iban = "NL86INGB0002445588";
        var name = "a name";
        var remittanceInformation = "some info";
        var amount = 1337.99m;

        var exception = Record.Exception(() => new PayloadGenerator.Girocode(
            iban: iban,
            bic: null,
            name: name,
            amount: amount,
            remittanceInformation: remittanceInformation,
            version: PayloadGenerator.Girocode.GirocodeVersion.Version1,
            encoding: PayloadGenerator.Girocode.GirocodeEncoding.UTF_8));

        Assert.NotNull(exception);
        Assert.IsType<PayloadGenerator.Girocode.GirocodeException>(exception);
        exception.Message.ShouldBe("The BIC entered isn't valid.");
    }

    [Fact]
    public void girocode_generator_version1_with_empty_bic_should_throw_exception()
    {
        var iban = "NL86INGB0002445588";
        var name = "a name";
        var remittanceInformation = "some info";
        var amount = 1337.99m;

        var exception = Record.Exception(() => new PayloadGenerator.Girocode(
            iban: iban,
            bic: string.Empty,
            name: name,
            amount: amount,
            remittanceInformation: remittanceInformation,
            version: PayloadGenerator.Girocode.GirocodeVersion.Version1,
            encoding: PayloadGenerator.Girocode.GirocodeEncoding.UTF_8));

        Assert.NotNull(exception);
        Assert.IsType<PayloadGenerator.Girocode.GirocodeException>(exception);
        exception.Message.ShouldBe("The BIC entered isn't valid.");
    }

    [Fact]
    public void girocode_generator_can_generate_payload_without_amount()
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Payee";

        var generator = new PayloadGenerator.Girocode(iban, bic, name);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nPayee\nDE33100205000001194700\n\n\n\n\n");
    }

    [Theory]
    [InlineData(false, PayloadGenerator.Girocode.GirocodeVersion.Version1, PayloadGenerator.Girocode.TypeOfRemittance.Structured)]
    [InlineData(false, PayloadGenerator.Girocode.GirocodeVersion.Version1, PayloadGenerator.Girocode.TypeOfRemittance.Unstructured)]
    [InlineData(false, PayloadGenerator.Girocode.GirocodeVersion.Version2, PayloadGenerator.Girocode.TypeOfRemittance.Structured)]
    [InlineData(false, PayloadGenerator.Girocode.GirocodeVersion.Version2, PayloadGenerator.Girocode.TypeOfRemittance.Unstructured)]
    [InlineData(true, PayloadGenerator.Girocode.GirocodeVersion.Version1, PayloadGenerator.Girocode.TypeOfRemittance.Structured)]
    [InlineData(true, PayloadGenerator.Girocode.GirocodeVersion.Version1, PayloadGenerator.Girocode.TypeOfRemittance.Unstructured)]
    [InlineData(true, PayloadGenerator.Girocode.GirocodeVersion.Version2, PayloadGenerator.Girocode.TypeOfRemittance.Structured)]
    [InlineData(true, PayloadGenerator.Girocode.GirocodeVersion.Version2, PayloadGenerator.Girocode.TypeOfRemittance.Unstructured)]
    public void girocode_generator_should_preserve_field_positions_without_amount(
        bool omitAmount,
        PayloadGenerator.Girocode.GirocodeVersion version,
        PayloadGenerator.Girocode.TypeOfRemittance typeOfRemittance)
    {
        var iban = "DE33100205000001194700";
        var bic = version == PayloadGenerator.Girocode.GirocodeVersion.Version1 ? "BFSWDE33BER" : null;
        var name = "Payee";
        var remittanceInformation = "Reference";
        var purposeOfCreditTransfer = "CHAR";
        var messageToGirocodeUser = "Thank you";

        var generator = omitAmount
            ? new PayloadGenerator.Girocode(iban, bic, name, remittanceInformation: remittanceInformation,
                typeOfRemittance: typeOfRemittance, purposeOfCreditTransfer: purposeOfCreditTransfer,
                messageToGirocodeUser: messageToGirocodeUser, version: version)
            : new PayloadGenerator.Girocode(iban, bic, name, amount: null, remittanceInformation: remittanceInformation,
                typeOfRemittance: typeOfRemittance, purposeOfCreditTransfer: purposeOfCreditTransfer,
                messageToGirocodeUser: messageToGirocodeUser, version: version);

        generator
            .ToString()
            .Split('\n')
            .ShouldBe(new[]
            {
                "BCD",
                version == PayloadGenerator.Girocode.GirocodeVersion.Version1 ? "001" : "002",
                "2",
                "SCT",
                bic ?? string.Empty,
                "Payee",
                iban,
                string.Empty,
                "CHAR",
                typeOfRemittance == PayloadGenerator.Girocode.TypeOfRemittance.Structured ? "Reference" : string.Empty,
                typeOfRemittance == PayloadGenerator.Girocode.TypeOfRemittance.Unstructured ? "Reference" : string.Empty,
                "Thank you",
            });
    }

    [Theory]
    [InlineData("0.01", "EUR0.01")]
    [InlineData("999999999.99", "EUR999999999.99")]
    [InlineData("12", "EUR12.00")]
    [InlineData("12.3400", "EUR12.34")]
    public void girocode_generator_should_format_specified_amount(string amountText, string expectedAmount)
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Payee";
        var amount = decimal.Parse(amountText, CultureInfo.InvariantCulture);

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount);
        var nullableGenerator = new PayloadGenerator.Girocode(iban, bic, name, (decimal?)amount);

        generator
            .ToString()
            .ShouldBe($"BCD\n001\n2\nSCT\nBFSWDE33BER\nPayee\nDE33100205000001194700\n{expectedAmount}\n\n\n\n");
        nullableGenerator
            .ToString()
            .ShouldBe(generator.ToString());
    }

    [Theory]
    [InlineData("0", "Amount has to be at least 0.01 and must be smaller or equal to 999999999.99.")]
    [InlineData("-0.01", "Amount has to be at least 0.01 and must be smaller or equal to 999999999.99.")]
    [InlineData("1000000000", "Amount has to be at least 0.01 and must be smaller or equal to 999999999.99.")]
    [InlineData("0.001", "Amount must have less than 3 digits after decimal point.")]
    [InlineData("10.521", "Amount must have less than 3 digits after decimal point.")]
    public void girocode_generator_should_throw_invalid_amount_exception(string amountText, string expectedMessage)
    {
        var iban = "DE33100205000001194700";
        var bic = "BFSWDE33BER";
        var name = "Payee";
        var amount = decimal.Parse(amountText, CultureInfo.InvariantCulture);

        var exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount));
        var nullableException = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, (decimal?)amount));

        exception.Message.ShouldBe(expectedMessage);
        nullableException.Message.ShouldBe(expectedMessage);
    }
}

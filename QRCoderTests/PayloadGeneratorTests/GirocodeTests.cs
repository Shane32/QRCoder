using static QRCoder.QRCodeGenerator;

namespace QRCoderTests.PayloadGeneratorTests;

public class GirocodeTests
{
    [Fact]
    public void girocode_generator_can_generate_payload_minimal()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00");
    }

    [Fact]
    public void girocode_generator_can_generate_payload_full()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";

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
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";

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
        string iban = "DE33 1002 0500 0001 1947 00";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }

    [Fact]
    public void girocode_generator_should_handle_bic_whitespaces()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSW DE 33 BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR10.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }


    [Fact]
    public void girocode_generator_should_fill_amount_decimals()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSW DE 33 BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 12m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nWikimedia Fördergesellschaft\nDE33100205000001194700\nEUR12.00\n1234\n\nDonation to Wikipedia.\nThanks for using Girocode");
    }


    [Fact]
    public void girocode_generator_should_throw_iban_exception()
    {
        string iban = "33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("The IBAN entered isn't valid.");
    }


    [Fact]
    public void girocode_generator_should_throw_bic_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "DWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("The BIC entered isn't valid.");
    }


    [Fact]
    public void girocode_generator_should_throw_name_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "A company with a name which is exactly 71 chars - and for that to long.";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("(Payee-)Name must be shorter than 71 chars.");
    }


    [Fact]
    public void girocode_generator_should_throw_amount_decimals_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.521m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Amount must have less than 3 digits after decimal point.");
    }

    [Fact]
    public void girocode_generator_should_throw_amount_min_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 0.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Amount has to be at least 0.01 and must be smaller or equal to 999999999.99.");
    }



    [Fact]
    public void girocode_generator_should_throw_amount_max_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 1999999999.99m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Amount has to be at least 0.01 and must be smaller or equal to 999999999.99.");
    }


    [Fact]
    public void girocode_generator_should_throw_purpose_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "12345";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Purpose of credit transfer can only have 4 chars at maximum.");
    }

    [Fact]
    public void girocode_generator_should_throw_remittance_unstructured_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "An unstructured remittance information which is longer than a tweet. This means that this unstructures remittance info has more than 140 chars.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Unstructured reference texts have to be shorter than 141 chars.");
    }

    [Fact]
    public void girocode_generator_should_throw_remittance_structured_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Structured remittance infos have to be shorter than 36 chars.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "Thanks for using Girocode";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Structured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Structured reference texts have to be shorter than 36 chars.");
    }

    [Fact]
    public void girocode_generator_should_throw_usermessage_exception()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;
        string remittanceInformation = "Donation to Wikipedia.";
        string purposeOfCreditTransfer = "1234";
        string messageToGirocodeUser = "The usermessage is shown to the user which scans the Girocode. It has to be shorter than 71 chars.";


        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount, remittanceInformation,
            PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, purposeOfCreditTransfer, messageToGirocodeUser));

        exception.Message.ShouldBe("Message to the Girocode-User reader texts have to be shorter than 71 chars.");
    }

    [Fact]
    public void girocode_generator_sets_encoding_parameters()
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Wikimedia Fördergesellschaft";
        decimal amount = 10.00m;

        var payload = new PayloadGenerator.Girocode(iban, bic, name, amount);

        payload.EccLevel.ShouldBe<ECCLevel>(ECCLevel.M);
        payload.EciMode.ShouldBe<EciMode>(EciMode.Default);
        payload.Version.ShouldBe(-1);
    }

    [Fact]
    public void girocode_generator_version2_with_null_bic_should_succeed()
    {
        string iban = "NL86INGB0002445588";
        string name = "a name";
        string remittanceInformation = "some info";
        decimal amount = 1337.99m;

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
            .ShouldBe("BCD\n002\n1\nSCT\n\na name\nNL86INGB0002445588\nEUR1337.99\n\n\nsome info");
    }

    [Fact]
    public void girocode_generator_version2_with_empty_bic_should_succeed()
    {
        string iban = "NL86INGB0002445588";
        string name = "a name";
        string remittanceInformation = "some info";
        decimal amount = 1337.99m;

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
            .ShouldBe("BCD\n002\n1\nSCT\n\na name\nNL86INGB0002445588\nEUR1337.99\n\n\nsome info");
    }

    [Fact]
    public void girocode_generator_version2_with_valid_bic_should_succeed()
    {
        string iban = "NL86INGB0002445588";
        string bic = "INGBNL2A";
        string name = "a name";
        string remittanceInformation = "some info";
        decimal amount = 1337.99m;

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
            .ShouldBe("BCD\n002\n1\nSCT\nINGBNL2A\na name\nNL86INGB0002445588\nEUR1337.99\n\n\nsome info");
    }

    [Fact]
    public void girocode_generator_version2_with_invalid_bic_should_throw_exception()
    {
        string iban = "NL86INGB0002445588";
        string bic = "INVALID";
        string name = "a name";
        string remittanceInformation = "some info";
        decimal amount = 1337.99m;

        Exception exception = Record.Exception(() => new PayloadGenerator.Girocode(
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
        string iban = "NL86INGB0002445588";
        string name = "a name";
        string remittanceInformation = "some info";
        decimal amount = 1337.99m;

        Exception exception = Record.Exception(() => new PayloadGenerator.Girocode(
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
        string iban = "NL86INGB0002445588";
        string name = "a name";
        string remittanceInformation = "some info";
        decimal amount = 1337.99m;

        Exception exception = Record.Exception(() => new PayloadGenerator.Girocode(
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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void girocode_generator_can_generate_payload_without_amount(bool omitAmount)
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Payee";

        PayloadGenerator.Girocode generator = omitAmount
            ? new PayloadGenerator.Girocode(iban, bic, name)
            : new PayloadGenerator.Girocode(iban, bic, name, amount: null);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nPayee\nDE33100205000001194700");
    }

    [Theory]
    [InlineData(false, "CHAR", "", PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, "", "\n\nCHAR")]
    [InlineData(false, "", "Reference", PayloadGenerator.Girocode.TypeOfRemittance.Structured, "", "\n\n\nReference")]
    [InlineData(false, "", "Reference", PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, "", "\n\n\n\nReference")]
    [InlineData(false, "", "", PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, "Thank you ", "\n\n\n\n\nThank you ")]
    [InlineData(true, "CHAR", "", PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, "", "\nEUR10.00\nCHAR")]
    [InlineData(true, "", "Reference", PayloadGenerator.Girocode.TypeOfRemittance.Structured, "", "\nEUR10.00\n\nReference")]
    [InlineData(true, "", "Reference", PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, "", "\nEUR10.00\n\n\nReference")]
    [InlineData(true, "", "", PayloadGenerator.Girocode.TypeOfRemittance.Unstructured, "Thank you ", "\nEUR10.00\n\n\n\nThank you ")]
    public void girocode_generator_should_end_payload_at_last_populated_field(
        bool hasAmount,
        string purposeOfCreditTransfer,
        string remittanceInformation,
        PayloadGenerator.Girocode.TypeOfRemittance typeOfRemittance,
        string messageToGirocodeUser,
        string expectedSuffix)
    {
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Payee";

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount: hasAmount ? 10m : (decimal?)null,
            remittanceInformation: remittanceInformation, typeOfRemittance: typeOfRemittance,
            purposeOfCreditTransfer: purposeOfCreditTransfer, messageToGirocodeUser: messageToGirocodeUser);

        generator
            .ToString()
            .ShouldBe("BCD\n001\n2\nSCT\nBFSWDE33BER\nPayee\nDE33100205000001194700" + expectedSuffix);
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
        string iban = "DE33100205000001194700";
        string? bic = version == PayloadGenerator.Girocode.GirocodeVersion.Version1 ? "BFSWDE33BER" : null;
        string name = "Payee";
        string remittanceInformation = "Reference";
        string purposeOfCreditTransfer = "CHAR";
        string messageToGirocodeUser = "Thank you";

        PayloadGenerator.Girocode generator = omitAmount
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
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Payee";
        decimal amount = decimal.Parse(amountText, CultureInfo.InvariantCulture);

        var generator = new PayloadGenerator.Girocode(iban, bic, name, amount);
        var nullableGenerator = new PayloadGenerator.Girocode(iban, bic, name, (decimal?)amount);

        generator
            .ToString()
            .ShouldBe($"BCD\n001\n2\nSCT\nBFSWDE33BER\nPayee\nDE33100205000001194700\n{expectedAmount}");
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
        string iban = "DE33100205000001194700";
        string bic = "BFSWDE33BER";
        string name = "Payee";
        decimal amount = decimal.Parse(amountText, CultureInfo.InvariantCulture);

        PayloadGenerator.Girocode.GirocodeException exception = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, amount));
        PayloadGenerator.Girocode.GirocodeException nullableException = Should.Throw<PayloadGenerator.Girocode.GirocodeException>(() => new PayloadGenerator.Girocode(iban, bic, name, (decimal?)amount));

        exception.Message.ShouldBe(expectedMessage);
        nullableException.Message.ShouldBe(expectedMessage);
    }
}

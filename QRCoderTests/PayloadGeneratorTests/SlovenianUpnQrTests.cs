namespace QRCoderTests.PayloadGeneratorTests;

public class SlovenianUpnQrTests
{
    [Fact]
    public void slovenian_upnqr_generator_should_generate_payload_simple()
    {
        string payerName = "Janez Novak";
        string payerAddress = "Slovenska cesta 1";
        string payerPlace = "1000 Ljubljana";
        string recipientName = "Podjetje d.o.o.";
        string recipientAddress = "Trzaska cesta 10";
        string recipientPlace = "2000 Maribor";
        string recipientIban = "SI56011006001234567";
        string description = "Payment for services";
        double amount = 150.50;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        generator
            .ToString()
            .ShouldBe("UPNQR\n\n\n\n\nJanez Novak\nSlovenska cesta 1\n1000 Ljubljana\n00000015050\n\n\nOTHR\nPayment for services\n\nSI56011006001234567\nSI00\nPodjetje d.o.o.\nTrzaska cesta 10\n2000 Maribor\n167\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_generate_payload_with_deadline()
    {
        string payerName = "Marko Kovac";
        string payerAddress = "Celovska cesta 20";
        string payerPlace = "1000 Ljubljana";
        string recipientName = "Test Company";
        string recipientAddress = "Main Street 5";
        string recipientPlace = "3000 Celje";
        string recipientIban = "SI56020170014356205";
        string description = "Invoice payment";
        double amount = 99.99;
        var deadline = new DateTime(2024, 12, 31);

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount, deadline);

        string payload = generator.ToString();

        payload.ShouldContain("31.12.2024\n");
        payload.ShouldContain("00000009999\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_generate_payload_with_custom_code()
    {
        string payerName = "Ana Horvat";
        string payerAddress = "Dunajska cesta 100";
        string payerPlace = "1000 Ljubljana";
        string recipientName = "Service Provider";
        string recipientAddress = "Business Park 1";
        string recipientPlace = "4000 Kranj";
        string recipientIban = "SI56031234567890123";
        string description = "Monthly subscription";
        double amount = 25.00;
        string code = "GDSV";

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount,
            recipientSiModel: "SI00", recipientSiReference: "", code: code);

        string payload = generator.ToString();

        payload.ShouldContain("GDSV\n");
        payload.ShouldContain("00000002500\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_generate_payload_with_reference()
    {
        string payerName = "Peter Zupan";
        string payerAddress = "Smartinska cesta 50";
        string payerPlace = "1000 Ljubljana";
        string recipientName = "Utility Company";
        string recipientAddress = "Energy Street 15";
        string recipientPlace = "1000 Ljubljana";
        string recipientIban = "SI56191234567890123";
        string description = "Electricity bill";
        double amount = 75.25;
        string recipientSiModel = "SI12";
        string recipientSiReference = "00012345678901234";

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount,
            recipientSiModel: recipientSiModel, recipientSiReference: recipientSiReference);

        string payload = generator.ToString();

        payload.ShouldContain("SI1200012345678901234\n");
        payload.ShouldContain("00000007525\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_limit_payer_name_length()
    {
        string payerName = "This is a very long name that exceeds the maximum allowed length of 33 characters";
        string payerAddress = "Address 1";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";
        double amount = 100.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        string payload = generator.ToString();

        // Name should be truncated to 33 characters
        payload.ShouldContain("This is a very long name that exc\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_limit_description_length()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "This is a very long description that exceeds the maximum allowed length of 42 characters for the purpose field";
        double amount = 50.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        string payload = generator.ToString();

        // Description should be truncated to 42 characters
        payload.ShouldContain("This is a very long description that excee\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_format_amount_correctly()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";

        // Test various amounts
        var generator1 = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, 0.01);
        generator1.ToString().ShouldContain("00000000001\n");

        var generator2 = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, 1.00);
        generator2.ToString().ShouldContain("00000000100\n");

        var generator3 = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, 1234.56);
        generator3.ToString().ShouldContain("00000123456\n");

        var generator4 = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, 999999.99);
        generator4.ToString().ShouldContain("00099999999\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_calculate_checksum_correctly()
    {
        string payerName = "Test Payer";
        string payerAddress = "Test Address";
        string payerPlace = "Test City";
        string recipientName = "Test Recipient";
        string recipientAddress = "Test Recipient Address";
        string recipientPlace = "Test Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Test Payment";
        double amount = 100.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        string payload = generator.ToString();

        // Checksum should be at the end as a 3-digit number
        string[] lines = payload.Split('\n');
        string checksumLine = lines[lines.Length - 2]; // Second to last line (last is empty)
        checksumLine.Length.ShouldBe(3);
        int.TryParse(checksumLine, out _).ShouldBeTrue();
    }

    [Fact]
    public void slovenian_upnqr_generator_should_trim_whitespace()
    {
        string payerName = "  Payer Name  ";
        string payerAddress = "  Address  ";
        string payerPlace = "  City  ";
        string recipientName = "  Recipient  ";
        string recipientAddress = "  Recipient Address  ";
        string recipientPlace = "  Recipient City  ";
        string recipientIban = "  SI56011006001234567  ";
        string description = "  Payment  ";
        double amount = 50.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        string payload = generator.ToString();

        payload.ShouldContain("Payer Name\n");
        payload.ShouldContain("Address\n");
        payload.ShouldContain("City\n");
        payload.ShouldContain("SI56011006001234567\n");
        payload.ShouldContain("Payment\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_uppercase_code_and_iban()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "si56011006001234567";
        string description = "Payment";
        double amount = 100.00;
        string code = "othr";

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount,
            code: code);

        string payload = generator.ToString();

        payload.ShouldContain("OTHR\n");
        payload.ShouldContain("SI56011006001234567\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_set_correct_qr_properties()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";
        double amount = 100.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        generator.Version.ShouldBe(15);
        generator.EccLevel.ShouldBe(QRCodeGenerator.ECCLevel.M);
        generator.EciMode.ShouldBe(QRCodeGenerator.EciMode.Iso8859_2);
    }

    [Fact]
    public void slovenian_upnqr_generator_should_handle_zero_amount()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";
        double amount = 0.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        string payload = generator.ToString();

        payload.ShouldContain("00000000000\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_limit_iban_length()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567890123456789012345"; // Too long
        string description = "Payment";
        double amount = 100.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount);

        string payload = generator.ToString();

        // IBAN should be truncated to 34 characters
        payload.ShouldContain("SI56011006001234567890123456789012\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_limit_reference_length()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";
        double amount = 100.00;
        string recipientSiModel = "SI12";
        string recipientSiReference = "123456789012345678901234567890"; // Too long

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount,
            recipientSiModel: recipientSiModel, recipientSiReference: recipientSiReference);

        string payload = generator.ToString();

        // Reference should be truncated to 22 characters
        payload.ShouldContain("SI121234567890123456789012\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_handle_empty_deadline()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";
        double amount = 100.00;

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount,
            deadline: null);

        string payload = generator.ToString();

        // Should have empty line for deadline
        string[] lines = payload.Split('\n');
        lines[13].ShouldBe(""); // Deadline line should be empty
    }

    [Fact]
    public void slovenian_upnqr_generator_should_uppercase_si_model()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";
        double amount = 100.00;
        string recipientSiModel = "si12";

        var generator = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, amount,
            recipientSiModel: recipientSiModel);

        string payload = generator.ToString();

        payload.ShouldContain("SI12\n");
    }

    [Fact]
    public void slovenian_upnqr_generator_should_round_amount_correctly()
    {
        string payerName = "Payer";
        string payerAddress = "Address";
        string payerPlace = "City";
        string recipientName = "Recipient";
        string recipientAddress = "Recipient Address";
        string recipientPlace = "Recipient City";
        string recipientIban = "SI56011006001234567";
        string description = "Payment";

        // Test rounding
        var generator1 = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, 10.555);
        generator1.ToString().ShouldContain("00000001056\n"); // Rounds to 10.56

        var generator2 = new PayloadGenerator.SlovenianUpnQr(
            payerName, payerAddress, payerPlace,
            recipientName, recipientAddress, recipientPlace,
            recipientIban, description, 10.554);
        generator2.ToString().ShouldContain("00000001055\n"); // Rounds to 10.55
    }
}

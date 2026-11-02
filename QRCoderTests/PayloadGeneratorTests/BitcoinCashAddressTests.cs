namespace QRCoderTests.PayloadGeneratorTests;

public class BitcoinCashAddressTests
{
    [Fact]
    public void bitcoincash_address_generator_can_generate_address()
    {
        string address = "qqtlfk37qyey50f4wfuhc7jw85zsdp8s2swffjk890";
        double amount = .123;
        string label = "Some Label to Encode";
        string message = "Some Message to Encode";

        var generator = new PayloadGenerator.BitcoinCashAddress(address, amount, label, message);

        generator
            .ToString()
            .ShouldBe("bitcoincash:qqtlfk37qyey50f4wfuhc7jw85zsdp8s2swffjk890?label=Some%20Label%20to%20Encode&message=Some%20Message%20to%20Encode&amount=.123");
    }

    [Fact]
    public void bitcoincash_address_generator_should_skip_missing_label()
    {
        string address = "qqtlfk37qyey50f4wfuhc7jw85zsdp8s2swffjk890";
        double amount = .123;
        string message = "Some Message to Encode";


        var generator = new PayloadGenerator.BitcoinCashAddress(address, amount, null, message);

        generator
            .ToString()
            .ShouldNotContain("label");
    }

    [Fact]
    public void bitcoincash_address_generator_should_skip_missing_message()
    {
        string address = "qqtlfk37qyey50f4wfuhc7jw85zsdp8s2swffjk890";
        double amount = .123;


        var generator = new PayloadGenerator.BitcoinCashAddress(address, amount);

        generator
            .ToString()
            .ShouldNotContain("message");
    }

    [Fact]
    public void bitcoincash_address_generator_should_round_to_satoshi()
    {
        string address = "qqtlfk37qyey50f4wfuhc7jw85zsdp8s2swffjk890";
        double amount = .123456789;


        var generator = new PayloadGenerator.BitcoinCashAddress(address, amount);

        generator
            .ToString()
            .ShouldContain("amount=.12345679");
    }

    [Fact]
    public void bitcoincash_address_generator_disregards_current_culture()
    {
#if NETCOREAPP1_1
        var currentCulture = CultureInfo.DefaultThreadCurrentCulture;
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-DE");
#else
        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
#endif

        string address = "qqtlfk37qyey50f4wfuhc7jw85zsdp8s2swffjk890";
        double amount = .123;


        var generator = new PayloadGenerator.BitcoinCashAddress(address, amount);

        generator
            .ToString()
            .ShouldBe("bitcoincash:qqtlfk37qyey50f4wfuhc7jw85zsdp8s2swffjk890?amount=.123");

#if NETCOREAPP1_1
        CultureInfo.DefaultThreadCurrentCulture = currentCulture;
#else
        Thread.CurrentThread.CurrentCulture = currentCulture;
#endif
    }
}

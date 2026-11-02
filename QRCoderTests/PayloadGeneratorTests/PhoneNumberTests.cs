namespace QRCoderTests.PayloadGeneratorTests;

public class PhoneNumberTests
{
    [Fact]
    public void phonenumber_should_build()
    {
        string number = "+495321123456";

        var generator = new PayloadGenerator.PhoneNumber(number);

        generator.ToString().ShouldBe("tel:+495321123456");
    }
}

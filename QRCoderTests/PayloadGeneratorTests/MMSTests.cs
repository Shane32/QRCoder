namespace QRCoderTests.PayloadGeneratorTests;

public class MMSTests
{
    [Fact]
    public void mms_should_build_type_MMS()
    {
        string number = "01601234567";
        string message = "A tiny MMS";
        PayloadGenerator.MMS.MMSEncoding encoding = PayloadGenerator.MMS.MMSEncoding.MMS;

        var generator = new PayloadGenerator.MMS(number, message, encoding);

        generator.ToString().ShouldBe("mms:01601234567?body=A%20tiny%20MMS");
    }


    [Fact]
    public void mms_should_build_type_MMSTO()
    {
        string number = "01601234567";
        string message = "A tiny SMS";
        PayloadGenerator.MMS.MMSEncoding encoding = PayloadGenerator.MMS.MMSEncoding.MMSTO;

        var generator = new PayloadGenerator.MMS(number, message, encoding);

        generator.ToString().ShouldBe("mmsto:01601234567?subject=A%20tiny%20SMS");
    }


    [Fact]
    public void mms_should_not_add_unused_params()
    {
        string number = "01601234567";

        var generator = new PayloadGenerator.MMS(number);

        generator.ToString().ShouldBe("mms:01601234567");
    }
}

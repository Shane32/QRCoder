namespace QRCoderTests.PayloadGeneratorTests;

public class UrlTests
{
    [Fact]
    public void url_should_build_http()
    {
        string url = "http://code-bude.net";

        var generator = new PayloadGenerator.Url(url);

        generator.ToString().ShouldBe("http://code-bude.net");
    }


    [Fact]
    public void url_should_build_https()
    {
        string url = "https://code-bude.net";

        var generator = new PayloadGenerator.Url(url);

        generator.ToString().ShouldBe("https://code-bude.net");
    }


    [Fact]
    public void url_should_build_https_all_caps()
    {
        string url = "HTTPS://CODE-BUDE.NET";

        var generator = new PayloadGenerator.Url(url);

        generator.ToString().ShouldBe("HTTPS://CODE-BUDE.NET");
    }


    [Fact]
    public void url_should_add_http()
    {
        string url = "code-bude.net";

        var generator = new PayloadGenerator.Url(url);

        generator.ToString().ShouldBe("http://code-bude.net");
    }
}

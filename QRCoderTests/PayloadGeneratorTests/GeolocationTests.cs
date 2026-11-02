namespace QRCoderTests.PayloadGeneratorTests;

public class GeolocationTests
{
    [Fact]
    public void geolocation_should_build_type_GEO()
    {
        string latitude = "51.227741";
        string longitude = "6.773456";
        PayloadGenerator.Geolocation.GeolocationEncoding encoding = PayloadGenerator.Geolocation.GeolocationEncoding.GEO;

        var generator = new PayloadGenerator.Geolocation(latitude, longitude, encoding);

        generator.ToString().ShouldBe("geo:51.227741,6.773456");
    }


    [Fact]
    public void geolocation_should_build_type_GoogleMaps()
    {
        string latitude = "51.227741";
        string longitude = "6.773456";
        PayloadGenerator.Geolocation.GeolocationEncoding encoding = PayloadGenerator.Geolocation.GeolocationEncoding.GoogleMaps;

        var generator = new PayloadGenerator.Geolocation(latitude, longitude, encoding);

        generator.ToString().ShouldBe("http://maps.google.com/maps?q=51.227741,6.773456");
    }


    [Fact]
    public void geolocation_should_escape_input()
    {
        string latitude = "51,227741";
        string longitude = "6,773456";
        PayloadGenerator.Geolocation.GeolocationEncoding encoding = PayloadGenerator.Geolocation.GeolocationEncoding.GEO;

        var generator = new PayloadGenerator.Geolocation(latitude, longitude, encoding);

        generator.ToString().ShouldBe("geo:51.227741,6.773456");
    }


    [Fact]
    public void geolocation_should_add_unused_params()
    {
        string latitude = "51.227741";
        string longitude = "6.773456";

        var generator = new PayloadGenerator.Geolocation(latitude, longitude);

        generator.ToString().ShouldBe("geo:51.227741,6.773456");
    }
}

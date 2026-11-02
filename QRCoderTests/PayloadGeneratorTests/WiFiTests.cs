namespace QRCoderTests.PayloadGeneratorTests;

public class WiFiTests
{
    [Fact]
    public void wifi_should_build_wep()
    {
        string ssid = "MyWiFiSSID";
        string password = "7heP4assw0rd";
        var authmode = PayloadGenerator.WiFi.Authentication.WEP;
        bool hideSSID = false;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WEP;S:MyWiFiSSID;P:7heP4assw0rd;;");
    }

    [Fact]
    public void wifi_should_build_wpa()
    {
        string ssid = "MyWiFiSSID";
        string password = "7heP4assw0rd";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;
        bool hideSSID = false;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:MyWiFiSSID;P:7heP4assw0rd;;");
    }

    [Fact]
    public void wifi_should_build_wpa2()
    {
        string ssid = "MyWiFiSSID";
        string password = "7heP4assw0rd";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA2;
        bool hideSSID = false;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA2;S:MyWiFiSSID;P:7heP4assw0rd;;");
    }

    [Fact]
    public void wifi_should_ignore_hiddenSSID_param()
    {
        string ssid = "MyWiFiSSID";
        string password = "7heP4assw0rd";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:MyWiFiSSID;P:7heP4assw0rd;;");
    }


    [Fact]
    public void wifi_should_add_hiddenSSID_param()
    {
        string ssid = "M\\y;W,i:FiSSID";
        string password = "7heP4assw0rd\\;:,";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;
        bool hideSSID = true;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:M\\\\y\\;W\\,i\\:FiSSID;P:7heP4assw0rd\\\\\\;\\:\\,;H:true;");
    }


    [Fact]
    public void wifi_should_escape_input()
    {
        string ssid = "MyWiFiSSID";
        string password = "7heP4assw0rd";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;
        bool hideSSID = true;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:MyWiFiSSID;P:7heP4assw0rd;H:true;");
    }


    [Fact]
    public void wifi_should_escape_hex_style1()
    {
        string ssid = "A9B7F18CCE";
        string password = "00105F0E6";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;
        bool hideSSID = true;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:\"A9B7F18CCE\";P:\"00105F0E6\";H:true;");
    }


    [Fact]
    public void wifi_should_escape_hex_style2()
    {
        string ssid = "a9b7f18cce";
        string password = "00105f0Ee6";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;
        bool hideSSID = true;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:\"a9b7f18cce\";P:\"00105f0Ee6\";H:true;");
    }


    [Fact]
    public void wifi_should_escape_hex_style3()
    {
        string ssid = "0xA9B7F18CCE";
        string password = "0x00105F0E6";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;
        bool hideSSID = true;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:\"0xA9B7F18CCE\";P:\"0x00105F0E6\";H:true;");
    }


    [Fact]
    public void wifi_should_escape_hex_style4()
    {
        string ssid = "0XA9B7F18CCE";
        string password = "0X00105F0E6";
        var authmode = PayloadGenerator.WiFi.Authentication.WPA;
        bool hideSSID = true;

        var generator = new PayloadGenerator.WiFi(ssid, password, authmode, hideSSID);

        generator.ToString().ShouldBe($"WIFI:T:WPA;S:\"0XA9B7F18CCE\";P:\"0X00105F0E6\";H:true;");
    }
}

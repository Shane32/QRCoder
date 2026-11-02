namespace QRCoderTests.PayloadGeneratorTests;

public class ShadowSocksConfigTests
{
    [Fact]
    public void shadowsocks_generator_can_generate_payload()
    {
        string host = "192.168.2.5";
        int port = 1;
        string password = "s3cr3t";
        PayloadGenerator.ShadowSocksConfig.Method method = PayloadGenerator.ShadowSocksConfig.Method.Rc4Md5;
        var generator = new PayloadGenerator.ShadowSocksConfig(host, port, password, method);

        generator
            .ToString()
            .ShouldBe("ss://cmM0LW1kNTpzM2NyM3RAMTkyLjE2OC4yLjU6MQ==");
    }

    [Fact]
    public void shadowsocks_generator_can_generate_payload_with_tag()
    {
        string host = "192.168.2.5";
        int port = 65535;
        string password = "s3cr3t";
        PayloadGenerator.ShadowSocksConfig.Method method = PayloadGenerator.ShadowSocksConfig.Method.Rc4Md5;
        string tag = "server42";
        var generator = new PayloadGenerator.ShadowSocksConfig(host, port, password, method, tag);

        generator
            .ToString()
            .ShouldBe("ss://cmM0LW1kNTpzM2NyM3RAMTkyLjE2OC4yLjU6NjU1MzU=#server42");
    }


    [Fact]
    public void shadowsocks_generator_should_throw_portrange_low_exception()
    {
        string host = "192.168.2.5";
        int port = 0;
        string password = "s3cr3t";
        PayloadGenerator.ShadowSocksConfig.Method method = PayloadGenerator.ShadowSocksConfig.Method.Rc4Md5;

        PayloadGenerator.ShadowSocksConfig.ShadowSocksConfigException exception = Should.Throw<PayloadGenerator.ShadowSocksConfig.ShadowSocksConfigException>(() => new PayloadGenerator.ShadowSocksConfig(host, port, password, method));

        exception.Message.ShouldBe("Value of 'port' must be within 0 and 65535.");
    }


    [Fact]
    public void shadowsocks_generator_should_throw_portrange_high_exception()
    {
        string host = "192.168.2.5";
        int port = 65536;
        string password = "s3cr3t";
        PayloadGenerator.ShadowSocksConfig.Method method = PayloadGenerator.ShadowSocksConfig.Method.Rc4Md5;

        PayloadGenerator.ShadowSocksConfig.ShadowSocksConfigException exception = Should.Throw<PayloadGenerator.ShadowSocksConfig.ShadowSocksConfigException>(() => new PayloadGenerator.ShadowSocksConfig(host, port, password, method));

        exception.Message.ShouldBe("Value of 'port' must be within 0 and 65535.");
    }


    [Fact]
    public void shadowsocks_generator_can_generate_payload_with_plugin()
    {
        string host = "192.168.100.1";
        int port = 8888;
        string password = "test";
        PayloadGenerator.ShadowSocksConfig.Method method = PayloadGenerator.ShadowSocksConfig.Method.BfCfb;
        string plugin = "obfs-local";
        string pluginOption = "obfs=http;obfs-host=google.com";
        var generator = new PayloadGenerator.ShadowSocksConfig(host, port, password, method, plugin, pluginOption);

        generator
            .ToString()
            .ShouldBe("ss://YmYtY2ZiOnRlc3Q@192.168.100.1:8888/?plugin=obfs-local%3bobfs%3dhttp%3bobfs-host%3dgoogle.com");
    }
}

namespace QRCoderTests.PayloadGeneratorTests;

public class MailTests
{
    [Fact]
    public void mail_should_build_type_mailto()
    {
        string receiver = "john@doe.com";
        string subject = "A test mail";
        string message = "Just see if it works!";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.MAILTO;

        var generator = new PayloadGenerator.Mail(receiver, subject, message, encoding);

        generator.ToString().ShouldBe("mailto:john@doe.com?subject=A%20test%20mail&body=Just%20see%20if%20it%20works%21");
    }


    [Fact]
    public void mail_should_build_type_mailto_receiver_only()
    {
        string receiver = "john@doe.com";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.MAILTO;

        var generator = new PayloadGenerator.Mail(mailReceiver: receiver, encoding: encoding);

        generator.ToString().ShouldBe("mailto:john@doe.com");
    }


    [Fact]
    public void mail_should_build_type_mailto_subject_only()
    {
        string receiver = "john@doe.com";
        string subject = "A test mail";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.MAILTO;

        var generator = new PayloadGenerator.Mail(receiver, subject, encoding: encoding);

        generator.ToString().ShouldBe("mailto:john@doe.com?subject=A%20test%20mail");
    }

    [Fact]
    public void mail_should_build_type_mailto_message_only()
    {
        string receiver = "john@doe.com";
        string message = "Just see if it works!";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.MAILTO;

        var generator = new PayloadGenerator.Mail(receiver, message: message, encoding: encoding);

        generator.ToString().ShouldBe("mailto:john@doe.com?body=Just%20see%20if%20it%20works%21");
    }

    [Fact]
    public void mail_should_build_type_mailto_no_receiver()
    {
        string subject = "A test mail";
        string message = "Just see if it works!";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.MAILTO;

        var generator = new PayloadGenerator.Mail(subject: subject, message: message, encoding: encoding);

        generator.ToString().ShouldBe("mailto:?subject=A%20test%20mail&body=Just%20see%20if%20it%20works%21");
    }

    [Fact]
    public void mail_should_build_type_MATMSG()
    {
        string receiver = "john@doe.com";
        string subject = "A test mail";
        string message = "Just see if it works!";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.MATMSG;

        var generator = new PayloadGenerator.Mail(receiver, subject, message, encoding);

        generator.ToString().ShouldBe("MATMSG:TO:john@doe.com;SUB:A test mail;BODY:Just see if it works!;;");
    }


    [Fact]
    public void mail_should_build_type_SMTP()
    {
        string receiver = "john@doe.com";
        string subject = "A test mail";
        string message = "Just see if it works!";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.SMTP;

        var generator = new PayloadGenerator.Mail(receiver, subject, message, encoding);

        generator.ToString().ShouldBe("SMTP:john@doe.com:A test mail:Just see if it works!");
    }


    [Fact]
    public void mail_should_escape_input_MATMSG()
    {
        string receiver = "john@doe.com";
        string subject = "A test mail";
        string message = "Just see if \\:;, it works!";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.MATMSG;

        var generator = new PayloadGenerator.Mail(receiver, subject, message, encoding);

        generator.ToString().ShouldBe("MATMSG:TO:john@doe.com;SUB:A test mail;BODY:Just see if \\\\\\:\\;\\, it works!;;");
    }

    [Fact]
    public void mail_should_escape_input_SMTP()
    {
        string receiver = "john@doe.com";
        string subject = "A test mail";
        string message = "Just see: if it works!";
        PayloadGenerator.Mail.MailEncoding encoding = PayloadGenerator.Mail.MailEncoding.SMTP;

        var generator = new PayloadGenerator.Mail(receiver, subject, message, encoding);

        generator.ToString().ShouldBe("SMTP:john@doe.com:A test mail:Just see\\: if it works!");
    }
}

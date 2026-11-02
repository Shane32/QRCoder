namespace QRCoderTests.PayloadGeneratorTests;

public class CalendarEventTests
{
    [Fact]
    public void calendarevent_should_build_universal()
    {
        string subject = "Release party";
        string description = "A small party for the new QRCoder. Bring some beer!";
        string location = "Programmer's paradise, Beachtown, Paradise";
        bool alldayEvent = false;
        var begin = new DateTime(2016, 01, 03, 12, 00, 00);
        var end = new DateTime(2016, 01, 03, 14, 30, 00);
        var encoding = PayloadGenerator.CalendarEvent.EventEncoding.Universal;

        var generator = new PayloadGenerator.CalendarEvent(subject, description, location, begin, end, alldayEvent, encoding);

        generator.ToString().ShouldBe($"BEGIN:VEVENT{Environment.NewLine}SUMMARY:Release party{Environment.NewLine}DESCRIPTION:A small party for the new QRCoder. Bring some beer!{Environment.NewLine}LOCATION:Programmer's paradise, Beachtown, Paradise{Environment.NewLine}DTSTART:20160103T120000{Environment.NewLine}DTEND:20160103T143000{Environment.NewLine}END:VEVENT");
    }


    [Fact]
    public void calendarevent_should_build_ical()
    {
        string subject = "Release party";
        string description = "A small party for the new QRCoder. Bring some beer!";
        string location = "Programmer's paradise, Beachtown, Paradise";
        bool alldayEvent = false;
        var begin = new DateTime(2016, 01, 03, 12, 00, 00);
        var end = new DateTime(2016, 01, 03, 14, 30, 0);
        var encoding = PayloadGenerator.CalendarEvent.EventEncoding.iCalComplete;

        var generator = new PayloadGenerator.CalendarEvent(subject, description, location, begin, end, alldayEvent, encoding);

        generator.ToString().ShouldBe($"BEGIN:VCALENDAR{Environment.NewLine}VERSION:2.0{Environment.NewLine}BEGIN:VEVENT{Environment.NewLine}SUMMARY:Release party{Environment.NewLine}DESCRIPTION:A small party for the new QRCoder. Bring some beer!{Environment.NewLine}LOCATION:Programmer's paradise, Beachtown, Paradise{Environment.NewLine}DTSTART:20160103T120000{Environment.NewLine}DTEND:20160103T143000{Environment.NewLine}END:VEVENT{Environment.NewLine}END:VCALENDAR");
    }


    [Fact]
    public void calendarevent_should_build_with_utc_datetime()
    {
        string subject = "Release party";
        string description = "A small party for the new QRCoder. Bring some beer!";
        string location = "Programmer's paradise, Beachtown, Paradise";
        bool alldayEvent = false;
        var begin = new DateTime(2016, 01, 03, 12, 00, 00, DateTimeKind.Utc);
        var end = new DateTime(2016, 01, 03, 14, 30, 00, DateTimeKind.Utc);
        var encoding = PayloadGenerator.CalendarEvent.EventEncoding.Universal;

        var generator = new PayloadGenerator.CalendarEvent(subject, description, location, begin, end, alldayEvent, encoding);

        generator.ToString().ShouldBe($"BEGIN:VEVENT{Environment.NewLine}SUMMARY:Release party{Environment.NewLine}DESCRIPTION:A small party for the new QRCoder. Bring some beer!{Environment.NewLine}LOCATION:Programmer's paradise, Beachtown, Paradise{Environment.NewLine}DTSTART:20160103T120000Z{Environment.NewLine}DTEND:20160103T143000Z{Environment.NewLine}END:VEVENT");
    }


    [Fact]
    public void calendarevent_should_build_with_utc_offset()
    {
        string subject = "Release party";
        string description = "A small party for the new QRCoder. Bring some beer!";
        string location = "Programmer's paradise, Beachtown, Paradise";
        bool alldayEvent = false;
        var begin = new DateTimeOffset(2016, 01, 03, 12, 00, 00, new TimeSpan(3, 0, 0));
        var end = new DateTimeOffset(2016, 01, 03, 14, 30, 00, new TimeSpan(3, 0, 0));
        var encoding = PayloadGenerator.CalendarEvent.EventEncoding.Universal;

        var generator = new PayloadGenerator.CalendarEvent(subject, description, location, begin, end, alldayEvent, encoding);

        generator.ToString().ShouldBe($"BEGIN:VEVENT{Environment.NewLine}SUMMARY:Release party{Environment.NewLine}DESCRIPTION:A small party for the new QRCoder. Bring some beer!{Environment.NewLine}LOCATION:Programmer's paradise, Beachtown, Paradise{Environment.NewLine}DTSTART:20160103T090000Z{Environment.NewLine}DTEND:20160103T113000Z{Environment.NewLine}END:VEVENT");
    }


    [Fact]
    public void calendarevent_should_build_allday()
    {
        string subject = "Release party";
        string description = "A small party for the new QRCoder. Bring some beer!";
        string location = "Programmer's paradise, Beachtown, Paradise";
        bool alldayEvent = true;
        var begin = new DateTime(2016, 01, 03);
        var end = new DateTime(2016, 01, 03);
        var encoding = PayloadGenerator.CalendarEvent.EventEncoding.Universal;

        var generator = new PayloadGenerator.CalendarEvent(subject, description, location, begin, end, alldayEvent, encoding);

        generator.ToString().ShouldBe($"BEGIN:VEVENT{Environment.NewLine}SUMMARY:Release party{Environment.NewLine}DESCRIPTION:A small party for the new QRCoder. Bring some beer!{Environment.NewLine}LOCATION:Programmer's paradise, Beachtown, Paradise{Environment.NewLine}DTSTART:20160103{Environment.NewLine}DTEND:20160103{Environment.NewLine}END:VEVENT");
    }


    [Fact]
    public void calendarevent_should_care_empty_fields()
    {
        string subject = "Release party";
        string description = "";
        string location = string.Empty;
        bool alldayEvent = false;
        var begin = new DateTime(2016, 01, 03, 12, 00, 00);
        var end = new DateTime(2016, 01, 03, 14, 30, 0);
        var encoding = PayloadGenerator.CalendarEvent.EventEncoding.Universal;

        var generator = new PayloadGenerator.CalendarEvent(subject, description, location, begin, end, alldayEvent, encoding);

        generator.ToString().ShouldBe($"BEGIN:VEVENT{Environment.NewLine}SUMMARY:Release party{Environment.NewLine}DTSTART:20160103T120000{Environment.NewLine}DTEND:20160103T143000{Environment.NewLine}END:VEVENT");
    }


    [Fact]
    public void calendarevent_should_add_unused_params()
    {
        string subject = "Release party";
        string description = "A small party for the new QRCoder. Bring some beer!";
        string location = "Programmer's paradise, Beachtown, Paradise";
        bool alldayEvent = false;
        var begin = new DateTime(2016, 01, 03, 12, 00, 00);
        var end = new DateTime(2016, 01, 03, 14, 30, 0);

        var generator = new PayloadGenerator.CalendarEvent(subject, description, location, begin, end, alldayEvent);

        generator.ToString().ShouldBe($"BEGIN:VEVENT{Environment.NewLine}SUMMARY:Release party{Environment.NewLine}DESCRIPTION:A small party for the new QRCoder. Bring some beer!{Environment.NewLine}LOCATION:Programmer's paradise, Beachtown, Paradise{Environment.NewLine}DTSTART:20160103T120000{Environment.NewLine}DTEND:20160103T143000{Environment.NewLine}END:VEVENT");
    }
}

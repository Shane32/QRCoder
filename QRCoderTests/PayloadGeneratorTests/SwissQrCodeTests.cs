using static QRCoder.PayloadGenerator.SwissQrCode.AdditionalInformation;
using static QRCoder.PayloadGenerator.SwissQrCode.Reference;
using static QRCoder.QRCodeGenerator;

namespace QRCoderTests.PayloadGeneratorTests;

public class SwissQrCodeTests
{
    [Fact]
    public void swissqrcode_generator_should_throw_reference_not_allowed()
    {
        ReferenceType refType = ReferenceType.NON;
        string reference = "1234567890123456";
        ReferenceTextType refTextType = ReferenceTextType.CreditorReferenceIso11649;

        SwissQrCodeReferenceException exception = Should.Throw<SwissQrCodeReferenceException>(() => new PayloadGenerator.SwissQrCode.Reference(refType, reference, refTextType));

        exception.Message.ShouldBe("Reference is only allowed when referenceType not equals \"NON\"");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_missing_reftexttype()
    {
        ReferenceType refType = ReferenceType.SCOR;
        string reference = "1234567890123456";

        SwissQrCodeReferenceException exception = Should.Throw<SwissQrCodeReferenceException>(() => new PayloadGenerator.SwissQrCode.Reference(refType, reference));

        exception.Message.ShouldBe("You have to set an ReferenceTextType when using the reference text.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_qrr_ref_too_long()
    {
        ReferenceType refType = ReferenceType.QRR;
        string reference = "9900050000000003200710123031234654574398214093682164062138462089364";
        ReferenceTextType refTextType = ReferenceTextType.QrReference;

        SwissQrCodeReferenceException exception = Should.Throw<SwissQrCodeReferenceException>(() => new PayloadGenerator.SwissQrCode.Reference(refType, reference, refTextType));

        exception.Message.ShouldBe("QR-references have to be shorter than 28 chars.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_qrr_ref_wrong_char()
    {
        ReferenceType refType = ReferenceType.QRR;
        string reference = "99000ABCDF5000032007101230";
        ReferenceTextType refTextType = ReferenceTextType.QrReference;

        SwissQrCodeReferenceException exception = Should.Throw<SwissQrCodeReferenceException>(() => new PayloadGenerator.SwissQrCode.Reference(refType, reference, refTextType));

        exception.Message.ShouldBe("QR-reference must exist out of digits only.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_qrr_ref_checksum_invalid()
    {
        ReferenceType refType = ReferenceType.QRR;
        string reference = "990005000000000320071012304";
        ReferenceTextType refTextType = ReferenceTextType.QrReference;

        SwissQrCodeReferenceException exception = Should.Throw<SwissQrCodeReferenceException>(() => new PayloadGenerator.SwissQrCode.Reference(refType, reference, refTextType));

        exception.Message.ShouldBe("QR-references is invalid. Checksum error.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_iso11649_ref_too_long()
    {
        ReferenceType refType = ReferenceType.QRR;
        string reference = "99000500000000032007101230312346545743982162138462089364";
        ReferenceTextType refTextType = ReferenceTextType.CreditorReferenceIso11649;

        SwissQrCodeReferenceException exception = Should.Throw<SwissQrCodeReferenceException>(() => new PayloadGenerator.SwissQrCode.Reference(refType, reference, refTextType));

        exception.Message.ShouldBe("Creditor references (ISO 11649) have to be shorter than 26 chars.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_unstructured_msg_too_long()
    {
        string billInformation = "This is sample bill information with a length below 140.";
        string unstructuredMessage = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum";

        SwissQrCodeAdditionalInformationException exception = Should.Throw<SwissQrCodeAdditionalInformationException>(() => new PayloadGenerator.SwissQrCode.AdditionalInformation(unstructuredMessage, billInformation));

        exception.Message.ShouldBe("Unstructured message and bill information must be shorter than 141 chars in total/combined.");
    }



    [Fact]
    public void swissqrcode_generator_should_generate_iban()
    {
        string iban = "CH2609000000857666015";
        PayloadGenerator.SwissQrCode.Iban.IbanType ibanType = PayloadGenerator.SwissQrCode.Iban.IbanType.Iban;

        var generator = new PayloadGenerator.SwissQrCode.Iban(iban, ibanType);

        generator
            .ToString()
            .ShouldBe("CH2609000000857666015");
    }

    [Fact]
    public void swissqrcode_generator_should_generate_iban_2()
    {
        string iban = "CH47048350000GABRIELS";
        PayloadGenerator.SwissQrCode.Iban.IbanType ibanType = PayloadGenerator.SwissQrCode.Iban.IbanType.Iban;

        var generator = new PayloadGenerator.SwissQrCode.Iban(iban, ibanType);

        generator
            .ToString()
            .ShouldBe("CH47048350000GABRIELS");
    }

    [Fact]
    public void swissqrcode_generator_should_generate_iban_qr()
    {
        string iban = "CH2430043000000789012";
        PayloadGenerator.SwissQrCode.Iban.IbanType ibanType = PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban;

        var generator = new PayloadGenerator.SwissQrCode.Iban(iban, ibanType);

        generator
            .ToString()
            .ShouldBe("CH2430043000000789012");
    }

    [Fact]
    public void swissqrcode_generator_should_remove_spaces_iban()
    {
        string iban = "CH26 0900 0000 8576 6601 5";
        PayloadGenerator.SwissQrCode.Iban.IbanType ibanType = PayloadGenerator.SwissQrCode.Iban.IbanType.Iban;

        var generator = new PayloadGenerator.SwissQrCode.Iban(iban, ibanType);

        generator
            .ToString()
            .ShouldBe("CH2609000000857666015");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_invalid_iban()
    {
        string iban = "CHC2609000000857666015";
        PayloadGenerator.SwissQrCode.Iban.IbanType ibanType = PayloadGenerator.SwissQrCode.Iban.IbanType.Iban;

        PayloadGenerator.SwissQrCode.Iban.SwissQrCodeIbanException exception = Should.Throw<PayloadGenerator.SwissQrCode.Iban.SwissQrCodeIbanException>(() => new PayloadGenerator.SwissQrCode.Iban(iban, ibanType));

        exception.Message.ShouldBe("The IBAN entered isn't valid.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_invalid_qriban()
    {
        string iban = "CHC2609000000857666015";
        PayloadGenerator.SwissQrCode.Iban.IbanType ibanType = PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban;

        PayloadGenerator.SwissQrCode.Iban.SwissQrCodeIbanException exception = Should.Throw<PayloadGenerator.SwissQrCode.Iban.SwissQrCodeIbanException>(() => new PayloadGenerator.SwissQrCode.Iban(iban, ibanType));

        exception.Message.ShouldBe("The QR-IBAN entered isn't valid.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_ivalid_iban_country()
    {
        string iban = "DE2609000000857666015";
        PayloadGenerator.SwissQrCode.Iban.IbanType ibanType = PayloadGenerator.SwissQrCode.Iban.IbanType.Iban;

        PayloadGenerator.SwissQrCode.Iban.SwissQrCodeIbanException exception = Should.Throw<PayloadGenerator.SwissQrCode.Iban.SwissQrCodeIbanException>(() => new PayloadGenerator.SwissQrCode.Iban(iban, ibanType));

        exception.Message.ShouldBe("The IBAN must start with \"CH\" or \"LI\".");
    }



    [Fact]
    public void swissqrcode_generator_should_generate_contact_simple()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "Bern";
        string country = "CH";

        var generator = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, null, null);

        generator
            .ToString()
            .ShouldBe("S\r\nJohn Doe\r\n\r\n\r\n3003\r\nBern\r\nCH\r\n");
    }

    [Fact]
    public void swissqrcode_generator_should_generate_contact_full()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        var generator = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber);

        generator
            .ToString()
            .ShouldBe("S\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_name_empty()
    {
        string name = "";
        string zip = "3003";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("Name must not be empty.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_name_too_long()
    {
        string name = "John Dorian Peter Charles Lord of the Rings and Master of Disaster Grayham";
        string zip = "3003";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("Name must be shorter than 71 chars.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_street_too_long()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude in der wunderschönen aber auch ziemlich teuren Stadt Bern in der Schweiz";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("Street must be shorter than 71 chars.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_street_with_illegal_char()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude 1 ♥";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe(@"Street must match the following pattern as defined in pain.001: ^([a-zA-Z0-9\.,;:'\ \+\-/\(\)?\*\[\]\{\}\\`´~ ^|]|[!""#%&<>÷=@_$£¡¢¤¥¦§¨©ª«¬®¯°±²³µ¶·¸¹º»¼½¾¿×Ø€]|[àáâäãåāăąçćĉċčďđèéêëēĕėęěĝğġģĥħìíîïĩīĭįıĳķĸĺļľŀłñńņňŉŋòóôöōŏőõŕŗřśŝşšșţťŧțùúûüũūŭůűųŵýÿŷźżžßÀÁÂÄÃÅĀĂĄÇĆĈĊČĎĐÈÉÊËĒĔĖĘĚĜĞĠĢĤĦÌÍÎÏĨĪĬĮİĲĴĵĶĹĻĽĿŁÑŃŅŇŊÒÓÔÖÕŌŎŐŔŖŘŚŜŞŠȘŢŤŦȚÙÚÛÜŨŪŬŮŰŲŴÝŶŸŹŻŽÆÐÞæðøþŒœſ])*$");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_housenumber_too_long()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "123456789123456789";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("House number must be shorter than 17 chars.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_zip_empty()
    {
        string name = "John Doe";
        string zip = "";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("Zip code must not be empty.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_zip_too_long()
    {
        string name = "John Doe";
        string zip = "30031234567891234";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("Zip code must be shorter than 17 chars.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_zip_has_illegal_char()
    {
        string name = "John Doe";
        string zip = "3003CHF♥";
        string city = "Bern";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe(@"Zip code must match the following pattern as defined in pain.001: ^([a-zA-Z0-9\.,;:'\ \+\-/\(\)?\*\[\]\{\}\\`´~ ^|]|[!""#%&<>÷=@_$£¡¢¤¥¦§¨©ª«¬®¯°±²³µ¶·¸¹º»¼½¾¿×Ø€]|[àáâäãåāăąçćĉċčďđèéêëēĕėęěĝğġģĥħìíîïĩīĭįıĳķĸĺļľŀłñńņňŉŋòóôöōŏőõŕŗřśŝşšșţťŧțùúûüũūŭůűųŵýÿŷźżžßÀÁÂÄÃÅĀĂĄÇĆĈĊČĎĐÈÉÊËĒĔĖĘĚĜĞĠĢĤĦÌÍÎÏĨĪĬĮİĲĴĵĶĹĻĽĿŁÑŃŅŇŊÒÓÔÖÕŌŎŐŔŖŘŚŜŞŠȘŢŤŦȚÙÚÛÜŨŪŬŮŰŲŴÝŶŸŹŻŽÆÐÞæðøþŒœſ])*$");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_city_empty()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("City must not be empty.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_city_too_long()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "Berner-Sangerhausen-Ober-Hinter-der-Alm-Stadt-am-Unter-Über-Berg";
        string country = "CH";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("City name must be shorter than 36 chars.");
    }


    [Fact]
    public void swissqrcode_generator_should_throw_wrong_countrycode()
    {
        string name = "John Doe";
        string zip = "3003";
        string city = "Bern";
        string country = "CHE";
        string street = "Parlamentsgebäude";
        string houseNumber = "1";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country, street, houseNumber));

        exception.Message.ShouldBe("Country must be a valid \"two letter\" country code as defined by ISO 3166-1, but it isn't.");
    }



    [Fact]
    public void swissqrcode_generator_should_generate_swisscode_simple()
    {
        var creditor = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.EUR;

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, creditor, reference);

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2430043000000789012\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nEUR\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nQRR\r\n990005000000000320071012303\r\n\r\nEPD");
    }

    [Fact]
    public void swissqrcode_generator_should_generate_swisscode_full()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.", "Some bill information here...");
        decimal amount = 100.25m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral);

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2430043000000789012\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n100.25\r\nCHF\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\nQRR\r\n990005000000000320071012303\r\nThis is my unstructured message.\r\nEPD\r\nSome bill information here...");
    }

    [Fact]
    public void swissqrcode_generator_sets_encoding_parameters()
    {
        var creditor = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.EUR;

        var payload = new PayloadGenerator.SwissQrCode(iban, currency, creditor, reference);

        payload.EccLevel.ShouldBe<ECCLevel>(ECCLevel.M);
        payload.EciMode.ShouldBe<EciMode>(EciMode.Utf8);
        payload.Version.ShouldBe(-1);
    }

    [Fact]
    public void swissqrcode_generator_should_generate_clean_end_linebreaks()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.");
        decimal amount = 100.25m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral);

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2430043000000789012\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n100.25\r\nCHF\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\nQRR\r\n990005000000000320071012303\r\nThis is my unstructured message.\r\nEPD");
    }


    [Fact]
    public void swissqrcode_generator_should_generate_swisscode_full_alt()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.", "Some bill information here...");
        decimal amount = 100.25m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral, "alt1", "alt2");

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2430043000000789012\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n100.25\r\nCHF\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\nQRR\r\n990005000000000320071012303\r\nThis is my unstructured message.\r\nEPD\r\nSome bill information here...\r\nalt1\r\nalt2");
    }

    [Fact]
    public void swissqrcode_generator_should_not_generate_space_as_thousands_separator()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.SCOR, "99000500000000032003", ReferenceTextType.CreditorReferenceIso11649);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.", "Some bill information here...");
        decimal amount = 1234567.89m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral, "alt1", "alt2");

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2609000000857666015\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n1234567.89\r\nCHF\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\nSCOR\r\n99000500000000032003\r\nThis is my unstructured message.\r\nEPD\r\nSome bill information here...\r\nalt1\r\nalt2");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_amount_too_big()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.", "Some bill information here...");
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        decimal amount = 1234567891.25m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);

        PayloadGenerator.SwissQrCode.SwissQrCodeException exception = Should.Throw<PayloadGenerator.SwissQrCode.SwissQrCodeException>(() => new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral));

        exception.Message.ShouldBe("Amount (including decimals) must be shorter than 13 places.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_incompatible_reftype()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.NON);
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.", "Some bill information here...");
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        decimal amount = 100.25m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);

        PayloadGenerator.SwissQrCode.SwissQrCodeException exception = Should.Throw<PayloadGenerator.SwissQrCode.SwissQrCodeException>(() => new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral));

        exception.Message.ShouldBe("If QR-IBAN is used, you have to choose \"QRR\" as reference type!");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_alt1_too_long()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR);
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.", "Some bill information here...");
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        decimal amount = 100.25m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);
        string alt1 = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean ma";

        PayloadGenerator.SwissQrCode.SwissQrCodeException exception = Should.Throw<PayloadGenerator.SwissQrCode.SwissQrCodeException>(() => new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral, alt1));

        exception.Message.ShouldBe("Alternative procedure information block 1 must be shorter than 101 chars.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_alt2_too_long()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR);
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("This is my unstructured message.", "Some bill information here...");
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        decimal amount = 100.25m;
        var reqDateOfPayment = new DateTime(2017, 03, 01);
        string alt1 = "lorem ipsum";
        string alt2 = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean ma";
        PayloadGenerator.SwissQrCode.SwissQrCodeException exception = Should.Throw<PayloadGenerator.SwissQrCode.SwissQrCodeException>(() => new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation, contactGeneral, amount, reqDateOfPayment, contactGeneral, alt1, alt2));

        exception.Message.ShouldBe("Alternative procedure information block 2 must be shorter than 101 chars.");
    }

    [Fact]
    public void swissqrcode_generator_should_validate_two_lettercodes()
    {
        string name = "John Doe";
        string zip = "12345";
        string city = "Gotham City";

        // Should work, as DE is a valid country code
        string country = "DE";
        Should.NotThrow(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country));

        // Should work, as de is a valid country code and case should be ignored
        country = "de";
        Should.NotThrow(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country));

        // Should work, as XK is is defined as special case (not officially ISO-3166-1,but used in the wild)
        // See https://en.wikipedia.org/wiki/XK_(user_assigned_code) and https://github.com/Shane32/QRCoder/issues/420
        country = "XK";
        Should.NotThrow(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country));


        // Should throw exception, as ZZ isn't a valid country code
        country = "ZZ";
        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress(name, zip, city, country));

        exception.Message.ShouldBe("Country must be a valid \"two letter\" country code as defined by ISO 3166-1, but it isn't.");
    }

    [Fact]
    public void swissqrcode_generator_should_generate_with_combined_address_type()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithCombinedAddress("John Doe", "CH", "Parlamentsgebäude 1", "3003 Bern");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference);

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2430043000000789012\r\nK\r\nJohn Doe\r\nParlamentsgebäude 1\r\n3003 Bern\r\n\r\n\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nCHF\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nQRR\r\n990005000000000320071012303\r\n\r\nEPD");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_combined_address_name_empty()
    {
        string name = "";
        string country = "CH";
        string addressLine1 = "Parlamentsgebäude 1";
        string addressLine2 = "3003 Bern";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithCombinedAddress(name, country, addressLine1, addressLine2));

        exception.Message.ShouldBe("Name must not be empty.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_combined_address_name_too_long()
    {
        string name = "John Dorian Peter Charles Lord of the Rings and Master of Disaster Grayham";
        string country = "CH";
        string addressLine1 = "Parlamentsgebäude 1";
        string addressLine2 = "3003 Bern";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithCombinedAddress(name, country, addressLine1, addressLine2));

        exception.Message.ShouldBe("Name must be shorter than 71 chars.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_combined_address_line1_too_long()
    {
        string name = "John Doe";
        string country = "CH";
        string addressLine1 = "This is a very long address line that exceeds the maximum allowed length of 70 characters for combined address";
        string addressLine2 = "City";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithCombinedAddress(name, country, addressLine1, addressLine2));

        exception.Message.ShouldBe("Address line 1 must be shorter than 71 chars.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_combined_address_line2_too_long()
    {
        string name = "John Doe";
        string country = "CH";
        string addressLine1 = "Parlamentsgebäude 1";
        string addressLine2 = "This is a very long address line that exceeds the maximum allowed length of 70 characters for combined address";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithCombinedAddress(name, country, addressLine1, addressLine2));

        exception.Message.ShouldBe("Address line 2 must be shorter than 71 chars.");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_combined_address_country_invalid()
    {
        string name = "John Doe";
        string country = "CHE";
        string addressLine1 = "Parlamentsgebäude 1";
        string addressLine2 = "3003 Bern";

        PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException exception = Should.Throw<PayloadGenerator.SwissQrCode.Contact.SwissQrCodeContactException>(() => PayloadGenerator.SwissQrCode.Contact.WithCombinedAddress(name, country, addressLine1, addressLine2));

        exception.Message.ShouldBe("Country must be a valid \"two letter\" country code as defined by ISO 3166-1, but it isn't.");
    }

    [Fact]
    public void swissqrcode_generator_should_generate_with_minimal_reference()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.NON);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference);

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2609000000857666015\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nCHF\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nNON\r\n\r\n\r\nEPD");
    }

    [Fact]
    public void swissqrcode_generator_should_generate_with_scor_reference()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.SCOR, "RF18539007547034", ReferenceTextType.CreditorReferenceIso11649);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference);

        generator
            .ToString()
            .ShouldBe("SPC\r\n0200\r\n1\r\nCH2609000000857666015\r\nS\r\nJohn Doe\r\nParlamentsgebäude\r\n1\r\n3003\r\nBern\r\nCH\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nCHF\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nSCOR\r\nRF18539007547034\r\n\r\nEPD");
    }


    [Fact]
    public void swissqrcode_generator_should_handle_liechtenstein_iban()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "9490", "Vaduz", "LI", "Städtle", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("LI21088100002324013AA", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.NON);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference);

        generator.ToString().ShouldContain("LI21088100002324013AA");
    }

    [Fact]
    public void swissqrcode_generator_should_throw_qrr_reference_without_qr_iban()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR, "990005000000000320071012303", ReferenceTextType.QrReference);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;

        PayloadGenerator.SwissQrCode.SwissQrCodeException exception = Should.Throw<PayloadGenerator.SwissQrCode.SwissQrCodeException>(() => new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference));

        exception.Message.ShouldBe("If non QR-IBAN is used, you have to choose either \"SCOR\" or \"NON\" as reference type!");
    }

    [Fact]
    public void swissqrcode_generator_should_handle_additional_information_without_bill_information()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.NON);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation("Unstructured message only");

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation);

        generator.ToString().ShouldContain("Unstructured message only\r\nEPD");
    }

    [Fact]
    public void swissqrcode_generator_should_handle_bill_information_without_unstructured_message()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.NON);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation(null, "Bill information only");

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation);

        generator.ToString().ShouldContain("\r\nEPD\r\nBill information only");
    }

    [Fact]
    public void swissqrcode_generator_should_handle_empty_additional_information()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2609000000857666015", PayloadGenerator.SwissQrCode.Iban.IbanType.Iban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.NON);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;
        var additionalInformation = new PayloadGenerator.SwissQrCode.AdditionalInformation();

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference, additionalInformation);

        generator.ToString().ShouldContain("\r\nEPD");
    }


    [Fact]
    public void swissqrcode_generator_should_handle_qr_iban_with_qrr_reference_default_counter()
    {
        var contactGeneral = PayloadGenerator.SwissQrCode.Contact.WithStructuredAddress("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        var iban = new PayloadGenerator.SwissQrCode.Iban("CH2430043000000789012", PayloadGenerator.SwissQrCode.Iban.IbanType.QrIban);
        var reference = new PayloadGenerator.SwissQrCode.Reference(ReferenceType.QRR);
        PayloadGenerator.SwissQrCode.Currency currency = PayloadGenerator.SwissQrCode.Currency.CHF;

        var generator = new PayloadGenerator.SwissQrCode(iban, currency, contactGeneral, reference);

        generator.ToString().ShouldContain("QRR\r\n\r\n");
    }
}

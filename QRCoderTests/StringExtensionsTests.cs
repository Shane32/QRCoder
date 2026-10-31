using System.Threading;

namespace QRCoderTests;

public class StringExtensionsTests
{
    [Fact]
    public void can_append_numbers_using_invariant_culture()
    {
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var culture = new CultureInfo("de-DE");
        culture.NumberFormat.NegativeSign = "~";

        try
        {
            Thread.CurrentThread.CurrentCulture = culture;
            var builder = new StringBuilder();
            builder.AppendInvariant(-42);
            builder.Append('|');
            builder.AppendInvariant(-1.5f);
            builder.Append('|');
            StringExtensions.AppendInvariant(builder, $"{-42}|{-1.5f:G7}|{12.5m:F2}");

            builder.ToString().ShouldBe("-42|-1.5|-42|-1.5|12.50");
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }
}

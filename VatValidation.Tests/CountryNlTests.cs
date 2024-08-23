namespace VatValidation.Tests;

public class CountryNlTests
{
	// formatted with leading zero after strip for national
	[Theory]
	[InlineData(true, "861994772B01", "NL861994772B01", "NL861994772B01", "861994772B01")]
	[InlineData(true, "86 1994772B01", "NL861994772B01", "NL861994772B01", "861994772B01")]
	[InlineData(false, "861994772C01", "NL861994772C01", "", "861994772C01")]
	[InlineData(false, "861994772B", "NL861994772B", "", "861994772B")]
	public void VatStripValidNational(bool expectedValid, string input, string expectNational, string expectVat, string expectedStriped)
	{
		var vat = new VatNumber(Countries.NL.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");

		var vatStrip = vat.VatStripped;
		Assert.Equal(expectedValid, vatStrip.Valid);
		Assert.Equal(expectedStriped, (string)vatStrip);
		Assert.Equal(vatStrip.Valid ? expectNational : expectedStriped, vatStrip.FormatNational);
		Assert.Equal(expectVat, vatStrip.FormatVat);
	}
}

namespace VatValidation.Tests;

public class CountryNlTests
{
	[Theory]
	[InlineData(true, "004495445B01", "NL004495445B01", "NL004495445B01", "004495445B01")]
	[InlineData(true, "NL004495445B01", "NL004495445B01", "NL004495445B01", "004495445B01")]
	[InlineData(true, "824155890B08", "NL824155890B08", "NL824155890B08", "824155890B08")]
	[InlineData(true, "824155890B01", "NL824155890B01", "NL824155890B01", "824155890B01")]
	[InlineData(false, "824155891B01", "824155891B01", "NL824155891B01", "824155891B01")]
	[InlineData(false, "NL824155891B01", "NL824155891B01", "NL824155891B01", "824155891B01")]
	[InlineData(false, "NL82x155891B01", "NL82x155891B01", "", "82155891B01")]
	[InlineData(false, "NL82x155891", "NL82x155891", "", "NL82155891")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat, string expectedStriped)
	{
		var vat = new VatNumber(Countries.NL.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
		Assert.Equal(expectVat, vat.FormatVat);
		Assert.Equal(expectedStriped, vat.FormatStripped);

		var vatStrip = vat.VatStripped;
		Assert.Equal(expectedValid, vatStrip.Valid);
		Assert.Equal(expectedStriped, (string)vatStrip);
		// Formatted if valid, otherwise original data
		if (!vatStrip.Valid)
			Assert.Equal(expectedStriped, vatStrip.FormatNational);
		Assert.Equal(expectVat, vatStrip.FormatVat);
	}

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

	[Theory]
	[InlineData(true, "861994772B01", "NL861994772B01")]
	[InlineData(true, "861994772B02", "NL861994772B02")]
	[InlineData(false, "861994772C02", "")]
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? "NL" : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

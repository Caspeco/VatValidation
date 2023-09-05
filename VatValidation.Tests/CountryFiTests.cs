namespace VatValidation.Tests;

public class CountryFiTests
{
	[Theory]
	[InlineData(true, "02017244", "0201724-4", "FI 02017244")]
	[InlineData(true, "0201724-4", "0201724-4", "FI 02017244")]
	[InlineData(false, "0201723-4", "0201723-4", "FI 02017234")]
	[InlineData(false, "02017234", "02017234", "FI 02017234")]
	[InlineData(false, "201723-4", "201723-4", "FI 2017234")]
	[InlineData(false, "2017234", "2017234", "FI 2017234")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat)
	{
		var vat = new VatNumber(Countries.FI.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
		Assert.Equal(expectVat, vat.FormatVat);
	}

	[Theory]
	[InlineData(true, "FI 0201724-4", "0201724-4")]
	[InlineData(true, "FI 02017244", "0201724-4")]
	[InlineData(true, "FI02017244", "0201724-4")]
	[InlineData(true, "02017244", "0201724-4")]
	[InlineData(false, "FI 0201723-4", "")]
	[InlineData(false, "FI 02017234", "")]
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? "FI" : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

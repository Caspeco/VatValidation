namespace VatValidation.Tests;

public class CountryDkTests
{
	[Theory]
	[InlineData(true, "25313763", "25 31 37 63", "DK 25313763")]
	[InlineData(true, "25 31 37 63", "25 31 37 63", "DK 25313763")]
	[InlineData(true, "29403473", "29 40 34 73", "DK 29403473")]
	[InlineData(false, "05 31 37 63", "05 31 37 63", "DK 05313763")]
	[InlineData(false, "05313763", "05313763", "DK 05313763")]
	[InlineData(false, "25 31 37 60", "25 31 37 60", "DK 25313760")]
	[InlineData(false, "25313760", "25313760", "DK 25313760")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat)
	{
		var vat = new VatNumber(Countries.DK.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
		Assert.Equal(expectVat, vat.FormatVat);
	}

	[Theory]
	[InlineData(true, "DK 25313763", "25 31 37 63")]
	[InlineData(true, "DK 25 31 37 63", "25 31 37 63")]
	[InlineData(true, "DK25313763", "25 31 37 63")]
	[InlineData(true, "25313763", "25 31 37 63")]
	[InlineData(true, "DK 29 40 34 73", "29 40 34 73")]
	[InlineData(false, "DK 25313762", "")]
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? "DK" : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

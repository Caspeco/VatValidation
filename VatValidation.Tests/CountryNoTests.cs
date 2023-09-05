namespace VatValidation.Tests;

public class CountryNoTests
{
	[Theory]
	[InlineData(true, "977074010", "977 074 010", "NO 977 074 010")]
	[InlineData(true, "977 074 010", "977 074 010", "NO 977 074 010")]
	[InlineData(false, "977074011", "977074011", "NO 977074011")]
	[InlineData(false, "977 074 011", "977 074 011", "NO 977 074 011")]
	[InlineData(false, "977 074 01", "977 074 01", "NO 977 074 01")]
	[InlineData(false, "97707401", "97707401", "NO 97707401")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat)
	{
		var vat = new VatNumber(Countries.NO.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
		Assert.Equal(expectVat, vat.FormatVat);
	}

	[Theory]
	[InlineData(true, "NO 977074010", "977 074 010")]
	[InlineData(true, "NO 977 074 010", "977 074 010")]
	[InlineData(true, "NO 977 074 010MVA", "977 074 010")]
	[InlineData(true, "NO977074010", "977 074 010")]
	[InlineData(true, "977074010", "977 074 010")]
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? "NO" : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

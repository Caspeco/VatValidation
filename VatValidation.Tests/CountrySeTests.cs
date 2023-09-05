namespace VatValidation.Tests;

public class CountrySeTests
{
	[Theory]
	[InlineData(true, "1010101010", "101010-1010", "SE 1010101010 01")]
	[InlineData(true, "5566778899", "556677-8899", "SE 5566778899 01")]
	[InlineData(true, "556677-8899", "556677-8899", "SE 5566778899 01")]
	[InlineData(false, "5566778890", "5566778890", "SE 5566778890 01")]
	[InlineData(false, "556677-8890", "556677-8890", "SE 5566778890 01")]
	[InlineData(false, "55667788", "55667788", "SE 55667788 01")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat)
	{
		var vat = new VatNumber(Countries.SE.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
		Assert.Equal(expectVat, vat.FormatVat);
	}

	[Theory]
	[InlineData(true, "SE 1010101010 01", "101010-1010")]
	[InlineData(true, "SE101010101001", "101010-1010")]
	[InlineData(true, "SE556677889901", "556677-8899")]
	[InlineData(true, "5566778899", "556677-8899")]
	[InlineData(false, "5566778898", "")]
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? "SE" : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

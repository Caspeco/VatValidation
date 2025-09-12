namespace VatValidation.Tests;

public class CountrySeTests
{
	[Theory]
	[InlineData(true, "SE 1010101010 01", "101010-1010")]
	[InlineData(true, "SE101010101001", "101010-1010")]
	[InlineData(true, "SE556677889901", "556677-8899")]
	[InlineData(true, "SE 5566778501", "556677-8501")]
	[InlineData(true, "SE5566778501", "556677-8501")]
	[InlineData(false, "5566778898", "")]
	[InlineData(false, "0566988259", "")] // BE number
	[InlineData(false, "566988259", "")] // BE number, also matches NO
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		var vat1 = VatNumber.Get("SE", input);
		if (expectedValid)
			Assert.Equal(expectedValid, vat1.Valid);
		// vat1 could be valid b
		var vat2 = VatNumber.Get(null, input);
		Assert.Equal(expectedValid, vat2.Valid);
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? "SE" : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}

	[Theory]
	[InlineData(0, "404 833 048")]
	[InlineData(0, "023928237")]
	[InlineData(0, "7	3	2	8	2	9	3	2	0")]
	public void LuhnTest(int expected, VatNumber input)
	{
		Assert.Equal(expected, Countries.CountryBase.LuhnSum([0, .. input.GetInts()]));
	}
}

namespace VatValidation.Tests;

public class CountryEeTests
{
	[Theory]
	[InlineData(true, "100931558", "100931558", "EE 100931558")]
	[InlineData(true, "100 931 558", "100931558", "EE 100931558")]
	[InlineData(true, "100594102", "100594102", "EE 100594102")]
	[InlineData(false, "100594103", "100594103", "EE 100594103")]
	[InlineData(false, "999999999", "999999999", "EE 999999999")]
	[InlineData(false, "999 999 999", "999 999 999", "EE 999 999 999")]
	[InlineData(false, "999 999 99", "999 999 99", "EE 999 999 99")]
	[InlineData(false, "99999999", "99999999", "EE 99999999")]
	[InlineData(false, "099999999", "099999999", "EE 099999999")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat)
	{
		var vat = new VatNumber(Countries.EE.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
		Assert.Equal(expectVat, vat.FormatVat);
	}

	[Theory]
	[InlineData(true, "EE 100931558", "100931558")]
	[InlineData(true, "EE 100 931 558", "100931558")]
	[InlineData(false, "EE 999 999 999", "")]
	[InlineData(false, "EE999999990", "")]
	[InlineData(true, "EE 100594102", "100594102")]
	[InlineData(true, "100931558", "100931558")]
	[InlineData(true, "100594102", "100594102")] // matches EE & NO
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? nameof(Countries.EE) : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

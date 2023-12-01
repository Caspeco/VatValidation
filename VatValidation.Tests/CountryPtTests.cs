namespace VatValidation.Tests;

public class CountryPtTests
{
	[Theory]
	[InlineData(true, "999999990", "999999990", "PT 999999990")]
	[InlineData(true, "287024008", "287024008", "PT 287024008")]
	[InlineData(true, "501442600", "501442600", "PT 501442600")]
	[InlineData(false, "999999999", "999999999", "PT 999999999")]
	[InlineData(false, "999 999 999", "999 999 999", "PT 999 999 999")]
	[InlineData(false, "999 999 99", "999 999 99", "PT 999 999 99")]
	[InlineData(false, "99999999", "99999999", "PT 99999999")]
	[InlineData(false, "099999999", "099999999", "PT 099999999")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat)
	{
		var vat = new VatNumber(Countries.PT.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
		Assert.Equal(expectVat, vat.FormatVat);
	}

	[Theory]
	[InlineData(true, "PT 999999990", "999999990")]
	[InlineData(true, "PT 999 999 990", "999999990")]
	[InlineData(false, "PT 999 999 999", "")]
	[InlineData(true, "PT999999990", "999999990")]
	[InlineData(true, "999999990", "999999990")]
	[InlineData(true, "PT 287024008", "287024008")]
	[InlineData(true, "PT 501442600", "501442600")]
	[InlineData(true, "287024008", "287024008")]
	[InlineData(true, "501442600", "501442600")]
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? nameof(Countries.PT) : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

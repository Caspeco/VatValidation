namespace VatValidation.Tests;

public class CountryBeTests
{
	[Theory]
	[InlineData(true, "0566.988.259", "0566.988.259", "BE 0566988259", "0566988259")]
	[InlineData(true, "0566988259", "0566.988.259", "BE 0566988259", "0566988259")]
	[InlineData(true, "566.988.259", "566.988.259", "BE 0566988259", "0566988259")]
	[InlineData(false, "0566988258", "0566988258", "BE 0566988258", "0566988258")]
	[InlineData(false, "0566.988.258", "0566.988.258", "BE 0566988258", "0566988258")]
	[InlineData(false, "566.988.258", "566.988.258", "BE 0566988258", "0566988258")]
	[InlineData(true, "0427.155.930", "0427.155.930", "BE 0427155930", "0427155930")]
	[InlineData(true, "427.155.930", "427.155.930", "BE 0427155930", "0427155930")]
	[InlineData(false, "66988259", "66988259", "BE 66988259", "66988259")]
	[InlineData(false, "66.988.259", "66.988.259", "BE 66988259", "66988259")]
	public void VatTest(bool expectedValid, string input, string expectNational, string expectVat, string expectedStriped)
	{
		var vat = new VatNumber(Countries.BE.Instance, input);
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
	[InlineData(true, "0566.988.259", "0566.988.259", "BE 0566988259", "0566988259")]
	[InlineData(true, "0566988259", "0566.988.259", "BE 0566988259", "0566988259")]
	[InlineData(true, "566.988.259", "0566.988.259", "BE 0566988259", "0566988259")]
	[InlineData(true, "0427.155.930", "0427.155.930", "BE 0427155930", "0427155930")]
	[InlineData(true, "427 155 930", "0427.155.930", "BE 0427155930", "0427155930")]
	[InlineData(false, "427.155.931", "0427.155.931", "BE 0427155931", "0427155931")]
	[InlineData(false, "427 155 931", "0427.155.931", "BE 0427155931", "0427155931")]
	public void VatStripValidNational(bool expectedValid, string input, string expectNational, string expectVat, string expectedStriped)
	{
		var vat = new VatNumber(Countries.BE.Instance, input);
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");

		var vatStrip = vat.VatStripped;
		Assert.Equal(expectedValid, vatStrip.Valid);
		Assert.Equal(expectedStriped, (string)vatStrip);
		Assert.Equal(vatStrip.Valid ? expectNational : expectedStriped, vatStrip.FormatNational);
		Assert.Equal(expectVat, vatStrip.FormatVat);
	}

	[Theory]
	[InlineData(true, "BE 0566988259", "0566.988.259")]
	[InlineData(true, "BE0566988259", "0566.988.259")]
	[InlineData(true, "BE0427155930", "0427.155.930")]
	[InlineData(true, "0427.155.930", "0427.155.930")]
	[InlineData(true, "427.155.930", "427.155.930")]
	[InlineData(true, "0427155930", "0427.155.930")]
	[InlineData(true, "427155930", "427.155.930")]
	[InlineData(false, "BE0566988258", "")]
	[InlineData(false, "0566988259", "")] // Also matching SE
	[InlineData(false, "566988259", "")] // Also matches NO
	public void TryParseTest(bool expectedValid, string input, string expectNational)
	{
		Assert.Equal(expectedValid, VatNumber.TryParse(input, out var vat));
		Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
		Assert.Equal(expectedValid ? "BE" : null, vat.CC);
		Assert.Equal(expectedValid, vat.Valid);
		Assert.Equal(expectNational, vat.FormatNational);
	}
}

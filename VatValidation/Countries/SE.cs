
namespace VatValidation.Countries;

/// <country>Sweden</country>
/// <testcases>
/// valid, in: 1010101010, national: 101010-1010, vat: SE 1010101010 01, stripped: 1010101010, vatstripped: SE 1010101010 01
/// valid, in: 5566778899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE 5566778899 01
/// valid, in: 556677-8899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE 5566778899 01
/// invalid, in: 556677-8899x, strippedvalid, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE 5566778899 01
/// invalid, in: 5566778890, national: 5566778890, vat: SE 5566778890 01, stripped: 5566778890, vatstripped: SE 5566778890 01
/// invalid, in: 556677-8890, national: 556677-8890, vat: SE 5566778890 01, stripped: 5566778890, vatstripped: SE 5566778890 01
/// invalid, in: 55667788, national: 55667788, vat: SE 55667788 01, stripped: 55667788, vatstripped: SE 55667788 01
/// valid, in: SE 556677-8899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE 5566778899 01
/// valid, in: SE556677-8899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE 5566778899 01
/// valid, in: SE 5566778899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE 5566778899 01
/// valid, in: SE5566778899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE 5566778899 01
/// valid, in: SE 556677-8501, national: 556677-8501, vat: SE 5566778501 01, stripped: 5566778501, vatstripped: SE 5566778501 01
/// valid, in: 5566778501, national: 556677-8501, vat: SE 5566778501 01, stripped: 5566778501, vatstripped: SE 5566778501 01
/// invalid, in: 5566778898, national: 5566778898, vat: SE 5566778898 01, stripped: 5566778898, vatstripped: SE 5566778898 01
/// valid, in: 0566988259, national: 056698-8259, vat: SE 0566988259 01, stripped: 0566988259, vatstripped: SE 0566988259 01, dontTryParse, comment: BE number
/// invalid, in: 566988259, national: 566988259, vat: SE 566988259 01, stripped: 566988259, vatstripped: SE 566988259 01, comment: BE number and also matches NO
/// </testcases>
public class SE : CountryBase
{
	private SE() { }
	public static ICountry Instance { get; } = new SE();

	public override bool Valid(VatNumber vat) => Valid(vat.GetIntsIfNoChars());

	internal static bool Valid(ReadOnlySpan<int> digits) => digits.Length == 10 && LuhnSum(digits) == 0;

	public override int MinLength => 10;
	public override int MinLengthVat => 14;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..6])}-{ToStr(d[6..])}");

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatStripped(vat)} 01";

	protected override string GetVatInner(string input)
	{
		var ints = GetIntsFromString(input);
		if (ints.Length == MinLength + 2 && input.EndsWith("01"))
		{
			input = input[2..];
			input = input[..^2];
		}
		return input;
	}
}

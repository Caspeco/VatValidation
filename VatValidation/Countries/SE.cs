
namespace VatValidation.Countries;

/// <country>Sweden</country>
/// <name>Momsnummer</name>
/// <shortname>Momsnr.</shortname>
/// <length>12, 10</length>
/// <checksum>luhn 10</checksum>
/// <status>Verified official</status>
/// <testcases>
/// valid, in: 1010101010, national: 101010-1010, vat: SE 1010101010 01, stripped: 1010101010, vatstripped: SE101010101001
/// valid, in: 5566778899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE556677889901
/// valid, in: 556677-8899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE556677889901
/// invalid, in: 556677-8899x, strippedvalid, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE556677889901
/// invalid, in: 5566778890, national: 5566778890, vat: SE 5566778890 01, stripped: 5566778890, vatstripped: SE556677889001
/// invalid, in: 556677-8890, national: 556677-8890, vat: SE 5566778890 01, stripped: 5566778890, vatstripped: SE556677889001
/// invalid, in: 55667788, national: 55667788, vat: SE 55667788 01, stripped: 55667788, vatstripped: SE5566778801
/// valid, in: SE 556677-8899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE556677889901
/// valid, in: SE556677-8899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE556677889901
/// valid, in: SE 5566778899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE556677889901
/// valid, in: SE5566778899, national: 556677-8899, vat: SE 5566778899 01, stripped: 5566778899, vatstripped: SE556677889901
/// valid, in: SE 556677-8501, national: 556677-8501, vat: SE 5566778501 01, stripped: 5566778501, vatstripped: SE556677850101
/// valid, in: 5566778501, national: 556677-8501, vat: SE 5566778501 01, stripped: 5566778501, vatstripped: SE556677850101
/// invalid, in: 5566778898, national: 5566778898, vat: SE 5566778898 01, stripped: 5566778898, vatstripped: SE556677889801
/// valid, in: 0566988259, national: 056698-8259, vat: SE 0566988259 01, stripped: 0566988259, vatstripped: SE056698825901, dontTryParse, comment: BE number
/// invalid, in: 566988259, national: 566988259, vat: SE 566988259 01, stripped: 566988259, vatstripped: SE56698825901, comment: BE number and also matches NO
/// </testcases>
public class SE : CountryBase
{
	private SE() { }
	public static ICountry Instance { get; } = new SE();

	protected override bool Valid(ReadOnlySpan<int> digits) => digits.Length == 10 && LuhnSum(digits) == 0;

	public override int MinLength => 10;
	public override int MinLengthVat => 14;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..6])}-{ToStr(d[6..])}");

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatStripped(vat)} 01";
	public override string FormatVatStripped(VatNumber vat) => $"{CC}{FormatStripped(vat)}01";

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

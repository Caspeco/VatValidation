
namespace VatValidation.Countries;

/// <country>Germany</country>
/// <name>USt-IdNr</name>
/// <shortname>USt-IdNr</shortname>
/// <length>8</length>
/// <checksum>mod 11, mod 10, ISO 7064</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: DE115235681, national: 115235681, vat: DE 115235681, stripped: 115235681, vatstripped: DE115235681, comment: Volkswagen AG Legal
/// valid, in: 115 235 681, national: 115235681, vat: DE 115235681, stripped: 115235681, vatstripped: DE115235681, dontTryParse, comment: FR conflict
/// valid, in: DE129274202, national: 129274202, vat: DE 129274202, stripped: 129274202, vatstripped: DE129274202, comment: Siemens AG
/// valid, in: 119511515, national: 119511515, vat: DE 119511515, stripped: 119511515, vatstripped: DE119511515, dontTryParse, comment: LT conflict
/// valid, in: 404 833 048, national: 404833048, vat: DE 404833048, stripped: 404833048, vatstripped: DE404833048, dontTryParse, comment: FR conflict
/// valid, in: 121 000 015, national: 121000015, vat: DE 121000015, stripped: 121000015, vatstripped: DE121000015, dontTryParse, comment: valid as LT
/// invalid, in: DE115235682, national: DE115235682, vat: DE 115235682, stripped: 115235682, vatstripped: DE115235682
/// invalid, in: 115235682, national: 115235682, vat: DE 115235682, stripped: 115235682, vatstripped: DE115235682
/// invalid, in: 115 235 682, national: 115 235 682, vat: DE 115235682, stripped: 115235682, vatstripped: DE115235682
/// invalid, in: 12345678, national: 12345678, vat: DE 12345678, stripped: 12345678, vatstripped: DE12345678
/// </testcases>
public class DE : CountryBase
{
	private DE() { }
	public static ICountry Instance { get; } = new DE();

	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);

	protected override bool Valid(ReadOnlySpan<int> digits) => Valid(digits.ToArray());
	internal static bool Valid(int[] digits) => digits.Length == 9 &&
		ValidateChecksum(digits);

	private static int ZeroAs10(int v) => v == 0 ? 10 : v;
	private static bool ValidateChecksum(int[] digits)
		// or calc idx 0-7 and verify (11 - p) % 10 == digits[8]
		=> digits.Aggregate(10, (p, d) => ZeroAs10((p + d) % 10) * 2 % 11) == 2;
}

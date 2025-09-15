
namespace VatValidation.Countries;

/// <country>Luxembourg</country>
/// <name>Numéro d'identification à la taxe sur la valeur ajoutéee</name>
/// <shortname>No. TVA</shortname>
/// <length>8</length>
/// <checksum>mod 89</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: 15027442, national: 15027442, vat: LU 15027442, stripped: 15027442, vatstripped: LU15027442
/// valid, in: LU15027442, national: 15027442, vat: LU 15027442, stripped: 15027442, vatstripped: LU15027442
/// invalid, in: 15027443, national: 15027443, vat: LU 15027443, stripped: 15027443, vatstripped: LU15027443
/// invalid, in: LU15027443, national: LU15027443, vat: LU 15027443, stripped: 15027443, vatstripped: LU15027443
/// </testcases>
public class LU : CountryBase
{
	/*
	 https://fedil.lu/en/members/ef0c5bb5-ece6-e611-80f5-c4346bac8ce8/
	 Test number: https://bv.lorvent.in/validators/vat/
	 */
	private LU() { }
	public static ICountry Instance { get; } = new LU();

	public override int MinLength => 8;

	protected override bool Valid(ReadOnlySpan<int> digits)
		=> digits.Length == 8
		&& CalcMod([.. digits[0..6]], 89) == digits[6] * 10 + digits[7];

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);
}

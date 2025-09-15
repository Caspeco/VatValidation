
namespace VatValidation.Countries;

/// <country>Italy</country>
/// <name>Imposta sul valore aggiunto</name>
/// <shortname>Partita IVA</shortname>
/// <length>11</length>
/// <checksum>luhn 10</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: 07643520567, national: 07643520567, vat: IT07643520567, stripped: 07643520567, vatstripped: IT07643520567
/// valid, in: 0764 3520 56 7, national: 07643520567, vat: IT07643520567, stripped: 07643520567, vatstripped: IT07643520567
/// valid, in: IT07643520567, national: 07643520567, vat: IT07643520567, stripped: 07643520567, vatstripped: IT07643520567
/// valid, in: IT 0764 3520 56 7, national: 07643520567, vat: IT07643520567, stripped: 07643520567, vatstripped: IT07643520567
/// valid, in: 00743110157, national: 00743110157, vat: IT00743110157, stripped: 00743110157, vatstripped: IT00743110157
/// valid, in: 99999999990, national: 99999999990, vat: IT99999999990, stripped: 99999999990, vatstripped: IT99999999990
/// invalid, in: 07643520568, national: 07643520568, vat: IT07643520568, stripped: 07643520568, vatstripped: IT07643520568
/// invalid, in: 99999999993, national: 99999999993, vat: IT99999999993, stripped: 99999999993, vatstripped: IT99999999993, dontTryParse, comment: valid as LV
/// invalid, in: 99999999999, national: 99999999999, vat: IT99999999999, stripped: 99999999999, vatstripped: IT99999999999
/// invalid, in: 0764 3520 56 8, national: 0764 3520 56 8, vat: IT07643520568, stripped: 07643520568, vatstripped: IT07643520568
/// invalid, in: 00743110158, national: 00743110158, vat: IT00743110158, stripped: 00743110158, vatstripped: IT00743110158
/// valid, in: 16117519997, national: 16117519997, vat: IT16117519997, stripped: 16117519997, vatstripped: IT16117519997, dontTryParse, comment: LV testcase valid as IT
/// </testcases>
public class IT : CountryBase
{
	private IT() { }
	public static ICountry Instance { get; } = new IT();

	public override int MinLength => 11;

	/*
	https://it.wikipedia.org/wiki/Partita_IVA
	*/

	private static readonly HashSet<int> _tabellaDegliUffici = [100, 120, 121, 888, 999];
	protected override bool Valid(ReadOnlySpan<int> digits) => FormatValid(digits) && LuhnSum([0, .. digits]) == 0;
	private static bool FormatValid(ReadOnlySpan<int> digits)
		=> digits.Length == 11 && (
		digits[7] == 0 || // 000-099
		_tabellaDegliUffici.Contains(ToInt([.. digits[7..10]])));

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);
	public override string FormatVat(VatNumber vat) => FormatVatStripped(vat);
}

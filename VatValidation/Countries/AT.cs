
namespace VatValidation.Countries;

/// <country>Austria</country>
/// <name>Umsatzsteuer-Identifikationsnummer</name>
/// <shortname>UID-Nummer</shortname>
/// <length>9</length>
/// <checksum>U + luhn(+4)</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: ATU10223006, national: ATU10223006, vat: ATU10223006, stripped: ATU10223006, vatstripped: ATU10223006
/// valid, in: U10223006, national: ATU10223006, vat: ATU10223006, stripped: ATU10223006, vatstripped: ATU10223006, dontTryParse, comment: ES conflict
/// valid, in: ATU13585627, national: ATU13585627, vat: ATU13585627, stripped: ATU13585627, vatstripped: ATU13585627
/// valid, in: ATU66059506, national: ATU66059506, vat: ATU66059506, stripped: ATU66059506, vatstripped: ATU66059506
/// valid, in: ATU42403001, national: ATU42403001, vat: ATU42403001, stripped: ATU42403001, vatstripped: ATU42403001
/// invalid, in: ATU10223005, national: ATU10223005, vat: ATU10223005, stripped: ATU10223005, vatstripped: ATU10223005
/// invalid, in: 10223006, national: 10223006, vat: 10223006, stripped: 10223006, vatstripped: 10223006
/// </testcases>
public class AT : CountryBase
{
	private AT() { }
	public static ICountry Instance { get; } = new AT();

	/*
	 https://www.bmf.gv.at/dam/jcr:d6794f8f-d321-43df-9840-1a841f9bf5dc/BMF_UID_Konstruktionsregeln_Stand_November%202020.pdf
	 https://web.archive.org/web/20241001123106/https://www.bmf.gv.at/dam/jcr:9f9f8d5f-5496-4886-aa4f-81a4e39ba83e/BMF_UID_Konstruktionsregeln.pdf
	 */

	public override int MinLength => 9;

	public override bool Valid(VatNumber vat) =>
		((string)vat).Length >= 9 &&
		(((string)vat)[0] == 'U' || ((string)vat).StartsWith("ATU")) &&
		Valid(vat.GetInts());
	protected override bool Valid(ReadOnlySpan<int> digits) => digits.Length == 8 &&
		LuhnSum([0, .. digits[..^1]], 4) == digits[^1];

	public override string FormatNational(VatNumber vat) => Valid(vat) ? $"{CC}U{ToStr(vat.GetInts())}" : (string)vat;
	public override string FormatStripped(VatNumber vat) => FormatNational(vat);
	public override string FormatVat(VatNumber vat) => FormatNational(vat);
	public override string FormatVatStripped(VatNumber vat) => FormatNational(vat);
}

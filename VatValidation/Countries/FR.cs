
namespace VatValidation.Countries;

/// <country>France</country>
/// <name>Numéro de TVA intracommunautaire / SIREN</name>
/// <shortname>N° TVA</shortname>
/// <length>11, 9</length>
/// <checksum>mod 97, luhn 10</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: 404 833 048, national: 404 833 048, vat: FR83 404 833 048, stripped: 404833048, vatstripped: FR83404833048, dontTryParse, comment: DE conflict
/// valid, in: FR 83 404 833 048, national: 404 833 048, vat: FR83 404 833 048, stripped: 404833048, vatstripped: FR83404833048
/// valid, in: 83 404 833 048, national: 404 833 048, vat: FR83 404 833 048, stripped: 404833048, vatstripped: FR83404833048
/// valid, in: FR70932035157, national: 932 035 157, vat: FR70 932 035 157, stripped: 932035157, vatstripped: FR70932035157
/// valid, in: 356000000, national: 356 000 000, vat: FR39 356 000 000, stripped: 356000000, vatstripped: FR39356000000
/// valid, in: 712049618, national: 712 049 618, vat: FR25 712 049 618, stripped: 712049618, vatstripped: FR25712049618
/// valid, in: 115235681, national: 115 235 681, vat: FR25 115 235 681, stripped: 115235681, vatstripped: FR25115235681, dontTryParse, comment: Valid as DE
/// invalid, in: 28712049619, national: 28712049619, vat: FR 28712049619, stripped: 28712049619, vatstripped: FR28712049619
/// invalid, in: 1712049619, national: 1712049619, vat: FR 1712049619, stripped: 1712049619, vatstripped: FR1712049619
/// invalid, in: 712049619, national: 712049619, vat: FR 712049619, stripped: 712049619, vatstripped: FR712049619
/// invalid, in: 71204961, national: 71204961, vat: FR 71204961, stripped: 71204961, vatstripped: FR71204961
/// invalid, in: FR28 712049619, national: 28712049619, vat: FR 28712049619, stripped: 28712049619, vatstripped: FR28712049619
/// </testcases>
public class FR : CountryBase
{
	private FR() { }
	public static ICountry Instance { get; } = new FR();

	protected override bool Valid(ReadOnlySpan<int> digits) => digits.Length == 9 ? ValidSiren(digits) : ValidMod97(digits);

	public override int MinLength => 9;
	public override int MinLengthVat => 13;

	public override string FormatStripped(VatNumber vat) => ToStr(GetSiren(vat.GetInts()));
	public override string FormatNational(VatNumber vat) => FormatNational(vat.GetInts());

	private ReadOnlySpan<int> GetSiren(ReadOnlySpan<int> d) => Valid(d) ? GetSirenValid(d) : d;
	private string FormatNational(ReadOnlySpan<int> d) => Valid(d) ? FormatSiren(GetSirenValid(d)) : ToStr(d);
	private static ReadOnlySpan<int> GetSirenValid(ReadOnlySpan<int> d) => d[^9..];
	private static string FormatSiren(ReadOnlySpan<int> d) => $"{ToStr(d[0..3])} {ToStr(d[3..6])} {ToStr(d[6..9])}";

	private string FormatKey(ReadOnlySpan<int> digits) =>
		Valid(digits) ?
		digits.Length switch
		{
			11 => ToStr(digits[0..2]),
			9 => $"{CalcKey(digits):00}",
			_ => "",
		} : "";
	public override string FormatVat(VatNumber vat) => FormatVat(vat.GetInts());

	public override string FormatVatStripped(VatNumber vat) => FormatVatStripped(vat.GetInts());

	private string FormatVat(ReadOnlySpan<int> d) => $"{CC}{FormatKey(d)} {FormatNational(d)}";

	private string FormatVatStripped(ReadOnlySpan<int> d) => $"{CC}{FormatKey(d)}{ToStr(GetSiren(d))}";

	/*
	https://fr.wikipedia.org/wiki/Code_Insee
	https://xml.insee.fr/schema/siret.html#controles
	https://annuaire-entreprises.data.gouv.fr/entreprise/checksum-932035157
	https://forum-des-professions-liberales.fr/actualite-bnc/tva/numero-de-tva-intracommunautaire/
	*/

	private static bool ValidSiren(ReadOnlySpan<int> digits) => digits.Length == 9 && LuhnSum([0, .. digits]) == 0;
	// 12 + 3 * mod97 alternative mod97 + "12"
	private static int CalcKey(ReadOnlySpan<int> digits) => CalcMod([.. digits, 1, 2], 97);

	private static bool ValidMod97(ReadOnlySpan<int> digits) => digits.Length == 11
		&& ValidSiren(digits[2..])
		&& CalcKey(digits[2..]) == digits[0] * 10 + digits[1];
}

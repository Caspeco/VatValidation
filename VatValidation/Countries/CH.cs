
namespace VatValidation.Countries;

/// <country>Switzerland</country>
/// <name>Numéro d'identification suisse des entreprises (IDE)</name>
/// <shortname>MWST/TVA/IVA</shortname>
/// <length>10</length>
/// <checksum>mod 11</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: CHE109322551, national: CHE-109.322.551, vat: CHE-109.322.551, stripped: CHE109322551, vatstripped: CHE109322551
/// valid, in: CHE123456788, national: CHE-123.456.788, vat: CHE-123.456.788, stripped: CHE123456788, vatstripped: CHE123456788
/// valid, in: E123456788, national: CHE-123.456.788, vat: CHE-123.456.788, stripped: CHE123456788, vatstripped: CHE123456788
/// invalid, in: 123456788, national: CHE-123.456.788, vat: CHE-123.456.788, stripped: CHE123456788, strippedvalid, vatstripped: CHE123456788, comment: only original format not valid
/// invalid, in: CHE123456789, national: CHE123456789, vat: CHE123456789, stripped: CHE123456789, vatstripped: CHE123456789
/// invalid, in: 123456787, national: 123456787, vat: 123456787, stripped: 123456787, vatstripped: 123456787, comment: ..89 is valid as PT
/// </testcases>
public class CH : CountryBase
{
	private CH() { }
	public static ICountry Instance { get; } = new CH();

	/*
	 Unique Enterprise Identification Number (UID)
	 eCH-0097 (http://www.ech.ch) Page 8
	 https://www.ech.ch/sites/default/files/imce/eCH-Dossier/eCH-Dossier_PDF_Publikationen/Hauptdokument/STAN_d_NUL_2021-07-02_eCH-0097_V5.2.0_Datenstandard%20Unternehmensidentifikation.pdf
	 https://www.bfs.admin.ch/bfs/en/home/registers/enterprise-register/enterprise-identification/uid-general.html
	 */

	public override int MinLength => 10;

	private static readonly int[] _multipliers = [5, 4, 3, 2, 7, 6, 5, 4, 1];

	public override bool Valid(VatNumber vat) =>
		((string)vat).Length >= 10 &&
		(((string)vat)[0] == 'E' || ((string)vat).StartsWith("CHE")) &&
		Valid(vat.GetInts());
	protected override bool Valid(ReadOnlySpan<int> digits) =>
		digits.Length == 9 &&
		ValidMod11([.. digits]);
	private static bool ValidMod11(int[] digits) =>
		(11 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) % 11 == 0;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{CC}E-{ToStr(d[0..3])}.{ToStr(d[3..6])}.{ToStr(d[6..9])}");
	public override string FormatVat(VatNumber vat) => FormatNational(vat);
	public override string FormatStripped(VatNumber vat) => Format(vat, Valid, d => $"{CC}E{ToStr(d)}");
	public override string FormatVatStripped(VatNumber vat) => FormatStripped(vat);
}

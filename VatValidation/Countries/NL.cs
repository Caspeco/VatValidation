namespace VatValidation.Countries;

public class NL : CountryBase
{
	private NL() { }
	public static ICountry Instance { get; } = new NL();

	public override bool Valid(VatNumber vat) => Valid((string?)vat ?? string.Empty);

	public override string CC => nameof(NL);
	public override int MinLength => 12;

	public override string FormatStripped(VatNumber vat) => string.Join("", ((string)vat).Where(charset.Contains))[^12..];
	public override string FormatNational(VatNumber vat) => Valid(vat) ? FormatVat(vat) : (string?)vat ?? string.Empty;

	public override string FormatVat(VatNumber vat) => FormatVat(FormatStripped(vat), vat);
	private string FormatVat(string striped, VatNumber vat) => striped.StartsWith(CC) ? striped : $"{CC}{FormatStripped(vat)}";

	private const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ+*";

	// https://business.gov.nl/regulation/using-checking-vat-numbers/
	// https://help.afas.nl/help/NL/SE/Fin_Config_VatIct_NrChck.htm
	// https://www.nalog.nl/en/baza-znanij/topic/chto-takoe-btw-nomer/
	// https://web.archive.org/web/20200920075207/http://kleineondernemer.nl/index.php/nieuw-btw-identificatienummer-vanaf-1-januari-2020-voor-eenmanszaken
	// https://github.com/DragonBe/vies/issues/93
	internal static bool Valid(string btw)
	{
		if (btw.Length == 14 && btw.StartsWith("NL"))
			btw = btw[2..];
		if (btw.Length != 12 || btw[^3] != 'B')
			return false;
		if (btw.Any(c => !charset.Contains(c)))
			return false;
		return ValidCommercial(GetIntsFromString(btw[..9])) || ValidPersonal(btw);
	}

	private static readonly int[] _multipliers = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, -1 };

	// includes old personal
	private static bool ValidCommercial(int[] digits) =>
		digits.Length == 9 && digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11 == 0;

	// new 2020 personal
	private static bool ValidPersonal(string btw) =>
		Convert.ToInt64(string.Join("", btw.Select(c => charset.IndexOf(c)))) % 97 == 1;
}

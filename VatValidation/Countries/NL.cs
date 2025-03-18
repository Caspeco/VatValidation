
namespace VatValidation.Countries;

/// <country>Netherlands</country>
/// <testcases>
/// valid, in: 004495445B01, national: NL004495445B01, stripped: 004495445B01, vat: NL004495445B01, vatstripped: NL004495445B01
/// valid, in: NL004495445B01, national: NL004495445B01, stripped: 004495445B01, vat: NL004495445B01, vatstripped: NL004495445B01
/// valid, in: 824155890B08, national: NL824155890B08, stripped: 824155890B08, vat: NL824155890B08, vatstripped: NL824155890B08
/// valid, in: 824155890B01, national: NL824155890B01, stripped: 824155890B01, vat: NL824155890B01, vatstripped: NL824155890B01
/// valid, in: NL000099998B57, national: NL000099998B57, stripped: 000099998B57, vat: NL000099998B57, vatstripped: NL000099998B57
/// invalid, in: NL000099998B01, national: NL000099998B01, stripped: 000099998B01, vat: NL000099998B01, vatstripped: NL000099998B01
/// invalid, in: 824155891B01, national: 824155891B01, stripped: 824155891B01, vat: NL824155891B01, vatstripped: NL824155891B01
/// invalid, in: NL824155891B01, national: NL824155891B01, stripped: 824155891B01, vat: NL824155891B01, vatstripped: NL824155891B01
/// invalid, in: NL82x155891B01, national: NL82x155891B01, stripped: 82155891B01, vat: , vatstripped:
/// invalid, in: NL82x155891, national: NL82x155891, stripped: NL82155891, vat: , vatstripped:
/// valid, in: 861994772B01, national: NL861994772B01, stripped: 861994772B01, vat: NL861994772B01, vatstripped: NL861994772B01
/// valid, in: 861994772B02, national: NL861994772B02, stripped: 861994772B02, vat: NL861994772B02, vatstripped: NL861994772B02
/// invalid, in: 861994772C01, national: 861994772C01, stripped: 861994772C01, vat: , vatstripped:
/// invalid, in: 861994772B, national: 861994772B, stripped: 861994772B, vat: , vatstripped:
/// invalid, in: 861994772C02, national: 861994772C02, stripped: 861994772C02, vat: , vatstripped:
/// </testcases>
public class NL : CountryBase
{
	private NL() { }
	public static ICountry Instance { get; } = new NL();

	public override bool Valid(VatNumber vat) => Valid(BtwStripp((string?)vat ?? string.Empty));

	public override int MinLength => 12;

	public override string FormatStripped(VatNumber vat) => BtwStripp(string.Join("", ((string)vat).Where(charset.Contains)));
	public override string FormatNational(VatNumber vat) => Valid(BtwStripp(vat)) ? FormatVat(vat) : (string?)vat ?? string.Empty;

	public override string FormatVat(VatNumber vat) => FormatStrippedVat(FormatStripped(vat));
	private string FormatStrippedVat(string stripped) => BtwFormatValid(stripped) ? $"{CC}{stripped}" : "";

	private const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ+*";

	private static string BtwStripp(string btw) => btw.Length > 12 && btw.StartsWith("NL") ? btw[2..] : btw;
	private static bool BtwFormatValid(string btw) => btw.Length == 12 &&
		btw[^3] == 'B' && btw.All(charset.Contains);

	// https://business.gov.nl/regulation/using-checking-vat-numbers/
	// https://help.afas.nl/help/NL/SE/Fin_Config_VatIct_NrChck.htm
	// https://www.nalog.nl/en/baza-znanij/topic/chto-takoe-btw-nomer/
	// https://web.archive.org/web/20200920075207/http://kleineondernemer.nl/index.php/nieuw-btw-identificatienummer-vanaf-1-januari-2020-voor-eenmanszaken
	// https://github.com/DragonBe/vies/issues/93
	private static bool Valid(string btwStripped) => BtwFormatValid(btwStripped)
		&& (ValidCommercial(GetIntsFromString(btwStripped[..9]).ToArray()) || ValidPersonal(btwStripped));

	private static readonly int[] _multipliers = [9, 8, 7, 6, 5, 4, 3, 2, -1];

	// includes old personal
	private static bool ValidCommercial(int[] digits) =>
		digits.Length == 9 && digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11 == 0;

	private static int CalcMod(IEnumerable<int> digits, int mod)
		=> digits.Aggregate((r, d) => (r * (d < 10 ? 10 : 100) + d) % mod);

	/// <summary>NL prefix is required to get correct mod97</summary>
	private static string ModPersonalFormat(string btw) => btw.Length == 12 ? $"NL{btw}" : btw;

	// new 2020 personal
	private static bool ValidPersonal(string btw) =>
		CalcMod(ModPersonalFormat(btw).Select(c => charset.IndexOf(c)), 97) == 1;
}

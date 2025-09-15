
namespace VatValidation.Countries;

// tests of non leading zero dosnt play well with <testcases>, left in legacy testcases
/// <country>Belgium</country>
/// <name>BTW identificatienummer</name>
/// <shortname>BTW-nr</shortname>
/// <length>10 (old 9)</length>
/// <checksum>mod 97</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: 0566.988.259, national: 0566.988.259, stripped: 0566988259, vat: BE 0566988259, vatstripped: BE0566988259, dontTryParse, comment: Also matching SE
/// valid, in: 0566988259, national: 0566.988.259, stripped: 0566988259, vat: BE 0566988259, vatstripped: BE0566988259, dontTryParse, comment: Also matching SE
/// valid, in: 0427.155.930, national: 0427.155.930, stripped: 0427155930, vat: BE 0427155930, vatstripped: BE0427155930
/// valid, in: 0427155930, national: 0427.155.930, stripped: 0427155930, vat: BE 0427155930, vatstripped: BE0427155930
/// invalid, in: 0566988258, national: 0566988258, stripped: 0566988258, vat: BE 0566988258, vatstripped: BE0566988258
/// valid, in: BE 0566988259, national: 0566.988.259, stripped: 0566988259, vat: BE 0566988259, vatstripped: BE0566988259
/// valid, in: BE0566988259, national: 0566.988.259, stripped: 0566988259, vat: BE 0566988259, vatstripped: BE0566988259
/// valid, in: BE0427155930, national: 0427.155.930, stripped: 0427155930, vat: BE 0427155930, vatstripped: BE0427155930
/// invalid, in: BE0566988258, national: BE0566988258, stripped: 0566988258, vat: BE 0566988258, vatstripped: BE0566988258
/// </testcases>
public class BE : CountryBase
{
	private BE() { }
	public static ICountry Instance { get; } = new BE();

	public override int MinLength => 9;

	public override string FormatStripped(VatNumber vat) => ToStr(FixLen(vat.GetInts()));

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..^6])}.{ToStr(d[^6..^3])}.{ToStr(d[^3..])}");

	private static ReadOnlySpan<int> FixLen(ReadOnlySpan<int> d) => d.Length == 9 ? new int[] { 0 }.Concat(d.ToArray()).ToArray() : d;

	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 9 || (d.Length == 10 && (d[0] == 0 || d[1] == 1));

	// https://www.fiducial.be/nl/news/Hoe-kunt-u-weten-of-uw-klant-u-een-correct-BTW-nummer-gaf
	protected override bool Valid(ReadOnlySpan<int> digits) =>
		ValidFormat(digits) && (
		Convert.ToInt32(ToStr(digits[..^2])) +
		Convert.ToInt32(ToStr(digits[^2..]))) % 97 == 0;
}

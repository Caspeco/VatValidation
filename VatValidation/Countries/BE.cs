namespace VatValidation.Countries;

public class BE : CountryBase
{
	private BE() { }
	public static ICountry Instance { get; } = new BE();

	public override bool Valid(VatNumber vat) => Valid(vat.GetIntsIfNoChars());

	public override int MinLength => 9;

	public override string FormatStripped(VatNumber vat) => ToStr(FixLen(vat.GetInts()));

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..^6])}.{ToStr(d[^6..^3])}.{ToStr(d[^3..])}");

	private static ReadOnlySpan<int> FixLen(ReadOnlySpan<int> d) => d.Length == 9 ? new int[] { 0 }.Concat(d.ToArray()).ToArray() : d;

	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 9 || (d.Length == 10 && (d[0] == 0 || d[1] == 1));

	// https://www.fiducial.be/nl/news/Hoe-kunt-u-weten-of-uw-klant-u-een-correct-BTW-nummer-gaf
	internal static bool Valid(ReadOnlySpan<int> digits) =>
		ValidFormat(digits) && (
		Convert.ToInt32(ToStr(digits[..^2])) +
		Convert.ToInt32(ToStr(digits[^2..]))) % 97 == 0;
}

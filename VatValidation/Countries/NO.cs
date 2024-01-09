namespace VatValidation.Countries;

public class NO : CountryBase
{
	private NO() { }
	public static ICountry Instance { get; } = new NO();

	public override bool Valid(VatNumber vat) => Valid(vat.GetIntsIfNoChars());

	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..3])} {ToStr(d[3..6])} {ToStr(d[6..9])}");

	public override string FormatVat(VatNumber vat) => ValidFormat(vat.GetInts().ToArray()) ? $"{CC} {FormatNational(vat)}" : "";

	private static readonly int[] _multipliers = [3, 2, 7, 6, 5, 4, 3, 2, 1];

	// https://vatstack.com/articles/norway-vat-number-validation
	private static bool ValidFormat(int[] d) => d.Length == 9 && (d[0] == 8 || d[0] == 9);
	internal static bool Valid(ReadOnlySpan<int> digits) => Valid(digits.ToArray());
	internal static bool Valid(int[] digits) => ValidFormat(digits) &&
		(11 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) % 11 == 0;
}

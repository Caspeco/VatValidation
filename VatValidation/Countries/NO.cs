namespace VatValidation.Countries;

public class NO : CountryBase
{
	private NO() { }
	public static ICountry Instance { get; } = new NO();

	public override bool Valid(VatNumber vat) => Valid(vat.GetInts());

	public override string CC => nameof(NO);
	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..3])} {ToStr(d[3..6])} {ToStr(d[6..9])}");

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatNational(vat)}";

	private static readonly int[] _multipliers = new int[] { 3, 2, 7, 6, 5, 4, 3, 2, 1 };

	internal static bool Valid(int[] digits) => digits.Length == 9 &&
		(11 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) % 11 == 0;
}
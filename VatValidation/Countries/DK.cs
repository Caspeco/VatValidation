namespace VatValidation.Countries;

public class DK : CountryBase
{
	private DK() { }
	public static ICountry Instance { get; } = new DK();

	public override bool Valid(VatNumber vat) => Valid(vat.GetInts());

	public override int MinLength => 8;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..2])} {ToStr(d[2..4])} {ToStr(d[4..6])} {ToStr(d[6..8])}");

	private static readonly int[] _multipliers = [2, 7, 6, 5, 4, 3, 2, 1];

	// https://da.wikipedia.org/wiki/Det_Centrale_Virksomhedsregister
	// https://datacvr.virk.dk/
	internal static bool Valid(ReadOnlySpan<int> digits) => Valid(digits.ToArray());
	internal static bool Valid(int[] digits) => digits.Length == 8 &&
		(digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) == 0;
}

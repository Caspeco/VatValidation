namespace VatValidation.Countries;

public class FI : CountryBase
{
	private FI() { }
	public static ICountry Instance { get; } = new FI();

	public override bool Valid(VatNumber vat) => Valid(vat.GetInts());

	public override string CC => nameof(FI);
	public override int MinLength => 8;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..7])}-{ToStr(d[7..])}");

	private static readonly int[] _multipliers = [7, 9, 10, 5, 8, 4, 2, 1];

	/// <summary>http://www.finlex.fi/sv/laki/ajantasa/2001/20010288</summary>
	internal static bool Valid(ReadOnlySpan<int> digits) => Valid(digits.ToArray());
	internal static bool Valid(int[] digits) => digits.Length == 8 &&
		(11 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) % 11 == 0;
}

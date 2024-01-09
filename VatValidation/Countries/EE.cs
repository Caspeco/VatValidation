namespace VatValidation.Countries;

public class EE : CountryBase
{
	private EE() { }
	public static ICountry Instance { get; } = new EE();

	public override bool Valid(VatNumber vat) => Valid(vat.GetIntsIfNoChars());

	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatNational(vat)}";

	private static readonly int[] _multipliers = [3, 7, 1, 3, 7, 1, 3, 7, 1];

	private static bool ValidFormat(int[] d) => d.Length == 9 && d[0] == 1 && d[1] == 0;

	/// <summary>Kaibemaksukohuslase (KMKR)</summary>
	internal static bool Valid(ReadOnlySpan<int> digits) => Valid(digits.ToArray());
	internal static bool Valid(int[] digits) => ValidFormat(digits) &&
		(10 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 10) % 10 == 0;
}


namespace VatValidation.Countries;

/// <country>Latvia</country>
/// <name>Pievienotās vērtības nodokļa reģistrācijas numurs</name>
/// <shortname>PVN</shortname>
/// <length>11</length>
/// <checksum>mod 11</checksum>
/// <status>No official source</status>
/// <testcases>
/// valid, in: 40003521600, national: 40003521600, vat: LV40003521600, stripped: 40003521600
/// valid, in: LV 4000 3521 600, national: 40003521600, vat: LV40003521600, stripped: 40003521600
/// invalid, in: 40003521601, national: 40003521601, vat: LV40003521601, stripped: 40003521601
/// invalid, in: 4000352160, national: 4000352160, vat: LV4000352160, stripped: 4000352160
/// invalid, in: 16117519997, national: 16117519997, vat: LV16117519997, stripped: 16117519997
/// </testcases>
public class LV : CountryBase
{
	private LV() { }
	public static ICountry Instance { get; } = new LV();

	public override int MinLength => 11;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);

	public override string FormatVat(VatNumber vat) => $"{CC}{FormatNational(vat)}";

	private static readonly int[] _multipliers = [9, 1, 4, 8, 3, 10, 2, 5, 7, 6, 1];

	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 11 && d[0] > 3;

	protected override bool Valid(ReadOnlySpan<int> digits) => ValidFormat(digits) && Valid(digits.ToArray());
	private static bool Valid(int[] digits) => digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11 == 3;
}

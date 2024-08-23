
namespace VatValidation.Countries;

/// <country>Norway</country>
/// <testcases>
/// valid, in: 977074010, national: 977 074 010, vat: NO 977 074 010, stripped: 977074010
/// valid, in: 977 074 010, national: 977 074 010, vat: NO 977 074 010, stripped: 977074010
/// invalid, in: 977 074 010x, strippedvalid, national: 977 074 010, vat: NO 977 074 010, stripped: 977074010
/// invalid, in: 977074011, national: 977074011, vat: NO 977074011, stripped: 977074011, vatstripped: NO 977074011
/// invalid, in: 977 074 011, national: 977 074 011, vat: NO 977 074 011, stripped: 977074011, vatstripped: NO 977074011
/// invalid, in: 977 074 01, national: 977 074 01, vat: , stripped: 97707401, vatstripped:
/// invalid, in: 97707401, national: 97707401, vat: , stripped: 97707401, vatstripped:
/// invalid, in: 777074016, national: 777074016, vat: , stripped: 777074016, vatstripped:
/// valid, in: 977 074 010MVA, national: 977 074 010, vat: NO 977 074 010, stripped: 977074010, vatstripped: NO 977 074 010
/// valid, in: NO 977 074 010MVA, national: 977 074 010, vat: NO 977 074 010, stripped: 977074010, vatstripped: NO 977 074 010
/// valid, in: NO977074010, national: 977 074 010, vat: NO 977 074 010, stripped: 977074010, vatstripped: NO 977 074 010
/// </testcases>
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
	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 9 && (d[0] == 8 || d[0] == 9);
	internal static bool Valid(ReadOnlySpan<int> digits) => ValidFormat(digits) && Valid(digits.ToArray());
	private static bool Valid(int[] digits) =>
		(11 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) % 11 == 0;
}

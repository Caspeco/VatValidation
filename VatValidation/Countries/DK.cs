
namespace VatValidation.Countries;

/// <country>Denmark</country>
/// <testcases>
/// valid, in: 25313763, national: 25 31 37 63, vat: DK 25313763, stripped: 25313763
/// valid, in: 25 31 37 63, national: 25 31 37 63, vat: DK 25313763, stripped: 25313763, vatstripped: DK 25313763
/// valid, in: 29403473, national: 29 40 34 73, vat: DK 29403473, stripped: 29403473, vatstripped: DK 29403473
/// invalid, in: 29403473x, strippedvalid, national: 29 40 34 73, vat: DK 29403473, stripped: 29403473, vatstripped: DK 29403473
/// invalid, in: 05 31 37 63, national: 05 31 37 63, vat: DK 05313763, stripped: 05313763, vatstripped: DK 05313763
/// invalid, in: 05313763, national: 05313763, vat: DK 05313763, stripped: 05313763, vatstripped: DK 05313763
/// invalid, in: 25 31 37 60, national: 25 31 37 60, vat: DK 25313760, stripped: 25313760, vatstripped: DK 25313760
/// invalid, in: 25313760, national: 25313760, vat: DK 25313760, stripped: 25313760, vatstripped: DK 25313760
/// valid, in: DK 25313763, national: 25 31 37 63, vat: DK 25313763, stripped: 25313763
/// valid, in: DK 25 31 37 63, national: 25 31 37 63, vat: DK 25313763, stripped: 25313763
/// valid, in: DK25313763, national: 25 31 37 63, vat: DK 25313763, stripped: 25313763, vatstripped: DK 25313763
/// valid, in: 25313763, national: 25 31 37 63, vat: DK 25313763, stripped: 25313763
/// valid, in: DK 29 40 34 73, national: 29 40 34 73, vat: DK 29403473, stripped: 29403473, vatstripped: DK 29403473
/// invalid, in: DK 25313762, national: DK 25313762, vat: DK 25313762, stripped: 25313762, vatstripped: DK 25313762
/// invalid, in: DK 25 31 37 62, national: DK 25 31 37 62, vat: DK 25313762, stripped: 25313762, vatstripped: DK 25313762
/// </testcases>
public class DK : CountryBase
{
	private DK() { }
	public static ICountry Instance { get; } = new DK();

	public override int MinLength => 8;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..2])} {ToStr(d[2..4])} {ToStr(d[4..6])} {ToStr(d[6..8])}");

	private static readonly int[] _multipliers = [2, 7, 6, 5, 4, 3, 2, 1];

	// https://da.wikipedia.org/wiki/Det_Centrale_Virksomhedsregister
	// https://datacvr.virk.dk/
	protected override bool Valid(ReadOnlySpan<int> digits) => Valid(digits.ToArray());
	internal static bool Valid(int[] digits) => digits.Length == 8 &&
		(digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) == 0;
}

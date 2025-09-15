
namespace VatValidation.Countries;

/// <country>Finland</country>
/// <name>Arvonlisäveronumero</name>
/// <shortname>ALV nro</shortname>
/// <length>8</length>
/// <checksum>mod 11-2</checksum>
/// <status>Verified official</status>
/// <testcases>
/// valid, in: 02017244, national: 0201724-4, vat: FI 02017244, stripped: 02017244, vatstripped: FI02017244
/// valid, in: 0201724-4, national: 0201724-4, vat: FI 02017244, stripped: 02017244, vatstripped: FI02017244
/// invalid, in: 02017244x, strippedvalid, national: 0201724-4, vat: FI 02017244, stripped: 02017244, vatstripped: FI02017244
/// invalid, in: 0201723-4, national: 0201723-4, vat: FI 02017234, stripped: 02017234, vatstripped: FI02017234
/// invalid, in: 02017234, national: 02017234, vat: FI 02017234, stripped: 02017234, vatstripped: FI02017234
/// invalid, in: 201723-4, national: 201723-4, vat: FI 2017234, stripped: 2017234, vatstripped: FI2017234
/// invalid, in: 2017234, national: 2017234, vat: FI 2017234, stripped: 2017234, vatstripped: FI2017234
/// valid, in: FI 0201724-4, national: 0201724-4, vat: FI 02017244, stripped: 02017244, vatstripped: FI02017244
/// valid, in: FI 02017244, national: 0201724-4, vat: FI 02017244, stripped: 02017244, vatstripped: FI02017244
/// valid, in: FI02017244, national: 0201724-4, vat: FI 02017244, stripped: 02017244, vatstripped: FI02017244
/// invalid, in: FI 0201723-4, national: FI 0201723-4, vat: FI 02017234, stripped: 02017234, vatstripped: FI02017234
/// invalid, in: FI 02017234, national: FI 02017234, vat: FI 02017234, stripped: 02017234, vatstripped: FI02017234
/// </testcases>
public class FI : CountryBase
{
	private FI() { }
	public static ICountry Instance { get; } = new FI();

	public override int MinLength => 8;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..7])}-{ToStr(d[7..])}");

	private static readonly int[] _multipliers = [7, 9, 10, 5, 8, 4, 2, 1];

	/// <summary>http://www.finlex.fi/sv/laki/ajantasa/2001/20010288</summary>
	protected override bool Valid(ReadOnlySpan<int> digits) => Valid(digits.ToArray());
	internal static bool Valid(int[] digits) => digits.Length == 8 &&
		(11 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 11) % 11 == 0;
}

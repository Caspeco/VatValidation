
namespace VatValidation.Countries;

/// <country>Estonia</country>
/// <testcases>
/// valid, in: 100931558, national: 100931558, vat: EE 100931558, stripped: 100931558, vatstripped: EE 100931558
/// valid, in: 100 931 558, national: 100931558, vat: EE 100931558, stripped: 100931558, vatstripped: EE 100931558
/// valid, in: 100594102, national: 100594102, vat: EE 100594102, stripped: 100594102, comment: matches both EE and NO
/// invalid, in: 100 594 102x, strippedvalid, national: 100594102, vat: EE 100594102, stripped: 100594102
/// invalid, in: 100594103, national: 100594103, vat: EE 100594103, stripped: 100594103, vatstripped: EE 100594103
/// invalid, in: 999999999, national: 999999999, vat: EE 999999999, stripped: 999999999, vatstripped: EE 999999999, dontTryParse
/// invalid, in: 999 999 999, national: 999 999 999, vat: EE 999 999 999, stripped: 999999999, vatstripped: EE 999999999, dontTryParse
/// invalid, in: 999 999 99, national: 999 999 99, vat: EE 999 999 99, stripped: 99999999, vatstripped: EE 99999999
/// invalid, in: 99999999, national: 99999999, vat: EE 99999999, stripped: 99999999, vatstripped: EE 99999999
/// invalid, in: 099999999, national: 099999999, vat: EE 099999999, stripped: 099999999, vatstripped: EE 099999999
/// valid, in: EE100931558, national: 100931558, vat: EE 100931558, stripped: 100931558, vatstripped: EE 100931558
/// valid, in: EE 100 931 558, national: 100931558, vat: EE 100931558, stripped: 100931558, vatstripped: EE 100931558
/// invalid, in: EE 999 999 999, national: EE 999 999 999, vat: EE EE 999 999 999, stripped: 999999999, vatstripped: EE 999999999
/// invalid, in: EE999999999, national: EE999999999, vat: EE EE999999999, stripped: 999999999, vatstripped: EE 999999999
/// valid, in: EE 100594102, national: 100594102, vat: EE 100594102, stripped: 100594102
/// </testcases>
public class EE : CountryBase
{
	private EE() { }
	public static ICountry Instance { get; } = new EE();

	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatNational(vat)}";

	private static readonly int[] _multipliers = [3, 7, 1, 3, 7, 1, 3, 7, 1];

	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 9 && d[0] == 1 && d[1] == 0;

	/// <summary>Kaibemaksukohuslase (KMKR)</summary>
	protected override bool Valid(ReadOnlySpan<int> digits) => ValidFormat(digits) && Valid(digits.ToArray());
	private static bool Valid(int[] digits) =>
		(10 - digits
		.Zip(_multipliers, (d, m) => d * m)
		.Sum() % 10) % 10 == 0;
}

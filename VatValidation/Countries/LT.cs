
namespace VatValidation.Countries;

/// <country>Lithuania</country>
/// <name>Pridėtinės vertės mokestis mokėtojo kodas</name>
/// <shortname>PVM kodas</shortname>
/// <length>9, 12</length>
/// <checksum>mod 11</checksum>
/// <status>No official source</status>
/// <testcases>
/// valid, in: 119511515, national: 119511515, vat: LT119511515, stripped: 119511515
/// valid, in: 100001919017, national: 100001919017, vat: LT100001919017, stripped: 100001919017
/// valid, in: 241102419, national: 241102419, vat: LT241102419, stripped: 241102419
/// valid, in: LT633 878 716, national: 633878716, vat: LT633878716, stripped: 633878716
/// invalid, in: 119511525, national: 119511525, vat: LT119511525, stripped: 119511525, dontTryParse
/// invalid, in: 100001919018, national: 100001919018, vat: LT100001919018, stripped: 100001919018
/// invalid, in: 11951151x, national: 11951151x, vat: LT11951151x, stripped: 11951151, vatstripped: LT11951151
/// </testcases>
public class LT : CountryBase
{
	private LT() { }
	public static ICountry Instance { get; } = new LT();

	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);

	public override string FormatVat(VatNumber vat) => $"{CC}{FormatNational(vat)}";

	private static bool ValidFormat(ReadOnlySpan<int> d) =>
		(d.Length == 9 && d[7] == 1 && (d[0] != 8 && d[0] != 9 /* dont accept conflicting NO */)) ||
		(d.Length == 12 && d[10] == 1);

	private static int SumMod11(int[] d, int idxWeightOffset) => d.Select((v, i) => v * ((i + idxWeightOffset) % 9 + 1)).Sum() % 11;
	protected override bool Valid(ReadOnlySpan<int> digits)
	{
		if (!ValidFormat(digits))
			return false;

		var d = digits[..^1].ToArray();
		var sum = SumMod11(d, 0);
		if (sum == 10)
			sum = SumMod11(d, 2);
		return sum % 10 == digits[^1];
	}
}

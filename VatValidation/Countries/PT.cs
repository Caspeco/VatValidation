
namespace VatValidation.Countries;

/// <country>Portugal</country>
/// <testcases>
/// valid, in: 999999990, national: 999999990, stripped: 999999990, vat: PT 999999990, vatstripped: PT 999999990
/// valid, in: 287024008, national: 287024008, stripped: 287024008, vat: PT 287024008, vatstripped: PT 287024008
/// valid, in: 287 024 008, national: 287024008, stripped: 287024008, vat: PT 287024008, vatstripped: PT 287024008
/// valid, in: 501442600, national: 501442600, stripped: 501442600, vat: PT 501442600, vatstripped: PT 501442600
/// invalid, in: 999999999, national: 999999999, stripped: 999999999, vat: PT 999999999, vatstripped: PT 999999999, dontTryParse
/// invalid, in: 999 999 999, national: 999 999 999, stripped: 999999999, vat: PT 999 999 999, vatstripped: PT 999999999, dontTryParse
/// invalid, in: 999 999 99, national: 999 999 99, stripped: 99999999, vat: PT 999 999 99, vatstripped: PT 99999999
/// invalid, in: 099999999, national: 099999999, stripped: 099999999, vat: PT 099999999, vatstripped: PT 099999999
/// valid, in: PT 999999990, national: 999999990, stripped: 999999990, vat: PT 999999990, vatstripped: PT 999999990
/// valid, in: PT 999 999 990, national: 999999990, stripped: 999999990, vat: PT 999999990, vatstripped: PT 999999990
/// valid, in: PT999999990, national: 999999990, stripped: 999999990, vat: PT 999999990, vatstripped: PT 999999990
/// valid, in: PT 287024008, national: 287024008, stripped: 287024008, vat: PT 287024008, vatstripped: PT 287024008
/// valid, in: PT 501442600, national: 501442600, stripped: 501442600, vat: PT 501442600, vatstripped: PT 501442600
/// invalid, in: PT 999 999 999, national: PT 999 999 999, stripped: 999999999, vat: PT PT 999 999 999, vatstripped: PT 999999999
/// </testcases>
public class PT : CountryBase
{
	private PT() { }
	public static ICountry Instance { get; } = new PT();

	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatNational(vat)}";

	private static readonly int[] _multipliers = [9, 8, 7, 6, 5, 4, 3, 2];

	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 9 && d[0] != 0;

	// https://pt.wikipedia.org/wiki/N%C3%BAmero_de_identifica%C3%A7%C3%A3o_fiscal
	protected override bool Valid(ReadOnlySpan<int> digits) => ValidFormat(digits) && Valid(digits.ToArray());
	private static bool Valid(int[] digits)
	{
		int digitoVerificacao = 11 - (digits
			.Zip(_multipliers, (d, m) => d * m)
			.Sum() % 11);
		if (digitoVerificacao > 9)
			digitoVerificacao = 0;
		// retornar validação
		return digitoVerificacao == digits[^1];
	}
}

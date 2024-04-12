namespace VatValidation.Countries;

public class PT : CountryBase
{
	private PT() { }
	public static ICountry Instance { get; } = new PT();

	public override bool Valid(VatNumber vat) => Valid(vat.GetIntsIfNoChars());

	public override int MinLength => 9;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, ToStr);

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatNational(vat)}";

	private static readonly int[] _multipliers = [9, 8, 7, 6, 5, 4, 3, 2];

	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 9 && d[0] != 0;

	// https://pt.wikipedia.org/wiki/N%C3%BAmero_de_identifica%C3%A7%C3%A3o_fiscal
	internal static bool Valid(ReadOnlySpan<int> digits) => ValidFormat(digits) && Valid(digits.ToArray());
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

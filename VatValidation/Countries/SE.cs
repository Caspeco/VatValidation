namespace VatValidation.Countries;

public class SE : CountryBase
{
	private SE() { }
	public static ICountry Instance { get; } = new SE();

	public override bool Valid(VatNumber vat) => Valid(vat.GetInts());

	internal static bool Valid(int[] digits) => digits.Length == 10 && LuhnSum(digits) == 0;

	public override string CC => nameof(SE);
	public override int MinLength => 10;
	public override int MinLengthVat => 14;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..6])}-{ToStr(d[6..])}");

	public override string FormatVat(VatNumber vat) => $"{CC} {FormatStripped(vat)} 01";
}

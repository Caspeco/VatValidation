
namespace VatValidation.Countries;

/// <country>Spain</country>
/// <testcases>
/// valid, in: A58818501, national: A58818501, vat: ES A58818501, stripped: A58818501
/// invalid, in: A58818502, national: A58818502, vat: ES A58818502, stripped: A58818502
/// invalid, in: A5881850x, national: A5881850x, vat: ES A5881850x, stripped: A5881850, vatstripped: ES A5881850
/// valid, in: B12345674, national: B12345674, vat: ES B12345674, stripped: B12345674
/// valid, in: S1454158E, national: S1454158E, vat: ES S1454158E, stripped: S1454158E
/// </testcases>
public class ES : CountryBase
{
	private ES() { }
	public static ICountry Instance { get; } = new ES();

	public override bool Valid(VatNumber vat) => Valid((string?)vat ?? string.Empty);

	public override int MinLength => 9;

	public override string FormatStripped(VatNumber vat) => Stripp((string?)vat ?? string.Empty);
	public override string FormatNational(VatNumber vat) => (Valid(vat) ? FormatStripped(vat) : (string?)vat) ?? string.Empty;

	/// <summary>NIF-IVA</summary>
	public override string FormatVat(VatNumber vat) => $"{CC} {vat}";

	private static string Stripp(string s) => string.Join("", s.Where(keepChars.Contains));

	const string legalFormLetter = "ABCDEFGHJNPQRSUVW";
	const string keepChars = $"0123456789{legalFormLetter}I";

	private static bool FormatValid(string nif) => nif.Length == 9
		&& legalFormLetter.Contains(nif[0]) && GetIntsFromString(nif).Length >= 7;

	// https://www.arintass.com/what-is-the-cif-nif-and-nif-iva/
	internal static bool Valid(string nif)
	{
		if (!FormatValid(nif))
			return false;

		var ints = GetIntsFromString(nif);

		var sum = LuhnSum(ints[..7]);
		return "NPQRSW".Contains(nif[0])
			? nif[^1] == "JABCDEFGHI"[sum]
			: ints[^1] == sum;
	}
}

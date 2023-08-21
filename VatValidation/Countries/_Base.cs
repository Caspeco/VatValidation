using System.Reflection;

namespace VatValidation.Countries;

public abstract class CountryBase : ICountry
{
	/// <summary>Validate VAT number for Country</summary>
	public abstract bool Valid(VatNumber vat);

	public virtual string FormatStripped(VatNumber vat) => ToStr(vat.GetInts());

	public abstract string FormatNational(VatNumber vat);

	public virtual string FormatVat(VatNumber vat) => $"{CC} {FormatStripped(vat)}";

	public abstract string CC { get; }
	public abstract int MinLength { get; }
	/// <summary>Length including country code any anything else required in VAT format</summary>
	public virtual int MinLengthVat => MinLength + 2;

	private static string Format(int[] d, VatNumber vat, Func<int[], bool> v, Func<int[], string> f) => v(d) ? f(d) : vat;
	protected static string Format(VatNumber vat, Func<int[], bool> valid, Func<int[], string> formatValid) =>
		Format(vat.GetInts(), vat, valid, formatValid);

	public static int[] GetIntsFromString(string s) => s.Select(c => c - '0')
		.Where(x => 0 <= x && x <= 9).ToArray();

	public static string ToStr(int[] ints) => string.Join("", ints);

	protected static int LuhnSum(int[] digits)
	{
		int sm = 0;
		for (int i = digits.Length - 1; i >= 0; i--)
		{
			int n = digits[i];
			sm += ((i & 1) == 0) ? (n > 4 ? n * 2 - 9 : n * 2) : n;
		}

		return (10 - sm % 10) % 10;
	}

	private static IEnumerable<Type> GetICountryTypes() => Assembly
		.GetExecutingAssembly() //In this case is the same assembly, if not Load the correct one
		.GetTypes()
		.Where(typeof(ICountry).IsAssignableFrom);

	private static IEnumerable<ICountry> GetICountryInstances() => GetICountryTypes()
		.Select(t => new { t, instance = t.GetProperty(nameof(SE.Instance), BindingFlags.Static | BindingFlags.Public) })
		.Where(a => a.instance is not null)
		.Select(a => (ICountry?)(a.instance?.GetValue(null)))
		.Where(c => c is not null).Select(c => c!);

	public static readonly Dictionary<string, ICountry> CcInstances = GetICountryInstances().ToDictionary(c => c.CC, StringComparer.OrdinalIgnoreCase)!;

	public static ICountry? GetCountryInstance(string cc) => CcInstances.TryGetValue(cc, out var instance) ? instance : null;
}

using System.Reflection;

namespace VatValidation.Countries;

public abstract class CountryBase : ICountry
{
	/// <summary>Validate VAT number for Country</summary>
	public abstract bool Valid(VatNumber vat);

	public virtual string FormatStripped(VatNumber vat) => ToStr(vat.GetInts());

	public abstract string FormatNational(VatNumber vat);

	public virtual string FormatVat(VatNumber vat) => $"{CC} {FormatStripped(vat)}";

	private string? _ccCache;
	public virtual string CC => _ccCache ??= string.Intern(GetType().Name);
	public abstract int MinLength { get; }
	/// <summary>Length including country code any anything else required in VAT format</summary>
	public virtual int MinLengthVat => MinLength + 2;

	/// <summary>Only called if <see cref="CC"/> and <see cref="MinLengthVat"/> matches</summary>
	protected virtual string GetVatInner(string input) => input[2..].Trim();

	public virtual bool TryParse(string input, out VatNumber vat)
	{
		input = input.Trim();
		if (input.Length < MinLength)
		{
			vat = VatNumber.Empty;
			return false;
		}

		if (input.Length >= MinLengthVat &&
			input.StartsWith(CC, StringComparison.OrdinalIgnoreCase))
		{
			input = GetVatInner(input);
		}

		var xvat = new VatNumber(this, input);
		if (!Valid(xvat))
		{
			vat = VatNumber.Empty;
			return false;
		}

		vat = xvat;
		return true;
	}

	protected delegate bool SpanBool(ReadOnlySpan<int> d);
	protected delegate string SpanString(ReadOnlySpan<int> d);

	private static string Format(ReadOnlySpan<int> d, VatNumber vat, SpanBool v, SpanString f) => v(d) ? f(d) : vat;
	protected static string Format(VatNumber vat, SpanBool valid, SpanString formatValid) =>
		Format(vat.GetInts(), vat, valid, formatValid);

	public static ReadOnlySpan<int> GetIntsFromString(string s) => s.Select(c => c - '0')
		.Where(x => 0 <= x && x <= 9).ToArray().AsSpan();

	protected static int CalcMod(IEnumerable<int> digits, int mod)
		=> digits.Aggregate((r, d) => (r * (d < 10 ? 10 : 100) + d) % mod);

	public static string ToStr(ReadOnlySpan<int> ints) => string.Join("", ints.ToArray());

	public static int LuhnSum(ReadOnlySpan<int> digits)
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

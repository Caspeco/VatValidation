using VatValidation.Countries;

namespace VatValidation;

[System.Diagnostics.DebuggerDisplay("{_vatNumber} {CC} {Valid}")]
public readonly struct VatNumber(string vatNumber) : IEquatable<VatNumber>
{
	public static readonly VatNumber Empty = new(string.Empty);
	private readonly string _vatNumber = vatNumber;
	private readonly ICountry? _country;

	public VatNumber(ICountry country, string vatNumber)
		: this(vatNumber)
	{
		_country = country;
	}

	public string? CC => _country?.CC;

	public bool Valid => _country?.Valid(this) ?? false;

	public string FormatStripped => _country?.FormatStripped(this) ?? _vatNumber;

	public string FormatNational => _country?.FormatNational(this) ?? _vatNumber;

	public string FormatVat => _country?.FormatVat(this) ?? _vatNumber;

	public VatNumber VatStripped => _country is null ? new(FormatStripped) : new(_country, FormatStripped);

	public ReadOnlySpan<int> GetInts() => CountryBase.GetIntsFromString(_vatNumber);

	public static implicit operator string(VatNumber vatNumber) => vatNumber._vatNumber;
	public static implicit operator VatNumber(string vatNumber) => new(vatNumber);

	public override string ToString() => _vatNumber;

	public bool Equals(VatNumber other) => (CC is null || other.CC is null || CC == other.CC)
		&& VatStripped._vatNumber == other.VatStripped._vatNumber;
	public override int GetHashCode() => VatStripped._vatNumber.GetHashCode() * 13 + (CC?.GetHashCode() ?? 0);
	public override bool Equals(object? other) => Equals(other as VatNumber? ?? Empty);
	public static bool operator ==(VatNumber vat1, VatNumber vat2) => vat1.Equals(vat2);
	public static bool operator !=(VatNumber vat1, VatNumber vat2) => !vat1.Equals(vat2);

	public static bool TryParse(string input, out VatNumber vat)
	{
		if (input.Length < 4)
		{
			vat = Empty;
			return false;
		}

		var cc = input[0..2];
		var ccInstance = CountryBase.GetCountryInstance(cc);
		if (ccInstance is null)
		{
			// loop and find candidates
			var valids = CountryBase.CcInstances.Values.Where(i => i.TryParse(input, out _)).ToList();
			if (valids.Count != 1)
			{
				vat = Empty;
				return false;
			}
			ccInstance = valids.First();
		}

		return ccInstance.TryParse(input, out vat);
	}

	public static VatNumber Get(string? cc, string orgno) =>
		(cc is not null && cc.Length == 2 && orgno.Length >= 4 && !orgno[..2].Equals(cc, StringComparison.InvariantCultureIgnoreCase) &&
		TryParse($"{cc} {orgno}", out var vat)) ||
		TryParse(orgno, out vat)
		? vat
		: Empty;
}

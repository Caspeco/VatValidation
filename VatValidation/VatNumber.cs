using VatValidation.Countries;

namespace VatValidation;

[System.Diagnostics.DebuggerDisplay("{_vatNumber} {CC} {Valid}")]
public readonly struct VatNumber
{
	private readonly string _vatNumber;
	private readonly ICountry? _country;

	public VatNumber(string vatNumber) => _vatNumber = vatNumber;

	public VatNumber(ICountry country, string vatNumber)
	{
		_country = country;
		_vatNumber = vatNumber;
	}

	public string? CC => _country?.CC;

	public bool Valid => _country?.Valid(this) ?? false;

	public string FormatStripped => _country?.FormatStripped(this) ?? _vatNumber;

	public string FormatNational => _country?.FormatNational(this) ?? _vatNumber;

	public string FormatVat => _country?.FormatVat(this) ?? _vatNumber;

	public int[] GetInts() => CountryBase.GetIntsFromString(_vatNumber);

	public static implicit operator string(VatNumber vatNumber) => vatNumber._vatNumber;
	public static implicit operator VatNumber(string vatNumber) => new(vatNumber);

	public override string ToString() => _vatNumber;
}

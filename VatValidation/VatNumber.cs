using VatValidation.Countries;

namespace VatValidation;

[System.Diagnostics.DebuggerDisplay("{_vatNumber} {CC} {Valid}")]
public readonly struct VatNumber
{
	public static readonly VatNumber Empty = new(string.Empty);
	private readonly string _vatNumber;
	private readonly ICountry? _country;

	public VatNumber(string vatNumber) => _vatNumber = vatNumber;

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

	public string FormatNumbersOnly => CountryBase.ToStr(GetInts());

	public int[] GetInts() => CountryBase.GetIntsFromString(_vatNumber);

	public static implicit operator string(VatNumber vatNumber) => vatNumber._vatNumber;
	public static implicit operator VatNumber(string vatNumber) => new(vatNumber);

	public override string ToString() => _vatNumber;

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
}

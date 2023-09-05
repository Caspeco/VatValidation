namespace VatValidation.Countries;

public interface ICountry
{
	bool Valid(VatNumber vat);
	string CC { get; }
	string FormatStripped(VatNumber vat);
	string FormatNational(VatNumber vat);
	string FormatVat(VatNumber vat);
	bool TryParse(string input, out VatNumber vat);
}

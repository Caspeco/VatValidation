
namespace VatValidation.Countries;

/// <country>Poland</country>
/// <name>Numer identyfikacji podatkowej</name>
/// <shortname>NIP</shortname>
/// <length>10</length>
/// <checksum>mod 11</checksum>
/// <status>Verified</status>
/// <testcases>
/// valid, in: 1234563218, national: 123-456-32-18, stripped: 1234563218, vat: PL1234563218, vatstripped: PL1234563218
/// valid, in: 123-456-32-18, national: 123-456-32-18, stripped: 1234563218, vat: PL1234563218, vatstripped: PL1234563218
/// valid, in: 123-45-63-218, national: 123-456-32-18, stripped: 1234563218, vat: PL1234563218, vatstripped: PL1234563218
/// valid, in: 0000000000, national: 000-000-00-00, stripped: 0000000000, vat: PL0000000000, vatstripped: PL0000000000, dontTryParse, comment: Valid as SE BE
/// invalid, in: 123-456-78-90, national: 123-456-78-90, stripped: 1234567890, vat: PL1234567890, vatstripped: PL1234567890
/// invalid, in: 1234563217, national: 1234563217, stripped: 1234563217, vat: PL1234563217, vatstripped: PL1234563217
/// valid, in: 999-999-9999, national: 999-999-99-99, stripped: 9999999999, vat: PL9999999999, vatstripped: PL9999999999, dontTryParse, comment: Valid as SE
/// valid, in: 7680002466, national: 768-000-24-66, stripped: 7680002466, vat: PL7680002466, vatstripped: PL7680002466
/// valid, in: 1251510868, national: 125-151-08-68, stripped: 1251510868, vat: PL1251510868, vatstripped: PL1251510868
/// </testcases>
public class PL : CountryBase
{
	private PL() { }
	public static ICountry Instance { get; } = new PL();

	public override int MinLength => 10;

	public override string FormatNational(VatNumber vat) => Format(vat, Valid, d => $"{ToStr(d[0..3])}-{ToStr(d[3..6])}-{ToStr(d[6..8])}-{ToStr(d[8..10])}");

	public override string FormatVat(VatNumber vat) => FormatVatStripped(vat);

	private static readonly int[] _multipliers = [6, 5, 7, 2, 3, 4, 5, 6, 7, -1];

	private static bool ValidFormat(ReadOnlySpan<int> d) => d.Length == 10;

	// https://pl.wikipedia.org/wiki/Numer_identyfikacji_podatkowej
	// https://poradnikprzedsiebiorcy.pl/-jak-sprawdzic-nip-czyli-numer-identyfikacji-podatkowej
	// https://aplikacja.ceidg.gov.pl/ceidg/ceidg.public.ui/search.aspx
	protected override bool Valid(ReadOnlySpan<int> digits) => ValidFormat(digits) && Valid(digits.ToArray());
	private static bool Valid(int[] digits)
		=> digits
			.Zip(_multipliers, (d, m) => d * m)
			.Sum() % 11 == 0;
}

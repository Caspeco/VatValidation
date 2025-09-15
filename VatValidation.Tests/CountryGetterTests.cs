using System.Globalization;

namespace VatValidation.Tests;

public class CountryGetterTests
{
	/// <summary>Some Countries does not use ISO 3166-1 a-2,
	/// but instead based on the national name,
	/// might match ISO 639-1 language code</summary>
	public static RegionInfo CountryNameLookup(string cc)
		=> new(cc == "EL" ? "GR" : cc);

	[Theory]
	[InlineData("se", "SE")]
	[InlineData("sE", "SE")]
	public void GetCountryTest(string cc, string expectedCc)
	{
		var instance = Countries.CountryBase.GetCountryInstance(cc);
		Assert.NotNull(instance);
		Console.WriteLine(instance.ToString());
		Assert.Equal(expectedCc, instance.CC);
	}

	public static TheoryData<string> GetCountryCodes() => [.. Countries.CountryBase.CcInstances.Keys];

	[Fact]
	public void EnsureGetCountrieCodesTest()
	{
		foreach (var cc in GetCountryCodes())
		{
			var region = CountryNameLookup(cc);
			Console.WriteLine($"{cc} -> Region: {region.EnglishName} {region.Name} {region.TwoLetterISORegionName}");
			Assert.Equal(cc, region.TwoLetterISORegionName);
		}
	}

	[Theory]
	[MemberData(nameof(GetCountryCodes))]
	public void ValidateCountryTypesAndCcTest(string ccKey)
	{
		var inst = Countries.CountryBase.CcInstances[ccKey];
		var cc = inst.CC;
		var region = CountryNameLookup(cc);
		Console.WriteLine($"{ccKey} -> {inst.GetType().FullName} ({cc}) Region: {region.EnglishName} {region.Name} {region.TwoLetterISORegionName}");
		Assert.Equal(cc, ccKey);
		Assert.Equal(cc, inst.GetType().Name);
		Assert.Equal(cc, region.TwoLetterISORegionName);
	}
}

using System.Globalization;

namespace VatValidation.Tests;

public class CountryGetterTests
{
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

	public static TheoryData<string> GetCountrieCodes() => new(Countries.CountryBase.CcInstances.Keys);

	[Theory]
	[MemberData(nameof(GetCountrieCodes))]
	public void ValidateCountryTypesAndCcTest(string ccKey)
	{
		var inst = Countries.CountryBase.CcInstances[ccKey];
		var cc = inst.CC;
		var region = new RegionInfo(cc);
		Console.WriteLine($"{ccKey} -> {inst.GetType().FullName} ({cc}) Region: {region.EnglishName} {region.Name} {region.TwoLetterISORegionName}");
		Assert.Equal(cc, ccKey);
		Assert.Equal(cc, inst.GetType().Name);
		Assert.Equal(cc, region.TwoLetterISORegionName);
	}
}

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

	[Fact]
	public void GetCountryTypes()
	{
		Console.WriteLine(string.Join("\n", Countries.CountryBase.CcInstances.Values.Select(t => t!.CC)));
	}
}

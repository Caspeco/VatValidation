
namespace VatValidation.Tests;

public class VatNumberTests
{
	[Theory]
	[InlineData(true, "x", "x")]
	[InlineData(true, "y", "x")]
	[InlineData(true, "SE 1010101010 01", "SE 101010-1010")]
	[InlineData(true, "SE 101010-1010", "SE 1010101010")]
	[InlineData(false, "SE 0566988259", "BE 0566988259")]
	public void VatNumberEquals(bool expected, string vatstr1, string vatstr2)
	{
		var vat1 = VatNumber.Get(null, vatstr1);
		var vat2 = VatNumber.Get(null, vatstr2);
		Console.WriteLine($"vat1: {vat1} vat2: {vat2}");
		Assert.True(vat1.Equals(vat1));
		Assert.True(vat2.Equals(vat2));
		Assert.Equal(expected, vat1.Equals(vat2));
		Assert.Equal(expected, vat2.Equals(vat1));
		if (expected)
			Assert.Equal(vat1, vat2);
		else Assert.NotEqual(vat1, vat2);
		Assert.Equal(expected, Equals(vat1, vat2));
		Assert.Equal(expected, vat1 == vat2);
		Assert.Equal(!expected, vat1 != vat2);
		Assert.False(Equals(vat1, null));

		try
		{
			// GetHashCode verification test
			var d = new Dictionary<VatNumber, string>
				{
					{ vat1, vatstr1 },
					{ vat2, vatstr2 }
				};
			Assert.False(expected); // if equals we should have gotten exception
		}
		catch (ArgumentException) when (expected) // ignore expected exception only if expected
		{
		}
	}

	[Theory]
	[InlineData(true, "x", "x")]
	[InlineData(false, "y", "x")]
	[InlineData(false, "SE 1010101010 01", "SE 101010-1010")]
	[InlineData(false, "SE 101010-1010", "SE 1010101010")]
	[InlineData(false, "SE 0566988259", "BE 0566988259")]
	public void VatNumberCastEquals(bool expected, string vatstr1, string vatstr2)
	{
		VatNumber vat1 = vatstr1;
		VatNumber vat2 = vatstr2;
		Console.WriteLine($"vat1: {vat1} vat2: {vat2}");
		Assert.True(vat1.Equals(vat1));
		Assert.True(vat2.Equals(vat2));
		Assert.Equal(expected, vat1.Equals(vat2));
		Assert.Equal(expected, vat2.Equals(vat1));
		Assert.Equal(expected, vat1 == vat2);
	}
}

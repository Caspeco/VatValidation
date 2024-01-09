using Towel;

namespace VatValidation.Tests;

public class TestcasesFromXmlComments
{
	private static bool IsLine(string line, string wanted)
	{
		if (!line.Contains(wanted))
			return false;
		if (line != wanted) throw new Exception(line);
		return true;
	}

	private static void AddTestcasesFromTypeXmlData(TheoryData<string, string> cases, Countries.ICountry inst)
	{
		Console.WriteLine($"{nameof(AddTestcasesFromTypeXmlData)} {inst.CC}");
		var documentation = inst.GetType().GetDocumentation()?.Trim();
		//	Assert.NotNull(documentation);
		Assert.NotEqual("", documentation);
		if (string.IsNullOrEmpty(documentation))
			return;
		Assert.Contains("<testcases>", documentation);
		Assert.Contains("</testcases>", documentation);
		var inTestcasesSection = false;
		foreach (var l in documentation.Split('\n'))
		{
			var line = l.Trim();
			if (IsLine(line, "<testcases>"))
			{
				inTestcasesSection = true;
				continue;
			}
			else if (!inTestcasesSection)
				continue;
			if (IsLine(line, "</testcases>"))
			{
				break;
			}

			var test = GetTestFromTestLine(inst.CC, line);
			Assert.NotNull(test);

			cases.Add(inst.CC, line);
		}
	}

	private class TestData
	{
		public readonly string? Input;
		public readonly bool ExpectedValid;
		public readonly string? ExpectedNational;
		public readonly string? ExpectedVat;
		public readonly string? ExpectedStripped;
		public readonly string? ExpectedVatStripped;
		public readonly bool DontTryParseOnInput = false;

		public TestData(string line)
		{
			Console.WriteLine($"Generating testdata from {line}");
			var spl = line.Split(',').Select(f => f.Trim()).ToArray();
			Console.WriteLine(string.Join("|", spl));
			Assert.True(spl[0] == "valid" || spl[0] == "invalid");
			for (int i = 0; i < spl.Length; i++)
			{
				var cspl = spl[i];
				var kcspl = cspl.Split([':'],2).Select(f => f.Trim()).ToArray();
				if (cspl == "valid") ExpectedValid = true;
				else if (cspl == "invalid") ExpectedValid = false;
				else if (kcspl[0] == "in" && kcspl.Length == 2) Input = kcspl[1];
				else if (kcspl[0] == "national" && kcspl.Length == 2) ExpectedNational = kcspl[1];
				else if (kcspl[0] == "vat" && kcspl.Length == 2) ExpectedVat = kcspl[1];
				else if (kcspl[0] == "stripped" && kcspl.Length == 2) ExpectedStripped = kcspl[1];
				else if (kcspl[0] == "vatstripped" && kcspl.Length == 2) ExpectedVatStripped = kcspl[1];
				else if (cspl == "dontTryParse") DontTryParseOnInput = true;
				else
				{
					Assert.Fail($"Unknown {cspl}");
				}
			}

			Assert.NotNull(Input);
			Assert.NotNull(ExpectedNational);
			Assert.NotNull(ExpectedVat);
			Assert.NotNull(ExpectedStripped);
		}
	}

	private static IEnumerable<Action> GetTestFromTestLine(string cc, string line)
	{
		var instance = Countries.CountryBase.CcInstances[cc];
		var data = new TestData(line);
		yield return () => // VatTest
		{
			var vat = new VatNumber(instance, data.Input!);
			Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
			Assert.Equal(data.ExpectedValid, vat.Valid);
			Assert.Equal(data.ExpectedNational, vat.FormatNational);
			Assert.Equal(data.ExpectedVat, vat.FormatVat);
			Assert.Equal(data.ExpectedStripped, vat.FormatStripped);
		};

		yield return () => // VatStripValidNational
		{
			var vat = new VatNumber(instance, data.Input!);
			Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");

			var vatStrip = vat.VatStripped;
			Assert.Equal(data.ExpectedValid, vatStrip.Valid);
			Assert.Equal(data.ExpectedStripped, (string)vatStrip);
			// Formatted if valid, otherwise original data
			if (!vatStrip.Valid)
				Assert.Equal(data.ExpectedStripped, vatStrip.FormatNational);
			//Assert.Equal(vatStrip.Valid ? expectNational : expectedStriped, vatStrip.FormatNational);
			Assert.Equal(vatStrip.Valid ? data.ExpectedNational : data.ExpectedStripped, vatStrip.FormatNational);
			Assert.Equal(data.ExpectedVatStripped ?? data.ExpectedVat, vatStrip.FormatVat);
		};

		if (!string.IsNullOrEmpty(data.ExpectedVat))
		{
			yield return () => // TryParse Known VAT
			{
				Assert.Equal(data.ExpectedValid, VatNumber.TryParse(data.ExpectedVat!, out var vat));
				Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
				Assert.Equal(data.ExpectedValid ? instance.CC : null, vat.CC);
				Assert.Equal(data.ExpectedValid, vat.Valid);
				Assert.Equal(data.ExpectedValid ? data.ExpectedVat : "", vat.FormatVat);
				Assert.Equal(data.ExpectedValid ? data.ExpectedNational : "", vat.FormatNational);
			};
		}

		if (!data.DontTryParseOnInput)
		{
			yield return () => // TryParse Known VAT
			{
				Assert.Equal(data.ExpectedValid, VatNumber.TryParse(data.Input!, out var vat));
				Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
				Assert.Equal(data.ExpectedValid ? instance.CC : null, vat.CC);
				Assert.Equal(data.ExpectedValid, vat.Valid);
				Assert.Equal(data.ExpectedValid ? data.ExpectedVat : "", vat.FormatVat);
				Assert.Equal(data.ExpectedValid ? data.ExpectedNational : "", vat.FormatNational);
			};
		}
	}

	public static TheoryData<string, string> GetCountryTestCases()
	{
		var cases = new TheoryData<string, string>();
		foreach (var inst in Countries.CountryBase.CcInstances.Values)
		{
			AddTestcasesFromTypeXmlData(cases, inst);
		}

		return cases;
	}

	public static TheoryData<string> GetCountryCodes() => CountryGetterTests.GetCountryCodes();

	[Theory]
	[MemberData(nameof(GetCountryCodes))]
	public void EnsureGetCountryTestCasesData(string ccKey)
	{
		// populates testdata per country
		var inst = Countries.CountryBase.CcInstances[ccKey];
		var cases = new TheoryData<string, string>();
		AddTestcasesFromTypeXmlData(cases, inst);
		foreach (var cs in cases)
		{
			Console.WriteLine($"{cs[0]} {cs[1]}");
		}
	}

	[Fact]
	public void EnsureGetCountryTestCasesTestSource()
	{
		// runs the actual test source
		var cases = GetCountryTestCases();
		foreach (var cs in cases)
		{
			Console.WriteLine($"{cs[0]} {cs[1]}");
		}
	}

	[Theory]
	[MemberData(nameof(GetCountryTestCases))]
	public void GetXmlDocumentationTest(string ccKey, string testline)
	{
		foreach (var func in GetTestFromTestLine(ccKey, testline))
			func();
	}
}

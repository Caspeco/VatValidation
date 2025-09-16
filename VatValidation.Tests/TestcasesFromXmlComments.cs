using System.Xml.Linq;
using Towel;
using Xunit.Abstractions;

namespace VatValidation.Tests;

public class TestcasesFromXmlComments
{
	private static IEnumerable<string> GetElementsLines(IEnumerable<XElement>? elms) =>
		elms?.SelectMany(e => e.Nodes().SelectMany(n => n.ToString().Trim().Split('\n').Select(l => l.Trim()))) ?? [];

	private static string[] GetTestCaseRows(Countries.ICountry inst)
	{
		Console.WriteLine($"{nameof(AddTestcasesFromTypeXmlData)} {inst.CC}");
		var documentation = inst.GetType().GetDocumentation()?.Trim();
		if (documentation is null)
			Console.WriteLine("all implementations must at least have basic XML testcases");
		Assert.NotNull(documentation);
		Assert.NotEqual("", documentation);
		var xmlo = XElement.Parse($"<root>{documentation}</root>");
		var testcases = xmlo.Elements("testcases");
		Assert.Single(testcases);
		var testcaserows = GetElementsLines(testcases)?.ToList() ?? [];

		var countryElms = xmlo.Elements("country");
		Assert.Single(countryElms);
		var region = new System.Globalization.RegionInfo(inst.CC);
		var country = region.EnglishName;
		Assert.Equal(country, string.Join('\n', GetElementsLines(countryElms)));

		return [.. testcaserows];
	}

	private static void AddTestcasesFromTypeXmlData(TheoryData<string, TestData> cases, Countries.ICountry inst)
	{
		foreach (var l in GetTestCaseRows(inst))
		{
			var data = new TestData(l);
			var test = GetTestFromTestLine(inst.CC, data);
			Assert.NotNull(test);

			cases.Add(inst.CC, data);
		}
	}

	public class TestData : IXunitSerializable
	{
		public string? Input { get; private set; }
		public bool ExpectedValid { get; private set; }
		public bool ExpectedValidStripped { get; private set; }
		public string? ExpectedNational { get; private set; }
		public string? ExpectedVat { get; private set; }
		public string? ExpectedStripped { get; private set; }
		public string? ExpectedVatStripped { get; private set; }
		public bool DontTryParseOnInput { get; private set; } = false;
		public string? Comment { get; private set; }

		private string? Line;

		public TestData() { } // for IXunitSerializable

		public TestData(string line) => ParseLine(line);

		/// <inheritdoc />
		public void Deserialize(IXunitSerializationInfo info) => ParseLine(info.GetValue<string>(nameof(Line)));

		/// <inheritdoc />
		public void Serialize(IXunitSerializationInfo info)
		{
			info.AddValue(nameof(Line), Line);
		}

		private void ParseLine(string line)
		{
			Line = line;
			Console.WriteLine($"Generating testdata from {line}");
			var spl = line.Split(',').Select(f => f.Trim()).ToArray();
			Console.WriteLine(string.Join("|", spl));
			Assert.True(spl[0] == "valid" || spl[0] == "invalid");
			for (int i = 0; i < spl.Length; i++)
			{
				var cspl = spl[i];
				var kcspl = cspl.Split([':'], 2).Select(f => f.Trim()).ToArray();
				if (cspl == "valid") ExpectedValidStripped = ExpectedValid = true;
				else if (cspl == "invalid") ExpectedValidStripped = ExpectedValid = false;
				else if (cspl == "strippedvalid") ExpectedValidStripped = true;
				else if (kcspl[0] == "in" && kcspl.Length == 2) Input = kcspl[1];
				else if (kcspl[0] == "national" && kcspl.Length == 2) ExpectedNational = kcspl[1];
				else if (kcspl[0] == "vat" && kcspl.Length == 2) ExpectedVat = kcspl[1];
				else if (kcspl[0] == "stripped" && kcspl.Length == 2) ExpectedStripped = kcspl[1];
				else if (kcspl[0] == "vatstripped" && kcspl.Length == 2) ExpectedVatStripped = kcspl[1];
				else if (cspl == "dontTryParse") DontTryParseOnInput = true;
				else if (kcspl[0] == "comment" && kcspl.Length == 2) Comment = kcspl[1];
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

		public override string ToString() => $"{Comment} {Line}".Trim();
	}

	private static IEnumerable<Action> GetTestFromTestLine(string cc, TestData data)
	{
		Console.WriteLine(data.ToString());
		var instance = Countries.CountryBase.CcInstances[cc];
		yield return () => // VatTest
		{
			var vat = new VatNumber(instance, data.Input!);
			Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
			Assert.Equal(data.ExpectedValid, vat.Valid);
			Assert.Equal(data.ExpectedNational, vat.FormatNational);
			Assert.Equal(data.ExpectedVat, vat.FormatVat);
			Assert.Equal(data.ExpectedStripped, vat.FormatStripped);
			if (data.ExpectedVatStripped is not null)
			{
				Assert.Equal(data.ExpectedVatStripped, vat.FormatVatStripped);
			}
		};

		yield return () => // VatStripValidNational
		{
			var vat = new VatNumber(instance, data.Input!);
			Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");

			var vatStrip = vat.VatStripped;
			Assert.Equal(data.ExpectedValidStripped, vatStrip.Valid);
			Assert.Equal(data.ExpectedStripped, (string)vatStrip);
			// Formatted if valid, otherwise original data
			if (!vatStrip.Valid)
				Assert.Equal(data.ExpectedStripped, vatStrip.FormatNational);
			Assert.Equal(vatStrip.Valid ? data.ExpectedNational : data.ExpectedStripped, vatStrip.FormatNational);
			if (vatStrip.Valid)
			{
				// make sure stripped but Valid VatNumber always formats fully
				Assert.Equal(data.ExpectedVat, vatStrip.FormatVat);
			}
			if (data.ExpectedVatStripped is not null)
			{
				Assert.Equal(data.ExpectedVatStripped, vatStrip.FormatVatStripped);
			}
		};

		if (!string.IsNullOrEmpty(data.ExpectedVat))
		{
			yield return () => // TryParse Known VAT
			{
				Assert.Equal(data.ExpectedValidStripped, VatNumber.TryParse(data.ExpectedVat!, out var vat));
				Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat}");
				Assert.Equal(data.ExpectedValidStripped ? instance.CC : null, vat.CC);
				Assert.Equal(data.ExpectedValidStripped, vat.Valid);
				Assert.Equal(data.ExpectedValidStripped ? data.ExpectedVat : "", vat.FormatVat);
				Assert.Equal(data.ExpectedValidStripped ? data.ExpectedNational : "", vat.FormatNational);
			};
		}

		yield return () => // dump if different match
		{
			var valids = Countries.CountryBase.CcInstances.Values
			.Where(i => i.TryParse(data.Input!, out _)).ToList();
			foreach (var v in valids)
			{
				if (v.CC == instance.CC)
					continue;
				Console.WriteLine($"{instance.CC} Valid with CC:  {v.CC}  inp: {data.Input}");
			}
		};

		if (!data.DontTryParseOnInput)
		{
			yield return () => // TryParse Known VAT
			{
				var parseValid = VatNumber.TryParse(data.Input!, out var vat);
				Console.WriteLine($"cc: {vat.CC} nat: {vat.FormatNational} vat: {vat.FormatVat} parsevalid: {parseValid}");
				Assert.Equal(data.ExpectedValid, parseValid);
				Assert.Equal(data.ExpectedValid ? instance.CC : null, vat.CC);
				Assert.Equal(data.ExpectedValid, vat.Valid);
				Assert.Equal(data.ExpectedValid ? data.ExpectedVat : "", vat.FormatVat);
				Assert.Equal(data.ExpectedValid ? data.ExpectedNational : "", vat.FormatNational);
			};
		}
	}

	public static TheoryData<string, TestData> GetCountryTestCases()
	{
		var cases = new TheoryData<string, TestData>();
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
		var cases = new TheoryData<string, TestData>();
		AddTestcasesFromTypeXmlData(cases, inst);
		foreach (var cs in cases)
		{
			Assert.Equal(ccKey, cs[0]);
			// dump testcase data for possible use with RunManualXmlDocumentationLineTest
			Console.WriteLine($"[InlineData(\"{ccKey}\", \"{cs[1]}\")]");
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
	public void GetXmlDocumentationTest(string ccKey, TestData data)
	{
		Assert.Multiple([.. GetTestFromTestLine(ccKey, data)]);
	}

	[Theory]
	[InlineData("SE", "invalid, in: 1, national: 1, vat: SE 1 01, stripped: 1, vatstripped: SE101")] // dummy to not leave this empty
	public void RunManualXmlDocumentationLineTest(string ccKey, string line)
	{
		var data = new TestData(line);
		var test = GetTestFromTestLine(ccKey, data);
		Assert.NotNull(test);

		Assert.Multiple([.. test]);
	}
}

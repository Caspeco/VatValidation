using System.Text;

namespace VatValidation.Tests;

public class SupportDataFromXmlComments: TestcasesFromXmlComments
{
	private static void UpdateDictFromInstance(Dictionary<string, string> valdict, Countries.ICountry inst)
	{
		foreach (var e in GetXmlDocumentation(inst)
			.Elements())
		{
			var name = e.Name.ToString();
			var value = string.Join("\n", GetElementsLines(e));
			valdict.Add(name, value);
		}

		foreach (var l in valdict["testcases"].Split("\n"))
		{
			if (valdict.ContainsKey("vatstripped"))
			{
				return;
			}
			var data = new TestData(l);
			if (!data.ExpectedValid)
			{
				continue;
			}

			if (data.ExpectedNational is null ||
				data.ExpectedStripped is null ||
				data.ExpectedVat is null ||
				data.ExpectedVatStripped is null)
			{
				continue;
			}

			valdict.Add("national", data.ExpectedNational);
			valdict.Add("stripped", data.ExpectedStripped);
			valdict.Add("vat", data.ExpectedVat);
			valdict.Add("vatstripped", data.ExpectedVatStripped);
		}
	}

	private static Dictionary<string, string> GetSupportTableProps() =>
		new()
		{
			{ "country", "Country" },
			{ "cc", "Prefix" },
			{ "name", "Name" },
			{ "shortname", "Short name" },
			{ "length", "Length" },
			{ "checksum", "Checksum" },
			{ "national", "FormatNational" },
			{ "stripped", "FormatStripped" },
			{ "vat", "FormatVat" },
			{ "vatstripped", "FormatVatStripped" },
			{ "status", "Status" },
		};

	private static HashSet<string> EUVatCountries() =>
		[
			"AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR",
			"DE", "EL", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL",
			"PL", "PT", "RO", "SK", "SI", "ES", "SE",
		];

	public static string GenerateSupportTable()
	{
		var sb = new StringBuilder();
		var propstouse = GetSupportTableProps();
		sb.AppendLine(string.Join(" | ", ["",
				.. propstouse.Values.Select(x => x),
				""]).Trim());
		sb.AppendLine(string.Join(" | ", ["",
				.. propstouse.Select(x => "----"),
				""]).Trim());

		var allVatCc = EUVatCountries();
		allVatCc.UnionWith(Countries.CountryBase.CcInstances.Keys);
		foreach (var cc in allVatCc
			.OrderBy(c => CountryGetterTests.CountryNameLookup(c).EnglishName))
		{
			var valdict = new Dictionary<string, string>
			{
				{ "cc", cc },
			};
			if (!Countries.CountryBase.CcInstances.TryGetValue(cc, out var inst))
			{
				valdict.Add("country", CountryGetterTests.CountryNameLookup(cc).EnglishName);
				valdict.Add("status", "Not yet supported");
				sb.AppendLine(string.Join(" | ", ["",
					.. propstouse.Keys.Select(x => valdict.GetValueOrDefault(x, "-")),
					""]).Trim());
				continue;
			}

			UpdateDictFromInstance(valdict, inst);

			try
			{
				sb.AppendLine(string.Join(" | ", ["",
					.. propstouse.Keys.Select(x => valdict[x]),
					""]).Trim());
			}
			catch (KeyNotFoundException)
			{
				Console.WriteLine($"CC: {inst.CC}");
				throw;
			}
		}

		return sb.ToString();
	}

	[Fact]
	public void GenerateTableDump()
	{
		Console.WriteLine(GenerateSupportTable());
	}

	private static FileInfo GetParentDirectoryWithFile(DirectoryInfo? dir, string filename)
	{
		while (dir is not null)
		{
			var fis = dir.EnumerateFiles(filename, SearchOption.AllDirectories)
				.FirstOrDefault();
			if (fis is not null)
			{
				return fis;
			}
			dir = dir.Parent;
		}
		throw new DirectoryNotFoundException();
	}

	[Fact]
	public void EnsureAndUpdateReadme()
	{
		const string startTag = "<!-- COUNTRIES START -->\n";
		const string endTag = "<!-- COUNTRIES END -->";
		// Find Readme
		var pathToStartSearch = new FileInfo(
			Environment.GetEnvironmentVariable("NCrunch.OriginalProjectPath")
			?? Environment.GetEnvironmentVariable("NCrunch.OriginalSolutionPath")
			?? Path.Combine(Environment.CurrentDirectory, "dummy.txt")).Directory;
		var readmefile = GetParentDirectoryWithFile(pathToStartSearch, "README.md");
		Console.WriteLine($"Working with {readmefile}");

		// Split Readme data
		var readmeData = File.ReadAllText(readmefile.FullName).AsSpan();
		var startTagPos = readmeData.IndexOf(startTag);
		if (startTagPos == -1)
		{
			throw new InvalidDataException($"The `{startTag}` was not found in {readmefile}");
		}
		var startTagPosEnd = startTagPos + startTag.Length;
		var endTagPos = readmeData.LastIndexOf(endTag);
		if (endTagPos == -1)
		{
			throw new InvalidDataException($"The `{endTag}` was not found in {readmefile}");
		}
		Console.WriteLine($"Replace data between {startTagPosEnd}-{endTagPos}");
		var newReadmeData = string.Concat(
			readmeData[0..startTagPosEnd],
			GenerateSupportTable(),
			readmeData[endTagPos..]
			).AsSpan();
		if (!newReadmeData.SequenceEqual(readmeData))
		{
			var byteWriter = new System.Buffers.ArrayBufferWriter<byte>();
			Encoding.UTF8.GetBytes(newReadmeData, byteWriter);
			using var f = readmefile.OpenWrite();
			f.SetLength(byteWriter.WrittenSpan.Length);
			f.Write(byteWriter.WrittenSpan);
			f.Flush();
			f.Close();
			Assert.Equal("", $"There was an update to {readmefile}, Make sure to commit");
		}
	}
}

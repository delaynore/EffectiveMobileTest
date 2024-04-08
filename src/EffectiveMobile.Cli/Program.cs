using EffectiveMobile.Cli.Options;
using EffectiveMobile.Core;
using EffectiveMobile.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;

namespace EffectiveMobile.Cli;

public static class Program
{
	public async static Task Main(string[] args)
	{
		try
		{
			await Run(args);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

	public async static Task Run(string[] args)
	{
		var options = ReadConfiguration(args);

		var analyzer = new AccessLogAnalyzer(
			new FileLogProvider(options.FileLog),
			new IPAddressFilter(options.AddressStart, options.AddressMask));

		var dict = await analyzer.GetNumberRequestPerIpAddress(
			options.TimeStart, options.TimeEnd);

		await new FileWriter(options.FileOutput)
			.WriteAllText(string.Join('\n', dict.Select(x => $"{x.Key}:{x.Value}")));

		Console.WriteLine("Success! Press any key to close...");
		Console.ReadKey();
	}

	private static CliOptions ReadConfiguration(string[] args)
	{
		var config = new ConfigurationBuilder()
			.AddEnvironmentVariables("--")
			.AddJsonFile("appsettings.json")
			.AddCommandLine(args)
			.Build();

		var options = new CliOptions();

		var fileLog = config.GetValue<string>("file-log");
		ArgumentNullException.ThrowIfNull(fileLog, "file-log");
		if (!File.Exists(fileLog))
		{
			throw new FileNotFoundException("file-log is not found");
		}
		options.FileLog = fileLog;

		var fileOutput = config.GetValue<string>("file-output");
		ArgumentNullException.ThrowIfNull(fileOutput, "file-output");
		options.FileOutput = fileOutput;

		var addressStart = config.GetValue<string>("address-start");
		if (addressStart is not null)
		{
			if (!IPAddress.TryParse(addressStart, out var parsedStart))
			{
				throw new ArgumentException("address-start is not valid IPv4 address");
			}
			options.AddressStart = parsedStart;

			var addressMask = config.GetValue<string>("address-mask");
			if (addressMask is not null)
			{
				if (!IPAddress.TryParse(addressMask, out var parsedMask))
				{
					throw new ArgumentException("address-mask is not valid IPv4 address");
				}
				options.AddressMask = parsedMask;
			}
		}
		else
		{
			options.AddressStart = IPAddress.Parse("0.0.0.0");
			options.AddressMask = IPAddress.Parse("0.0.0.0");
		}

		var timeStartString = config.GetValue<string>("time-start");
		ArgumentNullException.ThrowIfNull(timeStartString, "time-start");
		if (!DateTime.TryParseExact(timeStartString, "dd.MM.yyyy", null, DateTimeStyles.None, out var timeStart))
		{
			throw new FormatException("time-start is not valid date time format");
		}
		options.TimeStart = timeStart;

		var timeEndString = config.GetValue<string>("time-end");
		ArgumentNullException.ThrowIfNull(timeEndString, "time-end");
		if (!DateTime.TryParseExact(timeEndString, "dd.MM.yyyy", null, DateTimeStyles.None, out var timeEnd))
		{
			throw new FormatException("time-end is not valid date time format");
		}
		options.TimeEnd = timeEnd;

		return options;
	}
}

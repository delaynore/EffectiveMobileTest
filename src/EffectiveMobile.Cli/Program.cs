using EffectiveMobile.Cli.Options;
using EffectiveMobile.Cli.Validator;
using EffectiveMobile.Core;
using EffectiveMobile.Infrastructure;
using Microsoft.Extensions.Configuration;

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
		var options = new CliOptionsValidator(ReadConfiguration(args))
			.Validate();
		
		var analyzer = new AccessLogAnalyzer(
			new FileLogProvider(options.FileLog),
			new IPAddressFilter(options.AddressStart, options.AddressMask ?? CliOptions.DefaultMask));

		var dict = await analyzer.GetNumberRequestPerIpAddress(
			options.TimeStart, options.TimeEnd);

		await new FileWriter(options.FileOutput)
			.WriteAllText(string.Join('\n', dict.Select(x => $"{x.Key}:{x.Value}")));

		Console.WriteLine("Success! Press any key to close...");
		Console.ReadKey();
	}

	private static IConfiguration ReadConfiguration(string[] args)
	{
		return new ConfigurationBuilder()
			.AddEnvironmentVariables("--")
			.AddJsonFile("appsettings.json")
			.AddCommandLine(args)
			.Build();
	}
}
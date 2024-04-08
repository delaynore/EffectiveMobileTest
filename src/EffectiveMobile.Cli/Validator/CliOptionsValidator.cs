using EffectiveMobile.Cli.Options;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;

namespace EffectiveMobile.Cli.Validator;

public class CliOptionsValidator : IValidator<CliOptions>
{
	private readonly IConfiguration _configuration;
	public CliOptionsValidator(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public CliOptions Validate()
	{
		var options = new CliOptions();

		var fileLog = _configuration.GetValue<string>("file-log");
		options.FileLog = ValidateFileForRead(fileLog, "file-log");

		var fileOutput = _configuration.GetValue<string>("file-output");
		options.FileOutput = ValidateFile(fileOutput, "file-output");

		var addressStart = _configuration.GetValue<string>("address-start");
		options.AddressStart = ValidateIpAddress(addressStart, "address-start");

		var addressMask = _configuration.GetValue<string>("address-mask");
		options.AddressMask = ValidateIpAddress(addressMask, "address-mask");

		if (addressStart is null && options.AddressMask is { })
		{
			throw new ArgumentException("'address-mask' can't be used without 'address-start'");
		}

		var timeStartString = _configuration.GetValue<string>("time-start");
		options.TimeStart = ValidateDateTime(timeStartString, "time-start");

		var timeEndString = _configuration.GetValue<string>("time-end");
		options.TimeEnd = ValidateDateTime(timeEndString, "time-end");

		return options;
	}

	private string ValidateFile(string? file, string optionName)
	{
		ArgumentNullException.ThrowIfNull(file, optionName);
		return file;
	}

	private string ValidateFileForRead(string? file, string optionName)
	{
		ValidateFile(file, optionName);
		if (!File.Exists(file))
		{
			throw new FileNotFoundException($"'{optionName}' is not found");
		}
		return file;
	}

	private IPAddress ValidateIpAddress(string? ip, string optionName)
	{
		if (ip is null) return CliOptions.DefaultIp;

		if (!IPAddress.TryParse(ip, out var parsed))
		{
			throw new ArgumentException($"'{optionName}' is not valid IPv4 address");
		}

		return parsed;
	}

	private DateTime ValidateDateTime(string? timeString, string optionName)
	{
		ArgumentNullException.ThrowIfNull(timeString, optionName);

		if (!DateTime.TryParseExact(timeString, "dd.MM.yyyy", null, DateTimeStyles.None, out var time))
		{
			throw new FormatException($"'{optionName}' is not valid date time format");
		}

		return time;
	}
}
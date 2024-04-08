using System.Net;

namespace EffectiveMobile.Cli.Options;

public class CliOptions
{
	public static readonly IPAddress DefaultMask = IPAddress.Any;
	public static readonly IPAddress DefaultIp = IPAddress.Any;
	public string FileLog { get; set; } = default!;
	public string FileOutput { get; set; } = default!;
	public IPAddress AddressStart { get; set; } = default!;
	public IPAddress? AddressMask { get; set; }
	public DateTime TimeStart { get; set; }
	public DateTime TimeEnd { get; set; }
}

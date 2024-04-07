using System.Net;

namespace EffectiveMobile.Cli.Options;

public class CliOptions
{
	public string FileLog { get; set; } = default!;
	public string FileOutput { get; set; } = default!;
	public IPAddress AddressStart { get; set; } = default!;
	public IPAddress AddressMask { get; set; } = default!;
	public DateTime TimeStart { get; set; }
	public DateTime TimeEnd { get; set; }
}

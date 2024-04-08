using EffectiveMobile.Core.Interafaces;
using System.Net;

namespace EffectiveMobile.Core;

public class AccessLogAnalyzer
{
	private readonly ILogProvider _logProvider;
	private readonly IIPAddressFilter _iPAddressFilter;

	public AccessLogAnalyzer(
		ILogProvider logProvider, 
		IIPAddressFilter iPAddressFilter)
	{
		ArgumentNullException.ThrowIfNull(logProvider);
		ArgumentNullException.ThrowIfNull(iPAddressFilter);

		_logProvider = logProvider;
		_iPAddressFilter = iPAddressFilter;
	}

	public async Task<Dictionary<IPAddress, int>> GetNumberRequestPerIpAddress(
		DateTime timeStart,
		DateTime timeEnd)
	{
		if (timeEnd != DateTime.MaxValue 
			&& timeEnd <= DateTime.MaxValue.AddDays(-1).AddTicks(1)) 
		{
			timeEnd = timeEnd.AddDays(1).AddTicks(-1);
		}

		if (timeStart > timeEnd)
		{
			throw new ArgumentException($"{nameof(timeStart)} can't be later than {nameof(timeEnd)}");
		}

		var log = await _logProvider.GetLog();
		var dict = new Dictionary<IPAddress, int>();

		foreach (var (ip, dateTime) in LogParser.Parse(log))
		{
			if (dateTime < timeStart || dateTime > timeEnd) continue;
			if (!_iPAddressFilter.IsSatisfies(ip)) continue;

			dict.TryGetValue(ip, out var value);
			dict[ip] = value + 1;
		}

		return dict;
	}
}

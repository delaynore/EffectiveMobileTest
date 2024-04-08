using System.Globalization;
using System.Net;

namespace EffectiveMobile.Core;

public static class LogParser
{

	public static IEnumerable<(IPAddress ip, DateTime dateTime)> Parse(string[] lines)
	{
		ArgumentNullException.ThrowIfNull(lines);

		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line)) continue;

			var splited = line.Split(':', 2,
				StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			if (splited is null
				|| splited.Length != 2
				|| string.IsNullOrEmpty(splited[0])
				|| string.IsNullOrEmpty(splited[1])
				|| !DateTime.TryParseExact(splited[1], "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out var date)
				|| !IPAddress.TryParse(splited[0], out var ip)) continue;

			yield return (ip, date);
		}
	}
}

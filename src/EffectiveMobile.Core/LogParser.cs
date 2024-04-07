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

			var splited = line.Trim().Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
			var ip = IPAddress.Parse(splited[0]);
			var date = DateTime.ParseExact(splited[1], "yyyy-MM-dd HH:mm:ss", null);

			yield return (ip, date);
		}
	}
}

using EffectiveMobile.Core.Interafaces;
using System.Net;

namespace EffectiveMobile.Infrastructure;

public class IPAddressFilter : IIPAddressFilter
{
	private readonly uint _addressStart;
	private readonly uint _addressEnd;

	public IPAddressFilter(IPAddress addressStart, IPAddress addressMask)
	{
		ArgumentNullException.ThrowIfNull(addressStart);
		ArgumentNullException.ThrowIfNull(addressMask);

		_addressStart = IPAddressToUint(addressStart);
		_addressEnd = ~IPAddressToUint(addressMask) | _addressStart;
	}

	public bool IsSatisfies(IPAddress address)
	{
		ArgumentNullException.ThrowIfNull(address);

		var ip = IPAddressToUint(address);

		return ip >= _addressStart && ip <= _addressEnd;
	}

	private uint IPAddressToUint(IPAddress address)
	{
		var splitted = address.ToString().Split('.').Select(byte.Parse);
		var result = 0u;
		foreach (var item in splitted)
		{
			result = result << 8 | item;
		}
		return result;
	}
}

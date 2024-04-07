using System.Net;

namespace EffectiveMobile.Core.Interafaces;

public interface IIPAddressFilter
{
	bool IsSatisfies(IPAddress address);
}

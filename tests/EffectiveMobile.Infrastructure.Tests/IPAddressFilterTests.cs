using System.Net;

namespace EffectiveMobile.Infrastructure.Tests;

public class IPAddressFilterTests
{
	[Fact]
	public void Constructor_NullAddressStart_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(
			() => new IPAddressFilter(null!, IPAddress.Parse("255.255.255.0")));
	}

	[Fact]
	public void Constructor_NullAddressMask_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(
			() => new IPAddressFilter(IPAddress.Parse("192.168.1.0"), null!));
	}

	[Theory]
	[InlineData("192.168.1.0")]
	[InlineData("192.168.1.1")]
	[InlineData("192.168.1.2")]
	[InlineData("192.168.1.100")]
	[InlineData("192.168.1.254")]
	[InlineData("192.168.1.255")]
	public void IsSatisfies_AddressInRange_ReturnsTrue(string ip)
	{
		var filter = new IPAddressFilter(
			IPAddress.Parse("192.168.1.0"), IPAddress.Parse("255.255.255.0"));

		Assert.True(filter.IsSatisfies(IPAddress.Parse(ip)));
	}

	[Theory]
	[InlineData("192.168.0.0")]
	[InlineData("192.167.1.1")]
	[InlineData("0.0.0.0")]
	[InlineData("191.168.1.100")]
	[InlineData("192.167.1.254")]
	[InlineData("192.168.0.255")]
	public void IsSatisfies_AddressBelowRange_ReturnsFalse(string ip)
	{
		var filter = new IPAddressFilter(
			IPAddress.Parse("192.168.1.0"), IPAddress.Parse("255.255.255.0"));

		Assert.False(filter.IsSatisfies(IPAddress.Parse(ip)));
	}

	[Theory]
	[InlineData("42.126.42.2")]
	[InlineData("43.126.255.2")]
	[InlineData("43.127.42.2")]
	[InlineData("44.126.42.2")]
	[InlineData("43.125.0.0")]

	public void IsSatisfies_AddressAboveRange_ReturnsFalse(string ip)
	{
		var filter = new IPAddressFilter(
			IPAddress.Parse("43.126.42.2"), IPAddress.Parse("255.255.1.0"));

		Assert.False(filter.IsSatisfies(IPAddress.Parse(ip)));
	}

	[Fact]
	public void IsSatisfies_NullAddress_ThrowsArgumentNullException()
	{
		var filter = new IPAddressFilter(
			IPAddress.Parse("192.168.1.0"), IPAddress.Parse("255.255.255.0"));

		Assert.Throws<ArgumentNullException>(() => filter.IsSatisfies(null!));
	}
}

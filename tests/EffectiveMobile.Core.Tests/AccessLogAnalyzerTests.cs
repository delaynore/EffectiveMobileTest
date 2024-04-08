using EffectiveMobile.Core;
using EffectiveMobile.Core.Interafaces;
using Moq;
using System.Net;

namespace EffectiveMobile.Core.Tests
{
	public class AccessLogAnalyzerTests
	{
		[Fact]
		public async Task GetNumberRequestPerIpAddress_EmptyLog_ReturnsEmptyDictionary()
		{
			var logProviderMock = new Mock<ILogProvider>();
			logProviderMock.Setup(lp => lp.GetLog()).ReturnsAsync(new string[0]);

			var ipAddressFilterMock = new Mock<IIPAddressFilter>();
			ipAddressFilterMock.Setup(ipf => ipf.IsSatisfies(It.IsAny<IPAddress>())).Returns(true);

			var analyzer = new AccessLogAnalyzer(logProviderMock.Object, ipAddressFilterMock.Object);

			var result = await analyzer.GetNumberRequestPerIpAddress(DateTime.MinValue, DateTime.MaxValue);

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetNumberRequestPerIpAddress_SingleRequest_ReturnsSingleEntry()
		{
			var logProviderMock = new Mock<ILogProvider>();
			logProviderMock.Setup(lp => lp.GetLog()).ReturnsAsync(new[]
			{
				"192.168.1.100:2022-01-01 12:34:56"
			});

			var ipAddressFilterMock = new Mock<IIPAddressFilter>();
			ipAddressFilterMock.Setup(ipf => ipf.IsSatisfies(It.IsAny<IPAddress>())).Returns(true);

			var analyzer = new AccessLogAnalyzer(logProviderMock.Object, ipAddressFilterMock.Object);

			var result = await analyzer.GetNumberRequestPerIpAddress(DateTime.MinValue, DateTime.MaxValue);

			Assert.Single(result);
			Assert.Equal(IPAddress.Parse("192.168.1.100"), result.Keys.Single());
			Assert.Equal(1, result.Values.Single());
		}

		[Fact]
		public async Task GetNumberRequestPerIpAddress_MultipleRequests_ReturnsMultipleEntries()
		{
			var logProviderMock = new Mock<ILogProvider>();
			logProviderMock.Setup(lp => lp.GetLog()).ReturnsAsync(new[]
			{
				"192.168.1.100:2022-01-01 12:34:56",
				"192.168.1.100:2022-01-01 12:35:56",
				"192.168.1.101:2022-01-01 12:36:56",
				"192.168.1.101:2022-01-01 12:37:56",
				"192.168.1.101:2022-01-01 12:38:56"
			});

			var ipAddressFilterMock = new Mock<IIPAddressFilter>();
			ipAddressFilterMock.Setup(ipf => ipf.IsSatisfies(It.IsAny<IPAddress>())).Returns(true);

			var analyzer = new AccessLogAnalyzer(logProviderMock.Object, ipAddressFilterMock.Object);

			var result = await analyzer.GetNumberRequestPerIpAddress(DateTime.MinValue, DateTime.MaxValue);

			Assert.Equal(2, result.Count);
			Assert.True(result.ContainsKey(IPAddress.Parse("192.168.1.100")));
			Assert.True(result.ContainsKey(IPAddress.Parse("192.168.1.101")));
			Assert.Equal(2, result[IPAddress.Parse("192.168.1.100")]);
			Assert.Equal(3, result[IPAddress.Parse("192.168.1.101")]);
		}

		[Fact]
		public async Task GetNumberRequestPerIpAddress_FilteredRequests_ReturnsFilteredEntries()
		{
			var logProviderMock = new Mock<ILogProvider>();
			logProviderMock.Setup(lp => lp.GetLog()).ReturnsAsync(new[]
			{
				"192.168.1.100:2022-01-01 12:34:56",
				"192.168.1.101:2022-01-01 12:35:56",
				"192.168.1.102:2022-01-01 12:36:56",
				"192.168.1.103:2022-01-01 12:37:56",
				"192.168.1.104:2022-01-01 12:38:56"
			});

			var ipAddressFilterMock = new Mock<IIPAddressFilter>();
			ipAddressFilterMock.Setup(ipf => ipf.IsSatisfies(It.IsAny<IPAddress>())).Returns(true);

			var analyzer = new AccessLogAnalyzer(logProviderMock.Object, ipAddressFilterMock.Object);

			ipAddressFilterMock.Setup(
				ipf => ipf.IsSatisfies(IPAddress.Parse("192.168.1.100"))).Returns(true);
			ipAddressFilterMock.Setup(
				ipf => ipf.IsSatisfies(IPAddress.Parse("192.168.1.101"))).Returns(false);
			ipAddressFilterMock.Setup(
				ipf => ipf.IsSatisfies(IPAddress.Parse("192.168.1.102"))).Returns(false);
			ipAddressFilterMock.Setup(
				ipf => ipf.IsSatisfies(IPAddress.Parse("192.168.1.103"))).Returns(false);
			ipAddressFilterMock.Setup(
				ipf => ipf.IsSatisfies(IPAddress.Parse("192.168.1.104"))).Returns(false);

			var result = await analyzer.GetNumberRequestPerIpAddress(DateTime.MinValue, DateTime.MaxValue);

			Assert.Single(result);
			Assert.Equal(IPAddress.Parse("192.168.1.100"), result.Keys.Single());
			Assert.Equal(1, result.Values.Single());
		}
	}
}

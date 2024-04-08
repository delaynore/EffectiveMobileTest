using System.Net;

namespace EffectiveMobile.Core.Tests
{
	public class LogParserTests
	{
		[Fact]
		public void Parse_NullLines_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => LogParser.Parse(null!).ToList());
		}

		[Fact]
		public void Parse_EmptyLines_ReturnsEmptySequence()
		{
			var lines = new string[0];

			var result = LogParser.Parse(lines).ToList();

			Assert.Empty(result);
		}

		[Fact]
		public void Parse_SingleLine_ReturnsSingleResult()
		{
			var lines = new[] { "192.168.1.1:2022-01-01 12:34:56" };

			var result = LogParser.Parse(lines).ToList();

			Assert.Single(result);
			Assert.Equal(IPAddress.Parse("192.168.1.1"), result[0].ip);
			Assert.Equal(new DateTime(2022, 01, 01, 12, 34, 56), result[0].dateTime);
		}

		[Fact]
		public void Parse_MultipleLines_ReturnsMultipleResults()
		{
			var lines = new[]
			{
				"192.168.1.1:2022-01-01 12:34:56",
				"192.168.1.2:2022-01-01 12:35:56",
				"192.168.1.3:2022-01-01 12:36:56"
			};

			var result = LogParser.Parse(lines).ToList();

			Assert.Equal(3, result.Count);
			Assert.Equal(IPAddress.Parse("192.168.1.1"), result[0].ip);
			Assert.Equal(new DateTime(2022, 01, 01, 12, 34, 56), result[0].dateTime);
			Assert.Equal(IPAddress.Parse("192.168.1.2"), result[1].ip);
			Assert.Equal(new DateTime(2022, 01, 01, 12, 35, 56), result[1].dateTime);
			Assert.Equal(IPAddress.Parse("192.168.1.3"), result[2].ip);
			Assert.Equal(new DateTime(2022, 01, 01, 12, 36, 56), result[2].dateTime);
		}

		[Fact]
		public void Parse_InvalidLineFormat_SkipsLine()
		{
			var lines = new[]
			{
				"192.168.1.1:2022-01-01 12:34:56",
				"invalid line",
				"192.168.1.3:2022-01-01 12:36:56"
			};

			var result = LogParser.Parse(lines).ToList();

			Assert.Equal(2, result.Count);
			Assert.Equal(IPAddress.Parse("192.168.1.1"), result[0].ip);
			Assert.Equal(new DateTime(2022, 01, 01, 12, 34, 56), result[0].dateTime);
			Assert.Equal(IPAddress.Parse("192.168.1.3"), result[1].ip);
			Assert.Equal(new DateTime(2022, 01, 01, 12, 36, 56), result[1].dateTime);
		}
	}
}

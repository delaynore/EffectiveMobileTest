using EffectiveMobile.Core.Interafaces;

namespace EffectiveMobile.Infrastructure;

public class FileLogProvider : ILogProvider
{
	private readonly string _fileName;

	public FileLogProvider(string fileName)
	{
		ArgumentNullException.ThrowIfNull(fileName, nameof(fileName));

		_fileName = fileName;
	}


	public async Task<string[]> GetLog()
	{
		return await File.ReadAllLinesAsync(_fileName);
	}

}

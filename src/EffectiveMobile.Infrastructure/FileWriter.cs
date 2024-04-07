using EffectiveMobile.Core.Interafaces;

namespace EffectiveMobile.Infrastructure;

public class FileWriter : IWriter
{
	private readonly string _fileName;

    public FileWriter(string fileName)
	{
		ArgumentException.ThrowIfNullOrEmpty(fileName);

        _fileName = fileName;
    }

    public async Task WriteAllText(string text)
	{
		using var writer = new StreamWriter(_fileName);
		await writer.WriteAsync(text);
	}
}

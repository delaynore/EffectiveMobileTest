namespace EffectiveMobile.Core.Interafaces;

public interface ILogProvider
{
	Task<string[]> GetLog();
}

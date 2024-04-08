namespace EffectiveMobile.Cli.Validator;

public interface IValidator<T>
{
	T Validate();
}
namespace Eggshell.Resources
{
	public interface ICompiler<in T> : IObject
	{
		void Compile( T asset );
	}

	public interface ITester<T>
	{
		string Test( string asset );
	}
}

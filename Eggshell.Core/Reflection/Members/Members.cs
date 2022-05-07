using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Eggshell.Reflection
{
	public class Members<T, TInfo> : IEnumerable<T> where TInfo : MemberInfo where T : class, IMember<TInfo>
	{
		public Library Parent { get; }

		public Members( Library parent )
		{
			Parent = parent;
		}

		public void Add( T item ) { }

		// Enumerator

		public IEnumerator<T> GetEnumerator()
		{
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

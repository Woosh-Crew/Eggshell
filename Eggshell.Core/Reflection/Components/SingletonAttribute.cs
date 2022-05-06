﻿using System;

namespace Eggshell
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
	public class SingletonAttribute : Attribute, IComponent<Library>
	{
		public void OnAttached( Library item ) { }
	}
}

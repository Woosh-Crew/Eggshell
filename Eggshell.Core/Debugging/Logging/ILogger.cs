using System;
using System.Collections.Generic;
using Eggshell.Debugging.Logging;

namespace Eggshell.Debugging.Logging
{
	public interface ILogger
	{
		IReadOnlyCollection<Entry> All { get; }

		void Add( Entry entry );
		void Clear();
	}
}

//
// Extensions
// 

public static class LoggingProviderExtensions
{
	public static void Info<T>( this ILogger provider, T message, string stack = null )
	{
		provider?.Add( new()
		{
			Message = message.ToString(),
			Trace = string.IsNullOrWhiteSpace( stack ) ? Environment.StackTrace : stack,
			Level = "Info",
		} );
	}

	public static void Warning<T>( this ILogger provider, T message )
	{
		provider?.Add( new()
		{
			Message = message.ToString(),
			Trace = Environment.StackTrace,
			Level = "Warning",
		} );
	}

	public static void Error<T>( this ILogger provider, T message )
	{
		provider?.Add( new()
		{
			Message = message.ToString(),
			Trace = Environment.StackTrace,
			Level = "Error",
		} );
	}

	public static void Exception( this ILogger provider, Exception exception )
	{
		provider?.Add( new()
		{
			Message = $"{exception.Message}",
			Trace = exception.StackTrace,
			Level = "Exception",
		} );
	}
}

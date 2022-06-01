﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eggshell
{
	/// <summary>
	/// A IRoutine is a routine that can potentially have a UI representing it (Such as loading a map,
	/// connecting to a networked game, downloading an item while loading a map from the workshop).
	/// </summary>
	public interface IRoutine
	{
		/// <summary>
		/// The progress from 0 to 1 on how far done this routine is.
		/// </summary>load
		float Progress { get; }

		/// <summary>
		/// The text that should appear in the UI for when this request is loading.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Run this item through the routine. Which means it could have a UI representing
		/// its current loading state.
		/// </summary>
		void Load( Action loaded );

		/// <summary>
		/// Allows the injection of instructions before this instruction happens. Useful for preloading
		/// files, downloading maps, etc before actually loading the map
		/// </summary>
		IRoutine[] Inject() { return null; }
	}
	
	/// <summary>
	/// Routines might sound similar to coroutine, but they are completely different.
	/// Routines are a set of functions that fire based of a series of callbacks,
	/// allowing us to sequentially perform actions that usually are spanned across
	/// different frames, or different threads.
	/// </summary>
	public sealed class Routines : Module
	{
			public Action Started { get; set; }
		public Action Finished { get; set; }

		// Debug

		public Stopwatch Timing { get; private set; }

		// Current

		public IRoutine Current { get; private set; }
		public float Progress => Current.Progress;

		// States

		public void Start( params IRoutine[] request )
		{
			var final = new Request[request.Length];

			for ( var i = 0; i < request.Length; i++ )
			{
				final[i] = new( request[i] );
			}

			Start( final );
		}

		public void Start( params Func<IRoutine>[] request )
		{
			var final = new Request[request.Length];

			for ( var i = 0; i < request.Length; i++ )
			{
				final[i] = new( request[i] );
			}

			Start( final );
		}

		public void Start( params Request[] request )
		{
			Assert.IsEmpty( request );
			Assert.IsNotNull( Stack, "Already loading something" );

			Timing = Stopwatch.StartNew();

			// Build Queue
			Stack = Build( request );
			Amount = Stack.Count;

			Load();
			Started?.Invoke();
		}

		private void Finish()
		{
			Timing.Stop();
			Finished?.Invoke();

			Stack = null;
			Current = null;
			Amount = 0;
		}

		// Stack

		private static Stack<Request> Build( Request[] requests )
		{
			// Build Stack
			var stack = new Stack<Request>();

			for ( var i = requests.Length - 1; i >= 0; i-- )
			{
				stack.Push( requests[i] );
			}

			return stack;
		}

		private Stack<Request> Stack { get; set; }
		private int Amount { get; set; }

		// Loading

		private void Load()
		{
			if ( Stack.Count == 0 )
			{
				Finish();
				return;
			}

			while ( true )
			{
				var possible = Stack.Peek();

				// If it doesn't exist, just continue
				if ( possible.Loadable == null )
				{
					Stack.Pop();
					continue;
				}

				// We've injected, just load this
				if ( possible.Injected )
				{
					Loading( Stack.Pop().Loadable );
					return;
				}

				// Peek, see if we should inject
				var injection = possible.Loadable.Inject();

				if ( injection is { Length: > 0 } )
				{
					for ( var i = injection.Length - 1; i >= 0; i-- )
					{
						Stack.Push( new( injection[i] ) );
					}

					possible.Injected = true;

					// Rerun function, to see if injected
					// have more injections. injection inception..
					continue;
				}

				// If nothing to inject, start loading
				Loading( Stack.Pop().Loadable );
				break;
			}
		}

		private void Loading( IRoutine loadable )
		{
			if ( loadable == null )
			{
				OnLoad();
				return;
			}

			Current = loadable;
			loadable?.Load( OnLoad );
		}

		private void OnLoad()
		{
			Load();
		}

		public class Request
		{
			public bool Injected { get; internal set; }

			// Loadable

			private IRoutine _cached;

			public IRoutine Loadable
			{
				get
				{
					if ( _cached == null && Invoker == null )
					{
						return null;
					}

					return _cached ??= Invoker.Invoke();
				}
			}

			private Func<IRoutine> Invoker { get; }

			public Request( Func<IRoutine> request )
			{
				Invoker = request;
				Injected = false;
			}

			public Request( IRoutine request )
			{
				_cached = request;
				Injected = false;
			}
		}
	}
}

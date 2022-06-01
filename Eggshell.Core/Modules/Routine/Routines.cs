using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
		// Static API
		// --------------------------------------------------------------------------------------- //

		/// <summary>
		/// The progress of the current routine being processed. In the future this will be updated
		/// to be the progress of the whole routine.
		/// </summary>
		public static float Progress => Get<Routines>().Current.Progress;

		/// <summary>
		/// The help / loading text for the current routine that is being processed. Use this in
		/// your UI if you are using routines for loading maps.
		/// </summary>
		public static string Text => Get<Routines>().Current.Text;

		/// <summary>
		/// Starts a new routine based on an array of requests (params) with an optional callback
		/// parameter for when the routine is finished. 
		/// </summary>
		public static void Start( Action finished, params IRoutine[] request )
		{
			Get<Routines>().Finished = finished;
			Start( request );
		}

		/// <summary>
		/// Starts a new routine based on an array of func requests (params) with an optional
		/// callback parameter for when the routine is finished. 
		/// </summary>
		public static void Start( Action finished, params Func<IRoutine>[] request )
		{
			Get<Routines>().Finished = finished;
			Start( request );
		}

		/// <summary>
		/// Starts a new routine based on an array of requests. Nothing special other than
		/// that. Its advised you use Func version where you can.
		/// </summary>
		public static void Start( params IRoutine[] request )
		{
			var final = new Request[request.Length];

			for ( var i = 0; i < request.Length; i++ )
			{
				final[i] = new( request[i] );
			}

			Get<Routines>().Start( final );
		}

		/// <summary>
		/// Starts a new routine based on an array of func requests. Will invoke the func when
		/// its time for it to inject.
		/// </summary>
		public static void Start( params Func<IRoutine>[] request )
		{
			var final = new Request[request.Length];

			for ( var i = 0; i < request.Length; i++ )
			{
				final[i] = new( request[i] );
			}

			Get<Routines>().Start( final );
		}

		/// <summary>
		/// Creates a IRoutine based on an Action that has an Action as one of its parameters 
		/// (I know its weird) can be quite useful in a handful of scenarios (Such as prototyping
		/// a routine) 
		/// </summary>
		public static IRoutine Create( Action<Action> loaded, string text )
		{
			// Really weird I know...
			return new ActionBasedCallback( loaded, text );
		}

		/// <summary>
		/// Creates a IRoutine based on a task. The finish callback will be invoked once this task 
		/// has been completed. Progress depends on the task type.
		/// </summary>
		public static IRoutine Create( Task loaded, string text )
		{
			var operation = new TaskBasedCallback( loaded, text );
			return operation;
		}

		/// <summary>
		/// The time it took for the last routine that was running to completely finish. This
		/// is mostly used as debugging information.
		/// </summary>
		public Stopwatch Timing { get; private set; }

		private Action Finished { get; set; }
		private IRoutine Current { get; set; }

		// States

		private void Start( params Request[] request )
		{
			Assert.IsEmpty( request );
			Assert.IsNotNull( Stack, "Already loading something" );

			Timing = Stopwatch.StartNew();

			// Build Queue
			Stack = Build( request );
			Amount = Stack.Count;

			Load();
		}

		private void Finish()
		{
			Timing.Stop();
			Finished?.Invoke();

			Finished = null;
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

		// Internal Classes
		// --------------------------------------------------------------------------------------- //
		
		private class Request
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

		private class ActionBasedCallback : IRoutine
		{
			private readonly Action<Action> _operation;

			public ActionBasedCallback( Action<Action> loaded, string text )
			{
				Text = text;
				_operation = loaded;
			}

			// Loadable

			public float Progress { get; }
			public string Text { get; }

			public void Load( Action loaded )
			{
				_operation.Invoke( loaded );
			}
		}

		private class TaskBasedCallback : IRoutine
		{
			private readonly Task _task;

			public TaskBasedCallback( Task loaded, string text )
			{
				Text = text;
				_task = loaded;
			}

			// Loadable

			public float Progress { get; set; }
			public string Text { get; }

			public async void Load( Action loaded )
			{
				try
				{
					// Not sure if this is a good idea?
					await _task;
				}
				catch ( Exception e )
				{
					Terminal.Log.Exception( e );
				}

				loaded.Invoke();
			}
		}
	}
}

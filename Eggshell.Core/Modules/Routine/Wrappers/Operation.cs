using System;
using System.Threading.Tasks;

namespace Eggshell.Unity
{
	public static class Operation
	{
		/// <summary>
		/// Creates a IRoutine based on an Action that has an Action as one
		/// of its parameters (I know its weird) can be quite useful in a
		/// handful of scenarios (Such as prototyping a routine) 
		/// </summary>
		public static IRoutine Create( Action<Action> loaded, string text )
		{
			// Really weird I know...
			return new ActionBasedCallback( loaded, text );
		}

		/// <summary>
		/// Creates a IRoutine based on a task. The finish callback will be
		/// invoked once this task has been completed. Progress depends on
		/// the task type.
		/// </summary>
		public static IRoutine Create( Task loaded, string text )
		{
			var operation = new TaskBasedCallback( loaded, text );
			return operation;
		}
		
		// Internal Classes
		// --------------------------------------------------------------------------------------- //

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

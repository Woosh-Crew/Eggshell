using System;

namespace Eggshell
{
    /// <summary>
    /// Entry point to your Eggs, You'd want to
    /// inherit from this so Eggshell automatically
    /// creates your applications object. Make sure to
    /// call Crack in your programs entry point.
    /// </summary>
    public abstract class Project : Module
    {
        /// <summary>
        /// Has Eggshell already been booted up and initialized? From
        /// a preexisting bootstrap?
        /// </summary>
        protected static bool Booted { get; private set; }

        /// <summary>
        /// The currently active bootstrap, that was cached from the
        /// crack methods. Bootstraps are used for low level control of
        /// eggshell.
        /// </summary>
        public static Bootstrap Bootstrap { get; private set; }

        /// <summary>
        /// Uses Eggshells reflection system to find a
        /// bootstrap and uses that to initialize all
        /// modules created by source generators
        /// </summary>
        protected static void Crack()
        {
            Crack(Library.Database.Find<Bootstrap>().Create<Bootstrap>());
        }

        /// <summary>
        /// Initializes all Modules, created by
        /// source generators, through reflection
        /// </summary>
        protected static void Crack(Bootstrap bootstrap)
        {
            if (Booted)
            {
                Terminal.Log.Warning("Already booted Eggshell Project");
                return;
            }

            try
            {
                using (Terminal.Stopwatch("Eggshell Ready"))
                {
                    Bootstrap = bootstrap;
                    
                    bootstrap.Boot();
                    Booted = true;
                }
            }
            catch (Exception e)
            {
                Booted = false;
                Bootstrap = null;
                
                Terminal.Log.Error("Bootstrap Failed");
                Terminal.Log.Exception(e);
            }
        }
    }
}

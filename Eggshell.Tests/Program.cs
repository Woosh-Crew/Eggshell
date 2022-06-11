#define EGGSHELL

namespace Eggshell.Tests
{
    [Archive(Fallback = "game://testing.txt")]
    public class Testing : IObject
    {
        public Library ClassInfo { get; }

        public Testing()
        {
            ClassInfo = Library.Register(this);
        }
    }

    public class Component1 : IComponent<Console>
    {
        public Console Attached { get; set; }

        public void OnAttached()
        {
            Terminal.Log.Info("Attaching Component 1");
        }

        public void OnDetached()
        {
            Terminal.Log.Info("Detaching Component 1");
        }
    }

    public class Component2 : IComponent<Console>
    {
        public Console Attached { get; set; }
        
        public void OnAttached()
        {
            Terminal.Log.Info("Attaching Component 2");
        }

        public void OnDetached()
        {
            Terminal.Log.Info("Detaching Component 2");
        }
    }

    [Icon(Id = "terminal")]
    public class Console : Project
    {
        public Components<Console> Components { get; }

        public static void Main(string[] args)
        {
            Crack(new());
        }

        public Console()
        {
            Components = new(this);
        }

        protected override void OnReady()
        {
            Components.Create<Component1>();
            var order = Components.Replace<Component1, Component2>();
        }
    }
}

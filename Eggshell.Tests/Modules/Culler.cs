#define LOGGING

namespace Eggshell.Tests
{
    public class Culler : Module, Bootstrap.Callbacks
    {
        public bool OnModule(IModule module)
        {
            if (module is Clock)
            {
                return false;
            }
            
            Terminal.Log.Info($"Module - {module}");
            return true;
        }
    }
}

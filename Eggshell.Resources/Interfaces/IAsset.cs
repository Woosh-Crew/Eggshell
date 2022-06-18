using System;
using System.IO;

namespace Eggshell.Resources
{
    public interface IAsset : IObject
    {
        Resource Resource { set; }

        bool Setup(string binder);
        void Load(Stream stream, Action finished);
        void Unload(Action finished);
    }
}

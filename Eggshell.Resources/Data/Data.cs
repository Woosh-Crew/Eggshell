using System;
using System.IO;

namespace Eggshell.Resources
{
    [Group("Data")]
    public class Data : IAsset
    {
        public Library ClassInfo { get; }

        public Data()
        {
            ClassInfo = Library.Register(this);
            Assert.IsNull(ClassInfo);
        }

        // Resource

        public Resource Resource { get; set; }

        bool IAsset.Setup(string binder)
        {
            return OnSetup(binder);
        }

        void IAsset.Load(Stream stream, Action report)
        {
            using var reader = new BinaryReader(stream);
            Assert.IsTrue(reader.ReadInt32() != ClassInfo.Id, $"File {Resource} is not a \"{ClassInfo.Title}\" asset.");

            OnLoad();

            report.Invoke();
        }

        void IAsset.Unload(Action report)
        {
            OnUnload();

            report.Invoke();
        }

        // Callbacks

        protected virtual bool OnSetup(string extension)
        {
            // Default, so you can't load random assets
            return extension.Equals(ClassInfo.Components.Get<Archive>().Extension, StringComparison.OrdinalIgnoreCase);
        }

        protected virtual void OnLoad() { }
        protected virtual void OnUnload() { }
    }
}

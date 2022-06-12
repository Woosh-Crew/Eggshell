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

        bool IAsset.Setup(string extension)
        {
            return OnSetup(extension);
        }

        void IAsset.Load(Stream stream)
        {
            using var reader = new BinaryReader(stream);
            Assert.IsTrue(reader.ReadInt32() != ClassInfo.Id, $"File {Resource.Name} is not a \"{ClassInfo.Title}\" asset.");

            OnLoad();
        }

        void IAsset.Unload()
        {
            OnUnload();
        }

        // Callbacks

        protected virtual bool OnSetup(string extension) { return true; }
        protected virtual void OnLoad() { }
        protected virtual void OnUnload() { }
    }
}

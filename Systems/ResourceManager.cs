using LF2Clone.Base;
using LF2Clone.Misc.Logger;
using LF2Clone.Resources;

namespace LF2Clone.Systems
{
    public sealed class ResourceManager : System<ResourceManager>
    {
        public Dictionary<Guid, Resource> _loadedResourcesDict = new();
        //public Dictionary<Guid, SFX> _loadedSoundsDict = new();
        //public Dictionary<Guid, Texture> _loadedTexturesDict = new();
        //public Dictionary<Guid, Font> _loadedFontsDict = new();
        public List<string> _resourcePaths = new List<string>();
        public List<string> _resourceNames = new List<string>();

        public ResourceManager(ILogger logger) : base(logger)
        {
            _loadedResourcesDict = new Dictionary<Guid, Resource>();
            //_loadedSoundsDict = new Dictionary<Guid, SFX>();
            //_loadedTexturesDict = new();
            //_loadedFontsDict = new();
            _resourcePaths = new List<string>();
        }

        public void LoadResource(string path)
        {
            // load resources based on the file's extensions
        }

        public enum ResourceType
        {
            Texture,
            SFX
        }
    }
}

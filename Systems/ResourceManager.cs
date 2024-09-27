using LF2Clone.Base;
using LF2Clone.Misc.Logger;
using LF2Clone.Resources;

namespace LF2Clone.Systems
{
    public sealed class ResourceManager : System<ResourceManager>
    {
        // Windows specific
        private static Dictionary<string, ResourceType> _resourceTypeExtensions = new Dictionary<string, ResourceType>() 
        {
            [".png"] = ResourceType.Texture,
            [".jpeg"] = ResourceType.Texture,
            [".mp3"] = ResourceType.SFX,
            [".wav"] = ResourceType.SFX,
            [".ogg"] = ResourceType.SFX,
            [".vert"] = ResourceType.Shader, // Unknown if possible
            [".tesc"] = ResourceType.Shader, // Unknown if possible
            [".tese"] = ResourceType.Shader, // Unknown if possible
            [".geom"] = ResourceType.Shader, // Unknown if possible
            [".frag"] = ResourceType.Shader, // Unknown if possible
            [".comp"] = ResourceType.Shader, // Unknown if possible
            [".fs"] = ResourceType.Shader,
            [".vs"] = ResourceType.Shader,
            [".ttf"] = ResourceType.Font
        };

        private const int MAX_SOUND_SIZE = 150000;
        private string _defVertShaderPath;

        public Dictionary<string, SFX> _loadedSoundsDict = new();
        public Dictionary<string, Texture> _loadedTexturesDict = new();
        public Dictionary<string, Font> _loadedFontsDict = new();
        public Dictionary<string, Shader> _loadedShadersDict = new();
        public List<string> _resourcePaths = new List<string>();
        

        public ResourceManager(ILogger logger) : base(logger)
        {
            _loadedSoundsDict = new Dictionary<string, SFX>();
            _loadedTexturesDict = new Dictionary<string, Texture>();
            _loadedFontsDict = new Dictionary<string, Font>();
            _loadedShadersDict = new Dictionary<string, Shader>();

            _resourcePaths = new List<string>();
            _defVertShaderPath = string.Empty;
        }

        public override void Setup()
        {
            base.Setup();
            var defVertShaderPath = string.Format("{0}\\{1}", Environment.CurrentDirectory, "..\\..\\..\\Assets\\Shaders\\def_vert.vs");
            if (File.Exists(defVertShaderPath))
            {
                _defVertShaderPath = defVertShaderPath;
            }
        }

        public void LoadResource(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                _logger.LogError("Resource path is empty.");
                return;
            }

            if (!File.Exists(resourcePath))
            {
                _logger.LogError(string.Format("File at path {0} does not exist.", resourcePath));
                return;
            }

            // load resource based on the file's extensions
            var extension = Path.GetExtension(resourcePath);

            if (string.IsNullOrEmpty(extension))
            {
                _logger.LogError(string.Format("Extension for file at {0} is unknown."));
                return;
            }

            try
            {
                switch (_resourceTypeExtensions.ContainsKey(extension) ? _resourceTypeExtensions[extension] : ResourceType.Unknown)
                {
                    case ResourceType.Texture:
                        LoadTexture(resourcePath);
                        break;
                    case ResourceType.SFX:
                        LoadSFX(resourcePath);
                        break;
                    case ResourceType.Shader:
                        LoadShader(resourcePath);
                        break;
                    case ResourceType.Font:
                        LoadFont(resourcePath);
                        break;
                    default:
                        _logger.LogError(string.Format("Extension {0} not supported.", extension));
                        break;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(string.Format("\nFailed to load resource {0}. \nException: {1}", resourcePath, ex.ToString()));
            }
        }

        void LoadTexture(string resourcePath)
        {
            var name = Path.GetFileNameWithoutExtension(resourcePath);
            var loadedTexture = Raylib_cs.Raylib.LoadTexture(resourcePath);

            var texture = new Texture()
            {
                _id = Guid.NewGuid(),
                _path = resourcePath,
                _name = name,
                _sizeX = loadedTexture.Width,
                _sizeY = loadedTexture.Height,
                _texture = loadedTexture,
            };

            if (!_loadedTexturesDict.TryAdd(name, texture))
            {
                return;
            }
            _resourcePaths.Add(resourcePath);
            _logger.LogInfo(string.Format("Loaded texture. File added as: {0}.", name));
        }

        void LoadSFX(string resourcePath)
        {
            // find out if sound/music based of off audio length / file size
            var name = Path.GetFileNameWithoutExtension(resourcePath);
            var fileBytes = File.ReadAllBytes(resourcePath);

            object placeholder = new object();
            var type = SFX.SoundType.Sound;

            if (fileBytes.Length > MAX_SOUND_SIZE) // it should be considered sound until this value
            {
                placeholder = Raylib_cs.Raylib.LoadMusicStream(resourcePath);
                type = SFX.SoundType.Music;
                _logger.LogInfo(string.Format("File bigger than {0} bytes is considered music, not a sound.", MAX_SOUND_SIZE));
            }
            else
            {
                placeholder = Raylib_cs.Raylib.LoadSound(resourcePath);
                _logger.LogInfo(string.Format("Sound loaded.", MAX_SOUND_SIZE));
            }

            var duration = type switch
            {
                SFX.SoundType.Sound => fileBytes.Length / ((Raylib_cs.Sound)placeholder).Stream.SampleRate,
                SFX.SoundType.Music => Raylib_cs.Raylib.GetMusicTimeLength((Raylib_cs.Music)placeholder),
                _ => 1.0f
            };

            var sfx = new SFX()
            {
                _id = Guid.NewGuid(),
                _path = resourcePath,
                _name = name,
                _volumeNormalized = 1.0f,
                _value = placeholder,
                _type = type,
                _durationInSeconds = duration,
            };

            if (!_loadedSoundsDict.TryAdd(resourcePath, sfx))
            {
                return;
            }
            _resourcePaths.Add(resourcePath);
            _logger.LogInfo(string.Format("Loaded SFX. File added as: {0}.", name));
        }

        void LoadShader(string resourcePath)
        {
            if (string.IsNullOrEmpty(_defVertShaderPath))
            {
                throw new Exception("Lack of default shader. \n Full shader loading not supported yet.");
            }

            var name = Path.GetFileNameWithoutExtension(resourcePath);
            var loadedShader = Raylib_cs.Raylib.LoadShader(_defVertShaderPath, resourcePath);
            var shader = new Shader()
            {
                _id = Guid.NewGuid(),
                _path = resourcePath,
                _name = name,
                _shader = loadedShader,
            };
            _loadedShadersDict.TryAdd(resourcePath, shader);
            _resourcePaths.Add(resourcePath);
            _logger.LogInfo(string.Format("Loaded shader. File added as: {0}.", name));
        }

        void LoadFont(string resourcePath)
        {
            var name = Path.GetFileNameWithoutExtension(resourcePath);
            var loadedFont = Raylib_cs.Raylib.LoadFont(resourcePath);

            var font = new Font()
            {
                _id = Guid.NewGuid(),
                _path = resourcePath,
                _name = name,
                _font = loadedFont
            };

            if (!_loadedFontsDict.TryAdd(resourcePath, font))
            {
                return;
            }
            _resourcePaths.Add(resourcePath);
            _logger.LogInfo(string.Format("Loaded font. File added as: {0}.", name));
        }


        private enum ResourceType
        {
            Texture,
            SFX,
            Shader,
            Font,
            Unknown
        }
    }
}

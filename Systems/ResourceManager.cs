using LF2Clone.Base;
using LF2Clone.Events;
using LF2Clone.Misc.Logger;
using LF2Clone.Resources;
using static LF2Clone.Systems.ResourceManager;

namespace LF2Clone.Systems
{
    public sealed class ResourceManager : System<ResourceManager>
    {
        // Windows specific
        private readonly static Dictionary<string, ResourceType> _resourceTypeExtensionsDict = new Dictionary<string, ResourceType>() 
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
            [".ttf"] = ResourceType.Font,
            [".txt"] = ResourceType.Text
        };

        private const int MAX_SOUND_SIZE = 150000;
        private string _defVertShaderPath;
        private string _projectDirectory;

        public Dictionary<string, SFX> _loadedSoundsDict = new();
        public Dictionary<string, Texture> _loadedTexturesDict = new();
        public Dictionary<string, Font> _loadedFontsDict = new();
        public Dictionary<string, Shader> _loadedShadersDict = new();
        public List<string> _resourcePaths = new List<string>();
        private readonly FileSystemWatcher _watcher;

        public ResourceManager(ILogger logger) : base(logger)
        {
            _loadedSoundsDict = new Dictionary<string, SFX>();
            _loadedTexturesDict = new Dictionary<string, Texture>();
            _loadedFontsDict = new Dictionary<string, Font>();
            _loadedShadersDict = new Dictionary<string, Shader>();

            _resourcePaths = new List<string>();
            _defVertShaderPath = string.Empty;
            _projectDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;

            _watcher = new FileSystemWatcher(_projectDirectory);

            foreach (var key in _resourceTypeExtensionsDict.Keys)
            {
                _watcher.Filters.Add($"*{key}");
            }
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;

            //_watcher.Changed += ResourceAddedEvent;
            _watcher.Created += PotentialResourceAddedEvent;
            _watcher.Deleted += PotentialResourceDeletedEvent;
            _watcher.Renamed += PotentialResourceRenamedEvent;
        }

        void PotentialResourceAddedEvent(object sender, FileSystemEventArgs e)
        {
            try
            {
                var ext = Path.GetExtension(e.FullPath);

                if (_resourceTypeExtensionsDict.ContainsKey(ext))
                {
                    if (_resourcePaths.Contains(e.FullPath))
                    {
                        return;
                    }
                    _resourcePaths.Add(e.FullPath);
                    _logger.LogInfo(string.Format("New resource at {0}.", e.FullPath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error getting added file. Exception: {0}", ex.ToString()));
            }
        }

        void PotentialResourceDeletedEvent(object sender, FileSystemEventArgs e)
        {
            try
            {
                var ext = Path.GetExtension(e.FullPath);

                if (_resourceTypeExtensionsDict.ContainsKey(ext))
                {
                    if (!_resourcePaths.Contains(e.FullPath))
                    {
                        return;
                    }
                    var type = _resourceTypeExtensionsDict[ext];

                    Action<string>? removeResource = type switch
                    {
                        ResourceType.Texture => _loadedTexturesDict.ContainsKey(e.FullPath) 
                            ? delegate(string path) { UnloadResource(_loadedTexturesDict[path]); _loadedTexturesDict.Remove(path); } 
                            : null,
                        ResourceType.SFX => _loadedSoundsDict.ContainsKey(e.FullPath) 
                            ? delegate (string path) { UnloadResource(_loadedSoundsDict[path]); _loadedSoundsDict.Remove(path); } 
                            : null,
                        ResourceType.Shader => _loadedTexturesDict.ContainsKey(e.FullPath) 
                            ? delegate (string path) { UnloadResource(_loadedShadersDict[path]); _loadedShadersDict.Remove(path); } 
                            : null,
                        ResourceType.Font => _loadedTexturesDict.ContainsKey(e.FullPath) 
                            ? delegate (string path) { UnloadResource(_loadedFontsDict[path]); _loadedFontsDict.Remove(path); } 
                            : null,
                        ResourceType.Text => delegate (string path) { Console.WriteLine("Text files are not implemented yet."); },
                        _ => null
                    };

                    if (removeResource == null)
                    {
                        _logger.LogError(string.Format("Unrecognized file extension. No resource deleted."));
                        return;
                    }
                    removeResource.Invoke(e.FullPath);
                    _resourcePaths.Remove(e.FullPath);
                    _logger.LogInfo(string.Format("Resource {0} deleted.", e.FullPath));
                    return;
                }
                _logger.LogInfo(string.Format("File {0} deleted. No resource deleted.", e.FullPath));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error when unloading deleted resource. Exception: {0}", ex.ToString()));
            }
        }

        void PotentialResourceRenamedEvent(object sender, RenamedEventArgs e)
        {
            try
            {
                if (File.GetAttributes(e.FullPath) == FileAttributes.Directory) return;

                var extNew = Path.GetExtension(e.FullPath);
                var extOld = Path.GetExtension(e.OldFullPath);

                if (_resourceTypeExtensionsDict.ContainsKey(extNew))
                {
                    if (_resourcePaths.Contains(extNew))
                    {
                        return;
                    }
                    _resourcePaths.Add(e.FullPath);
                    _logger.LogInfo(string.Format("New resource {0} after renaming.", e.FullPath));
                }
                if (_resourceTypeExtensionsDict.ContainsKey(extOld))
                {
                    if (!_resourcePaths.Contains(e.OldFullPath))
                    {
                        return;
                    }
                    _resourcePaths.Remove(e.OldFullPath);
                    _logger.LogInfo(string.Format("Old resource {0} removed after renaming.", e.OldFullPath));
                    var type = _resourceTypeExtensionsDict[extOld];

                    switch (type)
                    {
                        case ResourceType.Texture:
                            if (_loadedTexturesDict.ContainsKey(e.OldFullPath))
                            {
                                var resource = _loadedTexturesDict[e.OldFullPath];
                                resource._path = e.FullPath;
                                _loadedTexturesDict.Remove(e.OldFullPath);
                                _loadedTexturesDict.Add(e.FullPath, resource);
                            } // it is not loaded then, no need to add it otherwise
                            break;
                        case ResourceType.SFX:
                            if (!_loadedSoundsDict.ContainsKey(e.OldFullPath))
                            {
                                var resource = _loadedSoundsDict[e.OldFullPath];
                                resource._path = e.FullPath;
                                _loadedSoundsDict.Remove(e.OldFullPath);
                                _loadedSoundsDict.Add(e.FullPath, resource);
                            }
                            break;
                        case ResourceType.Shader:
                            if (!_loadedShadersDict.ContainsKey(e.OldFullPath))
                            {
                                var resource = _loadedShadersDict[e.OldFullPath];
                                resource._path = e.FullPath;
                                _loadedShadersDict.Remove(e.OldFullPath);
                                _loadedShadersDict.Add(e.FullPath, resource);
                            }
                            break;
                        case ResourceType.Font:
                            if (!_loadedFontsDict.ContainsKey(e.OldFullPath))
                            {
                                var resource = _loadedFontsDict[e.OldFullPath];
                                resource._path = e.FullPath;
                                _loadedFontsDict.Remove(e.OldFullPath);
                                _loadedFontsDict.Add(e.FullPath, resource);
                            }
                            break;
                        case ResourceType.Text:
                            _logger.LogWarning("Text files as resources are not implemented yet.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error getting renamed file. Exception: {0}", ex.ToString()));
            }
        }

        public void SearchForResourcesInPath(string basePath)
        {
            var dirs = Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories)
                .Where(x => !(x.Contains(".vs\\") 
                    || x.Contains(".git\\") 
                    || x.Contains("obj\\") 
                    || x.Contains("Docs\\")));

            foreach (var dir in dirs)
            {
                if (_resourceTypeExtensionsDict.ContainsKey(Path.GetExtension(dir)))
                {
                    // long string comparison -> slow, think of something better
                    if (!_resourcePaths.Any(path => path.Equals(dir)))
                    {
                        _resourcePaths.Add(dir);
                    }
                }
            }
        }

        public override void Setup()
        {
            base.Setup();
            var defVertShaderPath = string.Format("{0}\\{1}", _projectDirectory, "\\Assets\\Shaders\\def_vert.vs");
            
            if (!File.Exists(defVertShaderPath))
            {
                _logger.LogWarning("Default vertex shader not loaded. This may lead to issues.");
                return;
            }
            
            _defVertShaderPath = defVertShaderPath;
        }

        public override void Awake()
        {
            SearchForResourcesInPath(_projectDirectory);
            // load resource paths here
        }

        public override void Destroy()
        {
            _watcher.Dispose();
            base.Destroy();
        }

        public bool LoadResource(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                _logger.LogError("Resource path is empty.");
                return false;
            }

            if (!File.Exists(resourcePath))
            {
                _logger.LogError(string.Format("File at path {0} does not exist.", resourcePath));
                return false;
            }

            // load resource based on the file's extensions
            var extension = Path.GetExtension(resourcePath);

            if (string.IsNullOrEmpty(extension))
            {
                _logger.LogError(string.Format("Extension for file at {0} is unknown."));
                return false;
            }

            try
            {
                switch (_resourceTypeExtensionsDict.ContainsKey(extension) ? _resourceTypeExtensionsDict[extension] : ResourceType.Unknown)
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
                    case ResourceType.Text:
                        _logger.LogInfo("Text file selected. Not implemented yet.");
                        break;
                    default:
                        _logger.LogError(string.Format("Extension {0} not supported.", extension));
                        break;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(string.Format("\nFailed to load resource {0}. \nException: {1}", resourcePath, ex.ToString()));
                return false;
            }

            return true;
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
            var fileBytesLength = new FileInfo(resourcePath).Length;

            object placeholder = new object();
            var type = SFX.SoundType.Sound;

            if (fileBytesLength > MAX_SOUND_SIZE) // it should be considered sound until this value
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
                SFX.SoundType.Sound => fileBytesLength / ((Raylib_cs.Sound)placeholder).Stream.SampleSize / 8 * ((Raylib_cs.Sound)placeholder).Stream.SampleRate,
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
            SFXLoaded.Invoke(this, new NewSFXEventArgs() { sfx = sfx });
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

        public bool UnloadResource(Resource resource)
        {
            try
            {
                Action<Resource>? unloadRes = resource.GetType() switch
                {
                    Type tex when tex == typeof(Texture) => delegate(Resource res) { UnloadTexture((res as Texture)!); },
                    Type tex when tex == typeof(SFX) => delegate(Resource res) { UnloadSFX((res as SFX)!); },
                    Type tex when tex == typeof(Shader) => delegate(Resource res) { UnloadShader((res as Shader)!); },
                    Type tex when tex == typeof(Font) => delegate(Resource res) { UnloadFont((res as Font)!); },
                    _ => null
                };

                // should not happen
                if (unloadRes == null)
                {
                    _logger.LogError(string.Format("Unable to unload resource {0}, as it is of an unknown type.", resource._name)); 
                    return false;
                }
                unloadRes.Invoke(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Unable to unload resource {0} because an exception occured. \nException: {1}", resource._name, ex.ToString()));
                return false;
            }

            return true;
        }

        void UnloadTexture(Texture texture)
        {
            try
            {
                Raylib_cs.Raylib.UnloadTexture(texture._texture);
            }
            catch
            {
                throw;
            }
            _loadedTexturesDict.Remove(texture._path);
        }

        void UnloadSFX(SFX sfx)
        {
            try
            {
                if (sfx._type == SFX.SoundType.Sound)
                {
                    Raylib_cs.Raylib.UnloadSound((Raylib_cs.Sound)sfx._value);
                }
                else
                {
                    Raylib_cs.Raylib.UnloadMusicStream((Raylib_cs.Music)sfx._value);
                }
                SFXUnloaded.Invoke(this, new SFXEventArgs() { _soundResourceId = sfx._id });
            }
            catch
            {
                throw;
            }
            _loadedSoundsDict.Remove(sfx._path);
        }

        void UnloadShader(Shader shader)
        {
            try
            {
                Raylib_cs.Raylib.UnloadShader(shader._shader);
            }
            catch
            {
                throw;
            }
            _loadedShadersDict.Remove(shader._path);
        }

        void UnloadFont(Font font)
        {
            try
            {
                Raylib_cs.Raylib.UnloadFont(font._font);
            }
            catch
            {
                throw;
            }
            _loadedFontsDict.Remove(font._path);
        }

        public delegate void NewSFXLoaded(object sender, NewSFXEventArgs e);
        public event NewSFXLoaded SFXLoaded;

        public delegate void LoadedSFXUnloaded(object sender, SFXEventArgs e);
        public event LoadedSFXUnloaded SFXUnloaded;

        private enum ResourceType
        {
            Texture,
            SFX,
            Shader,
            Font,
            Text,
            Unknown
        }
    }
}

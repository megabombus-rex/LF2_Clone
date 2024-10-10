using LF2Clone.Base;
using LF2Clone.Exceptions;
using LF2Clone.Misc.Logger;
using Newtonsoft.Json;

namespace LF2Clone.Systems
{
    public class SceneManager : System<SceneManager> // every system should be instantinated only once
    {
        private List<Scene> _loadedScenesList = new List<Scene>();
        private Scene? _currentScene;
        private Dictionary<int, string> _serializedScenesNamesDict = new();
        private string _scenesFolderPath;

        private ResourceManager _resourceManager;
        private SoundManager _soundManager;

        public SceneManager(ILogger logger, ResourceManager resourceManager, SoundManager soundManager) : base(logger)
        {
            _id = 0;
            _name = "SceneManager";
            _serializedScenesNamesDict = new();
            _scenesFolderPath = string.Empty;
            _resourceManager = resourceManager;
            _soundManager = soundManager;
        }

        public Scene? CurrentScene
        {
            get => _currentScene;
            private set => _currentScene = value;
        }

        public void Setup(string scenesPath)
        {
            base.Setup();
            _scenesFolderPath = scenesPath;
            var sceneFiles = Directory.GetFiles(scenesPath).Where(x => x.EndsWith(".LFsc.json")).ToArray();

            if (string.IsNullOrEmpty(_scenesFolderPath) || sceneFiles.Length < 1)
            {
                _logger.LogError("No scenes found at given path.");
                return;
            }

            foreach (var file in sceneFiles)
            {
                // .LFsc.json - 10 chars
                var sceneName = file.Substring(scenesPath.Length, file.Length - (scenesPath.Length + 10));
                var scene = DeserializeSceneAsync(scenesPath, sceneName).Result;
                if (!_serializedScenesNamesDict.TryAdd(scene._id, scene._name))
                {
                    _logger.LogError(string.Format("Scene with id {0} already exists. Scene {1} id not added.", scene._id, scene._name));
                    continue;
                }
            }

            foreach (var id in _serializedScenesNamesDict.Keys)
            {
                _logger.LogInfo(string.Format("Scene with id {0} is serialized and readable.", id.ToString()));
            }

            _logger.LogDebug("Scene Manager setup finished.");
        }

        public override void Awake()
        {
        }

        public override void Update()
        {
            base.Update();

            if (_currentScene != null)
            {
                _currentScene.Update();
                _currentScene.Draw();
            }
        }

        public override void Destroy()
        {
            TryUnloadSceneAsync(_currentScene._name);
            base.Destroy();
        }

        #region Scene loading


        // it should deserialize a scene, add it to _loadedScenesList
        // returns true if loading was successful, false otherwise
        public async Task<bool> TryLoadSceneAsync(string name)
        {
            if (string.IsNullOrEmpty(_scenesFolderPath))
            {
                _logger.LogError("Scene folder path not found.");
                return false;
            }

            try
            {
                if (_loadedScenesList.Select(x => x._name).Contains(name))
                {
                    _logger.LogError(string.Format("Scene with name {0} is already loaded.", name));
                    return false;
                }

                var scene = await DeserializeSceneAsync(_scenesFolderPath, name);
                if (scene != null)
                {
                    _loadedScenesList.Add(scene);
                    _logger.LogDebug(string.Format("Scene {0} loaded successfuly.", name));
                    return true;
                }
                _logger.LogError(string.Format("Scene {0} was not loaded successfuly.", name));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Exception while opening the file to deserialize. Exception: {0}", ex.ToString()));
                return false;
            }
        }

        public bool TryUnloadScene(int id)
        {
            if (string.IsNullOrEmpty(_scenesFolderPath))
            {
                _logger.LogError("Scene folder path not found.");
                return false;
            }

            Scene scene;

            try
            {
                scene = _loadedScenesList.First(x => x._id == id);
                _logger.LogDebug(string.Format("Found scene with id {0}", _id));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("A scene with name {0}, could not be found. Exception: {1}", id, ex.ToString()));
                return false;
            }

            try
            {
                if (!SerializeSceneAsync(_scenesFolderPath, scene, true).Result)
                {
                    _logger.LogError("Could not serialize a scene. Check if the provided path is correct.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not serialize a scene. Exception: {0}", ex.ToString()));
                return false;
            }

            // is this should be set as null or return false?
            if (_currentScene._id == scene._id)
            {
                _currentScene = null;
            }

            _loadedScenesList.Remove(scene);
            _logger.LogInfo(string.Format("Scene {0} unloaded.", scene._name));
            return true;
        }


        // serialize the scene, remove it from _loadedScenesList
        // returns true if unloading was successful, false otherwise
        public async Task<bool> TryUnloadSceneAsync(string name)
        {
            if (string.IsNullOrEmpty(_scenesFolderPath))
            {
                _logger.LogError("Scene folder path not found.");
                return false;
            }

            Scene scene;

            try
            {
                scene = _loadedScenesList.First(x => x._name == name);
                _logger.LogDebug(string.Format("Found scene with the name {0}", name));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("A scene with name {0}, could not be found. Exception: {1}", name, ex.ToString()));
                return false;
            }

            try
            {
                if (!await SerializeSceneAsync(_scenesFolderPath, scene, true))
                {
                    _logger.LogError("Could not serialize a scene. Check if the provided path is correct.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not serialize a scene. Exception: {0}", ex.ToString()));
                return false;
            }

            // is this should be set as null or return false?
            if (_currentScene._id == scene._id)
            {
                _currentScene = null;
            }

            _loadedScenesList.Remove(scene);
            _logger.LogInfo(string.Format("Scene {0} unloaded.", name));
            return true;
        }

        // returns true if the scene was serialized successfuly, otherwise false
        private async Task<bool> SerializeSceneAsync(string path, Scene scene, bool overwrite = false)
        {
            try
            {
                var attributes = File.GetAttributes(path);
                //LFsc - LF scene
                var filename = string.Format("{0}\\{1}.LFsc.json", path, scene._name);

                switch (attributes)
                {
                    case FileAttributes.Directory:
                        if (Directory.Exists(path))
                        {
                            if (File.Exists(filename) && overwrite || !File.Exists(filename))
                            {
                                using (var sw = new StreamWriter(filename))
                                {
                                    await sw.WriteAsync(JsonConvert.SerializeObject(scene, Formatting.Indented, new JsonSerializerSettings()
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                    }));
                                    sw.Dispose();
                                }
                                _logger.LogDebug(string.Format("File written as {0}", filename));
                                return true;
                            }
                        }
                        _logger.LogError("Directory not found.");
                        return false;
                    default:
                        if (File.Exists(path) || File.Exists(filename))
                        {
                            _logger.LogWarning("There is already a file with a name like this.");
                            if (overwrite)
                            {
                                using (var sw = new StreamWriter(filename))
                                {
                                    await sw.WriteAsync(JsonConvert.SerializeObject(scene, Formatting.Indented, new JsonSerializerSettings()
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                    }));
                                    sw.Dispose();
                                }
                                _logger.LogDebug(string.Format("File written as {0}", filename));
                            }
                            return true;
                        }
                        return false;
                }
            }
            catch
            {
                throw;
            }
        }

        private async Task<Scene?> DeserializeSceneAsync(string path, string name)
        {
            var filename = string.Format("{0}\\{1}.LFsc.json", path, name);
            Scene? scene = null;

            try
            {
                if (File.Exists(filename))
                {
                    using (var sr = new StreamReader(filename))
                    {
                        var read = await sr.ReadToEndAsync();

                        scene = JsonConvert.DeserializeObject<Scene>(read, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    }
                    try
                    {
                        if (scene != null) JoinNodesFromScene(scene);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }

            return scene;
        }

        // Used for joining the nodes after serialization of a scene (param).
        private void JoinNodesFromScene(Scene scene)
        {
            scene._root = scene._nodes.FirstOrDefault(x => x._id == 0)!; // root
            try
            {
                var nodeList = scene._nodes;
                foreach (var node in nodeList)
                {
                    var parent = scene._nodes.FirstOrDefault(x => x._id == node._parentId);
                    node.SetParent(parent);
                    if(node._id == 0)
                    {
                        continue; // do not add _root as a child of _root
                    }
                    node.GetParent().AddChild(node);
                }
            }
            catch
            {
                throw;
            }
        }

        public void ShowLoadedScenes()
        {
            foreach (var scene in _loadedScenesList)
            {
                _logger.LogInfo(scene._name);
            }

            if (_currentScene != null)
            {
                _logger.LogInfo(string.Format("Currently loaded scene: {0}", _currentScene._name));
            }
        }

        #endregion

        #region Scene switching

        public string[] SceneNames
        {
            get => _serializedScenesNamesDict.Values.ToArray();
        }

        public int[] SceneIds
        {
            get => _serializedScenesNamesDict.Keys.ToArray();
        }

        public void AwakeCurrentScene()
        {
            if (_currentScene == null)
            {
                _logger.LogWarning("No scene is currently loaded.");
                return;
            }
            _currentScene.Awake();
        }

        public async Task<bool> TrySetCurrentSceneAsync(int id)
        {
            if (!_serializedScenesNamesDict.ContainsKey(id))
            {
                _logger.LogError(string.Format("Scene with id {0} does not exist or is not tracked.", id.ToString()));
                return false;
            }
            else
            {
                if (!TryLoadSceneAsync(_serializedScenesNamesDict[id]).Result)
                {
                    _logger.LogInfo(string.Format("Scene with id {0} could not be loaded.", id.ToString()));
                }
            }

            var scene = _loadedScenesList.FirstOrDefault(x => x._id == id);
            if (!await TryChangeSceneAsync(scene))
            {
                _logger.LogError(string.Format("Scene with id {0} is not loaded.", id.ToString()));
                return false;
            }
            return true;
        }

        public async Task<bool> TrySetCurrentSceneAsync(string name)
        {
            if (!_serializedScenesNamesDict.ContainsValue(name))
            {
                _logger.LogError(string.Format("Scene with name {0} does not exist or is not tracked.", name));
                return false;
            }
            else
            {
                if (!TryLoadSceneAsync(name).Result)
                {
                    _logger.LogInfo(string.Format("Scene with name {0} could not be loaded.", name));
                }
            }

            var scene = _loadedScenesList.FirstOrDefault(x => x._name == name);
            if (!await TryChangeSceneAsync(scene))
            {
                _logger.LogInfo(string.Format("Scene with name {0} is not loaded.", name));
                return false;
            }
            _currentScene._nodes.ForEach(x => { if (x.GetLoggingCount() < 1) x.MessageSent += _logger.LogFromExternal; });
            _currentScene._nodes.ForEach(x => x.LogMessage(string.Format("Logger connected to Node {0}.", x._name)));
            return true;
        }

        private async Task<bool> TryChangeSceneAsync(Scene? scene)
        {
            if (scene == null)
            {
                return false;
            }

            if(_currentScene != null)
            {
                if (_currentScene._name != scene._name)
                {
                    await TryUnloadSceneAsync(_currentScene._name);
                }
            }
            _currentScene = scene;
            _logger.LogInfo(string.Format("Current scene loaded: {0}", scene._name));
            return true;
        }

        #endregion

        #region Scene edition

        public bool TryAddNewScene(Scene scene)
        {
            if (string.IsNullOrEmpty(_scenesFolderPath))
            {
                _logger.LogError("Scenes folder is not specified. Adding new scene aborted.");
                return false;
            }

            if (_serializedScenesNamesDict.ContainsKey(scene._id))
            {
                _logger.LogError(string.Format("Scene with id {0} already exists.", scene._id));
                return false;
            }

            if (_serializedScenesNamesDict.ContainsValue(scene._name))
            {
                _logger.LogError(string.Format("Scene with name {0} already exists.", scene._name));
                return false;
            }

            try
            {
                if (!SerializeSceneAsync(_scenesFolderPath, scene, true).Result)
                {
                    _logger.LogError("Could not serialize a scene. Check if the provided path is correct.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not serialize a scene. Exception: {0}", ex.ToString()));
                return false;
            }

            _serializedScenesNamesDict.Add(scene._id, scene._name);
            return true;
        }

        public bool TryDeleteScene(int id)
        {
            if (!_serializedScenesNamesDict.ContainsKey(id))
            {
                _logger.LogError(string.Format("Scene with id {0} does not exist.", id.ToString()));
                return false;
            }

            var sceneName = _serializedScenesNamesDict[id];
            var filename = string.Format("{0}\\{1}.LFsc.json", _scenesFolderPath, sceneName);
            bool anySceneChanged = false;
            // remove file and the serialized scene version
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    _serializedScenesNamesDict.Remove(id);
                    anySceneChanged = true;
                }
                else
                {
                    _logger.LogInfo(string.Format("Scene {0} is not serialized.", sceneName));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Exception while deleting file {0}: {1}", filename, ex.ToString()));
                return false;
            }

            // remove the deserialized version
            var scene = _loadedScenesList.Where(x => x._id == id).FirstOrDefault();
            if (scene != null)
            {
                if (_currentScene != null && _currentScene._id == id)
                {
                    _currentScene = null;
                }

                _loadedScenesList.Remove(scene);
                anySceneChanged = true;
            }
            return anySceneChanged;
        }

        public bool TryReparentNode(int newParentId, int nodeId)
        {
            if (newParentId == nodeId)
            {
                _logger.LogError("Node cannot be it's own parent.");
                return false;
            }

            try
            {
                var parent = _currentScene.GetNodeById(newParentId);
                _currentScene.ReparentNode(parent, nodeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Exception while reparenting. Exception: {0}", ex.ToString()));
                return false;
            }
        }

        #endregion

    }
}

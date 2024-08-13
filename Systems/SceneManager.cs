using LF2Clone.Base;
using LF2Clone.Misc.Logger;
using Newtonsoft.Json;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace LF2Clone.Systems
{
    public class SceneManager : System<SceneManager>
    {
        private List<Scene> _loadedScenesList = new List<Scene>();

        private Scene? _currentScene;

        private Dictionary<int, string> _serializedScenesNamesDict = new();

        private string _scenesFolderPath;

        public SceneManager()
        {
            _id = 0;
            _name = "SceneManager";
            _serializedScenesNamesDict = new();
            _scenesFolderPath = string.Empty;
        }

        public string ScenesFolderPath 
        { 
            get { return _scenesFolderPath; } 
            set { _scenesFolderPath = value; } 
        }

        public Scene? CurrentScene
        {
            get => _currentScene;
            private set => _currentScene = value;
        }

        public void Setup(ILogger logger, string scenesPath)
        {
            base.Setup(logger);
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
                var scene = DeserializeScene(scenesPath, sceneName);
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
        }

        public void ShowLoadedScenes()
        {
            foreach (var scene in _loadedScenesList)
            {
                _logger.LogInfo(scene._name);
            }
        }

        public bool TrySetCurrentScene(int id)
        {
            if (!_serializedScenesNamesDict.ContainsKey(id))
            {
                _logger.LogError(string.Format("Scene with id {0} does not exist or is not tracked.", id.ToString()));
                return false;
            }
            else
            {
                if (!TryLoadScene(_serializedScenesNamesDict[id]))
                {
                    _logger.LogInfo(string.Format("Scene with id {0} could not be loaded.", id.ToString()));
                }
            }

            var scene = _loadedScenesList.FirstOrDefault(x => x._id == id);
            if (!TryChangeScene(scene))
            {
                _logger.LogError(string.Format("Scene with id {0} is not loaded.", id.ToString()));
                return false;
            }
            return true;
        }

        public bool TrySetCurrentScene(string name)
        {
            if (!_serializedScenesNamesDict.ContainsValue(name))
            {
                _logger.LogError(string.Format("Scene with name {0} does not exist or is not tracked.", name));
                return false;
            }
            else
            {
                if (!TryLoadScene(name))
                {
                    _logger.LogInfo(string.Format("Scene with id {0} could not be loaded.", name));
                }
            }

            var scene = _loadedScenesList.FirstOrDefault(x => x._name == name);
            if (!TryChangeScene(scene))
            {
                _logger.LogInfo(string.Format("Scene with name {0} is not loaded.", name));
                return false;
            }
            return false;
        }

        private bool TryChangeScene(Scene? scene)
        {
            if (scene == null)
            {
                return false;
            }
            var currentName = scene._name;
            _currentScene = scene;
            _logger.LogInfo(string.Format("Current scene loaded: {0}", scene._name));

            if (!string.IsNullOrEmpty(currentName) && _currentScene._name != scene._name)
            {
                TryUnloadScene(currentName);
            }
            return true;
        }

        // it should deserialize a scene, add it to _loadedScenesList
        // returns true if loading was successful, false otherwise
        public bool TryLoadScene(string name)
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

                var scene = DeserializeScene(_scenesFolderPath, name);
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
                _logger.LogError(string.Format("Exception while opening the file to deserialize. Exception: {0}", ex.Message));
                return false;
            }
        }

        // serialize the scene, remove it from _loadedScenesList
        // returns true if unloading was successful, false otherwise
        public bool TryUnloadScene(string name)
        {
            if(string.IsNullOrEmpty(_scenesFolderPath))
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
                _logger.LogError(string.Format("A scene with name {0}, could not be found. Exception: {1}", name, ex.Message));
                return false;
            }

            try
            {
                if (!SerializeScene(_scenesFolderPath, scene, true))
                {
                    _logger.LogError("Could not serialize a scene. Check if the provided path is correct.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not serialize a scene. Exception: {0}", ex.Message));
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
        private bool SerializeScene(string path, Scene scene, bool overwrite = false)
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
                            if(File.Exists(filename) && overwrite || !File.Exists(filename))
                            {
                                using (var sw = new StreamWriter(filename))
                                {
                                    sw.Write(JsonConvert.SerializeObject(scene, Formatting.Indented));
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
                                    sw.Write(JsonConvert.SerializeObject(scene, Formatting.Indented));
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

        public Scene? DeserializeScene(string path, string name)
        {
            var filename = string.Format("{0}\\{1}.LFsc.json", path, name);
            Scene? scene = null;

            try
            {
                if (File.Exists(filename))
                {
                    using (var sr = new StreamReader(filename))
                    {
                        var read = sr.ReadToEnd();
                        scene = JsonConvert.DeserializeObject<Scene>(read);
                    }
                }
            }
            catch
            {
                throw;
            }

            return scene;
        }

    }
}

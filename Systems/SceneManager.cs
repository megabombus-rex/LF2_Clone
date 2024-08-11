using LF2Clone.Base;
using Newtonsoft.Json;

namespace LF2Clone.Systems
{
    public class SceneManager : System<SceneManager>
    {
        private List<Scene> _loadedScenesList = new List<Scene>();

        private Scene? _currentScene;

        private List<int> _scenesIds = new List<int>();
        private string _scenesFolderPath;

        public SceneManager()
        {
            _id = 0;
            _name = "SceneManager";
            _scenesIds = new List<int>();
            _scenesFolderPath = string.Empty;
        }

        public string ScenesFolderPath 
        { 
            get { return _scenesFolderPath; } 
            set { _scenesFolderPath = value; } 
        }


        public void ShowLoadedScenes()
        {
            foreach (var scene in _loadedScenesList)
            {
                _logger.LogInfo(scene._name);
            }
        }
        public void SetCurrentScene(int id)
        {
            var scene = _loadedScenesList.FirstOrDefault(x => x._id == id);
            if (scene == null)
            {
                _logger.LogError(string.Format("Scene with id {0} not found.", id));
                return;
            }
            _currentScene = scene;
            _logger.LogInfo(string.Format("Current scene loaded: {0}", scene._name));
        }

        public void SetCurrentScene(string name)
        {
            _currentScene = _loadedScenesList.FirstOrDefault(x => x._name == name);
            if (_currentScene == null)
            {
                _logger.LogError(string.Format("Scene with name {0} not found.", name));
            }
        }

        // it should deserialize a scene, add it to _loadedScenesList
        public void LoadScene(string name)
        {
            if (string.IsNullOrEmpty(_scenesFolderPath))
            {
                _logger.LogError("Scene folder path not found.");
                return;
            }

            try
            {
                var scene = DeserializeScene(_scenesFolderPath, name);
                if (scene != null)
                {
                    _loadedScenesList.Add(scene);
                    _logger.LogDebug(string.Format("Scene {0} loaded successfuly.", name));
                    return;
                }
                _logger.LogError(string.Format("Scene {0} was not loaded successfuly.", name));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Exception while opening the file to deserialize. Exception: {0}", ex.Message));
                return;
            }
        }

        // serialize the scene, remove it from _loadedScenesList
        public void UnloadScene(string name)
        {
            if(string.IsNullOrEmpty(_scenesFolderPath))
            {
                _logger.LogError("Scene folder path not found.");
                return;
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
                return;
            }

            try
            {
                if (!SerializeScene(_scenesFolderPath, scene, true))
                {
                    _logger.LogError("Could not serialize a scene. Check if the provided path is correct.");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not serialize a scene. Exception: {0}", ex.Message));
                return;
            }

            
            _loadedScenesList.Remove(scene);
            _logger.LogInfo(string.Format("Scene {0} unloaded.", name));
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

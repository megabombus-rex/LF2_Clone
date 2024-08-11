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

    public void SetCurrentScene(int id)
        {
            _currentScene = _loadedScenesList.FirstOrDefault(x => x._id == id);
            if (_currentScene == null)
            {
                _logger.LogError(string.Format("Scene with id {0} not found.", id));
            }
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
                _logger.LogInfo(string.Format("Found scene with the name {0}", name));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Scene with name {0}, could not be found. Exception: {1}", name, ex.Message));
                return;
            }

            try
            {
                SerializeScene(_scenesFolderPath, scene, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not serialize a scene. Exception: {0}", ex.Message));
                return;
            }

            
            _loadedScenesList.Remove(scene);
        }

        private void SerializeScene(string path, Scene scene, bool overwrite = false)
        {
            try
            {
                var attributes = File.GetAttributes(path);
                var filename = string.Format("{0}\\{1}.json", path, scene._name);

                switch (attributes)
                {
                    case FileAttributes.Directory:
                        if (Directory.Exists(path))
                        {
                            if(File.Exists(filename) && overwrite || !File.Exists(filename))
                            {
                                var sw = new StreamWriter(filename);
                                sw.Write(JsonConvert.SerializeObject(scene, Formatting.Indented));
                                sw.Dispose();
                                _logger.LogInfo(string.Format("File written as {0}", filename));
                                return;
                            }
                        }
                        _logger.LogError("Directory not found.");
                        return;
                    default:
                        if (File.Exists(path) || File.Exists(filename))
                        {
                            _logger.LogWarning("There is already a file with a name like this.");
                            if (overwrite)
                            {
                                var sw = new StreamWriter(filename);
                                sw.Write(JsonConvert.SerializeObject(scene, Formatting.Indented));
                                sw.Dispose();
                                _logger.LogInfo(string.Format("File written as {0}", filename));
                            }
                            return;
                        }
                        break;
                }
            }
            catch  (Exception ex)
            {
                _logger.LogError(string.Format("Exception finding the directory", ex.Message));
                throw;
            }
        }

        //public static Scene DeserializeScene(string path)
        //{
        //    
        //}

    }
}

using LF2Clone.Base;
using Newtonsoft.Json;

namespace LF2Clone.Systems
{
    public class SceneManager : System<SceneManager>
    {
        private List<Scene> _loadedScenesList = new List<Scene>();

        private Scene? _currentScene;

        private List<int> _scenesIds = new List<int>();

        public SceneManager()
        {
            _id = 0;
            _name = "SceneManager";
            _scenesIds = new List<int>();
        }

    public void SetCurrentScene(int id)
        {
            _currentScene = _loadedScenesList.FirstOrDefault(x => x._id == id);
            if (_currentScene == null)
            {
                Console.WriteLine(string.Format("Scene with id {0} not found.", id));
            }
        }

        public void SetCurrentScene(string name)
        {
            _currentScene = _loadedScenesList.FirstOrDefault(x => x._name == name);
            if (_currentScene == null)
            {
                Console.WriteLine(string.Format("Scene with name {0} not found.", name));
            }
        }

        // it should deserialize a scene
        public void LoadScene(string name)
        {

        }

        public void UnloadScene(string name)
        {

        }

        public void SerializeScene(string path, Scene scene, bool overwrite = false)
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
                                Console.WriteLine(string.Format("File written as {0}", filename));
                                return;
                            }
                        }
                        Console.WriteLine("Directory not found.");
                        return;
                    default:
                        if (File.Exists(path) || File.Exists(filename))
                        {
                            Console.WriteLine("There is already a file with a name like this.");
                            if (overwrite)
                            {
                                var sw = new StreamWriter(filename);
                                sw.Write(JsonConvert.SerializeObject(scene, Formatting.Indented));
                                sw.Dispose();
                                Console.WriteLine(string.Format("File written as {0}", filename));
                            }
                            return;
                        }
                        break;
                }
            }
            catch  (Exception ex)
            {
                Console.WriteLine(string.Format("Exception finding the directory", ex.Message));
                return;
            }
        }

        //public static Scene DeserializeScene(string path)
        //{
        //    
        //}

    }
}

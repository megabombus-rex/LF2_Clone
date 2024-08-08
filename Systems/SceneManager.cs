using LF2Clone.Base;

namespace LF2Clone.Systems
{
    public class SceneManager : System<SceneManager>
    {
        private List<Scene> loadedScenesList = new List<Scene>();

        private Scene? currentScene;

        public void SetCurrentScene(int id)
        {
            currentScene = loadedScenesList.FirstOrDefault(x => x._id == id);
            if (currentScene == null)
            {
                Console.WriteLine(string.Format("Scene with id {0} not found.", id));
            }
        }

        public void SetCurrentScene(string name)
        {
            currentScene = loadedScenesList.FirstOrDefault(x => x._name == name);
            if (currentScene == null)
            {
                Console.WriteLine(string.Format("Scene with name {0} not found.", name));
            }
        }

        public void LoadScene(string name)
        {

        }

        public void UnloadScene(string name)
        {

        }

    }
}

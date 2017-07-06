using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public static class SceneManager
    {
        internal static List<Scene> Scenes { get; private set; } = new List<Scene>();
        internal static int currentScene { get; private set; } = 0;

        internal static void RenderCurrentScene(IDisplay display) {
            Scenes[currentScene].RenderScene(display);
        }

        public static void AddScene(Scene scene)
        {
            Scenes.Add(scene);
        }

        internal static void UpdateScene(double deltaTime, bool beforeRender)
        {
            Scenes[currentScene].Update(deltaTime, beforeRender);
        }

        public static void GoToScene(int sceneNumber)
        {
            currentScene = sceneNumber;
        }
    }

    public struct Scene
    {
        public List<GameObject> Objects { get; private set; }

        public Scene(params GameObject[] objects)
        {
            Objects = new List<GameObject>(objects);
        }

        internal void addInstance(GameObject obj)
        {
            Objects.Add(obj);
        }

        internal void RenderScene(IDisplay display) {
            display.Render(Objects.Select(x => x.renderer).ToArray());
        }

        internal void Update(double deltaTime, bool beforeRender)
        {
            for (int i = 0; i < Objects.Count; i++) {
                //Console.WriteLine("Object " + i + " updating with deltaTime " + deltaTime);
                for (int j = 0; j < Objects[i].Behaviours.Count; j++) {
                    if (beforeRender) Objects[i].Behaviours[j].Update();
                    else Objects[i].Behaviours[j].LateUpdate();
                }
            }
        }
    }
}

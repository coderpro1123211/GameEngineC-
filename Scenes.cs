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

        public static Scene CurrentScene {
            get {
                return Scenes[currentScene];
            }
        }

        internal static void RenderCurrentScene(IDisplay display)
        {
            if (Scenes[currentScene].Objects != null)
            {
                Scenes[currentScene].RenderScene(display);
            }
        }

        public static void AddScene(Scene scene)
        {
            Scenes.Add(scene);
        }

        internal static void UpdateScene(double deltaTime, bool beforeRender)
        {
            if (Scenes[currentScene].Objects != null)
            {
                Scenes[currentScene].Update(deltaTime, beforeRender);
            }
        }

        public static void GoToScene(int sceneNumber)
        {
            currentScene = sceneNumber;
        }

        public static void LoadMap(int id)
        {
            Console.WriteLine("Loading map");
            Map map = MapManager.Maps[id];
            
            for (int x=0;x<map.mapData.GetLength(0);x++)
            {
                for (int y = 0; y < map.mapData.GetLength(1); y++)
                {
                    Console.WriteLine("Doi");
                    if (map.mapData[x, y] != null)
                    {
                        Scenes[currentScene].addInstance((GameObject)Activator.CreateInstance(map.mapData[x, y], x, y));
                    }
                }
            }
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

                    Objects[i].UpdateClipPos();
                }
            }
        }
    }
}

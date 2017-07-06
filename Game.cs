using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace GameEngine
{
    public static class Game
    {
        public static bool IsRunning {get; private set;}
        static Thread worker;
        static Stopwatch timer;
        internal static List<ConsoleKeyInfo> keys;
        internal static IDisplay display;

        public static void Init()
        {
            IsRunning = true;
            display = new AsciiDisplay();

            //Start da gaem
            Start();
        }

        internal static void Start()
        {
            SceneManager.AddScene(new Scene(new GameObject[] {new GameObject(new AsciiRenderable(ConsoleColor.Red, ConsoleColor.Blue, 85/20, 25/20, new char[,] {{'§'}}), 0, 0)}));
            keys = new List<ConsoleKeyInfo>();
            Console.CursorVisible = false;
            timer = new Stopwatch();
            worker = new Thread(WorkerLoop);
            worker.Start();
        }

        public static void Instantiate(GameObject obj)
        {
            Events.OnCreateInstance(obj);
            SceneManager.Scenes[SceneManager.currentScene].addInstance(obj);
        }

        public static void Destroy(GameObject obj)
        {
            Events.OnDestroyInstance(obj);

            //Remove the object
            SceneManager.Scenes[SceneManager.currentScene].Objects.Remove(obj);
        }

        internal static void WorkerLoop() {
            while(IsRunning)
            {
                timer.Reset();
                timer.Start();

                keys.Clear();
                if (Console.KeyAvailable) {
                    while (Console.KeyAvailable) keys.Add(Console.ReadKey(false));
                }
                InputManager.keys = keys;

                Console.WriteLine(InputManager.GetKeyPressed(ConsoleKey.LeftArrow));

#region Thread Work
                SceneManager.UpdateScene(Time.deltaTime, true);

                // Do rendering here
                //Console.BackgroundColor = ConsoleColor.Black;
                //Console.Clear();
                //SceneManager.RenderCurrentScene(display);

                SceneManager.UpdateScene(Time.deltaTime, true);
                Thread.Sleep(100);
                #endregion
                InputManager.lastKeys.Clear();
                InputManager.lastKeys.AddRange(keys);
                timer.Stop();
                Time.deltaTime = timer.ElapsedMilliseconds / 1000f;
            }
        }
    }

    public static class InputManager {
        internal static List<ConsoleKeyInfo> lastKeys;
        internal static List<ConsoleKeyInfo> keys;

        public static bool AnyKeyDown {
            get {
                return keys.Count() > 0;
            }
        }

        public static bool GetKey(ConsoleKey key) {
            return (from k in keys where k.Key == key select k).Count() > 0;
        }

        public static bool GetKeyPressed(ConsoleKey key)
        {
            return ((from k in keys where k.Key == key select k).Count() > 0) && ((from k in lastKeys where k.Key == key select k).Count() == 0);
        }
    }

    public static class Time
    {
        public static float deltaTime {get;internal set;}
    }
}

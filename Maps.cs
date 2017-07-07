using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameEngine
{
    public static class MapManager
    {
        static List<Map> maps;

        public static int loadMapFromFile(string path,List<Type> mapInsts)
        {
            string FileData = System.IO.File.ReadAllText(path);
            int width = (int)FileData[0];
            int height = (int)FileData[4];
            Console.Write("Width: " + width + "   Height: " + height);
            Console.ReadLine();
        }
    }

    internal struct Map
    {
        public Map(Type[,] mapData)
        {
            foreach(Type i in mapData)
            {
                SceneManager.Scenes[SceneManager.currentScene].addInstance((GameObject)Activator.CreateInstance(i));
            }
        }
    }
}

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
        private static List<Map> maps = new List<Map>();

        internal static List<Map> Maps { get => maps; set => maps = value; }

        public static int loadMapFromFile(string path, Type[] mapInsts) {
            Console.WriteLine(mapInsts[0]);
            System.IO.File.SetAttributes(path, System.IO.FileAttributes.Normal);
            var fileData = System.IO.File.ReadAllText(path);
            int width = int.Parse(fileData[0].ToString());
            int height = int.Parse(fileData[3].ToString());
            Console.WriteLine("Width: " + width + "  Height: " + height);
            int[,] mapData = new int[width, height];

            fileData = fileData.Substring(6).Replace(" ", "").Replace(Environment.NewLine, "");
            string[] aFileData = fileData.Split(',');

            for (int y = 0; y < height; y++)
            { 
                for (int x=0;x<width;x++)
                {
                    mapData[x, y] = int.Parse(aFileData[x+(y*height)]);
                }
            }

            Maps.Add(new Map(mapData, mapInsts));
            return Maps.Count()-1;
        }
    }

    internal struct Map
    {
        public Type[,] mapData;

        public Map(Type[,] mapData)
        {
            this.mapData = mapData;
        }

        public Map(int[,] mapData, Type[] mapInsts)
        {
            this.mapData = new Type[mapData.GetLength(0), mapData.GetLength(1)];
            for (int x=0;x<mapData.GetLength(0);x++)
            {
                for (int y=0;y<mapData.GetLength(1);y++)
                {
                    if (mapData[x, y] != -1)
                    {
                        this.mapData[x, y] = mapInsts[mapData[x, y]];
                    } else
                    {
                        this.mapData[x, y] = null;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            MapManager.loadMapFromFile("Z:/Oskar/Dev/Games/C# Game Engine/Game engine/Maps/map.txt", new List<Type>());

            Console.WriteLine(Console.BufferHeight + " - " + Console.BufferWidth);
            Console.BufferHeight = Console.WindowHeight = 25;
            Console.BufferWidth = Console.WindowWidth = 85;

            //Start da god damn gaem
            Game.Init();
        }
    }

    //Object classes
    public class Player : Behaviour
    {

        public override void OnCreate()
        {

        }

        public override void Update()
        {
            if (InputManager.GetKey(ConsoleKey.A) && Utils.TileFree(gameObject.position.x - 1, gameObject.position.y))
            {
                gameObject.position.x -= 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.D) && Utils.TileFree(gameObject.position.x + 1, gameObject.position.y))
            {
                gameObject.position.x += 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.W) && Utils.TileFree(gameObject.position.x, gameObject.position.y - 1))
            {
                gameObject.position.y -= 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.S) && Utils.TileFree(gameObject.position.x, gameObject.position.y + 1))
            {
                gameObject.position.y += 5 * Time.deltaTime;
            }
            else
            {
                //gameObject.x += 5 * Time.deltaTime;
            }

            if (InputManager.GetKey(ConsoleKey.J))
            {
                Game.cameraPosition += Position.left * 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.L))
            {
                Game.cameraPosition += Position.right * 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.I))
            {
                Game.cameraPosition += Position.up * 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.K))
            {
                Game.cameraPosition += Position.down * 5 * Time.deltaTime;
            }
        }

        public override void LateUpdate()
        {

        }
    }

    public class Wall : Behaviour
    {
        public override void LateUpdate()
        {

        }

        public override void OnCreate()
        {

        }

        public override void Update()
        {

        }
    }
}

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
            Console.WriteLine(Console.BufferHeight + " - " + Console.BufferWidth);
            Console.BufferHeight = Console.WindowHeight = 25;
            Console.BufferWidth = Console.WindowWidth = 85;

            //Start da god damn gaem
            Game.Init();
        }
    }

    //Object classes
    public class PlayerBehaviour: Behaviour
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

            if (InputManager.GetKey(ConsoleKey.LeftArrow))
            {
                Game.cameraPosition += Position.left * 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.RightArrow))
            {
                Game.cameraPosition += Position.right * 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.UpArrow))
            {
                Game.cameraPosition += Position.up * 5 * Time.deltaTime;
            }
            else if (InputManager.GetKey(ConsoleKey.DownArrow))
            {
                Game.cameraPosition += Position.down * 5 * Time.deltaTime;
            }
        }

        public override void LateUpdate()
        {

        }
    }

    public class Player: GameObject
    {
        public Player(float x, float y) : base(new AsciiRenderable(ConsoleColor.Green, ConsoleColor.Black, 85 / 2, 25 / 2, new char[,] { { '§', '#', '§' }, { '#', '#', '#' }, { '§', '#', '§' } }), x, y, new PlayerBehaviour())
        {

        }
    }

    public class WallBehaviour : Behaviour
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

    public class Wall : GameObject
    {
        public Wall(float x, float y) : base(new AsciiRenderable(ConsoleColor.Gray, ConsoleColor.Black, 85 / 2, 25 / 2, new char[,] { { '¤' } }), x, y, new WallBehaviour(), Builtin.Collision)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public interface IDisplay
    {
        bool Render(IRenderable[] objects);
    }

    internal class AsciiDisplay : IDisplay
    {
        public static AsciiDisplay Instance {get; private set;}
        
        public int width = 85, height = 25, cameraX = 0, cameraY = 0, cameraSize = 1;

        public AsciiDisplay() {Instance = this;}

        public AsciiDisplay(int width, int height) {
            this.width = width;
            this.height = height;
            Instance = this;
        }

        public bool Render(IRenderable[] objects) {

            for(int i = 0; i < objects.Length; i++) {
                AsciiRenderData data = (AsciiRenderData)objects[i].GetGraphic();
                Console.SetCursorPosition((int)(data.x*10) - cameraX, (int)(data.y*10) + cameraY);
                Console.ForegroundColor = data.fg;
                Console.BackgroundColor = data.bg;
                Console.Write(data.glyph[0,0]);
            }

            return true;
        }
    }

    internal class AsciiRenderable : IRenderable {
        AsciiRenderData data;

        public AsciiRenderable(ConsoleColor fg, ConsoleColor bg, float x, float y, char[,] glyph) {
            data = new AsciiRenderData(fg, bg, x, y, glyph);
        }

        public object GetGraphic() {
            return (object)data;
        }
    }

    internal struct AsciiRenderData {
        public ConsoleColor fg, bg;
        public float x, y;
        public char[,] glyph;

        public AsciiRenderData(ConsoleColor fg, ConsoleColor bg, float x, float y, char[,] glyph) {
            this.glyph = glyph;
            this.fg = fg;
            this.bg = bg;
            this.x = x;
            this.y = y;
        }
    }

    public interface IRenderable {
        object GetGraphic();
    }

    public class GameObject {
        internal List<Behaviour> Behaviours {get; private set;}
        public float x,y;
        public IRenderable renderer;

        public GameObject(IRenderable renderer, float x, float y, params Behaviour[] behaviours) {
            this.renderer = renderer;
            this.x = x;
            this.y = y;
            Behaviours = new List<Behaviour>(behaviours);
            for (int i = 0; i < behaviours.Length; i++) {
                behaviours[i].OnCreate();
            }
        }
    }

    public abstract class Behaviour {
        public abstract void OnCreate();
        public abstract void Update();
        public abstract void LateUpdate();
    }
}

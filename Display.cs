using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoubleBuffer;
using Buffer = DoubleBuffer.Buffer;

namespace GameEngine
{
    public interface IDisplay {
        bool Render(IRenderable[] objects);
        void SetCameraPosition(Position pos);
        Position GetCameraPosition();
    }

    internal class AsciiDisplay : IDisplay {
        public static AsciiDisplay Instance {get; private set;}
        Buffer buf;
        
        public int width = 85, height = 25, cameraSize = 1;
        public Position camera;

        public AsciiDisplay() {
            Instance = this;
            buf = new Buffer(width, height, width, height);
        }

        public AsciiDisplay(int width, int height) {
            this.width = width;
            this.height = height;
            buf = new Buffer(width, height, width, height);
            Instance = this;
        }

        public bool Render(IRenderable[] objects) {

            for(int i = 0; i < objects.Length; i++) {
                if (!(objects[i].GetGraphic() is AsciiRenderData)) continue; // Incorrect render data, continue.
                AsciiRenderData data = (AsciiRenderData)objects[i].GetGraphic();
                
                if (data.x - camera.x + Console.BufferWidth / 2 < 0 || data.x - camera.x + Console.BufferWidth / 2 >= Console.BufferWidth || data.y - camera.y + Console.BufferHeight / 2  < 0 || data.y - camera.y + Console.BufferHeight / 2 >= Console.BufferHeight) continue;
                // Console.SetCursorPosition();
                // Console.ForegroundColor = data.fg;
                // Console.BackgroundColor = data.bg;
                // Console.Write(data.glyph[0,0]);

                buf.Draw(""+data.glyph[0,0], (int)(data.x + Console.BufferWidth / 2 - camera.x), (int)(data.y + Console.BufferHeight / 2 + camera.y), (short)((short)data.fg + ((short)data.bg << 4)));
            }

            buf.Print();
            buf.Clear();

            return true;
        }

        public void SetCameraPosition(Position pos) {
            camera = pos;
        }

        public Position GetCameraPosition() {
            return camera;
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

        public void UpdateData(float x, float y) {
            data.x = (int)x;
            data.y = (int)y;
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
        void UpdateData(float x, float y);
    }

    public struct Position {
        public float x,y;
        public Position(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public static Position up {
            get {
                return new Position(0, 1);
            }
        }
        
        public static Position down {
            get {
                return new Position(0, -1);
            }
        }
        
        public static Position right {
            get {
                return new Position(1, 0);
            }
        }
        
        public static Position left {
            get {
                return new Position(-1, 0);
            }
        }

        public static Position operator *(Position left, float right) {
            return new Position(left.x * right, left.y * right);
        }

        public static Position operator /(Position left, float right) {
            return new Position(left.x / right, left.y / right);
        }

        public static Position operator +(Position left, Position right) {
            return new Position(left.x + right.x, left.y + right.y);
        }
    }

    public class GameObject {
        internal List<Behaviour> Behaviours {get; private set;}
        public Position position;
        public IRenderable renderer;

        internal bool hasCollision;

        public GameObject(IRenderable renderer, float x, float y, params Behaviour[] behaviours) {
            this.renderer = renderer;
            this.position = new Position(x, y);
            Behaviours = new List<Behaviour>(behaviours);
            for (int i = 0; i < behaviours.Length; i++) {
                behaviours[i].gameObject = this;
                behaviours[i].OnCreate();
                if (behaviours[i] is Builtin.CollisionComponent) {
                    hasCollision = true;
                }
            }
        }

        internal void UpdateClipPos() {
            renderer.UpdateData(position.x, position.y);
        }
    }

    public static class Builtin {
        public static CollisionComponent Collision {
            get {
                return new CollisionComponent();
            }
        }
        public class CollisionComponent : Behaviour {
            public override void OnCreate() {

            }
            public override void Update() {

            }
            public override void LateUpdate() {

            }
        }
    }

    public abstract class Behaviour {
        public GameObject gameObject;
        
        public abstract void OnCreate();
        public abstract void Update();
        public abstract void LateUpdate();
    }
}

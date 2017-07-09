using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;

namespace GameEngine
{
    public static class External {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow); // DON'T Mess with me
    }

    public static class Game
    {
        public static bool IsRunning {get; private set;}
        static Thread worker;
        static Stopwatch timer;
        internal static List<ConsoleKeyInfo> keys;
        internal static IDisplay display;
        //private NativeMethods.ConsoleHandle _handler;
        
        public static Position cameraPosition {
            get {
                return display.GetCameraPosition();
            }
            set {
                display.SetCameraPosition(value);
            }
        }

        public static void Init()
        {
            IsRunning = true;
            display = new AsciiDisplay();
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            if (!External.ShowScrollBar(handle, 1, false)) return;

            //Start da gaem
            Start();
        }

        internal static void Start()
        {
            Utils.SetConsoleMode();
            SceneManager.AddScene(new Scene(new GameObject[0]));
            Type[] types = new Type[2];
            types[0] = typeof(Player);
            types[1] = typeof(Wall);
            SceneManager.AddScene(new Scene());
            MapManager.loadMapFromFile(@"Maps\map.txt", types);
            MapManager.loadMapFromFile(@"Maps\square.txt", types);
            SceneManager.LoadMap(1, 0, -7);
            SceneManager.LoadMap(0, -3, 0);
            keys = new List<ConsoleKeyInfo>();
            InputManager.lastKeys = new List<ConsoleKeyInfo>();
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
                    while (Console.KeyAvailable) keys.Add(Utils.GetRawInput().key);
                }
                InputManager.keys = keys;

#region Thread Work
                SceneManager.UpdateScene(Time.deltaTime, true);

                // Do rendering here
                Console.BackgroundColor = ConsoleColor.Black;
                SceneManager.RenderCurrentScene(display);

                SceneManager.UpdateScene(Time.deltaTime, true);
                Thread.Sleep(10);    
#endregion

                if (InputManager.lastKeys == null) {
                    InputManager.lastKeys = new List<ConsoleKeyInfo>();
                }

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
            return ((from k in keys where k.Key == key select k).Count() == 1) && ((from k in lastKeys where k.Key == key select k).Count() == 0);
        }

        public static bool GetKeyReleased(ConsoleKey key)
        {
            return ((from k in keys where k.Key == key select k).Count() < 1) && ((from k in lastKeys where k.Key == key select k).Count() > 0);
        }
    }

    public static class Time
    {
        public static float deltaTime { get; internal set; }
    }

    public static class Utils
    {
        private static NativeMethods.ConsoleHandle _handler;
        public static bool TileFree(int x, int y)
        {
            foreach(var pos in (from o in SceneManager.Scenes[SceneManager.currentScene].Objects where o.hasCollision select o.position))
            {
                if ((int)pos.x == x && (int)pos.y == y) return false;
            }
            return true;
        }

        public static bool TileFree(float x, float y)
        {
            foreach(var pos in (from o in SceneManager.Scenes[SceneManager.currentScene].Objects where o.hasCollision select o.position))
            {
                if ((int)pos.x == (int)x && (int)pos.y == (int)y) return false;
            }
            return true;
        }

        public static bool TileFull(int x, int y)
        {
            return !TileFree(x, y);
        }

        public static GameObject GetObjectAtTile(int x, int y)
        {
            for (int i = 0; i < SceneManager.Scenes[SceneManager.currentScene].Objects.Count(); i++)
            {
                if ((int)SceneManager.Scenes[SceneManager.currentScene].Objects[i].position.x == x && (int)SceneManager.Scenes[SceneManager.currentScene].Objects[i].position.y == y)
                {
                    return SceneManager.Scenes[SceneManager.currentScene].Objects[i];
                }
            }
            return null;
        }

        internal static KeyWrapper GetRawInput() 
        {
            UInt32 events = 0;
            NativeMethods.GetNumberOfConsoleInputEvents(_handler, out events);

            if (events > 0)
            {
                var record = new NativeMethods.INPUT_RECORD();
                uint recordLen = 0;
                if (!(NativeMethods.ReadConsoleInput(_handler, ref record, 1, ref recordLen))) { throw new Win32Exception(); }

                return new KeyWrapper(new ConsoleKeyInfo(record.KeyEvent.UnicodeChar, 
                    (ConsoleKey)record.KeyEvent.wVirtualKeyCode,
                    1 << 4 == (record.KeyEvent.dwControlKeyState & 1 << 4), 
                    (1 << 1 == (record.KeyEvent.dwControlKeyState & 1 << 1)) || (1 << 0 == (record.KeyEvent.dwControlKeyState & 1 << 0)), 
                    1 << 3 == (record.KeyEvent.dwControlKeyState & 1 << 3) || (1 << 2 == (record.KeyEvent.dwControlKeyState & 1 << 2))));
            }
            return null;
        }

        internal static void SetConsoleMode()
        {
            _handler = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);

            int mode = 0;
            if (!(NativeMethods.GetConsoleMode(_handler, ref mode))) { throw new Win32Exception(); }

            mode |= NativeMethods.ENABLE_MOUSE_INPUT;
            mode &= ~NativeMethods.ENABLE_QUICK_EDIT_MODE;
            mode |= NativeMethods.ENABLE_EXTENDED_FLAGS;

            if (!(NativeMethods.SetConsoleMode(_handler, mode))) { throw new Win32Exception(); }
        }
    }

    internal class KeyWrapper {
        public ConsoleKeyInfo key;

        public KeyWrapper(ConsoleKeyInfo key) {
            this.key = key;
        }
    }
    
    internal class NativeMethods
    {
        public const Int32 STD_INPUT_HANDLE = -10;

        public const Int32 ENABLE_MOUSE_INPUT = 0x0010;
        public const Int32 ENABLE_QUICK_EDIT_MODE = 0x0040;
        public const Int32 ENABLE_EXTENDED_FLAGS = 0x0080;

        public const Int32 KEY_EVENT = 1;
        public const Int32 MOUSE_EVENT = 2;


        [DebuggerDisplay("EventType: {EventType}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public Int16 EventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
        }

        [DebuggerDisplay("{dwMousePosition.X}, {dwMousePosition.Y}")]
        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public Int32 dwButtonState;
            public Int32 dwControlKeyState;
            public Int32 dwEventFlags;
        }

        [DebuggerDisplay("{X}, {Y}")]
        public struct COORD
        {
            public UInt16 X;
            public UInt16 Y;
        }

        [DebuggerDisplay("KeyCode: {wVirtualKeyCode}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.Bool)]
            public Boolean bKeyDown;
            [FieldOffset(4)]
            public UInt16 wRepeatCount;
            [FieldOffset(6)]
            public UInt16 wVirtualKeyCode;
            [FieldOffset(8)]
            public UInt16 wVirtualScanCode;
            [FieldOffset(10)]
            public Char UnicodeChar;
            [FieldOffset(10)]
            public Byte AsciiChar;
            [FieldOffset(12)]
            public Int32 dwControlKeyState;
        };

        public class ConsoleHandle : SafeHandleMinusOneIsInvalid
        {
            public ConsoleHandle() : base(false) { }

            protected override bool ReleaseHandle()
            {
                return true; // Releasing console handle is not our business
            }
        }
        
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean GetConsoleMode(ConsoleHandle hConsoleHandle, ref Int32 lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern ConsoleHandle GetStdHandle(Int32 nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean ReadConsoleInput(ConsoleHandle hConsoleInput, ref INPUT_RECORD lpBuffer, UInt32 nLength, ref UInt32 lpNumberOfEventsRead);

        //Currently unused
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean PeekConsoleInput(ConsoleHandle hConsoleInput, ref INPUT_RECORD lpBuffer, UInt32 nLength, ref UInt32 lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetConsoleMode(ConsoleHandle hConsoleHandle, Int32 dwMode);

        [DllImport("kernel32.dll")]
        public static extern Boolean GetNumberOfConsoleInputEvents(ConsoleHandle hConsoleInput, out UInt32 lpcNumberOfEvents);
    }
}

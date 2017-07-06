using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GameEngine
{
    public delegate void onCreateInstanceDelegate(GameObject obj);
    public delegate void onDestroyInstanceDelegate(GameObject obj);

    public static class Events
    {
        public static event onCreateInstanceDelegate CreateInstance;
        public static event onDestroyInstanceDelegate DestroyInstance;

        internal static void OnCreateInstance(GameObject obj)
        {
            CreateInstance?.Invoke(obj);
        }

        internal static void OnDestroyInstance(GameObject obj)
        {
            DestroyInstance?.Invoke(obj);
        }
    }
}

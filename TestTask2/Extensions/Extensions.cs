using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TestTask2.Extensions
{
    /// <summary>
    /// Different Extensions for standart classes
    /// </summary>
    public static class Extensions
    {
        //public static object lockobj = new object();

        //public static void LAdd<T>(this HashSet<T> hashset, T value)
        //{
        //    lock (lockobj)
        //    {
        //        hashset.Add(value);
        //    }
        //}

        public static void LAdd<T>(this HashSet<T> hashset, T value, object lockObj)
        {
            lock (lockObj)
            {
                hashset.Add(value);
            }
        }

        //public static void LRemove<T>(this HashSet<T> hashset, T value)
        //{
        //    lock (lockobj)
        //    {
        //        hashset.Remove(value);
        //    }
        //}

        public static void LRemove<T>(this HashSet<T> hashset, T value, object lockObj)
        {
            lock (lockObj)
            {
                hashset.Remove(value);
            }
        }

        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }
    }
}
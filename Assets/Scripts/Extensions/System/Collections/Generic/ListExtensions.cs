using System;
using System.Collections.Generic;

namespace ICouldGames.Extensions.System.Collections.Generic
{
    public static class ListExtensions
    {
        private static readonly Random Random = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            for(var i=list.Count; i > 0; i--)
            {
                list.Swap(0, Random.Next(0, i));
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
using System;
using System.Collections.Generic;

namespace ICouldGames.Dependency.Singly
{
    public class SingletonContainer
    {
        private readonly Dictionary<Type, object> _internalCcontainer = new();

        public void Add<T>(T singleton) where T : class
        {
            _internalCcontainer.Add(typeof(T), singleton);
        }

        public T Get<T>()
        {
            return (T) _internalCcontainer[typeof(T)];
        }
    }
}
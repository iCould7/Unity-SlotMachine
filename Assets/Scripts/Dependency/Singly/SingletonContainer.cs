using System;
using System.Collections.Generic;
using ICouldGames.Common.Interfaces.Startup;

namespace ICouldGames.Dependency.Singly
{
    public class SingletonContainer
    {
        private readonly Dictionary<Type, object> _internalContainer = new();

        public void Add<T>(T singleton) where T : IInitializable
        {
            singleton.Init();
            _internalContainer.Add(typeof(T), singleton);
        }

        public T Get<T>() where T : IInitializable
        {
            return (T) _internalContainer[typeof(T)];
        }
    }
}
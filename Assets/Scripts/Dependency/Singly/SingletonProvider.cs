using System.Collections.Generic;
using ICouldGames.Common.Interfaces.Startup;
using UnityEngine;

namespace ICouldGames.Dependency.Singly
{
    public class SingletonProvider : MonoBehaviour
    {
        public static SingletonProvider Instance;

        [SerializeField] private List<SingletonInstaller> installers;

        private readonly SingletonContainer _container = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            foreach (var installer in installers)
            {
                installer.Install(_container);
            }
        }

        public T Get<T>() where T : IInitializable
        {
            return _container.Get<T>();
        }
    }
}
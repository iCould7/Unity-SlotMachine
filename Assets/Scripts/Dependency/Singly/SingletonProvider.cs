using System.Collections.Generic;
using UnityEngine;

namespace ICouldGames.Dependency.Singly
{
    public class SingletonProvider : MonoBehaviour
    {
        [SerializeField] private List<SingletonInstaller> installers;

        private readonly SingletonContainer _container = new();

        private void Awake()
        {
            foreach (var installer in installers)
            {
                installer.Install(_container);
            }
        }
    }
}
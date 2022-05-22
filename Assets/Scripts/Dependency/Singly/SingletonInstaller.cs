using UnityEngine;

namespace ICouldGames.Dependency.Singly
{
    public abstract class SingletonInstaller : ScriptableObject
    {
        public abstract void Install(SingletonContainer container);
    }
}
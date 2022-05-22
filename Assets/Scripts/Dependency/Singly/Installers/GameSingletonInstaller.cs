using ICouldGames.SlotMachine.Controller;
using ICouldGames.SlotMachine.Settings;
using ICouldGames.SlotMachine.Spin.Item;
using UnityEngine;

namespace ICouldGames.Dependency.Singly.Installers
{
    [CreateAssetMenu(fileName = "GameSingletonInstaller", menuName = "Singleton/Installers", order = 1)]
    public class GameSingletonInstaller : SingletonInstaller
    {
        public override void Install(SingletonContainer container)
        {
            container.Add<SlotMachineController>(new GameSlotMachineController(Resources.Load<SlotMachineSettings>("Settings/SlotMachineSettings")));
            container.Add(new SpinItemImageProvider());
        }
    }
}
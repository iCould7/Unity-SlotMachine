using System;
using ICouldGames.SlotMachine.Controller;
using ICouldGames.SlotMachine.Settings;

namespace ICouldGames.SlotMachine.Editor.Testing
{
    public class EditorTestSlotMachineController : SlotMachineController
    {
        public EditorTestSlotMachineController(SlotMachineSettings slotMachineSettings) : base(slotMachineSettings)
        {
        }

        public override void Init()
        {
            base.Init();
            SpinPicker = new Random(Environment.TickCount);
        }

        protected override void Save()
        {
        }
    }
}
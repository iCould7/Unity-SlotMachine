#if UNITY_EDITOR

using System;
using ICouldGames.SlotMachine.Settings;

namespace ICouldGames.SlotMachine.Controller
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

#endif
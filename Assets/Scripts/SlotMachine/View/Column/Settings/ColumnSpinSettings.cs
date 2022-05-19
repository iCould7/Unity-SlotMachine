using ICouldGames.SlotMachine.Spin.Item;

namespace ICouldGames.SlotMachine.View.Column.Settings
{
    public struct ColumnSpinSettings
    {
        public float StartingSpinSpeed;
        public float StartingSpinDuration;
        public float SpinStopDuration;
        public SpinItemType ResultItemType;
        public LeanTweenType SlowingTweenType;
    }
}
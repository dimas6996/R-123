﻿namespace R123.View
{
    public enum RangeSwitcherValues
    {
        FixFrequency1,
        FixFrequency2,
        FixFrequency3,
        FixFrequency4,
        SubFrequency2,
        SubFrequency1
    };
    public class RangeSwitcher : PositionSwitcher
    {
        public event DelegateChangeValue ValueChanged;
        public RangeSwitcher(System.Windows.Controls.Image image) :
            base(image, -3)
        {
            SetStartValue(/*Properties.Settings.Default.RangeSwitcher*/0, 6);
        }
        public new RangeSwitcherValues Value => (RangeSwitcherValues)base.Value;
        protected override void ValueIsUpdated()
        {
            ValueChanged?.Invoke();
            Properties.Settings.Default.RangeSwitcher = base.Value;
            Properties.Settings.Default.Save();

            System.Diagnostics.Trace.WriteLine($"Range = {Value}; ");
        }
    }
}

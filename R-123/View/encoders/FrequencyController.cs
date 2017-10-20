﻿using System.Windows.Input;

namespace R_123.View
{
    class FrequencyController : AnimationEncoder
    {
        public event DelegateChangeValue ValueChanged;
        public event DelegateChangeValue ValueChanged2;

        private const decimal minFirstSubFrequency = 20m;
        private const decimal minSecondSubFrequency = 35.75m;

        private System.Windows.Controls.Canvas canvas;
        public FrequencyController(System.Windows.Controls.Image image) : 
            base(image, Properties.Settings.Default.FrequencyController, 15.75m)
        {
            cursorImages = CursorImages.mouseIconLeftCenter;

            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            canvas = Options.canvas;

            Options.PositionSwitchers.Range.ValueChanged += UpdateValue;
            Options.Switchers.Power.ValueChanged += UpdateValue;
            for (int i = 0; i < Options.Switchers.SubFixFrequency.Length; i++)
                Options.Switchers.SubFixFrequency[i].ValueChanged += UpdateValue;

            image.MouseLeave += Image_MouseLeave;
        }
        protected override void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Options.Switchers.Power.Value == State.off ||
                Options.PositionSwitchers.Range.Value >= RangeSwitcherValues.SubFrequency2)
            {
                Options.CursorDisplay.SetCursor(cursorImages);
                Options.CursorDisplay.SetCtrlTextBlock(true);
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e) => 
            Options.CursorDisplay.SetCtrlTextBlock(false);


        protected override void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Options.Switchers.Power.Value == State.on &&
                Options.PositionSwitchers.Range.Value <= RangeSwitcherValues.FixFrequency4)
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Clamp[number].Value != 1)
                    return;
            }
            
            decimal delta = 0.025m;
            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0)
                delta *= 10;
            
            if (e.Delta > 0)
                CurrentValue += delta;
            else
                CurrentValue -= delta;
        }
        public new decimal Value => ValueToFrequency(base.Value);
        double cursorX;
        decimal startValue;
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Options.PositionSwitchers.Range.Value >= RangeSwitcherValues.SubFrequency2 ||
                Options.Switchers.Power.Value == State.off)
            {
                Options.Window.MouseMove += Canvas_MouseMove;
                Options.Window.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
                Options.Window.Cursor = Cursors.ScrollWE;

                cursorX = Mouse.GetPosition(canvas).X;
                startValue = (base.Value);
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            double coefficient = 0.003;
            double deltaX = cursorX - Mouse.GetPosition(canvas).X;

            base.CurrentValue = (System.Convert.ToDecimal(deltaX * coefficient) + startValue);
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Options.Window.MouseMove -= Canvas_MouseMove;
            Options.Window.MouseLeftButtonUp -= Canvas_MouseLeftButtonUp;
            Options.Window.Cursor = Cursors.AppStarting;
        }
        private void UpdateValue()
        {
            if (Options.Switchers.Power.Value == State.on)
            {
                if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency1)
                {
                    if (Options.Switchers.SubFixFrequency[0].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency1_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency1_2);
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency2)
                {
                    if (Options.Switchers.SubFixFrequency[1].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency2_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency2_2);
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency3)
                {
                    if (Options.Switchers.SubFixFrequency[2].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency3_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency3_2);
                }
                else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.FixFrequency4)
                {
                    if (Options.Switchers.SubFixFrequency[3].Value == SubFrequency.One)
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency4_1);
                    else
                        Animation = FrequencyToValue(Properties.Settings.Default.FixedFrequency4_2);
                }
                else
                    Animation = FrequencyToValue(Value);
            }
        }
        private decimal ValueToFrequency(decimal value)
        {
            if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1)
                return value + minFirstSubFrequency;
            else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2)
                return value + minSecondSubFrequency;
            else
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    return value + minFirstSubFrequency;
                else
                    return value + minSecondSubFrequency;
            }
        }

        private decimal FrequencyToValue(decimal frequency)
        {
            if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency1)
                return frequency - minFirstSubFrequency;
            else if (Options.PositionSwitchers.Range.Value == RangeSwitcherValues.SubFrequency2)
                return frequency - minSecondSubFrequency;
            else
            {
                int number = (int)Options.PositionSwitchers.Range.Value;
                if (Options.Switchers.SubFixFrequency[number].Value == SubFrequency.One)
                    return frequency - minFirstSubFrequency;
                else
                    return frequency - minSecondSubFrequency;
            }
        }
        protected override void ValueIsUpdated()
        {
            ValueChanged2?.Invoke();
            if (TimerIsEnabled == false)
            {
                ValueChanged?.Invoke();
                Properties.Settings.Default.FrequencyController = base.Value;
                Properties.Settings.Default.Save();

                System.Diagnostics.Trace.WriteLine($"Частота = {Value}; ");
            }
        }
    }
}

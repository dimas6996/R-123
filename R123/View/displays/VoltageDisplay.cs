﻿using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace R123.View
{
    public class VoltageDisplay
    {
        private Ellipse ellipse;
        private Line line;
        private double centerX, centerY;
        System.Windows.Threading.DispatcherTimer dispatcherTimer, dispatcherToNull;
        public VoltageDisplay(Ellipse ellipse, Line line)
        {
            this.line = line;
            this.ellipse = ellipse;
            centerX = line.Width;
            centerY = line.Height;
            Options.PressSpaceControl.ValueChanged += Update;
            Options.Switchers.Power.ValueChanged += Update;
            Options.PositionSwitchers.WorkMode.ValueChanged += Update;

            Options.Encoders.AthenaDisplay.ValueChanged += UpdateMaxValue;
            Options.Encoders.Frequency.AngleChanged += UpdateMaxValue;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan((long)(10e6 / 40));

            dispatcherToNull = new System.Windows.Threading.DispatcherTimer();
            dispatcherToNull.Tick += new EventHandler(DispatcherToNull_Tick);
            dispatcherToNull.Interval = new TimeSpan((long)(10e6 / 40));
        }
        private void Update()
        {
            if (Options.PressSpaceControl.Value && 
                Options.Switchers.Power.Value == State.on && 
                Options.PositionSwitchers.WorkMode.Value == WorkModeValue.Simplex)
            {
                dispatcherToNull.Stop();
                dispatcherTimer.Start();
            }
            else
            {
                dispatcherTimer.Stop();
                dispatcherToNull.Start();
            }
        }
        double maxAngle = 0, maxOpacity = 0;
        private void UpdateMaxValue()
        {
            maxOpacity = Options.Encoders.AthenaDisplay.Value;
            maxAngle = 75 * maxOpacity;
        }
        double angle = 0;
        double opacity = 0;
        const double addValueInAnimationLine = 6;
        const double addValueInAnimationOpacity = 0.05;
        double stepAngle, stepOpacity;

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (opacity > maxOpacity - addValueInAnimationOpacity && opacity >= addValueInAnimationOpacity)
                stepOpacity = -addValueInAnimationOpacity;
            else if (opacity < maxOpacity - 0.2 || opacity <= 0)
                stepOpacity = addValueInAnimationOpacity;
            ellipse.Opacity = opacity += stepOpacity;

            if (angle > maxAngle - addValueInAnimationLine && angle >= addValueInAnimationLine)
                stepAngle = -addValueInAnimationLine;
            else if (angle <= maxAngle - 15 || angle <= 0)
                stepAngle = addValueInAnimationLine;
            line.RenderTransform = new RotateTransform(angle += stepAngle, centerX, centerY);
        }
        private void DispatcherToNull_Tick(object sender, EventArgs e)
        {
            if (opacity >= addValueInAnimationOpacity)
                opacity -= addValueInAnimationOpacity;
            else if (opacity > 0)
                opacity = 0;
            ellipse.Opacity = opacity;

            if (angle > addValueInAnimationLine)
                angle -= addValueInAnimationLine;
            else if (angle > 0)
                angle = 0;
            line.RenderTransform = new RotateTransform(angle, centerX, centerY);

            if (opacity == 0 && angle == 0)
                dispatcherToNull.Stop();
        }
    }
}

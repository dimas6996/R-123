﻿using System;
using R123.NewRadio.Model;

namespace R123.Learning
{
    public class WorkingTest
    {
        public Func<bool>[] Conditions { get; private set; }
        private MainModel radio;

        public WorkingTest(MainModel radio)
        {
            this.radio = radio;

            Conditions = new Func<bool>[24];

            Conditions[0] = () => true;
            Conditions[1] = () => radio.WorkMode.Value == WorkModeState.Simplex;
            Conditions[2] = () => radio.Noise.Value == 1.0;
            Conditions[3] = () => radio.Scale.Value == Turned.On && radio.Power.Value == Turned.On;
            Conditions[4] = () => radio.Tangent.Value == Turned.On;
            Conditions[5] = () => radio.Volume.Value == 1.0;
            Conditions[6] = () => radio.Range.Value == RangeState.SmoothRange1;
            Conditions[7] = () => true;
            Conditions[8] = () => radio.Noise.Value < 0.5;
            Conditions[9] = () => radio.Range.Value == RangeState.SmoothRange2;
            Conditions[10] = () => radio.WorkMode.Value == WorkModeState.StandbyReception;
            Conditions[11] = () => true;
            Conditions[12] = () => true;
            Conditions[13] = () => radio.WorkMode.Value == WorkModeState.Simplex;
            Conditions[14] = () => radio.Tangent.Value == Turned.On;
            Conditions[15] = () => radio.Antenna.Value > 0.8;
            Conditions[16] = () => true;
            Conditions[17] = () => true;
            Conditions[18] = () => radio.Range.Value == RangeState.SmoothRange1;
            Conditions[19] = () => true;
            Conditions[20] = () => radio.Clamps[0] == ClampState.Fixed &&
                                   radio.Clamps[1] == ClampState.Fixed && 
                                   radio.Clamps[2] == ClampState.Fixed && 
                                   radio.Clamps[3] == ClampState.Fixed;
            Conditions[21] = () => radio.Antenna.Value > 0.8;
            Conditions[22] = () => radio.Range.Value == RangeState.FixedFrequency4;
            Conditions[23] = () => radio.Power.Value == Turned.Off;
        }
    }
}

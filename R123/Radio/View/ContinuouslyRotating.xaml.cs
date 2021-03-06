﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R123.Radio.View
{
    /// <summary>
    /// Логика взаимодействия для RotateImage.xaml
    /// </summary>
    public partial class ContinuouslyRotating : UserControl
    {
        protected Point centerImage;
        protected double mouseMoveFactor = 1;
        protected double changeAngleMouseWheel = 1;
        private double maxAngle;

        public ContinuouslyRotating(double maxAngle = 360)
        {
            InitializeComponent();
            this.maxAngle = maxAngle;
            TheImage.Cursor = Cursors.Hand;

            Loaded += (object sender, RoutedEventArgs e) => {
                centerImage.X = TheImage.ActualWidth / 2;
                centerImage.Y = TheImage.ActualHeight / 2;
            };

            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            LostMouseCapture += OnLostCapture;
        }

        protected virtual void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double newAngle = CheckingAngle(RotateControl.Angle - changeAngleMouseWheel * Math.Sign(e.Delta));

            if (RotateControl.Angle != newAngle && canRotate)
            {
                RequestRotateCommand?.Execute(newAngle);
                RequestEndRotateCommand?.Execute(newAngle);
            }
        }

        #region User image rotation
        protected virtual double CheckingAngle(double angle)
        {
            if (angle > maxAngle)
                return maxAngle;
            else if (angle < 0)
                return 0;
            else
                return angle;
        }

        private Vector previousMousePosition;
        protected bool canRotate = true;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            previousMousePosition = e.MouseDevice.GetPosition(this) - centerImage;
            MouseMove += OnMouseMove;
            Mouse.Capture(this);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FinishRotate();
            Mouse.Capture(null);
            if (canRotate)
                RequestEndRotateCommand?.Execute(newAngle);
        }
        double newAngle;
        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            Vector currentMousePosition = e.MouseDevice.GetPosition(this) - centerImage;
            double changeAngle = Vector.AngleBetween(previousMousePosition, currentMousePosition) * mouseMoveFactor;
            newAngle = CheckingAngle(RotateControl.Angle + changeAngle);

            if (RotateControl.Angle != newAngle && canRotate)
                RequestRotateCommand?.Execute(newAngle);

            previousMousePosition = currentMousePosition;
        }

        void OnLostCapture(object sender, MouseEventArgs e)
        {
            FinishRotate();
        }

        void FinishRotate()
        {
            MouseMove -= OnMouseMove;
        }
        #endregion

        #region dp ICommand RequestRotateCommand
        public ICommand RequestRotateCommand
        {
            get { return (ICommand)GetValue(RequestRotateCommandProperty); }
            set { SetValue(RequestRotateCommandProperty, value); }
        }

        public static readonly DependencyProperty RequestRotateCommandProperty =
            DependencyProperty.Register("RequestRotateCommand", typeof(ICommand),
                                        typeof(ContinuouslyRotating), new FrameworkPropertyMetadata(null, OnRequestRotateCommandChanged));

        private static void OnRequestRotateCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContinuouslyRotating o = d as ContinuouslyRotating;
            o.TestRequestRotateCommand(e.NewValue as ICommand);
        }

        public virtual void TestRequestRotateCommand(ICommand newValue)
        {
        }
        #endregion

        #region dp ICommand RequestEndRotateCommand
        public ICommand RequestEndRotateCommand
        {
            get { return (ICommand)GetValue(RequestEndRotateCommandProperty); }
            set { SetValue(RequestEndRotateCommandProperty, value); }
        }

        public static readonly DependencyProperty RequestEndRotateCommandProperty =
            DependencyProperty.Register("RequestEndRotateCommand", typeof(ICommand), typeof(ContinuouslyRotating));
        #endregion


        #region dp double Angle
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle",
            typeof(double),
            typeof(ContinuouslyRotating),
            new UIPropertyMetadata(0.0,
                new PropertyChangedCallback(AngleChanged)));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        private static void AngleChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            ContinuouslyRotating image = (ContinuouslyRotating)depObj;
            image.RotateControl.Angle = Convert.ToDouble(args.NewValue);
        }
        #endregion
    }
}
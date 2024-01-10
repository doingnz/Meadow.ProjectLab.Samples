﻿using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors.Buttons;

namespace GalleryViewer.Hardware
{
    internal class GalleryViewerHardware : IGalleryViewerHardware
    {
        protected IProjectLabHardware ProjLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public IButton RightButton { get; set; }

        public IButton LeftButton { get; set; }

        public IRgbPwmLed RgbPwmLed { get; set; }

        public void Initialize()
        {
            ProjLab = ProjectLab.Create();

            Display = ProjLab.Display;

            RightButton = ProjLab.RightButton;

            LeftButton = ProjLab.LeftButton;

            RgbPwmLed = ProjLab.RgbLed;
        }
    }
}

﻿using System;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Light;
using Meadow.Gateways.Bluetooth;
using Meadow.Hardware;
using Meadow.Units;

namespace ProjLab_Demo
{
    public class ProjectLabV1_Hardware : IHardwareConfig
    {
        public IGraphicsDisplay Display { get; protected set; }

        public ISpiBus SpiBus { get; protected set; }

        public II2cBus I2cBus { get; protected set; }

        public RgbPwmLed OnboardLed { get; protected set; }

        public PiezoSpeaker Speaker { get; protected set; }

        public Bh1750? Bh1750 { get; protected set; }

        public Bme680? Bme688 { get; protected set; }

        public PushButton UpButton { get; protected set; }

        public PushButton DownButton { get; protected set; }

        public PushButton LeftButton { get; protected set; }

        public PushButton RightButton { get; protected set; }


        public void Initialize(IF7FeatherMeadowDevice device)
        {
            //==== I2C Bus
            try
            {
                I2cBus = device.CreateI2cBus();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR creating I2C: {e.Message}");
            }

            //==== SPI BUS
            var spiConfig = new SpiClockConfiguration(
                new Frequency(48000, Frequency.UnitType.Kilohertz),
                SpiClockConfiguration.Mode.Mode3);

            SpiBus = device.CreateSpiBus(
                device.Pins.SCK,
                device.Pins.MOSI,
                device.Pins.MISO,
                spiConfig);

            //==== Display
            Display = new St7789(
            device: device,
            spiBus: SpiBus,
                chipSelectPin: device.Pins.A03,
                dcPin: device.Pins.A04,
                resetPin: device.Pins.A05,
                width: 240, height: 240,
                displayColorMode: ColorType.Format16bppRgb565);

            //==== Onboard LED
            OnboardLed = new RgbPwmLed(device: device,
                redPwmPin: device.Pins.OnboardLedRed,
                greenPwmPin: device.Pins.OnboardLedGreen,
                bluePwmPin: device.Pins.OnboardLedBlue);
            OnboardLed.StartPulse(WildernessLabsColors.ChileanFire);

            //==== Speaker
            Speaker = new PiezoSpeaker(device, device.Pins.D11);

            //==== BH1750
            try
            {
                Bh1750 = new Bh1750(
                    i2cBus: I2cBus,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, // the various modes take differing amounts of time.
                    lightTransmittance: 0.5, // lower this to increase sensitivity, for instance, if it's behind a semi opaque window
                    address: (byte)Bh1750.Addresses.Address_0x23
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bh1750: {e.Message}");
            }

            //==== BME688
            try
            {
                Bme688 = new Bme680(I2cBus, (byte)Bme680.Addresses.Address_0x76);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bme688: {e.Message}");
            }

            //==== Buttons
            UpButton = new PushButton(device, device.Pins.D15, ResistorMode.InternalPullDown);
            DownButton = new PushButton(device, device.Pins.D05, ResistorMode.InternalPullDown);
            LeftButton = new PushButton(device, device.Pins.D10, ResistorMode.InternalPullDown);

        }
    }
}
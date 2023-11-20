﻿using Meadow.Cloud_Client.Hardware;
using Meadow.Cloud_Client.Services;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Cloud_Client
{
    internal class MainCoordinator
    {
        private IMeadowCloudClientHardware hardware;
        private IWiFiNetworkAdapter network;
        private CloudService cloudService;
        private DisplayService displayService;

        private int currentGraphType = 0;

        private List<double> temperatureReadings = new List<double>();
        private List<double> pressureReadings = new List<double>();
        private List<double> humidityReadings = new List<double>();

        public MainCoordinator(IMeadowCloudClientHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

            hardware.RightButton.Clicked += RightButtonClicked;

            hardware.LeftButton.Clicked += LeftButtonClicked;

            cloudService = new CloudService();
            displayService = new DisplayService(hardware.Display);

            displayService.ShowSplashScreen();
            Thread.Sleep(3000);
            displayService.ShowDataScreen();
        }

        private void RightButtonClicked(object sender, EventArgs e)
        {
            currentGraphType = currentGraphType > 2 ? 0 : currentGraphType + 1;

            UpdateGraph();
        }

        private void LeftButtonClicked(object sender, EventArgs e)
        {
            currentGraphType = currentGraphType < 0 ? 2 : currentGraphType - 1;

            UpdateGraph();
        }

        private void UpdateGraph()
        {
            Resolver.Log.Info($"UpdateGraph() - {currentGraphType}");

            switch (currentGraphType)
            {
                case 0:
                    displayService.UpdateGraph(currentGraphType, temperatureReadings);
                    break;
                case 1:
                    displayService.UpdateGraph(currentGraphType, pressureReadings);
                    break;
                case 2:
                    displayService.UpdateGraph(currentGraphType, humidityReadings);
                    break;
            }
        }

        public async Task Run()
        {
            int TIMEZONE_OFFSET = -8; // UTC-8

            while (true)
            {
                displayService.UpdateWiFiStatus(network.IsConnected);
                displayService.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("dd/MM/yy hh:mm tt"));

                if (network.IsConnected)
                {
                    displayService.UpdateSyncStatus(true);

                    var readings = await cloudService.GetSensorReadings();

                    if (readings != null && readings.Count > 0)
                    {
                        temperatureReadings.Clear();
                        pressureReadings.Clear();
                        humidityReadings.Clear();

                        foreach (var reading in readings)
                        {
                            temperatureReadings.Add(reading.record.measurements.temperature);
                            pressureReadings.Add(reading.record.measurements.pressure);
                            humidityReadings.Add(reading.record.measurements.humidity);
                        }

                        UpdateGraph();
                    }
                }

                displayService.UpdateSyncStatus(false);

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
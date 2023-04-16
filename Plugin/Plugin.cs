﻿using FanControl.Plugins;

namespace FanControl.Acer_PO3630.Plugin
{
    public class Plugin : IPlugin2
    {
        private readonly IPluginLogger logger;
        private readonly IPluginDialog dialog;

        public static int iPluginCount = 7;
        public static int iPluginStagger = 5;
        public static int GetMax() => ((iPluginCount - 1) * iPluginStagger) + 1;
        public static int GetPos(int iIndex) => ((iIndex - 1) * iPluginStagger) + 1;

        public Plugin(IPluginLogger logger, IPluginDialog dialog)
        {
            this.logger = logger;
            this.dialog = dialog;
        }

        public string Name => "Acer PO3-630";


        public void Initialize()
        {
        }

        public void Load(IPluginSensorsContainer _container)
        {
            _container.ControlSensors.Add(new ControlSensor() { Id = "FrontFan", Name = "Front Fan", fan = Acer.Enums.Fan_Index.FrontFan, iIndex = 1 });
            _container.ControlSensors.Add(new ControlSensor() { Id = "RearFan", Name = "Rear Fan", fan = Acer.Enums.Fan_Index.RearFan, iIndex = 2 });
            _container.ControlSensors.Add(new ControlSensor() { Id = "CPUFan", Name = "CPU Fan", fan = Acer.Enums.Fan_Index.CPUFan, iIndex = 3 });

            _container.FanSensors.Add(new FanSensor() { Id = "FrontFan", Name = "Front Fan", fan = Acer.Enums.Fan_Index.FrontFan, iIndex = 4 });
            _container.FanSensors.Add(new FanSensor() { Id = "RearFan", Name = "Rear Fan", fan = Acer.Enums.Fan_Index.RearFan, iIndex = 5 });
            _container.FanSensors.Add(new FanSensor() { Id = "CPUFan", Name = "CPU Fan", fan = Acer.Enums.Fan_Index.CPUFan, iIndex = 6 });

            _container.TempSensors.Add(new TempSensor() { Id = "System", Name = "System", temp = Acer.Enums.Temp_Index.System, iIndex = 7 });

            Update();
        }

        public void Update()
        {
        }

        public void Close()
        {
        }
    }
}

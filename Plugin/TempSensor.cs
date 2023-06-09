﻿using FanControl.Acer_PO3630.Acer;
using FanControl.Plugins;

namespace FanControl.Acer_PO3630.Plugin
{
    internal class TempSensor : IPluginSensor
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float? Value { get; set; }

        public Acer.Enums.Temp_Index temp { get; set; }

        /// <summary>
        /// Update the "System" temperature.
        /// </summary>
        public async void Update()
        {
            Value = await temp.Get_SysTemp();
        }
    }
}

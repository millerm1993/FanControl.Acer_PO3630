using FanControl.Acer_PO3630.Acer;
using FanControl.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.Acer_PO3630.Plugin
{
    internal class TempSensor : IPluginSensor
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float? Value { get; set; }

        public Acer.Enums.Temp_Index temp { get; set; }

        public int iUpdateOn { get; set; }
        private int iUpdateCount { get; set; }

        /// <summary>
        /// Update the "System" temperature.
        /// </summary>
        public void Update()
        {
            iUpdateCount++;

            if (iUpdateCount == iUpdateOn)
            {
                Value = temp.Get_SysTemp();
            }

            if (iUpdateCount == 10)
            {
                iUpdateCount = 0;
            }
        }
    }
}

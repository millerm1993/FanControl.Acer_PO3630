using FanControl.Acer_PO3630.Acer;
using FanControl.Plugins;

namespace FanControl.Acer_PO3630.Plugin
{
    internal class TempSensor : IPluginSensor
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float? Value { get; set; }

        public Acer.Enums.Temp_Index temp { get; set; }

        public int iIndex { get; set; }
        private int iUpdateCount { get; set; }

        /// <summary>
        /// Update the "System" temperature.
        /// </summary>
        public void Update()
        {
            iUpdateCount++;

            if (iUpdateCount == Plugin.GetPos(iIndex))
            {
                Value = temp.Get_SysTemp().Result;
            }

            if (iUpdateCount == Plugin.GetMax())
            {
                iUpdateCount = 0;
            }
        }
    }
}

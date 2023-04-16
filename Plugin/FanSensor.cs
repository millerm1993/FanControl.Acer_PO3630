using FanControl.Acer_PO3630.Acer;
using FanControl.Plugins;

namespace FanControl.Acer_PO3630.Plugin
{
    internal class FanSensor : IPluginSensor
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float? Value { get; set; }

        public Acer.Enums.Fan_Index fan { get; set; }

        public int iIndex { get; set; }
        private int iUpdateCount { get; set; }

        /// <summary>
        /// Update the fan RPM the system thinks the fan is doing.
        /// </summary>
        public void Update()
        {
            iUpdateCount++;

            if (iUpdateCount == Plugin.GetPos(iIndex))
            {
                Value = fan.Get_FanRpm().Result;
            }

            if (iUpdateCount == Plugin.GetMax())
            {
                iUpdateCount = 0;
            }
        }
    }
}

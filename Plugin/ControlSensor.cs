using FanControl.Acer_PO3630.Acer;
using FanControl.Plugins;

namespace FanControl.Acer_PO3630.Plugin
{
    internal class ControlSensor : IPluginControlSensor
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float? Value { get; set; }

        public Acer.Enums.Fan_Index fan { get; set; }

        public int iUpdateOn { get; set; }
        private int iUpdateCount { get; set; }

        /// <summary>
        /// Update the fan speed percentage the system thinks the fan is doing.
        /// </summary>
        public void Update()
        {
            iUpdateCount++;

            if (iUpdateCount == iUpdateOn)
            {
                fan.Get_FanPercentage();
            }

            if (iUpdateCount == 10)
            {
                iUpdateCount = 0;
            }
        }

        /// <summary>
        /// Set a manual speed value.
        /// </summary>
        /// <param name="value">The percentage speed to run the fan.</param>
        public void Set(float value)
        {
            if (iUpdateCount == iUpdateOn)
            {
                //Value = fan.Set_FanPercentage(value);
                fan.Set_FanPercentage(value);
            }
        }

        /// <summary>
        /// Set the fan back to automatic control.
        /// </summary>
        public void Reset()
        {
            fan.Set_FanAuto();
        }
    }
}

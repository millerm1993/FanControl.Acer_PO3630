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

        /// <summary>
        /// Update the fan speed percentage the system thinks the fan is doing.
        /// </summary>
        public async void Update()
        {
            Value = await fan.Get_FanPercentage();
        }

        /// <summary>
        /// Set a manual speed value.
        /// </summary>
        /// <param name="value">The percentage speed to run the fan.</param>
        public async void Set(float value)
        {
            //Value = fan.Set_FanPercentage(value);
            await fan.Set_FanPercentage(value);
        }

        /// <summary>
        /// Set the fan back to automatic control.
        /// </summary>
        public async void Reset()
        {
            await fan.Set_FanAuto();
        }
    }
}

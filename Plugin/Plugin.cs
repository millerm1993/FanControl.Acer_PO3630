using FanControl.Plugins;

namespace FanControl.Acer_PO3630.Plugin
{
    public class Plugin : IPlugin2
    {
        private readonly IPluginLogger logger;
        private readonly IPluginDialog dialog;

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
            _container.ControlSensors.Add(new ControlSensor() { Id = "FrontFan", Name = "Front Fan", fan = Acer.Enums.Fan_Index.FrontFan, iUpdateOn = 1 });
            _container.ControlSensors.Add(new ControlSensor() { Id = "RearFan", Name = "Rear Fan", fan = Acer.Enums.Fan_Index.RearFan, iUpdateOn = 2 });
            _container.ControlSensors.Add(new ControlSensor() { Id = "CPUFan", Name = "CPU Fan", fan = Acer.Enums.Fan_Index.CPUFan, iUpdateOn = 3 });

            _container.FanSensors.Add(new FanSensor() { Id = "FrontFan", Name = "Front Fan", fan = Acer.Enums.Fan_Index.FrontFan, iUpdateOn = 4 });
            _container.FanSensors.Add(new FanSensor() { Id = "RearFan", Name = "Rear Fan", fan = Acer.Enums.Fan_Index.RearFan, iUpdateOn = 5 });
            _container.FanSensors.Add(new FanSensor() { Id = "CPUFan", Name = "CPU Fan", fan = Acer.Enums.Fan_Index.CPUFan, iUpdateOn = 6 });

            _container.TempSensors.Add(new TempSensor() { Id = "System", Name = "System", temp = Acer.Enums.Temp_Index.System, iUpdateOn = 7 });

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

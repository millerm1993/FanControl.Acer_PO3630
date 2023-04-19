using FanControl.Plugins;
using System.Threading.Tasks;

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
            _container.ControlSensors.Add(new ControlSensor() { Id = "FrontFan", Name = "Front Fan", fan = Acer.Enums.Fan_Index.FrontFan});
            _container.ControlSensors.Add(new ControlSensor() { Id = "RearFan", Name = "Rear Fan", fan = Acer.Enums.Fan_Index.RearFan});
            _container.ControlSensors.Add(new ControlSensor() { Id = "CPUFan", Name = "CPU Fan", fan = Acer.Enums.Fan_Index.CPUFan});

            _container.FanSensors.Add(new FanSensor() { Id = "FrontFan", Name = "Front Fan", fan = Acer.Enums.Fan_Index.FrontFan});
            _container.FanSensors.Add(new FanSensor() { Id = "RearFan", Name = "Rear Fan", fan = Acer.Enums.Fan_Index.RearFan});
            _container.FanSensors.Add(new FanSensor() { Id = "CPUFan", Name = "CPU Fan", fan = Acer.Enums.Fan_Index.CPUFan});

            _container.TempSensors.Add(new TempSensor() { Id = "System", Name = "System", temp = Acer.Enums.Temp_Index.System});

            Update();
        }

        public void Update()
        {
        }

        public async void Close()
        {
            Acer.PipeQueue.Stop();
        }
    }
}

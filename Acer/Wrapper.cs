using System;
using System.Threading.Tasks;
using static FanControl.Acer_PO3630.Acer.Enums;

namespace FanControl.Acer_PO3630.Acer
{
    public static class Wrapper
    {
        /// <summary>
        /// Returns the "System" temperature.
        /// </summary>
        public static async Task<float> Get_SysTemp(this Temp_Index tempIndex)
        {
            int iSysTemp = 0;
            AcerSysInfo_Index info_Index;


            switch (tempIndex)
            {
                case Temp_Index.System:
                    info_Index = AcerSysInfo_Index.System_Temp;
                    break;
                default:
                    throw new Exception("Invalid Temp Index");
            }

            iSysTemp = await Commands.Get_AcerSysInfo(info_Index);
            return iSysTemp;
        }

        /// <summary>
        /// Update the fan RPM the system thinks the fan is doing.
        /// </summary>
        public static async Task<float> Get_FanRpm(this Fan_Index fanIndex)
        {
            int iRpm = 0;
            AcerSysInfo_Index info_Index;

            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                    info_Index = AcerSysInfo_Index.System_FanRpm_1;
                    break;
                case Fan_Index.RearFan:
                    info_Index = AcerSysInfo_Index.System_FanRpm_2;
                    break;
                case Fan_Index.CPUFan:
                    info_Index = AcerSysInfo_Index.CPU_FanRpm;
                    break;
                default:
                    throw new Exception("Invalid System Info Index");
            }

            iRpm = await Commands.Get_AcerSysInfo(info_Index);

            return Convert.ToSingle((double)iRpm);
        }

        /// <summary>
        /// Update the fan speed percentage the system thinks the fan is doing.
        /// </summary>
        public static async Task<int> Get_FanPercentage(this Fan_Index fanIndex)
        {
            int iPercent = 0;
            AcerSysInfo_Index info_Index;

            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                    info_Index = AcerSysInfo_Index.System_FanPercent_1;
                    break;
                case Fan_Index.RearFan:
                    info_Index = AcerSysInfo_Index.System_FanPercent_2;
                    break;
                case Fan_Index.CPUFan:
                    info_Index = AcerSysInfo_Index.CPU_FanPercent;
                    break;
                default:
                    throw new Exception("Invalid System Info Index");
            }

            iPercent = await Commands.Get_AcerSysInfo(info_Index);
            return iPercent;
        }

        /// <summary>
        /// Set a manual speed value.
        /// </summary>
        /// <param name="value">The percentage speed to run the fan.</param>
        public static async Task<float> Set_FanPercentage(this Fan_Index fanIndex, float value)
        {
            ulong myValue;

            try
            {
                myValue = Convert.ToByte(value);
            }
            catch (Exception ex)
            {
                throw;
            }

            //Build the Command Array
            byte[] commandBytes = new byte[8];
            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                case Fan_Index.RearFan:
                    commandBytes[0] = 4;
                    commandBytes[3] = (byte)myValue;
                    commandBytes[5] = (byte)fanIndex;
                    break;
                case Fan_Index.CPUFan:
                    commandBytes[0] = 2;
                    commandBytes[2] = (byte)myValue;
                    break;
                default:
                    return 0;
            }

            await Commands.Set_AcerSysConfig(commandBytes);
            return value;
        }

        /// <summary>
        /// Set the fan back to automatic control.
        /// </summary>
        public static async Task Set_FanAuto(this Fan_Index fanIndex)
        {
            byte[] CommandBytes = new byte[8];
            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                case Fan_Index.RearFan:
                    CommandBytes[0] = 4;
                    CommandBytes[3] = 255;
                    CommandBytes[5] = (byte)fanIndex;
                    break;
                case Fan_Index.CPUFan:
                    CommandBytes[0] = 2;
                    CommandBytes[2] = 255;
                    break;
            }

            await Commands.Set_AcerSysConfig(CommandBytes);
        }

        /// <summary>
        /// Set the fan back to maximum speed.
        /// </summary>
        public static async Task Set_FanMax(this Fan_Index fanIndex)
        {
            ulong input = 0;

            byte[] commandBytes = new byte[8];
            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                case Fan_Index.RearFan:
                    commandBytes[0] = 4;
                    commandBytes[3] = 254;
                    commandBytes[5] = (byte)fanIndex;
                    break;
                case Fan_Index.CPUFan:
                    commandBytes[0] = 2;
                    commandBytes[2] = 254;
                    break;
            }

            if (input == 0)
            {
                return;
            }

            await Commands.Set_AcerSysConfig(commandBytes);
        }
    }
}

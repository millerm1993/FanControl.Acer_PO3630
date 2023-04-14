using System;
using static FanControl.Acer_PO3630.Acer.Enums;

namespace FanControl.Acer_PO3630.Acer
{
    public static class Wrapper
    {
        /// <summary>
        /// Update the fan RPM the system thinks the fan is doing.
        /// </summary>
        public static float Get_FanRpm(this Fan_Index fanIndex)
        {
            int iRpm = 0;
            SystemInfoData_Index info_Index = SystemInfoData_Index.None;

            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                    info_Index = SystemInfoData_Index.System_FanRpm_1;
                    break;
                case Fan_Index.RearFan:
                    info_Index = SystemInfoData_Index.System_FanRpm_2;
                    break;
                case Fan_Index.CPUFan:
                    info_Index = SystemInfoData_Index.CPU_FanRpm;
                    break;
            }

            if (info_Index == SystemInfoData_Index.None)
            {
                return 0;
            }

            Commands.Get_SystemInfo(ref iRpm, info_Index);
            return Convert.ToSingle((double)iRpm);
        }

        /// <summary>
        /// Update the fan speed percentage the system thinks the fan is doing.
        /// </summary>
        public static int Get_FanPercentage(this Fan_Index fanIndex)
        {
            int iPercent = 0;
            SystemInfoData_Index info_Index = SystemInfoData_Index.None;

            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                    info_Index = SystemInfoData_Index.System_FanPercent_1;
                    break;
                case Fan_Index.RearFan:
                    info_Index = SystemInfoData_Index.System_FanPercent_2;
                    break;
                case Fan_Index.CPUFan:
                    info_Index = SystemInfoData_Index.CPU_FanPercent;
                    break;
            }

            if (info_Index == SystemInfoData_Index.None)
            {
                return 0;
            }

            Commands.Get_SystemInfo(ref iPercent, info_Index);
            return iPercent;
        }

        /// <summary>
        /// Set a manual speed value.
        /// </summary>
        /// <param name="value">The percentage speed to run the fan.</param>
        public static float Set_FanPercentage(this Fan_Index fanIndex, float value)
        {
            ulong input = 0;
            ulong myValue;

            try
            {
                myValue = Convert.ToUInt64(value);
            }
            catch
            {
                return 0;
            }

            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                    input = 4UL | (myValue << 24) | 1099511627776UL;
                    break;
                case Fan_Index.RearFan:
                    input = 4UL | (myValue << 24) | 2199023255552UL;
                    break;
                case Fan_Index.CPUFan:
                    input = 2UL | myValue << 16;
                    break;
            }

            if (input == 0)
            {
                return 0;
            }

            Commands.Set_AcerSysConfig(input).GetAwaiter();
            return value;
        }

        /// <summary>
        /// Set the fan back to automatic control.
        /// </summary>
        public static void Set_FanAuto(this Fan_Index fanIndex)
        {
            ulong input = 0;

            switch (fanIndex)
            {
                case Fan_Index.FrontFan:
                    input = 1103789817860UL;
                    break;
                case Fan_Index.RearFan:
                    input = 2203301445636UL;
                    break;
                case Fan_Index.CPUFan:
                    input = 2UL | (ulong)byte.MaxValue << 16;
                    break;
            }

            if (input == 0)
            {
                return;
            }

            Commands.Set_AcerSysConfig(input).GetAwaiter();
        }
    }
}

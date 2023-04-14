namespace FanControl.Acer_PO3630.Acer
{
    public static class Enums
    {
        /// <summary>
        /// Which fan are we controlling.
        /// </summary>
        public enum Fan_Index
        {
            FrontFan = 1,
            RearFan = 2,
            CPUFan = 3,
        }

        /// <summary>
        /// Datapoints from PredatorSense.exe that are to be used in the named pipe.
        /// </summary>
        public enum SystemInfoData_Index
        {
            None = 0,
            CPU_Temp = 1,
            CPU_FanRpm = 2,
            System_Temp = 3,
            System_FanRpm_1 = 4,
            System_FanRpm_2 = 5,
            System_FanRpm_3 = 6,
            CPU_Voltage = 7,
            GPU1_Temp = 8,
            GPU1_Voltage = 9,
            GPU1_FanRpm = 10, // 0x0000000A
            GPU2_Temp = 11, // 0x0000000B
            GPU2_Voltage = 12, // 0x0000000C
            GPU2_FanRpm = 13, // 0x0000000D
            GPU3_Temp = 14, // 0x0000000E
            GPU3_Voltage = 15, // 0x0000000F
            GPU3_FanRpm = 16, // 0x00000010
            GPU4_Temp = 17, // 0x00000011
            GPU4_Voltage = 18, // 0x00000012
            GPU4_FanRpm = 19, // 0x00000013
            CPU_FanPercent = 20, // 0x00000014
            System_FanPercent_1 = 21, // 0x00000015
            System_FanPercent_2 = 22, // 0x00000016
            System_FanPercent_3 = 23, // 0x00000017
        }

    }
}

using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using TsDotNetLib;

namespace FanControl.Acer_PO3630.Acer
{
    public class Commands
    {
        /// <summary>
        /// Remains from testing.
        /// PredatorSense.exe set these values when setting fans to "MAX" mode.
        /// Seems to only actually set fans to 90% 🤷.
        /// </summary>
        static void BothFansMax()
        {
            ulong input = 0;

            //Set Rear To Max
            input = 1103756263428UL;
            Set_AcerSysConfig(input).GetAwaiter();
            Thread.Sleep(30);

            //Set Rear To Max
            input = 2203267891204UL;
            Set_AcerSysConfig(input).GetAwaiter();
        }



        /// <summary>
        /// Reads and parses system info.
        /// Equates to get_wmi_system_health_info() in PredatorSense.exe.
        /// </summary>
        /// <param name="dataRefrence">This field will be updated with the result of the requested data.</param>
        /// <param name="dataRequested"></param>
        /// <returns>If the requested data was successfull</returns>
        public static bool Get_SystemInfo(ref int dataRefrence, Acer.Enums.SystemInfoData_Index dataRequested)
        {
            ulong result = Get_AcerSysInfo((uint)(1 | (int)dataRequested << 8)).Result;

            if (((long)result & (long)byte.MaxValue) != 0L)
            {
                return false;
            }

            dataRefrence = (int)((long)(result >> 8) & (long)ushort.MaxValue);

            return true;
        }

        /// <summary>
        /// Sets system information.
        /// Equates to SetAcerGamingSystemConfiguration() in PredatorSense.exe.
        /// </summary>
        /// <param name="data">The payload to inject into the named pipe.</param>
        /// <returns>Response to setting the data.</returns>
        public static async Task<uint> Set_AcerSysConfig(ulong data)
        {
            try
            {
                NamedPipeClientStream cline_stream = new NamedPipeClientStream(".", "predatorsense_service_namedpipe", PipeDirection.InOut);
                cline_stream.Connect();
                int num = (int)await Task.Run<uint>((Func<uint>)(() =>
                {
                    IPCMethods.SendCommandByNamedPipe(cline_stream, 10, (object)data);
                    cline_stream.WaitForPipeDrain();
                    byte[] buffer = new byte[9];
                    cline_stream.Read(buffer, 0, buffer.Length);
                    return BitConverter.ToUInt32(buffer, 5);
                })).ConfigureAwait(false);
                cline_stream.Close();
                return (uint)num;
            }
            catch (Exception ex)
            {
                return uint.MaxValue;
            }
        }

        /// <summary>
        /// Sets system information.
        /// Equates to GetAcerGamingSystemInformation() in PredatorSense.exe.
        /// </summary>
        /// <param name="data">The payload to read from the named pipe.</param>
        /// <returns>The value of the data that was requested.</returns>
        public static async Task<ulong> Get_AcerSysInfo(uint intput)
        {
            try
            {
                NamedPipeClientStream cline_stream = new NamedPipeClientStream(".", "predatorsense_service_namedpipe", PipeDirection.InOut);
                cline_stream.Connect();
                long systemInformation = (long)await Task.Run<ulong>((Func<ulong>)(() =>
                {
                    IPCMethods.SendCommandByNamedPipe(cline_stream, 9, (object)intput);
                    cline_stream.WaitForPipeDrain();
                    byte[] buffer = new byte[13];
                    cline_stream.Read(buffer, 0, buffer.Length);
                    return BitConverter.ToUInt64(buffer, 5);
                })).ConfigureAwait(false);
                cline_stream.Close();
                return (ulong)systemInformation;
            }
            catch (Exception ex)
            {
                return (ulong)uint.MaxValue;
            }
        }
    }
}

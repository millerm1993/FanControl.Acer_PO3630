using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using static FanControl.Acer_PO3630.Acer.Enums;

namespace FanControl.Acer_PO3630.Acer
{
    internal class Commands
    {
        /// <summary>
        /// Sets system information.
        /// Equates to SetAcerGamingSystemConfiguration() in PredatorSense.exe.
        /// </summary>
        /// <param name="data">The payload to inject into the named pipe.</param>
        /// <returns>Response to setting the data.</returns>
        public static async Task<uint> Set_AcerSysConfig(byte[] commandBytes)
        {
            //Build Data Packet
            byte[] packetBytes = new byte[15];
            packetBytes[0] = (byte)PredetorMessageType_Index.Command;
            packetBytes[2] = 1;   // Number of messages in packet
            packetBytes[3] = (byte)commandBytes.Length;
            Array.Copy(commandBytes, 0, packetBytes, 7, commandBytes.Length);

            try
            {
                int num = 0;
                using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "predatorsense_service_namedpipe", PipeDirection.InOut))
                {
                    pipeStream.Connect();

                    pipeStream.Write(packetBytes, 0, packetBytes.Length);
                    pipeStream.WaitForPipeDrain();

                    byte[] buffer = new byte[9];
                    pipeStream.Read(buffer, 0, buffer.Length);

                    num = (int)BitConverter.ToUInt32(buffer, 5);

                    pipeStream.Close();
                }

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
        public static async Task<int> Get_AcerSysInfo(Acer.Enums.SystemInfoData_Index dataRequested)
        {
            uint input = (uint)(1 | (int)dataRequested << 8);
            var inputBytes = BitConverter.GetBytes(input);

            //Build Data Packet
            byte[] packetBytes = new byte[11];
            packetBytes[0] = (byte)PredetorMessageType_Index.Read;
            packetBytes[2] = // Number of messages in packet;
            packetBytes[3] = (byte)inputBytes.Length;
            Array.Copy(inputBytes, 0, packetBytes, 7, inputBytes.Length);

            long systemInformation = 0;
            try
            {
                using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "predatorsense_service_namedpipe", PipeDirection.InOut))
                {
                    pipeStream.Connect();

                    pipeStream.Write(packetBytes, 0, packetBytes.Length);
                    pipeStream.WaitForPipeDrain();

                    byte[] buffer = new byte[13];
                    pipeStream.Read(buffer, 0, buffer.Length);

                    systemInformation = (long)BitConverter.ToUInt64(buffer, 5);

                    pipeStream.Close();
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            ulong result = (ulong)systemInformation;

            if (((long)result & (long)byte.MaxValue) != 0L)
            {
                return -1;
            }

            return (int)((long)(result >> 8) & (long)ushort.MaxValue);
        }
    }
}

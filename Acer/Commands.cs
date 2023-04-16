using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using static FanControl.Acer_PO3630.Acer.Enums;

namespace FanControl.Acer_PO3630.Acer
{
    internal class Commands
    {
        /// <summary>
        /// Builds the data packet to be send over the named pipe.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="messageBytes"></param>
        /// <returns>The data packet.</returns>
        public static byte[] DataPacketBuilder(AcerMessageType_Index messageType, byte[] messageBytes)
        {
            //Calculate the length of the data packet
            int dataPacketSize = 7 + messageBytes.Length;

            //Build Data Packet
            byte[] packetBytes = new byte[dataPacketSize];
            packetBytes[0] = (byte)messageType;
            packetBytes[2] = 1;   // Number of messages in packet
            packetBytes[3] = (byte)messageBytes.Length;
            Array.Copy(messageBytes, 0, packetBytes, 7, messageBytes.Length);

            return packetBytes;
        }

        /// <summary>
        /// Sets system information.
        /// Equates to SetAcerGamingSystemConfiguration() in PredatorSense.exe.
        /// </summary>
        /// <param name="data">The payload to inject into the named pipe.</param>
        /// <returns>Response to setting the data.</returns>
        public static async Task<uint> Set_AcerSysConfig(byte[] commandBytes)
        {
            //Build Data Packet
            byte[] packetBytes = DataPacketBuilder(AcerMessageType_Index.Command, commandBytes);

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
        public static async Task<int> Get_AcerSysInfo(SystemInfoData_Index dataRequested)
        {
            //Build the Request
            byte[] requestBytes = new byte[4];
            requestBytes[0] = 1;
            requestBytes[1] = (byte)dataRequested;

            //Build Data Packet
            byte[] packetBytes = DataPacketBuilder(AcerMessageType_Index.Request, requestBytes);

            long result = 0;
            try
            {
                using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "predatorsense_service_namedpipe", PipeDirection.InOut))
                {
                    pipeStream.Connect();

                    pipeStream.Write(packetBytes, 0, packetBytes.Length);
                    pipeStream.WaitForPipeDrain();

                    byte[] buffer = new byte[13];
                    pipeStream.Read(buffer, 0, buffer.Length);

                    result = (long)BitConverter.ToUInt64(buffer, 5);

                    pipeStream.Close();
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            if (((long)result & (long)byte.MaxValue) != 0L)
            {
                return -1;
            }

            var rtn = (int)((long)(result >> 8) & (long)ushort.MaxValue);

            return rtn;
        }
    }
}

﻿using System;
using System.IO.Pipes;
using System.Threading;
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
        public static async Task<long> Set_AcerSysConfig(byte[] commandBytes)
        {
            //Build data packet
            byte[] packetBytes = await DataPacketBuilder(AcerMessageType_Index.Command, commandBytes);

            //Send data packet to the Predator Service
            long result = await DataPacketSender(packetBytes);

            return result;
        }

        /// <summary>
        /// Sets system information.
        /// Equates to GetAcerGamingSystemInformation() in PredatorSense.exe.
        /// </summary>
        /// <param name="data">The payload to read from the named pipe.</param>
        /// <returns>The value of the data that was requested.</returns>
        public static async Task<int> Get_AcerSysInfo(AcerSysInfo_Index dataRequested)
        {
            //Build the request
            byte[] requestBytes = new byte[4];
            requestBytes[0] = 1;
            requestBytes[1] = (byte)dataRequested;

            //Build data packet
            byte[] packetBytes = await DataPacketBuilder(AcerMessageType_Index.Request, requestBytes);

            //Send data packet to the Predator Service
            long result = await DataPacketSender(packetBytes);

            //Translate the result
            var rtn = (int)((result >> 8) & ushort.MaxValue);

            return rtn;
        }

        /// <summary>
        /// Builds the data packet to be sent over the named pipe.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="messageBytes"></param>
        /// <returns>The data packet.</returns>
        public static async Task<byte[]> DataPacketBuilder(AcerMessageType_Index messageType, byte[] messageBytes)
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
        /// Sends a data packet over the named pipe to the Predator Service and returns the response.
        /// </summary>
        /// <param name="packetBytes">The data packet to send to the Predator Service.</param>
        /// <returns>Response from the Predator Service.</returns>
        public static async Task<long> DataPacketSender(byte[] packetBytes)
        {
            long result = 0;
            try
            {
                using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "predatorsense_service_namedpipe", PipeDirection.InOut))
                {
                    pipeStream.Connect(250);
                    if (!pipeStream.IsConnected)
                    {
                        throw new Exception("Could not connect to Predator Service.");
                    }

                    pipeStream.Write(packetBytes, 0, packetBytes.Length);

                    WaitForPipeDrainWithTimeout(pipeStream, 1000);

                    byte[] buffer = new byte[13];
                    pipeStream.Read(buffer, 0, buffer.Length);

                    result = (long)BitConverter.ToUInt64(buffer, 5);

                    pipeStream.Close();
                    pipeStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public static void WaitForPipeDrainWithTimeout(NamedPipeClientStream pipe, int timeoutMilliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();

            // Start a new thread to wait for pipe drain
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    pipe.WaitForPipeDrain();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    if (!tcs.TrySetException(ex))
                    {
                        throw;
                    };
                }
            });

            // Wait for the task to complete, with a timeout
            if (!tcs.Task.Wait(timeoutMilliseconds))
            {
                throw new TimeoutException("Predator Service did not read data packet in a timely manner.");
            }

            // If the task completed with an exception, rethrow it
            if (tcs.Task.IsFaulted)
            {
                throw tcs.Task.Exception;
            }
        }
    }
}

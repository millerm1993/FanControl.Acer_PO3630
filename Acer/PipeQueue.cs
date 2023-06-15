using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FanControl.Acer_PO3630.Acer.Enums;

namespace FanControl.Acer_PO3630.Acer
{
    internal class PipeQueue
    {
        private static List<PipeQueueItem> queue { get; set; } = new List<PipeQueueItem>();
        private static bool LoopRunning = false;
        private static bool LoopStopping = false;

        public static async void Stop()
        {
            //Empty the queue wait a bit and stop the loop
            LoopStopping = true;

            queue.Clear();
            await Task.Delay(100);

            await Wrapper.Set_AllAuto(bForceQueue: true);

            await Task.Delay(100);
            queue.Clear();
            LoopRunning = false;
        }
        public static async Task<long> Add(byte[] packetBytes, bool bForceQueue = false)
        {
            if (!bForceQueue && LoopStopping)
            {
                return 0;
            }

            var messageType = (AcerMessageType_Index)packetBytes[0];
            byte iIdentifier = 0;

            //Find our Identifier
            switch (messageType)
            {
                case AcerMessageType_Index.Command:
                    iIdentifier = packetBytes[12];
                    break;
                case AcerMessageType_Index.Request:
                    iIdentifier = packetBytes[8];
                    break;
                default:
                    throw new Exception("Invalid Message Type");
            }

            try
            {
                int iItemIndex = queue.FindIndex(x => x.Identifier == iIdentifier);

                //Manage our queue
                if (iItemIndex > 1)
                {
                    //If something is not already in the queue and not currently executing (index 0), just update the packet.
                    queue[iItemIndex].DataBytes = packetBytes;
                }
                else
                {
                    //If something is not in the queue, add it.
                    queue.Add(new PipeQueueItem() { Identifier = iIdentifier, DataBytes = packetBytes });
                }

                //Wait for a result to be loaded.
                while (queue.Count > 0)
                {
                    //Make sure our loop is running
                    if (!LoopRunning)
                    {
                        PipeLoop();
                    }

                    //If we found a result for our Identifier, return it.
                    if (queue[0].Identifier == iIdentifier && queue[0].Result > 0)
                    {
                        return queue[0].Result;
                    }

                    //Wait before checking again
                    await Task.Delay(10);
                }
            }
            catch(Exception e)
            {
                return -100;
            }

            return -1;
        }

        public static async void PipeLoop()
        {
            try
            {
                LoopRunning = true;
                while (LoopRunning)
                {
                    //Let the service have a gap between requests
                    await Task.Delay(50);

                    //Make sure we have something in the queue
                    if (queue.Count == 0)
                    {
                        continue;
                    }

                    //Send our packet and wait for result
                    queue[0].Result = await DataPacketSender(queue[0].DataBytes);

                    //Give our queue loops time to find and load the Result
                    await Task.Delay(50);

                    //Remove this entry from the queue
                    queue.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                LoopRunning = false;
                throw;
            }
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

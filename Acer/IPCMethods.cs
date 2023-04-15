using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.Acer_PO3630.Acer
{
    internal class IPCMethods
    {
        //private const int CMF_CMD_INDEX = 0;
        //private const int CMF_CMD_SIZE = 2;
        //private const int CMF_NUM_ARG_INDEX = 2;
        //private const int CMF_NUM_ARG_SIZE = 1;
        //private const int CMF_ARG_SIZE_SIZE = 4;
        //private const int RMF_NUM_ARG_INDEX = 0;
        //private const int RMF_NUM_ARG_SIZE = 1;
        //private const int RMF_ARG_SIZE_SIZE = 4;

        //[DllImport("User32.dll")]
        //private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool PostMessage(IntPtr hwnd, uint wMsg, UIntPtr wParam, IntPtr lParam);

        public static void SendCommandByNamedPipe(
          NamedPipeClientStream client,
          int cmdCode,
          params object[] args)
        {
            int length = 3;
            for (int index = 0; index < args.Length; ++index)
            {
                int sizeOnMemory = IPCMethods.GetSizeOnMemory(args[index]);
                length += 4 + sizeOnMemory;
            }

            byte[] numArray = new byte[length];

            int destinationIndex1 = 0;
            Array.Copy((Array)BitConverter.GetBytes(cmdCode), 0, (Array)numArray, destinationIndex1, 2);

            int destinationIndex2 = destinationIndex1 + 2;
            Array.Copy((Array)BitConverter.GetBytes(args.Length), 0, (Array)numArray, destinationIndex2, 1);

            int destinationIndex3 = destinationIndex2 + 1;
            for (int index = 0; index < args.Length; ++index)
            {
                int sizeOnMemory = IPCMethods.GetSizeOnMemory(args[index]);
                Array.Copy((Array)BitConverter.GetBytes(sizeOnMemory), 0, (Array)numArray, destinationIndex3, 4);
                destinationIndex3 += 4;
                if (sizeOnMemory > 0)
                {
                    Array.Copy(!(args[index].GetType() == typeof(string)) ? (!(args[index].GetType() == typeof(byte[])) ? (Array)IPCMethods.AnyToBytes(args[index]) : (Array)args[index]) : (Array)Encoding.Unicode.GetBytes((string)args[index] + "\0"), 0, (Array)numArray, destinationIndex3, sizeOnMemory);
                    destinationIndex3 += sizeOnMemory;
                }
            }

            if (length != destinationIndex3)
            {
                throw new Exception("Something wrong while creating command message.");
            }

            client.Write(numArray, 0, numArray.Length);
        }

        //public static bool SendCommandByWindowMessage(
        //  string className,
        //  string windowName,
        //  uint message,
        //  uint wParam,
        //  int lParam)
        //{
        //    IntPtr window = IPCMethods.FindWindow(className, windowName);
        //    return !(window == IntPtr.Zero) && IPCMethods.PostMessage(window, message, new UIntPtr(wParam), new IntPtr(lParam));
        //}

        private static int GetSizeOnMemory(object obj) => !(obj.GetType() == typeof(string)) ? (!(obj.GetType() == typeof(byte[])) ? Marshal.SizeOf(obj) : ((byte[])obj).Length) : ((string)obj).Length * 2 + 2;

        private static byte[] AnyToBytes(object obj)
        {
            byte[] bytes = new byte[Marshal.SizeOf(obj)];
            GCHandle gcHandle = GCHandle.Alloc((object)bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, gcHandle.AddrOfPinnedObject(), false);
            gcHandle.Free();
            return bytes;
        }

        //public static T BytesToStruct<T>(byte[] bytes)
        //{
        //    if (bytes == null)
        //        return default(T);
        //    if (bytes.Length == 0)
        //        return default(T);
        //    int length = bytes.Length;
        //    IntPtr num = Marshal.AllocHGlobal(length);
        //    try
        //    {
        //        Marshal.Copy(bytes, 0, num, length);
        //        return (T)Marshal.PtrToStructure(num, typeof(T));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error in BytesToStruct ! " + ex.Message);
        //    }
        //    finally
        //    {
        //        Marshal.FreeHGlobal(num);
        //    }
        //}
    }
}

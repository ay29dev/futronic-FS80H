using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{ 

        public class Futronic
        {
        /// <summary>
        /// Using .net framwork 4.6 https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net462-developer-pack-offline-installer
        /// From the reference add new lib System.Drawing
        /// Add the ftrScanAPI in bin debug folder
        /// https://github.com/controlid/integracao/blob/master/Ponto%20Eletr%C3%B4nico/test-C%23-LeitorUSB/Futronic.cs
        /// </summary>
        #region Futronic API

        struct _FTRSCAN_FAKE_REPLICA_PARAMETERS
            {
                bool bCalculated;
                int nCalculatedSum1;
                int nCalculatedSumFuzzy;
                int nCalculatedSumEmpty;
                int nCalculatedSum2;
                double dblCalculatedTremor;
                double dblCalculatedValue;
            }

            struct _FTRSCAN_FRAME_PARAMETERS
            {
                int nContrastOnDose2;
                int nContrastOnDose4;
                int nDose;
                int nBrightnessOnDose1;
                int nBrightnessOnDose2;
                int nBrightnessOnDose3;
                int nBrightnessOnDose4;
                _FTRSCAN_FAKE_REPLICA_PARAMETERS FakeReplicaParams;
                _FTRSCAN_FAKE_REPLICA_PARAMETERS Reserved;

                public bool isOK { get { return nDose != -1; } }
            }

            struct _FTRSCAN_IMAGE_SIZE
            {
                public int nWidth;
                public int nHeight;
                public int nImageSize;
            }

            [DllImport("ftrScanAPI.dll")]
            static extern bool ftrScanIsFingerPresent(IntPtr ftrHandle, out _FTRSCAN_FRAME_PARAMETERS pFrameParameters);
            [DllImport("ftrScanAPI.dll")]
            static extern IntPtr ftrScanOpenDevice();
            [DllImport("ftrScanAPI.dll")]
            static extern void ftrScanCloseDevice(IntPtr ftrHandle);
            [DllImport("ftrScanAPI.dll")]
            static extern bool ftrScanSetDiodesStatus(IntPtr ftrHandle, byte byGreenDiodeStatus, byte byRedDiodeStatus);
            [DllImport("ftrScanAPI.dll")]
            static extern bool ftrScanGetDiodesStatus(IntPtr ftrHandle, out bool pbIsGreenDiodeOn, out bool pbIsRedDiodeOn);
            [DllImport("ftrScanAPI.dll")]
            static extern bool ftrScanGetImageSize(IntPtr ftrHandle, out _FTRSCAN_IMAGE_SIZE pImageSize);
            [DllImport("ftrScanAPI.dll")]
            static extern bool ftrScanGetImage(IntPtr ftrHandle, int nDose, byte[] pBuffer);

            #endregion

            static IntPtr device;
            //static void Main()
            //{
            //    bool x2 = false;
            //    IntPtr x = IntPtr.Zero;
            //    if (x == device)
            //    {
            //        x2 = true;
            //        device = ftrScanOpenDevice();
            //        Bitmap b = ExportBitMap();
            //    }
            //    Console.WriteLine("Hello, World!");
            //}

            public bool Init()
            {
                if (!Connected)
                    device = ftrScanOpenDevice();
                return Connected;
            }

            public bool Connected
            {
                get { return (device != IntPtr.Zero); }
            }

            public void Dispose()
            {
                if (Connected)
                {
                    ftrScanCloseDevice(device);
                    device = IntPtr.Zero;
                }
            }

            public Bitmap ExportBitMap()
            {
                if (!Connected)
                    return null;

                var t = new _FTRSCAN_IMAGE_SIZE();
                ftrScanGetImageSize(device, out t);
                byte[] arr = new byte[t.nImageSize];
                ftrScanGetImage(device, 4, arr);

                var bmp = new Bitmap(t.nWidth, t.nHeight);
                for (int x = 0; x < t.nWidth; x++)
                {
                    for (int y = 0; y < t.nHeight; y++)
                    {
                        int a = 255 - arr[y * t.nWidth + x];
                        bmp.SetPixel(x, y, Color.FromArgb(a, a, a));
                    }
                }
                return bmp;
            }

            public void GetDiodesStatus(out bool green, out bool red)
            {
                ftrScanGetDiodesStatus(device, out green, out red);
            }

            public void SetDiodesStatus(bool green, bool red)
            {
                ftrScanSetDiodesStatus(device, (byte)(green ? 255 : 0), (byte)(red ? 255 : 0));
            }

            public bool IsFinger()
            {
                var t = new _FTRSCAN_FRAME_PARAMETERS();
                bool dedo = ftrScanIsFingerPresent(device, out t);
                if (!t.isOK)
                {
                    Dispose();
                    return false;
                }
                else
                    return dedo;
            }
        }
    }

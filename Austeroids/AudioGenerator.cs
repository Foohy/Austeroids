using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using System.Runtime.InteropServices;
using System.Windows;

namespace Austeroids
{
    class AudioGenerator
    {
        private STREAMPROC renderAudioProc;
        private int[] audioData = new int[32768];
        private long time;
        private int outputHandle;
        private int sampleRate;

        public AudioGenerator(int sampleRate = 44100)
        {
            string email = Environment.GetEnvironmentVariable("BASS_REGISTRATION_EMAIL");
            string key = Environment.GetEnvironmentVariable("BASS_REGISTRATION_KEY");
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(key))
                BassNet.Registration(email, key);

            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            outputHandle = Bass.BASS_StreamCreatePush(sampleRate, 2, BASSFlag.BASS_DEFAULT, IntPtr.Zero); //BASS_SAMPLE_8BIT
            Console.WriteLine(Bass.BASS_ErrorGetCode());
            Bass.BASS_ChannelPlay(outputHandle, false);
            Bass.BASS_ChannelSetAttribute(outputHandle, BASSAttribute.BASS_ATTRIB_VOL, 1.0f);
            Console.WriteLine(Bass.BASS_ErrorGetCode());

            this.sampleRate = sampleRate;
        }

        public int GetSampleRate()
        {
            return sampleRate;
        }

        public bool NeedsMoreData()
        {
            return Bass.BASS_ChannelGetData(outputHandle, new byte[0], (int)BASSData.BASS_DATA_AVAILABLE) < 40000;
        }

        public void PushWorld(Vector[] Buffer, int length)
        {
            //Create sample data from the list of points
            for (int i = 0; i < length; i++)
            {
                audioData[i] = vectorToSample(Buffer[i]);
            }

            Bass.BASS_StreamPutData(outputHandle, audioData, length);
        }

        public void MakeSomeNoise()
        {
            int handle = Bass.BASS_StreamCreatePush(44100, 2, BASSFlag.BASS_DEFAULT, IntPtr.Zero); //BASS_SAMPLE_8BITS

            Bass.BASS_ChannelPlay(handle, false);
            byte[] nullArr = new byte[0];
            Random noise = new Random();
            while (true)
            {
                if (Bass.BASS_ChannelGetData(handle, nullArr, (int)BASSData.BASS_DATA_AVAILABLE) < 10000)
                {
                    for (int i = 0; i < 2048; i++)
                    {
                        time++;

                        Point pC = Interop.GetCursorPosition();
                        double width = 1920;
                        double height = 1080;
                        double correctPY = ((pC.Y / height) - 0.5) * -2 * ushort.MaxValue * 0.5;
                        double correctPX = ((pC.X / width) - 0.5) * 2 * ushort.MaxValue;
                        correctPX = Clamp(correctPX, -ushort.MaxValue / 3, ushort.MaxValue / 3);
                        correctPY = Clamp(correctPY, -ushort.MaxValue / 3, ushort.MaxValue / 3);

                        double mousePhase = 30000 - ((pC.Y / height) - 0.5) * 10000f;

                        double t = Math.Sqrt(pC.Y) * 2 + 1;// (Math.Cos(time / 40000f) + 1) * 100 + 11;
                        double spiral = 16384 - Math.Pow(i*2, 1.4);

                        ushort right = (ushort)(Math.Pow(Math.Sin(i*4 / t + time/40000f),2) * spiral + correctPY);
                        ushort left = (ushort)(Math.Cos(i * 4 / t + time / 400000f) * spiral + correctPX);

                        right += (ushort)((noise.NextDouble() - 0.5f) * 11241 * ((pC.Y / height) - 0.5));
                        left += (ushort)((noise.NextDouble() - 0.5f) * 11241 * ((pC.X / width) - 0.5));
                        //left += noise.NextDouble() - 0.5f;
                        //right = (ushort)(right < (ushort.MaxValue / 2) ? -10000 : 10000);
                        //left = (ushort)(left < (ushort.MaxValue / 2) ? -10000 : 10000);

                        audioData[i] = (int)normalizeSamples(left, right);
                    }

                    Bass.BASS_StreamPutData(handle, audioData, 2048);
                }
            }
        }

        private double Clamp(double n, double low, double high)
        {
            return Math.Max(Math.Min(n, high), low);
        }

        /*
        public void MakeSomeNoise() {
            renderAudioProc = new STREAMPROC(render);
            int handle = Bass.BASS_StreamCreate(22050*4, 2, BASSFlag.BASS_DEFAULT, renderAudioProc, IntPtr.Zero);
           

            //Bass.BASS_StreamPutData(handle, c, c.Length);
            Bass.BASS_ChannelPlay(handle, false);
        }
        
        private int render(int handle, IntPtr buffer, int length, IntPtr user)
        {

            int numInts = length / 4;
            int realLen = Math.Min(numInts, audioData.Length);
            Console.WriteLine("{0}, {1}", numInts, realLen);
            //Generate the scene data
            for (int i = 0; i < realLen; i++)
            {
                time++;

                Point pC = Interop.GetCursorPosition();

                double t = Math.Sqrt(pC.Y) * 8;// (Math.Cos(time / 40000f) + 1) * 100 + 11;

                ushort right = (ushort)(Math.Sin(i / t) * 8192 + pC.Y * -100f);
                ushort left = (ushort)(Math.Cos(i / t) * 8192 + pC.X * 100f);

                //right = (ushort)(right < (ushort.MaxValue / 2) ? -10000 : 10000);
                //left = (ushort)(left < (ushort.MaxValue / 2) ? -10000 : 10000);

                audioData[i] = (int)normalizeSamples(left, right);
            }

            //Console.WriteLine(Bass.BASS_StreamPutData(handle, audioData, audioData.Length));
            Marshal.Copy(audioData, 0, buffer, realLen);
            return realLen * 4;
        }*/
        private int vectorToSample(Vector v)
        {
            return (int)normalizeSamples((ushort)((v.X / 1000f) * ushort.MaxValue), (ushort)((v.Y / 1000f) * ushort.MaxValue));
        }

        private uint normalizeSamples(ushort left, ushort right)
        {
            //Console.WriteLine("{0:X}, {1:X}", left, right);
            left = Math.Min(left, ushort.MaxValue);

            uint rightShifted = ((uint)Math.Min(right, ushort.MaxValue)) << 16;
            //Console.WriteLine("{0:X}", rightShifted);
            //Console.WriteLine("{0:X}, {1:X}", left, (Math.Min(left, ushort.MaxValue) << 15));

            return left | rightShifted;
        }
    }
}

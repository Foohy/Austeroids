using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Austeroids
{
    class World
    {
        public List<Entity> Entities = new List<Entity>();
        protected Vector[] sampleBuffer = new Vector[32768];
        AudioGenerator audio = new AudioGenerator();
        Stopwatch sw = Stopwatch.StartNew();
        private int bufferLen = 3000;
        private float noiseAmount;
        private Vector noiseCenter = new Vector(0,0);
        long tick = 0;

        public void StartGame()
        {
            audio = new AudioGenerator(192000);
            sw = Stopwatch.StartNew();
            SetTone(440/2);
        }

        public void Tick()
        {
            //SetTone((int)CMath.SinBetween(sw.ElapsedMilliseconds / 1000f, 100, 440));
            //SetNoise(CMath.SinBetween(CurrentTime(), 0, 100.0f));
            if (audio.NeedsMoreData())
            {
                tick++;

                int bufferLength = 0;
                Array.Clear(sampleBuffer, 0, sampleBuffer.Length);
                for (int i = 0; i < Entities.Count; i++)
                {

                    Entity ent = Entities[i];

                    float curTime = CurrentTime();
                    ent.Think(curTime);

                    int length;
                    Vector[] points = ent.Draw(curTime, out length);
                    Array.Copy(points, 0, sampleBuffer, bufferLength, length);
                    bufferLength += length;


                    //SamplePoints.AddRange(points);
                }

                //Remove deleted entities
                Entities.RemoveAll(ent => ent.MarkedForDelete);

                for (int i = 0; i < bufferLength; i++)
                {
                    //float randAmt = Math.Max(0, 500 - (noiseCenter - sampleBuffer[i]).Length()) / 1500f;
                    //sampleBuffer[i] += Vector.RandomVector(noiseAmount * randAmt);
                }

                for (int i = bufferLength; i < bufferLen; i++)
                {
                    double perc = (Math.PI * 8) * (float)(i - bufferLength) / (bufferLen - bufferLength);
                    //Console.WriteLine("{0}, {1}", perc, bufferLength);

                    sampleBuffer[i] = new Vector((float)Math.Cos(perc), (float)Math.Sin(perc)) * 500;
                    sampleBuffer[i] += Vector.RandomVector(noiseAmount );
                }


                audio.PushWorld(sampleBuffer, bufferLen);
            }
        }

        public float CurrentTime()
        {
            return sw.ElapsedMilliseconds / 1000f;
        }

        public long CurrentTick()
        {
            return tick;
        }

        public void SetNoise(float amt)
        {
            noiseAmount = amt;
        }

        public void SetTone(int Hertz)
        {
            bufferLen = Math.Min(CMath.NearestOf(audio.GetSampleRate() * 8 / Hertz, 4), sampleBuffer.Length);
        }

        public T Create<T>() where T : Entity
        {
            //T ent = (T)Activator.CreateInstance(typeof(T), new object[] { v, n });
            T ent = (T)Activator.CreateInstance<T>();
            ent.SetWorld(this);

            Entities.Add(ent);
            return ent;
        }

        public void Destroy(Entity ent)
        {

        }

        public T[] GetByType<T>() where T : Entity
        {
            List<T> entities = new List<T>();

            foreach( Entity ent in Entities) {
                if (ent is T)
                {
                    entities.Add((T)ent);
                }
            }

            return entities.ToArray();
        }
    }
}

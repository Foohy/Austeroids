using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austeroids.Entities
{
    class Noise : Entity
    {
        private float startTime;
        private float startNoise;
        private float falloff;
        public override void Think(float curTime, float deltaTime)
        {
            float noiseLeft = CMath.Lerp(1 - (startTime + falloff - OwningWorld.CurrentTime()) / falloff, startNoise, 0);
            Console.WriteLine(noiseLeft);
            noiseLeft = Math.Max(noiseLeft, 0);

            OwningWorld.SetNoise(noiseLeft);

            if (noiseLeft <= 0)
                this.Destroy();
        }

        public void SetupNoise(float startNoiseAmt, float falloffTime)
        {
            startNoise = startNoiseAmt;
            falloff = falloffTime;

            this.startTime = OwningWorld.CurrentTime();
        }

        public override Vector[] Draw(float curTime, out int length)
        {
            length = 0;
            return new Vector[0];
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austeroids.Entities
{
    class Asteroid : Entity
    {
        Vector[] samplePointBuffer = new Vector[4096];
        float radius = 50;
        public bool Exploding { get { return explodeTime > 0; } }
        float explodeTime = 0;
        public Asteroid()
        {
            this.SetPosition((float)CMath.Rand.NextDouble() * 650 - 650 / 2,
                (float)CMath.Rand.NextDouble() * 650 - 650 / 2);

            radius = (float)CMath.Rand.NextDouble() * 50 + 40;
        }

        public void Explode()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector away = new Vector(1, 0).Rotate((float)((i / 4f - 1f) * Math.PI * 2 + CMath.Rand.NextDouble() * 2f));

                AsteroidGibs gib = OwningWorld.Create<AsteroidGibs>();
                gib.SetupGib(this.Position, away * (float)(CMath.Rand.NextDouble() * 2 + 0.1f));


            }

            OwningWorld.Create<Noise>().SetupNoise(200, 0.2f);
            explodeTime = OwningWorld.CurrentTime();
            //this.Destroy();
        }

        public bool Within(Vector test)
        {
            return (test - this.Position).LengthSquared() < radius * radius;
        }

        public override void Think(float curTime, float deltaTime)
        {
            if (Exploding && curTime - explodeTime > 4f)
            {
                this.Destroy();
            }
        }

        public override Vector[] Draw(float curTime, out int length)
        {
            length = 0;
            Vector[] Points = Render.DrawPoly(Render.DrawCircle(this.Position, radius, 6), 3);
            for (int i = 0; Exploding && i < Points.Length; i++)
            {
                Points[i] += Vector.RandomVector((curTime - explodeTime) * 200f + 50f);
            }
                length = Points.Length;
            samplePointBuffer = Points;
            /*
            for (int i = 0; i < 16; i++)
            {
                Array.Copy(points, 0, samplePointBuffer, length, points.Length);
                 length += points.Length;
            }*/
            
            //samplePointBuffer = Render.DrawCircle(this.Position, 50, ptCount);
            //length = ptCount;

            return samplePointBuffer;
        }
        
    }
}

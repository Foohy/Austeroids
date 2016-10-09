using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austeroids.Entities
{
    class AsteroidGibs : Entity
    {
        Vector velocity;
        float length;

        float dieTime = 0f;
        float startTime;

        public void SetupGib(Vector Position, Vector Velocity)
        {
            this.velocity = Velocity;
            this.SetPosition(Position);
            this.dieTime = OwningWorld.CurrentTime() + (float)CMath.Rand.NextDouble() * 5f + 2;
            this.startTime = OwningWorld.CurrentTime();
        }
        public override void Think(float curTime, float deltaTime)
        {
            SetPosition(this.Position.X + velocity.X, this.Position.Y + velocity.Y);

            if (dieTime < OwningWorld.CurrentTime()){
                this.Destroy();
            }
        }

        private float getRadius()
        {
            return (OwningWorld.CurrentTime() - startTime / (dieTime - startTime)) * 25;
        }

        public override Vector[] Draw(float curTime, out int length)
        {
            Vector[] Points = Render.DrawPoly(Render.DrawCircle(this.Position, getRadius(), 4), 3);
            length = Points.Length;
            return Points;
        }
    }
}

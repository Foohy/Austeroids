using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austeroids.Entities
{
    class Bullet : Entity
    {
        Vector velocity;
        float length;
        float spawnTime = 0;
        float lifeTime = 0;

        public Bullet()
        {

        }

        public void SetupBullet(Vector Position, Vector Velocity)
        {
            this.velocity = Velocity;
            this.SetPosition(Position);

            this.spawnTime = OwningWorld.CurrentTime();
            this.lifeTime = 5f;
        }

        public override void Think(float curTime, float deltaTime)
        {
            SetPosition(this.Position + velocity * deltaTime);

            Asteroid[] asses = OwningWorld.GetByType<Asteroid>();
            foreach (Asteroid ass in asses)
            {
                if (ass.Within(this.Position) && !ass.MarkedForDelete && !ass.Exploding)
                {
                    ass.Explode();
                    this.Destroy();
                    break;
                }
            }

            if (OwningWorld.CurrentTime() - spawnTime > lifeTime)
            {
                this.Destroy();
            }
        }


        public override Vector[] Draw(float curTime, out int length)
        {

            int numPoints = (int)(15 * (1 - (OwningWorld.CurrentTime() - spawnTime) / lifeTime));
            numPoints = Math.Max(numPoints, 0);
            Vector[] p = Render.DrawLine(this.Position, this.Position - this.velocity * 0.05f, numPoints);

            length = p.Length;
            return p;
        }
    }
}

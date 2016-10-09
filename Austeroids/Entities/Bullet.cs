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

        public Bullet()
        {

        }

        public void SetupBullet(Vector Position, Vector Velocity)
        {
            this.velocity = Velocity;
            this.SetPosition(Position);
        }

        public override void Think(float curTime)
        {
            SetPosition(this.Position.X + velocity.X, this.Position.Y + velocity.Y);

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
        }


        public override Vector[] Draw(float curTime, out int length)
        {
            Vector[] p = Render.DrawLine(this.Position, this.Position - this.velocity * 10f, 5 );

            length = p.Length;
            return p;
        }
    }
}

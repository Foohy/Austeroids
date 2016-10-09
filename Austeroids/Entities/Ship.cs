using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Austeroids.Entities
{

    class Ship : Entity
    {
        protected float rotation;
        float timeOffset = 0;
        Vector[] samplePointBuffer = new Vector[4096];
        Vector velocity = new Vector(0, 0);
        float rotationVelocity = 0;
        long lastShoot = 0;

        bool wasPressed = false;
        public Ship()
        {
            timeOffset = (float)(CMath.Rand.NextDouble() * 100f);
        }

        private void OnShoot() {
            Vector bulletVel = new Vector(0, 1f).Rotate(rotation - (float)Math.PI / 2f) * 10f;
            this.OwningWorld.Create<Bullet>().SetupBullet(this.Position, bulletVel);

            lastShoot = OwningWorld.CurrentTick();
        }

        public override void Think(float curTime)
        {
            curTime += timeOffset;

            if (Keyboard.IsKeyDown(Key.B) )
            {
                if (!wasPressed) {
                    OnShoot();
                    
                }
                wasPressed = true;
            }
            else
            {
                wasPressed = false;
            }

            float thrust = 0;
            if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.W)) { thrust += 1; }
            if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.S)) { thrust -= 1; }
            thrust *= 0.1f;

            float spin = 0;
            if (Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.A)) { spin += 1; }
            if (Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.D)) { spin -= 1; }
            spin *= 0.001f;

            rotationVelocity += spin;

            rotation = (rotation + rotationVelocity) % 360;
            velocity += new Vector(0, thrust).Rotate(rotation - (float)Math.PI / 2f);

            //Apply dampening
            velocity = velocity * (thrust != 0 ? 0.9999f : 0.994f);
            rotationVelocity = rotationVelocity * 0.99f;
            
            SetPosition(this.Position.X + velocity.X, this.Position.Y + velocity.Y);

            long tickDiff = OwningWorld.CurrentTick() - lastShoot;
            if (tickDiff > 100 || tickDiff % 16 == 0)
            {
                OwningWorld.SetTone((int)(this.velocity.Length()*16 + 90));
               
            }
            else OwningWorld.SetTone(1000 - (int)tickDiff * 2);
        }

        public override Vector[] Draw(float curTime, out int length)
        {

            Vector front, backUp, backDown;
            front = backUp = backDown = Vector.Zero;
            getRotatedPoints(out front, out backUp, out backDown);

            int Y = CMath.NearestOf((int)Interop.GetCursorPosition().Y / 8, 4);

            length = 0;


            if (OwningWorld.CurrentTick() % 2 == 0)
            {
                Array.Copy(Render.DrawCircle(this.Position, 20, 20), 0, samplePointBuffer, length, 20);
                length += 20;
            }
            else
            {
                Vector[] points = Render.DrawTri(front, backUp, backDown, 60); //60
                Array.Copy(points, 0, samplePointBuffer, 0, points.Length);
                length = points.Length;
            }



            return samplePointBuffer;
        }

        private void getRotatedPoints(out Vector front, out Vector rearUp, out Vector rearDown) 
        {
            front = this.Position + new Vector(50, 0).Rotate(rotation);
            rearUp = this.Position + new Vector(-50, 40).Rotate(rotation);
            rearDown = this.Position + new Vector(-50, -40).Rotate(rotation);
        }
    }
}

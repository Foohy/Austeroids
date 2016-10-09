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
        Vector[] samplePointBuffer = new Vector[4096];
        Vector velocity = new Vector(0, 0);
        float rotationVelocity = 0;
        long lastShoot = 0;

        bool wasPressed = false;
        public Ship()
        {
        }

        private void OnShoot() {
            Vector bulletVel = new Vector(0, 1f).Rotate(rotation - (float)Math.PI / 2f) * 2000f;
            this.OwningWorld.Create<Bullet>().SetupBullet(this.Position, bulletVel);

            lastShoot = OwningWorld.CurrentTick();
        }

        private bool IsBoosting()
        {
            return Keyboard.IsKeyDown(Key.F);
        }

        public override void Think(float curTime, float deltaTime)
        {
            if (Keyboard.IsKeyDown(Key.Space) )
            {
                if (!wasPressed) {
                    OnShoot();
                    
                }
                wasPressed = true;
            }
            else { wasPressed = false; }

            float thrust = 0;
            if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.W)) { thrust += 1; }
            if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.S)) { thrust -= 1; }
            thrust *= 7;

            if (IsBoosting()) thrust += 20;
            else if (velocity.Length() > 1500f) { thrust = 0f; }

            float spin = 0;
            if (Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.A)) { spin += 1; }
            if (Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.D)) { spin -= 1; }
            spin *= 5;

            rotationVelocity += spin * deltaTime;

            rotation = (rotation + rotationVelocity * deltaTime) % 360;
            velocity += new Vector(0, thrust).Rotate(rotation - (float)Math.PI / 2f);

            //Apply dampening
            float dampen = thrust != 0 ? 0.9999f : 0.994f;
            if (IsBoosting())
                dampen = 1f;
            velocity = velocity * (thrust != 0 ? 0.9999f : 0.994f);
            rotationVelocity = rotationVelocity * 0.99f;
            
            SetPosition(this.Position + velocity * deltaTime);

            long tickDiff = OwningWorld.CurrentTick() - lastShoot;
            if (tickDiff > 100 || tickDiff % 16 == 0)
            {
                OwningWorld.SetTone((int)(this.velocity.Length()/8 + 90));
               
            }
            else OwningWorld.SetTone(1000 - (int)tickDiff * 2);

 
            OwningWorld.SetNoise(IsBoosting() ? 20f + this.velocity.Length()/64 : 0f);
            if (this.velocity.Length() > 10000)
            {
                OwningWorld.SetNoise(0);
            }

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
            //else if 
            {
                Vector[] points = Render.DrawTri(front, backUp, backDown, 60); //60
                Array.Copy(points, 0, samplePointBuffer, 0, points.Length);
                length = points.Length;
            }

            if (IsBoosting())
            {
                Vector Booster = (backUp + backDown) * 0.5f;
                Vector[] points = Render.DrawLine(Booster, Booster - new Vector(1, 0).Rotate(rotation) * 30f, 10);
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] += new Vector(-((float)CMath.Rand.NextDouble() * 100f - 40f), (float)CMath.Rand.NextDouble() * 20 - 10f).Rotate(rotation);
                }
                    Array.Copy(points, 0, samplePointBuffer, length, points.Length);
                length += points.Length;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austeroids
{
    abstract class Entity
    {
        public bool MarkedForDelete { get; private set; }
        public World OwningWorld { get; private set; }
        public Vector Position { get; private set; }
        public Entity(Vector pos, String name)
        {
            Position = pos;
        }

        public Entity()
        {
            Position = new Vector(0, 0);
        }

        public abstract void Think(float curTime);

        /// <summary>
        /// Draw the entity 
        /// Probably one of the stranger signatures for a draw function I've ever seen
        /// </summary>
        /// <param name="curTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="SampleData"></param>
        /// <returns>The size of the sample buffer this entity takes up</returns>
        public abstract Vector[] Draw(float curTime, out int length);

        public void SetPosition(Vector pos) 
        {
            Position = pos;
        }

        public void SetPosition(float x, float y)
        {
            SetPosition(new Vector(x % 1000, y % 1000));
        }

        public void SetWorld(World world)
        {
            this.OwningWorld = world;
        }

        public void Destroy()
        {
            MarkedForDelete = true;
        }
    }
}

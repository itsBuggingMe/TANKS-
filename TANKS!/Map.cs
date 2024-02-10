using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace TANKS_
{
    internal class Map
    {
        public readonly Rectangle[] StaticHitboxes;
        public Map(Rectangle[] rectangles, RigidBodySimulation sim)
        {
            //prob never gonna use
            throw new NotImplementedException();
            StaticHitboxes = rectangles;
            foreach (var rect in rectangles)
            {
                var b = rect;
                //sim.AddBody(new Obstacle());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TANKS_
{
    public class Obstacle : StaticHitbox
    {
        public Obstacle(Rectangle b) { Verts = new Vector2[] { new(b.Left, b.Top), new(b.Right, b.Top), new(b.Right, b.Bottom), new(b.Left, b.Bottom), new(b.Left, b.Top) }; }
    }

    public abstract class StaticHitbox : IPhysicsObject
    {
        public Vector2[] Verts { get; internal set; }
    }
    public abstract class DynamicHitbox : IPhysicsObject
    {
        internal bool Delete;
        private Vector2[] _modelVerts;
        private Vector2 _locBacking;
        public Vector2[] Verts { get; internal set; }
        public Vector2 Velocity { get; internal set; }

        public Vector2 Location
        {
            get => _locBacking;
            internal set
            {
                _locBacking = value;
                for (int i = 0; i < _modelVerts.Length; i++)
                {
                    Verts[i] = _modelVerts[i] + _locBacking;
                }
            }
        }

        protected void SetModelVerts(Vector2[] newVerts)
        {
            _modelVerts = newVerts;
            Verts = new Vector2[newVerts.Length];
            for (int i = 0; i < _modelVerts.Length; i++)
            {
                Verts[i] = _modelVerts[i] + _locBacking;
            }
        }

        internal void ApplyForce(Vector2 amount)
        {
            Velocity += amount;
        }

        public List<IPhysicsObject> Collisions = new();
    }
    public interface IPhysicsObject
    {
        public Vector2[] Verts { get; }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TANKS_
{
    internal class RigidBodySimulation
    {
        public readonly List<DynamicHitbox> DynamicBodies = new();
        private readonly List<StaticHitbox> StaticBodies = new();

        private int totalTicks = 0;

        public void Update()
        {
            int DynamicMin = 0;
            int DynamicMax = DynamicBodies.Count - 1;
            //detect collisions
            for (int i = DynamicMin; i <= DynamicMax; i++)
            {
                DynamicHitbox thisBody = DynamicBodies[i];

                for (int j = DynamicMin; j <= DynamicMax; j++)
                {
                    DynamicHitbox otherBody = DynamicBodies[j];

                    if (IsCollison(thisBody, otherBody))
                    {
                        thisBody.Collisions.Add(otherBody);
                    }
                }

                for (int j = 0; j < StaticBodies.Count; j++)
                {
                    StaticHitbox otherStatic = StaticBodies[j];

                    if (IsCollison(thisBody, otherStatic))
                    {
                        thisBody.Collisions.Add(otherStatic);
                    }
                }
            }

            for (int i = 0; i < DynamicBodies.Count; i++)
            {
                DynamicHitbox collisionResolved = DynamicBodies[i];

                if (collisionResolved.Collisions.Count != 0)
                {
                    // This has a collision
                    // Implement old ca
                    GenericCollison(collisionResolved, collisionResolved.Collisions);

                    foreach (var someOtherDynamicHitbox in collisionResolved.Collisions.Where(H => H is DynamicHitbox))
                    {
                        DynamicHitbox otherHitbox = someOtherDynamicHitbox as DynamicHitbox;
                        Vector2 transVec = collisionResolved.Location - otherHitbox.Location;
                        if (transVec == Vector2.Zero)//fuck the nans
                            continue;

                        Vector2 pointTo = Vector2.Normalize(transVec);

                        if(collisionResolved is Tank tankThis && otherHitbox is Tank tankOther)
                        {
                            float otherVel = otherHitbox.Velocity.LengthSquared();
                            float thisVel = collisionResolved.Velocity.LengthSquared();

                            float total = otherVel + thisVel;
                            otherVel /= total;
                            thisVel /= total;

                            tankOther.Health -= (int)(otherVel * 5);
                            tankThis.Health -= (int)(thisVel * 5);

                            tankOther.ApplyForce(otherVel * 2 * -pointTo);
                            tankThis.ApplyForce(thisVel * 2 * pointTo);
                        }
                    }

                    collisionResolved.Collisions.Clear();
                }
            }
            for (int i = DynamicBodies.Count - 1; i >= 0; i--)
            {
                if (DynamicBodies[i].Delete)
                    DynamicBodies.RemoveAt(i);
            }
        }

        internal class Const
        {
            public const int CollisionIterations = 14;
        }

        public static void GenericCollison(DynamicHitbox toBeMoved, List<IPhysicsObject> collisionSubject)
        {
            //MOVE BACK
            Vector2 moveAVelocity = toBeMoved.Velocity;
            toBeMoved.Location -= moveAVelocity;

            Vector2 stepSize = moveAVelocity / Const.CollisionIterations;
            Vector2 stepY = new(0, stepSize.Y);
            Vector2 stepX = new(stepSize.X, 0);

            //X
            for (int j = 0; j < Const.CollisionIterations; j++)
            {
                toBeMoved.Location += stepX;

                if (collisionSubject.Any(collisionSubject => IsCollison(toBeMoved, collisionSubject)))
                {
                    toBeMoved.Location -= stepX;

                    moveAVelocity.X = 0;
                    break;
                }
            }
            //Y
            for (int j = 0; j < Const.CollisionIterations; j++)
            {
                toBeMoved.Location += stepY;

                if (collisionSubject.Any(collisionSubject => IsCollison(toBeMoved, collisionSubject)))
                {
                    toBeMoved.Location -= stepY;
                    moveAVelocity.Y = 0;
                    break;
                }
            }
        }

        public static bool IsCollison(IPhysicsObject objectA, IPhysicsObject objectB)
        {
            if(objectA == objectB)
            {
                return false;
            }

            Vector2[] Averts = objectA.Verts;
            Vector2[] Bverts = objectB.Verts;

            for (int i = 0; i < Averts.Length - 1; i++)
            {
                for (int j = 0; j < Bverts.Length - 1; j++)
                {
                    if (MathFunc.DoLinesIntersect(Averts[i], Averts[i + 1], Bverts[j], Bverts[j + 1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public void AddBody(IPhysicsObject _object)
        {
            if (_object is DynamicHitbox)
                DynamicBodies.Add(_object as DynamicHitbox);
            if (_object is StaticHitbox)
                StaticBodies.Add(_object as StaticHitbox);
        }

        public bool PolyCollidesWith(IPhysicsObject thisBody)
        {
            for (int j = 0; j < DynamicBodies.Count; j++)
            {
                DynamicHitbox otherBody = DynamicBodies[j];

                if (IsCollison(thisBody, otherBody))
                    return true;
            }

            for (int j = 0; j < StaticBodies.Count; j++)
            {
                StaticHitbox otherStatic = StaticBodies[j];

                if (IsCollison(thisBody, otherStatic))
                    return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TANKS_;

namespace TankClient
{
    internal class Thomas_The_Tank : Tank
    {
        protected override void Initalize()
        {
            TankColor = TankColor.Copper;
            Weapon = Weapon.Cannon;
        }
        Vector2 center = new Vector2(1280) * 0.5f;
        protected override void Update(Tank[] otherTanks)
        {
            if(!otherTanks.Any(t => t.Name != "Thomas The Tank"))
            {
                Shout("Targets Exterminated");
                if(Vector2.DistanceSquared(Location, center) > 600)
                {
                    RotateTowards(center);
                    Accelerate(1);
                }
                return;
            }

            int closestIndex = 0;
            float closestDistance = float.MaxValue;
            for(int i = 0; i < otherTanks.Length; i++)
            {
                if (otherTanks[i].Name == "Thomas The Tank")
                    continue;

                float distance = Vector2.DistanceSquared(otherTanks[i].Location, Location);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }
            Tank closestTank = otherTanks[closestIndex];

            float ticksTillReach = Vector2.Distance(closestTank.Location, Location) / 16;
            Vector2 pointTowardsLoc = closestTank.Location - RotateVector(Vector2.UnitX * closestTank.Velocity, BaseRotation) * ticksTillReach;
            RotateTurretTowards(
                pointTowardsLoc
                );

            Vector2 potentialTarget = RotateVectorOrigin(closestTank.Location, 90, Location);
            if(potentialTarget.X < 70 || potentialTarget.Y < 70 || potentialTarget.Y > 830 || potentialTarget.X > 830)
            {
                potentialTarget = center;
            }
            
            RotateTowards(potentialTarget);
            Accelerate(1);
            Fire();

            Shout("Attacking " + closestTank.Name);
        }

        public static Vector2 RotateVectorOrigin(Vector2 vector, float angle, Vector2 origin)
        {
            Vector2 translatedVector = vector - origin;

            float angleInRadians = angle;
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);

            float newX = translatedVector.X * cos - translatedVector.Y * sin;
            float newY = translatedVector.X * sin + translatedVector.Y * cos;

            Vector2 rotatedVector = new Vector2(newX, newY) + origin;

            return rotatedVector;
        }

        public static Vector2 RotateVector(Vector2 vector, float angle)
        {
            float angleInRadians = MathHelper.ToRadians(angle);
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);

            float newX = vector.X * cos - vector.Y * sin;
            float newY = vector.X * sin + vector.Y * cos;

            return new Vector2(newX, newY);
        }
    }
}

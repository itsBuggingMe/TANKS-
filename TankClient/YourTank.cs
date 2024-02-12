using TANKS_;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TankClient
{
    // The name of the class determines the name of the tank
    internal class Bot : Tank
    {
        protected override void Initalize()
        {
            //This function determines tank settings
            TankColor = TankColor.Copper; //TankColor.Copper, TankColor.Blue, TankColor.Green
            Weapon = Weapon.Double; //Weapon.Cannon, Weapon.Double, Weapon.Normal
        }

        Vector2 targetLoc = GetRandomPoint();

        private static Vector2 GetRandomPoint()
        {
            return new Vector2(Random.Shared.NextSingle() * 1080, Random.Shared.NextSingle() * 1080);
        }

        protected override void Update(Tank[] otherTanks)
        {
            if(otherTanks.Length != 0)
            {
                Vector2 targetLocation = otherTanks[0].Location;
                Vector2 targetVelocity = otherTanks[0].Velocity;

                float DistanceToOther = Vector2.Distance(targetLocation, Location);
                const float ProjectileVelocity = 32f;
                float ticksToArrive = DistanceToOther / ProjectileVelocity;

                Vector2 futureLocation = targetLocation + targetVelocity * ticksToArrive;
                RotateTurretTowards(futureLocation);
                Fire();

                RotateTowards(targetLoc);
                Accelerate(1);

                if (Vector2.Distance(otherTanks[0].Location, Location) < 200)
                {
                    targetLoc = GetRandomPoint();
                }
            }
            else
            {
                Shout("Lonely :(");
            }
        }
    }
}

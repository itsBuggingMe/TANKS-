using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TANKS_;

namespace TankClient
{
    internal class YourTank : Tank
    {
        public override void Initalize()
        {
            TankColor = TankColor.Blue;
            Weapon = Weapon.Cannon;
        }

        Vector2 targetLoc = GetRandomLoc();

        private static Vector2 GetRandomLoc()
        {
            return new Vector2(Random.Shared.Next(120, 960-120), Random.Shared.Next(120,960-120));
        }

        public override void Update(Tank[] otherTanks)
        {
            Accelerate(1);
            RotateTowards(targetLoc);
            if (otherTanks.Length != 0)
                RotateTurretTowards(otherTanks[0].Location);

            if(Vector2.Distance(targetLoc, Location) < 200)
            {
                targetLoc = GetRandomLoc();
            }
            Shout(targetLoc.ToString());
        }
    }
}

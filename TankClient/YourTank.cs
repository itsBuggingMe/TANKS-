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
            TankColor = TankColor.Copper;
            Weapon = Weapon.Normal;
        }

        Vector2 targetLoc = GetRandomLoc();

        private static Vector2 GetRandomLoc()
        {
            return new Vector2(Random.Shared.Next(0, 960), Random.Shared.Next(0,960));
        }

        public override void Update(Tank[] otherTanks)
        {
            Accelerate(1);
            RotateTowards(targetLoc);
            RotateTurretTowards(targetLoc);

            if(Vector2.Distance(targetLoc, Location) < 200)
            {
                targetLoc = GetRandomLoc();
            }
        }
    }
}

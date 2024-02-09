using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TANKS_;

namespace YourTank
{
    internal class Persons_Tank : Tank
    {
        protected override void Initalize()
        {
            TankColor = TankColor.Copper;
            Weapon = Weapon.Double;
        }

        protected override void Update(Tank[] otherTanks)
        {
            var kstat = Keyboard.GetState();

            if (kstat.IsKeyDown(Keys.W))
            {
                Accelerate(1);
            }
            if(kstat.IsKeyDown(Keys.A))
            {
                RotateBase(-1);
            }
            if (kstat.IsKeyDown(Keys.D))
            {
                RotateBase(1);
            }

            RotateTurretTowards(Mouse.GetState().Position.ToVector2());
            Fire();
        }
    }
}

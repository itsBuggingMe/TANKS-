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
        public override void Initalize()
        {
            TankColor = TankColor.Copper;
            Weapon = Weapon.Normal;
        }

        public override void Update(Tank[] otherTanks)
        {
            Accelerate(1);
            RotateTowards(Mouse.GetState().Position.ToVector2());
            RotateTurretTowards(Mouse.GetState().Position.ToVector2());
        }
    }
}

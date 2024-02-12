using TANKS_;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TankClient
{
    // The name of the class determines the name of the tank
    internal class Example_Tank : Tank
    {
        protected override void Initalize()
        {
            //This function determines tank settings
            TankColor = TankColor.Copper; //TankColor.Copper, TankColor.Blue, TankColor.Green
            Weapon = Weapon.Cannon; //Weapon.Cannon, Weapon.Double, Weapon.Normal
        }


        protected override void Update(Tank[] otherTanks)
        {
            if(otherTanks.Length != 0)
            {
                Tank target = otherTanks[0];
                RotateTurretTowards(target.Location);
                RotateTowards(target.Location);
                Accelerate(1);
                Fire();
            }
            else
            {
                Shout("Lonely :(");
            }
        }
    }
}

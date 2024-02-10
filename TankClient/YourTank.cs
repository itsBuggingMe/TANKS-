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

            //Note: tank color is cosmetic, but weapon changes the projectile and fire rate
        }


        string LastTankEliminated = "";
        protected override void Update(Tank[] otherTanks)
        {
            //Here is an example tank:
            if(otherTanks.Length == 0)
            {
                if(LastTankEliminated == "")
                {
                    Shout("Elimated" + LastTankEliminated);
                }
                else
                {
                    Shout("No one else here...");
                }
            }
            else
            {
                Tank target = otherTanks[0];

                RotateTurretTowards(target.Location);
                RotateTowards(target.Location);
                Fire();
                Shout("Attacking " + target.Name + " who is a " + target.TankColor.ToString() + " tank.");
                LastTankEliminated = target.Name;
            }
        }
    }
}

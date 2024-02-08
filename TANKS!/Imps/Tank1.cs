using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TANKS_.Imps
{
    internal class Tank1 : Tank
    {
        public Tank1() : base(TankColor.Copper, Weapon.Cannon, "test")
        {

        }

        public override void Update(Tank[] otherTanks)
        {

        }
    }
}

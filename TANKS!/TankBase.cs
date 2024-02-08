using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TANKS_
{
    internal abstract class TankBase
    {
        private Texture2D _base;
        private Texture2D _turret;
        public TankBase(TankColor color, Weapon weapon)
        {
            GameRoot.Instance.Content.Load();

        }
    }
}

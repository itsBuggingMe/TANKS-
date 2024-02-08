using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace TANKS_
{
    internal abstract class Tank
    {
        private readonly Texture2D _base;
        private readonly Texture2D _turret;
        private Vector2 Location;
        public readonly string Name;
        static readonly Point Size = new Point(50);
        public Rectangle GetBounds => RectangleFromCenterSize(Location.ToPoint(), Size);

        public Tank(TankColor color, Weapon weapon, string name)
        {
            Name = name;
            //TODO: remove global var
            _base = GameRoot.Instance.Content.Load<Texture2D>(
                EnumToStringColor(color, GetRandHullNum())
                );

            _turret = GameRoot.Instance.Content.Load<Texture2D>(
                EnumToStringWeapon(color, weapon)
                );
        }

        public abstract void Update(Tank[] otherTanks);

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {

        }

        public static Rectangle RectangleFromCenterSize(Point center, Point size)
        {
            return new Rectangle(center - new Point(size.X >> 1, size.Y >> 1), size);
        }
        #region ContentLoading
        private static string EnumToStringColor(TankColor color, int type) => color switch
        {
            TankColor.Copper => Path.Combine("Hulls_Color_A", $"Hull_0{type}"),
            TankColor.Green => Path.Combine("Hulls_Color_B", $"Hull_0{type}"),
            TankColor.Blue => Path.Combine("Hulls_Color_C", $"Hull_0{type}"),
            _ => throw new NotImplementedException(),
        };

        private static string EnumToStringWeapon(TankColor color, Weapon weapon)
        {
            string ColorString = color switch
            {
                TankColor.Copper => "Weapon_Color_A",
                TankColor.Green => "Weapon_Color_B",
                TankColor.Blue => "Weapon_Color_B",
                _ => throw new NotImplementedException()
            };

            string GunString = weapon switch
            {
                Weapon.Cannon => "Gun_07",
                Weapon.Double => "Gun_06",
                Weapon.Normal => "Gun_01",
                _ => throw new NotImplementedException()
            };

            return Path.Combine(ColorString, GunString);
        }

        private static int GetRandHullNum()
        {
            int rand = Random.Shared.Next(3);

            return rand switch
            {
                0 => 1,
                1 => 2,
                2 => 6,
                _ => throw new NotImplementedException()
            };
        }
        #endregion
    }
}

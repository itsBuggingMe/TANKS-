using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace TANKS_
{
    public abstract class Tank : DynamicHitbox
    {
        #region API
        public float BaseRotation => _Rotation;
        public float TurretRotation => _TurretRotation + _Rotation;

        public TankColor TankColor { get; protected set; } = TankColor.Copper;
        public Weapon Weapon { get; protected set; } = Weapon.Normal;
        public string Name { get; private set; } = "Unnamed";

        public string ShoutText { get; private set; }
        public int Health { get; internal set; } = 100;

        /// <summary>
        /// Moves the tank in the direction it is pointing in
        /// </summary>
        /// <param name="power">How much power to use. -1 for full backwards throttle, 1 for full forwards throttle</param>
        protected void Accelerate(float power) 
        {
            if (DoneActions.HasFlag(Actions.Accelerate))
                return;
            DoneActions |= Actions.Accelerate;

            _accel += Math.Clamp(power, -1, 1) * 0.25f; 
        }

        /// <summary>
        /// Rotates the tank base. The tank rotates relatively slowly.
        /// </summary>
        /// <param name="amount">Amount to rotate. 1 means full speed counterclockwise, -1 means full speed clockwise</param>
        protected void RotateBase(float amount)
        {
            if (DoneActions.HasFlag(Actions.RotateBase))
                return;
            DoneActions |= Actions.RotateBase;

            _Rotation += Math.Clamp(amount, -1, 1) * RotSpeed;
        }

        /// <summary>
        /// Rotates the tank turret. The turrey rotates relatively quickly.
        /// </summary>
        /// <param name="amount">Amount to rotate. 1 means full speed counterclockwise, -1 means full speed clockwise</param>
        protected void RotateTurret(float amount)
        {
            if (DoneActions.HasFlag(Actions.RotateTurr))
                return;
            DoneActions |= Actions.RotateTurr;

            _TurretRotation += Math.Clamp(amount, -1, 1) * RotSpeed * 2;
        }

        /// <summary>
        /// Rotate towards another location as fast as possible
        /// </summary>
        /// <param name="location">The location to point towards</param>
        protected void RotateTowards(Vector2 location)
        {
            //toB - fromA
            Vector2 diff = location - Location;
            if (diff == Vector2.Zero)
                return;
            float amt = MathFunc.GetAngleRad(diff) + MathHelper.PiOver2;
            RotateBase(SmoothRot(amt, BaseRotation));
        }

        /// <summary>
        /// Rotate turret towards another location. Turret rotation is faster than base rotation
        /// </summary>
        /// <param name="location">The location to point towards</param>
        protected void RotateTurretTowards(Vector2 location)
        {
            //toB - fromA
            Vector2 diff = location - Location;
            if (diff == Vector2.Zero)
                return;

            float amt = MathFunc.GetAngleRad(diff) + MathHelper.PiOver2;

            RotateTurret(SmoothRot(amt, TurretRotation));
        }

        /// <summary>
        /// Displays a section of text above the player
        /// </summary>
        /// <param name="text">The text to be displayed</param>
        protected void Shout(string text)
        {
            ShoutText = text;
            shoutTicksLeft = 120;
        }

        /// <summary>
        /// Fires the cannon. Will not do anything if cooldown has not passed
        /// </summary>
        protected void Fire()
        {
            if(FramesSinceFire > (int)Weapon)
            {
                FramesSinceFire = 0;
                TotalProj++;
                WantFire = true;
            }
        }
        #endregion
        private readonly Texture2D _base;
        private readonly Texture2D _turret;

        const float RotSpeed = 0.04f;

        const int size = 60;
        public Rectangle GetBounds { get; private set; }
        private float _accel;

        private float _Rotation;
        private float _TurretRotation;
        private int shoutTicksLeft;

        private static Texture2D TrackA = GameRoot.Instance.Content.Load<Texture2D>(Path.Combine("Tracks", "TrackA"));
        private static Texture2D TrackB = GameRoot.Instance.Content.Load<Texture2D>(Path.Combine("Tracks", "TrackB"));
        private Actions DoneActions = Actions.None;
        private int FramesSinceFire = 0;
        public int TotalProj { get; private set; }

        internal bool WantFire { get; private set; }
        public Tank()
        {
            Initalize();

            Name = GetType().Name.Replace('_', ' ');
            //TODO: remove global var
            _base = GameRoot.Instance.Content.Load<Texture2D>(
                EnumToStringColor(TankColor, GetRandHullNum())
                );

            _turret = GameRoot.Instance.Content.Load<Texture2D>(
                EnumToStringWeapon(TankColor, Weapon)
                );

            OriginTank = _base.Bounds.Size.ToVector2() * new Vector2(0.5f, 0.67f);
            OriginTurret = _turret.Bounds.Size.ToVector2() * new Vector2(0.5f, 0.67f);

            Rectangle b = MathFunc.RectangleFromCenterSize(Point.Zero, _base.Bounds.Size);

            SetModelVerts(new Vector2[] { new(b.Left, b.Top), new(b.Right, b.Top), new(b.Right, b.Bottom), new(b.Left, b.Bottom), new(b.Left, b.Top) }.Select(v => v * 0.2f).ToArray());
        }
        private Vector2 OriginTank;
        private Vector2 OriginTurret;
        private static Vector2 DrawSize = Vector2.One * 0.3f;

        internal void DoUpdate(Tank[] otherTanks)
        {
            DoneActions = Actions.None;
            FramesSinceFire++;
            WantFire = false;
            Update(otherTanks);
            
            if(DoneActions != Actions.None)
            {
                treadSwapScore += 0.8f;
            }

            Vector2 rotatedDir = MathFunc.RotateVectorRad(-Vector2.UnitY, _Rotation);
            Velocity += rotatedDir * _accel;
            Velocity *= 0.95f;
            Location += Velocity;

            _accel = 0;

            if(shoutTicksLeft == 0)
            {
                ShoutText = null;
            }
            shoutTicksLeft--;

            GetBounds = new((int)Location.X - (size >> 1), (int)Location.Y - (size >> 1), size, size);
        }


        protected abstract void Update(Tank[] otherTanks);


        float treadSwapScore = 0;
        internal void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Texture2D thisTrack = (int)treadSwapScore % 10 > 4 ? TrackA : TrackB;
            spriteBatch.Draw(thisTrack, Location, null, Color.White, _Rotation, new Vector2(91, 170), DrawSize, SpriteEffects.None, 0);
            spriteBatch.Draw(thisTrack, Location, null, Color.White, _Rotation, new Vector2(-49,170), DrawSize, SpriteEffects.None, 0);

            spriteBatch.Draw(_base, Location, null, Color.White, _Rotation, OriginTank, DrawSize, SpriteEffects.None, 0);
            spriteBatch.Draw(_turret, Location, null, Color.White, _Rotation + _TurretRotation, OriginTurret, DrawSize, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, 
                MathFunc.CenterStr(Name, Location - Vector2.UnitY * 70, font, out Vector2 newLoc, new Vector2(0.1f)), 
                newLoc, 
                Color.White, 0, Vector2.Zero, 0.1f, SpriteEffects.None, 0);
           
            if(ShoutText is not null)
            {
                spriteBatch.DrawString(font,
                MathFunc.CenterStr(ShoutText, Location - Vector2.UnitY * 100, font, out Vector2 newLoc1, new Vector2(0.1f)),
                newLoc1,
                Color.Black, 0, Vector2.Zero, 0.1f, SpriteEffects.None, 0);
            }

            Rectangle healhBack = MathFunc.RectangleFromCenterSize((Location + Vector2.UnitY * 40).ToPoint(), new Point(70, 4));
            spriteBatch.Draw(GameRoot.Instance.WhitePixel, healhBack, Color.Red);
            healhBack.Width = (int)((Health / 100f) * 70f);
            spriteBatch.Draw(GameRoot.Instance.WhitePixel, healhBack, Color.Green);
        }
        private float SmoothRot(float amt, float curr)
        {
            int dir = MathFunc.RotateDirection(curr, amt);
            float angleDiff = Math.Abs(MathFunc.GetAngleDiff(curr, amt));
            float smoothMutli = 1;
            if (angleDiff < MathFunc.PiOver16)
            {
                smoothMutli = angleDiff / MathFunc.PiOver16;
            }
            return smoothMutli * dir;
        }

        internal void SetLoc(Vector2 location)
        {
            Location = location;
        }

        [Flags]
        internal enum Actions : byte
        {
            None = 0,
            Accelerate = 1 << 1,
            RotateTurr = 1 << 2,
            RotateBase = 1 << 3,
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
                TankColor.Blue => "Weapon_Color_C",
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

        protected abstract void Initalize();
        #endregion
    }
}

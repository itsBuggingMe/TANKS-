using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;

namespace TANKS_
{
    public abstract class Tank
    {
        #region API
        public Vector2 Location => _loc;
        public float Velcity => _vel;
        public float BaseRotation => _Rotation;
        public float TurretRotation => _TurretRotation + _Rotation;

        public TankColor TankColor { get; protected set; } = TankColor.Copper;
        public Weapon Weapon { get; protected set; } = Weapon.Normal;
        public string Name { get; private set; } = "Unnamed";

        public string ShoutText { get; private set; }
        private int shoutTicksLeft;

        /// <summary>
        /// Moves the tank in the direction it is pointing in
        /// </summary>
        /// <param name="power">How much power to use. -1 for full backwards throttle, 1 for full forwards throttle</param>
        protected void Accelerate(float power) 
        {
            if (DoneActions.HasFlag(Actions.Accelerate))
                return;
            DoneActions |= Actions.Accelerate;

            _vel += Math.Clamp(power, -1, 1) * 0.15f; 
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
        #endregion


        private readonly Texture2D _base;
        private readonly Texture2D _turret;

        const float RotSpeed = 0.04f;

        public Rectangle GetBounds => MathFunc.RectangleFromCenterSize(_loc.ToPoint(), new Point(120));
        private Vector2 _loc = Vector2.One * 100;
        private float _vel;

        private float _Rotation;
        private float _TurretRotation;
        private float _turretGlobalRotation => _Rotation + _TurretRotation;

        private static Texture2D TrackA = GameRoot.Instance.Content.Load<Texture2D>(Path.Combine("Tracks", "TrackA"));
        private static Texture2D TrackB = GameRoot.Instance.Content.Load<Texture2D>(Path.Combine("Tracks", "TrackB"));
        private Actions DoneActions = Actions.None;
        private int FramesSinceFire = 0;
        

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
        }
        private Vector2 OriginTank;
        private Vector2 OriginTurret;
        private static Vector2 DrawSize = Vector2.One * 0.5f;

        internal void DoUpdate(Tank[] otherTanks)
        {
            DoneActions = Actions.None;
            FramesSinceFire++;

            Update(otherTanks);
            
            if(DoneActions != Actions.None)
            {
                treadSwapScore += 0.8f;
            }

            Vector2 rotatedDir = MathFunc.RotateVectorRad(-Vector2.UnitY, _Rotation);
            Vector2 prevLoc = _loc;
            _loc += rotatedDir * _vel;

            _vel *= 0.94f;
            if(_vel < 0.01f)
                _vel = 0;

            Rectangle left = GetBounds;

            if (
                left.Left < 0 ||
                left.Top < 0 ||
                left.Right > 960 ||
                left.Bottom > 960)
            {
                _vel *= -1f;
                _loc = prevLoc;
            }

            if(shoutTicksLeft == 0)
            {
                ShoutText = null;
            }
            shoutTicksLeft--;
        }


        internal abstract void Update(Tank[] otherTanks);


        float treadSwapScore = 0;
        protected void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Texture2D thisTrack = (int)treadSwapScore % 10 > 4 ? TrackA : TrackB;
            spriteBatch.Draw(thisTrack, _loc, null, Color.White, _Rotation, new Vector2(91, 170), DrawSize, SpriteEffects.None, 0);
            spriteBatch.Draw(thisTrack, _loc, null, Color.White, _Rotation, new Vector2(-49,170), DrawSize, SpriteEffects.None, 0);

            spriteBatch.Draw(_base, _loc, null, Color.White, _Rotation, OriginTank, DrawSize, SpriteEffects.None, 0);
            spriteBatch.Draw(_turret, _loc, null, Color.White, _turretGlobalRotation, OriginTurret, DrawSize, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, 
                MathFunc.CenterStr(Name, _loc - Vector2.UnitY * 70, font, out Vector2 newLoc, new Vector2(0.2f)), 
                newLoc, 
                Color.White, 0, Vector2.Zero, 0.2f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font,
                MathFunc.CenterStr(ShoutText, _loc - Vector2.UnitY * 120, font, out Vector2 newLoc1, new Vector2(0.15f)),
                newLoc1,
                Color.LightGray, 0, Vector2.Zero, 0.15f, SpriteEffects.None, 0);
        }

        private float SmoothRot(float amt, float curr)
        {
            int dir = MathFunc.RotateDirection(curr, amt);
            float angleDiff = Math.Abs(MathFunc.GetAngleDiff(curr, amt));
            float smoothMutli = 1;
            if (angleDiff < MathFunc.PiOver8)
            {
                smoothMutli = angleDiff / MathFunc.PiOver8;
            }
            return smoothMutli * dir;
        }

        internal void SetLoc(Vector2 location)
        {
            _loc = location;
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace TANKS_
{
    public class Projectile
    {
        public Vector2 Location { get; internal set; }
        public Vector2 Velocity { get; internal set; }

        internal readonly float Diameter;
        internal readonly ProjTypes Damage;
        private Texture2D texture;
        internal readonly float Rotation;

        public enum ProjTypes
        {
            Light_Shell = 10,
            Medium_Shell = 20,
            Heavy_Shell = 40,
        }

        public Projectile(Vector2 Velocity, Vector2 Location, ProjTypes Damage)
        {
            this.Location = Location;
            this.Velocity = Velocity;
            this.Diameter = 20;//bad idea? probs
            this.Damage = Damage;
            Rotation = MathFunc.GetAngleRad(Velocity) + MathHelper.PiOver2;
            texture = GameRoot.Instance.Content.Load<Texture2D>(Path.Combine("Effects", Damage.ToString()));
        }

        internal void Update()
        {
            Location += Velocity;
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Location, null, Color.White, Rotation, texture.Bounds.Size.ToVector2() * 0.5f, 0.7f, SpriteEffects.None, 0);
        }
    }
}

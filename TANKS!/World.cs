using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace TANKS_
{
    internal class World
    {
        private readonly List<Tank> Tanks = new();
        private readonly List<Projectile> Projectiles = new();
        private readonly List<ExplosionAnimation> animations = new();

        private RigidBodySimulation Sim = new RigidBodySimulation();

        public Stack<Tank> tanksToAdd = new();
        
        private Stopwatch tankCalcTimer = new Stopwatch();
        private volatile bool TankRunning;
        private volatile bool TankBad;
        private Rectangle Bounds = new Rectangle(0,0,1280,1280);

        public World()
        {
            Sim.AddBody(new Obstacle(Bounds));
        }

        public void AddTank(Tank tank)
        {
            lock(tanksToAdd)
            {
                tanksToAdd.Push(tank);
            }
        }

        public void Tick()
        {
            AddTank();
            
            UpdateTank();
            
            UpdateProj();

            Sim.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                Tanks.Clear();
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Projectiles.ForEach(p => p.Draw(spriteBatch));
            Tanks.ForEach(t => t.Draw(spriteBatch, font));
            for(int i = animations.Count - 1; i >= 0; i--)
            {
                if (animations[i].Frame < 0)
                {
                    animations.RemoveAt(i);
                }
                else
                {
                    animations[i].Animate(spriteBatch);
                }
            }
            /*
            foreach(var dynh in Sim.DynamicBodies)
            {
                foreach(var v in dynh.Verts)
                {
                    spriteBatch.Draw(GameRoot.Instance.WhitePixel, new Rectangle(v.ToPoint(), new Point(4)), Color.White);
                }
            }*/
        }

        void AddTank()
        {
            lock (tanksToAdd)
            {
                while (tanksToAdd.Count != 0)
                {
                    Tank tank = tanksToAdd.Pop();
                    Vector2 loc = Vector2.Zero;
                    while (true)
                    {
                        loc = MathFunc.PointInRectangle(Bounds, 100);
                        if (!Tanks.Any(t => Vector2.DistanceSquared(loc, t.Location) < 10_000))
                            break;
                    }
                    tank.SetLoc(loc);
                    Tanks.Add(tank);
                    Sim.AddBody(tank);
                }
            }
        }

        void UpdateTank()
        {
            for (int i = Tanks.Count - 1; i >= 0; i--)
            {
                Tank tank = Tanks[i];
                if(tank.Health <= 0)
                {
                    Tanks.RemoveAt(i);
                    tank.Delete = true;
                    continue;
                }

                tankCalcTimer.Reset();
                tankCalcTimer.Start();
                //copies list so tanks wont effect each other
                TankRunning = true;
                TankBad = false;
                Task.Run(() =>
                {
                    //dont crash if tank crashes
                    try
                    {
                        tank.DoUpdate(Tanks.Where(t => t != tank).ToArray());
                    }
                    catch (Exception e)
                    {
                        GameRoot.Instance.Write($"{tank.Name} exploded with exception: {e.Message}");
                        TankBad = true;
                    }
                    TankRunning = false;
                });

                //if tank too long kick it
                while (TankRunning)
                {
                    if (tankCalcTimer.ElapsedMilliseconds > 10)
                    {
                        TankBad = true;
                        GameRoot.Instance.Write($"{tank.Name} eliminated for taking too long");
                        break;
                    }
                }



                if (tank.WantFire)
                {
                    Projectiles.Add(new Projectile(MathFunc.RotateVectorRad(Vector2.UnitX, tank.TurretRotation - MathHelper.PiOver2) * 32, tank.Location, get(tank.Weapon), tank));
                }

                static Projectile.ProjTypes get(Weapon weapon) => weapon switch
                {
                    Weapon.Cannon => Projectile.ProjTypes.Heavy_Shell,
                    Weapon.Double => Projectile.ProjTypes.Light_Shell,
                    Weapon.Normal => Projectile.ProjTypes.Medium_Shell,
                    _ => throw new NotImplementedException()
                };

                if(TankBad)
                {
                    Tanks[i].Delete = true;
                    Tanks.RemoveAt(i);
                }
            }
        }

        void UpdateProj()
        {
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                var p = Projectiles[i];
                p.Update();

                if (Math.Abs(p.Location.X) + Math.Abs(p.Location.Y) > 2000)
                {
                    Projectiles.RemoveAt(i);
                    continue;
                }

                foreach (var t in Tanks)
                {
                    if (t == p.Parent)
                        continue;

                    if (Vector2.Distance(p.Location, t.Location) < p.Radius)
                    {
                        t.Health -= (int)p.Damage;
                        Projectiles.RemoveAt(i);
                        animations.Add(new ExplosionAnimation(p.Location));
                        break;
                    }
                }
            }
        }

        internal class ExplosionAnimation
        {
            public int Frame;
            public Vector2 Location;
            private static Texture2D[] frames = GetFrames();
            const int totalFrames = ('H' - 'A' - 1);
            public ExplosionAnimation(Vector2 location)
            {
                Frame = ('H' - 'A' - 1) << 1;
                Location = location;
            }
            static Vector2 center = new Vector2(128);
            public void Animate(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(frames[totalFrames - (Frame >> 1)], Location, null, Color.White, 0, center, 0.2f, SpriteEffects.None, 0);
                Frame--;
            }

            private static Texture2D[] GetFrames()
            {
                Texture2D[] texture2Ds = new Texture2D['H' - 'A'];
                for(int i = 'A'; i < 'H'; i++)
                {
                    texture2Ds[i - 'A'] = GameRoot.Instance.Content.Load<Texture2D>(Path.Combine("Effects", $"Explosion_{(char)i}"));
                }

                return texture2Ds;
            }
        }
    }
}

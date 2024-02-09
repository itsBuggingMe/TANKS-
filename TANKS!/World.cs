using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace TANKS_
{
    internal class World
    {
        private readonly List<Tank> Tanks = new();
        private readonly List<Projectile> Projectiles = new();

        public Stack<Tank> tanksToAdd = new();

        public Action<Tank, Exception> TankException => delegate {};
        private Stopwatch tankCalcTimer = new Stopwatch();
        private volatile bool TankRunning;
        private Rectangle Bounds = new Rectangle(0,0,960,960);

        public void AddTank(Tank tank)
        {
            lock(tanksToAdd)
            {
                tanksToAdd.Push(tank);
            }
        }

        public void Tick()
        {
            lock(tanksToAdd)
            {
                while(tanksToAdd.Count != 0)
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
                }
            }

            for(int i = Tanks.Count - 1; i >= 0; i--)
            {
                Tank tank = Tanks[i];
                tankCalcTimer.Reset();
                tankCalcTimer.Start();
                //copies list so tanks wont effect each other
                TankRunning = true;
                Task.Run(() =>
                {
                    //dont crash if tank crashes
                    try
                    {
                        tank.DoUpdate(Tanks.Where(t => t != tank).ToArray());
                    }
                    catch (Exception e)
                    {
                        TankException(tank, e);
                    }
                    TankRunning = false;
                });

                //if tank too long kick it
                while(TankRunning)
                {
                    if(tankCalcTimer.ElapsedMilliseconds > 20000)
                    {
                        Tanks.RemoveAt(i);
                        TankException(tank, new TimeoutException("Tank took too long"));
                        break;
                    }
                }

                if(tank.WantFire)
                {
                    Projectiles.Add(new Projectile(MathFunc.RotateVectorRad(Vector2.UnitX, tank.TurretRotation - MathHelper.PiOver2) * 32, tank.Location, get(tank.Weapon)));
                }

                static Projectile.ProjTypes get(Weapon weapon) => weapon switch
                {
                    Weapon.Cannon => Projectile.ProjTypes.Heavy_Shell,
                    Weapon.Double => Projectile.ProjTypes.Light_Shell,
                    Weapon.Normal => Projectile.ProjTypes.Medium_Shell,
                    _ => throw new NotImplementedException()
                };
            }

            Projectiles.ForEach(p => p.Update());

            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                Tanks.Clear();
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Projectiles.ForEach(p => p.Draw(spriteBatch));
            Tanks.ForEach(t => t.Draw(spriteBatch, font));
        }
    }
}

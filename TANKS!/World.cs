using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TANKS_
{
    internal class World
    {
        private readonly List<Tank> Tanks = new();
        
        public Action<Tank, Exception> TankException;
        private Stopwatch tankCalcTimer = new Stopwatch();
        private volatile bool TankRunning;

        public void AddTank()
        {

        }

        public void Tick()
        {
            for(int i = Tanks.Count - 1; i >= 0; i--)
            {
                Tank tank = Tanks[i];
                tankCalcTimer.Reset();

                //copies list so tanks wont effect each other
                TankRunning = true;
                Task.Run(() =>
                {
                    //dont crash if tank crashes
                    try
                    {
                        tank.Update(Tanks.ToArray());
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
                    if(tankCalcTimer.ElapsedMilliseconds > 1)
                    {
                        Tanks.RemoveAt(i);
                        TankException(tank, new TimeoutException("Tank took too long"));
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Tanks.ForEach(t => t.Draw(spriteBatch, font));
        }
    }
}

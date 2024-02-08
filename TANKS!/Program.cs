using TANKS_;

string path = @"C:\Users\Jason\Downloads\test.dll";
string code = @"using Microsoft.Xna.Framework.Graphics;

namespace TANKS_
{
    internal abstract class TankBase
    {
        private Texture2D _base;
        private Texture2D _turret;
        public TankBase(TankColor color, Weapon weapon)
        {
           //GameRoot.Instance.Content.Load();

        }
    }
}";

Compiler.TryCompileCode(code, path);

using var game = new TANKS_.GameRoot();
game.Run();

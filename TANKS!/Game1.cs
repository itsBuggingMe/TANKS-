using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TANKS_
{
    public class GameRoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static GameRoot Instance { get; private set; }
        private List<string> Console = new();
        public void WriteConsole(string s) => Console.Add(s);

        private World world = new World();

        private SpriteFont font;
        private RenderTarget2D Arena;

        static string thisFolder = Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).FullName;
        string[] AllowedNamespaces = new string[]
            {
                "System.Linq",
                "System",
                "System.Collections",
                "netstandard",
            };
        string[] AllowedAssemblies = new string[]
            {
                System.Reflection.Assembly.GetEntryAssembly().Location,
                Path.Combine(thisFolder, "DLLS", "MonoGame.Framework.dll"),
            };

        private RoslynCompiler Compiler;

        public GameRoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Instance = this;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Arena = new RenderTarget2D(_graphics.GraphicsDevice, 960, 960);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");


            Compiler = new RoslynCompiler(AllowedNamespaces, AllowedAssemblies);
        }

        private int ticks = 60;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(ticks++ % 120 == 0)
            {
                CheckTank();
            }

            world.Tick();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(Arena);
            GraphicsDevice.Clear(Color.SandyBrown);
            _spriteBatch.Begin();
            world.Draw(_spriteBatch, font);
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin();
            _spriteBatch.Draw(Arena, Vector2.Zero, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public const string EndPoint = "tanks";
        private HashSet<string> CompiledIds = new HashSet<string>();
        private void CheckTank()
        {
            ApiGetter.GetGenericString(s =>
            {
                string[] code = JsonConvert.DeserializeObject<string[]>(s);
                if(code == null || code.Length == 0)
                    return;

                ApiGetter.DeleteMap(EndPoint);

                for(int i = 0; i < code.Length; i++)
                {
                    string id = code[i][..20];
                    if(CompiledIds.Contains(id))
                    {
                        continue;
                    }

                    CompiledIds.Add(id);

                    string realCode = Encoding.UTF8.GetString(Convert.FromBase64String(code[i][21..]));
                    var newasm = Compiler.Compile(realCode);
                    
                    foreach(var type in newasm.GetTypes())
                    {
                        if(type.IsSubclassOf(typeof(Tank)))
                        {
                            object thing = Activator.CreateInstance(type);
                            world.AddTank((Tank)thing);
                            break;
                        }
                    }
                }
            }, EndPoint);
        }
    }
}

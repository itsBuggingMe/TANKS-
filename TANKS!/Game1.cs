using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using YourTank;
using static System.Net.Mime.MediaTypeNames;

namespace TANKS_
{
    public class GameRoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static GameRoot Instance { get; private set; }
        private List<WrappedText> Console = new();

        private World world;

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

        public Texture2D WhitePixel;
        public GameRoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Instance = this;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += ReWrapAll;
        }

        protected override void Initialize()
        {
            base.Initialize();

            WhitePixel = new Texture2D(GraphicsDevice, 1,1);
            WhitePixel.SetData(new Color[] { Color.White });
            world = new World();

            Arena = new RenderTarget2D(_graphics.GraphicsDevice, 1280, 1280);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");

            world.AddTank(new ControlTank());
            Compiler = new RoslynCompiler(AllowedNamespaces, AllowedAssemblies);
        }

        private int ticks = 60;

        KeyboardState KeyboardState;
        KeyboardState PrevKeyboardState;

        protected override void Update(GameTime gameTime)
        {
            PrevKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if(KeyboardState.IsKeyDown(Keys.F11) && !PrevKeyboardState.IsKeyDown(Keys.F11))
            {
                ToggleBorderless();
            }

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
            GraphicsDevice.Clear(Color.DarkOliveGreen);
            _spriteBatch.Begin();
            world.Draw(_spriteBatch, font);
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin();
            _spriteBatch.Draw(Arena, new Rectangle(Point.Zero, new(Window.ClientBounds.Height)), Color.White);
            Vector2 Start = new Vector2(Window.ClientBounds.Height + 24, 24);
            lock(Console)
            {
                for (int i = 0; i < Console.Count; i++)
                {
                    _spriteBatch.DrawString(font, Console[i].text, Start, Color.White, 0, Vector2.Zero, FontSize, SpriteEffects.None, 0);
                    Start += Vector2.UnitY * Console[i].height;
                }
                _spriteBatch.End();
            }
            if (Start.Y > Window.ClientBounds.Height && Console.Count > 0)
            {
                Console.RemoveAt(0);
            }


            base.Draw(gameTime);
        }

        private void ToggleBorderless()
        {
            if (Window.IsBorderless)
            {
                Window.IsBorderless = false;

                Window.Position = (new Point(_graphics.GraphicsDevice.DisplayMode.Width, _graphics.GraphicsDevice.DisplayMode.Height).ToVector2() * 0.5f - new Vector2(400, 240)).ToPoint();

                _graphics.PreferredBackBufferWidth = 800;
                _graphics.PreferredBackBufferHeight = 480;


                _graphics.ApplyChanges();
            }
            else
            {
                Window.IsBorderless = true;
                _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
                _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height;
                Window.Position = Point.Zero;

                _graphics.ApplyChanges();
            }
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
                    try
                    {
                        if(realCode.Contains("Enviroment.Exit"))
                        {
                            throw new Exception("Nice try");
                        }

                        var newasm = Compiler.Compile(realCode);

                        foreach (var type in newasm.GetTypes())
                        {
                            if (type.IsSubclassOf(typeof(Tank)))
                            {
                                Tank thing = Activator.CreateInstance(type) as Tank;
                                Write($"{thing.Name} Joined the Game!");
                                world.AddTank(thing);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Write($"Tank {id[4..8]} Compilation error: {e.Message}");
                    }
                }
            }, EndPoint);
        }
        const float FontSize = 0.3f;

        public void Write(object obj)
        {
            if(obj is not null)
            {
                lock(Console)
                {
                    Console.Add(WrapText(font, obj.ToString(), Window.ClientBounds.Width - Window.ClientBounds.Height - 24));
                }
            }
        }

        private void ReWrapAll(object Sender, EventArgs args)
        {
            lock (Console)
            {
                for (int i = 0; i < Console.Count; i++)
                {
                    Console[i] = WrapText(font, null, Window.ClientBounds.Width - Window.ClientBounds.Height, Console[i]);
                }
            }
        }

        private static WrappedText WrapText(SpriteFont spriteFont, string text, float maxLineWidth, WrappedText? old = null)
        {
            if(old.HasValue)
            {
                text = old.Value.originalText;
            }
            string[] words = text.Replace("  ", "").Replace('\n', ' ').Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            Vector2 charSize = spriteFont.MeasureString(" ") * FontSize;
            float spaceWidth = charSize.X;

            int iterations = 1;
            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word) * FontSize;

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    iterations++;
                    lineWidth = size.X + spaceWidth;
                }
            }

            return new WrappedText(text, sb.ToString(), iterations * charSize.Y);
        }

        private record struct WrappedText(string originalText, string text, float height);
    }
}

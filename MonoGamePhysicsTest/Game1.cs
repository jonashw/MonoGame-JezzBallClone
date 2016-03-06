using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGamePhysicsTest.Input;
using MonoGamePhysicsTest.Physics;

namespace MonoGamePhysicsTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Map _map;
        private Ball[] _balls;
        private readonly MouseEvents _mouse;
        private Cursor _cursor;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };
            _mouse = new MouseEvents(Window);
            IsMouseVisible = false;
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _mouse.OnLeftClick((x,y) => _cursor.StartDivider());
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _map = MapFactory.BasicRectangle(Content);
            _cursor = new Cursor(_map, new CursorTextures(
                Content.Load<Texture2D>("Cursor-Up"),
                Content.Load<Texture2D>("Cursor-Down"),
                Content.Load<Texture2D>("Cursor-Left"),
                Content.Load<Texture2D>("Cursor-Right"),
                Content.Load<Texture2D>("Cursor-Horizontal"),
                Content.Load<Texture2D>("Cursor-Vertical"),
                Content.Load<Texture2D>("Cursor-Disabled")));

            _mouse.OnRightClick((x,y) => _cursor.ToggleAxis());

            var ballTexture = new
            {
                Green = Content.Load<Texture2D>("Ball-Green"),
                Red = Content.Load<Texture2D>("Ball-Red"),
                Blue = Content.Load<Texture2D>("Ball-Blue"),
                Yellow = Content.Load<Texture2D>("Ball-Yellow")
            };
            _balls = new[]
            {
                new
                {
                    Position = new Vector2(30, 5),
                    Velocity = new Vector2(-0.25f, 0.6f),
                    Texture = ballTexture.Green
                },
                new
                {
                    Position = new Vector2(5, 5),
                    Velocity = new Vector2(0.1f, 0.4f),
                    Texture = ballTexture.Blue
                },
                new
                {
                    Position = new Vector2(20, 10),
                    Velocity = new Vector2(0.6f, -0.1f),
                    Texture = ballTexture.Yellow
                },
                new
                {
                    Position = new Vector2(50, 50),
                    Velocity = new Vector2(0.5f, -0.5f),
                    Texture = ballTexture.Red
                }
            }.Select(b =>
                new Ball(
                    b.Texture,
                    new BallPhysics(
                        _map,
                        b.Position*_map.TileSize,
                        b.Velocity * _map.TileSize,
                        b.Texture.Width,
                        b.Texture.Height))).ToArray();
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _cursor.Update(gameTime);
            foreach (var ball in _balls)
            {
                ball.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private readonly Color _backgroundColor = new Color(100,100,100);
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);
            _mouse.Update(gameTime);

            _spriteBatch.Begin();
            _map.Draw(_spriteBatch);
            _cursor.Draw(_spriteBatch);
            foreach (var ball in _balls)
            {
                ball.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

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
        private readonly KeyboardEvents _keyboard = new KeyboardEvents();
        private Cursor _cursor;
        private StringDrawer _stringDrawer;
        private readonly ILogger _logger = new DebugLogger();
        private EdgeTracer _edgeTracer = null;
        private Texture2D _tracterTexture;

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
            _keyboard.OnPress(Keys.Space, () => _playing = !_playing);
            base.Initialize();
        }

        private bool _playing = true;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _map = MapFactory.BasicRectangle(Content);
            _stringDrawer = new StringDrawer(Content.Load<SpriteFont>("Consolas"));
            _cursor = new Cursor(_map, new CursorTextures(
                Content.Load<Texture2D>("Cursor-Up"),
                Content.Load<Texture2D>("Cursor-Down"),
                Content.Load<Texture2D>("Cursor-Left"),
                Content.Load<Texture2D>("Cursor-Right"),
                Content.Load<Texture2D>("Cursor-Horizontal"),
                Content.Load<Texture2D>("Cursor-Vertical"),
                Content.Load<Texture2D>("Cursor-Disabled")),
                Content.Load<Texture2D>("Tile-GrayBlack"),
                _stringDrawer);

            _tracterTexture = Content.Load<Texture2D>("Tile-GrayBlack");
            _mouse.OnMiddleClick((x, y) =>
            {
                var maybeSlot = TileSlot.TryGetForPosition(x, y);
                if (!maybeSlot.HasValue)
                {
                    return;
                }
                _edgeTracer = new EdgeTracer(_map, maybeSlot.Value, _logger, _tracterTexture);
                _edgeTracer.Start();
            });

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

            _keyboard.Update(Keyboard.GetState());
            _mouse.Update(gameTime);

            if (!_playing)
            {
                return;
            }

            _cursor.Update(gameTime);
            foreach (var ball in _balls)
            {
                ball.Update(gameTime);
            }

            if (_edgeTracer != null)
            {
                _edgeTracer.Update();
                if (_edgeTracer.State == EdgeTracer.TracerState.Finished)
                {
                    _edgeTracer = null;
                }
            }

            base.Update(gameTime);
        }

        private readonly Color _backgroundColor = new Color(100,100,100);
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            _spriteBatch.Begin();


            _map.Draw(_spriteBatch);

            if (_edgeTracer != null)
            {
                _edgeTracer.Draw(_spriteBatch);
                _stringDrawer.Draw(_spriteBatch,
                    string.Format("Edge Tracer{{Starting Slot: ({0},{1}), Last Slot: ({2},{3})}}",
                        _edgeTracer.FirstEdgeSlot.ColumnIndex,
                        _edgeTracer.FirstEdgeSlot.RowIndex,
                        _edgeTracer.PreviousSlot.ColumnIndex,
                        _edgeTracer.PreviousSlot.RowIndex),
                    TileSlot.BottomRightPosition - new Vector2(520, 30));
            }
            _cursor.Draw(_spriteBatch);

            foreach (var ball in _balls)
            {
                ball.Draw(_spriteBatch);
            }
            _stringDrawer.Draw(_spriteBatch, "Percent Filled: " + _map.GetFillPercent().ToString("F2"), TileSlot.BottomLeftPosition + new Vector2(20,-20));

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

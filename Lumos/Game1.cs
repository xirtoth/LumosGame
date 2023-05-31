using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumos
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private SpriteFont _myFont;
        private Map _map;
        private Vector2 _cameraPosition;
        public int _renderAreaWidth = 1000;
        public int _renderAreaHeight = 10000;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _player = new Player(new Vector2(10 * 16, 19 * 16), "Henkka", Content.Load<Texture2D>("player"));
            _myFont = Content.Load<SpriteFont>("MyFont");
            TileTextures.WaterTexture = Content.Load<Texture2D>("water");
            TileTextures.DirtTexture = Content.Load<Texture2D>("dirt");
            TileTextures.DirtTopTexture = Content.Load<Texture2D>("dirttop");
            TileTextures.EmptyTexture = Content.Load<Texture2D>("empty");
            TileTextures.GrassTop = Content.Load<Texture2D>("grasstop");
            _map = new Map(10000, 500, Content.Load<Texture2D>("player"));
            _cameraPosition = _player.Pos;
            _map.GenerateMap();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                MouseState mouseState = Mouse.GetState();

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    // Get the position of the mouse click in the game world coordinates
                    Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y) + _cameraPosition;
                    // Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(_cameraPosition));

                    // Calculate the tile coordinates based on the mouse position
                    int tileX = (int)(mousePosition.X / 16);
                    int tileY = (int)(mousePosition.Y / 16);

                    // Check if the tile coordinates are within the map bounds
                    if (tileX >= 0 && tileX < _map.Width && tileY >= 0 && tileY < _map.Height)
                    {
                        // Change the tile at the clicked position
                        _map.MapData[tileX, tileY] = MapTiles.empty;
                    }
                }
                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    // Get the position of the mouse click in the game world coordinates
                    Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y) + _cameraPosition;
                    // Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(_cameraPosition));

                    // Calculate the tile coordinates based on the mouse position
                    int tileX = (int)(mousePosition.X / 16);
                    int tileY = (int)(mousePosition.Y / 16);

                    // Check if the tile coordinates are within the map bounds
                    if (tileX >= 0 && tileX < _map.Width && tileY >= 0 && tileY < _map.Height)
                    {
                        // Change the tile at the clicked position
                        _map.MapData[tileX, tileY] = _player.selectedTile;
                    }
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                //_player.Update(gameTime);
                _cameraPosition = _player.Pos - new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                // _map.Update(_player);
                _player.Update(gameTime);

                // TODO: Add your update logic here
                _map.Update();
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            _map.DrawMap(_spriteBatch, _player, _cameraPosition, GraphicsDevice.Viewport, GraphicsDevice, gameTime);
            _player.Draw(_spriteBatch, _cameraPosition, GraphicsDevice.Viewport);
            _spriteBatch.DrawString(_myFont, _player.Name + " " + _player.Pos.X + " " + _player.Pos.Y, new Vector2(-2, -2), Color.Black);
            _spriteBatch.DrawString(_myFont, _player.Name + " " + _player.PreviousPos.X + " " + _player.PreviousPos.Y, new Vector2(-2, 10), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
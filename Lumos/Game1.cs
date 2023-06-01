using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Lumos
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private Player _player;
        public SpriteFont _myFont;
        private Map _map;
        public Vector2 _cameraPosition;
        private MouseState _previousMouseState;
        private int MapSizeX = 2000;
        private int MapSizeY = 100;
        public int _renderAreaWidth = 1000;
        public int _renderAreaHeight = 10000;
        private float EdgePanSpeed = 2f;
        private int _frameCount;
        private float _elapsedTime;
        private Texture2D _uiRectangleTexture;
        private Texture2D _bg;
        private Texture2D _enemyTex;
        private List<Rectangle> _toolRectangles;
        public List<DamageMessage> _damageMessageList;
        private List<Enemy> _enemies;
        private float _currentTime = 0f;
        private float _lerpDuration = 100f; // Duration in seconds for each color transition
        private Color _startColor = Color.Black;
        private Color _endColor = Color.LightGoldenrodYellow;
        private bool _reverseColors = false;
        private Color _lerpedColor;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = false;
            //_graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.ApplyChanges();// TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _damageMessageList = new List<DamageMessage>();
            _enemies = new List<Enemy>();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _player = new Player(new Vector2(10 * 16, 19 * 16), "Henkka", Content.Load<Texture2D>("player"));
            _myFont = Content.Load<SpriteFont>("MyFont");
            TileTextures.WaterTexture = Content.Load<Texture2D>("water");
            TileTextures.DirtTexture = Content.Load<Texture2D>("dirt");
            TileTextures.DirtTopTexture = Content.Load<Texture2D>("dirttop");
            TileTextures.EmptyTexture = Content.Load<Texture2D>("empty");
            TileTextures.GrassTop = Content.Load<Texture2D>("grasstop");
            TileTextures.MyFont = Content.Load<SpriteFont>("MyFont");
            _enemyTex = Content.Load<Texture2D>("arrow");
            _bg = Content.Load<Texture2D>("bgmountain");
            _map = new Map(MapSizeX, MapSizeY, Content.Load<Texture2D>("player"));
            _cameraPosition = _player.Pos;
            _map.GenerateMap();
            _toolRectangles = GenerateRectangles(10, 50, 50, 20);
            _damageMessageList.Add(new DamageMessage("testi", 2f, new Vector2(10, 10), this));
            _damageMessageList.Add(new DamageMessage("testi2", 2f, new Vector2(20, 20), this));
            for (int i = 0; i < 10; i++)
            {
                _enemies.Add(new Enemy(_enemyTex, new Vector2(20 * i, 20 * i)));
            }

            // TODO: use this.Content to load your game content here
        }

        private List<Rectangle> GenerateRectangles(int count, int width, int height, int spacing)
        {
            List<Rectangle> rectangles = new List<Rectangle>();
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            int totalWidth = (count * width) + ((count - 1) * spacing);
            int startX = (screenWidth - totalWidth) / 2;

            for (int i = 0; i < count; i++)
            {
                int x = startX + (i * (width + spacing));
                int y = screenHeight - height - spacing; // Adjust the position as needed

                Rectangle rectangle = new Rectangle(x, y, width, height);
                rectangles.Add(rectangle);
            }

            return rectangles;
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateTime(gameTime);
                MouseState mouseState = CheckMouseInput();
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                //_player.Update(gameTime);
                //_cameraPosition = _player.Pos - new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                _player.Update(gameTime);

                // TODO: Add your update logic here

                _map.Update();
                _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _frameCount++;

                if (_elapsedTime >= 1.0f)
                {
                    Window.Title = "FPS: " + _frameCount;
                    _frameCount = 0;
                    _elapsedTime = 0.0f;
                }
                List<DamageMessage> damageMessagesCopy = new List<DamageMessage>(_damageMessageList);
                foreach (DamageMessage dm in damageMessagesCopy)
                {
                    dm.Update(gameTime, this);
                }
                foreach (Enemy e in _enemies)
                {
                    e.Update(gameTime, _cameraPosition);
                }

                base.Update(gameTime);
            }
        }

        private MouseState CheckMouseInput()
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Get the position of the mouse click in the game world coordinates
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                // Adjust the mouse position by the camera's position
                mousePosition += _cameraPosition;

                foreach (Enemy e in _enemies)
                {
                    int tileXX = (int)(mousePosition.X / 16);
                    int tileYY = (int)(mousePosition.Y / 16);
                    // Transform the mouse position to be relative to the camera
                    Point mousePoint = new Point((int)(mousePosition.X - _cameraPosition.X), (int)(mousePosition.Y - _cameraPosition.Y));

                    if (e.boundingBox.Contains(mousePoint))
                    {
                        e.TakeDamage(40, this);
                    }
                }
                // Calculate the tile coordinates based on the mouse position
                int tileX = (int)(mousePosition.X / 16);
                int tileY = (int)(mousePosition.Y / 16);

                // Check if the tile coordinates are within the map bounds
                if (tileX >= 0 && tileX < _map.Width && tileY >= 0 && tileY < _map.Height && _map.MapData[tileX, tileY].MapTile != MapTiles.empty)
                {
                    // Change the tile at the clicked position

                    _map.MapData[tileX, tileY] = new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false);
                    _damageMessageList.Add(new DamageMessage("testi", 2f, mousePosition + new Vector2(20, -20), this));
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
                    _map.MapData[tileX, tileY] = new Tile(MapTiles.dirt, TileTextures.DirtTexture, false, false);
                }
            }
            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                // Handle middle mouse button drag
                Vector2 mouseDelta = new Vector2(mouseState.X - _previousMouseState.X, mouseState.Y - _previousMouseState.Y);
                _cameraPosition -= mouseDelta;
            }
            // _cameraPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2) - mouseDelta);
            _previousMouseState = mouseState;
            const int edgeThreshold = 20; // Adjust the edge threshold as needed
            const float panSpeed = 5.0f; // Adjust the pan speed as needed
            Vector2 panDirection = Vector2.Zero;

            if (mouseState.X < edgeThreshold)
                panDirection.X = -EdgePanSpeed;
            else if (mouseState.X > GraphicsDevice.Viewport.Width - edgeThreshold)
                panDirection.X = EdgePanSpeed;

            if (mouseState.Y < edgeThreshold)
                panDirection.Y = -EdgePanSpeed;
            else if (mouseState.Y > GraphicsDevice.Viewport.Height - edgeThreshold)
                panDirection.Y = EdgePanSpeed;

            _cameraPosition += panDirection * panSpeed;
            return mouseState;
        }

        private void UpdateTime(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Increment the current time
            _currentTime += deltaTime;

            // Check if the current time has reached the lerp duration
            if (_currentTime >= _lerpDuration)
            {
                // Reverse the colors
                _reverseColors = !_reverseColors;
                _currentTime = 0f; // Reset the current time
            }

            // Calculate the normalized lerp value
            float lerpValue = MathHelper.Clamp(_currentTime / _lerpDuration, 0f, 1f);

            // Lerp between the start color and end color based on the lerp value

            if (_reverseColors)
            {
                _lerpedColor = Color.Lerp(_endColor, _startColor, lerpValue);
            }
            else
            {
                _lerpedColor = Color.Lerp(_startColor, _endColor, lerpValue);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_lerpedColor);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_bg, new Vector2(0, -1000), Color.White);
            foreach (Enemy e in _enemies)
            {
                e.Draw(_spriteBatch, _cameraPosition);
            }
            _map.DrawMap(_spriteBatch, _player, _cameraPosition, GraphicsDevice.Viewport, GraphicsDevice, gameTime);
            _player.Draw(_spriteBatch, _cameraPosition, GraphicsDevice.Viewport);
            _spriteBatch.DrawString(_myFont, _player.Name + " " + _player.Pos.X + " " + _player.Pos.Y, new Vector2(-2, -2), Color.Black);
            _spriteBatch.DrawString(_myFont, _player.Name + " " + _player.PreviousPos.X + " " + _player.PreviousPos.Y, new Vector2(-2, 10), Color.Black);
            foreach (Rectangle toolrectangle in _toolRectangles)
            {
                _spriteBatch.Draw(TileTextures.DirtTexture, toolrectangle, Color.Red);
            }
            foreach (DamageMessage dm in _damageMessageList)
            {
                dm.Draw(this);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
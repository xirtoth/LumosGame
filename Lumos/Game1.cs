using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Penumbra;
using Lumos.Entities;

namespace Lumos
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public Player _player;
        public SpriteFont _myFont;
        private readonly Random rand = new Random();
        public Map _map;
        public Vector2 _cameraPosition;
        private MouseState _previousMouseState;
        private int MapSizeX = 500;
        private int MapSizeY = 500;
        public int _renderAreaWidth = 1000;
        public int _renderAreaHeight = 10000;
        private float EdgePanSpeed = 2f;
        private int _frameCount;

        private int maxEnemies = 10;
        private int maxFlowers = 40;
        private float _elapsedTime;

        private Texture2D _bg;
        private Texture2D _enemyTex;
        private Texture2D _arrow;
        private List<Rectangle> _toolRectangles;
        public List<DamageMessage> _damageMessageList;
        public List<Enemy> _enemies;
        public List<Item> _items;
        public List<Projectile> _projectiles;
        public List<FlowerEntity> _flowerEntities;
        private float _currentTime = 0f;
        private float _lerpDuration = 100f; // Duration in seconds for each color transition
        private Color _startColor = Color.Black;
        private Color _endColor = Color.LightGoldenrodYellow;
        private bool _reverseColors = false;
        private Color _lerpedColor;
        private bool _wasMousePressed = false;
        private Effect _effect;
        private Effect _flashEffect;
        private RenderTarget2D lightingRenderTarget;
        public PenumbraComponent penumbra;
        public RainSystem rainsystem;

        private float testProjectileTimer = 0f;
        private float testProjectileTreshold = 0.0001f;

        private string mouseOverText;

        public Light light = new PointLight
        {
            Scale = new Vector2(1000f), // Range of the light source (how far the light will travel)
            ShadowType = ShadowType.Solid // Will not lit hulls themselves
        };

        public Light light2 = new PointLight
        {
            Scale = new Vector2(400f),
            ShadowType = ShadowType.Solid
        };

        public static Game1 Instance { get; set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            penumbra = new PenumbraComponent(this);
            Components.Add(penumbra);
            //penumbra.Lights.Add(light);
            penumbra.Lights.Add(light2);
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //_graphics.PreferredBackBufferWidth = 800;
            //_graphics.PreferredBackBufferHeight = 600;
            _graphics.IsFullScreen = false;
            _graphics.SynchronizeWithVerticalRetrace = true;

            // TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 700); // Sets the frame rate to 120 FPS
            IsFixedTimeStep = true;

            _graphics.ApplyChanges();// TODO: Add your initialization logic here
            Instance = this;
            base.Initialize();
            light.Color = Color.DarkGreen;
            light.Position = new Vector2(500, 500);
            light.Intensity = 2;
            light2.Color = Color.Purple;
            light2.Intensity = 3;
        }

        protected override void LoadContent()
        {
            TileTextures.LoadContent(Content);
            _flowerEntities = new List<FlowerEntity>();
            rainsystem = new RainSystem();
            rainsystem.AddRaindrops(5000);
            _damageMessageList = new List<DamageMessage>();
            _enemies = new List<Enemy>();
            _items = new List<Item>();
            _projectiles = new List<Projectile>();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _player = new Player(new Vector2(500, 300), "Henkka", Content.Load<Texture2D>("player_00"));
            _myFont = Content.Load<SpriteFont>("MyFont");

            // Effect shaderEffect = Content.Load<Effect>("black");
            // TorchShader torchShader = new TorchShader(shaderEffect);
            _effect = Content.Load<Effect>("black");
            _flashEffect = Content.Load<Effect>("flasheffect");
            lightingRenderTarget = new RenderTarget2D(GraphicsDevice, 64, 64);
            _arrow = Content.Load<Texture2D>("arrow");
            _enemyTex = Content.Load<Texture2D>("Spaceship1");
            _bg = Content.Load<Texture2D>("bgmountain");
            _map = new Map(MapSizeX, MapSizeY, Content.Load<Texture2D>("player"));
            /*  if (File.Exists("mapdata.json"))
              {
                  _map.MapData = SaveData.LoadMapData();
              }
              else
              {
                  _map.GenerateMap();
              }*/

            _map.GenerateMap();
            //_map.SaveMapToFiles();
            //_map.MapData = SaveData.LoadMapdata();
            _cameraPosition = _player.Pos;

            _toolRectangles = GenerateRectangles(10, 50, 50, 20);

            for (int i = 0; i < maxEnemies; i++)
            {
                _enemies.Add(new Enemy(TileTextures.Enemy1Walk, new Vector2(rand.Next(10, MapSizeX) * 16, rand.Next(10, MapSizeY) * 16)));
            }

            for (int i = 0; i < 0; i++)
            {
                _items.Add(new Item("apple", "shitty apple", TileTextures.Apple, new Vector2(i * 12, -60)));
            }

            for (int i = 0; i < maxFlowers; i++)
            {
                _flowerEntities.Add(new FlowerEntity(new Vector2(rand.Next(10, MapSizeX) * 16, rand.Next(10, MapSizeY) * 16)));
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            penumbra.Dispose();
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
                //CreateTestProjectiles(gameTime);
                //UpdateTime(gameTime);
                MouseState mouseState = CheckMouseInput();
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    SaveData.WriteMapData(_map.MapData);

                    Exit();
                }

                //_player.Update(gameTime);
                //_cameraPosition = _player.Pos - new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                _player.Update(gameTime);

                // TODO: Add your update logic here

                _map.Update(_player);

                List<Projectile> projectileCopy = new List<Projectile>(_projectiles);
                foreach (Projectile p in projectileCopy)
                {
                    p.Update(gameTime);
                }
                List<FlowerEntity> flowerCopy = new List<FlowerEntity>(_flowerEntities);
                foreach (FlowerEntity e in flowerCopy)
                {
                    e.Update(gameTime);
                }
                List<DamageMessage> damageMessagesCopy = new List<DamageMessage>(_damageMessageList);
                foreach (DamageMessage dm in damageMessagesCopy)
                {
                    dm.Update(gameTime, this);
                }
                foreach (Enemy e in _enemies)
                {
                    // if (e.IsVisible(this))
                    e.Update(gameTime, _cameraPosition, _player, this);
                }

                List<Item> itemsCopy = new List<Item>(_items);
                foreach (Item i in itemsCopy)
                {
                    i.Update(gameTime);
                }
                //_items = itemsCopy;

                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                _cameraPosition = _player.Pos - screenCenter;
                base.Update(gameTime);
            }
            rainsystem.Update(gameTime);
        }

        private void CreateTestProjectiles(GameTime gameTime)
        {
            // float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // testProjectileTimer += deltatime;
            // if (testProjectileTimer > testProjectileTreshold)
            // {
            float minValue = -1.0f; // Minimum value of the range
            float maxValue = 1.0f; // Maximum value of the range
            float randomNumber = (float)(rand.NextDouble() * (maxValue - minValue) + minValue);
            float randomNumber2 = (float)(rand.NextDouble() * (maxValue - minValue) + minValue);
            var test = new Vector2(randomNumber, randomNumber2);
            var direction = Vector2.Normalize(test);
            _projectiles.Add(new Projectile(3f, new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), direction, TileTextures.Apple));
            // testProjectileTimer = 0;
            // }
        }

        private MouseState CheckMouseInput()
        {
            List<Enemy> enemiesToDamage = new List<Enemy>();
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            // Adjust the mouse position by the camera's position
            mousePosition += _cameraPosition;
            int tileX = (int)(mousePosition.X / 16);
            int tileY = (int)(mousePosition.Y / 16);

            if (tileX >= 0 && tileX < _map.Width && tileY >= 0 && tileY < _map.Height)
            {
                if (_map.MapData[tileX, tileY].IsVisible)
                {
                    mouseOverText = _map.MapData[tileX, tileY].MapTile.ToString();
                }
                else
                {
                    mouseOverText = "__";
                }
            }
            // List<Enemy> copyEnemies = new List<Enemy>(_enemies);
            foreach (Enemy e in _enemies)
            {
                int tileXX = (int)(mousePosition.X / 16);
                int tileYY = (int)(mousePosition.Y / 16);
                // Transform the mouse position to be relative to the camera
                Point mousePoint = new Point((int)(mousePosition.X - _cameraPosition.X), (int)(mousePosition.Y - _cameraPosition.Y));

                if (e.boundingBox.Contains(mousePoint))
                {
                    mouseOverText = e.Name;
                    break;
                }
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Get the position of the mouse click in the game world coordinates

                // List<Enemy> copyEnemies = new List<Enemy>(_enemies);

                foreach (Enemy e in _enemies)
                {
                    int tileXX = (int)(mousePosition.X / 16);
                    int tileYY = (int)(mousePosition.Y / 16);
                    // Transform the mouse position to be relative to the camera
                    Point mousePoint = new Point((int)(mousePosition.X - _cameraPosition.X), (int)(mousePosition.Y - _cameraPosition.Y));

                    if (e.boundingBox.Contains(mousePoint))
                    {
                        enemiesToDamage.Add(e);
                        break;
                    }
                }
                // Calculate the tile coordinates based on the mouse position

                // Check if the tile coordinates are within the map bounds
                if (IsTileValid(tileX, tileY) && _map.MapData[tileX, tileY].MapTile != MapTiles.empty)
                {
                    // Change the tile at the clicked position
                    if (!_wasMousePressed)
                    {
                        _damageMessageList.Add(new DamageMessage("Removed " + _map.MapData[tileX, tileY].MapTile.ToString() + " at position " + tileX + "," + tileY, 5f, mousePosition + new Vector2(20, -20), this));
                        _items.Add(new Item(_map.MapData[tileX, tileY].MapTile.ToString(), "testi", _map.MapData[tileX, tileY].Texture, new Vector2(tileX * 16, tileY * 16)));
                        _map.MapData[tileX, tileY] = new Tile(MapTiles.empty, TileTextures.EmptyTexture, false, false);
                    }
                }
                _wasMousePressed = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                _wasMousePressed = false;
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                // Get the position of the mouse click in the game world coordinates
                //Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y) + _cameraPosition;
                // Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(_cameraPosition));

                // Calculate the tile coordinates based on the mouse position
                //int tileX = (int)(mousePosition.X / 16);
                //int tileY = (int)(mousePosition.Y / 16);

                // Check if the tile coordinates are within the map bounds
                if (IsTileValid(tileX, tileY))
                {
                    // Change the tile at the clicked position
                    _map.MapData[tileX, tileY] = new Tile(MapTiles.dirt, TileTextures.DirtTexture, true, false);
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
            // _cameraPosition = _enemies[0].Position;
            foreach (Enemy e in enemiesToDamage)
            {
                _enemies.Remove(e);
            }
            return mouseState;
        }

        private bool IsTileValid(int tileX, int tileY)
        {
            return tileX >= 0 && tileX < _map.Width && tileY >= 0 && tileY < _map.Height;
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
            penumbra.BeginDraw();

            _spriteBatch.Begin(SpriteSortMode.Deferred);

            // Draw background
            //  _spriteBatch.Draw(_bg, Vector2.Zero, _lerpedColor);
            rainsystem.Draw(_spriteBatch);
            _map.DrawMap(_spriteBatch, _player, _cameraPosition, GraphicsDevice.Viewport, GraphicsDevice, gameTime);

            // Draw inventory if toggled
            if (_player.InventoryToggled)
            {
                Game1.Instance._player.Inventory.UI.DrawInventory(Game1.Instance._player.Inventory);
            }

            foreach (FlowerEntity f in _flowerEntities)
            {
                f.Draw(_spriteBatch, _cameraPosition, gameTime);
            }
            // Draw enemies
            foreach (Enemy e in _enemies)
            {
                e.Draw(_spriteBatch, _cameraPosition, gameTime);
            }

            // Draw items and projectiles
            foreach (Item i in _items)
            {
                i.Draw(_spriteBatch);
            }

            foreach (Projectile p in _projectiles)
            {
                p.Draw(_spriteBatch);
            }

            // Draw map and player

            _player.Draw(_spriteBatch, _cameraPosition, GraphicsDevice.Viewport);

            _spriteBatch.End();

            // Draw lighting using Penumbra
            penumbra.Draw(gameTime);

            _spriteBatch.Begin();

            // Draw additional UI elements
            _spriteBatch.DrawString(_myFont, mouseOverText, new Vector2(100, 100), Color.Red);

            // Draw tool rectangles
            foreach (Rectangle toolRectangle in _toolRectangles)
            {
                _spriteBatch.Draw(TileTextures.DirtTexture, toolRectangle, Color.Red);
            }

            // Draw damage messages
            foreach (DamageMessage dm in _damageMessageList)
            {
                dm.Draw(this);
            }

            // Update and display FPS
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCount++;

            if (_elapsedTime >= 1.0f)
            {
                Window.Title = "FPS: " + _frameCount;
                _frameCount = 0;
                _elapsedTime = 0.0f;
            }

            _spriteBatch.End();
        }
    }
}
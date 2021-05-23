using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;

namespace InventoryManager
{
    public class GameWindow : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _defaultFont;

        private Inventory _engine;

        public GameWindow()
        {
            this.Window.AllowUserResizing = true;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS_CAP);
        }

        protected override void Initialize()
        {
            this.Window.Title = Config.WINDOW_TITLE;

            this._engine = new Inventory(new Vector2(0,0),10,5, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height, null, null);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _defaultFont = Content.Load<SpriteFont>("Default");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            this._engine.Update(this.GraphicsDevice, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            this._spriteBatch.Begin();

            //draw fps
            if (Config.DISPLAY_FPS)
            {
                Toolkit.DrawFPSAt(new Vector2(5, 5), _defaultFont, _spriteBatch, 1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            //draw invetory
            this._engine.Draw(new Vector2(0, 0), this._spriteBatch);

            this._spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

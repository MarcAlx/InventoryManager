using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;

namespace InventoryManager
{
    public class Herb : Item
    {
        public Herb(Vector2 pos) : base(pos,1,3,null) {
        }
    }

    public class Box : Item
    {
        public Box(Vector2 pos) : base(pos,2,2,null)
        {
        }
    }

    public class Egg : Item
    {
        public Egg(Vector2 pos) : base(pos,1,1,null)
        {
        }
    }

    public class GameWindow : Game
    {
        private enum ActionEnum {
            NEUTRAL,
            LEFT,
            RIGHT,
            UP,
            DOWN,
            SELECT,
            ROTATE_LEFT,
            ROTATE_RIGHT,
            UNSELECT,
            CANCEL
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _defaultFont;

        private Inventory _engine;

        private ActionEnum _currentAction = ActionEnum.NEUTRAL;
        private Timer _pollActionTimer;

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
            this._engine.StoreItem(new Herb(new Vector2(1,1)));
            this._engine.StoreItem(new Box(new Vector2(4, 3)));
            this._engine.StoreItem(new Egg(new Vector2(5, 0)));

            this._pollActionTimer = new Timer(() =>
            {
                switch (this._currentAction)
                {
                    case ActionEnum.CANCEL:
                        this._engine.CancelSelect();
                        break;
                    case ActionEnum.UP:
                        this._engine.MoveUp();
                        break;
                    case ActionEnum.DOWN:
                        this._engine.MoveDown();
                        break;
                    case ActionEnum.LEFT:
                        this._engine.MoveLeft();
                        break;
                    case ActionEnum.RIGHT:
                        this._engine.MoveRight();
                        break;
                    case ActionEnum.SELECT:
                        this._engine.TrySelect();
                        break;
                    case ActionEnum.UNSELECT:
                        this._engine.UnSelect();
                        break;
                    case ActionEnum.ROTATE_LEFT:
                        this._engine.RotateAntiClockwise();
                        break;
                    case ActionEnum.ROTATE_RIGHT:
                        this._engine.RotateClockwise();
                        break;
                    case ActionEnum.NEUTRAL:
                    default:
                        break;
                }
            }, Config.POLL_KEYBOARD_INTERVAL, true);

            this._pollActionTimer.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _defaultFont = Content.Load<SpriteFont>("Default");
        }

        protected override void Update(GameTime gameTime)
        {
            //poll keyboard
            KeyboardState keyBoardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyBoardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyBoardState.IsKeyDown(Keys.Left)){
                this._currentAction = ActionEnum.LEFT;
            }
            else if (keyBoardState.IsKeyDown(Keys.Right))
            {
                this._currentAction = ActionEnum.RIGHT;
            }
            else if (keyBoardState.IsKeyDown(Keys.Up))
            {
                this._currentAction = ActionEnum.UP;
            }
            else if (keyBoardState.IsKeyDown(Keys.Down))
            {
                this._currentAction = ActionEnum.DOWN;
            }
            else if (keyBoardState.IsKeyDown(Keys.L))
            {
                this._currentAction = ActionEnum.ROTATE_LEFT;
            }
            else if (keyBoardState.IsKeyDown(Keys.R))
            {
                this._currentAction = ActionEnum.ROTATE_RIGHT;
            }
            else if (keyBoardState.IsKeyDown(Keys.Enter))
            {
                if (this._engine.HasSelection)
                {
                    this._currentAction = ActionEnum.UNSELECT;
                }
                else
                {
                    this._currentAction = ActionEnum.SELECT;
                }
            }
            else if (keyBoardState.IsKeyDown(Keys.Space))
            {
                this._currentAction = ActionEnum.CANCEL;
            }
            else
            {
                this._currentAction = ActionEnum.NEUTRAL;
            }

            this._pollActionTimer.Update(gameTime.ElapsedGameTime);

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

            //draw inventory
            this._engine.Draw(new Vector2(0, 0), this._spriteBatch);

            this._spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Interface for moving things
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// Move up
        /// </summary>
        void MoveUp();

        /// <summary>
        /// Move down
        /// </summary>
        void MoveDown();

        /// <summary>
        /// Move left
        /// </summary>
        void MoveLeft();

        /// <summary>
        /// Move right
        /// </summary>
        void MoveRight();

        /// <summary>
        /// Move to specific grid position
        void Move(Vector2 to);
    }

    /// <summary>
    /// Interface to rotate thing
    /// </summary>
    public interface IRotatable
    {
        /// <summary>
        /// Current rotation angle, in radians
        /// </summary>
        float RotationAngle { get; }

        /// <summary>
        /// Rotate clockwise
        /// </summary>
        void RotateClockwise();

        /// <summary>
        /// Rotate anti clockwise
        /// </summary>
        void RotateAntiClockwise();
    }

    /// <summary>
    /// Defines an interface to select
    /// </summary>
    public interface ISelector<T>
    {
        /// <summary>
        /// Item selected
        /// </summary>
        T SelectedItem { get; } 

        /// <summary>
        /// True if it has selection
        /// </summary>
        bool HasSelection { get; }

        /// <summary>
        /// Try to select
        /// </summary>
        void TrySelect();

        /// <summary>
        /// Unselect
        /// </summary>
        void UnSelect();

        /// <summary>
        /// Cancel selection
        /// </summary>
        void CancelSelect();
    }

    /// <summary>
    /// Interface for drawing things
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Texture to draw
        /// </summary>
        Texture2D Texture2D { get; }

        /// <summary>
        /// Scale for texture
        /// </summary>
        Vector2 Scale => new Vector2(
            this.DrawingWidth / this.Texture2D.Width,
            this.DrawingHeight / this.Texture2D.Height);

        /// <summary>
        /// Rotation angle
        /// </summary>
        float RotationAngle { get; }

        /// <summary>
        /// Final drawing size
        /// </summary>
        int DrawingWidth { get; }

        /// <summary>
        /// Final drawing height
        /// </summary>
        int DrawingHeight { get; }

        /// <summary>
        /// Drawing position
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Update drawing according to size
        /// </summary>
        /// <param name="device"></param>
        /// <param name="drawingWidth"></param>
        /// <param name="drawingHeight"></param>
        void Update(GraphicsDevice device, int drawingWidth, int drawingHeight);
    }

    /// <summary>
    /// Represents an itme in a grid
    /// </summary>
    public interface IGridItem
    {
        /// <summary>
        /// Position of the item
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Grid in which the item is placed
        /// </summary>
        IGrid ReferenceGrid { get; }
    }

    /// <summary>
    /// Represents a grid
    /// </summary>
    public interface IGrid
    {
        /// <summary>
        /// Width of the grid
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the grid
        /// </summary>
        int Height { get; }
    }

    /// <summary>
    /// Represents an item that can move in a grid
    /// </summary>
    public abstract class MovableGridItem : IGridItem, IGrid, IMovable
    {
        public Vector2 Position { get; protected set; }

        public IGrid ReferenceGrid { get; set; }

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public Rectangle Bounds {
            get
            {
                return new Rectangle(Convert.ToInt32(this.Position.X), Convert.ToInt32(this.Position.Y), this.Width, this.Height);
            }
        }

        public MovableGridItem(Vector2 initialPosition)
        {
            this.Position = initialPosition;
        }

        public void MoveDown()
        {
            var newY = this.Position.Y + 1;
            if (newY < this.ReferenceGrid.Height)
            {
                this.Position = new Vector2(
                    this.Position.X,
                    Math.Min(newY, this.ReferenceGrid.Height)
                );
            }
        }

        public void MoveLeft()
        {
            this.Position = new Vector2(
                Math.Max(this.Position.X - 1, 0),
                this.Position.Y
            );
        }

        public void MoveRight()
        {
            var newX = this.Position.X + 1;
            if (newX < this.ReferenceGrid.Width)
            {
                this.Position = new Vector2(
                Math.Min(this.Position.X + 1, this.ReferenceGrid.Width),
                    this.Position.Y
                );
            }
        }

        public void MoveUp()
        {
            this.Position = new Vector2(
                this.Position.X,
                Math.Max(this.Position.Y - 1, 0)
            );
        }

        public void Move(Vector2 to)
        {
            if(to.X > 0 && to.X < this.ReferenceGrid.Width
            && to.Y > 0 && to.Y< this.ReferenceGrid.Height)
            {
                this.Position = to;
            }
        }
    }

    /// <summary>
    /// Represents an item that can move an rotate in a grid
    /// </summary>
    public abstract class MovableRotatableGridItem : MovableGridItem, IRotatable
    {
        public float RotationAngle { get; private set; } = 0;

        public MovableRotatableGridItem(Vector2 initialPosition) : base(initialPosition)
        {
        }

        public void RotateAntiClockwise()
        {
            var w = this.Width;
            this.Width = this.Height;
            this.Height = w;
            this.RotationAngle -= Convert.ToSingle(Toolkit.ConvertToRadians(90));
        }

        public void RotateClockwise()
        {
            var w = this.Width;
            this.Width = this.Height;
            this.Height = w;
            this.RotationAngle += Convert.ToSingle(Toolkit.ConvertToRadians(90));
        }
    }

    /// <summary>
    /// Represents a pointer, basically a movable grid item of size 1x1
    /// </summary>
    public class Pointer : MovableGridItem, IDrawable
    {
        public Texture2D Texture2D { get; protected set; }

        public float RotationAngle { get; protected set; }

        public int DrawingWidth { get; protected set; }

        public int DrawingHeight { get; protected set; }

        private bool hasTexture = false;

        public Pointer(Texture2D texture2D, IGrid referenceGrid) : base(new Vector2(0,0))
        {
            this.Width = 1;
            this.Height = 1;
            this.ReferenceGrid = referenceGrid;

            if (this.Texture2D == null)
            {
                hasTexture = false;
            }
            else
            {
                this.Texture2D = texture2D;
            }
        }

        public void Update(GraphicsDevice device, int drawingWidth, int drawingHeight)
        {
            if (!hasTexture)
            {
                this.Texture2D = new Texture2D(device, drawingWidth, drawingHeight, false, SurfaceFormat.Color);
                this.Texture2D.SetData(Enumerable.Range(0, drawingWidth * drawingHeight).Select(i => Color.White).ToArray());
            }
        }
    }

    /// <summary>
    /// Represents an item (extends it to your needs) that can move and rotate
    /// </summary>
    public abstract class Item : MovableRotatableGridItem, IDrawable
    {
        public Texture2D Texture2D { get; protected set; }

        public int DrawingWidth { get; protected set; }

        public int DrawingHeight { get; protected set; }

        public Item(Vector2 initialPosition,int width, int height, Texture2D texture2D) : base(initialPosition)
        {
            this.Width = width;
            this.Height = height;
            if (this.Texture2D == null)
            {
                hasTexture = false;
            }
            else
            {
                this.Texture2D = texture2D;
            }
        }

        public float Angle
        {
            get
            {
                return this.RotationAngle;
            }
        }

        private bool hasTexture = false;

        public void Update(GraphicsDevice device, int drawingWidth, int drawingHeight)
        {
            if (!hasTexture)
            {
                this.Texture2D = new Texture2D(device, drawingWidth, drawingHeight, false, SurfaceFormat.Color);
                this.Texture2D.SetData(Enumerable.Range(0, drawingWidth * drawingHeight).Select(i => Color.LightGreen).ToArray());
            }
        }
    }

    /// <summary>
    /// The Inventory, a drawable grid, that forward Movable and Rotatable to Pointer or selected item.
    /// And that can select
    /// </summary>
    public class Inventory : IMovable, IRotatable, IGrid, IDrawable, ISelector<Item>
    {
        /// <summary>
        /// Pointer moved by user
        /// </summary>
        private Pointer _pointer;

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public Texture2D Texture2D { get; protected set; }

        public int DrawingWidth { get; protected set; }

        public int DrawingHeight { get; protected set; }

        public Vector2 Scale => new Vector2(
            this.Width / this.Texture2D.Width,
            this.Height / this.Texture2D.Height);

        public float RotationAngle { get { return 0; } }

        public Vector2 Position { get; protected set; }

        /// <summary>
        /// Items stored in inventory
        /// </summary>
        public List<Item> StoredItems;

        private int _gridCellSizeWidth;
        private int _gridCellSizeHeight;

        public Item SelectedItem { get; private set; }

        private bool hasTexture = false;

        public bool HasSelection
        {
            get
            {
                return this.SelectedItem != null;
            }
        }

        public Inventory(
            Vector2 position,
            int width,
            int height,
            int drawingWidth,
            int drawingHeight,
            Texture2D texture2D,
            Texture2D pointerTexture2D)
        {
            this.DrawingWidth = drawingWidth;
            this.DrawingHeight = drawingHeight;
            this._gridCellSizeWidth = this.DrawingWidth / width;
            this._gridCellSizeHeight = this.DrawingHeight / height;
            this._pointer = new Pointer(pointerTexture2D, this);

            this.StoredItems = new List<Item>();
            this.Position = position;
            this.Width = width;
            this.Height = height;

            this.Texture2D = texture2D;
            this.hasTexture = this.Texture2D != null;
        }

        public void MoveDown()
        {
            MovableGridItem toMove = (this.SelectedItem != null) ? this.SelectedItem as MovableGridItem : this._pointer as MovableGridItem;
            toMove.MoveDown();
        }

        public void MoveLeft()
        {
            MovableGridItem toMove = (this.SelectedItem != null) ? this.SelectedItem as MovableGridItem : this._pointer as MovableGridItem;
            toMove.MoveLeft();
        }

        public void MoveRight()
        {
            MovableGridItem toMove = (this.SelectedItem != null) ? this.SelectedItem as MovableGridItem : this._pointer as MovableGridItem;
            toMove.MoveRight();
        }

        public void MoveUp()
        {
            MovableGridItem toMove = (this.SelectedItem != null) ? this.SelectedItem as MovableGridItem : this._pointer as MovableGridItem;
            toMove.MoveUp();
        }

        public void Move(Vector2 to)
        {
            //unsupported
        }

        public void TrySelect()
        {
            if (!this.HasSelection) {
                foreach (var item in this.StoredItems)
                {
                    if (item.Bounds.Intersects(this._pointer.Bounds))
                    {
                        this.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        public void UnSelect()
        {
            if (this.HasSelection)
            {
                foreach (var item in this.StoredItems)
                {
                    //an item intersects pointer
                    if (item != this.SelectedItem
                    &&  item.Bounds.Intersects(this.SelectedItem.Bounds))
                    {
                        //do nothing
                        return;
                    }
                }
                //no item conflicting -> apply selection
                this._pointer.Move(this.SelectedItem.Position);
                this.SelectedItem = null;
            }
        }

        public void CancelSelect()
        {
            if (this.HasSelection)
            {
                this.SelectedItem.Move(this._pointer.Position);
                this.SelectedItem = null;
            }
        }

        public void RotateClockwise()
        {
            if (this.SelectedItem != null)
            {
                this.SelectedItem.RotateAntiClockwise();
            }
        }

        public void RotateAntiClockwise()
        {
            if (this.SelectedItem != null)
            {
                this.SelectedItem.RotateClockwise();
            }
        }

        public void StoreItem(Item item)
        {
            item.ReferenceGrid = this;
            this.StoredItems.Add(item);
        }

        public void Update(GraphicsDevice device, int drawingWidth, int drawingHeight)
        {
            this.DrawingWidth = drawingWidth;
            this.DrawingHeight = drawingHeight;
            this._gridCellSizeWidth = this.DrawingWidth / this.Width;
            this._gridCellSizeHeight = this.DrawingHeight / this.Height;

            if (!this.hasTexture)
            {
                this.Texture2D = new Texture2D(device, drawingWidth, drawingHeight, false, SurfaceFormat.Color);
                this.Texture2D.SetData(Enumerable.Range(0, drawingWidth * drawingHeight).Select(i => Color.LightCoral).ToArray());
            }

            //update items
            foreach (var item in this.StoredItems)
            {
                item.Update(device, item.Width * this._gridCellSizeWidth, item.Height * this._gridCellSizeHeight);
            }

            //update pointer
            this._pointer.Update(device, this._pointer.Width * this._gridCellSizeWidth, this._pointer.Height * this._gridCellSizeHeight);
        }

        public void Draw(Vector2 position, SpriteBatch batch)
        {
            //draw inventory
            batch.Draw(this.Texture2D, position, Color.White);

            //draw items
            foreach(var item in this.StoredItems)
            {
                batch.Draw(
                item.Texture2D,
                new Vector2(
                    item.Position.X * this._gridCellSizeWidth,
                    item.Position.Y * this._gridCellSizeHeight),
                Color.White);
            }

            //draw pointer
            batch.Draw(
                this._pointer.Texture2D,
                new Vector2(
                    this._pointer.Position.X * this._gridCellSizeWidth,
                    this._pointer.Position.Y * this._gridCellSizeHeight),
                Color.White);
        }
    }
}

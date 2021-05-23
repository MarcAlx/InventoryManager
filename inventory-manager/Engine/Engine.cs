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
        /// True if rotated
        /// </summary>
        bool IsRotated { get; }

        /// <summary>
        /// Rotate
        /// </summary>
        void Rotate();

        /// <summary>
        /// Return rotation origin, according to cell size
        /// </summary>
        /// <param name="cellSize"></param>
        /// <returns></returns>
        Vector2 GetRotationOrigin(int cellSize);
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

        /// <summary>
        /// Update drawing according to a cell size
        /// </summary>
        /// <param name="device"></param>
        /// <param name="cellSize"></param>
        void Update(GraphicsDevice device, int cellSize);
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
            if (this.Position.Y + this.Height < this.ReferenceGrid.Height)
            {
                this.Position = new Vector2(
                    this.Position.X,
                    Math.Min(this.Position.Y + 1, this.ReferenceGrid.Height)
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
            if (this.Position.X + this.Width < this.ReferenceGrid.Width)
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
            if(to.X >= 0 && to.X < this.ReferenceGrid.Width
            && to.Y >= 0 && to.Y< this.ReferenceGrid.Height)
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

        public abstract Vector2 GetRotationOrigin(int cellSize);

        public bool IsRotated
        {
            get
            {
                return this.RotationAngle != 0;
            }
        }

        public MovableRotatableGridItem(Vector2 initialPosition) : base(initialPosition)
        {
        }

        public void Rotate()
        {
            var w = this.Width;
            this.Width = this.Height;
            this.Height = w;

            if (!this.IsRotated)
            {
                this.RotationAngle -= Convert.ToSingle(Toolkit.ConvertToRadians(90));
            }
            else
            {
                this.RotationAngle = 0;
            }
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

        public void Update(GraphicsDevice device, int cellSize)
        {
            this.Update(device, cellSize, cellSize);
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

        private int _initWidth;
        private int _initHeight;

        public Item(Vector2 initialPosition,int width, int height, Texture2D texture2D) : base(initialPosition)
        {
            this.Width = width;
            this.Height = height;
            this._initWidth = width;
            this._initHeight = height;
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
            this.DrawingWidth = drawingWidth;
            this.DrawingHeight = drawingHeight;
            if (!hasTexture)
            {
                this.Texture2D = new Texture2D(device, drawingWidth, drawingHeight, false, SurfaceFormat.Color);
                this.Texture2D.SetData(Enumerable.Range(0, drawingWidth * drawingHeight).Select(i => Color.LightGreen).ToArray());
            }
        }

        public void Update(GraphicsDevice device, int cellSize)
        {
            this.DrawingWidth = cellSize*this._initWidth;
            this.DrawingHeight = cellSize*this._initHeight;
            if (!hasTexture)
            {
                this.Texture2D = new Texture2D(device, this.DrawingWidth, this.DrawingHeight, false, SurfaceFormat.Color);
                this.Texture2D.SetData(Enumerable.Range(0, this.DrawingWidth * this.DrawingHeight).Select(i => Color.LightGreen).ToArray());
            }
        }

        public override Vector2 GetRotationOrigin(int cellSize)
        {
            if(!this.IsRotated)
            {
                return new Vector2();
            }
            else
            {
                return new Vector2(cellSize/2.0f,cellSize/2.0f);
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

        public bool IsRotated { get; protected set; } = false;

        /// <summary>
        /// Items stored in inventory
        /// </summary>
        public List<Item> StoredItems;

        private int _gridCellSizeWidth;
        private int _gridCellSizeHeight;

        public Item SelectedItem { get; private set; }

        private bool hasTexture = false;

        private bool _initialRotated;

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
                        this._initialRotated = this.SelectedItem.IsRotated;
                        break;
                    }
                }
            }
        }

        public void UnSelect()
        {
            if (this.HasSelection)
            {
                if(!(new Rectangle(0, 0, this.Width, this.Height)).Contains(this.SelectedItem.Bounds))
                {
                    return;
                }

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

        /// <summary>
        /// Return item at pos in grid if no item null
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Item ItemAt(Vector2 pos)
        {
            foreach (var item in this.StoredItems)
            {
                if (item.Bounds.Intersects(new Rectangle(Convert.ToInt32(pos.X),Convert.ToInt32(pos.Y),1,1)))
                {
                    return item;
                }
            }
            return null;
        }

        public void CancelSelect()
        {
            if (this.HasSelection)
            {
                this.SelectedItem.Move(this._pointer.Position);
                if (this._initialRotated!=this.SelectedItem.IsRotated)
                {
                    this.SelectedItem.Rotate();
                }
                this.SelectedItem = null;
            }
        }

        public void Rotate()
        {
            if (this.SelectedItem != null)
            {
                this.SelectedItem.Rotate();
            }
        }

        public Vector2 GetRotationOrigin(int cellSize)
        {
            return new Vector2();
        }

        /// <summary>
        /// Store an item in enventory, rearrange if it intersects another item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rearrange"></param>
        /// <returns>True if added, false if no place found</returns>
        public bool StoreItem(Item item, bool rearrange = true)
        {
            item.ReferenceGrid = this;

            bool findNewPlace = !(new Rectangle(0, 0, this.Width, this.Height)).Contains(item.Bounds);

            if (!findNewPlace)
            {
                foreach (var tmp in this.StoredItems)
                {
                    if (tmp.Bounds.Intersects(item.Bounds))
                    {
                        //do nothing
                        if (!rearrange)
                        {
                            return false;
                        }
                        else
                        {
                            findNewPlace = true;
                        }
                    }
                }
            }

            if (findNewPlace)
            {
                for(var i = 0; i < this.Width; i++)
                {
                    for (var j = 0; j < this.Height; j++)
                    {
                        var p = new Vector2(i, j);
                        if (this.ItemAt(p) == null) {
                            item.Move(p);
                            this.StoredItems.Add(item);
                            return true;
                        }
                    }
                }
            }
            else
            {
                this.StoredItems.Add(item);
                return true;
            }
            return false;
        }

        public void Update(GraphicsDevice device, int drawingWidth, int drawingHeight)
        {
            this.DrawingWidth = drawingWidth;
            this.DrawingHeight = drawingHeight;
            this._gridCellSizeWidth = this.DrawingWidth / this.Width;
            this._gridCellSizeHeight = this._gridCellSizeWidth;

            var effectiveHeight = (this._gridCellSizeWidth * this.Height);

            if (!this.hasTexture)
            {
                this.Texture2D = new Texture2D(device, drawingWidth, effectiveHeight, false, SurfaceFormat.Color);
                this.Texture2D.SetData(Enumerable.Range(0, drawingWidth * effectiveHeight).Select(i => Color.LightCoral).ToArray());
            }

            //update items
            foreach (var item in this.StoredItems)
            {
                item.Update(device, this._gridCellSizeWidth);
            }

            //update pointer
            this._pointer.Update(device, this._pointer.Width * this._gridCellSizeWidth, this._pointer.Height * this._gridCellSizeHeight);
        }

        public void Update(GraphicsDevice device, int cellSize)
        {
        }

        public void Draw(Vector2 position, SpriteBatch batch)
        {
            //draw inventory
            batch.Draw(this.Texture2D, position, Color.White);

            
            //draw items
            foreach (var item in this.StoredItems)
            {
                var x = Convert.ToInt32(item.Position.X) * this._gridCellSizeWidth;
                var y = Convert.ToInt32(item.Position.Y) * this._gridCellSizeWidth;

                batch.Draw(
                item.Texture2D,
                new Rectangle(
                    !item.IsRotated ? x : x + Convert.ToInt32(this._gridCellSizeWidth / 2),
                    !item.IsRotated ? y : y + Convert.ToInt32(this._gridCellSizeWidth / 2),
                    item.DrawingWidth,
                    item.DrawingHeight
                ),
                null,
                Color.White,
                item.RotationAngle,
                item.GetRotationOrigin(this._gridCellSizeWidth),
                SpriteEffects.None,
                0f);
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

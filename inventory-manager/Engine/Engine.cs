using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public interface IMovable
    {
        void MoveUp();

        void MoveDown();

        void MoveLeft();

        void MoveRigth();
    }

    public interface IRotatable
    {
        void RotateClockwise();

        void RotateAntiClockwise();
    }

    public interface IDrawable
    {
        Texture2D Texture2D { get; }

        Vector2 Scale => new Vector2(
            this.DrawingWidth / this.Texture2D.Width,
            this.DrawingHeight / this.Texture2D.Height);

        float Angle { get; }

        int DrawingWidth { get; }

        int DrawingHeight { get; }

        Vector2 Position { get; }

        void Update(GraphicsDevice device, int drawingWidth, int drawingHeight);
    }

    public interface IGridItem
    {
        Vector2 Position { get; }

        IGrid ReferenceGrid { get; }

        bool FitsIn(IGrid grid);
    }

    public interface IGrid
    {
        int Width { get; }

        int Height { get; }

        bool CanHold(IGridItem item);
    }

    public abstract class MovableGridItem : IGridItem, IGrid, IMovable
    {
        public Vector2 Position { get; protected set; }

        public IGrid ReferenceGrid { get; protected set; }

        public int Width { get; protected set; }

        public int Height { get; protected set; }

       public bool CanHold(IGridItem item)
        {
            throw new NotImplementedException();
        }

        public bool FitsIn(IGrid grid)
        {
            throw new NotImplementedException();
        }

        public void MoveDown()
        {
            this.Position = new Vector2(
                this.Position.X,
                Math.Max(this.Position.Y + this.Height + 1, this.ReferenceGrid.Height)
            );
        }

        public void MoveLeft()
        {
            this.Position = new Vector2(
                Math.Max(this.Position.X - 1, 0),
                this.Position.Y
            );
        }

        public void MoveRigth()
        {
            this.Position = new Vector2(
                Math.Max(this.Position.X + this.Width + 1, this.ReferenceGrid.Width),
                this.Position.Y
            );
        }

        public void MoveUp()
        {
            this.Position = new Vector2(
                this.Position.X,
                Math.Min(this.Position.Y - 1, 0)
            );
        }
    }

    public abstract class MovableRotatableGridItem : MovableGridItem, IRotatable
    {
        public void RotateAntiClockwise()
        {
            var w = this.Width;
            this.Width = this.Height;
            this.Height = w;
            //this.Angle -= Convert.ToSingle(Toolkit.ConvertToRadians(90));
        }

        public void RotateClockwise()
        {
            var w = this.Width;
            this.Width = this.Height;
            this.Height = w;
            //this.Angle += Convert.ToSingle(Toolkit.ConvertToRadians(90));
        }
    }

    public class Pointer : MovableGridItem, IDrawable
    {
        public Texture2D Texture2D { get; protected set; }

        public float Angle { get; protected set; }

        public int DrawingWidth { get; protected set; }

        public int DrawingHeight { get; protected set; }

        bool hasTexture = false;

        public Pointer(int drawingWidth, int drawingHeight, Texture2D texture2D)
        {
            this.Width = 1;
            this.Height = 1;

            if (this.Texture2D == null)
            {
                hasTexture = false;
            }
            else
            {
                this.Texture2D = texture2D;
            }
        }

        public new bool CanHold(IGridItem item)
        {
            return false;
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

    public abstract class Item : MovableRotatableGridItem, IDrawable
    {
        public Texture2D Texture2D { get; protected set; }

        public float Angle { get; protected set; }

        public int DrawingWidth { get; protected set; }

        public int DrawingHeight { get; protected set; }

        public void Update(GraphicsDevice device, int drawingWidth, int drawingHeight)
        {
            throw new NotImplementedException();
        }
    }

    public class Inventory : IMovable, IGrid, IDrawable
    {
        private Pointer _pointer;

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public Texture2D Texture2D { get; protected set; }

        public int DrawingWidth { get; protected set; }

        public int DrawingHeight { get; protected set; }

        public Vector2 Scale => new Vector2(
            this.Width / this.Texture2D.Width,
            this.Height / this.Texture2D.Height);

        public float Angle { get { return 0; } }

        public Vector2 Position { get; protected set; }

        public List<IGridItem> Items;

        private int _gridCellSizeWidth;
        private int _gridCellSizeHeight;

        private IGridItem _selectedItem;

        private bool hasTexture = false;

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
            this._pointer = new Pointer(this._gridCellSizeWidth,this._gridCellSizeHeight,pointerTexture2D);

            this.Items = new List<IGridItem>();
            this.Position = position;
            this.Width = width;
            this.Height = height;

            this.Texture2D = texture2D;
            this.hasTexture = this.Texture2D != null;
        }

        public bool CanHold(IGridItem item)
        {
            throw new NotImplementedException();
        }

        public void MoveDown()
        {
            if(this._selectedItem != null)
            {
                this._pointer.MoveDown();
            }
        }

        public void MoveLeft()
        {
            if (this._selectedItem != null)
            {
                this._pointer.MoveLeft();
            }
        }

        public void MoveRigth()
        {
            if (this._selectedItem != null)
            {
                this._pointer.MoveRigth();
            }
        }

        public void MoveUp()
        {
            if (this._selectedItem != null)
            {
                this._pointer.MoveUp();
            }
        }

        public void TrySelect()
        {

        }

        public void UnSelect()
        {

        }

        public void CancelSelect()
        {

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
                this.Texture2D.SetData(Enumerable.Range(0, drawingWidth * drawingHeight).Select(i => Color.Blue).ToArray());
            }
            this._pointer.Update(device, this._gridCellSizeWidth, this._gridCellSizeHeight);
        }

        public void Draw(Vector2 position, SpriteBatch batch)
        {
            //draw inventory
            batch.Draw(this.Texture2D, position, Color.White);
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

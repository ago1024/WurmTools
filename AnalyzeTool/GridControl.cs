using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AnalyzeTool
{
    class GridControl : Control
    {
        int gridSizeX = 32;
        int gridSizeY = 32;
        int cellWidth = 32;
        int cellHeight = 32;
        int gridWidth;
        int gridHeight;
        int borderSize = 1;

        public class Cell
        {
            public Cell(int cx, int cy)
            {
                x = cx;
                y = cy;
            }

            private int x;
            public int X
            {
                get { return x; }
                set { x = value; }
            }

            private int y;
            public int Y
            {
                get { return y; }
                set { y = value; }
            }

        }

        public delegate void OnCellPaintHandler(Object sender, Cell cell, PaintEventArgs eventArgs);
        public event OnCellPaintHandler CellPaint;

        public delegate void OnCellMouseClickHandler(Object sender, Cell cell, MouseEventArgs eventArgs);
        public event OnCellMouseClickHandler CellMouseClick;

        public delegate void OnCellMouseMoveHandler(Object sender, Cell cell, MouseEventArgs eventArgs);
        public event OnCellMouseMoveHandler CellMouseMove;

        public delegate void OnCellMouseEnterHandler(Object sender, Cell cell, MouseEventArgs eventArgs);
        public event OnCellMouseEnterHandler CellMouseEnter;

        public GridControl()
        {
            this.UpdateSize();
            this.DoubleBuffered = true;
        }

        private void UpdateSize() {
            if (borderSize < 0)
                borderSize = 0;
            this.gridWidth = gridSizeX * cellWidth + (gridSizeX + 1) * borderSize;
            this.gridHeight = gridSizeY * cellHeight + (gridSizeY + 1) * borderSize;
            this.ClientSize = new Size(gridWidth, gridHeight);
            this.Invalidate();
        }

        public int GridSizeX
        {
            get { return gridSizeX; }
            set
            {
                this.gridSizeX = value;
                this.UpdateSize();
            }
        }

        public int GridSizeY
        {
            get { return gridSizeY; }
            set
            {
                this.gridSizeY = value;
                this.UpdateSize();
            }
        }

        public int CellWidth
        {
            get { return cellWidth; }
            set
            {
                this.cellWidth = value;
                this.UpdateSize();
            }
        }

        public int CellHeight
        {
            get { return cellHeight; }
            set
            {
                this.cellHeight = value;
                this.UpdateSize();
            }
        }

        public int BorderSize
        {
            get { return borderSize; }
            set { 
                borderSize = value;
                this.UpdateSize();
            }

        }

        public bool IsCell(int x, int y)
        {
            if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Rectangle RectFromCell(int x, int y)
        {
            int left = borderSize + x * (cellWidth + borderSize);
            int top = borderSize + y * (cellHeight + borderSize);
            return new Rectangle(left, top, cellWidth, cellHeight);
        }

        private int CellFromX(int x)
        {
            return x / (cellWidth + borderSize);
        }

        private int CellFromY(int y)
        {
            return y / (cellHeight + borderSize);
        }

        public Cell CellFromPoint(int x, int y)
        {
            Point pt = new Point(x, y);

            int cx = CellFromX(x);
            int cy = CellFromY(y);

            Rectangle rect = RectFromCell(cx, cy);
            if (rect.Contains(pt))
            {
                return new Cell(cx, cy);
            }
            return null;
        }

        private void DrawGrid(Graphics graphics, Rectangle rect)
        {
            int lowx = Math.Max(0, CellFromX(rect.Left));
            int lowy = Math.Max(0, CellFromY(rect.Top));
            int highx = Math.Min(gridSizeX, CellFromX(rect.Right));
            int highy = Math.Min(gridSizeY, CellFromY(rect.Bottom));

            for (int x = lowx; x <= highx; x++)
            {
                int lineX = x * (cellWidth + borderSize);
                if (borderSize == 1)
                    graphics.DrawLine(Pens.Black, lineX, 0, lineX, gridHeight - 1);
                else
                    graphics.FillRectangle(Brushes.Black, lineX, 0, borderSize, gridHeight);
            }
            for (int y = lowy; y <= highy; y++)
            {
                int lineY = y * (cellHeight + borderSize);
                if (borderSize == 1)
                    graphics.DrawLine(Pens.Black, 0, lineY, gridWidth - 1, lineY);
                else
                    graphics.FillRectangle(Brushes.Black, 0, lineY, gridWidth, borderSize);
            }
        }

        override protected void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = e.ClipRectangle;
            Graphics g = e.Graphics;

            DrawGrid(g, rect);

            int lowx = Math.Max(0, CellFromX(rect.Left));
            int lowy = Math.Max(0, CellFromY(rect.Top));
            int highx = Math.Min(gridSizeX, CellFromX(rect.Right));
            int highy = Math.Min(gridSizeY, CellFromY(rect.Bottom));

            if (CellPaint != null)
            {
                GraphicsState state = g.Save();
                for (int x = lowx; x <= highx; x++)
                {
                    for (int y = lowy; y <= highy; y++)
                    {
                        if (IsCell(x, y))
                        {
                            Rectangle cellRect = RectFromCell(x, y);
                            CellPaint(this, new Cell(x, y), new PaintEventArgs(g, cellRect));
                        }
                    }
                }
                g.Restore(state);
            }

        }

        public virtual void PaintCell(int x, int y) {
            if (IsCell(x, y)) {
                Rectangle cellRect = RectFromCell(x, y);
                Invalidate(cellRect);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int cx = CellFromX(e.X);
            int cy = CellFromY(e.Y);

            if (IsCell(cx, cy) && CellMouseClick != null)
            {
                Rectangle rect = RectFromCell(cx, cy);
                CellMouseClick(this, new Cell(cx, cy), new MouseEventArgs(e.Button, e.Clicks, e.X - rect.Left, e.Y - rect.Top, e.Delta));
            }
        }

        private Cell lastCell = null;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int cx = CellFromX(e.X);
            int cy = CellFromY(e.Y);

            if (IsCell(cx, cy) && CellMouseMove != null)
            {
                Rectangle rect = RectFromCell(cx, cy);
                CellMouseMove(this, new Cell(cx, cy), new MouseEventArgs(e.Button, e.Clicks, e.X - rect.Left, e.Y - rect.Top, e.Delta));

                if (CellMouseEnter != null)
                {
                    if (lastCell == null)
                    {
                        lastCell = new Cell(cx, cy);
                        CellMouseEnter(this, new Cell(cx, cy), new MouseEventArgs(e.Button, e.Clicks, e.X - rect.Left, e.Y - rect.Top, e.Delta));
                    }
                    else if (lastCell.X != cx || lastCell.Y != cy)
                    {
                        CellMouseEnter(this, new Cell(cx, cy), new MouseEventArgs(e.Button, e.Clicks, e.X - rect.Left, e.Y - rect.Top, e.Delta));
                    }
                }
            }
        }

        public void Redraw()
        {
            Invalidate();
        }
    }
}

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace WurmTimer
{
    public class LabelProgressBar : ProgressBar
    {
        //----erlaubt es auf belibigen windowseigenen Controls zu zeichnen(außer auf Textboxen)----------

        //Interop Krams:
        [DllImport("user32.dll")]
        static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [StructLayout(LayoutKind.Sequential)]
        struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rectangle ToRectangle()
            { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }
            public static implicit operator Rectangle(RECT rect)
            {
                return rect.ToRectangle();
            }
        }

        [DllImport("user32.dll")]
        static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

        const int WM_PAINT = 15;
        const int WM_ERASEBKGND = 20;
        Bitmap buffer;
        protected override void WndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_ERASEBKGND:
                case WM_PAINT:
                    if (Width == 0 || Height == 0)
                        return;

                    //Buffer vorbereiten:
                    if (buffer == null)
                    {
                        buffer = new Bitmap(Width, Height);
                    }
                    if (buffer.Width != Width || buffer.Height != Height)
                    {
                        buffer.Dispose();//!!!
                        buffer = new Bitmap(Width, Height);
                    }
                    //Hdc besorgen:
                    IntPtr hdc = m.WParam;
                    PAINTSTRUCT ps=new PAINTSTRUCT();
                    bool callEndPaint = false ;
                    Rectangle drawingRegion;
                    if (hdc == IntPtr.Zero)
                    {
                        hdc = BeginPaint(Handle, out ps);
                        callEndPaint = true;
                        drawingRegion=ps.rcPaint;
                    }
                    else
                    {
                        drawingRegion=ClientRectangle;
                    }
                    if (hdc == IntPtr.Zero)
                    { return; }
                    //auf Buffer zeichnen:
                    using (Graphics ownGx = Graphics.FromImage(buffer))
                    {
                        IntPtr ownHdc=ownGx.GetHdc();
                        Message newM = new Message();
                        newM.Msg=m.Msg;
                        newM.HWnd = Handle;
                        newM.WParam=ownHdc;
                        newM.LParam=m.LParam;
                        DefWndProc(ref newM);
                        ownGx.ReleaseHdc(ownHdc);
                        //man kann hier den Buffer beliebig manipulieren.
                        if (m.Msg == WM_PAINT) OnPaint(new PaintEventArgs(ownGx, drawingRegion));
                        else OnPaintBackground(new PaintEventArgs(ownGx, drawingRegion));
                    }
                    //Buffer zeichnen:
                    if(m.Msg==WM_PAINT)
                    {
                        using (Graphics gx = Graphics.FromHdc(hdc))
                        {
                            gx.DrawImage(buffer, drawingRegion, drawingRegion, GraphicsUnit.Pixel);
                        }
                    }
                    //Aufräumen:
                    if (callEndPaint)
                    { EndPaint(Handle, ref ps); }
                    m.Result = IntPtr.Zero;
                    return;
                default:
                    base.WndProc(ref m);
                    return;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SizeF TextSize=e.Graphics.MeasureString(Text, Font);
            int cutoff = Width * Value / Maximum;
            Rectangle rect1 = new Rectangle(0, 0, cutoff, Height);
            Rectangle rect2 = new Rectangle(cutoff, 0, Width - cutoff, Height);
            e.Graphics.SetClip(rect1);
            e.Graphics.DrawString(Text, Font, new SolidBrush(LabelColor2), (Width - TextSize.Width) / 2, (Height - TextSize.Height) / 2);
            e.Graphics.SetClip(rect2);
            e.Graphics.DrawString(Text, Font, new SolidBrush(LabelColor), (Width - TextSize.Width) / 2, (Height - TextSize.Height) / 2);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (value != base.Text)
                {
                    base.Text = value;
                    Invalidate();
                }
            }
        }


        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        private Color labelColor = SystemColors.WindowText;
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public Color LabelColor
        {
            get
            {
                return labelColor;
            }
            set
            {
                labelColor = value;
                Invalidate();
            }
        }

        private Color labelColor2 = SystemColors.ActiveCaptionText;
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public Color LabelColor2
        {
            get
            {
                return labelColor2;
            }
            set
            {
                labelColor2 = value;
                Invalidate();
            }
        }


    }
}
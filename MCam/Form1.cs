using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace MCam
{


    public partial class Form1 : Form
    {
        #region:::::::::::::::::::::::::::::::::::::::::::Form level declarations:::::::::::::::::::::::::::::::::::::::::::

        public enum CursPos : int
        {

            WithinSelectionArea = 0,
            OutsideSelectionArea,
            TopLine,
            BottomLine,
            LeftLine,
            RightLine,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight

        }

        public enum ClickAction : int
        {

            NoClick = 0,
            Dragging,
            Outside,
            TopSizing,
            BottomSizing,
            LeftSizing,
            TopLeftSizing,
            BottomLeftSizing,
            RightSizing,
            TopRightSizing,
            BottomRightSizing

        }

        public ClickAction CurrentAction;
        public bool LeftButtonDown = false;
        public bool RectangleDrawn = false;
        public bool ReadyToDrag = false;

        public Point ClickPoint = new Point();
        public Point CurrentTopLeft = new Point();
        public Point CurrentBottomRight = new Point();
        public Point DragClickRelative = new Point();

        public int RectangleHeight = new int();
        public int RectangleWidth = new int();

        public int monwidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        public int monheight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        public int monX = System.Windows.Forms.Screen.PrimaryScreen.Bounds.X;
        public int monY = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y;

        Graphics g;
        Pen MyPen = new Pen(Color.FromArgb(255, 255, 255), 5);
        SolidBrush TransparentBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
        Pen EraserPen = new Pen(Color.FromArgb(255, 255, 255), 5);
        SolidBrush eraserBrush = new SolidBrush(Color.FromArgb(10, 10, 10));

        protected override void OnMouseClick(MouseEventArgs e)
        {

            base.OnMouseClick(e);

        }

        private Form m_InstanceRef = null;
        public Form InstanceRef
        {
            get
            {

                return m_InstanceRef;

            }
            set
            {

                m_InstanceRef = value;

            }
        }

        #endregion

        #region:::::::::::::::::::::::::::::::::::::::::::Mouse Event Handlers & Drawing Initialization:::::::::::::::::::::::::::::::::::::::::::
        public Form1()
        {

            InitializeComponent();
            this.MouseDown += new MouseEventHandler(mouse_Click);
            this.MouseDoubleClick += new MouseEventHandler(mouse_DClick);
            this.MouseUp += new MouseEventHandler(mouse_Up);
            this.MouseMove += new MouseEventHandler(mouse_Move);
            g = this.CreateGraphics();

        }
        #endregion
        

        #region:::::::::::::::::::::::::::::::::::::::::::Mouse Buttons:::::::::::::::::::::::::::::::::::::::::::
        public static string ffOffX;
        public static string ffOffY;
        public static string ffWidth;
        public static string ffHeight;
        private void mouse_DClick(object sender, MouseEventArgs e)
        {

            if (RectangleDrawn && (CursorPosition() == CursPos.WithinSelectionArea || CursorPosition() == CursPos.OutsideSelectionArea))
            {
                
                ffOffX = Convert.ToString(CurrentTopLeft.X);
                ffOffY = Convert.ToString(CurrentTopLeft.Y);
                ffWidth = Convert.ToString(RectangleWidth);
                ffHeight = Convert.ToString(RectangleHeight);
                this.InstanceRef.Show();
                this.Close();

            }

        }

        private void mouse_Click(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                SetClickAction();
                LeftButtonDown = true;
                ClickPoint = new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);

                if (RectangleDrawn)
                {

                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;
                    DragClickRelative.X = Cursor.Position.X - CurrentTopLeft.X;
                    DragClickRelative.Y = Cursor.Position.Y - CurrentTopLeft.Y;

                }

            }
        }

        private void mouse_Up(object sender, MouseEventArgs e)
        {

            RectangleDrawn = true;
            LeftButtonDown = false;
            CurrentAction = ClickAction.NoClick;

        }
        #endregion



        private void mouse_Move(object sender, MouseEventArgs e)
        {

            if (LeftButtonDown && !RectangleDrawn)
            {

                DrawSelection();

            }

            if (RectangleDrawn)
            {

                CursorPosition();
                if (LeftButtonDown && CursorPosition() == CursPos.OutsideSelectionArea) { DrawSelection(); }

                if (CurrentAction == ClickAction.Dragging)
                {

                    DragSelection();

                }

                if (CurrentAction != ClickAction.Dragging && CurrentAction != ClickAction.Outside)
                {

                    ResizeSelection();

                }

            }

        }



        private CursPos CursorPosition()
        {
            if (((Cursor.Position.X > CurrentTopLeft.X - 10 && Cursor.Position.X < CurrentTopLeft.X + 10)) && ((Cursor.Position.Y > CurrentTopLeft.Y + 10) && (Cursor.Position.Y < CurrentBottomRight.Y - 10)))
            {

                this.Cursor = Cursors.SizeWE;
                return CursPos.LeftLine;

            }
            if (((Cursor.Position.X >= CurrentTopLeft.X - 10 && Cursor.Position.X <= CurrentTopLeft.X + 10)) && ((Cursor.Position.Y >= CurrentTopLeft.Y - 10) && (Cursor.Position.Y <= CurrentTopLeft.Y + 10)))
            {

                this.Cursor = Cursors.SizeNWSE;
                return CursPos.TopLeft;

            }
            if (((Cursor.Position.X >= CurrentTopLeft.X - 10 && Cursor.Position.X <= CurrentTopLeft.X + 10)) && ((Cursor.Position.Y >= CurrentBottomRight.Y - 10) && (Cursor.Position.Y <= CurrentBottomRight.Y + 10)))
            {

                this.Cursor = Cursors.SizeNESW;
                return CursPos.BottomLeft;

            }
            if (((Cursor.Position.X > CurrentBottomRight.X - 10 && Cursor.Position.X < CurrentBottomRight.X + 10)) && ((Cursor.Position.Y > CurrentTopLeft.Y + 10) && (Cursor.Position.Y < CurrentBottomRight.Y - 10)))
            {

                this.Cursor = Cursors.SizeWE;
                return CursPos.RightLine;

            }
            if (((Cursor.Position.X >= CurrentBottomRight.X - 10 && Cursor.Position.X <= CurrentBottomRight.X + 10)) && ((Cursor.Position.Y >= CurrentTopLeft.Y - 10) && (Cursor.Position.Y <= CurrentTopLeft.Y + 10)))
            {

                this.Cursor = Cursors.SizeNESW;
                return CursPos.TopRight;

            }
            if (((Cursor.Position.X >= CurrentBottomRight.X - 10 && Cursor.Position.X <= CurrentBottomRight.X + 10)) && ((Cursor.Position.Y >= CurrentBottomRight.Y - 10) && (Cursor.Position.Y <= CurrentBottomRight.Y + 10)))
            {

                this.Cursor = Cursors.SizeNWSE;
                return CursPos.BottomRight;

            }
            if (((Cursor.Position.Y > CurrentTopLeft.Y - 10) && (Cursor.Position.Y < CurrentTopLeft.Y + 10)) && ((Cursor.Position.X > CurrentTopLeft.X + 10 && Cursor.Position.X < CurrentBottomRight.X - 10)))
            {

                this.Cursor = Cursors.SizeNS;
                return CursPos.TopLine;

            }
            if (((Cursor.Position.Y > CurrentBottomRight.Y - 10) && (Cursor.Position.Y < CurrentBottomRight.Y + 10)) && ((Cursor.Position.X > CurrentTopLeft.X + 10 && Cursor.Position.X < CurrentBottomRight.X - 10)))
            {

                this.Cursor = Cursors.SizeNS;
                return CursPos.BottomLine;

            }
            if (
                (Cursor.Position.X >= CurrentTopLeft.X + 10 && Cursor.Position.X <= CurrentBottomRight.X - 10) && (Cursor.Position.Y >= CurrentTopLeft.Y + 10 && Cursor.Position.Y <= CurrentBottomRight.Y - 10))
            {

                this.Cursor = Cursors.Hand;
                return CursPos.WithinSelectionArea;

            }

            this.Cursor = Cursors.Arrow;
            return CursPos.OutsideSelectionArea;
        }

        private void SetClickAction()
        {

            switch (CursorPosition())
            {
                case CursPos.BottomLine:
                    CurrentAction = ClickAction.BottomSizing;
                    break;
                case CursPos.TopLine:
                    CurrentAction = ClickAction.TopSizing;
                    break;
                case CursPos.LeftLine:
                    CurrentAction = ClickAction.LeftSizing;
                    break;
                case CursPos.TopLeft:
                    CurrentAction = ClickAction.TopLeftSizing;
                    break;
                case CursPos.BottomLeft:
                    CurrentAction = ClickAction.BottomLeftSizing;
                    break;
                case CursPos.RightLine:
                    CurrentAction = ClickAction.RightSizing;
                    break;
                case CursPos.TopRight:
                    CurrentAction = ClickAction.TopRightSizing;
                    break;
                case CursPos.BottomRight:
                    CurrentAction = ClickAction.BottomRightSizing;
                    break;
                case CursPos.WithinSelectionArea:
                    CurrentAction = ClickAction.Dragging;
                    break;
                case CursPos.OutsideSelectionArea:
                    CurrentAction = ClickAction.Outside;
                    break;
            }

        }
        public Region rgn()
        {
            var rgn = new Region(new Rectangle(monX, monY, monwidth, monheight));
            return rgn;
        }


        private void ResizeSelection()
        {

            if (CurrentAction == ClickAction.LeftSizing)
            {

                if (Cursor.Position.X < CurrentBottomRight.X - 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentTopLeft.X = Cursor.Position.X;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;

                }

            }
            if (CurrentAction == ClickAction.TopLeftSizing)
            {

                if (Cursor.Position.X < CurrentBottomRight.X - 10 && Cursor.Position.Y < CurrentBottomRight.Y - 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentTopLeft.X = Cursor.Position.X;
                    CurrentTopLeft.Y = Cursor.Position.Y;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;
                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;

                }
            }
            if (CurrentAction == ClickAction.BottomLeftSizing)
            {

                if (Cursor.Position.X < CurrentBottomRight.X - 10 && Cursor.Position.Y > CurrentTopLeft.Y + 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentTopLeft.X = Cursor.Position.X;
                    CurrentBottomRight.Y = Cursor.Position.Y;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;
                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;

                }

            }
            if (CurrentAction == ClickAction.RightSizing)
            {

                if (Cursor.Position.X > CurrentTopLeft.X + 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentBottomRight.X = Cursor.Position.X;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;

                }
            }
            if (CurrentAction == ClickAction.TopRightSizing)
            {

                if (Cursor.Position.X > CurrentTopLeft.X + 10 && Cursor.Position.Y < CurrentBottomRight.Y - 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentBottomRight.X = Cursor.Position.X;
                    CurrentTopLeft.Y = Cursor.Position.Y;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;
                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;

                }
            }
            if (CurrentAction == ClickAction.BottomRightSizing)
            {

                if (Cursor.Position.X > CurrentTopLeft.X + 10 && Cursor.Position.Y > CurrentTopLeft.Y + 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentBottomRight.X = Cursor.Position.X;
                    CurrentBottomRight.Y = Cursor.Position.Y;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;
                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;

                }
            }
            if (CurrentAction == ClickAction.TopSizing)
            {

                if (Cursor.Position.Y < CurrentBottomRight.Y - 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentTopLeft.Y = Cursor.Position.Y;
                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;

                }
            }
            if (CurrentAction == ClickAction.BottomSizing)
            {

                if (Cursor.Position.Y > CurrentTopLeft.Y + 10)
                {

                    //Erase the previous rectangle
                    var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
                    var path = new GraphicsPath(); path.AddRectangle(Rect);
                    var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);
                    CurrentBottomRight.Y = Cursor.Position.Y;
                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;

                }

            }

        }

        private void DragSelection()
        {
            //Ensure that the rectangle stays within the bounds of the screen

            //Erase the previous rectangle
            var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
            var path = new GraphicsPath(); path.AddRectangle(Rect);
            var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);


            if (Cursor.Position.X - DragClickRelative.X > 0 && Cursor.Position.X - DragClickRelative.X + RectangleWidth < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width)
            {

                CurrentTopLeft.X = Cursor.Position.X - DragClickRelative.X;
                CurrentBottomRight.X = CurrentTopLeft.X + RectangleWidth;

            }
            else
                //Selection area has reached the right side of the screen
                if (Cursor.Position.X - DragClickRelative.X > 0)
            {

                CurrentTopLeft.X = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - RectangleWidth;
                CurrentBottomRight.X = CurrentTopLeft.X + RectangleWidth;

            }
            //Selection area has reached the left side of the screen
            else
            {

                CurrentTopLeft.X = 0;
                CurrentBottomRight.X = CurrentTopLeft.X + RectangleWidth;

            }

            if (Cursor.Position.Y - DragClickRelative.Y > 0 && Cursor.Position.Y - DragClickRelative.Y + RectangleHeight < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
            {

                CurrentTopLeft.Y = Cursor.Position.Y - DragClickRelative.Y;
                CurrentBottomRight.Y = CurrentTopLeft.Y + RectangleHeight;

            }
            else
                //Selection area has reached the bottom of the screen
                if (Cursor.Position.Y - DragClickRelative.Y > 0)
            {

                CurrentTopLeft.Y = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - RectangleHeight;
                CurrentBottomRight.Y = CurrentTopLeft.Y + RectangleHeight;

            }
            //Selection area has reached the top of the screen
            else
            {

                CurrentTopLeft.Y = 0;
                CurrentBottomRight.Y = CurrentTopLeft.Y + RectangleHeight;

            }

        }

        private void DrawSelection()
        {

            this.Cursor = Cursors.Arrow;

            //Erase the previous rectangle
            var Rect = new Rectangle(CurrentTopLeft.X, CurrentTopLeft.Y, CurrentBottomRight.X - CurrentTopLeft.X, CurrentBottomRight.Y - CurrentTopLeft.Y);
            var path = new GraphicsPath(); path.AddRectangle(Rect);
            var wa = new Region(); wa = rgn(); wa.Exclude(path); g.FillRegion(eraserBrush, wa); g.FillRectangle(TransparentBrush, Rect);

            //Calculate X Coordinates
            if (Cursor.Position.X < ClickPoint.X)
            {

                CurrentTopLeft.X = Cursor.Position.X;
                CurrentBottomRight.X = ClickPoint.X;

            }
            else
            {

                CurrentTopLeft.X = ClickPoint.X;
                CurrentBottomRight.X = Cursor.Position.X;

            }

            //Calculate Y Coordinates
            if (Cursor.Position.Y < ClickPoint.Y)
            {

                CurrentTopLeft.Y = Cursor.Position.Y;
                CurrentBottomRight.Y = ClickPoint.Y;

            }
            else
            {

                CurrentTopLeft.Y = ClickPoint.Y;
                CurrentBottomRight.Y = Cursor.Position.Y;

            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
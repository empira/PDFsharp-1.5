#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2017 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

// Draw crosses to check layout calculation
#define DRAW_X_

#if DEBUG
// Test drawing in a bitmap. This is just a hack - don't use it!
#define DRAW_BMP_
#endif

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PdfSharp.Drawing;

#if !GDI
#error This file must only be included in GDI build.
#endif

namespace PdfSharp.Forms
{
    /* TODOs
     * 
     *  o Call render event only once. -> introduce an UpdatePage() function
     * 
     * Further stuff: set printable area; set text box (smallest rect that contains all content)
    */
    /// <summary>
    /// Represents a preview control for an XGraphics page. Can be used as an alternative to
    /// System.Windows.Forms.PrintPreviewControl.
    /// </summary>
    public class PagePreview : UserControl
    {
        /// <summary>
        /// A delegate for invoking the render function.
        /// </summary>
        public delegate void RenderEvent(XGraphics gfx);

        private Container components = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagePreview"/> class.
        /// </summary>
        public PagePreview()
        {
            _canvas = new PagePreviewCanvas(this);
            Controls.Add(_canvas);

            _hScrollBar = new HScrollBar();
            _hScrollBar.Visible = _showScrollbars;
            _hScrollBar.Scroll += OnScroll;
            _hScrollBar.ValueChanged += OnValueChanged;
            Controls.Add(_hScrollBar);

            _vScrollBar = new VScrollBar();
            _vScrollBar.Visible = _showScrollbars;
            _vScrollBar.Scroll += OnScroll;
            _vScrollBar.ValueChanged += OnValueChanged;
            Controls.Add(_vScrollBar);

            InitializeComponent();
            //OnLayout();

            _zoom = Zoom.FullPage;
            _printableArea = new RectangleF();
            //virtPageSize = new Size();
            //showNonPrintableArea = false;
            //virtualPrintableArea = new Rectangle();

            _printableArea.GetType();
            //showNonPrintableArea.GetType();
            //virtualPrintableArea.GetType();

            // Prevent bogus compiler warnings
            _posOffset = new Point();
            _virtualPage = new Rectangle();
        }

        readonly PagePreviewCanvas _canvas;
        readonly HScrollBar _hScrollBar;
        readonly VScrollBar _vScrollBar;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets the border style of the control.
        /// </summary>
        /// <value></value>
        [DefaultValue((int)BorderStyle.Fixed3D), Description("Determines the style of the border."), Category("Preview Properties")]
        public new BorderStyle BorderStyle
        {
            get { return _borderStyle; }
            set
            {
                if (!Enum.IsDefined(typeof(BorderStyle), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));

                if (value != _borderStyle)
                {
                    _borderStyle = value;
                    LayoutChildren();
                }
            }
        }
        BorderStyle _borderStyle = BorderStyle.Fixed3D;

        //    [DefaultValue(2), Description("TODO..."), Category("Preview Properties")]
        //    public PageSize PageSize
        //    {
        //      get { return pageSize2; }
        //      set
        //      {
        //        if (!Enum.IsDefined(typeof(PageSize), value))
        //          throw new InvalidEnumArgumentException("value", (int)value, typeof(PageSize));
        //
        //        if (value != pageSize2)
        //        {
        //          pageSize2 = value;
        //          //          base.RecreateHandle();
        //          //          integralHeightAdjust = true;
        //          //          try
        //          //          {
        //          //            base.Height = requestedHeight;
        //          //          }
        //          //          finally
        //          //          {
        //          //            integralHeightAdjust = false;
        //          //          }
        //        }
        //      }
        //    }
        //    PageSize pageSize2;

        /// <summary>
        /// Gets or sets the XGraphicsUnit of the page.
        /// The default value is XGraphicsUnit.Point.
        /// </summary>
        public XGraphicsUnit PageGraphicsUnit
        {
            get { return _pageGraphicsUnit; }
            set { _pageGraphicsUnit = value; }
        }
        XGraphicsUnit _pageGraphicsUnit = XGraphicsUnit.Point;

        /// <summary>
        /// This property was renamed. Use new property PageGraphicsUnit.
        /// </summary>
        [Obsolete("Property renamed, use PageGraphicsUnit")]
        public XGraphicsUnit PageUnit
        {
            get { return _pageGraphicsUnit; }
            set { _pageGraphicsUnit = value; }
        }

        /// <summary>
        /// Gets or sets a predefined zoom factor.
        /// </summary>
        [DefaultValue((int)Zoom.FullPage), Description("Determines the zoom of the page."), Category("Preview Properties")]
        public Zoom Zoom
        {
            get { return _zoom; }
            set
            {
                if ((int)value < (int)Zoom.Mininum || (int)value > (int)Zoom.Maximum)
                {
                    if (!Enum.IsDefined(typeof(Zoom), value))
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(Zoom));
                }
                if (value != _zoom)
                {
                    _zoom = value;
                    CalculatePreviewDimension();
                    SetScrollBarRange();
                    _canvas.Invalidate();
                }
            }
        }
        Zoom _zoom;

        /// <summary>
        /// Gets or sets an arbitrary zoom factor. The range is from 10 to 800.
        /// </summary>
        //[DefaultValue((int)Zoom.FullPage), Description("Determines the zoom of the page."), Category("Preview Properties")]
        public int ZoomPercent
        {
            get { return _zoomPercent; }
            set
            {
                if (value < (int)Zoom.Mininum || value > (int)Zoom.Maximum)
                    throw new ArgumentOutOfRangeException("value", value,
                      String.Format("Value must between {0} and {1}.", (int)Zoom.Mininum, (int)Zoom.Maximum));

                if (value != _zoomPercent)
                {
                    _zoom = (Zoom)value;
                    _zoomPercent = value;
                    CalculatePreviewDimension();
                    SetScrollBarRange();
                    _canvas.Invalidate();
                }
            }
        }
        int _zoomPercent;

        /// <summary>
        /// Gets or sets the color of the page.
        /// </summary>
        [Description("The background color of the page."), Category("Preview Properties")]
        public Color PageColor
        {
            get { return _pageColor; }
            set
            {
                if (value != _pageColor)
                {
                    _pageColor = value;
                    Invalidate();
                }
            }
        }
        Color _pageColor = Color.GhostWhite;

        /// <summary>
        /// Gets or sets the color of the desktop.
        /// </summary>
        [Description("The color of the desktop."), Category("Preview Properties")]
        public Color DesktopColor
        {
            get { return _desktopColor; }
            set
            {
                if (value != _desktopColor)
                {
                    _desktopColor = value;
                    Invalidate();
                }
            }
        }
        internal Color _desktopColor = SystemColors.ControlDark;

        /// <summary>
        /// Gets or sets a value indicating whether the scrollbars are visible.
        /// </summary>
        [DefaultValue(true), Description("Determines whether the scrollbars are visible."), Category("Preview Properties")]
        public bool ShowScrollbars
        {
            get { return _showScrollbars; }
            set
            {
                if (value != _showScrollbars)
                {
                    _showScrollbars = value;
                    _hScrollBar.Visible = value;
                    _vScrollBar.Visible = value;
                    LayoutChildren();
                }
            }
        }
        bool _showScrollbars = true;

        /// <summary>
        /// Gets or sets a value indicating whether the page is visible.
        /// </summary>
        [DefaultValue(true), Description("Determines whether the page visible."), Category("Preview Properties")]
        public bool ShowPage
        {
            get { return _showPage; }
            set
            {
                if (value != _showPage)
                {
                    _showPage = value;
                    _canvas.Invalidate();
                }
            }
        }
        internal bool _showPage = true;

        /// <summary>
        /// Gets or sets the page size in point.
        /// </summary>
        [Description("Determines the size (in points) of the page."), Category("Preview Properties")]
        public XSize PageSize
        {
            get { return new XSize((int)_pageSize.Width, (int)_pageSize.Height); }
            set
            {
                _pageSize = new SizeF((float)value.Width, (float)value.Height);
                CalculatePreviewDimension();
                Invalidate();
            }
        }

        /// <summary>
        /// This is a hack for Visual Studio 2008. The designer uses reflection for setting the PageSize property.
        /// This fails, even an implicit operator that converts Size to XSize exits.
        /// </summary>
        public Size PageSizeF
        {
            get { return new Size(Convert.ToInt32(_pageSize.Width), Convert.ToInt32(_pageSize.Height)); }
            set
            {
                _pageSize = value;
                CalculatePreviewDimension();
                Invalidate();
            }
        }

        /// <summary>
        /// Sets a delegate that is invoked when the preview wants to be painted.
        /// </summary>
        public void SetRenderFunction(Action<XGraphics> renderEvent)
        {
            _renderAction = renderEvent;
            Invalidate();
        }
        Action<XGraphics> _renderAction;

        /// <summary>
        /// Sets a delegate that is invoked when the preview wants to be painted.
        /// </summary>
        [Obsolete("Use SetRenderFunction")]
        public void SetRenderEvent(RenderEvent renderEvent)
        {
            _renderAction = new Action<XGraphics>(renderEvent);
            Invalidate();
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Name = "PagePreview";
            Size = new System.Drawing.Size(228, 252);
        }
        #endregion

        /// <summary>
        /// Raises the ZoomChanged event when the zoom factor changed.
        /// </summary>
        protected virtual void OnZoomChanged(EventArgs e)
        {
            if (ZoomChanged != null)
                ZoomChanged(this, e);
        }

        /// <summary>
        /// Occurs when the zoom factor changed.
        /// </summary>
        public event EventHandler ZoomChanged;

        /// <summary>
        /// Paints the background with the sheet of paper.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Accurate drawing prevents flickering
            Graphics gfx = e.Graphics;
            Rectangle clientRect = ClientRectangle;
            int d = 0;
            switch (_borderStyle)
            {
                case BorderStyle.FixedSingle:
                    gfx.DrawRectangle(SystemPens.WindowFrame, clientRect.X, clientRect.Y, clientRect.Width - 1, clientRect.Height - 1);
                    d = 1;
                    break;

                case BorderStyle.Fixed3D:
                    ControlPaint.DrawBorder3D(gfx, clientRect, Border3DStyle.Sunken);
                    d = 2;
                    break;
            }
            if (_showScrollbars)
            {
                int cxScrollbar = SystemInformation.VerticalScrollBarWidth;
                int cyScrollbar = SystemInformation.HorizontalScrollBarHeight;

                gfx.FillRectangle(new SolidBrush(BackColor),
                  clientRect.Width - cxScrollbar - d, clientRect.Height - cyScrollbar - d, cxScrollbar, cyScrollbar);
            }
        }

        /// <summary>
        /// Recalculates the preview dimension.
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CalculatePreviewDimension();
            SetScrollBarRange();
        }

        /// <summary>
        /// Invalidates the canvas.
        /// </summary>
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            _canvas.Invalidate();
        }

        /// <summary>
        /// Layouts the child controls.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            LayoutChildren();
        }

        void OnScroll(object obj, ScrollEventArgs e)
        {
            ScrollBar sc = obj as ScrollBar;
            if (sc != null)
            {
                //Debug.WriteLine(String.Format("OnScroll: {0}, {1}", sc.Value, e.NewValue));
            }
        }

        void OnValueChanged(object obj, EventArgs e)
        {
            ScrollBar sc = obj as ScrollBar;
            if (sc != null)
            {
                //Debug.WriteLine(String.Format("OnValueChanged: {0}", sc.Value));
                if (sc == _hScrollBar)
                    _posOffset.X = sc.Value;

                else if (sc == _vScrollBar)
                    _posOffset.Y = sc.Value;
            }
            _canvas.Invalidate();
        }

        void LayoutChildren()
        {
            Invalidate();
            Rectangle clientRect = ClientRectangle;
            switch (_borderStyle)
            {
                case BorderStyle.FixedSingle:
                    clientRect.Inflate(-1, -1);
                    break;

                case BorderStyle.Fixed3D:
                    clientRect.Inflate(-2, -2);
                    break;
            }
            int x = clientRect.X;
            int y = clientRect.Y;
            int cx = clientRect.Width;
            int cy = clientRect.Height;
            int cxScrollbar = 0;
            int cyScrollbar = 0;
            if (_showScrollbars && _vScrollBar != null && _hScrollBar != null)
            {
                cxScrollbar = _vScrollBar.Width;
                cyScrollbar = _hScrollBar.Height;
                _vScrollBar.Location = new Point(x + cx - cxScrollbar, y);
                _vScrollBar.Size = new Size(cxScrollbar, cy - cyScrollbar);
                _hScrollBar.Location = new Point(x, y + cy - cyScrollbar);
                _hScrollBar.Size = new Size(cx - cxScrollbar, cyScrollbar);
            }
            if (_canvas != null)
            {
                _canvas.Location = new Point(x, y);
                _canvas.Size = new Size(cx - cxScrollbar, cy - cyScrollbar);
            }
        }

        /// <summary>
        /// Calculates all values for drawing the page preview.
        /// </summary>
        internal void CalculatePreviewDimension(out bool zoomChanged)
        {
            // User may change display resolution while preview is running
            Graphics gfx = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hdc = gfx.GetHdc();
            DeviceInfos devInfo = DeviceInfos.GetInfos(hdc);
            gfx.ReleaseHdc(hdc);
            gfx.Dispose();
            int xdpiScreen = devInfo.LogicalDpiX;
            int ydpiScreen = devInfo.LogicalDpiY;
            //int cxScrollbar = SystemInformation.VerticalScrollBarWidth;
            //int cyScrollbar = SystemInformation.HorizontalScrollBarHeight;
            Rectangle rcCanvas = _canvas.ClientRectangle;

            Zoom zoomOld = _zoom;
            int zoomPercentOld = _zoomPercent;

            // Border around virtual page in pixel.
            const int leftBorder = 2;
            const int rightBorder = 4;  // because of shadow
            const int topBorder = 2;
            const int bottomBorder = 4;  // because of shadow
            const int horzBorders = leftBorder + rightBorder;
            const int vertBorders = topBorder + bottomBorder;

            // Calculate new zoom factor.
            switch (_zoom)
            {
                case Zoom.BestFit:
                BestFit:
                    //zoomPercent = Convert.ToInt32(25400.0 * (rcCanvas.Width - (leftBorder + rightBorder)) / (this.pageSize.Width * xdpiScreen));
                    _zoomPercent = (int)(7200f * (rcCanvas.Width - horzBorders) / (_pageSize.Width * xdpiScreen));
                    //--zoomPercent;  // prevent round up errors
                    break;

                case Zoom.TextFit:
                    // TODO: 'public Rectangle TextBox' property
                    goto BestFit;
                //zoomPercent = LongFromReal (25400.0 / (_cxUsedPage + 0) * 
                //                            (rcWnd.CX () - 2 * cxScrollbar) / xdpiScreen) - 3;
                //break;

                case Zoom.FullPage:
                    {
                        //int zoomX = Convert.ToInt32(25400.0 / (pageSize.Width) *
                        //  (rcCanvas.Width - (leftBorder + rightBorder)) / xdpiScreen);
                        //int zoomY = Convert.ToInt32(25400.0 / (pageSize.Height) *
                        //  (rcCanvas.Height - (topBorder + bottomBorder)) / ydpiScreen);
                        int zoomX = (int)(7200f * (rcCanvas.Width - horzBorders) / (_pageSize.Width * xdpiScreen));
                        int zoomY = (int)(7200f * (rcCanvas.Height - vertBorders) / (_pageSize.Height * ydpiScreen));
                        _zoomPercent = Math.Min(zoomX, zoomY);
                        //--zoomPercent;  // prevent round up errors
                    }
                    break;

                case Zoom.OriginalSize:
                    _zoomPercent = (int)(0.5 + 200f / (devInfo.ScaleX + devInfo.ScaleY));
                    _zoomPercent = (int)(0.5 + 100f / devInfo.ScaleX);
                    break;

                default:
                    _zoomPercent = (int)_zoom;
                    break;
            }

            // Bound to zoom limits
            _zoomPercent = Math.Max(Math.Min(_zoomPercent, (int)Zoom.Maximum), (int)Zoom.Mininum);
            if ((int)_zoom > 0)
                _zoom = (Zoom)_zoomPercent;

            // Size of page in preview window in pixel
            _virtualPage.X = leftBorder;
            _virtualPage.Y = topBorder;
            _virtualPage.Width = (int)(_pageSize.Width * xdpiScreen * _zoomPercent / 7200);
            _virtualPage.Height = (int)(_pageSize.Height * ydpiScreen * _zoomPercent / 7200);

            //// 2540 := (25.4mm * 100%) / 1mm
            //m_VirtualPrintableArea.X      = (int)printableArea.X * this.zoomPercent * xdpiScreen / 2540;
            //m_VirtualPrintableArea.Y      = (int)printableArea.Y * this.zoomPercent * xdpiScreen / 2540;
            //m_VirtualPrintableArea.Width  = (int)printableArea.Width  * this.zoomPercent * xdpiScreen / 2540;
            //m_VirtualPrintableArea.Height = (int)printableArea.Height * this.zoomPercent * xdpiScreen / 2540;

            // Border do not depend on zoom anymore
            _virtualCanvas = new Size(_virtualPage.Width + horzBorders, _virtualPage.Height + vertBorders);

            // Adjust virtual canvas to at least actual window size
            if (_virtualCanvas.Width < rcCanvas.Width)
            {
                _virtualCanvas.Width = rcCanvas.Width;
                _virtualPage.X = leftBorder + (rcCanvas.Width - horzBorders - _virtualPage.Width) / 2;
            }
            if (_virtualCanvas.Height < rcCanvas.Height)
            {
                _virtualCanvas.Height = rcCanvas.Height;
                _virtualPage.Y = topBorder + (rcCanvas.Height - vertBorders - _virtualPage.Height) / 2;
            }

            zoomChanged = zoomOld != _zoom || zoomPercentOld != _zoomPercent;
            if (zoomChanged)
                OnZoomChanged(new EventArgs());
        }

        internal void CalculatePreviewDimension()
        {
            bool zoomChanged;
            CalculatePreviewDimension(out zoomChanged);
        }

        internal bool RenderPage(Graphics gfx)
        {
            //delete m_RenderContext;
            //m_RenderContext = new HdcRenderContext(wdc.m_hdc);

            gfx.TranslateTransform(-_posOffset.X, -_posOffset.Y);
            gfx.SetClip(new Rectangle(_virtualPage.X + 1, _virtualPage.Y + 1, _virtualPage.Width - 1, _virtualPage.Height - 1));

            float scaleX = _virtualPage.Width / _pageSize.Width;
            float scaleY = _virtualPage.Height / _pageSize.Height;

            //gfx.SetSmoothingMode(SmoothingModeAntiAlias);
            //PaintBackground(gfx);

#if DRAW_BMP
            Matrix matrix = new Matrix();
            matrix.Translate(virtualPage.X, virtualPage.Y);
            matrix.Translate(-posOffset.X, -this.posOffset.Y);
            //matrix.Scale(scaleX, scaleY);
            gfx.Transform = matrix;

#if DRAW_X
            gfx.DrawLine(Pens.Red, 0, 0, pageSize.Width, pageSize.Height);
            gfx.DrawLine(Pens.Red, 0, pageSize.Height, pageSize.Width, 0);
#endif
            if (renderEvent != null)
            {
                Bitmap bmp = new Bitmap(virtualPage.Width, this.virtualPage.Height, gfx);
                Graphics gfx2 = Graphics.FromImage(bmp);
                gfx2.Clear(pageColor);
                gfx2.ScaleTransform(scaleX, scaleY);
                gfx2.SmoothingMode = SmoothingMode.HighQuality;
                XGraphics xgfx = XGraphics.FromGraphics(gfx2, new XSize(pageSize.Width, this.pageSize.Height));
                try
                {
                    renderEvent(xgfx);
                    gfx.DrawImage(bmp, 0, 0);
                }
                finally
                {
                    bmp.Dispose();
                }
            }
#else
            Matrix matrix = new Matrix();
            matrix.Translate(_virtualPage.X, _virtualPage.Y);
            matrix.Translate(-_posOffset.X, -_posOffset.Y);
            matrix.Scale(scaleX, scaleY);
            gfx.Transform = matrix;

#if DRAW_X
            gfx.DrawLine(Pens.Red, 0, 0, pageSize.Width, pageSize.Height);
            gfx.DrawLine(Pens.Red, 0, pageSize.Height, pageSize.Width, 0);
#endif

            if (_renderAction != null)
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                XGraphics xgfx = XGraphics.FromGraphics(gfx, new XSize(_pageSize.Width, _pageSize.Height), PageGraphicsUnit);
                try
                {
                    _renderAction(xgfx);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception");
                }
            }
#endif

            // Old C++ stuff, may be useful later...
#if false
      switch (m_mode)
      {
        case RenderModeDirect:
        {
          delete m_PreviewMetafile;
          m_PreviewMetafile = NULL;

          float cxPage = Metric::MillimetersToPoints(m_dimPage.cx / 10.0f);
          float cyPage = Metric::MillimetersToPoints(m_dimPage.cy / 10.0f);

          float scaleX = virtualPage.Width  / cxPage;
          float scaleY = virtualPage.Height / cyPage;

          Graphics gfx(m_RenderContext);
          gfx.SetSmoothingMode(SmoothingModeAntiAlias);
          PaintBackground(gfx, &virtualPage);

          Matrix matrix;
          matrix.Translate((float)virtualPage.X, (float)virtualPage.Y);
          matrix.Translate((float) - m_posOffset.x, (float) -m_posOffset.y);
          matrix.Scale(scaleX, scaleY);

          m_RenderContext->SetDefaultViewMatrix(&matrix);
          gfx.ResetTransform();
          if (m_PreviewRenderer && m_PreviewRenderer->CanRender())
            m_PreviewRenderer->Render(&gfx, m_Page);
        }
          break;

        case RenderModeMetafile:
        {
          Graphics gfx(m_RenderContext);
          if (m_PreviewMetafile == NULL)
          {
            float cxPage = Metric::MillimetersToPoints(m_dimPage.cx / 10.0f);
            float cyPage = Metric::MillimetersToPoints(m_dimPage.cy / 10.0f);

            //float factor = 72.0f / 96.0f;
            Rect  rcLogicalPage(0, 0, (int)cxPage, (int)cyPage);
            RectF rcFLogicalPage(0, 0, cxPage, cyPage);

            //DeviceName devname;
            //DESKTOP::QueryDefaultPrinter(devname); //HACK DRUCKER MUSS DA SEIN!
            //DeviceMode devmode(devname);

            //HDC hdc = ::CreateIC(devname.m_szDriver, devname.m_szDevice, devname.m_szOutput, devmode.m_pdm);

            //HDC hdc = m_Graphics->GetHDC();
            //HDC hdc = ::GetDC(NULL);
            HDC hdc = ::CreateIC("DISPLAY", NULL, NULL, NULL);


            float dpiX = gfx.GetDpiX();
            float dpiY = gfx.GetDpiY();

            // Even Petzold would be surprised about that...
            //                                                          Display                 | LaserJet
            //                                                     DPI   96 : 120               | 300
            // physical device size in MM                          ---------------------------------------------
            int horzSizeMM    = ::GetDeviceCaps(hdc, HORZSIZE);    // = 330 : 254               | 198 (not 210)
            int vertSizeMM    = ::GetDeviceCaps(hdc, VERTSIZE);    // = 254 : 203               | 288 (hot 297)

            // device size in pixel
            int horzSizePixel = ::GetDeviceCaps(hdc, HORZRES);     // = 1280 : 1280             | 4676
            int vertSizePixel = ::GetDeviceCaps(hdc, VERTRES);     // = 1024 : 1024             | 6814

            // 'logical' device resolution in DPI
            int logResX       = ::GetDeviceCaps(hdc, LOGPIXELSX);  // = 96 : 120                | 600
            int logResY       = ::GetDeviceCaps(hdc, LOGPIXELSY);  // = 96 : 120                | 600

            // physical pixel size in .01 MM units
            // accidentally(?) the result of GetPhysicalDimension!
            //float X1 = 100.0f * horzSizeMM / horzSizePixel;        // = 25.781250 : 19.843750   | 4.2343884
            //float Y1 = 100.0f * vertSizeMM / vertSizePixel;        // = 24.804688 : 19.824219   | 4.2265925

            // now we can get the 'physical' device resolution...
            float phyResX = horzSizePixel / (horzSizeMM / 25.4f);  // = 98.521210 : 128.00000   | 599.85052
            float phyResY = vertSizePixel / (vertSizeMM / 25.4f);  // = 102.40000 : 128.12611   | 600.95691

            // ...and rescale the size of the meta rectangle.
            float magicX = logResX / phyResX;                      // = 0.97440946 : 0.93750000 | 1.0002491
            float magicY = logResY / phyResY;                      // = 0.93750000 : 0.93657720 | 0.99840766

            // use A4 page in point
            // adjust size of A4 page so that meta file fits with DrawImage...
            RectF rcMagic(0, 0, magicX * cxPage, magicY * cyPage);
            m_PreviewMetafile = new Metafile(hdc, rcMagic, MetafileFrameUnitPoint,
              EmfTypeEmfPlusOnly, L"some description");

            SizeF size;
            float horzRes, vertRes;
            float height, width;
            MetafileHeader metafileHeader;

            // GetPhysicalDimension returns physical size of a pixel in .01 MM units!!
            m_PreviewMetafile->GetPhysicalDimension(&size);

            horzRes = (float)m_PreviewMetafile->GetHorizontalResolution();
            vertRes = (float)m_PreviewMetafile->GetVerticalResolution();
            height  = (float)m_PreviewMetafile->GetHeight();
            width   = (float)m_PreviewMetafile->GetWidth();
            m_PreviewMetafile->GetMetafileHeader(&metafileHeader);

            Graphics gfxMf(m_PreviewMetafile);
            dpiX = gfxMf.GetDpiX();
            dpiY = gfxMf.GetDpiY();
          
            m_PreviewMetafile->GetPhysicalDimension(&size);
            horzRes = (float)m_PreviewMetafile->GetHorizontalResolution();
            vertRes = (float)m_PreviewMetafile->GetVerticalResolution();
            height  = (float)m_PreviewMetafile->GetHeight();
            width   = (float)m_PreviewMetafile->GetWidth();
            m_PreviewMetafile->GetMetafileHeader(&metafileHeader);

            gfxMf.SetPageUnit(UnitPoint);
            if (m_PreviewRenderer && m_PreviewRenderer->CanRender())
              m_PreviewRenderer->Render(&gfxMf, m_Page);

            ::DeleteDC(hdc);
          }
          if (m_PreviewMetafile)
          {
            gfx.SetSmoothingMode(SmoothingModeAntiAlias);
            PaintBackground(gfx, &virtualPage);
            //Matrix matrix(1, 0, 0, 1, (float) - m_posOffset.x, (float) - m_posOffset.y);
            m_RenderContext->SetDefaultViewMatrix(&matrix);
            gfx.ResetTransform();
            gfx.DrawImage(m_PreviewMetafile, virtualPage);
          }
        }
          break;

        case RenderModeBitmap:
          break;
      }
#endif
            return true;
        }

        /// <summary>
        /// Paints the background and the empty page.
        /// </summary>
        internal void PaintBackground(Graphics gfx)
        {
            // Draw sharp paper borders and shadow.
            gfx.SmoothingMode = SmoothingMode.None;
            //gfx.SetCompositingMode(CompositingModeSourceOver); // CompositingModeSourceCopy
            //gfx.SetCompositingQuality(CompositingQualityHighQuality);

            gfx.TranslateTransform(-_posOffset.X, -_posOffset.Y);

            // Draw outer area. Use clipping to prevent flickering of page interior.
            gfx.SetClip(new Rectangle(_virtualPage.X, _virtualPage.Y, _virtualPage.Width + 3, _virtualPage.Height + 3), CombineMode.Exclude);
            gfx.SetClip(new Rectangle(_virtualPage.X + _virtualPage.Width + 1, _virtualPage.Y, 2, 2), CombineMode.Union);
            gfx.SetClip(new Rectangle(_virtualPage.X, _virtualPage.Y + _virtualPage.Height + 1, 2, 2), CombineMode.Union);
            gfx.Clear(_desktopColor);

#if DRAW_X
      gfx.DrawLine(Pens.Blue, 0, 0, virtualCanvas.Width, virtualCanvas.Height);
      gfx.DrawLine(Pens.Blue, virtualCanvas.Width, 0, 0, virtualCanvas.Height);
#endif
            gfx.ResetClip();

#if !DRAW_BMP
            // Fill page interior.
            SolidBrush brushPaper = new SolidBrush(_pageColor);
            gfx.FillRectangle(brushPaper, _virtualPage.X + 1, _virtualPage.Y + 1, _virtualPage.Width - 1, _virtualPage.Height - 1);
#endif

            //// draw non printable area
            //if (m_ShowNonPrintableArea)
            //{
            //SolidBrush brushNPA(+DESKTOP::QuerySysColor((SYSCLR_3DLIGHT)) | 0xFF000000);
            //
            //gfx.FillRectangle(&brushNPA, virtualPage.X, virtualPage.Y, virtualPage.Width, rcPrintableArea.Y - virtualPage.Y);
            //gfx.FillRectangle(&brushNPA, virtualPage.X, virtualPage.Y, rcPrintableArea.X - virtualPage.X, virtualPage.Height);
            //gfx.FillRectangle(&brushNPA, rcPrintableArea.X + rcPrintableArea.Width,
            //virtualPage.Y, virtualPage.X + virtualPage.Width - (rcPrintableArea.X + rcPrintableArea.Width), virtualPage.Height);
            //gfx.FillRectangle(&brushNPA, virtualPage.X, rcPrintableArea.Y + rcPrintableArea.Height,
            //virtualPage.Width, virtualPage.Y + virtualPage.Height - (rcPrintableArea.Y + rcPrintableArea.Height));
            //}
            //DrawDash(gfx, virtualPage);

            // Draw page border and shadow.
            Pen penPaperBorder = SystemPens.WindowText;
            Brush brushShadow = SystemBrushes.ControlDarkDark;
            gfx.DrawRectangle(penPaperBorder, _virtualPage);
            gfx.FillRectangle(brushShadow, _virtualPage.X + _virtualPage.Width + 1, _virtualPage.Y + 2, 2, _virtualPage.Height + 1);
            gfx.FillRectangle(brushShadow, _virtualPage.X + 2, _virtualPage.Y + _virtualPage.Height + 1, _virtualPage.Width + 1, 2);
        }

        /// <summary>
        /// Check clipping rectangle calculations.
        /// </summary>
        [Conditional("DEBUG")]
        void DrawDash(Graphics gfx, Rectangle rect)
        {
            Pen pen = new Pen(Color.GreenYellow, 1);
            pen.DashStyle = DashStyle.Dash;
            gfx.DrawRectangle(pen, rect);
        }

        /// <summary>
        /// Adjusts scroll bars.
        /// </summary>
        void SetScrollBarRange()
        {
            // Windows 7 issue: Must invalidate scroll bars to ensure that
            // the arrows are painted correctly.

            Rectangle clientRect = _canvas.ClientRectangle;
            Size clientAreaSize = clientRect.Size;

            // Scroll range
            int dx = _virtualCanvas.Width - clientAreaSize.Width;
            int dy = _virtualCanvas.Height - clientAreaSize.Height;

            //bool extendX = clientAreaSize.Width < virtualCanvas.Width;
            //bool extendY = clientAreaSize.Height < virtualCanvas.Height;

            if (ShowScrollbars && _hScrollBar != null)
            {
                if (_posOffset.X > dx)
                    _hScrollBar.Value = _posOffset.X = dx;

                if (dx > 0)
                {
                    _hScrollBar.Minimum = 0;
                    _hScrollBar.Maximum = _virtualCanvas.Width;
                    _hScrollBar.SmallChange = clientAreaSize.Width / 10;
                    _hScrollBar.LargeChange = clientAreaSize.Width;
                    _hScrollBar.Enabled = true;
                }
                else
                {
                    _hScrollBar.Minimum = 0;
                    _hScrollBar.Maximum = 0;
                    _hScrollBar.Enabled = false;
                }
                _hScrollBar.Invalidate();
            }

            if (ShowScrollbars && _vScrollBar != null)
            {
                if (_posOffset.Y > dy)
                    _vScrollBar.Value = _posOffset.Y = dy;

                if (dy > 0)
                {
                    _vScrollBar.Minimum = 0;
                    _vScrollBar.Maximum = _virtualCanvas.Height;
                    _vScrollBar.SmallChange = clientAreaSize.Height / 10;
                    _vScrollBar.LargeChange = clientAreaSize.Height;
                    _vScrollBar.Enabled = true;
                }
                else
                {
                    _vScrollBar.Minimum = 0;
                    _vScrollBar.Maximum = 0;
                    _vScrollBar.Enabled = false;
                }
                _vScrollBar.Invalidate();
            }
        }

        ///// <summary>
        ///// Calculates two interesting values...
        ///// </summary>
        //public static void GetMagicValues(IntPtr hdc, out float magicX, out float magicY)
        //{
        //  // Even Petzold would be surprised about that...

        //  // Physical device size in MM
        //  int horzSizeMM = GetDeviceCaps(hdc, HORZSIZE);
        //  int vertSizeMM = GetDeviceCaps(hdc, VERTSIZE);
        //  //
        //  // Display size in pixels                        1600 x 1200                    1280 x 1024
        //  //
        //  // My old Sony display with  96 DPI:                 ---                         330 x 254
        //  // My old Sony display with 120 DPI:                 ---                         254 x 203
        //  // My current Sony display with  96 DPI:          410 x 310                      410 x 310
        //  // My current Sony display with 120 DPI:          410 x 310                      410 x 310
        //  // My old Sony display with  96 DPI:                 ---                         360 x 290
        //  // My old Sony display with 120 DPI:                 ---                         360 x 290
        //  // My LaserJet 6L (300 DPI):                           198 (not 210) x 288 (nscot 297)


        //  // Device size in pixel
        //  int horzSizePixel = GetDeviceCaps(hdc, HORZRES);
        //  int vertSizePixel = GetDeviceCaps(hdc, VERTRES);
        //  //
        //  // Display size in pixels                        1600 x 1200                    1280 x 1024
        //  //
        //  // My old Sony display with  96 DPI:                 ---                        1280 x 1024
        //  // My old Sony display with 120 DPI:                 ---                        1280 x 1024
        //  // My current Sony display with  96 DPI:         1600 x 1200                    1280 x 1024
        //  // My current Sony display with 120 DPI:         1600 x 1200                    1280 x 1024
        //  //
        //  // My LaserJet 6L (600 DPI):                                    4676 x 6814

        //  // 'logical' device resolution in DPI
        //  int logResX = GetDeviceCaps(hdc, LOGPIXELSX);
        //  int logResY = GetDeviceCaps(hdc, LOGPIXELSY);
        //  //
        //  // Display size in pixels                        1600 x 1200                    1280 x 1024
        //  //
        //  // My old Sony display with  96 DPI:                 ---                          96 x  96
        //  // My old Sony display with 120 DPI:                 ---                         120 x 120
        //  // My current Sony display with  96 DPI:           96 x  96                       96 x  96
        //  // My current Sony display with 120 DPI:          120 x 120                      120 x 120
        //  //
        //  // My LaserJet 6L (600 DPI):                                     600 x 600

        //  // physical pixel size in .01 MM units
        //  // accidentally(?) the result of GetPhysicalDimension!
        //  //float X1 = 100.0f * horzSizeMM / horzSizePixel;        // = 25.781250 : 19.843750   | 4.2343884
        //  //float Y1 = 100.0f * vertSizeMM / vertSizePixel;        // = 24.804688 : 19.824219   | 4.2265925

        //  // Now we can get the 'physical' device resolution...
        //  float phyResX = horzSizePixel / (horzSizeMM / 25.4f);
        //  float phyResY = vertSizePixel / (vertSizeMM / 25.4f);
        //  //
        //  // Display size in pixels                        1600 x 1200                    1280 x 1024
        //  //
        //  // My old Sony display with  96 DPI:                 ---                   98.521210 x 102.40000
        //  // My old Sony display with 120 DPI:                 ---                   128.00000 x 128.12611
        //  // My current Sony display with  96 DPI:     99.12195 x 98.32258            79.29756 x 83.90193
        //  // My current Sony display with 120 DPI:     99.12195 x 98.32258            79.29756 x 83.90193
        //  //
        //  // My LaserJet 6L (600 DPI):                               599.85052 x 600.95691

        //  // ...and rescale the size of the meta rectangle.
        //  magicX = logResX / phyResX;
        //  magicY = logResY / phyResY;
        //  //
        //  // Display size in pixels                        1600 x 1200                    1280 x 1024
        //  //
        //  // My old Sony display with  96 DPI:                 ---                  0.97440946 x 0.93750000
        //  // My old Sony display with 120 DPI:                 ---                  0.93750000 x 0.93657720
        //  // My current Sony display with  96 DPI:  0.968503952 x 0.976377964       1.21062994 x 1.14419293
        //  // My current Sony display with 120 DPI:  1.21062994  x 1.22047246        1.51328743 x 1.43024123
        //  //
        //  // My LaserJet 6L (600 DPI):                               1.0002491 x 0.99840766
        //}

        //[DllImport("gdi32.dll")]
        //static extern int GetDeviceCaps(IntPtr hdc, int capability);
        //const int HORZSIZE = 4;
        //const int VERTSIZE = 6;
        //const int HORZRES = 8;
        //const int VERTRES = 10;
        //const int LOGPIXELSX = 88;
        //const int LOGPIXELSY = 90;

        /// <summary>
        /// Upper left corner of scroll area.
        /// </summary>
        Point _posOffset;

        /// <summary>
        /// Real page size in point.
        /// </summary>
        SizeF _pageSize = PageSizeConverter.ToSize(PdfSharp.PageSize.A4).ToSizeF();

        /// <summary>
        /// Page in pixel relative to virtual canvas.
        /// </summary>
        Rectangle _virtualPage;

        /// <summary>
        /// The size in pixels of an area that completely contains the virtual page and at least a small 
        /// border around it. If this area is larger than the canvas window, it is scrolled.
        /// </summary>
        Size _virtualCanvas;

        /// <summary>
        /// Printable area in point.
        /// </summary>
        readonly RectangleF _printableArea;
    }
}

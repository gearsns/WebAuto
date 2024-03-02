using System.Drawing.Imaging;

namespace WebAuto
{
    public partial class VerticalTabControl : TabControl
    {
        public delegate void AddedNewPageHandler();
        public event AddedNewPageHandler? AddedNewPage;
        private int _amimationIndex;
        private readonly System.Windows.Forms.Timer _timer = new();
        private const int TIMER_INTERVAL = 80;
        private readonly Bitmap _bitmapLoading = Properties.Resources.loading;
        private int _movingTabPage = -1;

        public VerticalTabControl()
        {
            InitializeComponent();
            //Paintイベントで描画できるようにする
            SetStyle(ControlStyles.UserPaint, true);
            //ダブルバッファリングを有効にする
            DoubleBuffered = true;
            //リサイズで再描画する
            ResizeRedraw = true;

            //ControlStyles.UserPaintをTrueすると、
            //SizeModeは強制的にTabSizeMode.Fixedにされる
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(80, 18);
            Appearance = TabAppearance.Normal;
            Multiline = true;

            DrawMode = TabDrawMode.OwnerDrawFixed;

            _timer.Tick += Timer_Tick;
            _timer.Interval = TIMER_INTERVAL;
        }

        private int TabIndexFromPoint(Point point)
        {
            for (int i = 0; i < TabCount - 1; ++i)
            {
                if (GetTabRect(i).Contains(point))
                {
                    return i;
                }
            }
            return -1;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            ++_amimationIndex;
            Guid[] dlist = _bitmapLoading.FrameDimensionsList;
            if (dlist.Length > 0)
            {
                FrameDimension fd = new(dlist[0]);
                if (_amimationIndex >= _bitmapLoading.GetFrameCount(fd))
                {
                    _amimationIndex = 0;
                }
                _ = _bitmapLoading.SelectActiveFrame(fd, _amimationIndex);
                AnimateLoadingTabsOrStopIfNonIsLoading();
            }
        }
        private void AnimateLoadingTabsOrStopIfNonIsLoading()
        {
            bool stopRunning = true;
            for (int i = 0; i < TabPages.Count; i++)
            {
                if (TabPages[i] is LoadingTabPage ltp
                    && ltp.Loading)
                {
                    stopRunning = false;
                    Rectangle r = GetTabRect(i);
                    int iconSize = r.Height;
                    Invalidate(new Rectangle(r.Right - iconSize, r.Top, iconSize, iconSize));
                }
            }
            if (stopRunning)
            {
                _timer.Stop();
            }
        }

        public void RemoveTab(TabPage tabPage)
        {
            int i = SelectedIndex;
            if (SelectedTab == tabPage)
            {
                if (i >= 0 && i < TabCount - 2)
                {
                    SelectedTab = TabPages[i + 1];
                }
                else if (i - 1 > 0 && i < TabCount)
                {
                    SelectedTab = TabPages[i - 1];
                }
                else if (TabPages.Count >= 2)
                {
                    SelectedTab = TabPages[^2];
                }
            }
            TabPages.Remove(tabPage);
            tabPage.Dispose();
        }

        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            Point pos = PointToClient(Cursor.Position);
            int i = TabIndexFromPoint(pos);
            if (i >= 0 && i < TabPages.Count - 1 && e.TabPage == TabPages[i])
            {
                Rectangle tabRect = GetTabRect(i);
                tabRect.Inflate(-2, -2);
                Rectangle imageRect = new(
                    tabRect.Right - 16,
                    tabRect.Top + ((tabRect.Height - 16) / 2),
                    16,
                    16);
                if (imageRect.Contains(pos))
                {
                    e.Cancel = true;
                }
            }
            base.OnSelecting(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _movingTabPage = -1;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_movingTabPage >= 0)
            {
                int i = TabIndexFromPoint(e.Location);
                if (i > _movingTabPage)
                {
                    //   0 1 2 3 4 5 6
                    // t     2
                    // i         4
                    // j     <-3
                    //   0 1 3 2 4 5 6
                    // j       <-4
                    //   0 1 3 4 2 5 6
                    for (int j = _movingTabPage + 1; j <= i; j++)
                    {
                        TabPage page = TabPages[j];
                        TabPages.Remove(page);
                        TabPages.Insert(j - 1, page);
                    }
                    _movingTabPage = i;
                }
                else if (0 <= i && i < _movingTabPage)
                {
                    //   0 1 2 3 4 5 6
                    // t       3
                    // i   1
                    // j     2->
                    //   0 1 3 2 4 5 6
                    // j   1->
                    //   0 3 1 2 4 5 6
                    for (int j = _movingTabPage - 1; j >= i; j--)
                    {
                        TabPage page = TabPages[j];
                        TabPages.Remove(page);
                        TabPages.Insert(j + 1, page);
                    }
                    _movingTabPage = i;
                }
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            int lastIndex = TabPages.Count - 1;
            if(lastIndex >= 0)
            {
                if (GetTabRect(lastIndex).Contains(e.Location))
                {
                    if (AddedNewPage == null)
                    {
                        TabPages.Insert(lastIndex, "New Tab");
                        SelectedIndex = lastIndex;
                        TabPages[lastIndex].UseVisualStyleBackColor = true;
                    }
                    else
                    {
                        AddedNewPage();
                    }
                }
                else
                {
                    int i = TabIndexFromPoint(e.Location);
                    if (i >= 0)
                    {
                        Rectangle tabRect = GetTabRect(i);
                        tabRect.Inflate(-2, -2);
                        Rectangle imageRect = new(
                            tabRect.Right - 16,
                            tabRect.Top + ((tabRect.Height - 16) / 2),
                            16,
                            16);
                        if (imageRect.Contains(e.Location))
                        {
                            RemoveTab(TabPages[i]);
                        }
                        else if (e.Button == MouseButtons.Left)
                        {
                            _movingTabPage = i;
                        }
                    }
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //TabControlの背景を塗る
            {
                using Brush backBrush = new SolidBrush(Color.Gainsboro);
                e.Graphics.FillRectangle(backBrush, ClientRectangle);
            }
            if (TabPages.Count == 0)
            {
                return;
            }

            Rectangle ClipRectangle = e.ClipRectangle;

            //TabPageの枠を描画する
            if (SelectedIndex >= 0 && SelectedIndex < TabCount)
            {
                TabPage page = TabPages[SelectedIndex];
                Rectangle pageRect = new(
                    page.Bounds.X - 2,
                    page.Bounds.Y - 2,
                    page.Bounds.Width + 5,
                    page.Bounds.Height + 5);
                TabRenderer.DrawTabPage(e.Graphics, pageRect);
            }

            //タブを描画する
            for (int i = 0; i < TabPages.Count; i++)
            {
                TabPage page = TabPages[i];
                bool bLoading = false;
                if (page is LoadingTabPage loadingTabPage)
                {
                    bLoading = loadingTabPage.Loading;
                    if (loadingTabPage.Loading && !_timer.Enabled)
                    {
                        _timer.Start();
                    }
                }
                Rectangle tabRect = GetTabRect(i);
                if (tabRect.Height <= 0 || tabRect.Width <= 0 || !ClipRectangle.IntersectsWith(tabRect))
                {
                    continue;
                }
                //選択されたタブとページの間の境界線を消すために、
                //描画する範囲を大きくする
                if (SelectedIndex == i)
                {
                    if (Alignment == TabAlignment.Top)
                    {
                        tabRect.Height += 1;
                    }
                    else if (Alignment == TabAlignment.Bottom)
                    {
                        tabRect.Y -= 2;
                        tabRect.Height += 2;
                    }
                    else if (Alignment == TabAlignment.Left)
                    {
                        tabRect.Width += 1;
                    }
                    else if (Alignment == TabAlignment.Right)
                    {
                        tabRect.X -= 2;
                        tabRect.Width += 2;
                    }
                }
                //画像のサイズを決定する
                Size imgSize;
                if (Alignment == TabAlignment.Left ||
                    Alignment == TabAlignment.Right)
                {
                    imgSize = new Size(tabRect.Height, tabRect.Width);
                }
                else
                {
                    imgSize = tabRect.Size;
                }

                //Bottomの時はTextを表示しない（Textを回転させないため）
                string tabText = page.Text;
                if (Alignment == TabAlignment.Bottom)
                {
                    tabText = "";
                }

                //タブの画像を作成する
                using Bitmap bmp = new(imgSize.Width, imgSize.Height);
                Graphics g = Graphics.FromImage(bmp);
                //高さに1足しているのは、下にできる空白部分を消すため
                {
                    Rectangle bounds = tabRect; // new Rectangle(0, 0, bmp.Width, bmp.Height + 1);
                    //StringFormatを作成
                    using StringFormat sf = new()
                    {
                        //水平垂直方向の中央に、行が完全に表示されるようにする
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Near
                    };
                    sf.FormatFlags |= StringFormatFlags.LineLimit;
                    //背景の描画
                    if (SelectedIndex == i)
                    {
                        using Brush backBrush = new SolidBrush(Color.White);
                        e.Graphics.FillRectangle(backBrush, bounds);
                        //Textの描画
                        bounds.Width -= 16;
                        using Brush foreBrush = new SolidBrush(Color.Black);
                        e.Graphics.DrawString(tabText, page.Font, foreBrush, bounds, sf);
                    }
                    else
                    {
                        using Brush backBrush = new SolidBrush(Color.LightGray);
                        e.Graphics.FillRectangle(backBrush, bounds);
                        //Textの描画
                        bounds.Width -= 16;
                        using Brush foreBrush = new SolidBrush(Color.DimGray);
                        e.Graphics.DrawString(tabText, page.Font, foreBrush, bounds, sf);
                    }
                    if (bLoading)
                    {
                        e.Graphics.DrawImage(_bitmapLoading, tabRect.Right - 16, tabRect.Top + ((tabRect.Height - 12) / 2), 12, 12);
                    }
                    else if(i == TabPages.Count - 1)
                    {
                        Bitmap closeImage = Properties.Resources.add;
                        e.Graphics.DrawImage(closeImage, tabRect.Right - 16, tabRect.Top + ((tabRect.Height - 12) / 2), 12, 12);
                    }
                    else
                    {
                        Bitmap closeImage = Properties.Resources.close;
                        e.Graphics.DrawImage(closeImage, tabRect.Right - 16, tabRect.Top + ((tabRect.Height - 12) / 2), 12, 12);
                    }
                }
                g.Dispose();

                //画像を回転する
                if (Alignment == TabAlignment.Bottom)
                {
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else if (Alignment == TabAlignment.Left)
                {
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                else if (Alignment == TabAlignment.Right)
                {
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }

                //Bottomの時はTextを描画する
                if (Alignment == TabAlignment.Bottom)
                {
                    using StringFormat sf = new()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g = Graphics.FromImage(bmp);
                    g.DrawString(page.Text,
                        page.Font,
                        SystemBrushes.ControlText,
                        new RectangleF(0, 0, bmp.Width, bmp.Height),
                        sf);
                    g.Dispose();
                }
                //画像を描画する
                e.Graphics.DrawImage(bmp, tabRect.X, tabRect.Y, bmp.Width, bmp.Height);
            }
        }
    }
}

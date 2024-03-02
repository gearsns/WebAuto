using System.ComponentModel;

namespace WebAuto.Component
{
    public partial class ImageButton : Button
    {
        public ImageButton()
        {
            InitializeComponent();
        }

        public ImageButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private Bitmap? disableImg;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Brush b = new SolidBrush(SystemColors.Control);
            pevent.Graphics.FillRectangle(b, ClientRectangle);
            if (BackgroundImage is Image img)
            {
                pevent.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                if (Enabled)
                {
                    pevent.Graphics.DrawImage(img, ClientRectangle);
                }
                else
                {
                    if(null == disableImg)
                    {
                        disableImg = new Bitmap(img.Width, img.Height);
                        System.Drawing.Imaging.ColorMatrix cm =
                            new([
                                [1, 0, 0, 0 ,0],
                                [0, 1, 0, 0, 0],
                                [0, 0, 1, 0, 0],
                                [0, 0, 0, 0.5f, 0],
                                [0, 0, 0, 0, 1]
                                ]);

                        System.Drawing.Imaging.ImageAttributes ia = new();
                        ia.SetColorMatrix(cm);
                        Graphics g = Graphics.FromImage(disableImg);
                        g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
                        g.Dispose();

                    }
                    pevent.Graphics.DrawImage(disableImg, ClientRectangle);
                }
            }
            //base.OnPaint(pevent);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }
    }
}

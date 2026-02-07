using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CaptureScreenForm
{
    public partial class Capture : Form
    {
        private int IndexOfScreen;
        private int selectX;
        private int selectY;
        private int selectWidth;
        private int selectHeight;
        private Bitmap screenshot;
        private readonly Pen pen = new Pen(new SolidBrush(Color.Red), 3) { DashStyle = DashStyle.DashDotDot };
        private bool isSelecting = false;   
        private System.Windows.Forms.PictureBox pictureBoxScreenshoot;
       
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

    

   
        private void InitializeComponent()
        {
            this.pictureBoxScreenshoot = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshoot)).BeginInit();
            this.SuspendLayout();

            this.pictureBoxScreenshoot.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxScreenshoot.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxScreenshoot.Name = "pictureBoxScreenshoot";
            this.pictureBoxScreenshoot.Size = new System.Drawing.Size(2174, 1014);
            this.pictureBoxScreenshoot.TabIndex = 0;
            this.pictureBoxScreenshoot.TabStop = false;
            this.pictureBoxScreenshoot.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxScreenshoot_MouseDown);
            this.pictureBoxScreenshoot.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxScreenshoot_MouseMove);
            this.pictureBoxScreenshoot.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxScreenshoot_MouseUp);
   
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1257, 273);
            this.Controls.Add(this.pictureBoxScreenshoot);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Capture";
            this.Text = "Capture";
            this.Load += new System.EventHandler(this.Capture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshoot)).EndInit();
            this.ResumeLayout(false);

        }

  

    
        public Capture(int indexOfScreen = -1)
        {
            InitializeComponent();
            this.IndexOfScreen = indexOfScreen;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.Manual;

            this.Top = 0;
            this.Left = 0;


            pictureBoxScreenshoot.Dock = DockStyle.Fill;
        }


        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private const int DESKTOPHORZRES = 118;
        private const int DESKTOPVERTRES = 117;
        private const int HORZRES = 8;
        private const int VERTRES = 10;

        private float GetScalingFactor()
        {
            Graphics g = this.CreateGraphics();
            IntPtr desktop = g.GetHdc();

            int logicalScreenHeight = GetDeviceCaps(desktop, VERTRES);
            int physicalScreenHeight = GetDeviceCaps(desktop, DESKTOPVERTRES);

            float scalingFactor = (float)physicalScreenHeight / logicalScreenHeight;

            g.ReleaseHdc(desktop);
            g.Dispose();

            return scalingFactor;
        }


        private void Capture_Load(object sender, EventArgs e)
        {
            this.Hide();

            float dpiScale = GetScalingFactor();
            Screen currentScreen;
            if (IndexOfScreen == -1)
            {
                 currentScreen = Screen.FromControl(this);
            }
            else { currentScreen = Screen.AllScreens[IndexOfScreen]; }
           
            Rectangle bounds = currentScreen.Bounds;

            int actualWidth = (int)(bounds.Width * dpiScale);
            int actualHeight = (int)(bounds.Height * dpiScale);

            screenshot = new Bitmap(actualWidth, actualHeight);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, new Size(actualWidth, actualHeight));
            }

            pictureBoxScreenshoot.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxScreenshoot.Image = screenshot;

            this.Show();
            Cursor = Cursors.Cross;
        }

        private void pictureBoxScreenshoot_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBoxScreenshoot.Image == null) return;

            if (e.Button == MouseButtons.Left)
            {
                selectX = e.X;
                selectY = e.Y;
                isSelecting = true;
            }
        }

        private void pictureBoxScreenshoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBoxScreenshoot.Image == null || !isSelecting) return;

            selectWidth = e.X - selectX;
            selectHeight = e.Y - selectY;

            pictureBoxScreenshoot.Refresh();

            using (Graphics g = pictureBoxScreenshoot.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawRectangle(pen, selectX, selectY, selectWidth, selectHeight);
            }
        }

        private void pictureBoxScreenshoot_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBoxScreenshoot.Image == null || !isSelecting) return;

            isSelecting = false;

            if (selectWidth > 0 && selectHeight > 0)
            {
                SaveToClipboard();
            }
        }

        private void SaveToClipboard()
        {
            if (selectWidth > 0 && selectHeight > 0)
            {
                Rectangle rect = new Rectangle(selectX, selectY, selectWidth, selectHeight);

                using (Bitmap originalImage = new Bitmap(pictureBoxScreenshoot.Image, pictureBoxScreenshoot.Width, pictureBoxScreenshoot.Height))
                {
                    using (Bitmap croppedImage = new Bitmap(selectWidth, selectHeight))
                    {
                        using (Graphics g = Graphics.FromImage(croppedImage))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.CompositingQuality = CompositingQuality.HighQuality;

                            g.DrawImage(originalImage, 0, 0, rect, GraphicsUnit.Pixel);
                        }

                        Clipboard.SetImage(croppedImage);
                    }
                }
            }

            this.Close();
        }
    }
}

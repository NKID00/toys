using System.Drawing;
using System.Windows.Forms;

namespace DongLibSharp
{
    public partial class ImageForm : Form
    {
        public ImageForm(Image img, int x, int y, AlignX alignx, AlignY aligny)
        {
            this.InitializeComponent();
            PictureBox.Image = img;
            Width = PictureBox.Width;
            Height = PictureBox.Height;
            switch (alignx)
            {
                case AlignX.Left:
                    break;
                case AlignX.Middle:
                    x -= PictureBox.Width / 2;
                    break;
                case AlignX.Right:
                    x -= PictureBox.Width;
                    break;
            }
            switch (aligny)
            {
                case AlignY.Top:
                    break;
                case AlignY.Middle:
                    y -= PictureBox.Height / 2;
                    break;
                case AlignY.Bottom:
                    y -= PictureBox.Height;
                    break;
            }
            Location = new Point(x, y);
        }

        private void PictureBox_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}

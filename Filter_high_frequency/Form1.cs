using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filter_high_frequency
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var picture = new OpenFileDialog();
            if(picture.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(picture.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Файл PNG (*.png)|*.png|Все файлы (*.*)|*.*";
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(saveFileDialog1.FileName);
            }
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter filter = new Filter(pictureBox1.Image);
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            int[] hist = filter.GetHist();
            int[] hist_new = filter.GetNewHist();
            filter.FilterHighFrequency();
            pictureBox2.Image = filter.BitmapToPicture();
            for (int i = 0;i<256;i++)
                chart1.Series[0].Points.AddXY(i, hist[i]);
            for(int i=0;i<255;i++)
                chart2.Series[0].Points.AddXY(i, hist_new[i]);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Реализация фильтра высоких частот.");
        }
    }
}

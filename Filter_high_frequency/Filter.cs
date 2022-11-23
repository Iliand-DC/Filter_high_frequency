﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace Filter_high_frequency
{
    internal class Filter
    {
        byte[,] image_matr;
        int[] hist;
        double[,] obr_image;
        int[] hist_new;
        double gmax, gmin;
        int fmax, fmin;
        int w_b, h_b;

        Bitmap bmp;
        Bitmap bmp_new;
        //Получение массива частот из изображения
        public Filter(Image image)
        {
            bmp_new = new Bitmap(image);
            bmp = new Bitmap(image);
            w_b = bmp.Width;
            h_b = bmp.Height;
            image_matr = new byte[w_b, h_b];
            obr_image = new double[w_b, h_b];
            hist = new int[256];
            hist_new = new int[256];

            fmin = 1000;
            fmax = -100;
            gmin = 1000;
            gmax = -100;

            for(int x = 0; x < w_b; x++)
            {
                for(int y = 0; y < h_b; y++)
                {
                    Color c = bmp.GetPixel(x, y);
                    int r = Convert.ToInt32(c.R);
                    int g = Convert.ToInt32(c.G);
                    int b = Convert.ToInt32(c.B);
                    int brit = Convert.ToInt32(0.299*r+0.587*g+0.114*b);
                    image_matr[x,y] = Convert.ToByte(brit);
                    hist[image_matr[x, y]]++;
                    if (image_matr[x, y] > fmax)
                        fmax = image_matr[x, y];
                    if (image_matr[x, y] < fmin)
                        fmin = image_matr[x, y];
                }
            }
        }
        //Получение изображения из массива частот
        public Image BitmapToPicture()
        {
            return (Image)bmp_new;
        }

        public int[] GetHist()
        {
            return hist;
        }

        public int[] GetNewHist()
        {
            return hist_new;
        }
        private Complex[,] DPF()
        {
            Complex[,] dpf_matrix = new Complex[w_b,h_b];
            for(int x=0; x < w_b; x++)
            {
                for (int y = 0; y < h_b; y++)
                {
                    for (int u = 0; u < w_b; u++)
                    {
                        for (int v = 0; v < h_b; v++)
                        {
                            dpf_matrix[x, y] = ((Complex)image_matr[x, y] * Complex.Exp((-Complex.ImaginaryOne) * 2 * Math.PI * ((u * x) / w_b + (v * y) / h_b))) / (w_b * h_b);
                        }
                    }
                }
            }
            return dpf_matrix;
        }
        private double[,] ReverseDPF(Complex[,] dpf_matrix)
        {
            Complex[,] revDpf = new Complex[w_b,h_b];
            double[,] reverseDpf = new double[w_b, h_b];
            for (int x = 0; x < w_b; x++)
            {
                for (int y = 0; y < h_b; y++)
                {
                    for (int u = 0; u < w_b; u++)
                    {
                        for (int v = 0; v < h_b; v++)
                        {
                            revDpf[x, y] = (dpf_matrix[x, y] * Complex.Exp((Complex.ImaginaryOne) * 2 * Math.PI * ((u * x) / w_b + (v * y) / h_b)));
                            reverseDpf[x, y] = (int)revDpf[x, y].Real;
                        }
                    }
                }
            }
            return reverseDpf;
        }
        public void FilterHighFrequency()
        {
            Complex[,] dpf_matrix = DPF();
            obr_image = ReverseDPF(dpf_matrix);
            for (int x=0;x<w_b;x++)
            {
                for(int y=0;y<h_b;y++)
                {
                //    if (image_matr[x, y] > 127)
                //        obr_image[x, y] = image_matr[x, y];
                //    else
                //        obr_image[x, y] = image_matr[x, y];

                    byte briteness = Convert.ToByte(obr_image[x, y]);
                    Color c = Color.FromArgb(briteness, briteness, briteness);
                    bmp_new.SetPixel(x, y, c);
                    hist_new[(int)obr_image[x, y]]++;
                    if (obr_image[x, y] > gmax)
                        gmax = obr_image[x, y];
                    if (obr_image[x, y] < gmin)
                        gmin = obr_image[x, y];
                }
            }
        }
    }
}

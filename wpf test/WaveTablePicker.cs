using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_test
{
    public partial class MainWindow : Window
    {
        private Ellipse ellipse_being_dragged = null;
        private void wavetable_sample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ellipse_being_dragged = (Ellipse)sender;
        }

        private void wave_table_picker_MouseMove(object sender, MouseEventArgs e)
        {
            if (ellipse_being_dragged == null)
            {
                return;
            }

            Point p = e.GetPosition(wave_table_picker);
            if (p.Y > wave_table_picker.Height)
            {
                p.Y = wave_table_picker.Height;
            }
            else if (p.Y < 0)
            {
                p.Y = 0;
            }
            int val = (int)Math.Round((p.Y / wave_table_picker.Height) * 15);

            double yval = val * (wave_table_picker.Height / 16.0);
            ellipse_being_dragged.Tag = 15 - val;

            ellipse_being_dragged.SetValue(Canvas.TopProperty, (double)(yval - 5));
            makeWaveTable();
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ellipse_being_dragged = null;
        }
        private void makeWaveTable()
        {
            var children = wave_table_picker.Children;
            Console.WriteLine();
            byte[] newtable = new byte[16];
            int newtable_ptr = 0;
            int on_nybble = 0;
            foreach (var child in children)
            {
                Ellipse e = (Ellipse)child;
                int b = (int)e.Tag;
                switch (on_nybble)
                {
                    case 0:

                        newtable[newtable_ptr] |= (byte)(b << 4);
                        on_nybble++;
                        break;
                    case 1:
                        newtable[newtable_ptr] |= (byte)b;
                        newtable_ptr++;
                        on_nybble = 0;
                        break;
                }
                Console.WriteLine(e.Tag.ToString());
            }
        }
        private void initSamplePicker()
        {
            var children = wave_table_picker.Children;
            foreach (var child in children)
            {
                Ellipse e = (Ellipse)child;
                e.Tag = 7;
            }
        }
    }
}

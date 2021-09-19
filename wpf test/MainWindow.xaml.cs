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
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameBoySound.SoundChip chip;
        public MainWindow()
        {
            InitializeComponent();
            initSamplePicker();
            chip = new GameBoySound.SoundChip();
        }
        
        private void numberBoxHandlePreview(object sender, TextCompositionEventArgs e, int max_num)
        {
            // forces in int between 0 and max_num to be inputted
            string txt = e.Text;
            int parsed;
            if (!int.TryParse(txt, out parsed))
            {
                e.Handled = true;
                return;
            }

            string old_txt = ((TextBox)sender).Text;
            if (txt.Length > 0)
                parsed = int.Parse(old_txt + txt);
            if (parsed > max_num || parsed < 0)
            {
                e.Handled = true;
                return;
            }
            e.Handled = false;
        }
        private void number_preview_2_bit(object sender, TextCompositionEventArgs e)
        {
            numberBoxHandlePreview(sender, e, 3);
        }
        private void number_preview_3_bit(object sender, TextCompositionEventArgs e)
        {
            numberBoxHandlePreview(sender, e, 7);
        }
        private void number_preview_4_bit(object sender, TextCompositionEventArgs e)
        {
            numberBoxHandlePreview(sender, e, 15);
        }
        private void number_preview_6_bit(object sender, TextCompositionEventArgs e)
        {
            numberBoxHandlePreview(sender, e, 63);
        }
        private void number_preview_8_bit(object sender, TextCompositionEventArgs e)
        {
            numberBoxHandlePreview(sender, e, 255);
        }
        private void number_preview_11_bit(object sender, TextCompositionEventArgs e)
        {
            numberBoxHandlePreview(sender, e, 2047);
        }
        
    }
    
}


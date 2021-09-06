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
            chip = new GameBoySound.SoundChip();
        }
        private void NR10_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text;
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                chip.setNR10(result);
            }
        }
        private void NR11_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text;
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                chip.setNR11(result);
            }
        }
        private void NR12_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text;
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                chip.setNR12(result);
            }
        }
        private void NR13_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text;
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                chip.setNR13(result);
            }
        }
        private void NR14_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text;
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                chip.setNR14(result);
            }
        }
    }
}

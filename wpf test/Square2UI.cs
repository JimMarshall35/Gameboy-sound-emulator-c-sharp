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
        private void NR21_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text.Length > 0 ? b.Text : "0";
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                int duty = (result & 0b1100_0000) >> 6;
                int length = result & 0b0011_1111;
                channel_2_duty.Text = duty.ToString();
                channel_2_length_load.Text = length.ToString();
                chip.setNR21(result);
            }
        }

        private void NR22_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text;
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                int volume = (result & 0b1111_0000) >> 4;
                bool env_add_mode = ((result & 0b0000_1000) >> 3) == 0 ? false : true;
                int volume_env_period = result & 0b0000_0111;
                channel_2_starting_volume.Text = volume.ToString();
                channel_2_env_add_mode.IsChecked = env_add_mode;
                channel_2_env_period.Text = volume_env_period.ToString();
                chip.setNR22(result);
            }
        }

        private void NR23_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text.Length > 0 ? b.Text : "0";
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                int freq = 0;
                freq |= result;
                int n14_value = NR24.Text.Length > 0 ? int.Parse(NR24.Text) : 0;
                freq |= (n14_value & 0b0000_0111) << 8;
                channel_2_frequency.Text = freq.ToString();
                chip.setNR23(result);
            }
        }

        private void NR24_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text.Length > 0 ? b.Text : "0";
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                bool trigger = (result & 0b1000_0000) == 0 ? false : true;
                bool length_enable = (result & 0b0100_0000) == 0 ? false : true;
                int freq = 0;
                int n13_value = NR23.Text.Length > 0 ? int.Parse(NR23.Text) : 0;
                freq |= n13_value;
                freq |= (result & 0b0000_0111) << 8;
                channel_2_frequency.Text = freq.ToString();
                channel_2_trigger.IsChecked = trigger;
                channel_2_length_enable.IsChecked = length_enable;
                chip.setNR24(result);
            }
        }

        private void channel_2_duty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR21 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;
            newval = newval << 6;

            int oldNR11 = NR21.Text.Length > 0 ? int.Parse(NR21.Text) : 0;
            oldNR11 &= 0b0011_1111;
            oldNR11 |= newval;

            NR21.Text = oldNR11.ToString();
            chip.setNR21((byte)oldNR11);
        }

        private void channel_2_length_load_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR21 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;


            int oldNR11 = NR21.Text.Length > 0 ? int.Parse(NR21.Text) : 0;
            oldNR11 &= 0b1100_0000;
            oldNR11 |= newval;

            NR21.Text = oldNR11.ToString();
            chip.setNR21((byte)oldNR11);
        }

        private void channel_2_starting_volume_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR22 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;
            newval = newval << 4;

            int oldNR12 = NR22.Text.Length > 0 ? int.Parse(NR22.Text) : 0;
            oldNR12 &= 0b0000_1111;
            oldNR12 |= newval;

            NR22.Text = oldNR12.ToString();
            chip.setNR22((byte)oldNR12);
        }

        private void channel_2_env_add_mode_Click(object sender, RoutedEventArgs e)
        {
            if (NR22 == null)
                return;
            bool is_checked = (bool)((CheckBox)sender).IsChecked;
            int mask = is_checked ? 0b0000_1000 : 0b0000_0000;
            int oldNR12 = NR22.Text.Length > 0 ? int.Parse(NR22.Text) : 0;
            oldNR12 &= 0b1111_0111;
            oldNR12 |= mask;
            NR22.Text = oldNR12.ToString();
            chip.setNR22((byte)oldNR12);
        }

        private void channel_2_env_period_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR22 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;


            int oldNR12 = NR22.Text.Length > 0 ? int.Parse(NR22.Text) : 0;
            oldNR12 &= 0b1111_1000;
            oldNR12 |= newval;

            NR22.Text = oldNR12.ToString();
            chip.setNR22((byte)oldNR12);
        }

        private void channel_2_frequency_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR23 == null || NR24 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;
            NR23.Text = (newval & 0xff).ToString();
            int old_nr14 = NR24.Text.Length > 0 ? int.Parse(NR24.Text) : 0;
            old_nr14 &= 0b1111_1000;
            old_nr14 |= newval >> 8;
            NR24.Text = old_nr14.ToString();
            chip.setNR24((byte)old_nr14);
            chip.setNR23((byte)(newval & 0xff));
        }

        private void channel_2_trigger_Click(object sender, RoutedEventArgs e)
        {
            if (NR24 == null)
                return;
            bool is_checked = (bool)((CheckBox)sender).IsChecked;
            int mask = is_checked ? 0b1000_0000 : 0b0000_0000;
            int oldNR14 = NR24.Text.Length > 0 ? int.Parse(NR24.Text) : 0;
            oldNR14 &= 0b0111_1111;
            oldNR14 |= mask;
            NR24.Text = oldNR14.ToString();
            chip.setNR24((byte)oldNR14);
        }

        private void channel_2_length_enable_Click(object sender, RoutedEventArgs e)
        {
            if (NR24 == null)
                return;
            bool is_checked = (bool)((CheckBox)sender).IsChecked;
            int mask = is_checked ? 0b0100_0000 : 0b0000_0000;
            int oldNR14 = NR24.Text.Length > 0 ? int.Parse(NR24.Text) : 0;
            oldNR14 &= 0b1011_1111;
            oldNR14 |= mask;
            NR24.Text = oldNR14.ToString();
            chip.setNR24((byte)oldNR14);
        }
    }
}

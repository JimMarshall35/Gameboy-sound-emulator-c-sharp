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
        private void NR10_changed(object sender, TextChangedEventArgs e)
        {


            TextBox b = (TextBox)(sender);
            string content = b.Text.Length > 0 ? b.Text : "0";
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                int sweep_period = (result & 0b0111_0000) >> 4;
                bool negate = ((result & 0b0000_1000) >> 3) == 0 ? false : true;
                int shift = result & 0b0000_0111;
                channel_1_sweep_period.Text = sweep_period.ToString().Length > 0 ? sweep_period.ToString() : "0";

                channel_1_sweep_negate.IsChecked = negate;
                channel_1_sweep_shift.Text = shift.ToString().Length > 0 ? shift.ToString() : "0";
                chip.setNR10(result);
            }
        }
        private void NR11_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text.Length > 0 ? b.Text : "0";
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                int duty = (result & 0b1100_0000) >> 6;
                int length = result & 0b0011_1111;
                channel_1_duty.Text = duty.ToString();
                channel_1_length_load.Text = length.ToString();
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
                int volume = (result & 0b1111_0000) >> 4;
                bool env_add_mode = ((result & 0b0000_1000) >> 3) == 0 ? false : true;
                int volume_env_period = result & 0b0000_0111;
                channel_1_starting_volume.Text = volume.ToString();
                channel_1_env_add_mode.IsChecked = env_add_mode;
                channel_1_env_period.Text = volume_env_period.ToString();
                chip.setNR12(result);
            }
        }
        private void NR13_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text.Length > 0 ? b.Text : "0";
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                int freq = 0;
                freq |= result;
                int n14_value = NR14.Text.Length > 0 ? int.Parse(NR14.Text) : 0;
                freq |= (n14_value & 0b0000_0111) << 8;
                channel_1_frequency.Text = freq.ToString();
                chip.setNR13(result);
            }
        }
        private void NR14_changed(object sender, TextChangedEventArgs e)
        {
            TextBox b = (TextBox)(sender);
            string content = b.Text.Length > 0 ? b.Text : "0";
            byte result;
            if (Byte.TryParse(content, out result) && chip != null)
            {
                bool trigger = (result & 0b1000_0000) == 0 ? false : true;
                bool length_enable = (result & 0b0100_0000) == 0 ? false : true;
                int freq = 0;
                int n13_value = NR13.Text.Length > 0 ? int.Parse(NR13.Text) : 0;
                freq |= n13_value;
                freq |= (result & 0b0000_0111) << 8;
                channel_1_frequency.Text = freq.ToString();
                channel_1_trigger.IsChecked = trigger;
                channel_1_length_enable.IsChecked = length_enable;
                chip.setNR14(result);
            }
        }
        private void channel_1_sweep_period_changed(object sender, TextChangedEventArgs e)
        {
            if (NR10 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;
            newval = newval << 4;
            int oldNR10 = NR10.Text.Length > 0 ? int.Parse(NR10.Text) : 0;
            oldNR10 &= 0b0000_1111;
            oldNR10 |= newval;
            NR10.Text = oldNR10.ToString();
            chip.setNR10((byte)oldNR10);

        }
        private void channel_1_sweep_shift_changed(object sender, TextChangedEventArgs e)
        {
            if (NR10 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;
            int oldNR10 = NR10.Text.Length > 0 ? int.Parse(NR10.Text) : 0;
            oldNR10 &= 0b1111_1000;
            oldNR10 |= newval;
            NR10.Text = oldNR10.ToString();
            chip.setNR10((byte)oldNR10);

        }
        private void channel_1_sweep_negate_Click(object sender, RoutedEventArgs e)
        {
            if (NR10 == null)
                return;
            bool is_checked = (bool)((CheckBox)sender).IsChecked;
            int mask = is_checked ? 0b0000_1000 : 0b0000_0000;
            int oldNR10 = NR10.Text.Length > 0 ? int.Parse(NR10.Text) : 0;
            oldNR10 &= 0b1111_0111;
            oldNR10 |= mask;
            NR10.Text = oldNR10.ToString();
            chip.setNR10((byte)oldNR10);
        }

        private void channel_1_duty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR11 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;
            newval = newval << 6;

            int oldNR11 = NR11.Text.Length > 0 ? int.Parse(NR11.Text) : 0;
            oldNR11 &= 0b0011_1111;
            oldNR11 |= newval;

            NR11.Text = oldNR11.ToString();
            chip.setNR11((byte)oldNR11);
        }

        private void channel_1_length_load_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR11 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;


            int oldNR11 = NR11.Text.Length > 0 ? int.Parse(NR11.Text) : 0;
            oldNR11 &= 0b1100_0000;
            oldNR11 |= newval;

            NR11.Text = oldNR11.ToString();
            chip.setNR11((byte)oldNR11);
        }

        private void channel_1_starting_volume_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR12 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;
            newval = newval << 4;

            int oldNR12 = NR12.Text.Length > 0 ? int.Parse(NR12.Text) : 0;
            oldNR12 &= 0b0000_1111;
            oldNR12 |= newval;

            NR12.Text = oldNR12.ToString();
            chip.setNR12((byte)oldNR12);
        }

        private void channel_1_env_add_mode_Click(object sender, RoutedEventArgs e)
        {
            if (NR12 == null)
                return;
            bool is_checked = (bool)((CheckBox)sender).IsChecked;
            int mask = is_checked ? 0b0000_1000 : 0b0000_0000;
            int oldNR12 = NR12.Text.Length > 0 ? int.Parse(NR12.Text) : 0;
            oldNR12 &= 0b1111_0111;
            oldNR12 |= mask;
            NR12.Text = oldNR12.ToString();
            chip.setNR12((byte)oldNR12);
        }

        private void channel_1_env_period_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR12 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;


            int oldNR12 = NR12.Text.Length > 0 ? int.Parse(NR12.Text) : 0;
            oldNR12 &= 0b1111_1000;
            oldNR12 |= newval;

            NR12.Text = oldNR12.ToString();
            chip.setNR12((byte)oldNR12);
        }

        private void channel_1_frequency_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NR13 == null || NR14 == null)
                return;
            TextBox t = (TextBox)sender;
            int newval = t.Text.Length > 0 ? int.Parse(t.Text) : 0;

            NR13.Text = (newval & 0xff).ToString();
            int old_nr14 = NR14.Text.Length > 0 ? int.Parse(NR14.Text) : 0;
            old_nr14 &= 0b1111_1000;
            old_nr14 |= newval >> 8;

            NR14.Text = old_nr14.ToString();
            chip.setNR14((byte)old_nr14);
            chip.setNR13((byte)(newval & 0xff));

        }

        private void channel_1_trigger_Click(object sender, RoutedEventArgs e)
        {
            if (NR14 == null)
                return;
            bool is_checked = (bool)((CheckBox)sender).IsChecked;
            int mask = is_checked ? 0b1000_0000 : 0b0000_0000;
            int oldNR14 = NR14.Text.Length > 0 ? int.Parse(NR14.Text) : 0;
            oldNR14 &= 0b0111_1111;
            oldNR14 |= mask;
            NR14.Text = oldNR14.ToString();
            chip.setNR14((byte)oldNR14);
        }

        private void channel_1_length_enable_Click(object sender, RoutedEventArgs e)
        {
            if (NR14 == null)
                return;
            bool is_checked = (bool)((CheckBox)sender).IsChecked;
            int mask = is_checked ? 0b0100_0000 : 0b0000_0000;
            int oldNR14 = NR14.Text.Length > 0 ? int.Parse(NR14.Text) : 0;
            oldNR14 &= 0b1011_1111;
            oldNR14 |= mask;
            NR14.Text = oldNR14.ToString();
            chip.setNR14((byte)oldNR14);
        }
    }
}

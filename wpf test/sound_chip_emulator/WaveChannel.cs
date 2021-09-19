using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace GameBoySound
{
    class WaveChannel : ISampleProvider
    {
        private const int wave_table_length = 32;
        private enum WaveVol : byte
        {
            OFF = 0,
            FULL = 1,
            HALF = 2,
            QUARTER = 3
        }
        public float[] processed_wave_table;

        private bool enabled = false;

        private bool active = false;

        private bool length_enabled;
        private int length_in_samples = 0;
        private int onsample = 0;

        private WaveVol volume = WaveVol.OFF;
        private float gain = 0;

        private float frequency = 0;

        private float phase = 0;

        public WaveFormat WaveFormat { get; }

        public WaveChannel(WaveRegisters w)
        {
            processed_wave_table = new float[wave_table_length];
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(16000, 1);
            setFromRegister(w);
        }
        public void setFromRegister(WaveRegisters w)
        {
            processWaveTable(w.wave_table);

            // NR30
            enabled = (w.NR30 & 0b1000_0000) > 0 ? true : false;

            // NR31
            byte rawlen = w.NR31;
            float len_seconds = (256.0f - rawlen) * (1/256.0f);
            length_in_samples = (int)(len_seconds * WaveFormat.SampleRate);

            // NR32
            volume = (WaveVol)((w.NR32 & 0b0110_0000) >> 5);
            setGain();

            // NR33 / NR34
            short rawf = w.NR33;
            rawf |= (short)((w.NR34 & 0b00000111) << 8);
            setFrequencyFromRaw(rawf);

            // set active
            active = ((w.NR34 & 0b10000000) > 0) ? true : false;


            // set length enabled
            length_enabled = ((w.NR34 & 0b01000000) > 0) ? true : false;

        }

        private void setGain()
        {
            switch (volume)
            {
                case WaveVol.OFF:
                    gain = 0.0f;
                    break;
                case WaveVol.FULL:
                    gain = 1.0f;
                    break;
                case WaveVol.HALF:
                    gain = 0.5f;
                    break;
                case WaveVol.QUARTER:
                    gain = 0.25f;
                    break;
                default:
                    gain = 0.0f;
                    break;
            }
        }

        private void processWaveTable(byte[] table)
        {
            for(int i=0; i<16; i++)
            {
                byte input_byte = table[i];
                int output_index = 2 * i;
                processed_wave_table[output_index] = -1 + (((input_byte >> 4) / 15.0f)*2);
                processed_wave_table[output_index + 1] = -1 + (((input_byte & 0b0000_1111) / 15.0f)*2);
            }
        }
        public int Read(float[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = getNextSample();
            }
            return count;
        }
        private void setFrequencyFromRaw(short rawf)
        {
            frequency = 65536.0f / (float)(2048 - rawf);
        }
        private float getNextSample()
        {
            // this code has just been copied from my C++ synthesizer 
            // probably not very efficient to use with such a small wave table as this program uses
            if (!active)
                return 0.0f;
            phase += wave_table_length * frequency / WaveFormat.SampleRate;
            while (phase >= wave_table_length)
            {
                phase -= wave_table_length;
            }

            if (length_enabled)
            {
                onsample++;
                if (onsample > length_in_samples)
                    active = false;
            }

            return processed_wave_table[(int)phase] * gain;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace GameBoySound
{
    enum SweepDirection : int
    {
        ADD = 0,
        SUB = 1
    }
    enum VolumeEnvelopeDirection : int
    {
        OFF = 0,
        UP = 1,
        DOWN = -1
    }
    public class SquareBase : ISampleProvider
    {
        private const int envelope_clock_freq = 64;
        public Duty waveform { get; set; }

        private float phase = 0;
        private float frequency;

        protected bool active;

        private int onsample = 0;
        private int length_in_samples = 0;
        private bool length_enabled;

        private float gain;
        private int raw_volume;
        private VolumeEnvelopeDirection envelope_direction = VolumeEnvelopeDirection.OFF;
        private int envelope_period_samples = 0;
        private int envelope_timer = 0;

        private float[,] tables = new float[4, 8]{
            { -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f,  1.0f },
            {  1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f,  1.0f },
            {  1.0f, -1.0f, -1.0f, -1.0f, -1.0f,  1.0f,  1.0f,  1.0f },
            { -1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f, -1.0f }
        };
        public SquareBase(SquareRegisters s)
        {
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(16000, 1);
            setFromRegister(s);
        }
        public virtual void setFromRegister(SquareRegisters s)
        {
            onsample = 0;
            waveform = (Duty)((s.NR11 & 0b1100_0000) >> 6);

            //set length
            byte rawl = (byte)(s.NR11 & (byte)0b0011_1111);
            float len_seconds = (64.0f - (float)rawl) * (1.0f / 256.0f);
            length_in_samples = (int)(len_seconds * (float)WaveFormat.SampleRate);


            // set frequency
            short rawf = s.NR13;
            rawf |= (short)((s.NR14 & 0b00000111) << 8);
            setFrequencyFromRaw(rawf);



            // set volume envelope
            byte rawg = (byte)((s.NR12 & 0b11110000) >> 4);
            raw_volume = rawg;
            gain = rawg / 16.0f;
            envelope_direction = (s.NR12 & 0b0000_1000) == 0 ? VolumeEnvelopeDirection.DOWN : VolumeEnvelopeDirection.UP;
            int env_period_raw = s.NR12 & 0b0000_0111;
            if (env_period_raw == 0)
                envelope_direction = VolumeEnvelopeDirection.OFF;
            envelope_period_samples = (int)(((float)env_period_raw / envelope_clock_freq) * (float)WaveFormat.SampleRate);
            envelope_timer = 0;


            // set active
            if ((s.NR14 & 0b10000000) > 0)
            {
                active = true;
            }
            else
            {
                active = false;
            }

            // set length enabled
            if ((s.NR14 & 0b01000000) > 0)
            {
                length_enabled = true;
            }
            else
            {
                length_enabled = false;
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
        public WaveFormat WaveFormat { get; }
        protected virtual float getNextSample()
        {
            // this code has just been copied from my C++ synthesizer 
            // probably not very efficient to use with such a small wave table as this program uses
            if (!active)
                return 0.0f;
            phase += 8 * frequency / WaveFormat.SampleRate;
            while (phase >= 8)
            {
                phase -= 8;
            }

            if (length_enabled)
            {
                onsample++;
                if (onsample > length_in_samples)
                    active = false;
            }

            if (envelope_direction != VolumeEnvelopeDirection.OFF)
            {
                envelope_timer++;
                if (envelope_timer >= envelope_period_samples)
                {
                    envelope_timer = 0;
                    volumeEnvelope();
                }
            }
            return tables[(int)waveform, (int)phase] * gain;

        }
        private void volumeEnvelope()
        {

            if (raw_volume > 0 && raw_volume < 16)
            {
                raw_volume += (int)envelope_direction;
                gain = (float)raw_volume / 16.0f;
            }
        }
        protected void setFrequencyFromRaw(short rawf)
        {
            frequency = 131072 / (2048 - rawf);//(float)(2048 - rawf) * 4;
        }
    }
    public sealed class Square1 : SquareBase
    {
        // adds a frequency sweep to the base square wave channel, set by the register NR10
        private short shadow_frequency = 0;
        private int sweep_period = 0;
        private int sweep_shift_number = 0;
        private int sweep_period_in_samples = 0;
        private int sweep_timer = 0;
        private SweepDirection sweep_direction;

        private const int sweep_clock_freq = 128;

        public Square1(SquareRegisters s) : base(s)
        {

            // set frequency sweep
            sweep_period = (s.NR10 & 0b0111_0000) >> 4;
            sweep_shift_number = s.NR10 & 0b0000_0111;
            sweep_direction = ((s.NR10 & 0b0000_1000) == 0) ? SweepDirection.ADD : SweepDirection.SUB;
            sweep_period_in_samples = (int)(((float)sweep_period / sweep_clock_freq) * (float)WaveFormat.SampleRate);
            sweep_timer = 0;
            shadow_frequency = (short)((s.NR14 & 0b00000111) << 8);
        }

        public override void setFromRegister(SquareRegisters s)
        {
            base.setFromRegister(s);
            sweep_period = (s.NR10 & 0b0111_0000) >> 4;
            sweep_shift_number = s.NR10 & 0b0000_0111;
            sweep_direction = ((s.NR10 & 0b0000_1000) == 0) ? SweepDirection.ADD : SweepDirection.SUB;
            sweep_period_in_samples = (int)(((float)sweep_period / sweep_clock_freq) * (float)WaveFormat.SampleRate);
            sweep_timer = 0;
            shadow_frequency = (short)((s.NR14 & 0b00000111) << 8);

        }
        protected override float getNextSample()
        {
            if (sweep_period > 0)
            {
                sweep_timer++;
                if (sweep_timer >= sweep_period_in_samples)
                {
                    sweep_timer = 0;
                    sweepFrequency();
                }
            }
            return base.getNextSample();
        }
        private void sweepFrequency()
        {
            if (sweep_period == 0)
            {
                return;
            }
            int newval;
            switch (sweep_direction)
            {
                case SweepDirection.ADD:
                    newval = shadow_frequency + (shadow_frequency >> sweep_shift_number);
                    break;
                case SweepDirection.SUB:
                    newval = shadow_frequency - (shadow_frequency >> sweep_shift_number);
                    break;
                default:
                    newval = 0;
                    break;
            }
            if (newval <= 2047)
            {
                shadow_frequency = (short)newval;
                setFrequencyFromRaw(shadow_frequency);
            }
            else
            {
                active = false;
            }
        }
    }
}
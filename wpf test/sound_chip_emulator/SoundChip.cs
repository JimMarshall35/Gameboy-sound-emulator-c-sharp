using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NAudio;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System.Threading;

namespace GameBoySound
{
    public enum Duty : byte
    {
        D12 = 0,
        D25 = 1,
        D50 = 2,
        D75 = 3
    }
    public struct SquareRegisters
    {
        /*
                Square 1
            NR10 FF10 -PPP NSSS Sweep period, negate, shift
            NR11 FF11 DDLL LLLL Duty, Length load (64-L)
            NR12 FF12 VVVV APPP Starting volume, Envelope add mode, period
            NR13 FF13 FFFF FFFF Frequency LSB
            NR14 FF14 TL-- -FFF Trigger, Length enable, Frequency MSB

                Square 2
            FF15 ---- ---- Not used
            NR21 FF16 DDLL LLLL Duty, Length load (64-L)
            NR22 FF17 VVVV APPP Starting volume, Envelope add mode, period
            NR23 FF18 FFFF FFFF Frequency LSB
            NR24 FF19 TL-- -FFF Trigger, Length enable, Frequency MSB
         */
        public byte NR10;   // not used in square2
        public byte NR11;
        public byte NR12;
        public byte NR13;
        public byte NR14;
    }

    public struct WaveRegisters
    {
        /*
                Wave
            NR30 FF1A E--- ---- DAC power
            NR31 FF1B LLLL LLLL Length load (256-L)
            NR32 FF1C -VV- ---- Volume code (00=0%, 01=100%, 10=50%, 11=25%)
            NR33 FF1D FFFF FFFF Frequency LSB
            NR34 FF1E TL-- -FFF Trigger, Length enable, Frequency MSB
         */
        public byte NR30;
        public byte NR31;
        public byte NR32;
        public byte NR33;
        public byte NR34;
    }
    public struct NoiseRegisters 
    {
        /*
                Noise
                 FF1F ---- ---- Not used
            NR41 FF20 --LL LLLL Length load (64-L)
            NR42 FF21 VVVV APPP Starting volume, Envelope add mode, period
            NR43 FF22 SSSS WDDD Clock shift, Width mode of LFSR, Divisor code
            NR44 FF23 TL-- ---- Trigger, Length enable
         */
        public byte NR41;
        public byte NR42;
        public byte NR43;
        public byte NR44;
    }
    public struct SoundChipRegisters
    {
        public SquareRegisters square1;
        public SquareRegisters square2;
        public WaveRegisters wave;
        public NoiseRegisters noise;
        /*
                Control/Status
            NR50 FF24 ALLL BRRR Vin L enable, Left vol, Vin R enable, Right vol
            NR51 FF25 NW21 NW21 Left enables, Right enables
            NR52 FF26 P--- NW21 Power control/status, Channel length statuses
         
         */
        public byte NR50;
        public byte NR51;
        public byte NR52;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] wave_table; 

    }


    
    enum SoundChipChannels : int
    {
        SQUARE1 = (1<<0),
        SQUARE2 = (1<<2),
        WAVE    = (1<<3),
        NOISE   = (1<<4)
    }
    public class SoundChip
    {
        private SoundChipRegisters registers;
        private Square1 square1;
        private readonly MixingSampleProvider mixer;
        private readonly IWavePlayer outputDevice;
        public SoundChip()
        {
            //mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(16000, 1));
            registers.square1.NR14 = 0;//0b1100_0011;
            registers.square1.NR13 = 0;//255;// 0xbb;
            registers.square1.NR12 = 0;//0xff; //0x73;
            registers.square1.NR11 = 0;//0; //0x96;
            registers.square1.NR10 = 0;//0x15;
            square1 = new Square1(registers.square1);

            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(16000, 1));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();

            mixer.AddMixerInput(square1);
        }
        private void update(SoundChipChannels channels)
        {
            if(((int)channels & (int)SoundChipChannels.SQUARE1) !=  0)
            {
                square1.setFromRegister(registers.square1);
            }
        }
        

        public void setNR10(byte newval)
        {
            registers.square1.NR10 = newval;
            update(SoundChipChannels.SQUARE1);
        }
        public void setNR11(byte newval)
        {
            registers.square1.NR11 = newval;
            update(SoundChipChannels.SQUARE1);
        }
        public void setNR12(byte newval)
        {
            registers.square1.NR12 = newval;
            update(SoundChipChannels.SQUARE1);
        }
        public void setNR13(byte newval)
        {
            registers.square1.NR13 = newval;
            update(SoundChipChannels.SQUARE1);
        }
        public void setNR14(byte newval)
        {
            registers.square1.NR14 = newval;
            update(SoundChipChannels.SQUARE1);
        }


    }
}

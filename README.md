# Gameboy-sound-emulator-c-sharp

As i write this i've so far implemented square wave channel 1.

Channel 2 is the same except with no frequency sweep - should have done that one first and inherted it for channel 1 - going to now have to do it in reverse.

Wave channel and noise should be simple enough to implement.

Final UI will let you input real life values and have them be converted to register values, ie seconds for envelope and sweep, Hz for frequency, ect.
Will also let you input a wave table for the wave channel with an interactive graph. Will port oscilloscope code from "synthesizer" repository into wpf if possible,
also allow notes to be played with the keyboard.

possible eventual additions:
- Some kind of scripting language to play tunes and make sound effects that can also be used to generate z80 assembly code.
- some kind of sequencer to make music with

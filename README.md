![SCSP](https://imgur.com/dunQD3U.png)
## What is SCSP?
SCSP is a simple C#-based synthesizer. The synthesizer has one Oscillator for modulation (no additive synthesis), with volume slider (modifies the host's default playback device volume, unfortunately soundplayer() does not feature volume control in c#), pitch slider to play keys outside the pre-programmed 12 notes, sample rate slider & bits per sample for quality modulation. 

A total of 4-wave forms are playable, with a sine, square, saw and triangle. A fifth "white-noise" wave form which generates random frequency samples is also available. 
For some reason, I wasn't able to get the sample rate & bits per sample labels to initialize properly, so they're static for now until I figure it out. 

## Installation :
1. [Download](https://www.mediafire.com/file/du2o0c39umhfdvs/SCSP.zip/file)
2. run setup.exe
3. Enjoy!

## Built With

* [Visual Studio](https://visualstudio.microsoft.com/)

## Contributing

Please read CONTRIBUTING.MD for my rules and things to keep in mind when contributing

## Author(s)
Project and code was built by me.
Resources used :

http://soundfile.sapp.org/doc/WaveFormat/

https://docs.microsoft.com/en-us/archive/blogs/dawate/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c

https://www.youtube.com/watch?v=fp1Snqq9ovw

## License

This project is licensed under the GPLv3

![GNU GPLV3](https://imgur.com/imkUoGR.png)

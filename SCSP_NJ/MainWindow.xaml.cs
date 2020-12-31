using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Security.Permissions;
using System.Media;
using System.IO;
using System.Text;
using CSCore.CoreAudioAPI;
using AudioSwitcher.AudioApi.CoreAudio;

// summary : looking to set a bool to true through button, enabling selection between sine and square etc...
// passing a bool from private void grid to mainwindow

namespace SCSP_NJ
{
    public partial class MainWindow : Window
    {

        CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;


        public int SampleRate = 44100;
        public short BitsPerSample = 16;
        public short Amplitude = short.MaxValue;
        public bool sin = true;
        public bool square = false;
        public bool sawtooth = false;
        public bool triangle = false;
        public bool white_noise = false;
        public float frequency = 440f;
        //public const short Amplitude;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void SCSP1_KeyDown(object sender, KeyEventArgs e)
        {
            short[] wave = new short[SampleRate];


            if (e.Key == Key.H)
            {
                frequency = (float)PitchSlider.Value + 493f;
            }
            else if (e.Key == Key.J)
            {
                frequency = (float)PitchSlider.Value + 523f;
            }
            else if (e.Key == Key.Z)
            {
                frequency = (float)PitchSlider.Value + 466f;
            }
            else if (e.Key == Key.G)
            {
                frequency = (float)PitchSlider.Value + 440f;
            }
            else if (e.Key == Key.T)
            {
                frequency = (float)PitchSlider.Value + 415f;
            }
            else if (e.Key == Key.F)
            {
                frequency = (float)PitchSlider.Value + 391f;
            }
            else if (e.Key == Key.R)
            {
                frequency = (float)PitchSlider.Value + 369f;
            }
            else if (e.Key == Key.D)
            {
                frequency = (float)PitchSlider.Value + 349f;
            }
            else if (e.Key == Key.E)
            {
                frequency = (float)PitchSlider.Value + 329f;
            }
            else if (e.Key == Key.S)
            {
                frequency = (float)PitchSlider.Value + 311f;
            }
            else if (e.Key == Key.W)
            {
                frequency = (float)PitchSlider.Value + 293f;
            }
            else if (e.Key == Key.A)
            {
                frequency = (float)PitchSlider.Value + 277f;
            }
            else if (e.Key == Key.Q)
            {
                frequency = (float)PitchSlider.Value + 261f;
            }
            try
            {


                // conversion from short to byte 
                byte[] binaryWave = new byte[SampleRate * sizeof(short)];
                short tempSample;

                int samplesPerWaveLength = (int)(SampleRate / frequency);
                if (samplesPerWaveLength < 0)
                {
                    samplesPerWaveLength = 1;
                }
                short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLength);


                Random random = new Random();
                if (sin)
                {
                    for (int i = 0; i < SampleRate; i++)
                    {
                        // standar sin() equation 
                        // i = unit of time for each sample 
                        // w = 2 x pi x f
                        // short.MaxValue = max. possible amplitude 

                        wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SampleRate) * i));
                    }
                }
                else if (square)
                {
                    for (int j = 0; j < SampleRate; j++)
                    {
                        for (int channel = 0; channel < 2; channel++)
                        {
                            wave[j] = Convert.ToInt16(short.MaxValue * Math.Sign(Math.Sin((Math.PI * 2 * frequency) / SampleRate * j)));
                        }

                    }
                }
                else if (sawtooth)
                {
                    for (int k = 0; k < SampleRate; k++)
                    {
                        tempSample = -short.MaxValue;
                        for (int l = 0; l < samplesPerWaveLength && k < SampleRate; l++)
                        {
                            tempSample += ampStep;
                            wave[k++] = Convert.ToInt16(tempSample);
                        }

                    }
                }
                else if (triangle)
                {
                    tempSample = short.MaxValue;
                    for (int m = 0; m < SampleRate; m++)
                    {
                        if (Math.Abs(tempSample + ampStep) > short.MaxValue)
                        {
                            ampStep = (short)-ampStep;
                        }
                        tempSample += ampStep;
                        wave[m] = Convert.ToInt16(tempSample);
                    }
                }
                else if (white_noise)
                {
                    for (int n = 0; n < SampleRate; n++)
                    {
                        wave[n] += Convert.ToInt16(random.Next(-short.MaxValue, short.MaxValue));
                    }
                }

                //Split every short of wave into two bytes
                //Buffer.BlockCopy(src array, offset, destination array, start location, number of bytes to copy)
                Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));
                // converting data to .WAV
                // Writing first block of the .WAV, ChunkID
                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    // next after ChunkID is ChunckSize, which depends on subChunkSize
                    // SubchunkSize is calculated by multiplying Nb. of sample * Audio Channels (in our case 1) * the BitsPerSample / 8 (blockAlign)
                    short blockAlign = (short)(BitsPerSample / 8);
                    int subChunk2Size = SampleRate * blockAlign;
                    //ChunkID
                    binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });
                    //ChunckSize
                    binaryWriter.Write(36 + subChunk2Size);
                    //Format
                    binaryWriter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                    //Subchunk1ID
                    //Subchunk1Size
                    binaryWriter.Write(16);
                    // Audio Format (1 for PCM), has to be cast to short since this block is in 2- bits instead of 4
                    binaryWriter.Write((short)1);
                    //Number of channels = 1
                    binaryWriter.Write((short)1);
                    //SampleRate
                    binaryWriter.Write(SampleRate);
                    //ByteRate = blockAlign
                    binaryWriter.Write(SampleRate * blockAlign);
                    //blockAlign in 2 bits (short)
                    binaryWriter.Write(blockAlign);
                    //BitsPerSample
                    binaryWriter.Write(BitsPerSample);
                    //dataID
                    binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
                    //Subchunk2size again
                    binaryWriter.Write(subChunk2Size);
                    //the actual sound data, in 1 Byte intervals (need to convert wave 16-bit to 8bit)
                    binaryWriter.Write(binaryWave);

                    // Playback, position 0 = beginning of stream
                    memoryStream.Position = 0;
                    new SoundPlayer(memoryStream).Play();
                    //System.Windows.MessageBox.Show("hello");

                }
            }
            catch (System.Exception)
            {
                System.Windows.MessageBox.Show("An error has occured during generation.");

            }
        }
        


        private void Minimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SineC_Checked(object sender, RoutedEventArgs e)
        {
            SquareC.IsChecked = false;
            SawC.IsChecked = false;
            TriangleC.IsChecked = false;
            WN_C.IsChecked = false;
            sin = true;
            square = false;
            triangle = false;
            sawtooth = false;
            white_noise = false;

        }

        private void SquareC_Checked(object sender, RoutedEventArgs e)
        {
            SineC.IsChecked = false;
            SawC.IsChecked = false;
            TriangleC.IsChecked = false;
            WN_C.IsChecked = false;
            sin = false;
            square = true;
            triangle = false;
            sawtooth = false;
            white_noise = false;
        }

        private void SawC_Checked(object sender, RoutedEventArgs e)
        {
            SquareC.IsChecked = false;
            SineC.IsChecked = false;
            TriangleC.IsChecked = false;
            WN_C.IsChecked = false;
            sin = false;
            square = false;
            triangle = false;
            sawtooth = true;
            white_noise = false;
        }

        private void TriangleC_Checked(object sender, RoutedEventArgs e)
        {
            SquareC.IsChecked = false;
            SawC.IsChecked = false;
            SineC.IsChecked = false;
            WN_C.IsChecked = false;
            sin = false;
            square = false;
            triangle = true;
            sawtooth = false;
            white_noise = false;
        }

        private void WN_C_Checked(object sender, RoutedEventArgs e)
        {
            SquareC.IsChecked = false;
            SawC.IsChecked = false;
            TriangleC.IsChecked = false;
            SineC.IsChecked = false;
            sin = false;
            square = false;
            triangle = false;
            sawtooth = false;
            white_noise = true;
        }
        private void Volume_Initialized(object sender, EventArgs e)
        {

            Volume.Content = "-";
        }

        private void AmplitudeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            defaultPlaybackDevice.Volume = (int)AmplitudeSlider.Value;
            String output = "";
            int SliderVal = (int)AmplitudeSlider.Value;
            Volume.Content = SliderVal.ToString(output) + "%";
        }

        private void PitchSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Pitch.Content = (int)PitchSlider.Value;
        }

        private void SampleRateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SampleRate = (int)SampleRateSlider.Value + 1;

        }


        private void BitsPerSampleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void AmplitudeSlider_Initialized(object sender, EventArgs e)
        {
            AmplitudeSlider.IsSnapToTickEnabled = true;
        }

        private void PitchSlider_Initialized(object sender, EventArgs e)
        {
            PitchSlider.IsSnapToTickEnabled = true;
        }

        private void SampleRateSlider_Initialized(object sender, EventArgs e)
        {
            SampleRateSlider.IsSnapToTickEnabled = true;
        }

        private void BitsPerSampleSlider_Initialized(object sender, EventArgs e)
        {
            BitsPerSampleSlider.IsSnapToTickEnabled = true;
        }
    }


}

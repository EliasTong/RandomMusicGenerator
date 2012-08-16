using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.ComponentModel;

namespace Random_Song_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string fileName = null;
        private string folderName = null;
        private int iterations = -1;
        private long workStarted;
        private BackgroundWorker worker;
        private int currentSong = 0;
        private long maxSize = -1; //in kilobytes
        private int songsPerAlbum = -1;
        private int avgSongsPerAlbum = -1;
        private bool randomizeSongsPerAlbum = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void browseSourceClicked(object sender, RoutedEventArgs e)
        {
            sourceTextBox.Background = Brushes.White;
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".mp3"; // Default file extension
            dlg.Filter = "MP3 Files (.mp3)|*.mp3|M4A Files (.m4a)|*.m4a"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                fileName = dlg.FileName;
                sourceTextBox.Text = fileName;
            }
        }

        //Bah, have to use windows forms!
        private void browseDestClicked(object sender, RoutedEventArgs e)
        {
            destTextBox.Background = Brushes.White;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folderName = dialog.SelectedPath;
                destTextBox.Text = folderName;
            }
        }

        private void startClicked(object sender, RoutedEventArgs e)
        {
            errorText.Content = "";

            if (checkIteration() && checkMaxSize() && checkSongsPerAlbum() && checkAvgSongsPerAlbum() && checkSource() && checkDest())
            {
                //must calculate the number of iterations based on the given maximum size
                if (!(bool)infinityCheckBox.IsChecked)
                {
                    calculateIterationsByMaxSize();
                }
                if ((bool)randomCheckBox.IsChecked)
                {
                    randomizeSongsPerAlbum = true;
                }
                start();
            }
        }

        private void calculateIterationsByMaxSize()
        {
            FileInfo f = new FileInfo(fileName);
            long fileLength = f.Length / 1000;   //in bytes - must convert to kb
            iterations = (int)(maxSize / fileLength);
            iterationsLabel.Content = iterations;
            iterationsTextBox.Text = iterations.ToString();
            Console.WriteLine("Need " + iterations + " iterations to fill " + maxSize + " kb");
        }

        private bool checkIteration()
        {
            string iterationsText = iterationsTextBox.Text;
            if (iterationsText == "" && !(bool)copyCheckBox.IsChecked)
            {
                iterationsTextBox.Background = Brushes.LavenderBlush;
                errorText.Content = "Iterations cannot be empty";
                return false;
            }
            //can be blank if the copy check box is checked
            else if (iterationsText == "")
            {
                //do nothing
                return true;
            }
            else
            {
                try
                {
                    iterations = Convert.ToInt32(iterationsText);
                    if (iterations < 0)
                    {
                        errorText.Content = "Iteration must be positive";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (FormatException fe)
                {
                    errorText.Content = "Iteration is not a sequence of digits";
                }
                catch (OverflowException oe)
                {
                    errorText.Content = "Iteration value cannot be represented with just 32 bits";
                }
            }
            return false;
        }

        private bool checkMaxSize()
        {
            string maxSizeText = maxSizeTextBox.Text;
            if (maxSizeText == "" && !(bool)infinityCheckBox.IsChecked)
            {
                maxSizeTextBox.Background = Brushes.LavenderBlush;
                errorText.Content = "Max size cannot be empty";
                return false;
            }
            //can be blank if infinity check box is checked
            else if (maxSizeText == "")
            {
                //do nothing
                return true;
            }
            else
            {
                try
                {
                    maxSize = Convert.ToInt64(maxSizeText);
                    if (maxSize < 0)
                    {
                        errorText.Content = "Max size must be positive";
                        return false;
                    }
                    else
                    {
                        if (maxSizeComboBox.SelectedIndex == 0)
                        {
                            //do nothing -we're storing max size as kb already
                            Console.WriteLine("Max Size is " + maxSize + " kb");
                        }
                        else if (maxSizeComboBox.SelectedIndex == 1)
                        {
                            maxSize = convertMBtoKB(maxSize);
                            Console.WriteLine("Max Size is " + maxSize + " kb");
                        }
                        else
                        {
                            maxSize = convertGBtoKB(maxSize);
                            Console.WriteLine("Max Size is " + maxSize + " kb");
                        }
                        return true;
                    }
                }
                catch (FormatException fe)
                {
                    errorText.Content = "Max Size is not a sequence of digits";
                }
                catch (OverflowException oe)
                {
                    errorText.Content = "Max Size value cannot be represented with just 32 bits";
                }
            }
            return false;
        }

        private bool checkSongsPerAlbum()
        {
            string songsPerAlbumText = songsPerAlbumTextBox.Text;
            if (songsPerAlbumText == "" && !(bool)randomCheckBox.IsChecked)
            {
                songsPerAlbumTextBox.Background = Brushes.LavenderBlush;
                errorText.Content = "Songs per album cannot be empty";
                return false;
            }
            //can be blank if random check box is checked
            else if (songsPerAlbumText == "")
            {
                //do nothing
                return true;
            }
            else
            {
                try
                {
                    songsPerAlbum = Convert.ToInt32(songsPerAlbumText);
                    if (songsPerAlbum < 0)
                    {
                        errorText.Content = "Songs per album must be positive";
                        return false;
                    }
                    else if (songsPerAlbum == 0)
                    {
                        errorText.Content = "Songs per album must be > 0";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (FormatException fe)
                {
                    errorText.Content = "Songs per album is not a sequence of digits";
                }
                catch (OverflowException oe)
                {
                    errorText.Content = "Songs per album value cannot be represented with just 32 bits";
                }
            }
            return false;
        }

        private bool checkAvgSongsPerAlbum()
        {
            string avgSongsPerAlbumText = avgSongsPerAlbumTextBox.Text;
            if (avgSongsPerAlbumText == "" && (bool)randomCheckBox.IsChecked)
            {
                avgSongsPerAlbumTextBox.Background = Brushes.LavenderBlush;
                errorText.Content = "Avg songs per album cannot be empty";
                return false;
            }
            //can be blank if random check box is checked
            else if (avgSongsPerAlbumText == "")
            {
                //do nothing
                return true;
            }
            else
            {
                try
                {
                    avgSongsPerAlbum = Convert.ToInt32(avgSongsPerAlbumText);
                    if (avgSongsPerAlbum < 0)
                    {
                        errorText.Content = "Avg songs per album must be positive";
                        return false;
                    }
                    else if (avgSongsPerAlbum == 0)
                    {
                        errorText.Content = "Avg songs per album must be > 0";
                        return false;
                    }
                    else if (avgSongsPerAlbum > 10)
                    {
                        errorText.Content = "Avg songs per album must be <= 10";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (FormatException fe)
                {
                    errorText.Content = "Avg songs per album is not a sequence of digits";
                }
                catch (OverflowException oe)
                {
                    errorText.Content = "Avg songs per album value cannot be represented with just 32 bits";
                }
            }
            return false;
        }

        private bool checkSource()
        {
            if (sourceTextBox.Text == "")
            {
                sourceTextBox.Background = Brushes.LavenderBlush;
                return false;
            }
            else
            {
                fileName = sourceTextBox.Text;
                return true;
            }
        }


        private bool checkDest()
        {
            if (destTextBox.Text == "")
            {
                destTextBox.Background = Brushes.LavenderBlush;
                return false;
            }
            else
            {
                folderName = destTextBox.Text;
                return true;
            }
        }


        //start generator
        private void start()
        {
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            statusMessage.Content = "Working";
            workStarted = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Random rand = new Random();
            int albumInc = 0;
            int maxSongs = 1;   //songs per album
            int[] seeds = null; //if random songs per album, needs array of seeds
            if (randomizeSongsPerAlbum)
            {
                seeds = SongsPerAlbumSeeds.getSeeds(avgSongsPerAlbum);
                maxSongs = generateTrackCountForAlbum(seeds);
            }
            else
            {
                maxSongs = songsPerAlbum;
            }
            string album = NameGenerator.generateName();
            string artist = NameGenerator.generateName();
            for (int i = 0; i < iterations; i++)
            {
                if (worker.CancellationPending)
                {
                    break;
                }
                currentSong = i + 1;
                float prog = ((i + 1) / (float)iterations);
                Console.WriteLine("PROGRESS " + prog);
                worker.ReportProgress( (int)(prog * 100),"Working");
                string songName = NameGenerator.generateName();
                if (albumInc == maxSongs)
                {
                    album = NameGenerator.generateName();
                    artist = NameGenerator.generateName();
                    albumInc = 0;
                    if (randomizeSongsPerAlbum)
                    {
                        maxSongs = generateTrackCountForAlbum(seeds);
                    }
                }
                albumInc++;
                int trackNumber = rand.Next(1, 21);
                string newPath = folderName + "\\" + songName + System.IO.Path.GetExtension(fileName);
                try
                {
                    System.IO.File.Copy(fileName, newPath, true);           //copy song to destination folder
                }
                catch(IOException ioe){
                    Console.WriteLine("Failed To Write To Disk: " + ioe.Message);
                    continue;
                }
                //now alter metadata
                TagLib.File tagFile = TagLib.File.Create(newPath); // track is the name of the mp3
                tagFile.Tag.Title = songName;
                tagFile.Tag.Album = album;
                tagFile.Tag.AlbumArtists = new string[] { artist };
                tagFile.Tag.Performers = new string[] { artist };
                tagFile.Tag.Track = (uint)trackNumber;
                tagFile.Save();
            }
        }

        private int generateTrackCountForAlbum(int[] seeds)
        {
            if (seeds != null)
            {
                Random rand = new Random();
                return seeds[rand.Next(0,seeds.Length)];
            }
            else
            {
                return 1;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Progress Changed: " + e.ProgressPercentage);
            progressBar.Value = e.ProgressPercentage;
            percentageLabel.Content = e.ProgressPercentage + "%";
            currentSongLabel.Content = currentSong;
        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            startButton.IsEnabled = true;
            long workEnded = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            statusMessage.Content = "Finished in " + (workEnded - workStarted) + " ms [" + ((workEnded - workStarted)/1000) + " s]";
        }

        private void iterationClicked(object sender, System.Windows.Input.MouseEventArgs e)
        {
            iterationsTextBox.Background = Brushes.White;
        }

        private void testClicked(object sender, RoutedEventArgs e)
        {
            if (checkIteration() && checkMaxSize() && checkSource() && checkDest())
            {
                //must calculate the number of iterations based on the given maximum size
                if (!(bool)infinityCheckBox.IsChecked)
                {
                    calculateIterationsByMaxSize();
                }
            }
        }

        private void iterationTextChanged(object sender, TextChangedEventArgs e)
        {
            iterationsLabel.Content = iterationsTextBox.Text;
        }

        private void copyChecked(object sender, RoutedEventArgs e)
        {
            iterationsTextBox.IsEnabled = false;
            infinityCheckBox.IsEnabled = false;
        }

        private void copyUnchecked(object sender, RoutedEventArgs e)
        {
            iterationsTextBox.IsEnabled = true;
            infinityCheckBox.IsEnabled = true;
        }

        private long convertMBtoKB(long mb)
        {
            return mb * 1000;
        }

        private long convertGBtoKB(long gb)
        {
            return convertMBtoKB(gb * 1000);
        }

        private void maxSizeEntered(object sender, System.Windows.Input.MouseEventArgs e)
        {
            maxSizeTextBox.Background = Brushes.White;
        }

        private void infinityChecked(object sender, RoutedEventArgs e)
        {
            maxSizeTextBox.IsEnabled = false;
            maxSizeComboBox.IsEnabled = false;
            copyCheckBox.IsEnabled = false;
        }

        private void infinityUnchecked(object sender, RoutedEventArgs e)
        {
            maxSizeTextBox.IsEnabled = true;
            maxSizeComboBox.IsEnabled = true;
            copyCheckBox.IsEnabled = true;
        }

        private void randomChecked(object sender, RoutedEventArgs e)
        {
            songsPerAlbumTextBox.IsEnabled = false;
            avgSongsPerAlbumTextBox.IsEnabled = true;
        }

        private void randomUnchecked(object sender, RoutedEventArgs e)
        {
            songsPerAlbumTextBox.IsEnabled = true;
            avgSongsPerAlbumTextBox.IsEnabled = false;
        }

        private void stopClicked(object sender, RoutedEventArgs e)
        {
            if (worker.IsBusy)
            {
                startButton.IsEnabled = true;
                stopButton.IsEnabled = false;
                worker.CancelAsync();
                progressBar.Value = 0;
                percentageLabel.Content = "0%";
                iterationsLabel.Content = iterationsTextBox.Text;
                currentSongLabel.Content = "0";
                statusMessage.Content = "Stopped";
            }
        }
    }

}

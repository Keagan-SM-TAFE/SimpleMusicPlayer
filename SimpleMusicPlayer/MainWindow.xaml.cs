using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NAudio.Wave;

namespace SimpleMusicPlayer
{
    /**
     * Author:             Keagan Young
     * Date:               16 - 06 - 2021
     * Project Name:       Music Player Project Assesment.WPF application.
     * Version:            1.6
     * Description:        This project was created as part of a Diploma in Software Development.
     *                     The requirements of this task are:
     *                          - Must contain dynamic data structures
     *                          - Must contain hashing techniques
     *                          - Must contain sorting algorithm
     *                          - Must contain searching technique
     *                          - Must contain 3rd party library
     *                          - Must have a GUI
     *                          - Must adhere to coding standards
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool songTimer = false;
        bool songSearched = false;
        bool pauseSong = false;
        // NAudio Variables
        private readonly WaveOutEvent waveOut;
        private MediaFoundationReader media;
        // Timer
        DispatcherTimer Timer;
        // Linked List
        // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.linkedlist-1?view=net-5.0
        LinkedListNode<string> node;
        readonly LinkedList<string> songPlaylist = new LinkedList<string>();
        readonly ObservableCollection<SongsList> songItems = new ObservableCollection<SongsList>();

        /// <summary>
        /// Initialize main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            waveOut = new WaveOutEvent();
        }

        /// <summary>
        /// Drag window event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs @event)
        {
            this.DragMove();
        }

        /// <summary>
        /// Window exit button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Btn_exit_Click(object sender, RoutedEventArgs @event)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Window maximize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Btn_maximize_Click(object sender, RoutedEventArgs @event)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Window minimize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Btn_minimize_Click(object sender, RoutedEventArgs @event)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Search song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnSeach_Click(object sender, RoutedEventArgs @event)
        {
            if (songPlaylist.Count > 0 && songPlaylist.First != null && songPlaylist.Last != null)
            {
                string target = "";
                if (BinarySearch(songPlaylist.First, songPlaylist.Last) != null)
                {
                    target = BinarySearch(songPlaylist.First, songPlaylist.Last).Value;
                }
                foreach (var item in songItems)
                {
                    if (item.SongPath == target)
                    {
                        item.Backcolor = "#66FFF300";
                    }
                }
                songSearched = true;
            }
        }

        /// <summary>
        /// Reset song searched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnReset_Click(object sender, RoutedEventArgs @event)
        {
            if (songSearched == true)
            {
                foreach (var item in songItems)
                {
                    item.Backcolor = "{x:Null}";
                }
                txtSongName.Clear();
                songSearched = false;
            }
        }

        /// <summary>
        /// Add a new song button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void SelectFileButton_Click(object sender, RoutedEventArgs @event)
        {
            string songName;
            // Create a new OpenFileDialog
            OpenFileDialog openFileDL = new OpenFileDialog();
            // Make sure the dialog checks for existence of the selected file
            openFileDL.Multiselect = true;
            openFileDL.CheckFileExists = true;
            // Allow selection of all audio file types only
            openFileDL.Filter = "Audio (*.mp3,*.mp4,*.wav,*.m4a,*.flac,*.acc,*wma)|*.mp3;*.mp4;*.wav;*.m4a;*.flac;*.aac;*.wma|All Files (*.*)|*.*";
            Nullable<bool> ofdResult = openFileDL.ShowDialog();
            // Activate the file selection dialog
            if (ofdResult == true)
            {
                foreach (var item in openFileDL.FileNames)
                {
                    // Get the selected file's path from the dialog
                    string[] data = item.Split('\\');
                    songName = data[^1];
                    // Add to the List View
                    songItems.Add(new SongsList(songName, item, "{x:Null}"));
                    // Add to the List View
                    songPlaylist.AddLast(item);
                    txtSongCounts.Text = songPlaylist.Count.ToString();
                }
            }
            SortList();
        }

        /// <summary>
        /// Count current time of the song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void Timer_Tick(object sender, EventArgs @event)
        {
            if (media.CurrentTime < media.TotalTime)
            {
                txtPosition.Text = media.CurrentTime.ToString(@"hh\:mm\:ss");
                sldTime.Value = media.CurrentTime.TotalSeconds;
            }
            else
            {
                waveOut.Stop();
                sldTime.Value = 0;
                media.Position = 0;
                Timer.Stop();
                songTimer = false;
            }
        }

        /// <summary>
        /// Play song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void PlayOnceAsyncButton_Click(object sender, RoutedEventArgs @event)
        {
            if (songPlaylist.Count > 0)
            {
                if (pauseSong == true)
                {
                    waveOut.Play();
                    Timer.Start();
                    pauseSong = false;
                }
                else
                {
                    PlaySong();
                }
            }
        }

        /// <summary>
        /// Stop song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void StopButton_Click(object sender, RoutedEventArgs @event)
        {
            if (pauseSong == false)
            {
                waveOut.Pause();
                Timer.Stop();
                pauseSong = true;
            }
        }

        /// <summary>
        /// Next song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnNext_Click(object sender, RoutedEventArgs @event)
        {
            if (node.Next == null)
            {
                MessageBox.Show("Please add a song to the playlist!");
            }
            else
            {
                StopSong();
                node = node.Next;
                LoadSong(node.Value);
                PlaySong();
                pauseSong = false;
            }
        }

        /// <summary>
        /// Previous song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnPre_Click(object sender, RoutedEventArgs @event)
        {
            if (node.Previous == null)
            {
                MessageBox.Show("There is no previous song in the playlist!");
            }
            else
            {
                StopSong();
                btnPre.IsEnabled = true;
                node = node.Previous;
                LoadSong(node.Value);
                PlaySong();
                pauseSong = false;
            }
        }

        /// <summary>
        /// Play first song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnFirst_Click(object sender, RoutedEventArgs @event)
        {
            StopSong();
            node = songPlaylist.First;
            LoadSong(node.Value);
            PlaySong();
            pauseSong = false;
        }

        /// <summary>
        /// Play last song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void BtnLast_Click(object sender, RoutedEventArgs @event)
        {
            StopSong();
            node = songPlaylist.Last;
            LoadSong(node.Value);
            PlaySong();
            pauseSong = false;
        }

        /// <summary>
        /// Click song in list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs @event)
        {
            SongsList list = songItems[lvSongs.SelectedIndex];
            songItems.RemoveAt(lvSongs.SelectedIndex);
            songPlaylist.Remove(list.SongPath);
            lvSongs.ItemsSource = songItems;
            if (songPlaylist.Count > 0)
            {
                StopSong();
                node = songPlaylist.First;
                LoadSong(node.Value);
            }
            else
            {
                sldTime.Value = 0;
                txtPosition.Text = "00:00:00";
                txtFullLength.Text = "00:00:00";
                txtName.Text = "";
            }
        }

        //
        //
        // User Defined
        // Functions
        //
        //

        /// <summary>
        /// Play a song function
        /// </summary>
        private void PlaySong()
        {
            // Stop if a song is playing to start new one
            if (songTimer == true)
            {
                Timer.Stop();
            }
            // Time counter of the song
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            // Set max width of the slider
            sldTime.Maximum = media.TotalTime.TotalSeconds;
            sldTime.Value = 0;
            // Play song
            waveOut.Play();
            Timer.Start();
            songTimer = true;
        }

        /// <summary>
        /// Stop song function
        /// </summary>
        private void StopSong()
        {
            // Stop and reset all controls
            waveOut.Stop();
            media.Position = 0;
            if (songTimer == true)
            {
                Timer.Stop();
            }
            sldTime.Value = 0;
            songTimer = false;
            txtPosition.Text = "00:00:00";
        }

        /// <summary>
        /// Load song function
        /// </summary>
        /// <param name="path"></param>
        private void LoadSong(string path)
        {
            // Load the next/prev/first/last/current song
            try
            {
                media = new MediaFoundationReader(path);
                waveOut.Init(media);
                TimeSpan timespan = media.TotalTime;
                txtFullLength.Text = timespan.ToString(@"hh\:mm\:ss");
                string[] data = path.Split('\\');
                txtName.Text = data[^1];
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Sort song List finction
        /// </summary>
        private void SortList()
        {
            if (songPlaylist.Count > 0)
            {
                LinkedListNode<string> sorted = MergeSort(songPlaylist.First, songPlaylist.Last);
                string name, path;
                songPlaylist.Clear();
                songItems.Clear();
                while (sorted != null)
                {
                    path = sorted.Value;
                    songPlaylist.AddLast(sorted.Value);
                    string[] data = path.Split('\\');
                    name = data[^1];
                    songItems.Add(new SongsList(name, path, "{x:Null}"));
                    sorted = sorted.Next;
                }
                node = songPlaylist.First;
                LoadSong(node.Value);
                lvSongs.ItemsSource = songItems;
            }
        }

        /// <summary>
        /// Merge sort function
        /// </summary>
        /// <param name="Head"></param>
        /// <param name="Tail"></param>
        /// <returns></returns>
        private LinkedListNode<string> MergeSort(LinkedListNode<string> Head, LinkedListNode<string> Tail)
        {
            if (Head == Tail)
            {
                LinkedList<string> linkedList = new LinkedList<string>();
                linkedList.AddLast(Head.Value);
                return linkedList.First;
            }
            LinkedListNode<string> middle = MidNode(Head, Tail);
            LinkedListNode<string> firstList = MergeSort(Head, middle);
            LinkedListNode<string> secondList = MergeSort(middle.Next, Tail);
            LinkedListNode<string> combinedList = MergeTwoSortedList(firstList, secondList);
            return combinedList;
        }

        /// <summary>
        /// Mid Node function
        /// </summary>
        /// <param name="Head"></param>
        /// <param name="Tail"></param>
        /// <returns></returns>
        private static LinkedListNode<string> MidNode(LinkedListNode<string> Head, LinkedListNode<string> Tail)
        {
            LinkedListNode<string> listOne = Head;
            LinkedListNode<string> listTwo = Head;
            while (listOne != Tail && listOne.Next != Tail)
            {
                listOne = listOne.Next.Next;
                listTwo = listTwo.Next;
            }
            return listTwo;
        }

        /// <summary>
        /// Merge two sorted list function
        /// </summary>
        /// <param name="firstList"></param>
        /// <param name="secondList"></param>
        /// <returns></returns>
        private static LinkedListNode<string> MergeTwoSortedList(LinkedListNode<string> firstList, LinkedListNode<string> secondList)
        {
            LinkedListNode<string> first = firstList;
            LinkedListNode<string> second = secondList;

            LinkedList<string> resultlist = new LinkedList<string>();

            while (first != null && second != null)
            {
                if (string.Compare(first.Value, second.Value, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    resultlist.AddLast(first.Value);
                    first = first.Next;
                }
                else
                {
                    resultlist.AddLast(second.Value);
                    second = second.Next;
                }
            }
            while (first != null)
            {
                resultlist.AddLast(first.Value);
                first = first.Next;
            }
            while (second != null)
            {
                resultlist.AddLast(second.Value);
                second = second.Next;
            }

            return resultlist.First;
        }

        /// <summary>
        /// Binary search function
        /// </summary>
        /// <param name="Low"></param>
        /// <param name="High"></param>
        /// <returns></returns>
        private LinkedListNode<string> BinarySearch(LinkedListNode<string> Low, LinkedListNode<string> High)
        {
            while (string.Compare(Low.Value, High.Value, StringComparison.Ordinal) <= 0)
            {
                LinkedListNode<string> mid = MidNode(Low, High);
                string[] data = mid.Value.Split('\\');
                data = data[^1].Split('.');
                if (string.Compare(txtSongName.Text, data[0], StringComparison.Ordinal) > 0)
                {
                    Low = mid.Next;
                }
                else if (string.Compare(txtSongName.Text, data[0], StringComparison.Ordinal) < 0)
                {
                    High = mid.Previous;
                }
                else
                {
                    return mid;
                }
                if (High == null)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
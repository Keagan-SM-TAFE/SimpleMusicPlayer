using System;
using System.ComponentModel;

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
    /// SongsList
    /// </summary>
    class SongsList : INotifyPropertyChanged
    {
        private string songName;
        private string songPath;
        private string backcolor;

        /// <summary>
        /// SongName
        /// </summary>
        public String SongName
        {
            get 
            { 
                return songName; 
            }
            set
            {
                if (songName == value)
                {
                    return;
                }
                songName = value;
                RaisePropertyChanged("SongName");
            }
        }

        /// <summary>
        /// SongPath
        /// </summary>
        public String SongPath
        {
            get 
            { 
                return songPath; 
            }
            set
            {
                if (songPath == value)
                {
                    return;
                }
                songPath = value;
                RaisePropertyChanged("SongPath");
            }
        }

        /// <summary>
        /// Backcolor
        /// </summary>
        public String Backcolor
        {
            get 
            { 
                return backcolor; 
            }
            set
            {
                if (backcolor == value)
                {
                    return;
                }
                backcolor = value;
                RaisePropertyChanged("Backcolor");
            }
        }

        /// <summary>
        /// SongsList
        /// </summary>
        /// <param name="songName"></param>
        /// <param name="songPath"></param>
        /// <param name="backColour"></param>
        public SongsList(string songName, string songPath, string backColour)
        {
            this.SongName = songName;
            this.SongPath = songPath;
            this.Backcolor = backColour;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// RaisePropertyChanged
        /// </summary>
        /// <param name="propName"></param>
        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
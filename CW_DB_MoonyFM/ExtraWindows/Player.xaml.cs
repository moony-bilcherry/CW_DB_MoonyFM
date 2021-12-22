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
using System.Windows.Shapes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
using Microsoft.Win32;
using System.IO;

namespace CW_DB_MoonyFM.ExtraWindows
{
    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        Int32 songId;
        string artistNameVal, albumNameVal, songNameVal;
        byte[] audioByteArr;
        MediaPlayer mediaPlayerObj = new MediaPlayer();

        public Player()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT song_blob FROM DBMOONYFM.song_table WHERE song_id = " + songId.ToString();
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    audioByteArr = reader.GetValue(0) as byte[];

                    using (FileStream bytesToAudio = File.Create("current.mp3"))
                    {
                        bytesToAudio.Write(audioByteArr, 0, audioByteArr.Length);
                        Stream audioFile = bytesToAudio;
                        bytesToAudio.Close();
                    }

                    mediaPlayerObj.Open(new Uri(@"T:\Uni\5_sem\CW_DB\CW_DB_MoonyFM\CW_DB_MoonyFM\bin\Debug\current.mp3"));
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Stop();
        }

        public Player(Int32 id, string artist, string album, string song)
        {
            InitializeComponent();
            con.ConnectionString = connectionString;

            this.songId = id;
            this.artistNameVal = artist;
            this.albumNameVal = album;
            this.songNameVal = song;

            artistName.Text = artist;
            albumName.Text = album;
            songName.Text = song;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerObj.Close();
            this.Close();
            Application.Current.Windows[0].Show();
        }
    }
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
using System.IO;
using CW_DB_MoonyFM.ExtraWindows;

namespace CW_DB_MoonyFM.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SongUC.xaml
    /// </summary>
    public partial class SongUC : UserControl
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        Int32 year, songId;
        string song, album, artist;
        byte[] cover, audio;

        private void playSong_Click(object sender, RoutedEventArgs e)
        {
            Player play = new Player(this.songId, this.artist, this.album, this.song);
            play.Show();
            Application.Current.Windows[0].Hide();
        }

        private void deleteSong_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Вы уверены, что хотите удалить эту песню?", "Удаление песни", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.DELETE_SONG";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_ID", OracleDbType.Int32, 10).Value = songId;
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Песня успешно удалена");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Ошибка при удалении песни");
                }
                con.Close();
            }
        }

        private void saveSong_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Хотите добавить песню в плейлист?", "Добавить в плейлист", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.SAVE_SONG";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_USER_ID", OracleDbType.Int32, 10).Value = DataWorker.CurrentUserId;
            cmd.Parameters.Add("P_SONG_ID", OracleDbType.Int32, 10).Value = songId;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Песня успешно сохранена в ваш плейлист!");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибка при добавлени песни в плейлист");
            }
            con.Close();
        }

        private void editSong_Click(object sender, RoutedEventArgs e)
        {
            UpdateSong updateWin = new UpdateSong(this.songId, this.artist, this.album, this.song);
            updateWin.Show();
            Application.Current.Windows[0].Hide();
        }

        private void checkRole()
        {
            if (DataWorker.CurrentUserRole == "OWNER")
            {
                adminButtons.Visibility = Visibility.Visible;
                return;
            }
            else return;
        }

        public SongUC()
        {
            InitializeComponent();
            checkRole();
            con.ConnectionString = connectionString;
        }

        public SongUC(Int32 id, string artistName, string albumName, string songName, Int16 yearReleased)
        {
            con.ConnectionString = connectionString;
            InitializeComponent();
            checkRole();
            this.songId = id;
            this.artist = artistName;
            this.album = albumName;
            this.song = songName;
            this.year = yearReleased;

            blockArtistName.Text = artistName;
            blockAlbumName.Text = albumName;
            blockSongName.Text = songName;
            blockYear.Text = yearReleased.ToString();

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT album_blob FROM DBMOONYFM.artist_album_song_view WHERE song_id = " + id.ToString();
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    cover = reader.GetValue(0) as byte[];
                    image.StreamSource = new MemoryStream(reader.GetValue(0) as byte[]);
                    image.EndInit();
                    albumCover.Source = image;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            con.Close();
        }
    }
}

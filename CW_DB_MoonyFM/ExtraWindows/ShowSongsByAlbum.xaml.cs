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
using CW_DB_MoonyFM.UserControls;

namespace CW_DB_MoonyFM.ExtraWindows
{
    /// <summary>
    /// Логика взаимодействия для ShowSongsByAlbum.xaml
    /// </summary>
    public partial class ShowSongsByAlbum : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        string artistVal, albumVal;

        public ShowSongsByAlbum()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        public ShowSongsByAlbum(string artist, string album)
        {
            InitializeComponent();
            con.ConnectionString = connectionString;

            this.artistVal = artist;
            artistName.Text = artist;

            this.albumVal = album;
            albumName.Text = album;
        }

        public void loadSongs()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT * FROM DBMOONYFM.artist_album_song_view WHERE upper(artist_name) LIKE '%' || :search || '%' AND upper(album_name) LIKE '%' || :search2 || '%' ORDER BY song_name ASC";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new OracleParameter("search", artistVal.ToUpper()));
            cmd.Parameters.Add(new OracleParameter("search2", albumVal.ToUpper()));
            OracleDataReader reader = cmd.ExecuteReader();
            songList.Children.Clear();
            while (reader.Read())
            {
                SongUC song = new SongUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt16(4));
                songList.Children.Add(song);
            }
            con.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadSongs();
        }
    }
}

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

namespace CW_DB_MoonyFM.ExtraWindows
{
    /// <summary>
    /// Логика взаимодействия для UpdateSong.xaml
    /// </summary>
    public partial class UpdateSong : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        Int32 songId;
        string artistNameVal, albumNameVal, oldSongName;

        public UpdateSong()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        public UpdateSong(Int32 id, string artist, string album, string old)
        {
            InitializeComponent();
            con.ConnectionString = connectionString;

            this.songId = id;
            this.artistNameVal = artist;
            this.albumNameVal = album;
            this.oldSongName = old;
            thisSongName.Text = old;
            artistName.Text = artist;
            albumName.Text = album;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(albumName.Text.Trim()))
            {
                MessageBox.Show("Заполните поле");
                return;
            }
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.UPDATE_SONG_NAME";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_SONG_ID", OracleDbType.Int16, 10).Value = songId;
            cmd.Parameters.Add("P_NEW_NAME", OracleDbType.Varchar2, 30).Value = newSongName.Text.Trim();
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Название песни успешно обновлено");
                this.Close();
                Application.Current.Windows[0].Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Это название песни уже существует");
            }
            con.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }
    }
}

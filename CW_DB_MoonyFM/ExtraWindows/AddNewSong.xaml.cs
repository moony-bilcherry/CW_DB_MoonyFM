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
    /// Логика взаимодействия для AddNewSong.xaml
    /// </summary>
    public partial class AddNewSong : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";

        Int32 songId;
        string artist, album, song;
        byte[] audio;
        string audioName;

        public void ArtistComboBox()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT artist_name FROM DBMOONYFM.artist_table ORDER BY artist_name";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                artistNameCombo.Items.Add(reader.GetString(0));
            }
            con.Close();
        }

        public void AlbumComboBox()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT album_name FROM DBMOONYFM.artist_album_view WHERE upper(artist_name) = :search ORDER BY album_name ASC";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new OracleParameter("search", artist.ToUpper()));
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                albumNameCombo.Items.Add(reader.GetString(0));
            }
            con.Close();
        }

        public AddNewSong()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            ArtistComboBox();
        }

        private void attachAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Audio Files|*.mp3;*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    if (!openFileDialog.FileName.ToLower().EndsWith(".mp3"))
                    {
                        throw new Exception("Файл должен быть .mp3");
                    }
                        audioName = openFileDialog.FileName;
                    audio = File.ReadAllBytes(openFileDialog.FileName);
                }
                openFileDialog = null;
                audioPath.Text = audioName;
            }
            catch (System.ArgumentException ae)
            {
                audioName = "";
                MessageBox.Show(ae.Message.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void artistNameCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            artistNameCombo.IsEnabled = false;
            albumNameCombo.IsEnabled = true;
        }

        private void albumNameCombo_DropDownOpened(object sender, EventArgs e)
        {
            artist = artistNameCombo.Text.Trim();
            albumNameCombo.Items.Clear();
            AlbumComboBox();
        }


        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            album = albumNameCombo.Text.Trim();
            song = songName.Text.Trim();
            if (String.IsNullOrWhiteSpace(artist) || String.IsNullOrWhiteSpace(album) || String.IsNullOrWhiteSpace(song) || String.IsNullOrWhiteSpace(audioName))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.CREATE_SONG";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ARTIST_NAME", OracleDbType.Varchar2, 30).Value = artist;
            cmd.Parameters.Add("P_ALBUM_NAME", OracleDbType.Varchar2, 30).Value = album;
            cmd.Parameters.Add("P_SONG_NAME", OracleDbType.Varchar2, 30).Value = song;
            try
            {
                cmd.ExecuteNonQuery();
                con.Close();

                // чтобы получить ид:

                con.Open();
                OracleCommand cmd3 = con.CreateCommand();
                cmd3.CommandText = "SELECT * FROM DBMOONYFM.artist_album_song_view  WHERE upper(song_name) = upper('"+ song + "') and upper(album_name) = upper('" + album + "') ORDER BY song_id ASC";
                cmd3.CommandType = CommandType.Text;
                OracleDataReader reader = cmd3.ExecuteReader();
                while (reader.Read())
                {
                    songId = reader.GetInt32(0);
                }
                con.Close();


                con.Open();
                // запись изображения в таблицу
                FileStream fls;
                fls = new FileStream(audioName, FileMode.Open, FileAccess.Read);
                byte[] blob = new byte[fls.Length];

                fls.Read(blob, 0, System.Convert.ToInt32(fls.Length));
                fls.Close();

                if (audioName != "")
                {
                    OracleCommand cmd2 = con.CreateCommand();
                    OracleTransaction txn;
                    txn = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd2.Transaction = txn;

                    cmd2.CommandText = "UPDATE song_table " +
                                      "SET " +
                                      "song_blob = :ImageFront " +
                                      "WHERE song_id = :id";

                    cmd2.Parameters.Add(":ImageFront", OracleDbType.Blob);
                    cmd2.Parameters[":ImageFront"].Value = blob;

                    cmd2.Parameters.Add(":id", OracleDbType.Int16);
                    cmd2.Parameters[":id"].Value = songId;

                    cmd2.ExecuteNonQuery();
                    txn.Commit();
                    con.Close();
                    con.Dispose();

                    MessageBox.Show("Данные добавлены в поле blob из " + audioName);

                    this.Close();
                    Application.Current.Windows[0].Show();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибка при добавлении песни");
            }
            con.Close();

        }

        private void albumNameCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            albumNameCombo.IsEnabled = false;
            songName.IsReadOnly = false;
            attachAudio.IsEnabled = true;
        }
    }
}

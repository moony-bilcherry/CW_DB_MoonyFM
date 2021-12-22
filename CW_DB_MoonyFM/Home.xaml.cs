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
using CW_DB_MoonyFM.ExtraWindows;
using CW_DB_MoonyFM.UserControls;

namespace CW_DB_MoonyFM
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        OracleConnection con = new OracleConnection();
        


        public Home()
        {
            InitializeComponent();
            con.ConnectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        }

        public void settingsDataFill()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.SEARCH_USER";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_USER_LOGIN", OracleDbType.Varchar2, 30).Value = DataWorker.CurrentUserLogin;
            cmd.Parameters.Add("O_USER_LOGIN", OracleDbType.Varchar2, 30);
            cmd.Parameters["O_USER_LOGIN"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_USER_PASSWORD", OracleDbType.Varchar2, 30);
            cmd.Parameters["O_USER_PASSWORD"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();
                settingUserLogin.Text = Convert.ToString(cmd.Parameters["O_USER_LOGIN"].Value);
                settingUserPassword.Text = Convert.ToString(cmd.Parameters["O_USER_PASSWORD"].Value);
            }
            catch (Exception exc)
            {
                MessageBox.Show("User is not found");
            }
            con.Close();
        }

        public void loadArtists()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM DBMOONYFM.artist_table ORDER BY artist_name ASC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            artistList.Children.Clear();
            while (reader.Read())
            {
                ArtistUC artist = new ArtistUC(reader.GetInt16(0), reader.GetString(1));
                artistList.Children.Add(artist);
            }
            con.Close();
        }

        public void loadAlbums()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM DBMOONYFM.artist_album_view ORDER BY album_name ASC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            albumList.Children.Clear();
            while (reader.Read())
            {
                //AlbumUC artist = new AlbumUC(reader.GetString(0), reader.GetString(1), reader.GetInt16(2));
                AlbumUC artist = new AlbumUC(reader.GetInt16(0), reader.GetString(1), reader.GetString(2), reader.GetInt16(3));
                albumList.Children.Add(artist);
            }
            con.Close();
        }

        public void loadSongs()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM DBMOONYFM.artist_album_song_view ORDER BY song_name ASC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            songList.Children.Clear();
            while (reader.Read())
            {
                SongUC song = new SongUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt16(4));
                songList.Children.Add(song);
            }
            con.Close();
        }

        public void loadPlaylist()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM DBMOONYFM.artist_album_song_user_view where user_id =" + DataWorker.CurrentUserId.ToString() + " ORDER BY song_name ASC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            playlistList.Children.Clear();
            while (reader.Read())
            {
                SavedUC song = new SavedUC(reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt16(5));
                playlistList.Children.Add(song);
            }
            con.Close();
        }

        private void BackSettingClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }

        private void ChangeLoginClick(object sender, RoutedEventArgs e)
        {
            settingUserLogin.IsReadOnly = false;
            settingChangeLoginButton.IsEnabled = false;
            settingUpdateLoginButton.IsEnabled = true;
            settingCancelLoginButton.IsEnabled = true;
        }

        private void UpdateLoginClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to change your login?", "Update login", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.UPDATE_USER_LOGIN";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_USER_LOGIN", OracleDbType.Varchar2, 30).Value = DataWorker.CurrentUserLogin;
                cmd.Parameters.Add("P_NEW_USER_LOGIN", OracleDbType.Varchar2, 30).Value = settingUserLogin.Text.Trim();
                try
                {
                    cmd.ExecuteNonQuery();
                    DataWorker.CurrentUserLogin = settingUserLogin.Text.Trim();
                    settingUserLogin.IsReadOnly = true;
                    settingChangeLoginButton.IsEnabled = true;
                    settingUpdateLoginButton.IsEnabled = false;
                    settingCancelLoginButton.IsEnabled = false;
                }
                catch (Exception exc)
                {
                    MessageBox.Show("This login is already taken?");
                }
                con.Close();
            }
            else
            {
                settingsDataFill();
            }
        }

        private void CancelLoginClick(object sender, RoutedEventArgs e)
        {
            settingUserLogin.IsReadOnly = true;
            settingChangeLoginButton.IsEnabled = true;
            settingUpdateLoginButton.IsEnabled = false;
            settingCancelLoginButton.IsEnabled = false;
            settingsDataFill();
        }

        private void ChangePasswordClick(object sender, RoutedEventArgs e)
        {
            settingUserPassword.IsReadOnly = false;
            settingChangePasswordButton.IsEnabled = false;
            settingUpdatePasswordButton.IsEnabled = true;
            settingCancelPasswordButton.IsEnabled = true;
        }

        private void UpdatePasswordClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to change your password?", "Update password", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.UPDATE_USER_PASSWORD";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_USER_LOGIN", OracleDbType.Varchar2, 30).Value = DataWorker.CurrentUserLogin;
                cmd.Parameters.Add("P_NEW_USER_PASSWORD", OracleDbType.Varchar2, 30).Value = settingUserPassword.Text.Trim();
                try
                {
                    cmd.ExecuteNonQuery();
                    settingUserPassword.IsReadOnly = true;
                    settingChangePasswordButton.IsEnabled = true;
                    settingUpdatePasswordButton.IsEnabled = false;
                    settingCancelPasswordButton.IsEnabled = false;
                }
                catch (Exception exc)
                {
                    MessageBox.Show("User is not found");
                }
                con.Close();
            }
            else
            {
                settingsDataFill();
            }
        }

        private void CancelPasswordClick(object sender, RoutedEventArgs e)
        {
            settingUserPassword.IsReadOnly = true;
            settingChangePasswordButton.IsEnabled = true;
            settingUpdatePasswordButton.IsEnabled = false;
            settingCancelPasswordButton.IsEnabled = false;
            settingsDataFill();
        }

        private void DeleteUserClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Вы уверены, что хотите удалить этот аккаунт?", "Удаление пользователя", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.DELETE_USER";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_LOGIN", OracleDbType.Varchar2, 30).Value = DataWorker.CurrentUserLogin;
                try
                {
                    cmd.ExecuteNonQuery();
                    DataWorker.CurrentUserLogin = settingUserLogin.Text.Trim();
                    this.NavigationService.Navigate(new Uri("Login.xaml", UriKind.Relative));
                }
                catch (Exception exc)
                {
                    MessageBox.Show("This login is already taken?");
                }
                con.Close();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadAlbums();
            loadArtists();
            loadSongs();
            loadPlaylist();
            settingsDataFill();
            if (DataWorker.CurrentUserRole == "OWNER")
            {
                deleteUserButton.IsEnabled = false;
                tableControlButton.Visibility = Visibility.Visible;
                addNewArtistButton.Visibility = Visibility.Visible;
                addNewAlbumButton.Visibility = Visibility.Visible;
                addNewSongButton.Visibility = Visibility.Visible;
            }
        }

        private void addNewArtistButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewArtist addArtistWin = new AddNewArtist();
            addArtistWin.Show();
            Application.Current.Windows[0].Hide();
        }

        private void addNewAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewAlbum addAlbumWin = new AddNewAlbum();
            addAlbumWin.Show();
            Application.Current.Windows[0].Hide();
        }

        private void searchButtonArtist_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT * FROM DBMOONYFM.artist_table WHERE upper(artist_name) LIKE '%' || :search || '%' ORDER BY artist_name ASC";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new OracleParameter("search", searchBarArtist.Text.Trim().ToUpper()));
            OracleDataReader reader = cmd.ExecuteReader();
            artistList.Children.Clear();
            searchBarArtist.Clear();
            while (reader.Read())
            {
                ArtistUC artist = new ArtistUC(reader.GetInt16(0), reader.GetString(1));
                artistList.Children.Add(artist);
            }
            con.Close();
        }

        private void searchButtonAlbums_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT * FROM DBMOONYFM.artist_album_view WHERE upper(album_name) LIKE '%' || :search || '%' ORDER BY album_name ASC";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new OracleParameter("search", searchBarAlbums.Text.Trim().ToUpper()));
            OracleDataReader reader = cmd.ExecuteReader();
            albumList.Children.Clear();
            searchBarAlbums.Clear();
            while (reader.Read())
            {
                //AlbumUC artist = new AlbumUC(reader.GetString(0), reader.GetString(1), reader.GetInt16(2));
                AlbumUC artist = new AlbumUC(reader.GetInt16(0), reader.GetString(1), reader.GetString(2), reader.GetInt16(3));
                albumList.Children.Add(artist);
            }
            con.Close();
        }

        private void addNewSongButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewSong addSongWin = new AddNewSong();
            addSongWin.Show();
            Application.Current.Windows[0].Hide();
        }

        private void searchButtonSongs_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT * FROM DBMOONYFM.artist_album_song_view WHERE upper(song_name) LIKE '%' || :search || '%' ORDER BY song_name ASC";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new OracleParameter("search", searchBarSongs.Text.Trim().ToUpper()));
            OracleDataReader reader = cmd.ExecuteReader();
            songList.Children.Clear();
            searchBarSongs.Clear();
            while (reader.Read())
            {
                SongUC song = new SongUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt16(4));
                songList.Children.Add(song);
            }
            con.Close();
        }

        private void tableControlButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow win = new AdminWindow();
            win.Show();
            Application.Current.Windows[0].Hide();
        }

        private void searchButtonPlaylist_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = @"SELECT * FROM DBMOONYFM.artist_album_song_user_view WHERE upper(song_name) LIKE '%' || :search || '%' AND user_id = " + DataWorker.CurrentUserId.ToString() + " ORDER BY song_name ASC";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new OracleParameter("search", searchBarPlaylist.Text.Trim().ToUpper()));
            OracleDataReader reader = cmd.ExecuteReader();
            playlistList.Children.Clear();
            searchBarPlaylist.Clear();
            while (reader.Read())
            {
                SavedUC song = new SavedUC(reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt16(5));
                playlistList.Children.Add(song);
            }
            con.Close();
        }
    }
}

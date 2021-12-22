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
    /// Логика взаимодействия для AlbumUC.xaml
    /// </summary>
    public partial class AlbumUC : UserControl
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        Int16 year, albumId;
        string album, artist;
        byte[] cover;

        private void checkRole()
        {
            if (DataWorker.CurrentUserRole == "OWNER")
            {
                adminButtons.Visibility = Visibility.Visible;
                return;
            }
            else return;
        }

        private void editAlbum_Click(object sender, RoutedEventArgs e)
        {
            UpdateAlbum updateWin = new UpdateAlbum(this.albumId, this.artist, this.album);
            updateWin.Show();
            Application.Current.Windows[0].Hide();
        }

        private void showSongs_Click(object sender, RoutedEventArgs e)
        {
            ShowSongsByAlbum show = new ShowSongsByAlbum(artist, album);
            show.Show();
            Application.Current.Windows[0].Hide();
        }

        public AlbumUC()
        {
            InitializeComponent();
            checkRole();
            con.ConnectionString = connectionString;
        }

        public AlbumUC(Int16 id, string artistName, string albumName, Int16 yearReleased)
        {
            con.ConnectionString = connectionString;
            InitializeComponent();
            checkRole();
            this.albumId = id;
            this.album = albumName;
            this.artist = artistName;
            this.year = yearReleased;

            blockAlbumName.Text = albumName;
            blockArtistName.Text = artistName;
            blockYear.Text = yearReleased.ToString();

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT album_blob FROM DBMOONYFM.artist_album_view WHERE album_id = " + id.ToString();
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(reader.GetValue(0) as byte[]);
                    image.EndInit();
                    albumCover.Source = image;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }

        }
    }
}

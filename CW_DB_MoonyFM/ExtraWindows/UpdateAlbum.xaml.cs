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
using System.Text.RegularExpressions;

namespace CW_DB_MoonyFM.ExtraWindows
{
    /// <summary>
    /// Логика взаимодействия для UpdateAlbum.xaml
    /// </summary>
    public partial class UpdateAlbum : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        Int16 albumId;
        string artistNameYeah, oldAlbumName, newAlbumName;

        public UpdateAlbum()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        public UpdateAlbum(Int16 id, string artist, string old)
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
            this.albumId = id;
            this.artistNameYeah = artist;
            this.oldAlbumName = old;

            artistName.Text = artist;
            thisAlbumName.Text = old;
        }

        private void albumPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(albumName.Text.Trim()) || String.IsNullOrWhiteSpace(albumName.Text.Trim()))
            {
                MessageBox.Show("Заполните хоть одно из полей");
                return;
            }
            if (!String.IsNullOrWhiteSpace(albumName.Text.Trim()))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.UPDATE_ALBUM_NAME";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_ALBUM_ID", OracleDbType.Int16, 10).Value = albumId;
                cmd.Parameters.Add("P_NEW_NAME", OracleDbType.Varchar2, 30).Value = albumName.Text.Trim();
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Название альбома успешно обновлено");
                    this.Close();
                    Application.Current.Windows[0].Show();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Это название альбома уже существует");
                }
                con.Close();
            }
            if (!String.IsNullOrWhiteSpace(albumYear.Text.Trim()))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.UPDATE_ALBUM_YEAR";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_ALBUM_ID", OracleDbType.Int16, 10).Value = albumId;
                cmd.Parameters.Add("P_NEW_YEAR", OracleDbType.Int16, 10).Value = Convert.ToInt16(albumYear.Text.Trim());
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Год выпуска альбома успешно обновлен");
                    this.Close();
                    Application.Current.Windows[0].Show();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Это название альбома уже существует");
                }
                con.Close();
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }
    }
}

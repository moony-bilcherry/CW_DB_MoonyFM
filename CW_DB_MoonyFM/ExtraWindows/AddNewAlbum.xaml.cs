using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Media.Imaging;

namespace CW_DB_MoonyFM.ExtraWindows
{
    /// <summary>
    /// Логика взаимодействия для AddNewAlbum.xaml
    /// </summary>
    public partial class AddNewAlbum : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        byte[] image;
        string imageName;

        public AddNewAlbum()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

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

        private void albumPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(albumName.Text.Trim()) || String.IsNullOrWhiteSpace(artistNameCombo.Text.Trim()) || String.IsNullOrWhiteSpace(albumYear.Text.Trim()) || String.IsNullOrWhiteSpace(imageName))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.CREATE_ALBUM";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ARTIST_NAME", OracleDbType.Varchar2, 30).Value = artistNameCombo.Text.Trim();
            cmd.Parameters.Add("P_ALBUM_NAME", OracleDbType.Varchar2, 30).Value = albumName.Text.Trim();
            cmd.Parameters.Add("P_ALBUM_RELEASED", OracleDbType.Int16, 10).Value = Convert.ToInt16(albumYear.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                con.Close();

                con.Open();
                // запись изображения в таблицу
                FileStream fls;
                fls = new FileStream(imageName, FileMode.Open, FileAccess.Read);
                byte[] blob = new byte[fls.Length];
                fls.Read(blob, 0, System.Convert.ToInt32(fls.Length));
                fls.Close();

                if(imageName != "")
                {
                    OracleCommand cmd2 = con.CreateCommand();
                    OracleTransaction txn;
                    txn = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd2.Transaction = txn;

                    cmd2.CommandText = "UPDATE album_table " +
                                      "SET " +
                                      "album_blob = :ImageFront " +
                                      "WHERE UPPER(album_name) = UPPER('"+ albumName.Text.Trim() + "')";

                    cmd2.Parameters.Add(":ImageFront", OracleDbType.Blob);
                    cmd2.Parameters[":ImageFront"].Value = blob;

                    cmd2.ExecuteNonQuery();
                    txn.Commit();
                    con.Close();
                    con.Dispose();

                    MessageBox.Show("Данные добавлены в поле blob из " + imageName);

                    this.Close();
                    Application.Current.Windows[0].Show();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибка при добавлении альбома");
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
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            ArtistComboBox();
        }

        private void attachPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Image Files|*.jpg;*.png;"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    imageName = openFileDialog.FileName;
                    image = File.ReadAllBytes(openFileDialog.FileName);
                    var bitmImg = new BitmapImage();
                    using (var mem = new MemoryStream(image))
                    {
                        mem.Position = 0;
                        bitmImg.BeginInit();
                        bitmImg.CacheOption = BitmapCacheOption.OnLoad;
                        bitmImg.StreamSource = mem;
                        bitmImg.EndInit();
                    }
                    bitmImg.Freeze();
                    coverImage.Source = bitmImg;
                }
                openFileDialog = null;
            }
            catch (System.ArgumentException ae)
            {
                imageName = "";
                MessageBox.Show(ae.Message.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}

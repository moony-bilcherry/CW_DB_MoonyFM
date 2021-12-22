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

namespace CW_DB_MoonyFM.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ArtistUC.xaml
    /// </summary>
    public partial class ArtistUC : UserControl
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        Int16 id;
        string name;

        private void checkRole()
        {
            if (DataWorker.CurrentUserRole == "OWNER")
            {
                adminButtons.Visibility = Visibility.Visible;
                return;
            }
            else return;
        }

        public ArtistUC()
        {
            InitializeComponent();
            checkRole();
            con.ConnectionString = connectionString;
        }
        public ArtistUC(Int16 artId, string artName)
        {
            InitializeComponent();
            checkRole();
            con.ConnectionString = connectionString;
            this.id = artId;
            this.name = artName;
            blockArtistId.Text = artId.ToString();
            blockArtistName.Text = artName;
        }

        private void editArtist_Click(object sender, RoutedEventArgs e)
        {
            UpdateArtist updateWin = new UpdateArtist(this.name);
            updateWin.Show();
            Application.Current.Windows[0].Hide();
        }

        private void showSongs_Click(object sender, RoutedEventArgs e)
        {
            ShowSongsByArtist show = new ShowSongsByArtist(name);
            show.Show();
            Application.Current.Windows[0].Hide();
        }

        private void deleteArtist_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Вы уверены, что хотите удалить этого исполнителя?", "Удаление исполнителя", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.DELETE_ARTIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_ID", OracleDbType.Int16, 10).Value = id;
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Исполнитель успешно удален");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Этот логин уже занят");
                }
                con.Close();
            }
        }
    }
}

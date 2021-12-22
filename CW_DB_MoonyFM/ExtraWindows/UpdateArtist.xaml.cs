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
    /// Логика взаимодействия для UpdateArtist.xaml
    /// </summary>
    public partial class UpdateArtist : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        string oldName, newName;

        public UpdateArtist()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        public UpdateArtist(string old) {
            InitializeComponent();
            con.ConnectionString = connectionString;
            this.oldName = old;
            thisArtistName.Text = old;
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(newArtistName.Text.Trim()))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.UPDATE_ARTIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_OLD_ARTIST", OracleDbType.Varchar2, 30).Value = oldName;
                cmd.Parameters.Add("P_NEW_ARTIST", OracleDbType.Varchar2, 30).Value = newArtistName.Text.Trim();
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Исполнитель успешно обновлен");
                    this.Close();
                    Application.Current.Windows[0].Show();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Исполнитель с таким именем уже существует");
                }
                con.Close();
            }
            else
            {
                MessageBox.Show("Заполните поле");
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }
    }
}

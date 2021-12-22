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
    /// Логика взаимодействия для AddNewArtist.xaml
    /// </summary>
    public partial class AddNewArtist : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";

        public AddNewArtist()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrWhiteSpace(artistName.Text.Trim())) {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "DBMOONYFM.CREATE_ARTIST";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_ARTIST_NAME", OracleDbType.Varchar2, 30).Value = artistName.Text.Trim();
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Исполнитель успешно создан");
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

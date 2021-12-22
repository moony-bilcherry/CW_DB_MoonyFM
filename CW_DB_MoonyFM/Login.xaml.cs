using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;
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

namespace CW_DB_MoonyFM
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";

        public Login()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.LOG_IN_USER";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_USER_LOGIN", OracleDbType.Varchar2, 30).Value = userLogin.Text.Trim();
            cmd.Parameters.Add("P_USER_PASSWORD", OracleDbType.Varchar2, 30).Value = userPassword.Password.Trim();
            cmd.Parameters.Add("O_USER_ID", OracleDbType.Int32, 10);
            cmd.Parameters["O_USER_ID"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_USER_LOGIN", OracleDbType.Varchar2, 30);
            cmd.Parameters["O_USER_LOGIN"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_USER_ROLE", OracleDbType.Varchar2, 30);
            cmd.Parameters["O_USER_ROLE"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();
                string user = Convert.ToString(cmd.Parameters["O_USER_LOGIN"].Value);
                string role = Convert.ToString(cmd.Parameters["O_USER_ROLE"].Value);

                int id = Convert.ToInt32((decimal)(OracleDecimal)(cmd.Parameters["O_USER_ID"].Value));

                DataWorker.CurrentUserLogin = user;
                DataWorker.CurrentUserRole = role;
                DataWorker.CurrentUserId = id;
                MessageBox.Show("Пользователь: " + user + "; роль: " + role + "; id: " + id.ToString());

                this.NavigationService.Navigate(new Uri("Home.xaml", UriKind.Relative));
                        
            }
            catch (Exception exc)
            {
                MessageBox.Show("Неверный логин или пароль");
            }
            con.Close();
            
        }

        private void btnToRegistration_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Register.xaml", UriKind.Relative));
        }
    }
}

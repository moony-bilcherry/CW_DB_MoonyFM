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

namespace CW_DB_MoonyFM
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Page
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";

        public Register()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }



        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(userLogin.Text.Trim()) || String.IsNullOrWhiteSpace(userPassword.Password.Trim()) || String.IsNullOrWhiteSpace(userRepeatPassword.Password.Trim()))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            if (userPassword.Password.Trim() != userRepeatPassword.Password.Trim())
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.REGISTER_USER";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_USER_LOGIN", OracleDbType.Varchar2, 30).Value = userLogin.Text.Trim();
            cmd.Parameters.Add("P_USER_PASSWORD", OracleDbType.Varchar2, 30).Value = userPassword.Password.Trim();
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Пользователь успешно создан");
                this.NavigationService.Navigate(new Uri("Login.xaml", UriKind.Relative));
                //Login login = new Login();
                //this.Close();
                //login.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("This username is already taken!");
            }
            con.Close();
        }
    }
}

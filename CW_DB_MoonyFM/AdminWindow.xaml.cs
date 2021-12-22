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
using CW_DB_MoonyFM.UserControls;

namespace CW_DB_MoonyFM
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        OracleConnection con = new OracleConnection();
        String connectionString = "DATA SOURCE=192.168.2.10:1521/orcl;PERSIST SECURITY INFO=True;USER ID=DBMOONYFM;PASSWORD=Pa$$w0rd";
        int rowMargin = 20, rowCounter = 0, rowCounterSearch = 0;
        string searchLineBuffer;

        public AdminWindow()
        {
            InitializeComponent();
            con.ConnectionString = connectionString;
        }

        public void load20Users()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            //cmd.CommandText = "SELECT * FROM DBMOONYFM.user_role_full_view ORDER BY user_login ASC";

            cmd.CommandText = "SELECT * FROM (select user_id, user_login, decr, role_name, row_number() over (order by user_login) rn from DBMOONYFM.user_role_full_view) where rn between :n and :m ORDER BY user_login ASC";
            cmd.Parameters.Add(new OracleParameter("n", rowCounter));
            cmd.Parameters.Add(new OracleParameter("m", rowCounter + rowMargin));

            rowCounter += rowMargin;
            rowCounter++;

            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            userList.Children.Clear();
            while (reader.Read())
            {
                UserUC us = new UserUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                userList.Children.Add(us);
            }
            con.Close();
        }

        public void search20Users()
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();

            if(searchBarUser.Text.Trim().ToUpper() != searchLineBuffer)
            {
                rowCounterSearch = 0;
            }

            searchLineBuffer = searchBarUser.Text.Trim().ToUpper();

            cmd.CommandText = "SELECT * FROM (select user_id, user_login, decr, role_name, row_number() over (order by user_login) rn from DBMOONYFM.user_role_full_view) where upper(user_login) LIKE '%' || :search || '%' AND rn between :n and :m  ORDER BY user_login ASC";
            cmd.Parameters.Add(new OracleParameter("search", searchLineBuffer));
            cmd.Parameters.Add(new OracleParameter("n", rowCounterSearch));
            cmd.Parameters.Add(new OracleParameter("m", rowCounterSearch + rowMargin));

            rowCounterSearch += rowMargin;
            rowCounterSearch++;

            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            userList.Children.Clear();
            while (reader.Read())
            {
                UserUC us = new UserUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                userList.Children.Add(us);
            }
            con.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Windows[0].Show();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void searchButtonUser_Click(object sender, RoutedEventArgs e)
        {
            search20Users();
            //con.Open();
            //OracleCommand cmd = con.CreateCommand();
            //cmd.CommandText = @"SELECT * FROM DBMOONYFM.user_role_full_view WHERE upper(user_login) LIKE '%' || :search || '%' ORDER BY user_login ASC";
            //cmd.CommandType = CommandType.Text;
            //cmd.Parameters.Add(new OracleParameter("search", searchBarUser.Text.Trim().ToUpper()));
            //OracleDataReader reader = cmd.ExecuteReader();
            //userList.Children.Clear();
            //searchBarUser.Clear();
            //while (reader.Read())
            //{
            //    UserUC us = new UserUC(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            //    userList.Children.Add(us);
            //}
            //con.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            load20Users();
        }

        private void xmlExportButton_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.USERS_EXPORT";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
                xmlExportButton.IsEnabled = false;
                xmlImportButton.IsEnabled = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Export error");
            }
            con.Close();
        }

        private void xmlImportButton_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "DBMOONYFM.ARTIST_IMPORT";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
                xmlExportButton.IsEnabled = false;
                xmlImportButton.IsEnabled = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Import error");
            }
            con.Close();
        }

        private void showNext20_Click(object sender, RoutedEventArgs e)
        {
            load20Users();
        }
    }
}

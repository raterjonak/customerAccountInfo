using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomerAccountInfoApp
{
    public partial class CustomerAccountInfoUI : Form
    {
        private String connectionString =
            ConfigurationManager.ConnectionStrings["CustomerAccountConString"].ConnectionString;

        Account accountObj=new Account();

        private double balance;
        public CustomerAccountInfoUI()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            accountObj.accountNo =Convert.ToInt32(accountNoTextBox.Text);
            accountObj.customerName = cutomerNameTextBox.Text;
            accountObj.email = emailTextBox.Text;
            accountObj.openingDate = openingDateTextBox.Text;

            accountObj.balance = 0;
            int accountNolength = accountNoTextBox.Text.Length;

            if (accountNolength <8)

            {

                MessageBox.Show("your Account number should be atleast eight digit.");
            }

            else
            {

                if (IsAccountExist(accountObj.accountNo))
                {
                    MessageBox.Show("your account number already exist.");
                }

                else
                {



                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = "Insert Into account_tbl Values('" + accountObj.accountNo + "','" +
                                   accountObj.customerName +
                                   "','" + accountObj.email + "','" + accountObj.openingDate + "','" +
                                   accountObj.balance +
                                   "')";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Your account number "+accountObj.accountNo+" has been created successful.");

                        ShowAllAccountInfo();
                        ClearAllTextBox();
                    }
                    else
                    {
                        MessageBox.Show("Insertion Fail!");
                    }
                }
            }



        }

        bool IsAccountExist(int accountno)
        {
            bool isAccountExist = false;
            SqlConnection connection=new SqlConnection(connectionString);
            string query = "Select * From account_tbl Where accountNumber='" + accountno + "'";
            SqlCommand command=new SqlCommand(query,connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                isAccountExist = true;
                break;
                
            }
            return isAccountExist;
        }

      

        public void ShowAllAccountInfo()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM account_tbl";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Account> accountList = new List<Account>();
            while (reader.Read())
            {
                Account account = new Account();
                account.accountNo = int.Parse(reader["accountNumber"].ToString());
                account.customerName = reader["customerName"].ToString();
               account.openingDate = reader["openingDate"].ToString();
                account.balance =double.Parse( reader["balance"].ToString());

                accountList.Add(account);
            }

            reader.Close();
            connection.Close();

            LoadAccountListView(accountList);
        }

        public void LoadAccountListView(List<Account> accounts)
        {
            customerAccountInfoListView.Items.Clear();
            foreach (var account in accounts)
            {
                ListViewItem item = new ListViewItem(account.accountNo.ToString());
                item.SubItems.Add(account.customerName);
               // item.SubItems.Add(account.email);
                item.SubItems.Add(account.openingDate);
                item.SubItems.Add(account.balance.ToString());


                customerAccountInfoListView.Items.Add(item);
            }
        }

        private void CustomerAccountInfoUI_Load(object sender, EventArgs e)
        {
            ShowAllAccountInfo();
        }

        public void ClearAllTextBox()
        {
            accountNoTextBox.Text = "";
            cutomerNameTextBox.Text="";
            emailTextBox.Text = "";
            openingDateTextBox.Text = "";
            
        }

        private void depositeButton_Click(object sender, EventArgs e)
        {
          
        }

        private void depositeButton_Click_1(object sender, EventArgs e)
        {


            accountObj.accountNo = int.Parse(tranAccountNoTextBox.Text);

            if (IsAccountExist(accountObj.accountNo))
            {

                double deposite = double.Parse(amountTextBox.Text);

                if (deposite < 1)
                {
                    MessageBox.Show("Deposit amount should be positive value.");
                }

                else
                {



                    Account acObj = getAccount(accountObj.accountNo);

                    double newBalance = deposite + acObj.balance;

                    SqlConnection connection = new SqlConnection(connectionString);
                    string query = "Update  account_tbl Set balance=" + newBalance + " Where accountNumber='" +
                                   accountObj.accountNo + "'";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();

                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Deposit succesful");
                        ShowAllAccountInfo();
                    }

                    else
                    {
                        MessageBox.Show("Deposite operation Fail");
                    }
                }
            }

            else
            {
                MessageBox.Show("Sorry! your account number does not exist.");
            }
        }

        Account getAccount(int acNo)
        {


            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM account_tbl where accountNumber='" + acNo + "'";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            Account account = new Account();

            while (reader.Read())
            {

                account.accountNo = int.Parse(reader["accountNumber"].ToString());
                account.customerName = reader["customerName"].ToString();
                account.openingDate = reader["openingDate"].ToString();
                account.balance = double.Parse(reader["balance"].ToString());

            }


            reader.Close();
            connection.Close();

            return account;

        }

        private void withdrawButton_Click(object sender, EventArgs e)
        {
            accountObj.accountNo = int.Parse(tranAccountNoTextBox.Text);

            if (IsAccountExist(accountObj.accountNo))
            {
                
            
            double withdraw = double.Parse(amountTextBox.Text);

            Account acObj = getAccount(accountObj.accountNo);

            if (withdraw > acObj.balance)
            {
                MessageBox.Show("You have not enough balance to withdraw.");
            }

            else
            {
                if (withdraw < 0)
                {
                    MessageBox.Show("Withdrawal amount should be positive value.");
                }

                else
                {




                    double newBalance = acObj.balance - withdraw;

                    SqlConnection connection = new SqlConnection(connectionString);
                    string query = "Update  account_tbl Set balance=" + newBalance + " Where accountNumber='" +
                                   accountObj.accountNo + "'";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();

                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowAffected > 0)
                    {
                        MessageBox.Show("withdraw succesful");
                        ShowAllAccountInfo();
                    }

                    else
                    {
                        MessageBox.Show("withdraw operation Fail");
                    }
                }

           
            }
            }

            else
            {
                MessageBox.Show("Your account Number does not exist!!");
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            accountObj.accountNo = int.Parse(accountSearchTextBox.Text);

            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM account_tbl Where accountNumber Like'"+accountObj.accountNo+"%'";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Account> accountList = new List<Account>();
            while (reader.Read())
            {
                Account account = new Account();
                account.accountNo = int.Parse(reader["accountNumber"].ToString());
                account.customerName = reader["customerName"].ToString();
                account.openingDate = reader["openingDate"].ToString();
                account.balance = double.Parse(reader["balance"].ToString());

                accountList.Add(account);
            }

            reader.Close();
            connection.Close();

            customerAccountInfoListView.Items.Clear();

            LoadAccountListView(accountList);
        }
    }
}

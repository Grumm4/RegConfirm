using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RegConfirm
{
    internal class LogicMethods
    {
        private int code;
        private List<string> domensEmail = new List<string>(3)
        {
          "@mail.ru",
          "@yandex.ru",
          "@gmail.com"
        };
        private static string connStr = "";
        private MySqlConnection conn = new MySqlConnection(LogicMethods.connStr);

        private static string sha256(string randomString)
        {
            SHA256Managed shA256Managed = new SHA256Managed();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in shA256Managed.ComputeHash(Encoding.UTF8.GetBytes(randomString)))
                stringBuilder.Append(num.ToString("x2"));
            return stringBuilder.ToString();
        }

        internal void Login(TextBox t1, TextBox t2, TextBox t3, Button b1)
        {
            if (b1.Text == "Войти")
            {
                b1.Text = "Войти";
                t3.Visible = false;
                string cmdText = "SELECT * FROM t_user WHERE emailUser = @ue AND passUser = @up";
                conn.Open();
                DataTable dataTable = new DataTable();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                MySqlCommand mySqlCommand = new MySqlCommand(cmdText, conn);
                mySqlCommand.Parameters.Add("@ue", MySqlDbType.VarChar, 25);
                mySqlCommand.Parameters.Add("@up", MySqlDbType.VarChar, 25);
                mySqlCommand.Parameters["@ue"].Value = t1.Text;
                mySqlCommand.Parameters["@up"].Value = sha256(t2.Text);
                mySqlDataAdapter.SelectCommand = mySqlCommand;
                mySqlDataAdapter.Fill(dataTable);
                conn.Close();
                if (dataTable.Rows.Count > 0)
                {
                    conn.Open();
                    if (Convert.ToString(new MySqlCommand($"SELECT isActivated FROM t_user WHERE emailUser = '{t1.Text}'", conn).ExecuteScalar()) == "0")
                    {
                        MessageBox.Show("Вы не активировали свой аккаунт! Мы снова выслали вам код активации, введите его в поле ниже \\/");
                        SendEmailAsync(t1).GetAwaiter();
                        t3.Visible = true;
                        b1.Text = "Подтвердить";
                    }
                    else
                    {
                        MessageBox.Show("Вы успешно вошли и подтвердили свой аккаунт!");
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Неверные данные авторизации!");
                }
            }
            else
            {
                if (b1.Text == "Подтвердить")
                {
                    conn.Open();
                    MySqlCommand mySqlCommand = new MySqlCommand($"SELECT codeUser FROM t_user WHERE emailUser = '{t1.Text}'", conn);
                    if (t3.Text == Convert.ToString(mySqlCommand.ExecuteScalar()))
                    {
                        new MySqlCommand($"UPDATE t_user SET isActivated = '1', codeUser = '0' WHERE emailUser = '{t1.Text}'", conn).ExecuteNonQuery();
                        b1.Text = "Войти";
                        t3.Visible = false;
                        MessageBox.Show("Успешная активация!");
                    }
                    else
                    {
                        MessageBox.Show("Код активации неверный!");
                    }
                    conn.Close();
                }
            }
        }

        internal async Task SendEmailAsync(TextBox t1)
        {
            string sql = $"SELECT codeUser FROM t_user WHERE emailUser = '{t1.Text}'";
            MySqlCommand cmd = new MySqlCommand(sql, this.conn);
            this.code = Convert.ToInt32(cmd.ExecuteScalar());
            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
            smtp.Credentials = new NetworkCredential("", "");
            smtp.EnableSsl = true;
            MailAddress from = new MailAddress("", "Activation");
            MailAddress to = new MailAddress(t1.Text);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Активация";
            m.Body = string.Format($"Ваш код активации: {code}");
            try
            {
                await smtp.SendMailAsync(m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            code = 0;
        }

        internal void RegistrationM(TextBox t1, TextBox t2, TextBox t3, TextBox t4, Button b1)
        {
            if (b1.Text != "Подтвердить email")
            {
                string cmdText = "SELECT * FROM t_user WHERE loginUser = @un AND passUser = @up AND emailUser = @ue";
                conn.Open();
                DataTable dataTable = new DataTable();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                MySqlCommand mySqlCommand = new MySqlCommand(cmdText, conn);
                mySqlCommand.Parameters.Add("@un", MySqlDbType.VarChar, 25);
                mySqlCommand.Parameters.Add("@up", MySqlDbType.VarChar, 25);
                mySqlCommand.Parameters.Add("@ue", MySqlDbType.VarChar, 25);
                mySqlCommand.Parameters["@ue"].Value = t1.Text;
                mySqlCommand.Parameters["@un"].Value = t2.Text;
                mySqlCommand.Parameters["@up"].Value = sha256(t3.Text);
                conn.Close();
                code = new Random().Next(100000, 999999);
                foreach (string str in domensEmail)
                {
                    if (Convert.ToString(mySqlCommand.Parameters["@ue"].Value).Contains(str))
                    {
                        conn.Open();
                        mySqlDataAdapter.SelectCommand = mySqlCommand;
                        mySqlDataAdapter.Fill(dataTable);
                        conn.Close();
                        if (dataTable.Rows.Count > 0)
                        {
                            MessageBox.Show("Такой email уже зарегистрирован!");
                        }
                        else
                        {
                            conn.Open();
                            new MySqlCommand($"INSERT INTO t_user (loginUser, emailUser, passUser, codeUser) VALUES ('{t2.Text}', '{t1.Text}', '{sha256(t3.Text)}', '{Convert.ToString(code)}')", conn).ExecuteNonQuery();
                            conn.Close();
                            MessageBox.Show("Введите код подтверждения, высланный вам на почту, в поле ниже \\/");
                            t4.Visible = true;
                            b1.Text = "Подтвердить email";
                        }
                    }
                }
            }
            else if (b1.Text == "Подтвердить email")
            {
                conn.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("SELECT codeUser FROM t_user WHERE emailUser = '" + t1.Text + "'", conn);
                if (t4.Text == Convert.ToString(mySqlCommand.ExecuteScalar()))
                {
                    MessageBox.Show("Вы успешно подтвердили аккаунт!");
                    new MySqlCommand("UPDATE t_user SET isActivated = 1, codeUser = 0 WHERE emailUser = '" + t1.Text + "'", conn).ExecuteNonQuery();
                    Registration.reg.Close();
                    LogIn.log.Show();
                }
                else
                {
                    MessageBox.Show("Код активации неверный!");
                }
                conn.Close();
            }
        }
    }
}

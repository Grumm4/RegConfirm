using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegConfirm
{
    public partial class LogIn : Form
    {
        private LogicMethods lm = new LogicMethods();
        public static LogIn log = new LogIn();
        public LogIn()
        {
            InitializeComponent();
        }

        private void LogIn_Load(object sender, EventArgs e)
        {
            this.textBox2.UseSystemPasswordChar = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lm.Login(this.textBox1, this.textBox2, this.textBox3, this.button1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Registration.reg.Show();
        }

        private void textBox1_MouseEnter(object sender, EventArgs e) => toolTip1.Show("Login", textBox1);

        private void textBox2_MouseEnter(object sender, EventArgs e) => toolTip1.Show("Pass", textBox2);

        private void textBox3_MouseEnter(object sender, EventArgs e) => toolTip1.Show("Code", textBox3);

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.textBox2.UseSystemPasswordChar = false;
                checkBox1.Text = "Скрыть пароль";
            }

            else
            {
                this.textBox2.UseSystemPasswordChar = true;
                checkBox1.Text = "Показать пароль";
            }
        }
    }
}

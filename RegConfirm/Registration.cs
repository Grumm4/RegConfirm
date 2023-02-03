using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RegConfirm
{
    public partial class Registration : Form
    {
        public static Registration reg = new Registration();
        private LogicMethods lm = new LogicMethods();

        public Registration()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0 && textBox2.Text.Length != 0 && textBox3.Text.Length != 0)
            {
                lm.RegistrationM(textBox1, textBox2, textBox3, textBox4, button1);
                lm.SendEmailAsync(textBox1).GetAwaiter();
            }
            else
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
            }
        }

        private void textBox1_MouseEnter(object sender, EventArgs e) => toolTip1.Show("Email", textBox1);

        private void textBox2_MouseEnter(object sender, EventArgs e) => toolTip1.Show("Login", textBox2);

        private void textBox3_MouseEnter(object sender, EventArgs e) => toolTip1.Show("Password", textBox3);

        private void textBox4_MouseEnter(object sender, EventArgs e) => toolTip1.Show("Code", textBox4);

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.textBox3.UseSystemPasswordChar = false;
                checkBox1.Text = "Скрыть пароль";
            }

            else
            {
                this.textBox3.UseSystemPasswordChar = true;
                checkBox1.Text = "Показать пароль";
            }
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            textBox3.UseSystemPasswordChar = true;
        }
    }
}

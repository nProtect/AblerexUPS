using System;
using System.Windows.Forms;
using System.Threading;

namespace AblerexUpsApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 2;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 3;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            ReadUPSParameter();
            button2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Write confirm?", "UPSApp", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string szCommand = "Mzzzz" +
                    comboBox1.SelectedIndex.ToString() +
                    comboBox2.SelectedIndex.ToString() +
                    "04" +
                    comboBox3.SelectedIndex.ToString() +
                    "00" +
                    comboBox4.SelectedIndex.ToString() +
                    comboBox5.SelectedIndex.ToString() +
                    comboBox6.SelectedIndex.ToString() + "99999999";

                Form1.connUPS.SendCommand(szCommand);
                //MessageBox.Show(szCommand, "UPSApp");
                MessageBox.Show("Write success! Please restart UPS for take effect.", "UPSApp");
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.timer1instance.Enabled = true;
        }

        private bool ReadUPSParameter()
        {
            Form1.connUPS.SendCommand("Q3");
            Thread.Sleep(1000);

            if (Form1.connUPS.Q3DataAvaiable == true)
            {
                comboBox1.SelectedIndex = Form1.connUPS.DisplaySystem;
                comboBox2.SelectedIndex = Form1.connUPS.VoltageOutput;
                comboBox3.SelectedIndex = Form1.connUPS.UPSMode;
                comboBox4.SelectedIndex = Form1.connUPS.FineTuning;
                comboBox5.SelectedIndex = Form1.connUPS.BypassWindow;
                comboBox6.SelectedIndex = Form1.connUPS.Synchronization;

                MessageBox.Show("Read parameter from ups SUCCESS!", "UPSApp");

                Form1.connUPS.Q3DataAvaiable = false;
                return true;
            }
            else
            {
                MessageBox.Show("UPS Data read failure!", "UPSApp");
                return false;
            }
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            ReadUPSParameter();
        }
    }
}

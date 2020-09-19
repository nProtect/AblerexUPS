using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace AblerexUpsApp
{
    public partial class Form1 : Form
    {
        Form2 form2 = null;
        public static Timer timer1instance = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String[] szLstPort = SerialPort.GetPortNames();
            foreach(String portName in szLstPort)
            {
                comboBox1.Items.Add(portName);
            }

            //comboBox1.SelectedIndex = 0;
            SetConnUPS(new AblerexRS232());
            timer1instance = timer1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool connResult = GetConnUPS().UPSConnect(comboBox1.SelectedItem.ToString());
            if(connResult == true)
            {
                button4.Enabled = false;
                comboBox1.Enabled = false;
                timer1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetConnUPS().SendCommand("T");
        }

        int iCmdState = 0;
        public static AblerexRS232 connUPS;

        public AblerexRS232 GetConnUPS()
        {
            return connUPS;
        }

        public void SetConnUPS(AblerexRS232 value)
        {
            connUPS = value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Command Cycle
            if(iCmdState == 0)
            {
                GetConnUPS().SendCommand("I");
                iCmdState = 1;
            }
            else if(iCmdState == 1)
            {
                GetConnUPS().SendCommand("F");
                iCmdState = 2;
            }
            else if (iCmdState == 2)
            {
                GetConnUPS().SendCommand("Q1");
            }
            else
            {
                iCmdState = 0;
            }

            //show to label
            label1.Text = "Brand: " + GetConnUPS().Brand;
            label2.Text = "Model: " + GetConnUPS().Model;
            label3.Text = "Firmware Version: " + GetConnUPS().FirmwareVersion;

            label4.Text = "Rated Voltage: " + GetConnUPS().RatedVoltage + "V";
            label5.Text = "Rated Current: " + GetConnUPS().RatedCurrent + "A";
            label6.Text = "Battery Voltage: " + GetConnUPS().BatteryVoltage + "V";
            label7.Text = "Rated Frequency: " + GetConnUPS().RatedFrequency + "Hz";

            label8.Text = "Input Voltage: " + GetConnUPS().InputVoltage + "V";
            label9.Text = "Output Voltage: " + GetConnUPS().OutputVoltage + "V";
            label10.Text = "UPS Load: " + GetConnUPS().UPSLoad + "%" + " (" + GetConnUPS().UPSLoadAmp + "A - " + GetConnUPS().UPSLoadWatt + "W)";
            label11.Text = "Battery(%): " + GetConnUPS().BatteryPercent + "%"; //TODO
            label12.Text = "Temperature(C): " + GetConnUPS().Temperature + "C";
            label13.Text = "Input Frequency: " + GetConnUPS().InputFrequency + "Hz";
            label14.Text = "Status Flag: " + GetConnUPS().StatusFlag;
            label15.Text = "Raw Data: " + GetConnUPS().RawData;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetConnUPS().SendCommand("CT");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(GetConnUPS().IsOpen() == false)
            {
                MessageBox.Show("Please connect to ups before use!", "UPSApp");
                return;
            }

            if(form2 == null || form2.IsDisposed == true)
            {
                form2 = new Form2();
                form2.Owner = this;
                form2.Show();

                timer1.Enabled = false; //prevent command confilct
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace AblerexUpsApp
{
    public class AblerexRS232
    {
        //Serial port class
        SerialPort rs232;

        //Get Method
        //UPS Information
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public string FirmwareVersion { get; private set; }

        //UPS Status Variable
        public float InputVoltage { get; private set; }
        public float OutputVoltage { get; private set; }
        public double BatteryPercent { get; private set; }
        public int UPSLoad { get; private set; }
        public double UPSLoadAmp { get; private set; }
        public float UPSLoadWatt { get; private set; }
        public float InputFrequency { get; private set; }
        public float Temperature { get; private set; }
        public string StatusFlag { get; private set; }
        public string RawData { get; private set; }

        //Rated Information
        public float RatedVoltage { get; private set; }
        public int RatedCurrent { get; private set; }
        public float BatteryVoltage { get; private set; }
        public float RatedFrequency { get; private set; }

        //Q3 Information
        public int DisplaySystem { get; private set; }
        public int VoltageOutput { get; private set; }
        public int UPSMode { get; private set; }
        public int FineTuning { get; private set; }
        public int BypassWindow { get; private set; }
        public int Synchronization { get; private set; }
        public bool Q3DataAvaiable { get; set; }

        public bool UPSConnect(string szPortName)
        {
            try
            {
                rs232 = new SerialPort(szPortName);

                //Settings from ablerex pic datasheet
                rs232.BaudRate = 2400;
                rs232.Parity = Parity.None;
                rs232.StopBits = StopBits.One;
                rs232.NewLine = "\r"; //Ablerex PIC using 0xD as new line character.

                rs232.Open();
                rs232.DataReceived += DataReceiveHandler;
                Q3DataAvaiable = false;

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("COM Port failed!", "Error!");
                return false;
            }
        }

        public void UPSDisconect()
        {
            if (IsOpen() == true)
            {
                rs232.Close();
            }
        }

        public void DataReceiveHandler(object sender, SerialDataReceivedEventArgs e)
        {
            /*
             Q1 Command
             (000.8 160.0 230.1 009 50.0 2.22 32.1 10000001 (batt mode)
             (214.3 110.0 214.1 009 50.0 2.12 32.0 00100001 (power mode with eco bypass)
             (212.6 110.0 229.7 008 49.9 2.22 32.4 00000001 (power mode normal)

             F Command
             #230.0 008 048.0 50.0

             I Command
             #Ablerex         RS-RT 2000 AS041811
             */
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            Console.WriteLine(indata); //for debug

            double BatVal;
            bool upsType = false;

            if (indata.Substring(0, 1) == "#")
            {
                indata = indata.Substring(1);

                if (indata.Contains("Ablerex") || indata.Contains("ABLEREX")) //I Command
                {
                    string[] ModelInfo = indata.Split(' ');
                    Brand = ModelInfo[0];
                    Model = ModelInfo[9] + " " + ModelInfo[10];
                    FirmwareVersion = ModelInfo[11];
                }
                else //F Command
                {
                    string[] ratedInfo = indata.Split(' ');
                    
                    RatedVoltage = (float)Convert.ToDouble(ratedInfo[0]);
                    RatedCurrent = Convert.ToInt32(ratedInfo[1]);
                    BatteryVoltage = (float)Convert.ToDouble(ratedInfo[2]);
                    RatedFrequency = (float)Convert.ToDouble(ratedInfo[3]);
                }
            }
            else if (indata.Substring(0, 1) == "(") //Q1 Command
            {
                indata = indata.Substring(1);
                RawData = indata;
                string[] upsState = indata.Split(' ');

                InputVoltage = (float)Convert.ToDouble(upsState[0]);
                OutputVoltage = (float)Convert.ToDouble(upsState[2]);
                UPSLoad = Convert.ToInt32(upsState[3]);
                UPSLoadAmp = (Convert.ToDouble(UPSLoad) / Convert.ToDouble(100.0)) * RatedCurrent;
                UPSLoadWatt = (float)Convert.ToDouble(OutputVoltage * UPSLoadAmp);

                InputFrequency = (float)Convert.ToDouble(upsState[4]);
                Temperature = (float)Convert.ToDouble(upsState[6]);

                //UPS Battery Capacity
                StatusFlag = upsState[7];
                BatVal = Convert.ToDouble(upsState[5]);

                if (Convert.ToInt16(RawData.Substring(42, 1)) == 0)
                {
                    upsType = false;
                }
                else if (Convert.ToInt16(RawData.Substring(42, 1)) == 1)
                {
                    upsType = true;
                }

                //BatteryVoltage
                BatteryPercent = CalCapacity(upsType, BatVal.ToString(), BatteryVoltage.ToString());
            }
            else if (indata.Substring(0, 5) == "M02.0" || indata.Substring(0, 5) == "M00.0") //Q3 Command
            {
                indata = indata.Substring(5);

                DisplaySystem = Convert.ToInt32(indata.Substring(0, 1)); //Display System
                VoltageOutput = Convert.ToInt32(indata.Substring(1, 1)); //Vout
                UPSMode = Convert.ToInt32(indata.Substring(4, 1)); //UPS Mode
                FineTuning = Convert.ToInt32(indata.Substring(7, 1)); //Fine Tuning
                BypassWindow = Convert.ToInt32(indata.Substring(8, 1)); //Bypass Window
                Synchronization = Convert.ToInt32(indata.Substring(9, 1)); //Synchronization

                Q3DataAvaiable = true;
            }
        }

        private double CalCapacity(bool UPStype, string BatVol, string RatVol)
        {
            double num = double.Parse(BatVol);
            double num2 = double.Parse(RatVol);
            double value;
            if (!UPStype)
            {
                value = (num - 1.67) / 0.54 * 90.0 + 0.45;
                if (num > 2.21)
                {
                    value = 100.0;
                }
                if (num < 1.67)
                {
                    value = 0.0;
                }
            }
            else
            {
                double num3 = num2 / 12.0;
                if (num3 < 1.0)
                {
                    num3 = 1.0; //num3 = battery count
                }

                value = (num / num3 - 10.0) / 3.3 * 91.0 + 0.45;
                if (num / num3 > 13.3)
                {
                    value = 100.0;
                }
                if (num / num3 < 10.0)
                {
                    value = 0.0;
                }
            }
            return value;
        }

        public void SendCommand(string szCommand)
        {
            if (IsOpen() == true)
            {
                string szWriteCommand = szCommand + "\r";
                rs232.Write(szWriteCommand);
            }
        }

        public bool IsOpen()
        {
            if(rs232 != null)
            {
                return rs232.IsOpen;
            }
            else
            {
                return false;
            }
        }
    }
}

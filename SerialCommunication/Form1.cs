using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        SerialPort SerialPortArduino = new SerialPort();

        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (SerialPortArduino.IsOpen)
            {
                SerialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }
            else
            {
                SerialPortArduino.PortName = comboBoxPoort.SelectedItem.ToString();
                SerialPortArduino.BaudRate = Convert.ToInt32(comboBoxBaudrate.SelectedItem);
                SerialPortArduino.ReadTimeout = 500;  //timeout
                SerialPortArduino.WriteTimeout = 500;  //timeout
                SerialPortArduino.Open();
                radioButtonVerbonden.Checked = true;
                buttonConnect.Text = "Disconnect";
            }
        }

        private void checkBoxDigital2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerOefening5.Enabled = tabControl.SelectedIndex == 5;
        }

        private void timerOefening5_Tick(object sender, EventArgs e)
        {

            try
            {
                if (SerialPortArduino.IsOpen)
                {
                    // ─── Gewenste Temperatuur (analoge pin 0) ───────────────
                    double rc_gewenst = 40.0 / 1023.0;
                    double offset_gewenst = 5.0;

                    SerialPortArduino.ReadExisting();
                    SerialPortArduino.WriteLine("get a0");
                    int rawGewenst = Convert.ToInt32(SerialPortArduino.ReadLine().Trim());
                    double gewensteTemp = rc_gewenst * rawGewenst + offset_gewenst;

                    labelGewensteTemp.Text = gewensteTemp.ToString("F1") + " °C";

                    // ─── Huidige Temperatuur (analoge pin 1) ─────────────────
                    double rc_huidig = 500.0 / 1023.0;
                    double offset_huidig = 0.0;

                    SerialPortArduino.ReadExisting();
                    SerialPortArduino.WriteLine("get a1");
                    int rawHuidig = Convert.ToInt32(SerialPortArduino.ReadLine().Trim());
                    double huidigeTemp = rc_huidig * rawHuidig + offset_huidig;

                    labelHuidigeTemp.Text = huidigeTemp.ToString("F1") + " °C";

                    // ─── Led aansturen (digitale pin 2) ──────────────────────
                    if (huidigeTemp < gewensteTemp)
                    {
                        SerialPortArduino.WriteLine("set d2 1");  // LED AAN
                    }
                    else
                    {
                        SerialPortArduino.WriteLine("set d2 0");  // LED UIT
                    }
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = "error: " + ex.Message;
                SerialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";

                timerOefening5.Enabled = false; // Stop de timer!
                labelStatus.Text = "error: " + ex.Message;
                SerialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
            }

        }
    }
}

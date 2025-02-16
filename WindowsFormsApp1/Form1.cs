using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();
            LoadAvailablePorts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // This runs when the form loads. You can leave it empty or add logic.
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle selection change (you can leave it empty for now)
        }

        // Step 1: Load Available COM Ports
        private void LoadAvailablePorts()
        {
            cmbPorts.Items.Clear();
            cmbPorts.Items.AddRange(SerialPort.GetPortNames());
            if (cmbPorts.Items.Count > 0)
                cmbPorts.SelectedIndex = 0;  // Select first port
            else
                MessageBox.Show("No COM ports found. Connect a device and restart.");
        }

        // Step 2: Connect to Selected COM Port
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cmbPorts.SelectedItem == null)
            {
                MessageBox.Show("Select a COM port first.");
                return;
            }

            try
            {
                serialPort = new SerialPort(cmbPorts.SelectedItem.ToString(), 1152000, Parity.None, 8, StopBits.One);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                lblStatus.Text = "Connected to " + cmbPorts.SelectedItem.ToString();
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                btnSend.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Step 3: Handle Incoming Data from Serial Device
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine(); // Read incoming data
            Invoke(new Action(() => txtReceivedData.AppendText(data + Environment.NewLine))); // Update UI safely
        }

        // Step 4: Send Data to Serial Device
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.WriteLine(txtSendData.Text);
                txtSendData.Clear();
            }
            else
            {
                MessageBox.Show("Serial port is not open.");
            }
        }

        // Step 5: Disconnect Serial Port
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                lblStatus.Text = "Disconnected";
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                btnSend.Enabled = false;
            }
        }

        // Step 6: Clear Received Data
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceivedData.Clear();
        }

        // Step 7: Close Serial Port When Closing App
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
        }
    }
}

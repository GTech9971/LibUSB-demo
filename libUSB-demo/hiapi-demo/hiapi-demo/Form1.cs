using HidApiAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hiapi_demo {
    public partial class Form1 : Form {

        private const int VID = 0x04D8;
        private const int PID = 0x003F;

        private HidDevice hidDevice;

        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            var devices = HidDeviceManager.GetManager().SearchDevices(0, 0);

            if (devices.Any()) {
                foreach (var device in devices) {
                    device.Connect();

                    Console.WriteLine(
                      $"device: {device.Path()}\n" +
                      $"manufacturer: {device.Manufacturer()}\n" +
                      $"product: {device.Product()}\n" +
                      $"serial number: {device.SerialNumber()}\n");

                    device.Disconnect();
                }

            }
        }


        private void buttonToggle_Click(object sender, EventArgs e) {
            this.hidDevice = HidDeviceManager.GetManager().SearchDevices(VID, PID).FirstOrDefault();

            if (this.hidDevice == null) {
                MessageBox.Show("Not found device", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                bool connect = this.hidDevice.Connect();
                if (connect == false) {
                    throw new Exception("device not connect");
                }

                byte[] data = new byte[64];
                data[0] = 0x80;
                int ret = this.hidDevice.Write(data);
                if (ret == -1) {
                    throw new Exception("Cannot write data from device");
                }

                byte[] buff = new byte[64];
                ret = this.hidDevice.Read(buff, buff.Length);
                if (ret == -1) {
                    throw new Exception("Cannot read data from device");
                }
                Console.WriteLine(buff[0]);

                data[0] = 0x70;
                ret = this.hidDevice.Write(data);
                if (ret == -1) {
                    throw new Exception("Cannot write data from device");
                }

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.StackTrace);
            } finally {
                if (this.hidDevice.IsConnected) {
                    this.hidDevice.Disconnect();
                }
            }

        }
    }
}

using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Windows.Forms;

namespace libUSB_demo {
    public partial class Form1 : Form {

        private const int VENDER_ID = 0x04D8;
        private const int PRODUCT_ID = 0x003F;

        public static UsbDevice MyUsbDevice;
        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(VENDER_ID, PRODUCT_ID);

        public Form1() {
            InitializeComponent();
        }

        private void buttonToggle_Click(object sender, EventArgs e) {
            UsbEndpointWriter writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            ErrorCode ec = ErrorCode.None;

            int bytesWritten;
            byte[] data = new byte[64];
            data[0] = 0x80;

            ec = writer.Write(data, 2000, out bytesWritten);
            if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

            //書き込み
            UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
            byte[] readBuffer = new byte[64];
            int bytesRead;


            ec = reader.Read(readBuffer, 100, out bytesRead);
            textBoxConsole.AppendText(readBuffer[0].ToString());
            textBoxConsole.AppendText(readBuffer[1].ToString());
        }

        private void buttonConnect_Click(object sender, EventArgs e) {
            ErrorCode ec = ErrorCode.None;
            try {
                MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

                if (MyUsbDevice == null) throw new Exception("Device Not Found.");
                IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null)) {
                    wholeUsbDevice.SetConfiguration(1);
                    wholeUsbDevice.ClaimInterface(0);
                }

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxConsole.AppendText(ex.Message);
                textBoxConsole.AppendText(ex.StackTrace);
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

        }
    }
}

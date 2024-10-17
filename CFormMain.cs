using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class CFormMain : Form
    {
        public TcpClient mTcpClient = null;
        public NetworkStream mStream = null;

        public CFormMain()
        {
            InitializeComponent();

            // txtSend에 KeyDown 이벤트 핸들러 추가
            txtSend.KeyDown += new KeyEventHandler(txtSend_KeyDown);
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            if (mTcpClient == null)
            {
                mTcpClient = new TcpClient(AddressFamily.InterNetwork);
                IPAddress iPAddress = IPAddress.Parse(txtIpAddress.Text);
                int port = Convert.ToInt32(txtPort.Text);

                IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
                try
                {
                    mTcpClient.Connect(iPEndPoint);
                    timerReceive.Enabled = true;
                    mStream = mTcpClient.GetStream();
                }
                catch (SocketException)
                {
                    MessageBox.Show("서버에 접속하세요!", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    mTcpClient = null;
                }
            }
            else
            {
                mStream.Close();
                mStream.Dispose();
                mStream = null;

                mTcpClient.Close();
                mTcpClient.Dispose();
                mTcpClient = null;

                timerReceive.Enabled = false;
            }
        }


        private void btSend_Click(object sender, EventArgs e)
        {
            SendText();
        }

        // txtSend의 KeyDown 이벤트 핸들러
        private void txtSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendText();

                // 텍스트 전송 후 Enter 키의 기본 동작(예: 줄바꿈) 방지
                e.SuppressKeyPress = true;
            }
        }

        // 텍스트 전송 코드를 별도의 메서드로 추출
        private void SendText()
        {
            if (mTcpClient == null || mTcpClient.Connected == false)
            {
                MessageBox.Show("서버에 접속하세요!", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] data = Encoding.Default.GetBytes(txtSend.Text);
            mTcpClient.Client.Send(data);

            // 메시지 전송 후, 메시지 입력창 비우기
            txtSend.Text = string.Empty;
        }

        private void timerReceive_Tick(object sender, EventArgs e)
        {
            if (mTcpClient == null || mTcpClient.Connected == false || mStream == null)
            {
                return;
            }

            if (mStream.DataAvailable == true)
            {
                byte[] bzReceive = new byte[1024];

                mStream.ReadAsync(bzReceive, 0, 1024);

                string strReceive = Encoding.Default.GetString(bzReceive);

                StringBuilder sb = new StringBuilder(strReceive);

                txtReceive.Text += sb;
                txtReceive.AppendText("\r\n");
            }
        }

        private void txtReceive_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceive.Text = string.Empty;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (mTcpClient != null && mTcpClient.Connected)
            {
                mStream.Close();
                mStream.Dispose();
                mStream = null;

                mTcpClient.Close();
                mTcpClient.Dispose();
                mTcpClient = null;

                timerReceive.Enabled = false;
            }
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            if (mTcpClient != null && mTcpClient.Connected)
            {
                string command = "안녕하세요!"; // 전송할 명령
                byte[] data = Encoding.Default.GetBytes(command);
                mTcpClient.Client.Send(data);
            }
            else
            {
                MessageBox.Show("서버에 접속되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

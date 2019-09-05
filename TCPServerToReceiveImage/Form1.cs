using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPServerToReceiveImage
{
    public partial class Form1 : Form
    {

        #region 变量

        static Socket receiveSocket;

        static Socket clientSocket;


        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private async void StartListen_button(object sender, EventArgs e)
        {

            //监听的ip地址
            string ipAddress = textBox1.Text;

            //监听的端口
            string port = textBox2.Text;
            button1.Enabled = false;

            richTextBox1.AppendText("监听。。。。\n");
            receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), int.Parse(port));

            receiveSocket.Bind(ipEndPoint);
            int value = 0;

            while (!button1.Enabled)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            receiveSocket.Listen(1);
                            MemoryStream ms = new MemoryStream();
                            byte[] receiveBuffer = new byte[10 * 1024 * 1024];
                            int length = 0;
                            clientSocket = receiveSocket.Accept();


                            while ((length = clientSocket.Receive(receiveBuffer)) > 0)
                            {
                                ms.Write(receiveBuffer, 0, length);

                            }

                            ms.Flush();

                            Bitmap bm = new Bitmap(ms);
                            bm.Save(@"E:\研二上学期\项目\ProjectFileStorage\TCPServerForImage\receive"+Interlocked.Increment(ref value)+".jpg", ImageFormat.Png);

                            ms.Close();
                            

                        }
                        catch (Exception ex)
                        {
                            
                            richTextBox1.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(ex.Message);
                            }));
                        }
                        
                    });
                    richTextBox1.AppendText($"已经接收:receive{value}.jpg-" + "100/100--" + DateTime.Now + "-\n");

                }
                catch (Exception ex)
                {

                    richTextBox1.AppendText(ex.Message + "\n");
                }
            }
        }

        private void StopListen_button(object sender, EventArgs e)
        {

            richTextBox1.AppendText("Stop monitor.");

            try
            {

                button1.Enabled = true;

                //关闭发送连接
                receiveSocket.Shutdown(SocketShutdown.Both);
                receiveSocket.Close();

                //关闭接收数据的socket
                clientSocket.Shutdown(SocketShutdown.Receive);
                clientSocket.Close();



            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.Message + "\n");
            }

        }
    }
}


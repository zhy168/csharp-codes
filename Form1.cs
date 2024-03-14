using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;

namespace form_test_1
{
 
    

    public partial class Form1 : Form
    {
        //参数区域
        double[] Values = new double[30];    
        Stopwatch sw = Stopwatch.StartNew();
        private  SPL<double> spl;
        public static double dis;
        public  double[] y_value = new double[] { };
        public  double[] x_value = new double[] { };
        private void InitData()
        {
            spl =  new SPL<double>()
            {
                Color = Color.FromArgb(68, 110, 200),
                MarkerSize = 3,
                Smooth = false
            };
            if (spl.Count != 0)
            {
                x_value = spl.GetXs().ToArray();
                y_value = spl.GetYs().ToArray();
                spl.Clear();
            }
            else
            {
                spl.Add(DateTime.Now.ToOADate(), 0);
                spl.AddRange(x_value,y_value);
                formsPlot1.Plot.Add(spl);
                formsPlot1.Plot.XAxis.DateTimeFormat(true);
                formsPlot1.Plot.AxisAuto();
                formsPlot1.Refresh();

            }
        }

        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;  
            InitData();
            formsPlot2.Plot.XLabel("时间");
            formsPlot2.Plot.YLabel("mm");
            formsPlot2.Plot.Title("distance : mm");

        }

        List<double> datetime = new List<double>();
        List<double> value1 = new List<double>();
        //Random random = new Random();

        private void useTCP()
        {
            string connectIP = textBox1.Text.Trim();
            int PORT = int.Parse(textBox2.Text.Trim());
            Socket _socket = null;
            byte[] buffer = new byte[4];

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(connectIP);
                IPEndPoint endPoint = new IPEndPoint(address, PORT);
                _socket.Connect(endPoint);
                while (true) 
                {
                    dis = bytesToInt(buffer, 0);
                    int length = _socket.Receive(buffer);
                    richTextBox1.AppendText("成功接收 "+ length.ToString()+ "bytes\n");
                    richTextBox1.AppendText("distance " + dis.ToString() + "\n");

                    
                    datetime.Add(DateTime.Now.ToOADate());
                    formsPlot2.Plot.XAxis.DateTimeFormat(true);
                    formsPlot2.Plot.AxisAuto();
                    value1.Add(dis);

                    //value1.Add(random.NextDouble() * 10);
                    if (value1.Count() > 30)
                    {
                        value1.RemoveAt(0);
                        datetime.RemoveAt(0);
                    }
                    formsPlot2.Plot.AddSignalXY(datetime.ToArray(), value1.ToArray(), Color.Red);
                    richTextBox1.AppendText("List count : " + value1.Count.ToString() + "\n");

                    this.Invoke(new Action(() => { formsPlot2.Refresh(); }));
                    
                    //Array.Clear(buffer, 0, buffer.Length);
                    Thread.Sleep(500);

                }

            }
            catch (Exception ex)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                value1.Clear();
                richTextBox1.AppendText("use_SOCKET_ERROR : " + ex.Message);

            }


        }

        public static int bytesToInt(byte[] src, int offset = 0)
        {
            int value;
            value = (int)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }

        private void btn_click(object sender, EventArgs e)
        {
            Thread thread2 = new Thread(new ThreadStart(useSPL));
            thread2.Name = "formplot1";
            thread2.IsBackground = true;
            thread2.Start();


        }

      
        private void useSPL()
        {
            while (true)
            {
                double x = DateTime.Now.ToOADate();
                double y = dis;
                spl.Add(x, y);
                if (spl.Count > 10)
                {
                    spl.GetXs().RemoveAt(0);
                    spl.GetYs().RemoveAt(0);
                }
                formsPlot1.Plot.AxisAuto();
                this.Invoke(new Action(() => { formsPlot1.Refresh(); }));
                Thread.Sleep(500);
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread3 = new Thread(new ThreadStart(useTCP));
            thread3.Name = "TCP线程1，formplot2";
            thread3.IsBackground = true;
            thread3.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            datetime.Clear();
            value1.Clear();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
    public class SPL<T> : ScatterPlotList<T>
    {
        public List<T> GetXs()
        {
            return Xs;
        }
        public List<T> GetYs()
        {
            return Ys;
        }
    }
}

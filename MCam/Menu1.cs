using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace MCam
{
    public partial class Menu1 : Form
    {
        public int pid;
        public string pname;
        public bool Goto;
        const bool F = false;
        const bool T = true;
        public int fps = 30;
        public Menu1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e){}
        [DllImport("User32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            returnpath();
        }
        public string returnpath()
        {
            string repath = saveFileDialog1.FileName;
            return (repath);
        }

        public void button1_Click(object sender, EventArgs e)
        {
            CreateProc();
        }
        public StreamWriter sw;
        public void CreateProc()
        {
            if (returnpath() == "") {saveFileDialog1.ShowDialog();};
            if (returnpath() == "") {return;};
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get()){ coreCount += int.Parse(item["NumberOfCores"].ToString()); }
            if (String.IsNullOrEmpty(Form1.ffHeight) || String.IsNullOrEmpty(Form1.ffWidth)) { Form1.ffOffX = Convert.ToString(System.Windows.Forms.Screen.PrimaryScreen.Bounds.X); Form1.ffOffY = Convert.ToString(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y); Form1.ffHeight = Convert.ToString(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height); Form1.ffWidth = Convert.ToString(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width); };
            string cmd = "ffmpeg -y -f gdigrab -draw_mouse "+ Convert.ToString(Convert.ToInt32(checkBox1.Checked)) + " -framerate " + fps + " -offset_x " + Form1.ffOffX + " -offset_y " + Form1.ffOffY + " -video_size " + Form1.ffWidth +"x" + Form1.ffHeight + " -i desktop -pix_fmt +yuv420p -b:v "+ textBox2.Text +" -threads "+ coreCount + " \"" + returnpath() + "\"";
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C mode con cols=76 lines=20 & " + cmd;
            Console.WriteLine(process.StartInfo.Arguments);
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;
            
            //* Start process and handlers
            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.Start();
            sw = process.StandardInput;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            int procidint = process.Id;
            string procid = procidint.ToString();
            //* PostMessage(process.MainWindowHandle, 0x0101, 0x59, 0);
            pid = process.Id;
            pname = process.ProcessName;
            Goto=T; RecordCheck();
            //this.WindowState = FormWindowState.Minimized;
        }
        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
            CheckForIllegalCrossThreadCalls = false;
            try {
                textBox3.AppendText(outLine.Data + Environment.NewLine);
            }
            catch (NullReferenceException) { }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e) {
            //*CreateProc().WriteLine("q");
            //*PostMessage(CreateProc().MainWindowHandle, 0x0100, 0x51, 0); //*.MainWindowHandle
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                sw.Write("q");
            }
            catch (System.ArgumentException) { pid = 0;Goto=F;}
            catch (System.NullReferenceException) { pid = 0; Goto = F; }
        }

        async Task RecordCheck()
        {
        RecCheck:
            label1.Visible = false;
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                if (process.ProcessName == pname) {label1.Visible = true;}
            }
            catch (System.ArgumentException) {pid=0;Goto=F;};
            await Task.Delay(2000);
            if (Goto==T) {goto RecCheck;}
        }
        string bf = "";
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try { fps = Convert.ToInt32(textBox1.Text); }
            catch (System.FormatException) { textBox1.Text = bf;};
            bf = textBox1.Text;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.InstanceRef = this;
            form1.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
                        try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                sw.Write("q");
            }
            catch (System.ArgumentException) { pid = 0;Goto=F;}
            catch (System.NullReferenceException) { pid = 0; Goto = F; }
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HTCLIENT)
                        m.Result = (IntPtr)HTCAPTION;
                    return;
            }
            base.WndProc(ref m);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
    }
}

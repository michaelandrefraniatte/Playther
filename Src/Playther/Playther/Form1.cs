using System;
using System.Windows.Forms;
using WebView2 = Microsoft.Web.WebView2.WinForms.WebView2;
using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace Playther
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        public static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        public static uint CurrentResolution = 0;
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        public WebView2 webView21 = new WebView2();
        private static int x, y, cx, cy;
        private async void Form1_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            TrayMenuContext();
            this.ShowInTaskbar = false;
            cx = this.Size.Width;
            cy = this.Size.Height;
            x = width - cx;
            y = 0;
            this.Location = new Point(x, y);
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--disable-web-security", "--autoplay-policy=no-user-gesture-required");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await webView21.EnsureCoreWebView2Async(environment);
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "assets", CoreWebView2HostResourceAccessKind.DenyCors);
            string filepath = @"file:///" + System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "/").Replace("Playther.exe", "") + "assets/index.html";
            webView21.Source = new System.Uri(filepath);
            webView21.DefaultBackgroundColor = Color.Transparent;
            webView21.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView21.Dock = DockStyle.Fill;
            this.Controls.Add(webView21);
            using (System.IO.StreamWriter createdfile = new System.IO.StreamWriter(Application.StartupPath + @"\temphandle"))
            {
                createdfile.WriteLine(Process.GetCurrentProcess().MainWindowHandle);
            }
        }
        private void TrayMenuContext()
        {
            this.notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.notifyIcon1.ContextMenuStrip.Items.Add("Quit", null, this.MenuTest1_Click);
        }
        void MenuTest1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\temphandle"))
                using (System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + @"\temphandle"))
                {
                    IntPtr handle = new IntPtr(int.Parse(file.ReadLine()));
                    ShowWindow(handle, 9);
                    SetForegroundWindow(handle);
                    Microsoft.VisualBasic.Interaction.AppActivate("Playther");
                }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            webView21.Dispose();
        }
    }
}
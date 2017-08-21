using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelperLibrary.Loading
{
    public partial class InitialApplicationLoader : Form
    {
        private readonly Color DARK_BACKGROUND_COLOR = Color.FromArgb(45, 45, 48);
        private readonly Color DARK_FOREGROUND_COLOR = Color.FromArgb(212, 212, 212);

        private readonly Color LIGHT_BACKGROUND_COLOR = Color.WhiteSmoke;        
        private readonly Color LIGHT_FOREGROUND_COLOR = Color.Black;

        private string applicationTitle;
        private Theme windowTheme;
        private Action onLoadingAbort;
        private List<ToDo> todos;

        public delegate void OnLoadingFinished(object sender, EventArgs e);
        public event OnLoadingFinished LoadingFinished;

        public InitialApplicationLoader(string applicationTitle, Theme windowTheme, Action onLoadingAbort)
        {
            todos = new List<ToDo>();

            this.applicationTitle = applicationTitle;
            this.windowTheme = windowTheme;
            this.onLoadingAbort = onLoadingAbort;

            InitializeComponent();

            ApplyTheme();
        }

        public void AddToDo(ToDo toDo)
        {
            todos.Add(toDo);
        }

        public void StartLoading()
        {
            this.ShowDialog();            
        }

        private void InitialApplicationLoader_Load(object sender, EventArgs e)
        {
            ExecuteToDos();
        }

        private void PrepareGUI()
        {
            int todos = this.todos.Count;
            pgb_loading.Maximum = todos;
        }

        private async void ExecuteToDos()
        {
            PrepareGUI();
            for (int i = 0; i < todos.Count(); i++)
            {
                ToDo todo = todos[i];

                lbl_loading_description.Text = todo.Description;
                lbl_loading_current.Text = (i + 1).ToString();

                await Task.Run(() => todo.ToDoAction);

                pgb_loading.Value++;
            }
        }

        private void ApplyTheme()
        {
            switch (windowTheme)
            {
                case Theme.Dark:                    
                    lbl_window_title.Text = applicationTitle + " | Initializing Application";
                    break;
                case Theme.Light:
                    pnl_app.Hide();
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;  
                    this.Text = applicationTitle + " | Initializing Application";
                    this.BackColor = LIGHT_BACKGROUND_COLOR;
                    lbl_loading_description.ForeColor = LIGHT_FOREGROUND_COLOR;
                    lbl_loading_max.ForeColor = LIGHT_FOREGROUND_COLOR;
                    break;
            }                        
        }

        public enum Theme
        {
            Dark,
            Light
        }

        private void pb_close_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure to quit the Application?", "Exit Application", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                onLoadingAbort();
                Environment.Exit(1);
            }
        }

        #region CustomWindowHandling
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void pnl_app_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                const int resizeArea = 10;
                Point cursorPosition = PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                if (cursorPosition.X >= ClientSize.Width - resizeArea && cursorPosition.Y >= ClientSize.Height - resizeArea)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }

            base.WndProc(ref m);
        }
        
        private void pb_close_MouseDown(object sender, MouseEventArgs e)
        {
            pb_close.Image = Properties.Resources.close_pressed;
        }

        private void pb_close_MouseEnter(object sender, EventArgs e)
        {
            pb_close.Image = Properties.Resources.close_hover;
        }

        private void pb_close_MouseLeave(object sender, EventArgs e)
        {
            pb_close.Image = Properties.Resources.close;
        }

        private void pb_close_MouseUp(object sender, MouseEventArgs e)
        {
            pb_close.Image = Properties.Resources.close;
        }
        #endregion
    }
}

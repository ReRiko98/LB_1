using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;


namespace NoteApp
{
    public class UIManager
    {
        private Form mainForm;
        private Panel currentPanel;
        private Dictionary<string, Panel> screens;

        public UIManager(string title, int width = 800, int height = 600)
        {
            screens = new Dictionary<string, Panel>();

            // Инициализация главного окна
            mainForm = new Form
            {
                Text = title,
                Width = width,
                Height = height,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.LightGray
            };

            currentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            mainForm.Controls.Add(currentPanel);
        }

        // Метод для добавления нового экрана
        public void AddScreen(string screenName, Panel panel)
        {
            if (!screens.ContainsKey(screenName))
            {
                screens[screenName] = panel;
            }
        }

        // Метод для переключения между экранами
        public void SwitchScreen(string screenName)
        {
            if (screens.ContainsKey(screenName))
            {
                currentPanel.Controls.Clear();
                currentPanel.Controls.Add(screens[screenName]);
            }
        }

        public void Run() => Application.Run(mainForm);
    }

    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new View.NoteList());
        }
    }
}

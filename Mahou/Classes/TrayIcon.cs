using System;
using System.Windows.Forms;

namespace Mahou
{
    public class TrayIcon
    {
        public event EventHandler<EventArgs> Exit;
        public event EventHandler<EventArgs> ShowHide;
        public NotifyIcon trIcon;
        ContextMenuStrip cMenu;
        ToolStripMenuItem Exi, ShHi;
        public TrayIcon(bool? visible = true)
        {
            trIcon = new NotifyIcon();
            cMenu = new ContextMenuStrip();
            trIcon.Icon = Properties.Resources.MahouTrayHD;
            trIcon.Visible = visible == true;
            Exi = new ToolStripMenuItem("Exit", null, ExitHandler);
            ShHi = new ToolStripMenuItem("Show/Hide", null, ShowHideHandler);
            cMenu.Items.Add(ShHi);
            cMenu.Items.Add(Exi);
            trIcon.Text = "Mahou (魔法)\nA magical layout switcher.";
            trIcon.ContextMenuStrip = cMenu;
            trIcon.MouseDoubleClick += ShowHideHandler;
            trIcon.BalloonTipClicked += ExitHandler;
        }
        void ExitHandler(object sender, EventArgs e)
        {
            if (Exit != null)
            {
                Exit(this, null);
            }
        }
        void ShowHideHandler(object sender, EventArgs e)
        {
            if (ShowHide != null)
            {
                ShowHide(this, null);
            }
        }
        public void Hide()
        {
            trIcon.Visible = false;
        }
        public void Show()
        {
            trIcon.Visible = true;
        }
        public void RefreshText(string TrText, string ShHiText, string ExiText)
        {
            trIcon.Text = TrText;
            ShHi.Text = ShHiText;
            Exi.Text = ExiText;
        }
    }
}

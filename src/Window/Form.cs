using System.Diagnostics;
using System.Windows.Forms;

sealed partial class Form : System.Windows.Forms.Form
{
    internal Form()
    {
        Text = "Flarial Launcher";
        MinimizeBox = MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        ClientSize = new(400, 300);
        StartPosition = FormStartPosition.CenterScreen;

        TabControl tabControl = new() { Dock = DockStyle.Fill }; Controls.Add(tabControl);
        tabControl.TabPages.Add(new PlayPage());
        tabControl.TabPages.Add(new VersionsPage());
    }
}
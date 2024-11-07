using System.Diagnostics;
using System.Windows.Forms;

sealed class Form : System.Windows.Forms.Form
{
    internal Form()
    {
        FormBorderStyle = FormBorderStyle.FixedSingle;
        ClientSize = new(400, 300);
        TabControl tabControl = new() { Dock = DockStyle.Fill }; Controls.Add(tabControl);
        tabControl.TabPages.Add(new Play());
    }
}
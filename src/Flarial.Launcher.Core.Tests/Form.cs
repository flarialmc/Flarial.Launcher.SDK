using System.Drawing;
using System.Windows.Forms;

sealed class Form : System.Windows.Forms.Form
{
    internal Form()
    {
        Text = "Flarial Launcher";
        Font = SystemFonts.MessageBoxFont;
        ClientSize = new(800 / 2, 600 / 2);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        TabControl value = new() { Dock = DockStyle.Fill }; Controls.Add(value);
        value.TabPages.AddRange([new Play(this), new Versions(this)]);
    }
}
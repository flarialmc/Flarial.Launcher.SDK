using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Flarial.Launcher;

sealed class Form : System.Windows.Forms.Form
{
    internal Catalog Catalog = default;

    internal Form()
    {
        Text = "Flarial Launcher";
        Font = SystemFonts.MessageBoxFont;
        ClientSize = LogicalToDeviceUnits(new Size(400, 300));
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        TabControl tabControl = new()
        {
            Dock = DockStyle.Fill,
            Visible = false
        };
        Controls.Add(tabControl);

        Load += async (_, _) =>
        {
            Catalog = await Catalog.GetAsync();
         
            SuspendLayout();
            Controls.Add(new Pages(new Play(this), new Versions(this)));
            ResumeLayout();
        };
    }
}
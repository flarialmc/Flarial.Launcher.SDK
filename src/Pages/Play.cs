using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

sealed class Play : TabPage
{
    internal Play()
    {
        Text = "Play";
        TableLayoutPanel tableLayoutPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.White
        };
        Controls.Add(tableLayoutPanel);

        Button button = new()
        {
            Text = "Launch",
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.None,
            Width = 50 * 3,
            Height = 20 * 3
        };
        tableLayoutPanel.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        tableLayoutPanel.Controls.Add(button);

        ProgressBar progressBar = new()
        {
            Dock = DockStyle.Bottom,
            Height = 23,
            Visible = false
        };
        progressBar.VisibleChanged += (sender, _) =>
        {
            var progressBar = (ProgressBar)sender;
            if (!progressBar.Visible) { progressBar.Value = 0; progressBar.Style = ProgressBarStyle.Marquee; }
        };
        tableLayoutPanel.RowStyles.Add(new() { SizeType = SizeType.AutoSize, Height = 0 });
        tableLayoutPanel.Controls.Add(progressBar);


        button.Click += (_, _) =>
        {
            button.Enabled = false;
            progressBar.Visible = true;

            button.Text = "Downloading...";
            progressBar.Style = ProgressBarStyle.Blocks;

            button.Text = "Waiting...";
            progressBar.Style = ProgressBarStyle.Marquee;

            button.Enabled = true;
            progressBar.Visible = false;
        };
    }
}
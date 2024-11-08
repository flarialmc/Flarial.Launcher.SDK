using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Launcher;

sealed partial class PlayPage : TabPage
{
    internal PlayPage()
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

        CheckBox checkBox = new() { Text = "Beta", AutoSize = true, Margin = default, Anchor = AnchorStyles.None, Enabled = false };
        tableLayoutPanel.Controls.Add(checkBox);

        ProgressBar progressBar = new()
        {
            Dock = DockStyle.Bottom,
            Height = 23,
            Visible = false,
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 1
        };
        progressBar.VisibleChanged += (sender, _) =>
        {
            var progressBar = (ProgressBar)sender;
            if (!progressBar.Visible) { progressBar.Value = 0; progressBar.Style = ProgressBarStyle.Marquee; }
        };
        tableLayoutPanel.RowStyles.Add(new() { SizeType = SizeType.AutoSize, Height = 0 });
        tableLayoutPanel.Controls.Add(progressBar);


        button.Click += async (_, _) =>
        {
            button.Enabled = false;
            progressBar.Visible = true;

            button.Text = "Downloading...";
            await Client.DownloadAsync((_) =>
            {
                if (progressBar.Value != _)
                {
                    if (progressBar.Style is ProgressBarStyle.Marquee)
                        progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Value = _;
                }
            }, checkBox.Checked);

            button.Text = "Waiting...";
            progressBar.Style = ProgressBarStyle.Marquee;
            await Client.LaunchAsync(checkBox.Checked);

            button.Text = "Launch";
            button.Enabled = true;
            progressBar.Visible = false;
        };
    }
}
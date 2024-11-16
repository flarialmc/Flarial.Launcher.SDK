using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Flarial.Launcher;
using Minecraft.UWP;


sealed class Play : TabPage
{
    internal Play(Form _)
    {
        Text = "Play";
        TableLayoutPanel panel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.White
        };
        Controls.Add(panel);

        Button button = new()
        {
            Text = "Launch",
            Width = 150,
            Height = 50,
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.None
        };

        CheckBox checkBox = new()
        {
            Text = "Beta",
            AutoSize = true,
            Anchor = AnchorStyles.None
        };

        ProgressBar progressBar = new()
        {
            Dock = DockStyle.Fill,
            Height = 23,
            Visible = false,
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 1
        };

        panel.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.Controls.AddRange([button, checkBox, progressBar]);

        button.Click += async (_, _) =>
        {
            progressBar.Visible = !(button.Enabled = checkBox.Enabled = false);
           
            if (_.Entries.Contains(await Game.VersionAsync()))
            {
                await Game.TerminateAsync();
                await Client.DownloadAsync(checkBox.Checked, (_) =>
                {
                    if (progressBar.Value != _)
                    {
                        if (progressBar.Style is ProgressBarStyle.Marquee) progressBar.Style = ProgressBarStyle.Blocks;
                        progressBar.Value = _;
                    }
                });

                button.Text = "Waiting...";
                progressBar.Value = 0;
                progressBar.Style = ProgressBarStyle.Marquee;
                await Client.ActivateAsync(checkBox.Checked);
            }

            progressBar.Visible = !(button.Enabled = checkBox.Enabled = true);
            button.Text = "Launch";
        };
    }
}
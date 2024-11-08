using System;
using System.Drawing;
using System.Windows.Forms;
using Launcher;

sealed partial class VersionsPage : TabPage
{
    internal VersionsPage()
    {
        Text = "Versions";

        TableLayoutPanel tableLayoutPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.White,
            Enabled = false,
        };
        Controls.Add(tableLayoutPanel);

        FlowLayoutPanel flowLayoutPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false
        };
        tableLayoutPanel.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        tableLayoutPanel.Controls.Add(flowLayoutPanel, 0, 0);

        Button button = new() { Text = "Install", BackColor = Color.Transparent, Dock = DockStyle.Fill };
        tableLayoutPanel.Controls.Add(button, 0, 1);

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
        tableLayoutPanel.Controls.Add(progressBar, 0, -1);

        button.Click += async (_, _) =>
        {
            progressBar.Visible = !(flowLayoutPanel.Enabled = button.Visible = false);
            for (var index = 0; index < flowLayoutPanel.Controls.Count; index++)
            {
                var radioButton = (RadioButton)flowLayoutPanel.Controls[index];
                if (radioButton.Checked)
                {
                    await ((ValueTuple<string, string>)radioButton.DataContext).InstallAsync((_) =>
                    {
                        if (progressBar.Value != _)
                        {
                            if (progressBar.Style is ProgressBarStyle.Marquee)
                                progressBar.Style = ProgressBarStyle.Blocks;
                            progressBar.Value = _;
                        }
                    });
                }
            }
            progressBar.Visible = !(flowLayoutPanel.Enabled = button.Visible = true);
        };

        VisibleChanged += async (sender, _) =>
        {
            if (!tableLayoutPanel.Enabled)
            {
                foreach (var item in await Versions.GetAsync())
                    flowLayoutPanel.Controls.Add(new RadioButton() { Text = item.Name, AutoSize = true, AutoCheck = true, DataContext = item });
                ((RadioButton)flowLayoutPanel.Controls[0]).Checked = true;
                tableLayoutPanel.Enabled = true;
            }
        };
    }
}
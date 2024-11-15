using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Flarial.Launcher;

sealed class Versions : TabPage
{
    internal Versions(Form _)
    {
        Text = "Versions";
        TableLayoutPanel panel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.White,
            Enabled = false
        };
        Controls.Add(panel);

        ListBox listBox = new() { Dock = DockStyle.Fill };

        Button button1 = new()
        {
            Text = "Install",
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Height = 23
        };

        Button button2 = new()
        {
            Text = "Cancel",
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Height = 23,
            Visible = false
        };

        ProgressBar progressBar = new()
        {
            Dock = DockStyle.Fill,
            Height = 23,
            Visible = false,
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 1
        };

        VersionEntries entries = default;

        CancellationTokenSource source = default;

        _.FormClosed += (_, _) => { try { source.Cancel(); } catch (Exception _) when (_ is ObjectDisposedException or NullReferenceException) { } };

        _.Shown += async (_, _) =>
        {
            foreach (var item in (entries = await VersionManager.GetAsync()).Reverse()) listBox.Items.Add(item);
            listBox.SelectedIndex = 0;
            panel.Enabled = true;
        };

        button1.Click += async (_, _) =>
        {
            using (source = new())
            {
                button2.Visible = progressBar.Visible = !(button1.Visible = listBox.Enabled = false);
                await entries[(string)listBox.SelectedItem].InstallAsync((_) =>
                {
                    if (progressBar.Value != _)
                    {
                        if (progressBar.Style is ProgressBarStyle.Marquee) progressBar.Style = ProgressBarStyle.Blocks;
                        progressBar.Value = _;
                    }
                }, source.Token);
                button2.Visible = progressBar.Visible = !(button2.Enabled = button1.Visible = listBox.Enabled = true);
                progressBar.Value = 0;
                progressBar.Style = ProgressBarStyle.Marquee;
            }
        };

        button2.Click += async (async_, _) =>
        {
            button2.Enabled = false;
            await Task.Run(() => source.Cancel());
        };

        panel.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.Controls.AddRange([listBox, button1, button2, progressBar]);
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Flarial.Launcher;

sealed class Versions : UserControl
{
    internal Versions(Form _)
    {
        Text = "Versions";
        Dock = DockStyle.Fill;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Margin = default;

        TableLayoutPanel panel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Enabled = false,
            Margin = default
        };
        Controls.Add(panel);

        ListBox listBox = new()
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            Margin = default
        };

        Button button1 = new()
        {
            Text = "Install",
            Dock = DockStyle.Fill,
            Margin = default
        };

        ProgressBar progressBar = new()
        {
            Dock = DockStyle.Fill,
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 1,
            Margin = default
        };

        Button button2 = new()
        {
            Text = "Cancel",
            Dock = DockStyle.Fill,
            Margin = default
        };

        TableLayoutPanel tableLayoutPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Visible = default,
            Enabled = default,
            Margin = default
        };

        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel.Controls.Add(progressBar, 0, 0);
        tableLayoutPanel.Controls.Add(button2, 1, 0);

        Request request = default;

        Application.ThreadExit += (_, _) => { request?.Cancel(); };

        listBox.VisibleChanged += async (_, _) =>
        {
            if (!panel.Enabled)
            {
                await Task.Run(() => { foreach (var item in _.Catalog.Reverse()) listBox.Items.Add(item); });
                listBox.SelectedIndex = default;
                panel.Enabled = true;
            }
        };

        button1.Click += async (_, _) =>
        {
            if (!Minecraft.Installed) return;

            SuspendLayout();
            tableLayoutPanel.Visible = true;
            button1.Visible = listBox.Enabled = default;
            ResumeLayout();

            request = await _.Catalog.InstallAsync((string)listBox.SelectedItem, (_) =>
            {
                if (progressBar.Value != _)
                {
                    if (progressBar.Style is ProgressBarStyle.Marquee) progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Value = _;
                }
            });

            tableLayoutPanel.Enabled = true;
            try { await request; } catch (OperationCanceledException) { }

            SuspendLayout();
            tableLayoutPanel.Visible = tableLayoutPanel.Enabled = default;
            button1.Visible = listBox.Enabled = true;

            progressBar.Value = default;
            progressBar.Style = ProgressBarStyle.Marquee;
            ResumeLayout();
        };

        button2.Click += async (_, _) =>
        {
            tableLayoutPanel.Enabled = default;
            await request.CancelAsync();
        };

        panel.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.Controls.Add(listBox, 0, 0);
        panel.Controls.Add(button1, 0, 1);
        panel.Controls.Add(tableLayoutPanel, 0, 2);
    }
}
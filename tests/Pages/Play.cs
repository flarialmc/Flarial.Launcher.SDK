using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Flarial.Launcher.SDK;

sealed class Play : UserControl
{
    internal Play(Form _)
    {
        Text = "Play";
        Dock = DockStyle.Fill;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Margin = default;

        TableLayoutPanel panel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = default
        };
        Controls.Add(panel);

        Button button = new()
        {
            Text = "Launch",
            Dock = DockStyle.Fill,
            Anchor = AnchorStyles.None,
            Margin = default
        };

        CheckBox checkBox = new()
        {
            Text = "Beta",
            AutoSize = true,
            Anchor = AnchorStyles.None,
            Margin = default
        };

        ProgressBar progressBar = new()
        {
            Dock = DockStyle.Fill,
            Visible = default,
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 1,
            Margin = default
        };

        panel.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        panel.Controls.Add(button, 0, 0);
        panel.Controls.Add(checkBox, 0, 1);
        panel.Controls.Add(progressBar, 0, 1);

        button.Click += async (_, _) =>
        {
            try
            {

                SuspendLayout();
                progressBar.Visible = true;
                button.Enabled = checkBox.Visible = default;
                ResumeLayout();

                if (!await Licensing.CheckAsync()) throw new LicenseException(typeof(object));
                if (!checkBox.Checked && !await _.Catalog.CompatibleAsync()) return;

                await Client.DownloadAsync(checkBox.Checked, (_) => Invoke(() =>
                {
                    if (progressBar.Value != _)
                    {
                        if (progressBar.Style is ProgressBarStyle.Marquee) progressBar.Style = ProgressBarStyle.Blocks;
                        progressBar.Value = _;
                    }
                }));

                SuspendLayout();
                progressBar.Value = 0;
                progressBar.Style = ProgressBarStyle.Marquee;
                ResumeLayout();

                await Client.LaunchAsync(checkBox.Checked);
            }
            finally
            {
                SuspendLayout();
                progressBar.Visible = default;
                button.Enabled = checkBox.Visible = true;
                button.Text = "Launch";
                ResumeLayout();
            }

        };
    }
}
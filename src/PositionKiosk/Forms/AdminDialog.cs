using System.Drawing;
using System.Windows.Forms;
using PositionKiosk.Core;

namespace PositionKiosk.Forms;

public enum AdminAction
{
    None,
    Exit,
    Reload,
    DevTools,
    Unlock,
}

public sealed class AdminDialog : Form
{
    private readonly KioskConfig _config;
    private readonly Action<string> _logFailure;

    private readonly TextBox _passwordBox;
    private readonly Button _verifyButton;
    private readonly Button _exitButton;
    private readonly Button _reloadButton;
    private readonly Button _devToolsButton;
    private readonly Button _unlockButton;

    public AdminAction Result { get; private set; } = AdminAction.None;

    public AdminDialog(KioskConfig config, Action<string> logFailure)
    {
        _config = config;
        _logFailure = logFailure;

        Text = "Position Kiosk 管理员";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(360, 280);

        var lbl = new Label
        {
            Text = "管理员密码：",
            Location = new Point(20, 25),
            AutoSize = true,
        };

        _passwordBox = new TextBox
        {
            Location = new Point(20, 50),
            Size = new Size(320, 25),
            PasswordChar = '*',
        };
        _passwordBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { VerifyPassword(); e.SuppressKeyPress = true; }
        };

        _verifyButton = new Button { Text = "验证", Location = new Point(250, 85), Size = new Size(90, 30) };
        _verifyButton.Click += (_, _) => VerifyPassword();

        _exitButton = MakeActionButton("退出程序", 20, AdminAction.Exit);
        _reloadButton = MakeActionButton("重载页面", 110, AdminAction.Reload);
        _devToolsButton = MakeActionButton("打开 DevTools", 200, AdminAction.DevTools);
        _unlockButton = MakeActionButton("解除锁定", 20, AdminAction.Unlock);

        var unlockLabel = new Label
        {
            Text = "（解除锁定：临时变为可移动的普通窗口，便于维护）",
            Location = new Point(20, 195),
            Size = new Size(320, 60),
            ForeColor = Color.Gray,
        };

        Controls.AddRange(new Control[]
        {
            lbl, _passwordBox, _verifyButton,
            _exitButton, _reloadButton, _devToolsButton, _unlockButton, unlockLabel,
        });

        SetActionButtonsEnabled(false);
        AcceptButton = _verifyButton;
    }

    private Button MakeActionButton(string text, int x, AdminAction action)
    {
        var btn = new Button
        {
            Text = text,
            Location = new Point(x, 150),
            Size = new Size(110, 35),
            Enabled = false,
        };
        btn.Click += (_, _) =>
        {
            Result = action;
            DialogResult = DialogResult.OK;
            Close();
        };
        return btn;
    }

    private void SetActionButtonsEnabled(bool enabled)
    {
        _exitButton.Enabled = enabled;
        _reloadButton.Enabled = enabled;
        _devToolsButton.Enabled = enabled;
        _unlockButton.Enabled = enabled;
    }

    private void VerifyPassword()
    {
        if (PasswordHasher.Verify(_passwordBox.Text, _config.AdminPasswordHash, _config.PasswordSalt))
        {
            SetActionButtonsEnabled(true);
            _passwordBox.Enabled = false;
            _verifyButton.Enabled = false;
            _ = MessageBox.Show(this, "验证通过，请选择操作。", "Position Kiosk",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            _logFailure("admin auth failed");
            MessageBox.Show(this, "密码错误", "Position Kiosk",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _passwordBox.Clear();
        }
    }
}

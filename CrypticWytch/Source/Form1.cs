using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace CrypticWytch
{
    public partial class Form1 : Form
    {
        private Panel? _keySetupPanel;
        private Panel? _cryptoPanel;
        private byte[]? _sessionKey;
        private TextBox? _txtInput;

        public Form1()
        {
            InitializeComponent();
            SetupCustomWindow();
            CreatePanels();
        }

        private void SetupCustomWindow()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new System.Drawing.Size(550, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = new Icon("Witch.ico");

            Panel titleBar = new Panel
            {
                Height = 35,
                Dock = DockStyle.Top,
                BackColor = System.Drawing.Color.FromArgb(40, 40, 40)
            };

            Label titleLabel = new Label
            {
                Text = "Cryptic Wytch",
                ForeColor = System.Drawing.Color.Lime,
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(10, 8),
                AutoSize = true
            };

            Button minimizeButton = new Button
            {
                Text = "—",
                Size = new System.Drawing.Size(35, 35),
                Location = new System.Drawing.Point(this.Width - 70, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = System.Drawing.Color.Lime,
                BackColor = System.Drawing.Color.FromArgb(40, 40, 40)
            };
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            Button closeButton = new Button
            {
                Text = "✕",
                Size = new System.Drawing.Size(35, 35),
                Location = new System.Drawing.Point(this.Width - 35, 0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = System.Drawing.Color.Lime,
                BackColor = System.Drawing.Color.FromArgb(40, 40, 40)
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => Application.Exit();

            titleBar.Controls.Add(titleLabel);
            titleBar.Controls.Add(minimizeButton);
            titleBar.Controls.Add(closeButton);
            this.Controls.Add(titleBar);
            titleBar.MouseDown += TitleBar_MouseDown;
        }

        private void CreatePanels()
        {
            _keySetupPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Black,
                BackgroundImage = Properties.Resources.background,
                BackgroundImageLayout = ImageLayout.Stretch
            };

            Label lblInstruction = new Label
            {
                Text = "🔐 Установите сеансовый ключ",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(100, 70),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Lime,
                BackColor = System.Drawing.Color.Transparent
            };

            Button btnGenCopy = new Button
            {
                Text = "🔑 Gen && Copy",
                Size = new System.Drawing.Size(230, 70),
                Location = new System.Drawing.Point(30, 115),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            btnGenCopy.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            btnGenCopy.FlatAppearance.BorderSize = 1;
            btnGenCopy.Click += BtnGenCopy_Click;

            Button btnImport = new Button

            {
                Text = "📋 Import Key",
                Size = new System.Drawing.Size(230, 70),
                Location = new System.Drawing.Point(270, 115),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            btnImport.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            btnImport.FlatAppearance.BorderSize = 1;
            btnImport.Click += BtnImport_Click;

            _keySetupPanel.Controls.Add(lblInstruction);
            _keySetupPanel.Controls.Add(btnGenCopy);
            _keySetupPanel.Controls.Add(btnImport);

            _cryptoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Black,
                BackgroundImage = Properties.Resources.background,
                BackgroundImageLayout = ImageLayout.Stretch,
                Visible = false
            };
            Button btnHelp = new Button
            {
                Text = "❓ Help",
                Size = new System.Drawing.Size(100, 40),
                Location = new System.Drawing.Point(430, 350),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            btnHelp.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            btnHelp.FlatAppearance.BorderSize = 1;
            btnHelp.Click += (s, e) =>
            {
                string readmePath = Path.Combine(Application.StartupPath, "README.txt");
                if (File.Exists(readmePath))
                    System.Diagnostics.Process.Start("notepad.exe", readmePath);
                else
                    MessageBox.Show("Файл README.txt не найден в папке с программой.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            _keySetupPanel.Controls.Add(btnHelp);

            Label lblActive = new Label
            {
                Text = "🔑 Session key active",
                ForeColor = System.Drawing.Color.Lime,
                BackColor = System.Drawing.Color.Transparent,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Location = new System.Drawing.Point(420, 380)
            };

            _txtInput = new TextBox
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(500, 180),
                Multiline = true,
                Font = new System.Drawing.Font("Consolas", 10),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime,
                BorderStyle = BorderStyle.FixedSingle,
            };
            _txtInput.MaxLength = 5000;

            Button btnEncryptCopy = new Button
            {
                Text = "🔒 Encrypt && Copy",
                Size = new System.Drawing.Size(220, 60),
                Location = new System.Drawing.Point(20, 240),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            btnEncryptCopy.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            btnEncryptCopy.FlatAppearance.BorderSize = 1;
            btnEncryptCopy.Click += BtnEncryptCopy_Click;

            Button btnPasteDecrypt = new Button
            {
                Text = "🔑 Paste && Decrypt",
                Size = new System.Drawing.Size(220, 60),
                Location = new System.Drawing.Point(300, 240),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            btnPasteDecrypt.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            btnPasteDecrypt.FlatAppearance.BorderSize = 1;
            btnPasteDecrypt.Click += BtnPasteDecrypt_Click;

            Button btnResetKey = new Button
            {
                Text = "🔄 Reset Session Key",
                Size = new System.Drawing.Size(220, 60),
                Location = new System.Drawing.Point(20, 330),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            btnResetKey.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            btnResetKey.FlatAppearance.BorderSize = 1;
            btnResetKey.Click += BtnResetKey_Click;

            _cryptoPanel.Controls.Add(_txtInput);
            _cryptoPanel.Controls.Add(btnEncryptCopy);
            _cryptoPanel.Controls.Add(btnPasteDecrypt);
            _cryptoPanel.Controls.Add(btnResetKey);
            _cryptoPanel.Controls.Add(lblActive);

            this.Controls.Add(_keySetupPanel);
            this.Controls.Add(_cryptoPanel);
        }

                private void CopyToClipboard(string text)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetText(text);
                    return;
                }
                catch (ExternalException)
                {
                    System.Threading.Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            MessageBox.Show("Не удалось скопировать текст в буфер обмена.\nВозможно, другая программа блокирует буфер.",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private byte[] GenerateKey()
        {
            byte[] key = new byte[32];
            RandomNumberGenerator.Fill(key);
            return key;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private void TitleBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void SwitchToCryptoMode()
        {
            if (_keySetupPanel != null) _keySetupPanel.Visible = false;
            if (_cryptoPanel != null) _cryptoPanel.Visible = true;
        }

        private void SwitchToKeySetupMode()
        {
            if (_keySetupPanel != null) _keySetupPanel.Visible = true;
            if (_cryptoPanel != null) _cryptoPanel.Visible = false;
            if (_txtInput != null) _txtInput.Text = "";
        }

        private void BtnGenCopy_Click(object? sender, EventArgs e)
        {
            _sessionKey = GenerateKey();
            string base64Key = Convert.ToBase64String(_sessionKey);
            CopyToClipboard(base64Key);
            MessageBox.Show($"Ключ сгенерирован и скопирован в буфер!\n\nBase64: {base64Key}", "Ключ создан", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SwitchToCryptoMode();
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            string? clipboardText = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                MessageBox.Show("Буфер обмена пуст!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clipboardText.Length != 44)
            {
                MessageBox.Show("Неверный формат ключа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                byte[] key = Convert.FromBase64String(clipboardText);
                if (key.Length != 32)
                {
                    MessageBox.Show("Ключ должен быть 32 байта (256 бит).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _sessionKey = key;
                MessageBox.Show("Ключ успешно импортирован", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SwitchToCryptoMode();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат Base64 в буфере обмена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnEncryptCopy_Click(object? sender, EventArgs e)
        {
            if (_sessionKey == null)
            {
                MessageBox.Show("Ключ не установлен", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SwitchToKeySetupMode();
                return;
            }

            if (_txtInput == null || string.IsNullOrWhiteSpace(_txtInput.Text))
            {
                MessageBox.Show("Нет текста для шифрования", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string encrypted = AesGcmHelper.Encrypt(_txtInput.Text, _sessionKey);
                CopyToClipboard(encrypted);
                MessageBox.Show("Текст зашифрован и скопирован в буфер", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка шифрования: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPasteDecrypt_Click(object? sender, EventArgs e)
        {
            if (_sessionKey == null)
            {
                MessageBox.Show("Ключ не установлен", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SwitchToKeySetupMode();
                return;
            }

            string? clipboardText = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                MessageBox.Show("Буфер обмена пуст", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string decrypted = AesGcmHelper.Decrypt(clipboardText, _sessionKey);
                _txtInput!.Text = decrypted;
                MessageBox.Show("Текст успешно расшифрован", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат данных в буфере. Возможно, это не зашифрованный текст.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Ошибка расшифровки. Возможно, неверный ключ или данные повреждены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расшифровки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnResetKey_Click(object? sender, EventArgs e)
        {
            _sessionKey = null;
            if (_txtInput != null) _txtInput.Text = "";
            SwitchToKeySetupMode();
            MessageBox.Show("Ключ сеанса сброшен.", "Сброс", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
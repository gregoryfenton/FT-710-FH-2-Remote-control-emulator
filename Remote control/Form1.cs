using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Remote_control
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer? freqPollTimer;
        private DateTime lastFrequencyUpdate = DateTime.MinValue;
        
        private SerialPort? serialPort;
        private bool isConnected = false;
        private string aiStatusInitial = "0";
        private bool aiWasEnabled = false;
        private bool isBKINEnabled = false;

        private string? lastSentCommand;
        private readonly System.Windows.Forms.Timer riPollTimer = new System.Windows.Forms.Timer();
        private int leftRightStep = 1000;
        private int upDownMultiplier = 5;

        private string iniFilePath = Path.Combine(Application.StartupPath, "settings.ini");

        // Stores custom CAT commands for buttons 1-5
        private string[] mainButtonCatCommands = new string[5] { "1;", "2;", "3;", "4;", "5;" };

        private ToolTip tooltip = new ToolTip();

        public Form1()
        {
            InitializeComponent();

            // Wire button click events
            ButtonMain1.Click += MainButton_Click;
            ButtonMain2.Click += MainButton_Click;
            ButtonMain3.Click += MainButton_Click;
            ButtonMain4.Click += MainButton_Click;
            ButtonMain5.Click += MainButton_Click;
            ButtonMainMem.Click += MemButton_Click;
            ButtonMainPB.Click += PBButton_Click;
            ButtonMainDec.Click += ButtonMainDec_Click;

            ButtonMain1.MouseDown += MainButton_MouseDown;
            ButtonMain2.MouseDown += MainButton_MouseDown;
            ButtonMain3.MouseDown += MainButton_MouseDown;
            ButtonMain4.MouseDown += MainButton_MouseDown;
            ButtonMain5.MouseDown += MainButton_MouseDown;

            ButtonMainLeft.Click += LeftRightButton_RightClick_Or_Click;
            ButtonMainRight.Click += LeftRightButton_RightClick_Or_Click;
            ButtonMainUp.Click += UpDownButton_RightClick_Or_Click;
            ButtonMainDown.Click += UpDownButton_RightClick_Or_Click;

            ButtonFreq0.Click += FreqButton_Click;
            ButtonFreq1.Click += FreqButton_Click;
            ButtonFreq2.Click += FreqButton_Click;
            ButtonFreq3.Click += FreqButton_Click;
            ButtonFreq4.Click += FreqButton_Click;
            ButtonFreq5.Click += FreqButton_Click;
            ButtonFreq6.Click += FreqButton_Click;
            ButtonFreq7.Click += FreqButton_Click;
            ButtonFreq8.Click += FreqButton_Click;
            ButtonFreq9.Click += FreqButton_Click;

            ButtonFreqClear.Click += (s, e) => FrequencyChangeTextBox.Clear();
            ButtonFreqSet.Click += SetFrequency_Click;

            ModeComboBox.SelectedIndexChanged += ModeComboBox_SelectedIndexChanged;

            ComPortButton.Click += ComPortButton_Click;

            FrequencyChangeTextBox.KeyDown += FrequencyChangeTextBox_KeyDown;

            // Initialize status labels and texts
            Status1Name.Text = "SWR";
            Status2Name.Text = "Msg";
            Status3Name.Text = "TxMode";
            Status4Name.Text = "Tuner";
            Status5Name.Text = "Scan";
            Status6Name.Text = "Sql";

            SetAllStatusTexts("---");

            // DEC button is Bkin button
            ButtonMainDec.Text = "Bkin";

            riPollTimer.Interval = 300; // 300 ms
            riPollTimer.Tick += (s, e) => SendCommand("RI0;BI;");

            // Set tooltips on static controls
            InitializeTooltips();

            LoadSettings();

            UpdateMainButtonTooltips();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateComPorts();
            PopulateModeComboBox();
            FrequencyTextBox.ReadOnly = true;
            FrequencyTextBox.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            freqPollTimer = new System.Windows.Forms.Timer();
            freqPollTimer.Interval = 1000; // 1 second
            freqPollTimer.Tick += FreqPollTimer_Tick;

            // Default ComSpeed to 38400 if available, else first item
            if (ComSpeedComboBox.Items.Count > 0)
            {
                int index = ComSpeedComboBox.Items.IndexOf("38400");
                ComSpeedComboBox.SelectedIndex = index >= 0 ? index : 0;
            }

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // If focus is inside FrequencyChangeTextBox, ignore arrow keys so cursor works normally
            if (FrequencyChangeTextBox.Focused)
                return;

            if (e.KeyCode == Keys.Left)
            {
                e.Handled = true;
                ChangeFrequency(-leftRightStep);
                ResetPBMemState();
            }
            else if (e.KeyCode == Keys.Right)
            {
                e.Handled = true;
                ChangeFrequency(leftRightStep);
                ResetPBMemState();
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                ChangeFrequency(leftRightStep * upDownMultiplier);
                ResetPBMemState();
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                ChangeFrequency(-leftRightStep * upDownMultiplier);
                ResetPBMemState();
            }
        }

        private void FreqPollTimer_Tick(object? sender, EventArgs e)
        {
            if (isConnected && (DateTime.Now - lastFrequencyUpdate).TotalMilliseconds > 1000)
            {
                SendCommand("IF;");
            }
        }

        private void PopulateComPorts()
        {
            ComPortComboBox.Items.Clear();

            // Get ports, sort by numeric part, e.g. COM1 < COM2 < COM10
            var ports = SerialPort.GetPortNames()
                .Where(p => p.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p =>
                {
                    if (int.TryParse(p.Substring(3), out int num)) return num;
                    return int.MaxValue;
                })
                .ToArray();

            ComPortComboBox.Items.AddRange(ports);

            // Select highest port if not already selected from settings
            if (ComPortComboBox.SelectedItem == null && ports.Length > 0)
                ComPortComboBox.SelectedItem = ports.Last();
        }

        private void PopulateModeComboBox()
        {
            ModeComboBox.Items.Clear();
            ModeComboBox.Items.AddRange(new object[]
            {
                "NONE", "CW-L", "CW-U", "LSB", "USB", "FM-N", "AM-N", "FM", "AM",
                "DATA-L", "DATA-U", "DATA-FM-N", "DATA-FM", "RTTY-U", "PSK"
            });
            ModeComboBox.SelectedIndex = 4; // USB default
        }

        private void ComPortButton_Click(object? sender, EventArgs e)
        {
            if (!isConnected)
            {
                if (ComPortComboBox.SelectedItem == null || ComSpeedComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a COM port and speed.");
                    return;
                }
                
                try
                {
                    serialPort = new SerialPort(
                        ComPortComboBox.SelectedItem.ToString()!,
                        int.Parse(ComSpeedComboBox.SelectedItem.ToString()!)
                    );
                    serialPort.DataReceived += SerialPort_DataReceived;
                    serialPort.Open();

                    isConnected = true;
                    ComPortButton.Text = "Discon";
                    SetAllStatusTexts("WAIT");

                    SaveSettings();                    
                    SendCommand("AI;");

                    riPollTimer.Start();
                    lastFrequencyUpdate = DateTime.Now;
                    lastFrequency = "999999999";
                    freqPollTimer!.Start();
                }
                catch (Exception ex)
                {                    
                    isConnected = false;
                    ComPortButton.Text = "Connect";
                    FrequencyTextBox.Text = string.Empty;
                    SetAllStatusTexts("---");
                    MessageBox.Show("Error opening serial port: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    if (serialPort != null && serialPort.IsOpen)
                    {
                        riPollTimer.Stop();
                        freqPollTimer!.Stop();
                        serialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error closing serial port: " + ex.Message);
                }

                isConnected = false;
                ComPortButton.Text = "Connect";
                SetAllStatusTexts("---");
            }
        }

        private void SerialPort_DataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null) return;

            try
            {
                string data = serialPort.ReadExisting();

                // Radio responses may be concatenated separated by ';'
                var messages = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var msg in messages)
                {
                    BeginInvoke(new Action(() => HandleRadioResponse(msg + ";")));
                }
            }
            catch { }
        }

        private void HandleRadioResponse(string data)
        {
            if (data == "?;")
            {
                if (!string.IsNullOrEmpty(lastSentCommand))
                    MessageBox.Show($"Unrecognized CAT command: {lastSentCommand}");
                return;
            }

            if (data.StartsWith("AI"))
            {
                aiStatusInitial = data.Length > 2 ? data.Substring(2, 1) : "0";
                aiWasEnabled = (aiStatusInitial == "1");
                if (!aiWasEnabled)
                {
                    SendCommand("AI1;");
                    aiWasEnabled = true;
                }
            }
            else if (data.StartsWith("IF"))
            {
                if (data.Length >= 14)
                {
                    string freqStr = data.Substring(5, 9);
                    if (long.TryParse(freqStr, out long freq))
                    {
                        FrequencyTextBox.Text = freq.ToString();
                        lastFrequencyUpdate = DateTime.Now;
                        HighlightTextBoxBriefly();
                    }
                }
            }
            else if (data.StartsWith("MD"))
            {
                if (data.Length >= 4)
                {
                    string modeCode = data.Substring(3, 1);
                    ModeComboBox.SelectedIndex = ModeCodeToIndex(modeCode);
                }
            }
            else if (data.StartsWith("RI"))
            {
                ParseRiStatus(data);
            }
            else if (data.StartsWith("BI"))
            {
                if (data.Length >= 3)
                {
                    isBKINEnabled = data.Substring(2, 1) == "1";
                    UpdateDecButton();
                }
            }
        }
        private void UpdateDecButton()
        {
            ButtonMainDec.Text = "BkIn";
            ButtonMainDec.BackColor = isBKINEnabled ? Color.LightGreen : SystemColors.Control;
        }


        private bool ChangingTextBoxColor = false;
        private String lastFrequency = "";
        private async void HighlightTextBoxBriefly()
        {
            if (!ChangingTextBoxColor)
            {
                ChangingTextBoxColor = true;
                if (lastFrequency != FrequencyTextBox.Text)
                {
                    Color original = FrequencyTextBox.BackColor;
                    FrequencyTextBox.BackColor = Color.Green;
                    await Task.Delay(500); // Wait 500 milliseconds
                    FrequencyTextBox.BackColor = original;
                    lastFrequency = FrequencyTextBox.Text;
                }
                ChangingTextBoxColor = false;
            }
        }

        private void ParseRiStatus(string data)
        {
            if (data.Length < 11) return;

            char p2 = data[3];
            char p3 = data[4];
            char p4 = data[5];
            char p5 = data[6];
            char p6 = data[7];
            char p7 = data[8];
            char p8 = data[9];

            Status1Text.Text = p2 switch
            {
                '0' => "Normal",
                '1' => "HI-SWR",
                _ => "---"
            };
            Status2Text.Text = p3 switch
            {
                '0' => "Stop",
                '1' => "Rec",
                '2' => "P/B",
                _ => "---"
            };
            Status3Text.Text = p4 switch
            {
                '0' => "Rx",
                '1' => "Tx",
                '2' => "Tx-INH",
                _ => "---"
            };
            Status4Text.Text = p6 switch
            {
                '0' => "Stop",
                '1' => "Tuning",
                _ => "---"
            };
            Status5Text.Text = p7 switch
            {
                '0' => "Stop",
                '1' => "Active",
                '2' => "Paused",
                _ => "---"
            };
            Status6Text.Text = p8 switch
            {
                '0' => "Closed",
                '1' => "Busy",
                _ => "---"
            };
        }

        private int ModeCodeToIndex(string code) => code switch
        {
            "0" => 0,   // NONE
            "7" => 1,   // CW-L
            "3" => 2,   // CW-U
            "1" => 3,   // LSB
            "2" => 4,   // USB
            "6" => 5,   // FM-N
            "D" => 6,   // AM-N
            "4" => 7,   // FM
            "5" => 8,   // AM
            "8" => 9,   // DATA-L
            "C" => 10,  // DATA-U
            "F" => 11,  // DATA-FM-N
            "A" => 12,  // DATA-FM
            "9" => 13,  // RTTY-U
            "E" => 14,  // PSK
            _ => 0
        };

        private string IndexToModeCode(int index) => index switch
        {
            0 => "0",
            1 => "7",
            2 => "3",
            3 => "1",
            4 => "2",
            5 => "6",
            6 => "D",
            7 => "4",
            8 => "5",
            9 => "8",
            10 => "C",
            11 => "F",
            12 => "A",
            13 => "9",
            14 => "E",
            _ => "0"
        };

        private void ModeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isConnected && serialPort?.IsOpen == true)
            {
                string modeCode = IndexToModeCode(ModeComboBox.SelectedIndex);
                SendCommand($"MD0{modeCode};");
            }
        }

        private void MainButton_Click(object? sender, EventArgs e)
        {
            if (!isConnected || serialPort == null || !serialPort.IsOpen) return;

            if (sender is Button button)
            {
                int btnIndex = -1;
                if (button == ButtonMain1) btnIndex = 0;
                else if (button == ButtonMain2) btnIndex = 1;
                else if (button == ButtonMain3) btnIndex = 2;
                else if (button == ButtonMain4) btnIndex = 3;
                else if (button == ButtonMain5) btnIndex = 4;

                if (btnIndex == -1) return;

                // If P/B active
                if (ButtonMainPB.BackColor == System.Drawing.Color.LightGreen)
                {
                    SendCommand($"PB0{btnIndex + 1};");
                    ResetPBMemState();
                }
                // If MEM active
                else if (ButtonMainMem.BackColor == System.Drawing.Color.LightGreen)
                {
                    SendCommand($"LM0{btnIndex + 1};");
                    ResetPBMemState();
                }
                else
                {
                    SendCommand(mainButtonCatCommands[btnIndex]);
                    ResetPBMemState();
                }
            }
        }

        private void MemButton_Click(object? sender, EventArgs e)
        {
            if (ButtonMainMem.BackColor == System.Drawing.Color.LightGreen)
            {
                ButtonMainMem.BackColor = System.Drawing.SystemColors.Control;
            }
            else
            {
                ButtonMainMem.BackColor = System.Drawing.Color.LightGreen;
                ButtonMainPB.BackColor = System.Drawing.SystemColors.Control;
                StartPBMemTimeout();
            }
        }

        private void PBButton_Click(object? sender, EventArgs e)
        {
            if (ButtonMainPB.BackColor == System.Drawing.Color.LightGreen)
            {
                ButtonMainPB.BackColor = System.Drawing.SystemColors.Control;
            }
            else
            {
                ButtonMainPB.BackColor = System.Drawing.Color.LightGreen;
                ButtonMainMem.BackColor = System.Drawing.SystemColors.Control;
                StartPBMemTimeout();
            }
        }

        private System.Windows.Forms.Timer? pbMemTimeoutTimer = null;

        private void StartPBMemTimeout()
        {
            if (pbMemTimeoutTimer == null)
            {
                pbMemTimeoutTimer = new System.Windows.Forms.Timer();
                pbMemTimeoutTimer.Interval = 10000; // 10 seconds
                pbMemTimeoutTimer.Tick += (s, e) =>
                {
                    ResetPBMemState();
                    pbMemTimeoutTimer?.Stop();
                };
            }
            pbMemTimeoutTimer.Stop();
            pbMemTimeoutTimer.Start();
        }

        private void ResetPBMemState()
        {
            ButtonMainPB.BackColor = System.Drawing.SystemColors.Control;
            ButtonMainMem.BackColor = System.Drawing.SystemColors.Control;
            pbMemTimeoutTimer?.Stop();
        }

        private void LeftRightButton_RightClick_Or_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn == ButtonMainLeft)
                {
                    if ((e as MouseEventArgs)?.Button == MouseButtons.Right)
                    {
                        ChangeLeftRightStep();
                        return;
                    }
                    ChangeFrequency(-leftRightStep);
                    ResetPBMemState();
                }
                else if (btn == ButtonMainRight)
                {
                    if ((e as MouseEventArgs)?.Button == MouseButtons.Right)
                    {
                        ChangeLeftRightStep();
                        return;
                    }
                    ChangeFrequency(leftRightStep);
                    ResetPBMemState();
                }
            }
        }

        private void UpDownButton_RightClick_Or_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                int step = leftRightStep * upDownMultiplier;

                if (btn == ButtonMainUp)
                {
                    if ((e as MouseEventArgs)?.Button == MouseButtons.Right)
                    {
                        ChangeUpDownMultiplier();
                        return;
                    }
                    ChangeFrequency(step);
                    ResetPBMemState();
                }
                else if (btn == ButtonMainDown)
                {
                    if ((e as MouseEventArgs)?.Button == MouseButtons.Right)
                    {
                        ChangeUpDownMultiplier();
                        return;
                    }
                    ChangeFrequency(-step);
                    ResetPBMemState();
                }
            }
        }

        private void ChangeLeftRightStep()
        {
            int[] options = { 1, 10, 100, 1000, 6250, 12500, 25000 };
            int currentIndex = Array.IndexOf(options, leftRightStep);
            int nextIndex = (currentIndex + 1) % options.Length;
            leftRightStep = options[nextIndex];
            MessageBox.Show($"Left/Right step set to {leftRightStep} Hz");
        }

        private void ChangeUpDownMultiplier()
        {
            int[] options = { 2, 3, 4, 5, 10 };
            int currentIndex = Array.IndexOf(options, upDownMultiplier);
            int nextIndex = (currentIndex + 1) % options.Length;
            upDownMultiplier = options[nextIndex];
            MessageBox.Show($"Up/Down multiplier set to {upDownMultiplier}x");
        }

        private void ChangeFrequency(int deltaHz)
        {
            if (!long.TryParse(FrequencyTextBox.Text, out long currentFreq)) return;
            long newFreq = currentFreq + deltaHz;
            if (newFreq >= 30000 && newFreq <= 75000000 && isConnected && serialPort != null && serialPort.IsOpen)
            {
                SendCommand($"FA{newFreq:D9};");
            }
        }

        private void FreqButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                FrequencyChangeTextBox.AppendText(btn.Text);
            }
        }

        private void SetFrequency_Click(object? sender, EventArgs e)
        {
            if (long.TryParse(FrequencyChangeTextBox.Text, out long freq))
            {
                if (freq >= 30000 && freq <= 75000000 && isConnected && serialPort != null && serialPort.IsOpen)
                {
                    SendCommand($"FA{freq:D9};");
                    FrequencyChangeTextBox.Clear();
                    ResetPBMemState();
                }
            }
        }

        private void ButtonMainDec_Click(object? sender, EventArgs e)
        {
            if (!isConnected || serialPort == null || !serialPort.IsOpen) return;

            isBKINEnabled = !isBKINEnabled;
            SendCommand($"BI{(isBKINEnabled ? "1" : "0")};");
            UpdateDecButton();
        }

        private void FrequencyChangeTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SetFrequency_Click(sender, EventArgs.Empty);
            }
        }

        private void MainButton_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && sender is Button btn)
            {
                int btnIndex = -1;
                if (btn == ButtonMain1) btnIndex = 0;
                else if (btn == ButtonMain2) btnIndex = 1;
                else if (btn == ButtonMain3) btnIndex = 2;
                else if (btn == ButtonMain4) btnIndex = 3;
                else if (btn == ButtonMain5) btnIndex = 4;

                if (btnIndex != -1)
                {
                    string currentCmd = mainButtonCatCommands[btnIndex];
                    string input = Prompt.ShowDialog($"Enter custom CAT command for Button {btnIndex + 1}:", "Edit CAT Command", currentCmd);
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        mainButtonCatCommands[btnIndex] = input.Trim();
                        UpdateMainButtonTooltips();
                        SaveSettings();
                    }
                }
            }
        }

        private void SendCommand(string command)
        {
            if (!isConnected || serialPort == null || !serialPort.IsOpen) return;

            try
            {
                lastSentCommand = command;
                serialPort.Write(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send command: {ex.Message}");
            }
        }

        private void SetAllStatusTexts(string text)
        {
            Status1Text.Text = text;
            Status2Text.Text = text;
            Status3Text.Text = text;
            Status4Text.Text = text;
            Status5Text.Text = text;
            Status6Text.Text = text;
        }

        private void InitializeTooltips()
        {
            tooltip.SetToolTip(ComPortComboBox, "Select the COM port of the radio.");
            tooltip.SetToolTip(ComSpeedComboBox, "Select the baud rate.");
            tooltip.SetToolTip(ComPortButton, "Connect or disconnect the serial port.");
            tooltip.SetToolTip(FrequencyTextBox, "Current frequency in Hz (read-only).");
            tooltip.SetToolTip(FrequencyChangeTextBox, "Enter frequency to set (30000 - 75000000 Hz). Press Enter or Set.");
            tooltip.SetToolTip(ButtonFreqClear, "Clear frequency input.");
            tooltip.SetToolTip(ButtonFreqSet, "Set frequency to entered value.");

            tooltip.SetToolTip(ButtonMainLeft, "Decrease frequency by step (right-click to change step).");
            tooltip.SetToolTip(ButtonMainRight, "Increase frequency by step (right-click to change step).");
            tooltip.SetToolTip(ButtonMainUp, "Increase frequency by step × multiplier (right-click to change multiplier).");
            tooltip.SetToolTip(ButtonMainDown, "Decrease frequency by step × multiplier (right-click to change multiplier).");

            tooltip.SetToolTip(ButtonMainPB, "Push/Back toggle button.\nActive: Press buttons 1-5 to playback the message in that slot.");
            tooltip.SetToolTip(ButtonMainMem, "Memory toggle button.\nActive: Press buttons 1-5 to record new message in that slot.");

            tooltip.SetToolTip(ModeComboBox, "Select radio mode.");

            // Add tooltips for buttons 1-5 with dynamic text
            UpdateMainButtonTooltips();
        }

        private void UpdateMainButtonTooltips()
        {
            string[] btnNames = { "ButtonMain1", "ButtonMain2", "ButtonMain3", "ButtonMain4", "ButtonMain5" };
            Button[] buttons = { ButtonMain1, ButtonMain2, ButtonMain3, ButtonMain4, ButtonMain5 };

            for (int i = 0; i < 5; i++)
            {
                string tip = $"Send CAT Command: {mainButtonCatCommands[i]}\nRight click to change the command.";
                tooltip.SetToolTip(buttons[i], tip);
            }
        }

        private void LoadSettings()
        {
            if (!File.Exists(iniFilePath)) return;

            try
            {
                var lines = File.ReadAllLines(iniFilePath);
                var dict = new Dictionary<string, string>();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (line.StartsWith("#")) continue;
                    int eqIdx = line.IndexOf('=');
                    if (eqIdx < 1) continue;
                    string key = line.Substring(0, eqIdx).Trim();
                    string value = line.Substring(eqIdx + 1).Trim();
                    dict[key] = value;
                }

                if (dict.TryGetValue("ComPort", out var port))
                {
                    PopulateComPorts();
                    if (ComPortComboBox.Items.Contains(port))
                        ComPortComboBox.SelectedItem = port;
                }
                if (dict.TryGetValue("ComSpeed", out var speed))
                {
                    int idx = ComSpeedComboBox.Items.IndexOf(speed);
                    if (idx >= 0)
                        ComSpeedComboBox.SelectedIndex = idx;
                }

                for (int i = 0; i < 5; i++)
                {
                    string key = $"MainButtonCmd{i + 1}";
                    if (dict.TryGetValue(key, out var cmd))
                    {
                        mainButtonCatCommands[i] = cmd;
                    }
                }

                UpdateMainButtonTooltips();
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                var lines = new List<string>();
                lines.Add("# Remote control app settings");
                lines.Add($"ComPort={ComPortComboBox.SelectedItem}");
                lines.Add($"ComSpeed={ComSpeedComboBox.SelectedItem}");

                for (int i = 0; i < 5; i++)
                {
                    lines.Add($"MainButtonCmd{i + 1}={mainButtonCatCommands[i]}");
                }

                File.WriteAllLines(iniFilePath, lines);
            }
            catch { }
        }
    }

    // Utility prompt dialog for right-click custom command input
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string defaultValue = "")
        {
            using Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };

            Label textLabel = new Label() { Left = 10, Top = 10, Text = text, AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 10, Top = 35, Width = 360, Text = defaultValue };
            Button confirmation = new Button() { Text = "OK", Left = 220, Width = 75, Top = 70, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = 300, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);

            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : defaultValue;
        }
    }
}

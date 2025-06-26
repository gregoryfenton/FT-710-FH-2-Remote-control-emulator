using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private bool MessageBoxIsOpen = false;
        private bool ignoreVolumeResponse = false;
        private bool ignoreSquelchResponse = false;
        private bool ignoreRFGainResponse = false;

        private System.Windows.Forms.Timer volumeResponseTimer;
        private System.Windows.Forms.Timer squelchResponseTimer;
        private System.Windows.Forms.Timer rfGainResponseTimer;
        private System.Windows.Forms.Timer? connectedTimer;

        private DateTime? connectedStartTime;

        private string? lastSentCommand;
        private readonly System.Windows.Forms.Timer riPollTimer = new System.Windows.Forms.Timer();
        private int leftRightStep = 1000;
        private int upDownMultiplier = 5;

        private string iniFilePath = Path.Combine(Application.StartupPath, "settings.ini");

        private string[] mainButtonCatCommands = new string[5] { "1;", "2;", "3;", "4;", "5;" };

        private ToolTip tooltip = new ToolTip();

        private System.Windows.Forms.Timer? pbMemTimeoutTimer = null;

        private bool ChangingTextBoxColor = false;
        private string lastFrequency = "";
        private readonly string[] BandEdgeLabels = new string[]
        {
            "1.8–2.0 MHz",     // 160M
            "3.5–3.8 MHz",     // 80M
            "5.3515–5.3785 MHz - Mind the gaps!",     // 60M
            "7.0–7.2 MHz",     // 40M
            "10.1–10.15 MHz",  // 30M
            "14.0–14.35 MHz",  // 20M
            "18.068–18.168 MHz", // 17M
            "21.0–21.45 MHz",  // 15M
            "24.89–24.99 MHz", // 12M
            "28.0–29.7 MHz",   // 10M
            "50.0–54.0 MHz",   // 6M
            "70.0+ MHz / Gen"  // 4M / Gen
        };
        private readonly long[] BandEdgeLow = new long[]
        {
            1800000,  // 160M
            3500000,  // 80M
            5351500,  // 60M
            7000000,  // 40M
            10100000, // 30M
            14000000, // 20M
            18068000, // 17M
            21000000, // 15M
            24890000, // 12M
            28000000, // 10M
            50000000, // 6M
            70000000  // 4M / Gen
        };
        private readonly long[] BandEdgeHigh = new long[]
        {
            2000000,  // 160M
            3800000,  // 80M
            5378500,  // 60M
            7200000,  // 40M
            10150000, // 30M
            14350000, // 20M
            18168000, // 17M
            21450000, // 15M
            24990000, // 12M
            29700000, // 10M
            54000000, // 6M
            100000000 // 4M / Gen (wide open)
        };

        public Form1()
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
            this.Text += $" v{version}";

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

            ButtonMainLeft.MouseDown += LeftRightButton_RightClick_Or_Click;
            ButtonMainRight.MouseDown += LeftRightButton_RightClick_Or_Click;
            ButtonMainUp.MouseDown += UpDownButton_RightClick_Or_Click;
            ButtonMainDown.MouseDown += UpDownButton_RightClick_Or_Click;

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
            riPollTimer.Tick += (s, e) => SendCommand("RI0;BI;AG0;SQ0;RG0;");

            volumeResponseTimer = new System.Windows.Forms.Timer { Interval = 500 };
            volumeResponseTimer.Tick += (s, e) => { ignoreVolumeResponse = false; volumeResponseTimer.Stop(); };

            squelchResponseTimer = new System.Windows.Forms.Timer { Interval = 500 };
            squelchResponseTimer.Tick += (s, e) => { ignoreSquelchResponse = false; squelchResponseTimer.Stop(); };

            rfGainResponseTimer = new System.Windows.Forms.Timer { Interval = 500 };
            rfGainResponseTimer.Tick += (s, e) => { ignoreRFGainResponse = false; rfGainResponseTimer.Stop(); };
            
            // Set tooltips on static controls
            InitializeTooltips();

            LoadSettings();

            UpdateMainButtonTooltips();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateComPorts();
            PopulateModeComboBox();
            PopulateBandSelectComboBox();
            ConfigureSliders();
            FrequencyTextBox.ReadOnly = true;
            FrequencyTextBox.Text = $"v{Assembly.GetExecutingAssembly().GetName().Version!.ToString()}";
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
                    connectedStartTime = DateTime.Now;

                    if (connectedTimer == null)
                    {
                        connectedTimer = new System.Windows.Forms.Timer();
                        connectedTimer.Interval = 1000; // 1 second
                        connectedTimer.Tick += ConnectedTimer_Tick;
                    }
                    connectedTimer.Start();
                    ComPortButton.Text = "Disconnect";
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
                    FrequencyTextBox.Text = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
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
                connectedTimer?.Stop();
                ConnectedTime.Text = ""; // Clear the label
                connectedStartTime = null;
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
        private void ModeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isConnected && serialPort?.IsOpen == true)
            {
                string modeCode = IndexToModeCode(ModeComboBox.SelectedIndex);
                SendCommand($"MD0{modeCode};");
            }
        }
        private void BandSelectDropDown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int index = BandSelectDropDown.SelectedIndex;
            if (index >= 0 && index < BandEdgeLabels.Length)
            {
                BandEdgesLabel.Text = $"{BandEdgeLabels[index]}";
                string catCommand = $"BS{index:D2};";
                SendCommand(catCommand);
            }
        }

        private void FreqPollTimer_Tick(object? sender, EventArgs e)
        {
            if (isConnected && (DateTime.Now - lastFrequencyUpdate).TotalMilliseconds > 1000)
            {
                SendCommand("IF;");
            }
        }
        private void ConnectedTimer_Tick(object? sender, EventArgs e)
        {
            if (connectedStartTime.HasValue)
            {
                TimeSpan elapsed = DateTime.Now - connectedStartTime.Value;
                ConnectedTime.Text = $"Connected {(int)elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
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
        private void PopulateBandSelectComboBox()
        {
            BandSelectDropDown.Items.Clear();

            BandSelectDropDown.Items.AddRange(new string[]
            {
                "160M band",    // 1.8 MHz
                "80M band",     // 3.5 MHz
                "60M band",     // 5 MHz
                "40M band",     // 7 MHz
                "30M band",     // 10 MHz
                "20M band",     // 14 MHz
                "17M band",     // 18 MHz
                "15M band",     // 21 MHz
                "12M band",     // 24.5 MHz
                "10M band",     // 28 MHz
                "6M band",      // 50 MHz
                "4M / Gen"      // 70 MHz / Gen
            });

            BandSelectDropDown.SelectedIndexChanged += BandSelectDropDown_SelectedIndexChanged;
            BandSelectDropDown.SelectedIndex = 0; // Optional: select default
        }
        private void UpdateBandEdgeLabel(long frequencyHz)
        {
            int band = BandSelectDropDown.SelectedIndex;
            if (band < 0 || band >= BandEdgeLow.Length)
                return;

            if (frequencyHz < BandEdgeLow[band] || frequencyHz > BandEdgeHigh[band])
            {
                // Out of band
                BandEdgesLabel.BackColor = Color.Red;
                BandEdgesLabel.ForeColor = Color.White;
            }
            else
            {
                // In band
                BandEdgesLabel.BackColor = SystemColors.Control;
                BandEdgesLabel.ForeColor = SystemColors.ControlText;
            }

            // Update the text again (optional if already done elsewhere)
            BandEdgesLabel.Text = $"{BandEdgeLabels[band]}";
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
            tooltip.SetToolTip(BandSelectDropDown, "Select radio band.");

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

        private void ConfigureSliders()
        {
            // Volume
            VolumeTrackBar.Minimum = 0;
            VolumeTrackBar.Maximum = 255;
            VolumeTrackBar.TickFrequency = 16;
            VolumeTrackBar.ValueChanged += VolumeTrackBar_ValueChanged;
            tooltip.SetToolTip(VolumeTrackBar, $"Volume: {VolumeTrackBar.Value}");

            // Squelch
            SquelchTrackBar.Minimum = 0;
            SquelchTrackBar.Maximum = 100;
            SquelchTrackBar.TickFrequency = 10;
            SquelchTrackBar.ValueChanged += SquelchTrackBar_ValueChanged;
            tooltip.SetToolTip(SquelchTrackBar, $"Squelch: {SquelchTrackBar.Value}");

            // RF Gain
            RFGainTrackBar.Minimum = 0;
            RFGainTrackBar.Maximum = 255;
            RFGainTrackBar.TickFrequency = 16;
            RFGainTrackBar.ValueChanged += RFGainTrackBar_ValueChanged;
            tooltip.SetToolTip(RFGainTrackBar, $"RF Gain: {RFGainTrackBar.Value}");
        }
        private void VolumeTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            int val = VolumeTrackBar.Value;
            string cat = $"AG0{val:D3};";
            SendCommand(cat);
            tooltip.SetToolTip(VolumeTrackBar, $"Volume: {val}");
            ignoreVolumeResponse = true;
            volumeResponseTimer.Stop();
            volumeResponseTimer.Start();
        }
        private void SquelchTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            int val = SquelchTrackBar.Value;
            string cat = $"SQ0{val:D3};";
            SendCommand(cat);
            tooltip.SetToolTip(SquelchTrackBar, $"Squelch: {val}");
            ignoreSquelchResponse = true;
            squelchResponseTimer.Stop();
            squelchResponseTimer.Start();
        }
        private void RFGainTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            int val = RFGainTrackBar.Value;
            string cat = $"RG0{val:D3};";
            SendCommand(cat);
            tooltip.SetToolTip(RFGainTrackBar, $"RF Gain: {val}");
            ignoreRFGainResponse = true;
            rfGainResponseTimer.Stop();
            rfGainResponseTimer.Start();
        }

        private void ShowStepSizeMenu(Control button)
        {
            var menu = new ContextMenuStrip();
            var stepValues = new[] { 1, 10, 100, 1000, 6250, 10000 }; // Sorted ascending
            foreach (var step in stepValues)
            {
                var item = new ToolStripMenuItem($"{step:N0} Hz");
                item.Tag = step;
                item.Click += (s, e) =>
                {
                    leftRightStep = (int)((ToolStripMenuItem)s!).Tag!;
                };
                menu.Items.Add(item);
            }
            menu.Show(button, button.PointToClient(Cursor.Position));
        }
        private void ShowMultiplierMenu(Control button)
        {
            var menu = new ContextMenuStrip();
            var multValues = new[] { 1, 2, 3, 4, 5, 10 }; // Sorted ascending
            foreach (var mult in multValues)
            {
                var item = new ToolStripMenuItem($"× {mult}");
                item.Tag = mult;
                item.Click += (s, e) =>
                {
                    upDownMultiplier = (int)((ToolStripMenuItem)s!).Tag!;
                    MessageBox.Show($"Multiplier set to ×{upDownMultiplier}", "Multiplier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                menu.Items.Add(item);
            }
            menu.Show(button, button.PointToClient(Cursor.Position));
        }

        private void HandleRadioResponse(string data)
        {
            if (data == "?;")
            {
                if (!string.IsNullOrEmpty(lastSentCommand) && !MessageBoxIsOpen)
                {
                    MessageBoxIsOpen = true;
                    MessageBox.Show("Unrecognised CAT Command: " + lastSentCommand);
                    MessageBoxIsOpen = false;
                }
                return;
            }

            if (data.StartsWith("AG0") && !ignoreVolumeResponse)
            {
                if (int.TryParse(data.Substring(3, 3), out int vol))
                {
                    VolumeTrackBar.Value = Math.Clamp(vol, VolumeTrackBar.Minimum, VolumeTrackBar.Maximum);
                    VolumeValueLabel.Text = $"AF Gain\n{VolumeTrackBar.Value.ToString()}";
                }
            }
            else if (data.StartsWith("SQ0") && !ignoreSquelchResponse)
            {
                if (int.TryParse(data.Substring(3, 3), out int squelch))
                {
                    SquelchTrackBar.Value = Math.Clamp(squelch, SquelchTrackBar.Minimum, SquelchTrackBar.Maximum);
                    SquelchValueLabel.Text = $"Squelch\n{SquelchTrackBar.Value.ToString()}";
                }
            }
            else if (data.StartsWith("RG0") && !ignoreRFGainResponse)
            {
                if (int.TryParse(data.Substring(3, 3), out int rf))
                {
                    RFGainTrackBar.Value = Math.Clamp(rf, RFGainTrackBar.Minimum, RFGainTrackBar.Maximum);
                    RFGainValueLabel.Text = $"RF Gain\n{RFGainTrackBar.Value.ToString()}";
                }
            }
            else if (data.StartsWith("AI"))
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
                        int bandIndex = GetBandIndexFromFrequency(freq);

                        // Update band dropdown if changed
                        if (BandSelectDropDown.SelectedIndex != bandIndex)
                            BandSelectDropDown.SelectedIndex = bandIndex;

                        // Update label appearance
                        UpdateBandEdgeLabel(freq);
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
        private void UpdateDecButton()
        {
            ButtonMainDec.Text = "BkIn";
            ButtonMainDec.BackColor = isBKINEnabled ? Color.LightGreen : SystemColors.Control;
        }

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
        private int GetBandIndexFromFrequency(long frequencyHz)
        {
            if (frequencyHz < 2000000) return 0;    // 1.8 MHz
            if (frequencyHz < 4000000) return 1;    // 3.5 MHz
            if (frequencyHz < 6000000) return 2;    // 5 MHz
            if (frequencyHz < 8000000) return 3;    // 7 MHz
            if (frequencyHz < 11000000) return 4;   // 10 MHz
            if (frequencyHz < 15000000) return 5;   // 14 MHz
            if (frequencyHz < 19000000) return 6;   // 18 MHz
            if (frequencyHz < 22000000) return 7;   // 21 MHz
            if (frequencyHz < 25000000) return 8;   // 24.5 MHz
            if (frequencyHz < 30000000) return 9;   // 28 MHz
            if (frequencyHz < 60000000) return 10;  // 50 MHz
            return 11;                              // 70 MHz / Gen or above
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
        private void LeftRightButton_RightClick_Or_Click(object? sender, MouseEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn == ButtonMainLeft)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        ChangeLeftRightStep();
                        tooltip.SetToolTip(ButtonMainLeft, $"Decrease frequency by {leftRightStep} (right-click to change step).");
                        return;
                    }
                    ChangeFrequency(-leftRightStep);
                    ResetPBMemState();
                }
                else if (btn == ButtonMainRight)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        ShowStepSizeMenu(btn);
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
                        ShowMultiplierMenu(btn);
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
        private void FreqButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                FrequencyChangeTextBox.AppendText(btn.Text);
            }
        }
        private void SetFrequency_Click(object? sender, EventArgs e)
        {
            if(float.TryParse(FrequencyChangeTextBox.Text, out float f))
            {
                if (f <= 75)
                {
                    f = f * 1000000;
                    FrequencyChangeTextBox.Text = ((long)f).ToString()!;
                }
            }
            if (long.TryParse(FrequencyChangeTextBox.Text, out long freq))
            {
                if (freq >= 30000 && freq <= 75000000 && isConnected && serialPort != null && serialPort.IsOpen)
                {
                    SetFrequency(freq);
                    FrequencyChangeTextBox.Clear();
                    ResetPBMemState();
                    UpdateBandEdgeLabel(freq);  // <- check visual feedback
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

        private void FrequencyChangeTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SetFrequency_Click(sender, EventArgs.Empty);
            }
        }


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
        private void ChangeLeftRightStep()
        {
            int[] options = { 1, 10, 100, 1000, 6250, 12500, 25000 };
            int currentIndex = Array.IndexOf(options, leftRightStep);
            int nextIndex = (currentIndex - 1);
            if(nextIndex <0)
                    nextIndex = options.Length - 1;
            leftRightStep = options[nextIndex];
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
                SetFrequency(newFreq);
            }
        }
        private void SetFrequency(long newFrequencyHz)
        {
            int newBandIndex = GetBandIndexFromFrequency(newFrequencyHz);

            if (BandSelectDropDown.SelectedIndex != newBandIndex)
            {
                BandSelectDropDown.SelectedIndex = newBandIndex; // Triggers the band change
                                                                 // Note: BandSelectDropDown_SelectedIndexChanged will send BSxx;
            }

            // Then send the frequency CAT command
            string freqCommand = $"FA{newFrequencyHz:D9};";  // or appropriate command for your radio
            SendCommand(freqCommand);
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

namespace Remote_control
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComPortComboBox = new ComboBox();
            ComPortButton = new Button();
            ComSpeedComboBox = new ComboBox();
            ButtonMain1 = new Button();
            ButtonMain2 = new Button();
            ButtonMain3 = new Button();
            ButtonMainMem = new Button();
            ButtonMain5 = new Button();
            ButtonMain4 = new Button();
            ButtonMainRight = new Button();
            ButtonMainUp = new Button();
            ButtonMainLeft = new Button();
            ButtonMainDec = new Button();
            ButtonMainDown = new Button();
            ButtonMainPB = new Button();
            FrequencyTextBox = new TextBox();
            FrequencyChangeTextBox = new TextBox();
            ButtonFreq1 = new Button();
            ButtonFreq2 = new Button();
            ButtonFreq3 = new Button();
            ButtonFreq4 = new Button();
            ButtonFreq5 = new Button();
            ButtonFreq6 = new Button();
            ButtonFreq7 = new Button();
            ButtonFreq8 = new Button();
            ButtonFreq9 = new Button();
            ButtonFreqClear = new Button();
            ButtonFreq0 = new Button();
            ButtonFreqSet = new Button();
            Status1Name = new Label();
            Status2Name = new Label();
            Status3Name = new Label();
            Status4Name = new Label();
            Status5Name = new Label();
            Status6Name = new Label();
            Status6Text = new Label();
            Status5Text = new Label();
            Status4Text = new Label();
            Status3Text = new Label();
            Status2Text = new Label();
            Status1Text = new Label();
            ModeComboBox = new ComboBox();
            BandSelectDropDown = new ComboBox();
            BandEdgesLabel = new Label();
            VolumeTrackBar = new TrackBar();
            SquelchTrackBar = new TrackBar();
            RFGainTrackBar = new TrackBar();
            VolumeValueLabel = new Label();
            SquelchValueLabel = new Label();
            RFGainValueLabel = new Label();
            ConnectedTime = new Label();
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SquelchTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RFGainTrackBar).BeginInit();
            SuspendLayout();
            // 
            // ComPortComboBox
            // 
            ComPortComboBox.FormattingEnabled = true;
            ComPortComboBox.Location = new Point(12, 6);
            ComPortComboBox.Name = "ComPortComboBox";
            ComPortComboBox.Size = new Size(111, 28);
            ComPortComboBox.TabIndex = 1;
            // 
            // ComPortButton
            // 
            ComPortButton.Location = new Point(129, 6);
            ComPortButton.Name = "ComPortButton";
            ComPortButton.Size = new Size(107, 29);
            ComPortButton.TabIndex = 2;
            ComPortButton.Text = "Connect";
            ComPortButton.UseVisualStyleBackColor = true;
            // 
            // ComSpeedComboBox
            // 
            ComSpeedComboBox.FormattingEnabled = true;
            ComSpeedComboBox.Items.AddRange(new object[] { "300", "1200", "2400", "4800", "9600", "14400", "19200", "28800", "33600", "38400", "57600", "115200", "250000" });
            ComSpeedComboBox.Location = new Point(12, 40);
            ComSpeedComboBox.Name = "ComSpeedComboBox";
            ComSpeedComboBox.Size = new Size(111, 28);
            ComSpeedComboBox.TabIndex = 3;
            // 
            // ButtonMain1
            // 
            ButtonMain1.Location = new Point(12, 112);
            ButtonMain1.Name = "ButtonMain1";
            ButtonMain1.Size = new Size(100, 29);
            ButtonMain1.TabIndex = 4;
            ButtonMain1.Text = "1";
            ButtonMain1.UseVisualStyleBackColor = true;
            // 
            // ButtonMain2
            // 
            ButtonMain2.Location = new Point(120, 112);
            ButtonMain2.Name = "ButtonMain2";
            ButtonMain2.Size = new Size(100, 29);
            ButtonMain2.TabIndex = 5;
            ButtonMain2.Text = "2";
            ButtonMain2.UseVisualStyleBackColor = true;
            // 
            // ButtonMain3
            // 
            ButtonMain3.Location = new Point(230, 112);
            ButtonMain3.Name = "ButtonMain3";
            ButtonMain3.Size = new Size(100, 29);
            ButtonMain3.TabIndex = 6;
            ButtonMain3.Text = "3";
            ButtonMain3.UseVisualStyleBackColor = true;
            // 
            // ButtonMainMem
            // 
            ButtonMainMem.Location = new Point(230, 147);
            ButtonMainMem.Name = "ButtonMainMem";
            ButtonMainMem.Size = new Size(100, 29);
            ButtonMainMem.TabIndex = 9;
            ButtonMainMem.Text = "Mem";
            ButtonMainMem.UseVisualStyleBackColor = true;
            // 
            // ButtonMain5
            // 
            ButtonMain5.Location = new Point(120, 147);
            ButtonMain5.Name = "ButtonMain5";
            ButtonMain5.Size = new Size(100, 29);
            ButtonMain5.TabIndex = 8;
            ButtonMain5.Text = "5";
            ButtonMain5.UseVisualStyleBackColor = true;
            // 
            // ButtonMain4
            // 
            ButtonMain4.Location = new Point(12, 147);
            ButtonMain4.Name = "ButtonMain4";
            ButtonMain4.Size = new Size(100, 29);
            ButtonMain4.TabIndex = 7;
            ButtonMain4.Text = "4";
            ButtonMain4.UseVisualStyleBackColor = true;
            // 
            // ButtonMainRight
            // 
            ButtonMainRight.Location = new Point(230, 182);
            ButtonMainRight.Name = "ButtonMainRight";
            ButtonMainRight.Size = new Size(100, 29);
            ButtonMainRight.TabIndex = 12;
            ButtonMainRight.Text = "►";
            ButtonMainRight.UseVisualStyleBackColor = true;
            // 
            // ButtonMainUp
            // 
            ButtonMainUp.Location = new Point(120, 182);
            ButtonMainUp.Name = "ButtonMainUp";
            ButtonMainUp.Size = new Size(100, 29);
            ButtonMainUp.TabIndex = 11;
            ButtonMainUp.Text = "▲";
            ButtonMainUp.UseVisualStyleBackColor = true;
            // 
            // ButtonMainLeft
            // 
            ButtonMainLeft.Location = new Point(12, 182);
            ButtonMainLeft.Name = "ButtonMainLeft";
            ButtonMainLeft.Size = new Size(100, 29);
            ButtonMainLeft.TabIndex = 10;
            ButtonMainLeft.Text = "◄";
            ButtonMainLeft.UseVisualStyleBackColor = true;
            // 
            // ButtonMainDec
            // 
            ButtonMainDec.Location = new Point(230, 215);
            ButtonMainDec.Name = "ButtonMainDec";
            ButtonMainDec.Size = new Size(100, 29);
            ButtonMainDec.TabIndex = 15;
            ButtonMainDec.Text = "BkIn";
            ButtonMainDec.UseVisualStyleBackColor = true;
            // 
            // ButtonMainDown
            // 
            ButtonMainDown.Location = new Point(120, 217);
            ButtonMainDown.Name = "ButtonMainDown";
            ButtonMainDown.Size = new Size(100, 29);
            ButtonMainDown.TabIndex = 14;
            ButtonMainDown.Text = "▼";
            ButtonMainDown.UseVisualStyleBackColor = true;
            // 
            // ButtonMainPB
            // 
            ButtonMainPB.Location = new Point(12, 217);
            ButtonMainPB.Name = "ButtonMainPB";
            ButtonMainPB.Size = new Size(100, 29);
            ButtonMainPB.TabIndex = 13;
            ButtonMainPB.Text = "P/B";
            ButtonMainPB.UseVisualStyleBackColor = true;
            // 
            // FrequencyTextBox
            // 
            FrequencyTextBox.Cursor = Cursors.IBeam;
            FrequencyTextBox.Location = new Point(12, 79);
            FrequencyTextBox.Name = "FrequencyTextBox";
            FrequencyTextBox.Size = new Size(111, 27);
            FrequencyTextBox.TabIndex = 16;
            FrequencyTextBox.TextAlign = HorizontalAlignment.Right;
            // 
            // FrequencyChangeTextBox
            // 
            FrequencyChangeTextBox.Cursor = Cursors.IBeam;
            FrequencyChangeTextBox.Location = new Point(114, 286);
            FrequencyChangeTextBox.Name = "FrequencyChangeTextBox";
            FrequencyChangeTextBox.Size = new Size(111, 27);
            FrequencyChangeTextBox.TabIndex = 17;
            // 
            // ButtonFreq1
            // 
            ButtonFreq1.Location = new Point(12, 317);
            ButtonFreq1.Name = "ButtonFreq1";
            ButtonFreq1.Size = new Size(100, 29);
            ButtonFreq1.TabIndex = 18;
            ButtonFreq1.Text = "1";
            ButtonFreq1.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq2
            // 
            ButtonFreq2.Location = new Point(120, 317);
            ButtonFreq2.Name = "ButtonFreq2";
            ButtonFreq2.Size = new Size(100, 29);
            ButtonFreq2.TabIndex = 19;
            ButtonFreq2.Text = "2";
            ButtonFreq2.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq3
            // 
            ButtonFreq3.Location = new Point(230, 317);
            ButtonFreq3.Name = "ButtonFreq3";
            ButtonFreq3.Size = new Size(100, 29);
            ButtonFreq3.TabIndex = 20;
            ButtonFreq3.Text = "3";
            ButtonFreq3.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq4
            // 
            ButtonFreq4.Location = new Point(12, 352);
            ButtonFreq4.Name = "ButtonFreq4";
            ButtonFreq4.Size = new Size(100, 29);
            ButtonFreq4.TabIndex = 21;
            ButtonFreq4.Text = "4";
            ButtonFreq4.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq5
            // 
            ButtonFreq5.Location = new Point(120, 352);
            ButtonFreq5.Name = "ButtonFreq5";
            ButtonFreq5.Size = new Size(100, 29);
            ButtonFreq5.TabIndex = 22;
            ButtonFreq5.Text = "5";
            ButtonFreq5.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq6
            // 
            ButtonFreq6.Location = new Point(230, 352);
            ButtonFreq6.Name = "ButtonFreq6";
            ButtonFreq6.Size = new Size(100, 29);
            ButtonFreq6.TabIndex = 23;
            ButtonFreq6.Text = "6";
            ButtonFreq6.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq7
            // 
            ButtonFreq7.Location = new Point(12, 387);
            ButtonFreq7.Name = "ButtonFreq7";
            ButtonFreq7.Size = new Size(100, 29);
            ButtonFreq7.TabIndex = 24;
            ButtonFreq7.Text = "7";
            ButtonFreq7.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq8
            // 
            ButtonFreq8.Location = new Point(120, 387);
            ButtonFreq8.Name = "ButtonFreq8";
            ButtonFreq8.Size = new Size(100, 29);
            ButtonFreq8.TabIndex = 25;
            ButtonFreq8.Text = "8";
            ButtonFreq8.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq9
            // 
            ButtonFreq9.Location = new Point(230, 387);
            ButtonFreq9.Name = "ButtonFreq9";
            ButtonFreq9.Size = new Size(100, 29);
            ButtonFreq9.TabIndex = 26;
            ButtonFreq9.Text = "9";
            ButtonFreq9.UseVisualStyleBackColor = true;
            // 
            // ButtonFreqClear
            // 
            ButtonFreqClear.Location = new Point(12, 422);
            ButtonFreqClear.Name = "ButtonFreqClear";
            ButtonFreqClear.Size = new Size(100, 29);
            ButtonFreqClear.TabIndex = 27;
            ButtonFreqClear.Text = "Clear";
            ButtonFreqClear.UseVisualStyleBackColor = true;
            // 
            // ButtonFreq0
            // 
            ButtonFreq0.Location = new Point(120, 422);
            ButtonFreq0.Name = "ButtonFreq0";
            ButtonFreq0.Size = new Size(100, 29);
            ButtonFreq0.TabIndex = 28;
            ButtonFreq0.Text = "0";
            ButtonFreq0.UseVisualStyleBackColor = true;
            // 
            // ButtonFreqSet
            // 
            ButtonFreqSet.Location = new Point(230, 422);
            ButtonFreqSet.Name = "ButtonFreqSet";
            ButtonFreqSet.Size = new Size(100, 29);
            ButtonFreqSet.TabIndex = 29;
            ButtonFreqSet.Text = "Set";
            ButtonFreqSet.UseVisualStyleBackColor = true;
            // 
            // Status1Name
            // 
            Status1Name.Font = new Font("Segoe UI", 7.8F);
            Status1Name.Location = new Point(7, 459);
            Status1Name.Name = "Status1Name";
            Status1Name.Size = new Size(50, 20);
            Status1Name.TabIndex = 30;
            Status1Name.Text = "label1";
            Status1Name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status2Name
            // 
            Status2Name.Font = new Font("Segoe UI", 7.8F);
            Status2Name.Location = new Point(63, 459);
            Status2Name.Name = "Status2Name";
            Status2Name.Size = new Size(50, 20);
            Status2Name.TabIndex = 31;
            Status2Name.Text = "label2";
            Status2Name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status3Name
            // 
            Status3Name.Font = new Font("Segoe UI", 7.8F);
            Status3Name.Location = new Point(119, 459);
            Status3Name.Name = "Status3Name";
            Status3Name.Size = new Size(50, 20);
            Status3Name.TabIndex = 32;
            Status3Name.Text = "label3";
            Status3Name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status4Name
            // 
            Status4Name.Font = new Font("Segoe UI", 7.8F);
            Status4Name.Location = new Point(175, 459);
            Status4Name.Name = "Status4Name";
            Status4Name.Size = new Size(50, 20);
            Status4Name.TabIndex = 33;
            Status4Name.Text = "label4";
            Status4Name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status5Name
            // 
            Status5Name.Font = new Font("Segoe UI", 7.8F);
            Status5Name.Location = new Point(226, 459);
            Status5Name.Name = "Status5Name";
            Status5Name.Size = new Size(50, 20);
            Status5Name.TabIndex = 34;
            Status5Name.Text = "label5";
            Status5Name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status6Name
            // 
            Status6Name.Font = new Font("Segoe UI", 7.8F);
            Status6Name.Location = new Point(282, 459);
            Status6Name.Name = "Status6Name";
            Status6Name.Size = new Size(50, 20);
            Status6Name.TabIndex = 35;
            Status6Name.Text = "label6";
            Status6Name.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status6Text
            // 
            Status6Text.Font = new Font("Segoe UI", 6F);
            Status6Text.Location = new Point(282, 479);
            Status6Text.Name = "Status6Text";
            Status6Text.Size = new Size(50, 20);
            Status6Text.TabIndex = 41;
            Status6Text.Text = "Status6";
            Status6Text.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status5Text
            // 
            Status5Text.Font = new Font("Segoe UI", 6F);
            Status5Text.Location = new Point(226, 479);
            Status5Text.Name = "Status5Text";
            Status5Text.Size = new Size(50, 20);
            Status5Text.TabIndex = 40;
            Status5Text.Text = "Status5";
            Status5Text.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status4Text
            // 
            Status4Text.Font = new Font("Segoe UI", 6F);
            Status4Text.Location = new Point(175, 479);
            Status4Text.Name = "Status4Text";
            Status4Text.Size = new Size(50, 20);
            Status4Text.TabIndex = 39;
            Status4Text.Text = "Status4";
            Status4Text.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status3Text
            // 
            Status3Text.Font = new Font("Segoe UI", 6F);
            Status3Text.Location = new Point(119, 479);
            Status3Text.Name = "Status3Text";
            Status3Text.Size = new Size(50, 20);
            Status3Text.TabIndex = 38;
            Status3Text.Text = "Status3";
            Status3Text.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status2Text
            // 
            Status2Text.Font = new Font("Segoe UI", 6F);
            Status2Text.Location = new Point(63, 479);
            Status2Text.Name = "Status2Text";
            Status2Text.Size = new Size(50, 20);
            Status2Text.TabIndex = 37;
            Status2Text.Text = "Status2";
            Status2Text.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Status1Text
            // 
            Status1Text.Font = new Font("Segoe UI", 6F);
            Status1Text.Location = new Point(7, 479);
            Status1Text.Name = "Status1Text";
            Status1Text.Size = new Size(50, 20);
            Status1Text.TabIndex = 36;
            Status1Text.Text = "Status1";
            Status1Text.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ModeComboBox
            // 
            ModeComboBox.FormattingEnabled = true;
            ModeComboBox.Location = new Point(129, 78);
            ModeComboBox.Name = "ModeComboBox";
            ModeComboBox.Size = new Size(111, 28);
            ModeComboBox.TabIndex = 42;
            // 
            // BandSelectDropDown
            // 
            BandSelectDropDown.FormattingEnabled = true;
            BandSelectDropDown.Location = new Point(12, 252);
            BandSelectDropDown.Name = "BandSelectDropDown";
            BandSelectDropDown.Size = new Size(111, 28);
            BandSelectDropDown.TabIndex = 43;
            // 
            // BandEdgesLabel
            // 
            BandEdgesLabel.AutoSize = true;
            BandEdgesLabel.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BandEdgesLabel.ImageAlign = ContentAlignment.TopLeft;
            BandEdgesLabel.Location = new Point(129, 257);
            BandEdgesLabel.Name = "BandEdgesLabel";
            BandEdgesLabel.Size = new Size(77, 17);
            BandEdgesLabel.TabIndex = 44;
            BandEdgesLabel.Text = "Band Edges";
            // 
            // VolumeTrackBar
            // 
            VolumeTrackBar.Location = new Point(347, 6);
            VolumeTrackBar.Name = "VolumeTrackBar";
            VolumeTrackBar.Orientation = Orientation.Vertical;
            VolumeTrackBar.Size = new Size(56, 130);
            VolumeTrackBar.TabIndex = 45;
            // 
            // SquelchTrackBar
            // 
            SquelchTrackBar.Location = new Point(347, 162);
            SquelchTrackBar.Name = "SquelchTrackBar";
            SquelchTrackBar.Orientation = Orientation.Vertical;
            SquelchTrackBar.Size = new Size(56, 130);
            SquelchTrackBar.TabIndex = 46;
            // 
            // RFGainTrackBar
            // 
            RFGainTrackBar.Location = new Point(347, 321);
            RFGainTrackBar.Name = "RFGainTrackBar";
            RFGainTrackBar.Orientation = Orientation.Vertical;
            RFGainTrackBar.Size = new Size(56, 130);
            RFGainTrackBar.TabIndex = 47;
            // 
            // VolumeValueLabel
            // 
            VolumeValueLabel.Font = new Font("Segoe UI", 7.8F);
            VolumeValueLabel.Location = new Point(341, 130);
            VolumeValueLabel.Margin = new Padding(0);
            VolumeValueLabel.Name = "VolumeValueLabel";
            VolumeValueLabel.Size = new Size(56, 20);
            VolumeValueLabel.TabIndex = 48;
            VolumeValueLabel.Text = "AF Gain";
            VolumeValueLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // SquelchValueLabel
            // 
            SquelchValueLabel.Font = new Font("Segoe UI", 7.8F);
            SquelchValueLabel.Location = new Point(347, 286);
            SquelchValueLabel.Margin = new Padding(0);
            SquelchValueLabel.Name = "SquelchValueLabel";
            SquelchValueLabel.Size = new Size(56, 20);
            SquelchValueLabel.TabIndex = 49;
            SquelchValueLabel.Text = "Squelch";
            SquelchValueLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // RFGainValueLabel
            // 
            RFGainValueLabel.Font = new Font("Segoe UI", 7.8F);
            RFGainValueLabel.Location = new Point(347, 445);
            RFGainValueLabel.Margin = new Padding(0);
            RFGainValueLabel.Name = "RFGainValueLabel";
            RFGainValueLabel.Size = new Size(61, 20);
            RFGainValueLabel.TabIndex = 50;
            RFGainValueLabel.Text = "RF Gain";
            RFGainValueLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ConnectedTime
            // 
            ConnectedTime.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ConnectedTime.Location = new Point(129, 38);
            ConnectedTime.Name = "ConnectedTime";
            ConnectedTime.Size = new Size(201, 30);
            ConnectedTime.TabIndex = 51;
            ConnectedTime.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 538);
            Controls.Add(ConnectedTime);
            Controls.Add(RFGainValueLabel);
            Controls.Add(SquelchValueLabel);
            Controls.Add(VolumeValueLabel);
            Controls.Add(RFGainTrackBar);
            Controls.Add(SquelchTrackBar);
            Controls.Add(VolumeTrackBar);
            Controls.Add(BandEdgesLabel);
            Controls.Add(BandSelectDropDown);
            Controls.Add(ModeComboBox);
            Controls.Add(Status6Text);
            Controls.Add(Status5Text);
            Controls.Add(Status4Text);
            Controls.Add(Status3Text);
            Controls.Add(Status2Text);
            Controls.Add(Status1Text);
            Controls.Add(Status6Name);
            Controls.Add(Status5Name);
            Controls.Add(Status4Name);
            Controls.Add(Status3Name);
            Controls.Add(Status2Name);
            Controls.Add(Status1Name);
            Controls.Add(ButtonFreqSet);
            Controls.Add(ButtonFreq0);
            Controls.Add(ButtonFreqClear);
            Controls.Add(ButtonFreq9);
            Controls.Add(ButtonFreq8);
            Controls.Add(ButtonFreq7);
            Controls.Add(ButtonFreq6);
            Controls.Add(ButtonFreq5);
            Controls.Add(ButtonFreq4);
            Controls.Add(ButtonFreq3);
            Controls.Add(ButtonFreq2);
            Controls.Add(ButtonFreq1);
            Controls.Add(FrequencyChangeTextBox);
            Controls.Add(FrequencyTextBox);
            Controls.Add(ButtonMainDec);
            Controls.Add(ButtonMainDown);
            Controls.Add(ButtonMainPB);
            Controls.Add(ButtonMainRight);
            Controls.Add(ButtonMainUp);
            Controls.Add(ButtonMainLeft);
            Controls.Add(ButtonMainMem);
            Controls.Add(ButtonMain5);
            Controls.Add(ButtonMain4);
            Controls.Add(ButtonMain3);
            Controls.Add(ButtonMain2);
            Controls.Add(ButtonMain1);
            Controls.Add(ComSpeedComboBox);
            Controls.Add(ComPortButton);
            Controls.Add(ComPortComboBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "Form1";
            Text = "FT-710 Remote control";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)VolumeTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)SquelchTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)RFGainTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ComboBox ComPortComboBox;
        private Button ComPortButton;
        private ComboBox ComSpeedComboBox;
        private Button ButtonMain1;
        private Button ButtonMain2;
        private Button ButtonMain3;
        private Button ButtonMain4;
        private Button ButtonMain5;
        private Button ButtonMainMem;
        private Button ButtonMainRight;
        private Button ButtonMainUp;
        private Button ButtonMainLeft;
        private Button ButtonMainDec;
        private Button ButtonMainDown;
        private Button ButtonMainPB;
        private TextBox FrequencyTextBox;
        private TextBox FrequencyChangeTextBox;
        private Button ButtonFreq1;
        private Button ButtonFreq2;
        private Button ButtonFreq3;
        private Button ButtonFreq4;
        private Button ButtonFreq5;
        private Button ButtonFreq6;
        private Button ButtonFreq7;
        private Button ButtonFreq8;
        private Button ButtonFreq9;
        private Button ButtonFreqClear;
        private Button ButtonFreq0;
        private Button ButtonFreqSet;
        private Label Status1Name;
        private Label Status2Name;
        private Label Status3Name;
        private Label Status4Name;
        private Label Status5Name;
        private Label Status6Name;
        private Label Status6Text;
        private Label Status5Text;
        private Label Status4Text;
        private Label Status3Text;
        private Label Status2Text;
        private Label Status1Text;
        private ComboBox ModeComboBox;
        private ComboBox BandSelectDropDown;
        private Label BandEdgesLabel;
        private TrackBar VolumeTrackBar;
        private TrackBar SquelchTrackBar;
        private TrackBar RFGainTrackBar;
        private Label VolumeValueLabel;
        private Label SquelchValueLabel;
        private Label RFGainValueLabel;
        private Label ConnectedTime;
    }
}

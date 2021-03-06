﻿using System;
using System.Windows.Forms;
using Dinah.Core.Windows.Forms;

namespace LibationWinForms.BookLiberation
{
    public partial class AutomatedBackupsForm : Form
    {
        public bool KeepGoingVisible
        {
            get => keepGoingCb.Visible;
            set => keepGoingCb.Visible = value;
        }

        public bool KeepGoingChecked => keepGoingCb.Checked;

        public bool KeepGoing
            => keepGoingCb.Visible
            && keepGoingCb.Enabled
            && keepGoingCb.Checked;

        public AutomatedBackupsForm()
        {
            InitializeComponent();
        }

		public void WriteLine(string text)
			=> logTb.UIThread(() => logTb.AppendText($"{DateTime.Now} {text}{Environment.NewLine}"));

		public void FinalizeUI()
        {
            keepGoingCb.Enabled = false;
            logTb.AppendText("");
        }

        private void AutomatedBackupsForm_FormClosing(object sender, FormClosingEventArgs e) => keepGoingCb.Checked = false;
    }
}

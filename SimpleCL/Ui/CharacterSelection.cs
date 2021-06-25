﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SilkroadSecurityApi;
using SimpleCL.Enums;
using SimpleCL.Enums.Common;
using SimpleCL.Model;
using SimpleCL.Model.Character;
using SimpleCL.Network;
using SimpleCL.Util;

namespace SimpleCL.Ui
{
    public partial class CharacterSelection : Form
    {
        private readonly Server _agent;
        public CharacterSelection(List<Character> chars, Server agent)
        {
            _agent = agent;
            InitializeComponent();
            characterListDataGridView.DataSource = chars;
            
            FormBorderStyle = FormBorderStyle.FixedSingle;
            CenterToScreen();

            characterListDataGridView.KeyDown += (sender, args) =>
            {
                DataGridViewCell currentCell = characterListDataGridView.CurrentCell;
                if (args.KeyCode == Keys.Enter && currentCell != null)
                {
                    SelectCharacter(characterListDataGridView.Rows[currentCell.RowIndex], args);
                }
            };
        }

        private void SelectCharacter(object sender, EventArgs args)
        {
            Character selected = (Character) ((DataGridViewRow) sender).DataBoundItem;
            if (selected == null)
            {
                return;
            }

            LocalPlayer.Get.MaxHp = selected.Hp;
            LocalPlayer.Get.MaxMp = selected.Mp;
            Packet characterJoin = new Packet(Opcodes.Agent.Request.CHARACTER_SELECTION_JOIN);
            characterJoin.WriteAscii(selected.Name);
            _agent.Inject(characterJoin);
            Dispose(true);
        }
    }
}
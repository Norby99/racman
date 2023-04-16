﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace racman
{
    public partial class UYAUnlocks : Form
    {
        public rac3 game;

        // sorry
        private List<Label> levelLabels = new List<Label>();
        private List<ComboBox> levelCombos = new List<ComboBox>();
        private List<UYAItem> weapons = new List<UYAItem>();

        public static List<string> gcItems = new List<string>
        {
            "Plasma Coil", "Lava Gun", "Bouncer", "Miniturret", "Shield Charger"
        };

        private List<UYAItem> items = new List<UYAItem>
        {
            // new UYAItem("#", 0x00, 0x4A8, 0x5F0),
            // new UYAItem("#", 0x01, 0x4A9, 0x5F4),
            new UYAItem("Heli Pack", 0x02, 0x4AA, 0x5F8),
            new UYAItem("Thruster Pack", 0x03, 0x4AB, 0x5FC),
            // new UYAItem("Hydro Pack", 0x04, 0x4AC, 0x600),
            // new UYAItem("Map-o-matic", 0x05, 0x4AD, 0x604),
            // new UYAItem("Commando Suit", 0x06, 0x4AE, 0x608),
            new UYAItem("Bolt Grabber v2", 0x07, 0x4AF, 0x60C),
            // new UYAItem("Levitator", 0x08, 0x4B0, 0x610),
            // new UYAItem("Omniwrench", 0x09, 0x4B1, 0x614),
            new UYAItem("Bomb Glove", 0x0A, 0x4B2, 0x618),
            new UYAItem("Hypershot", 0x0B, 0x4B3, 0x61C),
            // new UYAItem("Paradox ERR", 0x0C, 0x4B4, 0x620),
            new UYAItem("Gravity Boots", 0x0D, 0x4B5, 0x624),
            new UYAItem("Grindboots", 0x0E, 0x4B6, 0x628),
            // new UYAItem("Glider", 0x0F, 0x4B7, 0x62C),
            new UYAItem("Plasma Coil", 0x10, 0x4B8, 0x630, 8), // GC item
            new UYAItem("Lava Gun", 0x11, 0x4B9, 0x634, 8), // GC item
            new UYAItem("Refractor", 0x12, 0x4BA, 0x638),
            new UYAItem("Bouncer", 0x13, 0x4BB, 0x63C, 8), // GC item
            new UYAItem("The Hacker", 0x14, 0x4BC, 0x640),
            new UYAItem("Miniturret", 0x15, 0x4BD, 0x644, 8), // GC item
            new UYAItem("Shield Charger", 0x16, 0x4BE, 0x648, 8), // GC item
            // new UYAItem("Paradox ERR", 0x17, 0x4BF, 0x64C),
            // new UYAItem("Paradox ERR", 0x18, 0x4C0, 0x650),
            // new UYAItem("Paradox ERR", 0x19, 0x4C1, 0x654),
            new UYAItem("The Hacker", 0x1A, 0x4C2, 0x658),
            // new UYAItem("#", 0x1B, 0x4C3, 0x65C),
            new UYAItem("Grindboots", 0x1C, 0x4C4, 0x660),
            new UYAItem("Charge Boots", 0x1D, 0x4C5, 0x664),
            new UYAItem("Tyhrra Guise", 0x1E, 0x4C6, 0x668),
            new UYAItem("Warp Pad", 0x1F, 0x4C7, 0x66C),
            new UYAItem("Nano Pak", 0x20, 0x4C8, 0x670),
            // new UYAItem("Star Map", 0x21, 0x4C9, 0x674),
            new UYAItem("Master Plan", 0x22, 0x4CA, 0x678),
            new UYAItem("PDA", 0x23, 0x4CB, 0x67C),
            // new UYAItem("Third Person Mode", 0x24, 0x4CC, 0x680),
            // new UYAItem("First Person Mode", 0x25, 0x4CD, 0x684),
            // new UYAItem("Lock Strafe", 0x26, 0x4CE, 0x688),
            new UYAItem("Shock Blaster", 0x27, 0x4CF, 0x68C, 8),
            new UYAItem("N60 Storm", 0x2F, 0x4D7, 0x6AC, 8),
            new UYAItem("Infector", 0x37, 0x4DF, 0x6CC, 8),
            new UYAItem("Annihilator", 0x3F, 0x4E7, 0x6EC, 8),
            new UYAItem("Spitting Hydra", 0x47, 0x4EF, 0x70C, 8),
            new UYAItem("Disc Blade Gun", 0x4F, 0x4F7, 0x72C, 8),
            new UYAItem("Agents of Doom", 0x57, 0x4FF, 0x74C, 8),
            new UYAItem("Rift Inducer", 0x5F, 0x507, 0x76C, 8),
            new UYAItem("Holoshield", 0x67, 0x50F, 0x78C, 8),
            new UYAItem("Flux Rifle", 0x6F, 0x517, 0x7AC, 8),
            new UYAItem("Nitro Launcher", 0x77, 0x51F, 0x7CC, 8),
            new UYAItem("Plasma Whip", 0x7F, 0x527, 0x7EC, 8),
            new UYAItem("Suck Cannon", 0x87, 0x52F, 0x80C, 8),
            new UYAItem("Quack-O-Ray", 0x8F, 0x537, 0x82C, 8),
            new UYAItem("R3YNO", 0x97, 0x53F, 0x84C, 5),

        };

        private List<string> itemNames => items.Select(obj => obj.name).ToList();
        private UYAItem itemByName(string itemName) => items.Find(obj => obj.name == itemName);

        public UYAUnlocks(rac3 game)
        {
            this.game = game;
            InitializeComponent();

            levelLabels = new List<Label>
            {
                label3, label4, label5, label6, label7, 
                label8, label9, label10, label11, label12, 
                label13, label14, label15, label16, label17, 
                label18, label19, label20, label21, label22
            };
            levelCombos = new List<ComboBox>
            {
                comboBox1, comboBox2, comboBox3, comboBox4, comboBox5,
                comboBox6, comboBox7, comboBox8, comboBox9, comboBox10,
                comboBox11, comboBox12, comboBox13, comboBox14, comboBox15,
                comboBox16, comboBox17, comboBox18, comboBox19, comboBox20
            };
        }

        private void UYAUnlocks_Load(object sender, EventArgs e)
        {
            int index = 0;
            foreach (var it in items)
            {
                checklistItems.Items.Add(it.name, it.IsUnlocked(game));
                Console.Write(it.name);
                Console.WriteLine(it.levels);
                if (it.levels > 1)
                {
                    weapons.Add(it);
                    levelLabels[index].Text = it.name;
                    var selected = it.GetVersionHeuristic(game);
                    if (selected == 0) selected = 1;

                    for (int i = 1; i <= it.levels; i++)
                    {
                        levelCombos[index].Items.Add("v" + i.ToString());
                    }
                    levelCombos[index].SelectedIndex = (int)selected - 1;

                    // Lexical scoping strikes again
                    var indexCopy = index;
                    levelCombos[index].SelectedIndexChanged += (_sender, _e) =>
                    {
                        var newVersion = levelCombos[indexCopy].SelectedIndex + 1;
                        weapons[indexCopy].SetVersion(game, (uint)newVersion);
                    };

                    index++;
                }
            }
        }

        private void UYAUnlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool enable = true;
            if (e.NewValue == CheckState.Checked) enable = true;
            if (e.NewValue == CheckState.Unchecked) enable = false;
            // WHY DOES THIS EXIST
            if (e.NewValue == CheckState.Indeterminate) return;

            var gameItem = items[e.Index];
            gameItem.LockOrUnlock(game, enable);
        }

        private void buttonUnlockAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checklistItems.Items.Count; i++) checklistItems.SetItemChecked(i, true);
            foreach (var it in items) it.LockOrUnlock(game, true);
        }

        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checklistItems.Items.Count; i++) checklistItems.SetItemChecked(i, false);
            foreach (var it in items) it.LockOrUnlock(game, false);
        }

        private void buttonUpgrade_Click(object sender, EventArgs e)
        {
            foreach (var it in items) it.SetVersion(game, it.levels);
            foreach (var combo in levelCombos)
            {
                combo.SelectedIndex = combo.Items.Count - 1;
            }
        }

        private void buttonUpgrade_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("except the ryno i guess that goes to v5", buttonUpgrade);
        }

        private void buttonDowngrade_Click(object sender, EventArgs e)
        {
            // SetExp works wrongly for some weapons and this is a workaround
            game.api.WriteMemory(game.pid, rac3.addr.expArray, new byte[726]);

            foreach (var it in items)
            {
                it.SetVersion(game, 1);
                // Have to do this or it upgrades again instantly
                it.SetExp(game, 0);
            }
            foreach (var combo in levelCombos)
            {
                combo.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var neededItems = new string[]
            {
                "Miniturret", "Shield Charger", "Shock Blaster", "Rift Inducer", "Flux Rifle", 
                "Nitro Launcher", "Plasma Whip", "Suck Cannon", "PDA", "Charge Boots"
            };
            foreach (var name in neededItems)
            {
                var item = itemByName(name);
                item.LockOrUnlock(game, true);
                item.SetVersion(game, item.levels);
            }

            // Special case RYNO v4
            var ryno = itemByName("R3YNO");
            ryno.SetVersion(game, 4);
            ryno.LockOrUnlock(game, true);
            // Supposedly the exp for the R3YNO v4
            // However this is one of the setExps that doesnt work so enjoy your graphical bugs/levelling lol lmao
            ryno.SetExp(game, 2560001);

        }
    }

    // This basically corresponds to a row of the spreadsheet.
    // https://docs.google.com/spreadsheets/d/1uZBCG_QkMCzCIdYgSZr1CfKIFTNrRYJrNMuQDCtAOWo/edit "items" tab
    // However working offsets seem to be different so the constructor changes some of them.
    public class UYAItem
    {
        public string name;
        public uint id;
        public uint unlockOffset;
        public uint expOffset;
        public uint levels;

        public UYAItem(string name, uint id, uint unlockOffset, uint expOffset)
            : this(name, id, unlockOffset, expOffset, 1) { }

        public UYAItem(string name, uint id, uint unlockOffset, uint expOffset, uint levels)
        {
            this.name = name;
            this.id = id;
            // The first item has unlock value 0x4A8
            this.unlockOffset = unlockOffset - 0x4A8;
            // The first item has exp value 0x5F0
            this.expOffset = expOffset - 0x5F0;
            this.levels = levels;
        }

        public uint VersionNTableOffset(uint version)
        {
            // v1 weapons same as original offset in items table
            if (version == 1) return id;

            // What the fuck, insomniac? WHY????? WHAT IS WRONG WITH YOU?
            if (name == "Bouncer")
            {
                if (version == 2) return 0xA6;
                else return 0xB1 + version;
            }
            else if (name == "Plasma Coil")
            {
                if (version == 2) return 0xA0;
                else return 0xB7 + version;
            }
            else if (name == "Shield Charger")
            {
                if (version == 2) return 0xA7;
                else return 0xBD + version;
            }
            else if (name == "Lava Gun")
            {
                if (version == 2) return 0xA1;
                else return 0xAB + version;
            }
            else if (name == "Miniturret")
            {
                if (version == 2) return 0xA2;
                else return 0xA5 + version;
            }

            return id + version - 1;
        }

        public bool IsUnlocked(rac3 game)
        {
            var res = game.api.ReadMemory(game.pid, rac3.addr.unlockArray + unlockOffset, 1);
            return res[0] == 1;
        }

        public void LockOrUnlock(rac3 game, bool enable) =>
               game.api.WriteMemory(game.pid, rac3.addr.unlockArray + unlockOffset, new byte[] { Convert.ToByte(enable) });

        public void SetExp(rac3 game, uint value) =>
            game.api.WriteMemory(game.pid, rac3.addr.expArray + expOffset, value);

        public void SetVersion(rac3 game, uint version)
        {
            if (levels <= 1) return;
            game.api.WriteMemory(game.pid, rac3.addr.itemArray + id, new byte[] { (byte)VersionNTableOffset(version) });
        }

        // Doesn't work correctly for GC weapons because what the fuck are you talking about, insomniac?
        public uint GetVersionHeuristic(rac3 game)
        {
            if (UYAUnlocks.gcItems.Contains(name)) return 0;
            var val = game.api.ReadMemory(game.pid, rac3.addr.itemArray + id, 1);
            return val[0] - id + 1;
        }
    }
}
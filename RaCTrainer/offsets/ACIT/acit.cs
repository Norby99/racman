﻿using racman.offsets;
using System;
using System.Collections.Generic;
using racman.offsets.ACIT;

namespace racman
{
    public class acit : IGame, IAutosplitterAvailable
    {
        public dynamic Planets = new
        {
            agorian_arena = ("Agorian Arena"),
            axiom_city = ("Axion City"),
            front_end = ("Front End"),
            galacton_ship = ("Vorselon Ship"),
            gimlick_valley = ("Gimlick Valey"),
            great_clock_a = ("Great Clock 1"),
            great_clock_b = ("Great Clock 2"),
            great_clock_c = ("Great Clock 3"),
            great_clock_d = ("Great Clock 4"),
            great_clock_e = ("Great Clock 5"),
            insomniac_museum = ("Insomniac Museum"),
            krell_canyon = ("Krell Canyon"),
            molonoth = ("Molonoth Fields"),
            nefarious_statio = ("Nefarious Station"),
            space_sector_1 = ("Space Sector 1"),
            space_sector_2 = ("Space Sector 2"),
            space_sector_3 = ("Space Sector 3"),
            space_sector_4 = ("Space Sector 4"),
            space_sector_5 = ("Space Sector 5"),
            tombli = ("Tombli Outpost"),
            valkyrie_fleet = ("Valkyrie Fleet"),
            zolar_forest = ("Zolar Forest")
        };

        public static ACITAddresses addr;

        public bool HasInputDisplay => addr.inputOffset > 0 && addr.analogOffset > 0 && addr.currentPlanet > 0;
        public bool IsAutosplitterSupported => addr.IsAutosplitterSupported;
        public bool IsSelfKillSupported => addr.IsSelfKillSupported;
        public bool HasWeaponUnlock => addr.weapons > 0;
        public bool canRemoveCutscenes => addr.cutscenesArray != null && addr.cutscenesArray.Length > 0;

        private long lastUnlocksUpdate = 0;
        private ACITWeaponFactory weaponFactory;
        // array storing every cutscene path initial byte
        private byte[][] cutscenesInitByteArray;

        public acit(IPS3API api) : base(api)
        {
            addr = new ACITAddresses(api.getGameTitleID());
            weaponFactory = new ACITWeaponFactory();
            if (canRemoveCutscenes)
            {
                cutscenesInitByteArray = ReadCutsceneStrings();
            }
        }

        public IEnumerable<(uint addr, uint size)> AutosplitterAddresses => new (uint, uint)[]
        {
            (addr.currentPlanet, 4),        // current planet
            (addr.gameState1Ptr, 4),        // game state1
            (addr.cutsceneState1Ptr, 4),    // cutscene state1
            (addr.cutsceneState2Ptr, 4),    // cutscene state2
            (addr.cutsceneState3Ptr, 4),    // cutscene state3
            (addr.saveFileIDPtr, 4),        // save file ID
            (addr.boltCount, 4),            // bolt count
            (addr.playerCoords, 4),         // player X coord
            (addr.playerCoords + 0x8, 4),   // player Y coord
            (addr.playerCoords + 0x4, 4),   // player Z coord
            (addr.azimuthHPPtr, 4),         // azimuth HP
            (addr.timerPtr, 4),             // timer
        };

        public override void ResetLevelFlags()
        {
            throw new NotImplementedException();
        }

        public override void SetupFile()
        {
            throw new NotImplementedException();
        }

        public override void SetFastLoads(bool toggle = false)
        {
            throw new NotImplementedException();
        }

        public override void ToggleInfiniteAmmo(bool toggle = false)
        {
            throw new NotImplementedException();
        }

        public override void CheckInputs(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates internal list of unlocked items. There was a bug in the Ratchetron C# API that maked it unfeasibly slow to get each item as a single byte.
        /// </summary>
        private void UpdateUnlocks()
        {
            if (DateTime.Now.Ticks < lastUnlocksUpdate + 10000000)
            {
                return;
            }

            byte[] memory = api.ReadMemory(pid, addr.weapons, ACITWeaponFactory.weaponCount * ACITWeaponFactory.weaponMemoryLenght);

            weaponFactory.updateWeapons(memory);

            lastUnlocksUpdate = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Set the unlock state of a weapon (unlocked or not).
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="unlockState"></param>
        public void setUnlockState(ACITWeapon weapon, bool unlockState)
        {
            weapon.isUnlocked = unlockState;
            api.WriteMemory(pid, addr.weapons + (weapon.index * ACITWeaponFactory.weaponMemoryLenght) + ACITWeaponFactory.weaponUnlockOffset, BitConverter.GetBytes(unlockState));

        }

        /// <summary>
        /// Get a list of all weapons.
        /// </summary>
        /// <returns></returns>
        public List<ACITWeapon> GetWeapons()
        {
            UpdateUnlocks();
            return HasWeaponUnlock ? weaponFactory.weapons : null;
        }

        /// <summary>
        /// Set the level of a weapon.
        /// It's not perfect, it only works for level 1, 5 and 10.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="level"></param>
        public void setWeaponLevel(ACITWeapon weapon, uint level)
        {
            level--;
            uint xp = (uint)(level == 0 ? 0 : 0xFF);
            uint weaponIndex = weapon.index;
            api.WriteMemory(pid, addr.weapons + (weaponIndex * ACITWeaponFactory.weaponMemoryLenght) + ACITWeaponFactory.weaponlevel1Offset, BitConverter.GetBytes(xp));
            api.WriteMemory(pid, addr.weapons + (weaponIndex * ACITWeaponFactory.weaponMemoryLenght) + ACITWeaponFactory.weaponlevel2Offset, BitConverter.GetBytes(xp));
            api.WriteMemory(pid, addr.weapons + (weaponIndex * ACITWeaponFactory.weaponMemoryLenght) + ACITWeaponFactory.weaponlevel3Offset, BitConverter.GetBytes(xp));
            api.WriteMemory(pid, addr.weapons + (weaponIndex * ACITWeaponFactory.weaponMemoryLenght) + ACITWeaponFactory.weaponlevel4Offset, BitConverter.GetBytes(xp));

            api.WriteMemory(pid, addr.weapons + (weaponIndex * ACITWeaponFactory.weaponMemoryLenght) + ACITWeaponFactory.weaponLevelOffset, BitConverter.GetBytes(level));
            weapon.level = level;
        }

        private byte[][] ReadCutsceneStrings()
        {
            byte[][] bytes = new byte[addr.cutscenesArray.Length][];
            for (int i = 0; i < addr.cutscenesArray.Length; i++)
            {
                bytes[i] = api.ReadMemory(pid, addr.cutscenesArray[i], 4);
            }
            return bytes;
        }

        /// <summary>
        /// Enable or disable cutscenes.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableCutscenes(bool enable)
        {
            if (enable)
            {
                for (int i = 0; i < cutscenesInitByteArray.Length; i++)
                {
                    api.WriteMemory(pid, addr.cutscenesArray[i], cutscenesInitByteArray[i]);
                }
                Console.WriteLine("Cutscenes enabled!");
            }
            else
            {
                for (int i = 0; i < cutscenesInitByteArray.Length; i++)
                {
                    api.WriteMemory(pid, addr.cutscenesArray[i], new byte[] { 0 });
                }
                Console.WriteLine("Cutscenes disabled!");
            }
        }

        /// <summary>
        /// Inverting analog sticks offsets.
        /// </summary>
        protected override void SetupInputDisplayMemorySubsAnalogs()
        {
            int analogRSubID = api.SubMemory(pid, addr.analogOffset + 8, 8, (value) =>
            {
                Inputs.ry = BitConverter.ToSingle(value, 0);
                Inputs.rx = BitConverter.ToSingle(value, 4);
            });

            int analogYSubID = api.SubMemory(pid, addr.analogOffset, 8, (value) =>
            {
                Inputs.ly = BitConverter.ToSingle(value, 0);
                Inputs.lx = BitConverter.ToSingle(value, 4);
            });
        }
    }
}
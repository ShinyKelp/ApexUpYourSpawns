
namespace ApexUpYourSpawns
{
    using BepInEx.Logging;
    using Menu.Remix.MixedUI;
    using RWCustom;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using static ApexUtils;
    
    //Options interface class.
    public class ApexUpYourSpawnsOptions : OptionInterface
    {
        public Configurable<bool> fillLineages;
        public Configurable<bool> forceFreshSpawns;
        public Configurable<bool> balancedSpawns;

        private Configurable<string> comboBoxConfig, presetTextConfig;

        private OpSimpleButton replacementDescription, extrasDescription, replacementDescription2, extrasDescription2,
            savePresetButton, loadPresetButton, removePresetButton;
        private OpCheckBox fillLineageCheck, forceFreshCheck, balancedSpawnsCheck;
        private OpTextBox presetText;
        private OpComboBox presetsComboBox;
        private UIelement[] UIFixed;
        private Dictionary<string, OpUpdown> OpUpDownRefs;

        public OptionConfigs OpConfigs { get; private set; }


        public ApexUpYourSpawnsOptions()
        {
            Debug.Log("OPTIONS CTOR.");
            OpUpDownRefs = new Dictionary<string, OpUpdown>();

            fillLineages = this.config.Bind<bool>("FillLineages", false);
            forceFreshSpawns = this.config.Bind<bool>("ForceFreshSpawns", false);
            balancedSpawns = this.config.Bind<bool>("BalancedSpawns", true);

            balancedSpawns.OnChange += () => ApexUtils.BalancedSpawns = balancedSpawns.Value;
            fillLineages.OnChange += () => ApexUtils.FillLineages = fillLineages.Value;
            forceFreshSpawns.OnChange += () => ApexUtils.ForceFreshSpawns = forceFreshSpawns.Value;

            ConfigurableInfo info = null;
            comboBoxConfig = this.config.Bind<string>("PresetComboBox", "Default", info);
            presetTextConfig = this.config.Bind<string>(null, "", info);

            OpConfigs = new OptionConfigs(config);
            //Set the base game configs
            OpConfigs.UpdateBindings();
        }

        public override void Initialize()
        {
            // === CREATE BASE UI ELEMENTS
            OpUpDownRefs.Clear();
            List<ListItem> boxList = new List<ListItem>();
            ListItem def = new ListItem("Default");
            ListItem nulls = new ListItem("Nulls");

            boxList.Add(def);
            boxList.Add(nulls);

            if (Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
            {
                foreach (string filename in Directory.GetFiles(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
                {
                    string[] splitted = filename.Split('\\');
                    string aloneName = splitted[splitted.Length - 1].Split('.')[0];
                    boxList.Add(new ListItem(aloneName));
                }
            }
            else
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns");

            if(!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates");


            Vector2 presetsPos = new Vector2(220, 535);

            presetsComboBox = new OpComboBox(comboBoxConfig, presetsPos, 90, boxList);
            presetsPos.y += 32;
            presetText = new OpTextBox(presetTextConfig, presetsPos, 90f);

            presetsPos.x -= 95f;
            presetsPos.y -= 2f;

            savePresetButton = new OpSimpleButton(presetsPos, new Vector2(88, 28), "SAVE PRESET")
            {
                description = "Save preset (if text is empty, saves currently selected one)"
            };

            presetsPos.y -= 32f;
            loadPresetButton = new OpSimpleButton(presetsPos, new Vector2(88, 28), "LOAD PRESET")
            {
                description = "Load selected preset"
            };

            presetsPos.y += 16;
            presetsPos.x += 204;
            removePresetButton = new OpSimpleButton(presetsPos, new Vector2(108, 30), "REMOVE PRESET")
            {
                description = "Remove selected preset"
            };
            loadPresetButton.OnClick += LoadPreset;
            savePresetButton.OnClick += SavePreset;
            removePresetButton.OnClick += RemovePreset;

            fillLineageCheck = new OpCheckBox(fillLineages, new Vector2(160f, 13f))
            {
                description = "Empty slots in lineages will be filled, so every lineage will have a creature every cycle."
            };

            balancedSpawnsCheck = new OpCheckBox(balancedSpawns, new Vector2(262f, 13f))
            {
                description = "To balance creature density, certain configs will behave differently in certain regions."
            };

            forceFreshCheck = new OpCheckBox(forceFreshSpawns, new Vector2(394f, 13f))
            {
                description = "Spawns will be reset and randomized every cycle."
            };


            replacementDescription = new OpSimpleButton(new Vector2(137, 483), new Vector2(16, 16), "i")
            {
                description = "% chance to replace a creature with another.\n" + 
                "*: Creature is not from a spawner. Replacements vary between cycles."
            };
            extrasDescription = new OpSimpleButton(new Vector2(457, 483), new Vector2(16, 16), "i")
            {
                description = "Increase the creature amount of each den by random value from 0 to X.\n" + 
                "Does not apply to lineages."
            };

            replacementDescription2 = new OpSimpleButton(new Vector2(167, 483), new Vector2(16, 16), "i")
            {
                description = "Den: Each DEN has a chance to be replaced instead of each individual creature.\n" +
                    "Inv: Invasion. Original creature is not replaced."
            };
            extrasDescription2 = new OpSimpleButton(new Vector2(487, 483), new Vector2(16, 16), "i")
            {
                description = "\\10: Value is divided by 10. Decimals determine chance of one extra addition.\n" +
                    "Example: 11 becomes 1.1 => 0, 1 or 2 extras; but 2 is very unlikely."
            };

            UIFixed = new UIelement[]
            {
                new OpLabel(10f, 524f, "Options", true),
                new OpLabel(-2f, 483f, "Replacements", true),
                new OpLabel(325f, 483f, "Extra spawns", true),
                replacementDescription,
                extrasDescription,
                replacementDescription2,
                extrasDescription2,
                fillLineageCheck,
                new OpLabel(187f, 16f, "Fill Lineages"),
                balancedSpawnsCheck,
                new OpLabel(289f, 16f, "Balanced spawns"),
                forceFreshCheck,
                new OpLabel(421f, 16f, "Randomize every cycle"),
                presetsComboBox,
                presetText,
                loadPresetButton,
                savePresetButton,
                removePresetButton
            };

            //TABS
            List<OpTab> tabs = new List<OpTab>();

            //VANILLA TAB

            Configurable<int>[] UIVanillaReplacementConfigs = OpConfigs.VanillaRepConfigs.Values.ToArray(); //This.

            Configurable<int>[] UIVanillaExtraConfigs = OpConfigs.VanillaExtraConfigs.Values.ToArray();

            OpTab vanillaTab = CreateTab(UIVanillaReplacementConfigs, UIVanillaExtraConfigs, " Vanilla");
            tabs.Add(vanillaTab);

            //MSC / DLC TAB
            if(ModManager.MSC || ModManager.Watcher)
            {
                Configurable<int>[] UIDLCExtraConfigs = OpConfigs.DLCExtraConfigs.Values.ToArray();

                Configurable<int>[] UIDLCReplacementConfigs = OpConfigs.DLCRepConfigs.Values.ToArray();

                OpTab mscTab = CreateTab(UIDLCReplacementConfigs, UIDLCExtraConfigs, ModManager.MSC ? " More Slugcats" : " DLC Expansion");
                tabs.Add(mscTab);
            }

            //WATCHER TAB
            if (ModManager.Watcher)
            {
                Configurable<int>[] UIWatcherRepConfigs = OpConfigs.WatcherRepConfigs.Values.ToArray();

                Configurable<int>[] UIWatcherExtraConfigs = OpConfigs.WatcherExtraConfigs.Values.ToArray();

                OpTab watcherTab = CreateTab(UIWatcherRepConfigs, UIWatcherExtraConfigs, " The Watcher");
                tabs.Add(watcherTab);
            }
            
            //MODS TAB
            //Set the mod configs
            List<Configurable<int>> enabledModsRepConfigs = OpConfigs.ModRepConfigs.Values.ToList();
            List<Configurable<int>> enabledModsExtraConfigs = OpConfigs.ModExtraConfigs.Values.ToList();

            if(enabledModsExtraConfigs.Count > 0 || enabledModsRepConfigs.Count > 0)
            {
                OpTab modsTab = CreateTab(enabledModsRepConfigs.ToArray(), enabledModsExtraConfigs.ToArray(), " Mods");
                tabs.Add(modsTab);
            }

            //END

            this.Tabs = tabs.ToArray();

            enabledModsRepConfigs.Clear();
            enabledModsExtraConfigs.Clear();

            FillLineages = fillLineages.Value;
            ForceFreshSpawns = forceFreshSpawns.Value;
            BalancedSpawns = balancedSpawns.Value;
            
        }

        private OpTab CreateTab(Configurable<int>[] replacementConfigs, Configurable<int>[] extraConfigs, string tabName)
        {
            string auxString;
            float scrollBoxSize = 60f + 35f * (Mathf.Max(replacementConfigs.Length, extraConfigs.Length));
            int replaceLength = replacementConfigs.Length * 2;
            int extraLength = extraConfigs.Length * 2;

            UIelement[] totalElements = new UIelement[extraLength + replaceLength];

            //Replacements
            for (int i = 0; i < replacementConfigs.Length; ++i)
            {
                OpConfigs.LabelsMap.TryGetValue(replacementConfigs[i], out auxString);
                totalElements[i * 2] = new OpLabel(80f, scrollBoxSize - 30f - (35f * i), auxString);
                totalElements[i * 2 + 1] = new OpUpdown(replacementConfigs[i], new Vector2(10f, scrollBoxSize - 35f - (35f * i)), 60f);
            }

            //Extras
            for (int i = 0; i < extraConfigs.Length; ++i)
            {
                OpConfigs.LabelsMap.TryGetValue(extraConfigs[i], out auxString);
                totalElements[replaceLength + i * 2] = new OpLabel(400f, scrollBoxSize - 30f - (35f * i), auxString);
                totalElements[replaceLength + i * 2 + 1] = new OpUpdown(extraConfigs[i], new Vector2(330f, scrollBoxSize - 35f - (35f * i)), 60f);
            }


            OpScrollBox scrollBox = new OpScrollBox(new Vector2(0f, 55f), new Vector2(580f, 420f), scrollBoxSize, false, false, true);
            OpTab tab = new OpTab(this, tabName);
            tab._AddItem(scrollBox);
            scrollBox.AddItems(totalElements);
            foreach (UIelement u in tab.items)
                if (u is OpUpdown op)
                    OpUpDownRefs.Add(op.Key, op);

            
            tab.OnPreActivate += () => {
                tab.AddItems(UIFixed);
            };
            //NOTE: Do not remove items on PostDeactivate, it causes reload bug. (The game does it automatically)
            return tab;
        }

        public void UpdateAllBindings()
        {
            UnityEngine.Debug.Log("UPDATING ALL BINDINGS.");
            OpConfigs.UpdateBindings();
            OpConfigs.UpdateModBindings();
        }
        public void UpdateModBindings()
        {
            OpConfigs.UpdateModBindings();
        }

        #region Presets
        private void SavePreset(UIfocusable trigger)
        {
            string presetName = presetText.value;
            if (presetName == "" || presetName is null)
                presetName = presetsComboBox.GetItemList()[presetsComboBox.GetIndex()].name;
            if (presetName == "Nulls" || presetName == "Default")
                return;

            if (!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns");

            string filePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/" + presetName + ".txt";
            Dictionary<string, int> valuesToSave = new Dictionary<string, int>();
            bool alreadyExists;
            if (alreadyExists = File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                string line = sr.ReadLine();
                while (line != null)
                {
                    string[] splits = line.Split('|');
                    valuesToSave.Add(splits[0], Int32.Parse(splits[1]));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
                
            foreach(KeyValuePair<string, OpUpdown> pair in OpUpDownRefs)
            {
                if (valuesToSave.ContainsKey(pair.Key))
                    valuesToSave[pair.Key] = Int32.Parse(pair.Value.value);
                else
                    valuesToSave.Add(pair.Key, Int32.Parse(pair.Value.value));
            }

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                foreach (KeyValuePair<string, int> pair in valuesToSave)
                    writer.WriteLine(pair.Key + "|" + pair.Value);
            }

            if (!alreadyExists)
                presetsComboBox.AddItems(false, new ListItem(presetName));
            presetText.Reset();
        }

        private void LoadPreset(UIfocusable trigger)
        {
            string presetName = presetsComboBox.GetItemList()[presetsComboBox.GetIndex()].name;
            if (presetName == "Nulls")
                SetNulls();
            else if (presetName == "Default")
                SetDefaults();
            else
            {
                StreamReader sr = new StreamReader(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/" + presetName + ".txt");
                string line = sr.ReadLine();
                while(line != null)
                {
                    string[] splits = line.Split('|');
                    if(OpUpDownRefs.ContainsKey(splits[0]))
                        SetValue(OpUpDownRefs[splits[0]], Int32.Parse(splits[1]));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }

        private void RemovePreset(UIfocusable trigger)
        {
            string presetName = presetsComboBox.GetItemList()[presetsComboBox.GetIndex()].name;
            if (presetName == "Nulls" || presetName == "Default")
                return;
            presetsComboBox.RemoveItems(true, presetName);
            File.Delete(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/" + presetName + ".txt");
        }

        private void SetDefaults()
        {
            foreach(OpUpdown op in OpUpDownRefs.Values)
                op.Reset();
        }

        private void SetNulls()
        {
            string aux;
            foreach (OpUpdown op in OpUpDownRefs.Values)
            {
                aux = op.defaultValue;
                op.defaultValue = "0";
                op.Reset();
                op.defaultValue = aux;
            }
        }

        private void SetValue(OpUpdown op, int value)
        {
            string aux = op.defaultValue;
            op.defaultValue = value.ToString();
            op.Reset();
            op.defaultValue = aux;
        }

        #endregion
    }
}
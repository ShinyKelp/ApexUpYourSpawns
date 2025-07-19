using BepInEx.Logging;
using Menu.Remix.MixedUI;
using RWCustom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ApexUpYourSpawns
{
    public class ApexUpYourSpawnsOptions : OptionInterface
    {
        private readonly ManualLogSource Logger;

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
        private Dictionary<string, OpUpdown> optionsRefs;

        private ApexUpYourSpawnsMod apexMod;
        private bool appliedSharedDLC = false;
        private bool appliedMSC = false;
        private bool appliedWatcher = false;

        Dictionary<Configurable<int>, string> labelsMap = new Dictionary<Configurable<int>, string>();

        Dictionary<string, Configurable<int>> VanillaRepConfigs = new Dictionary<string, Configurable<int>>();
        Dictionary<string, Configurable<int>> VanillaExtraConfigs = new Dictionary<string, Configurable<int>>();
        Dictionary<string, Configurable<int>> DLCRepConfigs = new Dictionary<string, Configurable<int>>();
        Dictionary<string, Configurable<int>> DLCExtraConfigs = new Dictionary<string, Configurable<int>>();
        Dictionary<string, Configurable<int>> WatcherRepConfigs = new Dictionary<string, Configurable<int>>();
        Dictionary<string, Configurable<int>> WatcherExtraConfigs = new Dictionary<string, Configurable<int>>();
        Dictionary<string,Configurable<int>> ModRepConfigs = new Dictionary<string, Configurable<int>>();
        Dictionary<string, Configurable<int>> ModExtraConfigs = new Dictionary<string, Configurable<int>>();

        public ApexUpYourSpawnsOptions(ApexUpYourSpawnsMod modInstance, ManualLogSource loggerSource)
        {
            Logger = loggerSource;
            apexMod = modInstance;

            optionsRefs = new Dictionary<string, OpUpdown>();

            fillLineages = this.config.Bind<bool>("FillLineages", false);
            forceFreshSpawns = this.config.Bind<bool>("ForceFreshSpawns", false);
            balancedSpawns = this.config.Bind<bool>("BalancedSpawns", true);

            ConfigurableInfo info = null;
            comboBoxConfig = this.config.Bind<string>("PresetComboBox", "Default", info);
            presetTextConfig = this.config.Bind<string>(null, "", info);


            // Vanilla configs
            // Replacements
            AddConfigOption(VanillaRepConfigs, "RedLizardChance", 6, "Lizard > Red Lizard");
            AddConfigOption(VanillaRepConfigs, "LargeCentipedeChance", 0, "Small Cent > Large Centipede");
            AddConfigOption(VanillaRepConfigs, "RedCentipedeChance", 8, "Large Cent > Red Centipede");
            AddConfigOption(VanillaRepConfigs, "KingVultureChance", 10, "Vulture > King Vulture");
            AddConfigOption(VanillaRepConfigs, "SpitterSpiderChance", 15, "Big Spider > Spitter Spider");
            AddConfigOption(VanillaRepConfigs, "CicadaCentiwingChance", 12, "Cicada > Centiwing"); // original string was "FlyingPredatorChance"
            AddConfigOption(VanillaRepConfigs, "BrotherLongLegsChance", 4, "L.Mice/Snail/??? > LongLegs (Den)");
            AddConfigOption(VanillaRepConfigs, "DaddyLongLegsChance", 10, "Brother > DaddyLongLegs");
            AddConfigOption(VanillaRepConfigs, "LeechLizardChance", 10, "Leeches > Aquatic Lizard (Den)");
            AddConfigOption(VanillaRepConfigs, "CyanLizChance", 0, "Blue > Cyan Lizard");
            AddConfigOption(VanillaRepConfigs, "TubeWormSpiderInv", 20, "Grappleworm > Big Spider (Inv)");
            AddConfigOption(VanillaRepConfigs, "JetfishSalamanderChance", 6, "Jetfish > Salamander");
            AddConfigOption(VanillaRepConfigs, "CicadaNoodleFlyChance", 4, "Cicada > Noodlefly"); // original string was "CicadaNoodleflyChance"
            AddConfigOption(VanillaRepConfigs, "MonsterKelpChance", 5, "Pole Plant > Monster Kelp");

            // Extras
            AddConfigOption(VanillaExtraConfigs, "GreenLizExtras", 4, "Green Lizards (/10)");
            AddConfigOption(VanillaExtraConfigs, "BlueLizExtras", 6, "Blue Lizards (/10)");
            AddConfigOption(VanillaExtraConfigs, "PinkLizExtras", 8, "Pink Lizards (/10)");
            AddConfigOption(VanillaExtraConfigs, "WhiteLizExtras", 4, "White Lizards (/10)");
            AddConfigOption(VanillaExtraConfigs, "BlackLizExtras", 4, "Black Lizards (/10)");
            AddConfigOption(VanillaExtraConfigs, "YellowLizExtras", 25, "Yellow Lizards (/10)");
            AddConfigOption(VanillaExtraConfigs, "SalExtras", 12, "Salamanders (/10)");
            AddConfigOption(VanillaExtraConfigs, "CyanLizExtras", 2, "Cyan Lizards (/10)");
            AddConfigOption(VanillaExtraConfigs, "PrecycleCreatureExtras", 10, "Shelter Failure Spawns (/10)");
            AddConfigOption(VanillaExtraConfigs, "ScavengerExtras", 4, "Scavengers", false);
            AddConfigOption(VanillaExtraConfigs, "VultureExtras", 1, "Vultures", false);
            AddConfigOption(VanillaExtraConfigs, "CentipedeExtras", 3, "Centipedes (/10)");
            AddConfigOption(VanillaExtraConfigs, "CentiWingExtras", 0, "Centiwings (/10)");
            AddConfigOption(VanillaExtraConfigs, "BigSpiderExtras", 25, "Big Spiders (/10)");
            AddConfigOption(VanillaExtraConfigs, "DropwigExtras", 10, "Dropwigs (/10)");
            AddConfigOption(VanillaExtraConfigs, "EggbugExtras", 10, "Eggbugs (/10)");
            AddConfigOption(VanillaExtraConfigs, "CicadaExtras", 10, "Cicadas (/10)");
            AddConfigOption(VanillaExtraConfigs, "SnailExtras", 3, "Snails (/10)");
            AddConfigOption(VanillaExtraConfigs, "JetfishExtras", 6, "Jetfish (/10)");
            AddConfigOption(VanillaExtraConfigs, "LmiceExtras", 3, "Lantern Mice (/10)");
            AddConfigOption(VanillaExtraConfigs, "SmallCentExtras", 8, "Small Centipedes (/10)");
            AddConfigOption(VanillaExtraConfigs, "MirosExtras", 2, "Miros Birds", false);
            AddConfigOption(VanillaExtraConfigs, "SpiderExtras", 0, "Small Spiders", false);
            AddConfigOption(VanillaExtraConfigs, "LeechExtras", 0, "Leeches", false);
            AddConfigOption(VanillaExtraConfigs, "TubeWormExtras", 4, "Grappleworms (/10)", false);
            AddConfigOption(VanillaExtraConfigs, "KelpExtras", 6, "Monster Kelp (/10)");
            AddConfigOption(VanillaExtraConfigs, "LeviathanExtras", 4, "Leviathans (/10)");
            AddConfigOption(VanillaExtraConfigs, "NightCreatureExtras", 10, "Night Creatures (/10)");
            AddConfigOption(VanillaExtraConfigs, "DeerExtras", 2, "Deer", false);

        }

        private void SetExpansionBindings()
        {   
            // Shared DLC
            if ((ModManager.Watcher || ModManager.MSC) && !appliedSharedDLC)
            {
                appliedSharedDLC = true;
                AddConfigOption(DLCRepConfigs, "MirosVultureChance", 15, "Vultures > Miros Vulture");
                AddConfigOption(DLCRepConfigs, "EliteScavengerChance", 12, "Scavenger > Elite Scavenger");
                AddConfigOption(DLCRepConfigs, "TerrorLongLegsChance", 10, "Daddy > MotherLongLegs"); // originally "MotherLongLegsChance"
                AddConfigOption(DLCRepConfigs, "SeaLeechAquapedeChance", 10, "Sea Leeches > Aquapede (Den)");
                AddConfigOption(DLCRepConfigs, "JetfishAquapedeChance", 20, "Jetfish > Aquapede"); // originally "AquapedeChance"
                AddConfigOption(DLCRepConfigs, "YeekLizardChance", 10, "Yeek > Caramel/Strawberry Liz");
                AddConfigOption(DLCRepConfigs, "CaramelLizChance", 15, "Green > Caramel Lizard");
                AddConfigOption(DLCRepConfigs, "StrawberryLizChance", 5, "Pink > Strawberry Lizard");
                AddConfigOption(DLCRepConfigs, "EelLizChance", 10, "Salamander > Eel Lizard");
                AddConfigOption(DLCRepConfigs, "JungleLeechChance", 0, "Leech > Jungle Leech");
                AddConfigOption(DLCRepConfigs, "MotherSpiderChance", 10, "Small Spiders > Mother Spider (Den)");
                AddConfigOption(DLCRepConfigs, "StowawayChance", 3, "Ceiling Fruits > Stowawaybug Trap (*)");
                AddConfigOption(DLCRepConfigs, "GiantJellyfishChance", 10, "Jellyfish > Giant Jellyfish (*)");

                AddConfigOption(DLCExtraConfigs, "CaramelLizExtras", 4, "Caramel Lizards (/10)");
                AddConfigOption(DLCExtraConfigs, "ZoopLizExtras", 8, "Strawberry Lizards (/10)");
                AddConfigOption(DLCExtraConfigs, "EelLizExtras", 4, "Eel Lizards (/10)");
                AddConfigOption(DLCExtraConfigs, "AquapedeExtras", 5, "Aquapedes (/10)");
                AddConfigOption(DLCExtraConfigs, "YeekExtras", 8, "Yeeks (/10)");
            }

            // MSC
            if (ModManager.MSC && !appliedMSC)
            {
                appliedMSC = true;
                AddConfigOption(DLCRepConfigs, "TrainLizardChance", 10, "Red Lizard > Train Lizard");
                AddConfigOption(DLCRepConfigs, "FireBugChance", 30, "Eggbug > Firebug");
                AddConfigOption(DLCRepConfigs, "HunterLongLegsChance", 20, "Slugpup > HunterLongLegs (*)");
                AddConfigOption(DLCRepConfigs, "KingScavengerChance", 5, "Elite > King Scavenger (*)");
            }

            if(ModManager.MSC && apexMod.activeMods.Contains("ShinyKelp.ScavengerTweaks"))
                AddConfigOption(DLCRepConfigs, "KingScavengerChance", 5, "Elite > King Scavenger (*)");

            // Watcher
            if (ModManager.Watcher && !appliedWatcher)
            {
                appliedWatcher = true;
                AddConfigOption(WatcherRepConfigs, "ScavengerDiscipleChance", 4, "Scavenger > Disciple");
                AddConfigOption(WatcherRepConfigs, "ScavengerTemplarChance", 10, "Scavenger > Templar");
                AddConfigOption(WatcherRepConfigs, "BlizzardLizardChance", 2, "Lizards > Blizzard");
                AddConfigOption(WatcherRepConfigs, "MirosLoachChance", 20, "Miros Bird > Loach (Den)");
                AddConfigOption(WatcherRepConfigs, "DeerLoachInvChance", 5, "Deer > Loach (Inv)");
                AddConfigOption(WatcherRepConfigs, "LoachMirosChance", 0, "Loach > Miros Bird (Den)");
                AddConfigOption(WatcherRepConfigs, "RotLoachChance", 10, "Loach > Rot Loach");
                AddConfigOption(WatcherRepConfigs, "VultureBigMothChance", 12, "Vulture > Big Moth");
                AddConfigOption(WatcherRepConfigs, "BigMothVultureChance", 0, "Big Moth > Vulture");
                AddConfigOption(WatcherRepConfigs, "CicadaSmallMothChance", 15, "Cicada > Small Moth");
                AddConfigOption(WatcherRepConfigs, "SmallMothCicadaChance", 15, "Small Moth > Cicada");
                AddConfigOption(WatcherRepConfigs, "SmallMothNoodleflyChance", 0, "Small Moth > Noodlefly");
                AddConfigOption(WatcherRepConfigs, "SmallMothCentiwingChance", 0, "Small Moth > Centiwing");
                AddConfigOption(WatcherRepConfigs, "DeerSkywhaleChance", 15, "Deer > Skywhale");
                AddConfigOption(WatcherRepConfigs, "SnailBarnacleChance", 30, "Snail > Barnacle");
                AddConfigOption(WatcherRepConfigs, "BarnacleSnailChance", 8, "Barnacle > Snail");
                AddConfigOption(WatcherRepConfigs, "BlackBasiliskLizChance", 10, "Black Lizard > Basilisk Lizard");
                AddConfigOption(WatcherRepConfigs, "GroundIndigoLizChance", 0, "Grounded Lizards > Indigo lizard");
                AddConfigOption(WatcherRepConfigs, "DrillCrabMirosChance", 0, "Drill Crab > Miros Bird (Den)");
                AddConfigOption(WatcherRepConfigs, "MirosDrillCrabChance", 25, "Miros Bird > Drill Crab (Den)");
                AddConfigOption(WatcherRepConfigs, "DrillCrabLoachChance", 0, "Drill Crab > Loach (Den)");
                AddConfigOption(WatcherRepConfigs, "LoachDrillCrabChance", 0, "Loach > Drill Crab (Den)");
                AddConfigOption(WatcherRepConfigs, "DeerDrillCrabInvChance", 18, "Deer > Drill Crab (Inv)"); // originally "DeerDrillCrabChance"
                AddConfigOption(WatcherRepConfigs, "LeechFrogChance", 25, "Leeches > Frog (Den)");
                AddConfigOption(WatcherRepConfigs, "MouseRatChance", 33, "Lantern Mouse > Rat");
                AddConfigOption(WatcherRepConfigs, "GrubSandGrubChance", 20, "Vulture Grub > Sand Grub (*)");
                AddConfigOption(WatcherRepConfigs, "PopcornSandWormTrapChance", 10, "Popcorn Plant > Sand Worm Trap (*)");
                AddConfigOption(WatcherRepConfigs, "HazerTardigradeChance", 30, "Hazer > Tardigrade (*)");
                AddConfigOption(WatcherRepConfigs, "PearlFireSpriteChance", 10, "Pearl > FireSprite (*)");

                AddConfigOption(WatcherExtraConfigs, "LoachExtras", 2, "Loaches", false);
                AddConfigOption(WatcherExtraConfigs, "BigMothExtras", 3, "Big Moths");
                AddConfigOption(WatcherExtraConfigs, "SmallMothExtras", 6, "Small Moths (/10)");
                AddConfigOption(WatcherExtraConfigs, "BasiliskLizExtras", 4, "Basilisk Lizards (/10)");
                AddConfigOption(WatcherExtraConfigs, "IndigoLizExtras", 8, "Indigo Lizards  (/10)");
                AddConfigOption(WatcherExtraConfigs, "BarnacleExtras", 0, "Barnacles  (/10)");
                AddConfigOption(WatcherExtraConfigs, "DrillCrabExtras", 3, "Drill Crabs", false);
                AddConfigOption(WatcherExtraConfigs, "SkywhaleExtras", 2, "Skywhales", false);
                AddConfigOption(WatcherExtraConfigs, "FrogExtras", 5, "Frogs (/10)");
                AddConfigOption(WatcherExtraConfigs, "RatExtras", 0, "Rats (/10)");
            }
        }

        public override void Initialize()
        {
            // === CREATE BASE UI ELEMENTS
            optionsRefs.Clear();
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

            //Set the base game configs
            SetExpansionBindings();

            //TABS
            List<OpTab> tabs = new List<OpTab>();

            //VANILLA TAB

            Configurable<int>[] UIVanillaReplacementConfigs = VanillaRepConfigs.Values.ToArray(); //This.

            Configurable<int>[] UIVanillaExtraConfigs = VanillaExtraConfigs.Values.ToArray();

            OpTab vanillaTab = CreateTab(UIVanillaReplacementConfigs, UIVanillaExtraConfigs, labelsMap, " Vanilla");
            tabs.Add(vanillaTab);

            //MSC / DLC TAB
            if(ModManager.MSC || ModManager.Watcher)
            {
                Configurable<int>[] UIDLCExtraConfigs = DLCExtraConfigs.Values.ToArray();

                Configurable<int>[] UIDLCReplacementConfigs = DLCRepConfigs.Values.ToArray();

                OpTab mscTab = CreateTab(UIDLCReplacementConfigs, UIDLCExtraConfigs, labelsMap, ModManager.MSC ? " More Slugcats" : " DLC Expansion");
                tabs.Add(mscTab);
            }

            //WATCHER TAB
            if (ModManager.Watcher)
            {
                Configurable<int>[] UIWatcherRepConfigs = WatcherRepConfigs.Values.ToArray();

                Configurable<int>[] UIWatcherExtraConfigs = WatcherExtraConfigs.Values.ToArray();

                OpTab watcherTab = CreateTab(UIWatcherRepConfigs, UIWatcherExtraConfigs, labelsMap, " The Watcher");
                tabs.Add(watcherTab);
            }
            
            //MODS TAB
            //Set the mod configs
            List<Configurable<int>> enabledModsRepConfigs = ModRepConfigs.Values.ToList();
            List<Configurable<int>> enabledModsExtraConfigs = ModExtraConfigs.Values.ToList();

            if(enabledModsExtraConfigs.Count > 0 || enabledModsRepConfigs.Count > 0)
            {
                OpTab modsTab = CreateTab(enabledModsRepConfigs.ToArray(), enabledModsExtraConfigs.ToArray(), labelsMap, " Mods");
                tabs.Add(modsTab);
            }

            //END

            this.Tabs = tabs.ToArray();

            enabledModsRepConfigs.Clear();
            enabledModsExtraConfigs.Clear();
            
        }

        private OpTab CreateTab(Configurable<int>[] replacementConfigs, Configurable<int>[] extraConfigs, Dictionary<Configurable<int>, string> labelsMap, string tabName)
        {
            string auxString;
            float scrollBoxSize = 60f + 35f * (Mathf.Max(replacementConfigs.Length, extraConfigs.Length));
            int replaceLength = replacementConfigs.Length * 2;
            int extraLength = extraConfigs.Length * 2;

            UIelement[] totalElements = new UIelement[extraLength + replaceLength];

            //Replacements
            for (int i = 0; i < replacementConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(replacementConfigs[i], out auxString);
                totalElements[i * 2] = new OpLabel(80f, scrollBoxSize - 30f - (35f * i), auxString);
                totalElements[i * 2 + 1] = new OpUpdown(replacementConfigs[i], new Vector2(10f, scrollBoxSize - 35f - (35f * i)), 60f);
            }

            //Extras
            for (int i = 0; i < extraConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(extraConfigs[i], out auxString);
                totalElements[replaceLength + i * 2] = new OpLabel(400f, scrollBoxSize - 30f - (35f * i), auxString);
                totalElements[replaceLength + i * 2 + 1] = new OpUpdown(extraConfigs[i], new Vector2(330f, scrollBoxSize - 35f - (35f * i)), 60f);
            }


            OpScrollBox scrollBox = new OpScrollBox(new Vector2(0f, 55f), new Vector2(580f, 420f), scrollBoxSize, false, false, true);
            OpTab tab = new OpTab(this, tabName);
            tab._AddItem(scrollBox);
            scrollBox.AddItems(totalElements);
            foreach (UIelement u in tab.items)
                if (u is OpUpdown op)
                    optionsRefs.Add(op.Key, op);

            
            tab.OnPreActivate += () => {
                tab.AddItems(UIFixed);
            };
            //NOTE: Do not remove items on PostDeactivate, it causes reload bug. (The game does it automatically)
            return tab;
        }

        public void InitModConfigs()
        {
            HashSet<string> activeMods = apexMod.activeMods;

            if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                AddConfigOption(ModRepConfigs, "SporantulaChance", 4, "Small Insects > Sporantula (Inv)");
                AddConfigOption(ModExtraConfigs, "SporantulaExtras", 25, "Sporantulas (/10)");
                AddConfigOption(ModRepConfigs, "ScutigeraChance", 15, "Centipede > Scutigera");
                AddConfigOption(ModExtraConfigs, "ScutigeraExtras", 0, "Scutigeras (/10)");
                AddConfigOption(ModRepConfigs, "RedRedHorrorCentiChance", 10, "Red Centipede > Red Horror Centi");
                AddConfigOption(ModRepConfigs, "WingRedHorrorCentiChance", 4, "Centiwing > Red Horror Centi");
                AddConfigOption(ModRepConfigs, "WaterSpitterChance", 10, "Aquatic Lizards > Water Spitter");
                AddConfigOption(ModExtraConfigs, "WaterSpitterExtras", 0, "Water Spitters (/10)");
                AddConfigOption(ModRepConfigs, "FatFireFlyChance", 10, "Vultures > Fat Firefly");
                AddConfigOption(ModRepConfigs, "SurfaceSwimmerChance", 20, "EggBug > Surface Swimmer");
                AddConfigOption(ModExtraConfigs, "SurfaceSwimmerExtras", 5, "Surface Swimmer");
                AddConfigOption(ModRepConfigs, "BounceBallChance", 10, "Snail > Bouncing Ball");
                AddConfigOption(ModExtraConfigs, "BounceBallExtras", 10, "Bouncing Ball");
                AddConfigOption(ModRepConfigs, "CritterHoverflyChance", 7, "Critters > Hoverfly (Inv)");
                AddConfigOption(ModExtraConfigs, "HoverflyExtras", 15, "Hoverfly (/10)");
                AddConfigOption(ModRepConfigs, "NoodleEaterChance", 10, "Noodlefly > Noodle Eater (Inv)");
                AddConfigOption(ModExtraConfigs, "NoodleEaterExtras", 6, "Noodle Eater (/10)", false);
                AddConfigOption(ModRepConfigs, "ThornbugChance", 20, "Eggbug > Thornbug (Inv)");
                AddConfigOption(ModExtraConfigs, "ThornbugExtras", 4, "Thornbug (/10)", false);
                AddConfigOption(ModRepConfigs, "MiniLeviathanChance", 25, "Leviathan > Mini Leviathan (Inv)");
                AddConfigOption(ModExtraConfigs, "MiniLeviathanExtras", 3, "Mini Leviathan", false);
                AddConfigOption(ModRepConfigs, "PolliwogChance", 10, "Salamander > Polliwog");
                AddConfigOption(ModExtraConfigs, "PolliwogExtras", 8, "Polliwog (/10)");
                AddConfigOption(ModRepConfigs, "HunterSeekerCyanChance", 6, "Cyan Liz > Hunter Seeker");
                AddConfigOption(ModRepConfigs, "HunterSeekerWhiteChance", 6, "White Liz > Hunter Seeker");
                AddConfigOption(ModExtraConfigs, "HunterSeekerExtras", 2, "Hunter Seeker (/10)");
                AddConfigOption(ModRepConfigs, "SilverLizChance", 15, "Grounded Lizards > Silver Liz");
                AddConfigOption(ModExtraConfigs, "SilverLizExtras", 2, "Silver Lizard (/10)");
                AddConfigOption(ModRepConfigs, "VultureEchoLeviChance", 10, "Vultures > Echo Leviathan (Den)");
                AddConfigOption(ModExtraConfigs, "EchoLeviExtras", 0, "Echo Leviathans (/10)");
                AddConfigOption(ModRepConfigs, "BlizzorChance", 7, "Miros Bird > Blizzor");
                AddConfigOption(ModRepConfigs, "SalamanderSalamoleChance", 5, "Salamander > Mole Salamander");
                AddConfigOption(ModRepConfigs, "BlackSalamoleChance", 5, "Black liz > Mole Salamander");
                AddConfigOption(ModExtraConfigs, "BlizzorExtras", 2, "Blizzor", false);
                AddConfigOption(ModExtraConfigs, "SalamoleExtras", 10, "Mole Salamander (/10)", false);
            }

            if (activeMods.Contains("ShinyKelp.AngryInspectors"))
            {
                AddConfigOption(ModRepConfigs, "InspectorChance", 8, "LongLegs/??? > Inspector (Inv)");
            }

            if (activeMods.Contains("moredlls"))
            {
                AddConfigOption(ModRepConfigs, "MExplosiveLongLegsChance", 5, "LongLegs > Explosive DLL");
                AddConfigOption(ModRepConfigs, "MZappyLongLegsChance", 5, "LongLegs > Zappy DLL");
            }

            if (activeMods.Contains("ShinyKelp.LizardVariants"))
            {
                AddConfigOption(ModRepConfigs, "MintLizardChance", 10, "Ground Lizards > Mint Lizard");
                AddConfigOption(ModRepConfigs, "RyanLizardChance", 4, "Cyan Lizard > Ryan Lizard");
                AddConfigOption(ModRepConfigs, "YellowLimeLizardChance", 16, "Yellow Lizard > YellowLime Lizard");
                AddConfigOption(ModExtraConfigs, "MintLizardExtras", 4, "Mint Lizards (/10)");
            }

            if (activeMods.Contains("thefriend"))
            {
                AddConfigOption(ModRepConfigs, "MotherLizardChance", 3, "Ground Lizards > Mother Lizard (+ youngs)");
                AddConfigOption(ModExtraConfigs, "YoungLizardExtras", 0, "Young Lizards (/10)");
                AddConfigOption(ModRepConfigs, "LostYoungLizardChance", 5, "Small Lizards > Lost Young Lizard");
                AddConfigOption(ModRepConfigs, "SnowSpiderChance", 10, "Big Spider > Snow Spider");
                AddConfigOption(ModExtraConfigs, "SnowSpiderExtras", 4, "Snow Spiders (/10)");
            }

            if (activeMods.Contains("Outspector"))
            {
                AddConfigOption(ModRepConfigs, "OutspectorChance", 20, "Inspector > Outspector");
                AddConfigOption(ModExtraConfigs, "OutspectorExtras", 12, "Outspectors (/10)");
                if (activeMods.Contains("ShinyKelp.AngryInspectors"))
                    AddConfigOption(ModRepConfigs, "InspectorOutspectorInvChance", 10, "Outspector > Inspector (Inv)");
            }

            if (activeMods.Contains("theincandescent"))
            {
                AddConfigOption(ModRepConfigs, "IcyBlueFreezerInvChance", 5, "Freezer > Icy Blue Liz (Den, Inv)");
                AddConfigOption(ModRepConfigs, "IcyBlueBlueChance", 10, "Blue Liz > Icy Blue Liz (Den)");
                AddConfigOption(ModRepConfigs, "IcyBlueYellowChance", 15, "Yellow Liz > Icy Blue Liz");
                AddConfigOption(ModRepConfigs, "FreezerLizChance", 5, "Icy Blue / Caramel > Freezer Liz (Den)");
                AddConfigOption(ModRepConfigs, "CyanwingChance", 4, "Yellow Centipedes > Cyanwing (Den)");
                AddConfigOption(ModRepConfigs, "WingCyanwingChance", 10, "Centiwing > Cyanwing (Den)");
                AddConfigOption(ModRepConfigs, "JetfishBabyAquapedeChance", 7, "Jetfish > Infant Aquapede (Inv)");
                AddConfigOption(ModRepConfigs, "BabyAquapedeInvChance", 20, "Aquapede > Infant Aquapede (Inv)");
                AddConfigOption(ModExtraConfigs, "BabyAquapedeExtras", 22, "Infant Aquapedes (/10)");
                AddConfigOption(ModExtraConfigs, "IcyBlueLizExtras", 6, "Icy Blue Lizards (/10)");
                AddConfigOption(ModRepConfigs, "AquapedeBabyAquaChance", 5, "Infant Aquapede > Aquapede");
                AddConfigOption(ModRepConfigs, "ChillipedeChance", 10, "Ground Lizards > Chillipede");
            }

            if (activeMods.Contains("Pitch Black") || activeMods.Contains("lurzard.pitchblack"))
            {
                AddConfigOption(ModRepConfigs, "CritterLittleLongLegsChance", 5, "Snail/LMice/??? > Little LongLegs (Inv)");
                AddConfigOption(ModRepConfigs, "BrotherLittleLongLegChance", 20, "Brother > Little LongLegs (Inv)");
                AddConfigOption(ModRepConfigs, "NightTerrorChance", 2, "Centipedes > Night Terror");
                AddConfigOption(ModExtraConfigs, "LittleLongLegsExtras", 5, "Little LongLegs (/10)");
            }

            if (activeMods.Contains("drainmites"))
            {
                AddConfigOption(ModRepConfigs, "DrainMiteChance", 25, "Scavengers > Drain Mites (Den)(Inv)");
                AddConfigOption(ModExtraConfigs, "DrainMiteExtras", 30, "Drain Mites");
            }

            if (activeMods.Contains("Croken.bombardier-vulture"))
            {
                AddConfigOption(ModRepConfigs, "BombVultureChance", 6, "Vultures > Bombardier Vulture");
            }

            if (activeMods.Contains("pkuya.thevanguard"))
            {
                AddConfigOption(ModRepConfigs, "ToxicSpiderChance", 20, "Spitter > Toxic Spider");
            }

            if (activeMods.Contains("myr.moss_fields") || activeMods.Contains("ShinyKelp.Udonfly"))
            {
                AddConfigOption(ModRepConfigs, "FatNootChance", 10, "Noodlefly > Fat Noodlefly");
            }

            if (activeMods.Contains("shrimb.scroungers"))
            {
                AddConfigOption(ModRepConfigs, "ScroungerChance", 10, "Scavenger > Scrounger");
                AddConfigOption(ModExtraConfigs, "ScroungerExtras", 4, "Scroungers", false);
            }

            if (activeMods.Contains("Croken.Mimicstarfish"))
            {
                AddConfigOption(ModRepConfigs, "BllMimicstarfishChance", 7, "BLL > Mimic Starfish");
                AddConfigOption(ModRepConfigs, "CritterMimicstarfishChance", 3, "Aquatic creatures > Mimic Starfish");
            }

            if (activeMods.Contains("ShinyKelp.AlbinoKings"))
            {
                AddConfigOption(ModRepConfigs, "AlbinoVultureChance", 10, "Vultures > Albino Vultures");
            }

            if (activeMods.Contains("bebra.gregtech_lizard"))
            {
                AddConfigOption(ModRepConfigs, "CentiElectricLizChance", 5, "Centipedes > Electric Lizard (Inv)");
                AddConfigOption(ModRepConfigs, "LizardElectricLizChance", 5, "Lizards > Electric Lizard (Inv)");
            }

            if (activeMods.Contains("bry.bubbleweavers"))
            {
                AddConfigOption(ModRepConfigs, "SpiderWeaverChance", 10, "Spitter Spider > Bubble Weaver");
                if (activeMods.Contains("lb-fgf-m4r-ik.swalkins"))
                    AddConfigOption(ModRepConfigs, "SSwimmerWeaverChance", 15, "Surface Swimmer > Spider Weaver (Inv)");
                AddConfigOption(ModExtraConfigs, "BubbleWeaverExtras", 4, "Bubble Weaver (/10)");
            }
        }

        private void AddConfigOption(Dictionary<string, Configurable<int>> dictionary, string name, int defaultVal, string labelDesc = "", bool isCompleteRange = true)
        {
            if (!dictionary.ContainsKey(name))
            {
                Configurable<int> config = this.config.Bind<int>(name, defaultVal, new ConfigAcceptableRange<int>(0, isCompleteRange ? 100 : 20));
                dictionary.Add(name, config);
            }
            if (!labelsMap.ContainsKey(dictionary[name]))
            {
                labelsMap.Add(dictionary[name], labelDesc);
            }
        }

        public int GetConfigValue(string configName)
        {

            if (VanillaRepConfigs.ContainsKey(configName))
                return VanillaRepConfigs[configName].Value;
            else if(VanillaExtraConfigs.ContainsKey(configName))
                return VanillaExtraConfigs[configName].Value;
            else if(DLCRepConfigs.ContainsKey(configName))
                return DLCRepConfigs[configName].Value;
            else if (DLCExtraConfigs.ContainsKey(configName))
                return DLCExtraConfigs[configName].Value;
            else if(WatcherRepConfigs.ContainsKey(configName))
                return WatcherRepConfigs[configName].Value;
            else if(WatcherExtraConfigs.ContainsKey(configName))
                return WatcherExtraConfigs[configName].Value;
            else if (ModRepConfigs.ContainsKey(configName))
                return ModRepConfigs[configName].Value;
            else if (ModExtraConfigs.ContainsKey(configName))
                return ModExtraConfigs[configName].Value;
            else 
                UnityEngine.Debug.Log("--AUYS-- Warning, key not found in options dictionary: " + configName);
            return 0;
        }
        
        public Configurable<int> GetModConfig(string configName)
        {
            if(ModRepConfigs.ContainsKey(configName))
                return ModRepConfigs[configName];
            else if(ModExtraConfigs.ContainsKey(configName))
                return ModExtraConfigs[configName];
            else
                UnityEngine.Debug.Log("--AUYS-- Warning, key not found in modded options dictionary: " + configName);
            return null;
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
                
            foreach(KeyValuePair<string, OpUpdown> pair in optionsRefs)
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
                    if(optionsRefs.ContainsKey(splits[0]))
                        SetValue(optionsRefs[splits[0]], Int32.Parse(splits[1]));
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
            foreach(OpUpdown op in optionsRefs.Values)
                op.Reset();
        }

        private void SetNulls()
        {
            string aux;
            foreach (OpUpdown op in optionsRefs.Values)
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
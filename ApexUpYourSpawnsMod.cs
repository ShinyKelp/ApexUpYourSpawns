﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using Watcher;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ApexUpYourSpawns
{
    [BepInPlugin("ShinyKelp.ApexUpYourSpawns", "ApexUpYourSpawns", "1.5.0")]

    public class ApexUpYourSpawnsMod : BaseUnityPlugin
    {
        #region Vanilla Creature Variables
        private bool ForceFreshSpawns { get { return options.forceFreshSpawns.Value; } }
        private bool FillLineages { get { return options.fillLineages.Value; } }
        private bool BalancedSpawns { get { return options.balancedSpawns.Value; } }

        // Vanilla - Replacements
        float RedLizardChance { get { return (float)options.GetConfigValue("RedLizardChance") / 100; } }
        float LargeCentipedeChance { get { return (float)options.GetConfigValue("LargeCentipedeChance") / 100; } }
        float RedCentipedeChance { get { return (float)options.GetConfigValue("RedCentipedeChance") / 100; } }
        float SpitterSpiderChance { get { return (float)options.GetConfigValue("SpitterSpiderChance") / 100; } }
        float KingVultureChance { get { return (float)options.GetConfigValue("KingVultureChance") / 100; } }
        float BrotherLongLegsChance { get { return (float)options.GetConfigValue("BrotherLongLegsChance") / 100; } }
        float DaddyLongLegsChance { get { return (float)options.GetConfigValue("DaddyLongLegsChance") / 100; } }
        float CicadaCentiwingChance { get { return (float)options.GetConfigValue("CicadaCentiwingChance") / 100; } }
        float LeechLizardChance { get { return (float)options.GetConfigValue("LeechLizardChance") / 100; } }
        float CyanLizChance { get { return (float)options.GetConfigValue("CyanLizChance") / 100; } }
        float TubeWormBigSpiderChance { get { return (float)options.GetConfigValue("TubeWormSpiderInv") / 100; } }
        float JetfishSalamanderChance { get { return (float)options.GetConfigValue("JetfishSalamanderChance") / 100; } }
        float CicadaNoodleflyChance { get { return (float)options.GetConfigValue("CicadaNoodleFlyChance") / 100; } }
        float MonsterKelpChance { get { return (float)options.GetConfigValue("MonsterKelpChance") / 100; } }

        // Vanilla - Extras
        int ExtraGreens { get { return options.GetConfigValue("GreenLizExtras"); } }
        int ExtraPinks { get { return options.GetConfigValue("PinkLizExtras"); } }
        int ExtraBlues { get { return options.GetConfigValue("BlueLizExtras"); } }
        int ExtraWhites { get { return options.GetConfigValue("WhiteLizExtras"); } }
        int ExtraBlacks { get { return options.GetConfigValue("BlackLizExtras"); } }
        int ExtraYellows { get { return options.GetConfigValue("YellowLizExtras"); } }
        int ExtraSals { get { return options.GetConfigValue("SalExtras"); } }
        int ExtraCyans { get { return options.GetConfigValue("CyanLizExtras"); } }
        int ExtraSpiders { get { return options.GetConfigValue("BigSpiderExtras"); } }
        int ExtraVultures { get { return options.GetConfigValue("VultureExtras"); } }
        int ExtraScavengers { get { return options.GetConfigValue("ScavengerExtras"); } }
        int ExtraSmallCents { get { return options.GetConfigValue("SmallCentExtras"); } }
        int ExtraEggbugs { get { return options.GetConfigValue("EggbugExtras"); } }
        int ExtraCicadas { get { return options.GetConfigValue("CicadaExtras"); } }
        int ExtraSnails { get { return options.GetConfigValue("SnailExtras"); } }
        int ExtraJetfish { get { return options.GetConfigValue("JetfishExtras"); } }
        int ExtraLMice { get { return options.GetConfigValue("LmiceExtras"); } }
        int ExtraCentipedes { get { return options.GetConfigValue("CentipedeExtras"); } }
        int ExtraCentiwings { get { return options.GetConfigValue("CentiWingExtras"); } }
        int ExtraPrecycleSals { get { return options.GetConfigValue("PrecycleCreatureExtras"); } }
        int ExtraDropwigs { get { return options.GetConfigValue("DropwigExtras"); } }
        int ExtraMiros { get { return options.GetConfigValue("MirosExtras"); } }
        int ExtraSmallSpiders { get { return options.GetConfigValue("SpiderExtras"); } }
        int ExtraLeeches { get { return options.GetConfigValue("LeechExtras"); } }
        int ExtraTubeworms { get { return options.GetConfigValue("TubeWormExtras"); } }
        int ExtraKelp { get { return options.GetConfigValue("KelpExtras"); } }
        int ExtraLeviathans { get { return options.GetConfigValue("LeviathanExtras"); } }
        int ExtraNightCreatures { get { return options.GetConfigValue("NightCreatureExtras"); } }
        int ExtraDeer { get { return options.GetConfigValue("DeerExtras"); } }

        // MSC - Replacements
        float SeaLeechAquapedeChance { get { return (float)options.GetConfigValue("SeaLeechAquapedeChance") / 100; } }
        float MirosVultureChance { get { return (float)options.GetConfigValue("MirosVultureChance") / 100; } }
        float EliteScavengerChance { get { return (float)options.GetConfigValue("EliteScavengerChance") / 100; } }
        float TerrorLongLegsChance { get { return (float)options.GetConfigValue("TerrorLongLegsChance") / 100; } }
        float YeekLizardChance { get { return (float)options.GetConfigValue("YeekLizardChance") / 100; } }
        float AquapedeChance { get { return (float)options.GetConfigValue("JetfishAquapedeChance") / 100; } }
        float GiantJellyfishChance { get { return (float)options.GetConfigValue("GiantJellyfishChance") / 100; } }
        float CaramelLizChance { get { return (float)options.GetConfigValue("CaramelLizChance") / 100; } }
        float StrawberryLizChance { get { return (float)options.GetConfigValue("StrawberryLizChance") / 100; } }
        float EelLizChance { get { return (float)options.GetConfigValue("EelLizChance") / 100; } }
        float JungleLeechChance { get { return (float)options.GetConfigValue("JungleLeechChance") / 100; } }
        float MotherSpiderChance { get { return (float)options.GetConfigValue("MotherSpiderChance") / 100; } }
        float StowawayChance { get { return (float)options.GetConfigValue("StowawayChance") / 100; } }
        float TrainLizardChance { get { return (float)options.GetConfigValue("TrainLizardChance") / 100; } }
        float FireBugChance { get { return (float)options.GetConfigValue("FireBugChance") / 100; } }
        float KingScavengerChance { get { return (float)options.GetConfigValue("KingScavengerChance") / 100; } }

        // MSC - Extras
        int ExtraCaramels { get { return options.GetConfigValue("CaramelLizExtras"); } }
        int ExtraEellizs { get { return options.GetConfigValue("EelLizExtras"); } }
        int ExtraZoops { get { return options.GetConfigValue("ZoopLizExtras"); } }
        int ExtraYeeks { get { return options.GetConfigValue("YeekExtras"); } }
        int ExtraAquapedes { get { return options.GetConfigValue("AquapedeExtras"); } }

        // Watcher - Replacements
        float ScavengerDiscipleChance { get { return (float)options.GetConfigValue("ScavengerDiscipleChance") / 100; } }
        float ScavengerTemplarChance { get { return (float)options.GetConfigValue("ScavengerTemplarChance") / 100; } }
        float BlizzardLizardChance { get { return (float)options.GetConfigValue("BlizzardLizardChance") / 100; } }
        float MirosLoachChance { get { return (float)options.GetConfigValue("MirosLoachChance") / 100; } }
        float DeerLoachInvChance { get { return (float)options.GetConfigValue("DeerLoachInvChance") / 100; } }
        float LoachMirosChance { get { return (float)options.GetConfigValue("LoachMirosChance") / 100; } }
        float RotLoachChance { get { return (float)options.GetConfigValue("RotLoachChance") / 100; } }
        float VultureBigMothChance { get { return (float)options.GetConfigValue("VultureBigMothChance") / 100; } }
        float BigMothVultureChance { get { return (float)options.GetConfigValue("BigMothVultureChance") / 100; } }
        float CicadaSmallMothChance { get { return (float)options.GetConfigValue("CicadaSmallMothChance") / 100; } }
        float SmallMothCicadaChance { get { return (float)options.GetConfigValue("CicadaSmallMothChance") / 100; } }
        float SmallMothNoodleflyChance { get { return (float)options.GetConfigValue("SmallMothNoodleflyChance") / 100; } }
        float SmallMothCentiwingChance { get { return (float)options.GetConfigValue("SmallMothCentiwingChance") / 100; } }
        float DeerSkywhaleChance { get { return (float)options.GetConfigValue("DeerSkywhaleChance") / 100; } }
        float SnailBarnacleChance { get { return (float)options.GetConfigValue("SnailBarnacleChance") / 100; } }
        float BarnacleSnailChance { get { return (float)options.GetConfigValue("BarnacleSnailChance") / 100; } }
        float BlackBasiliskLizChance { get { return (float)options.GetConfigValue("BlackBasiliskLizChance") / 100; } }
        float GroundIndigoLizChance { get { return (float)options.GetConfigValue("GroundIndigoLizChance") / 100; } }
        float DrillCrabMirosChance { get { return (float)options.GetConfigValue("DrillCrabMirosChance") / 100; } }
        float MirosDrillCrabChance { get { return (float)options.GetConfigValue("MirosDrillCrabChance") / 100; } }
        float DrillCrabLoachChance { get { return (float)options.GetConfigValue("DrillCrabLoachChance") / 100; } }
        float LoachDrillCrabChance { get { return (float)options.GetConfigValue("LoachDrillCrabChance") / 100; } }
        float DeerDrillCrabInvChance { get { return (float)options.GetConfigValue("DeerDrillCrabInvChance") / 100; } }
        float LeechFrogChance { get { return (float)options.GetConfigValue("LeechFrogChance") / 100; } }
        float MouseRatChance { get { return (float)options.GetConfigValue("MouseRatChance") / 100; } }
        float GrubSandGrubChance { get { return (float)options.GetConfigValue("GrubSandGrubChance") / 100; } }
        float PopcornSandWormTrapChance { get { return (float)options.GetConfigValue("PopcornSandWormTrapChance") / 100; } }
        float HazerTardigradeChance { get { return (float)options.GetConfigValue("HazerTardigradeChance") / 100; } }
        float PearlFireSpriteChance { get { return (float)options.GetConfigValue("PearlFireSpriteChance") / 100; } }

        // Watcher - Extras
        int LoachExtras { get { return options.GetConfigValue("LoachExtras"); } }
        int BigMothExtras { get { return options.GetConfigValue("BigMothExtras"); } }
        int SmallMothExtras { get { return options.GetConfigValue("SmallMothExtras"); } }
        int SkywhaleExtras { get { return options.GetConfigValue("SkywhaleExtras"); } }
        int BasiliskLizExtras { get { return options.GetConfigValue("BasiliskLizExtras"); } }
        int IndigoLizExtras { get { return options.GetConfigValue("IndigoLizExtras"); } }
        int BarnacleExtras { get { return options.GetConfigValue("BarnacleExtras"); } }
        int DrillCrabExtras { get { return options.GetConfigValue("DrillCrabExtras"); } }
        int FrogExtras { get { return options.GetConfigValue("FrogExtras"); } }
        int RatExtras { get { return options.GetConfigValue("RatExtras"); } }

        // Mods
        float InspectorChance { get { return (float)options.GetConfigValue("InspectorChance") / 100; } }
        float RyanLizardChance { get { return (float)options.GetConfigValue("RyanLizardChance") / 100; } }

        // Static variables (needed in IL functions)

        static float HunterLongLegsChance;

        #endregion

        #region Modded Creature variables

        //Struct with object references. They cannot be primitives since they must access the values in-game, not during mod init.
        private class ModCreatureReplacement
        {
            public CreatureTemplate.Type type;
            private readonly Configurable<int> repChanceConfig;
            public readonly Dictionary<string, float> localMultipliers;
            public readonly Dictionary<string, int> localAdditions;
            public readonly bool perDenReplacement, isInvasion, overrideBalance;
            public ModCreatureReplacement(CreatureTemplate.Type type, Configurable<int> repChance, bool isInvasion = false, bool perDenReplacement = false, Dictionary<string, float> localMultipliers = null, Dictionary<string, int> localAdditions = null, bool overrideBalance = false)
            {
                this.type = type;
                this.repChanceConfig = repChance;
                this.localAdditions = localAdditions;
                this.localMultipliers = localMultipliers;
                this.perDenReplacement = perDenReplacement;
                this.isInvasion = isInvasion;
                this.overrideBalance = overrideBalance;
            }
            public ModCreatureReplacement(CreatureTemplate.Type type, Configurable<int> repChance, Dictionary<string, float> localMultipliers, Dictionary<string, int> localAdditions = null, bool overrideBalance = false)
            {
                this.type = type;
                this.repChanceConfig = repChance;
                this.localAdditions = localAdditions;
                this.localMultipliers = localMultipliers;
                this.perDenReplacement = false;
                this.isInvasion = false;
                this.overrideBalance = overrideBalance;
            }
            public float repChance
            {
                get => repChanceConfig.Value / 100f;
            }
            public void ClearDicts()
            {
                if (!(localMultipliers is null))
                    localMultipliers.Clear();
                if (!(localAdditions is null))
                    localAdditions.Clear();
            }
            ~ModCreatureReplacement()
            {
                ClearDicts();
            }
        }

        private class ModCreatureExtras
        {
            private Configurable<int> extrasConfig;
            public Dictionary<string, float> localMultipliers;
            public Dictionary<string, int> localAdditions;
            public bool divideByTen, overrideBalance;
            public ModCreatureExtras(Configurable<int> extras, bool divideByTen = true, Dictionary<string, float> localMultipliers = null, Dictionary<string, int> localAdditions = null, bool overrideBalance = false)
            {
                this.extrasConfig = extras;
                this.localAdditions = localAdditions;
                this.localMultipliers = localMultipliers;
                this.divideByTen = divideByTen;
                this.overrideBalance = overrideBalance;
            }
            public ModCreatureExtras(Configurable<int> extras, Dictionary<string, float> localMultipliers, Dictionary<string, int> localAdditions = null, bool overrideBalance = false)
            {
                this.extrasConfig = extras;
                this.localAdditions = localAdditions;
                this.localMultipliers = localMultipliers;
                this.divideByTen = true;
                this.overrideBalance = overrideBalance;
            }
            public int amount
            {
                get => extrasConfig.Value;
            }
            public void ClearDicts()
            {
                if (!(localMultipliers is null))
                    localMultipliers.Clear();
                if (!(localAdditions is null))
                    localAdditions.Clear();
            }
            ~ModCreatureExtras()
            {
                ClearDicts();
            }
        }

        private Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>> modCreatureReplacements;
        private Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>> modCreatureAncestorReplacements;
        private Dictionary<CreatureTemplate.Type, ModCreatureExtras> modCreatureExtras;
        public HashSet<string> activeMods;

        //Mod dependent
        public bool hasAngryInspectors, hasLizardVariants;

        #endregion

        #region Mod Setup Functions

        private bool IsInit;

        private bool hasUpdatedRefs;

        private bool logSpawners;

        private bool lastWasError;

        private bool hasSharedDLC;

        private bool hasDeers = false;

        private CreatureTemplate.Type vanillaHorizontalSpawn = null;

        private CreatureTemplate.Type[] horizontalSpawns;

        private HashSet<string> bannedRooms;

        private ApexUpYourSpawnsOptions options;

        private RainWorldGame game;
        
        Dictionary<string, List<World.Lineage>> lineagesToSave = new Dictionary<string, List<World.Lineage>>();
        //Dictionary<string, List<World.SimpleSpawner>> spawnersToSave = new Dictionary<string, List<World.SimpleSpawner>>();
        

        private static readonly HashSet<string> supportedMods = new HashSet<string>(
                new string[]
                {        
                    "ShinyKelp.AngryInspectors",
                    "moredlls",
                    "ShinyKelp.LizardVariants",
                    "thefriend",
                    "Outspector",
                    "theincandescent",
                    "ShinyKelp.ScavengerTweaks",
                    "ShinyKelp.CustomRelationships",
                    "lurzard.pitchblack",
                    "Croken.bombardier-vulture",
                    "pkuyo.thevanguard",
                    "drainmites",
                    "myr.moss_fields",
                    "ShinyKelp.Udonfly",
                    "shrimb.scroungers",
                    "shrimb.frostbite",
                    "Croken.Mimicstarfish",
                    "ShinyKelp.AlbinoKings",
                    "bebra.gregtech_lizard",
                    "bry.bubbleweavers",
                    "lb-fgf-m4r-ik.modpack"
                }
            );
        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;

            lastWasError = false;
            hasUpdatedRefs = false;

            if(bannedRooms is null)
                bannedRooms = new HashSet<string>();
            bannedRooms.Clear();
            bannedRooms.Add("SB_GOR01");
            bannedRooms.Add("SB_H03");
            bannedRooms.Add("SB_H02");
            bannedRooms.Add("SB_E04");
            bannedRooms.Add("SB_C06");

            if (modCreatureReplacements is null)
                modCreatureReplacements = new Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>>();
            if(modCreatureAncestorReplacements is null)
                modCreatureAncestorReplacements = new Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>>();
            if(modCreatureExtras is null)
                modCreatureExtras = new Dictionary<CreatureTemplate.Type, ModCreatureExtras>();
            if (activeMods is null)
                activeMods = new HashSet<string>();
            logSpawners = true;
            options = new ApexUpYourSpawnsOptions(this, Logger);

        }

        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;
                if (ModManager.MSC || ModManager.Watcher)
                    hasSharedDLC = true;
                //hooks go here
                On.GameSession.ctor += GameSessionOnctor;
                //On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
                On.WorldLoader.GeneratePopulation += GenerateCustomPopulation;
                if (hasSharedDLC)
                {
                    On.JellyFish.PlaceInRoom += JellyfishSpawn;
                    On.DangleFruit.PlaceInRoom += ReplaceStowawayBugBlueFruit;
                    On.MoreSlugcats.GooieDuck.PlaceInRoom += ReplaceStowawayBugGooieDuck;
                }

                On.WinState.CycleCompleted += SaveSpawnersOnCycleComplete;
                On.RainWorldGame.GoToDeathScreen += RainWorldGame_GoToDeathScreen;
                On.RainWorldGame.GoToStarveScreen += RainWorldGame_GoToStarveScreen;
                On.Menu.CharacterSelectPage.AbandonButton_OnPressDone += CharacterSelectPage_AbandonButton_OnPressDone;
                

                //PupLongLegs
                if (ModManager.MSC)
                {
                    IL.AbstractRoom.RealizeRoom += ReplaceSlugpupForHLLRoom;
                    IL.World.SpawnPupNPCs += ReplaceSlugpupForHLL;
                    On.DaddyLongLegs.ctor += GiveHunterDaddyPupColor;
                    On.DaddyGraphics.ApplyPalette += GiveHunterDaddyPupPalette;
                    On.DaddyGraphics.HunterDummy.ApplyPalette += GiveHunterDaddyDummyPupPalette;
                    On.DaddyGraphics.HunterDummy.DrawSprites += GiveHunterDaddyDummyPupSprites;
                    On.DaddyGraphics.HunterDummy.Update += GiveHunterDaddyDummyPupSize;
                    On.DaddyGraphics.HunterDummy.ctor += GiveHunterDaddyDummyPupTailSize;
                    //On.DaddyGraphics.DaddyDangleTube.ApplyPalette += GiveHunterDaddyDangleTubePupPallete;
                    //On.DaddyGraphics.DaddyTubeGraphic.ApplyPalette += GiveHunterDaddyTubePupPallete;
                    //On.DaddyGraphics.DaddyDeadLeg.ApplyPalette += GiveHunterDaddyDeadLegPupPallete;
                }

                if(ModManager.Watcher)
                {
                    On.SkyWhaleAbstractAI.CheckBlacklist += SkyWhale_ExtraBlacklist;
                    On.VultureGrub.PlaceInRoom += ReplaceVultureWithSandGrub;
                    On.SeedCob.PlaceInRoom += AddSandTrapToPopcorn;
                    On.Watcher.SandGrubNetwork.MarkConsumed += PreventSandGrubPullCrash;
                    On.Hazer.PlaceInRoom += ReplaceHazerWithTardigrade;
                    On.Room.ReadyForAI += AddBoxWormsOnPearls;
                }
                //*/
                ClearDictionaries();
                SetUpModDependencies();
                hasUpdatedRefs = false;

                if(activeMods.Contains("ShinyKelp.ScavengerTweaks"))
                {
                    On.Room.ReadyForAI += AddScavKings;
                }

                if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
                {
                    On.LizardGraphics.ctor += ForceBlackMoleSalamander;
                }
                MachineConnector.SetRegisteredOI("ShinyKelp.ApexUpYourSpawns", options);
                IsInit = true;
                UnityEngine.Debug.Log("Apex Up Your Spawns setup finished successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        //Apparently, limiting skywhales to deer's room attractions is not enough. Idk why.
        readonly HashSet<string> LF_Skywhale_Blacklist = new HashSet<string>
        {
            "LF_M04",
            "LF_M02",
            "LF_A01",
            "LF_A10",
            "LF_C01",
            "LF_C02",
            "LF_B01",
            "LF_B02",
            "LF_D04",
            "LF_D01",
            "LF_D02",
            "LF_D03",
            "LF_E01",
            "LF_E02",
            "LF_E03",
            "LF_E04",
            "LF_F02",
        };
        private bool SkyWhale_ExtraBlacklist(On.SkyWhaleAbstractAI.orig_CheckBlacklist orig, SkyWhaleAbstractAI self, string room)
        {
            return orig(self, room) || LF_Skywhale_Blacklist.Contains(room);
        }

        private void ClearDictionaries()
        {
            if (!(modCreatureReplacements is null))
            {
                foreach (KeyValuePair<CreatureTemplate.Type, List<ModCreatureReplacement>> pair in modCreatureReplacements)
                {
                    foreach (ModCreatureReplacement modRep in pair.Value)
                        modRep.ClearDicts();
                    pair.Value.Clear();
                }
                modCreatureReplacements.Clear();
            }

            if (!(modCreatureAncestorReplacements is null))
            {
                foreach (KeyValuePair<CreatureTemplate.Type, List<ModCreatureReplacement>> pair in modCreatureAncestorReplacements)
                {
                    foreach (ModCreatureReplacement modRep in pair.Value)
                        modRep.ClearDicts();
                    pair.Value.Clear();
                }
                modCreatureAncestorReplacements.Clear();
            }

            if (!(modCreatureExtras is null))
            {
                foreach (KeyValuePair<CreatureTemplate.Type, ModCreatureExtras> pair in modCreatureExtras)
                {
                    pair.Value.ClearDicts();
                }
                modCreatureExtras.Clear();
            }

            if (!(bannedRooms is null))
                bannedRooms.Clear();

            
            if (!(lineagesToSave is null))
            {
                foreach (List<World.Lineage> lins in lineagesToSave.Values)
                    lins.Clear();
                lineagesToSave.Clear();
            }
            /*
            if (!(spawnersToSave is null))
            {
                foreach (List<World.SimpleSpawner> spw in spawnersToSave.Values)
                    spw.Clear();
                spawnersToSave.Clear();
            }
            */
        }

        private void SetUpModDependencies()
        {
            hasAngryInspectors = hasLizardVariants = false;
            List<CreatureTemplate.Type> horizontalSpawnsList = new List<CreatureTemplate.Type>()
            {
                CreatureTemplate.Type.MirosBird,
                CreatureTemplate.Type.Deer
            };
            activeMods.Clear();

            foreach (ModManager.Mod mod in ModManager.ActiveMods)
            {
                if (supportedMods.Contains(mod.id))
                    activeMods.Add(mod.id);
            }
            if (logSpawners)
            {
                UnityEngine.Debug.Log("Mods detected: ");
                foreach (string s in activeMods)
                    UnityEngine.Debug.Log(s);
            }

            options.InitModConfigs();

            //DO NOT TRY TO USE StaticWorld.GetCreatureTemplate. IT DOES NOT WORK AT MOD LOADING TIME. USE new CreatureTemplate.Type("name")
            if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                //Sporantula
                Dictionary<string, float> localMultipliers = new Dictionary<string, float>();
                localMultipliers.Add("SmallNeedleWorm", .5f);
                localMultipliers.Add("BigNeedleWorm", .5f);
                localMultipliers.Add("Centiwing", .5f);
                localMultipliers.Add("CicadaA", .5f);
                localMultipliers.Add("CicadaB", .5f);
                localMultipliers.Add("AquaCenti", 0f);
                localMultipliers.Add("Woven Nest", 0.2f);
                CreatureTemplate.Type sporType = new CreatureTemplate.Type("Sporantula");
                ModCreatureReplacement sporantulaRep = new ModCreatureReplacement(
                    sporType,
                    options.GetModConfig("SporantulaChance"),
                    true,
                    false,
                    localMultipliers,
                    null
                    );
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Centipede, sporantulaRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallNeedleWorm, sporantulaRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BigNeedleWorm, sporantulaRep);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.BigSpider, sporantulaRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaA, sporantulaRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaB, sporantulaRep);
                modCreatureExtras.Add(sporType, new ModCreatureExtras(
                    options.GetModConfig("SporantulaExtras")
                    ));

                //Scutigera
                Dictionary<string, float> localMultipliersS = new Dictionary<string, float>();
                localMultipliersS.Add("GWArtificer", 2f);
                localMultipliersS.Add("GWSpear", 2f);
                CreatureTemplate.Type scutType = new CreatureTemplate.Type("Scutigera");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede,
                    new ModCreatureReplacement(
                        scutType,
                        options.GetModConfig("ScutigeraChance"),
                        localMultipliersS
                    )
                    );
                modCreatureExtras.Add(scutType, new ModCreatureExtras(
                    options.GetModConfig("ScutigeraExtras")
                    ));

                //Red horror
                Dictionary<string, int> localAdditionsRed = new Dictionary<string, int>();
                localAdditionsRed.Add("SI", 10);
                localAdditionsRed.Add("VS", 0);
                localAdditionsRed.Add("OE", 10);
                localAdditionsRed.Add("FR", 20);
                localAdditionsRed.Add("!", -10);
                CreatureTemplate.Type horrType = new CreatureTemplate.Type("RedHorrorCenti");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.RedCentipede,
                    new ModCreatureReplacement(
                        horrType,
                        options.GetModConfig("RedRedHorrorCentiChance"), null, localAdditionsRed
                ));

                Dictionary<string, int> localAdditionsWing = new Dictionary<string, int>();
                localAdditionsWing.Add("SI", 0);
                localAdditionsWing.Add("LC", 10);
                localAdditionsWing.Add("VS", 5);
                localAdditionsWing.Add("SU", 0);
                localAdditionsWing.Add("SD", 0);
                localAdditionsWing.Add("!", -5);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centiwing,
                    new ModCreatureReplacement(
                        horrType,
                        options.GetModConfig("WingRedHorrorCentiChance"), null, localAdditionsWing
                ));

                //Water spitter
                CreatureTemplate.Type watType = new CreatureTemplate.Type("WaterSpitter");
                ModCreatureReplacement wSpitter = new ModCreatureReplacement(
                    watType, options.GetModConfig("WaterSpitterChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, wSpitter);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.EelLizard, wSpitter);
                modCreatureExtras.Add(watType, new ModCreatureExtras(
                    options.GetModConfig("WaterSpitterExtras"), true));

                //Fat firefly
                Dictionary<string, int> localAdditionsFFF = new Dictionary<string, int>();
                Dictionary<string, float> localMultipliersFFF = new Dictionary<string, float>();
                localAdditionsFFF.Add("GW", 10);
                localAdditionsFFF.Add("BL", 25);
                localMultipliersFFF.Add("Saint", .5f);
                localMultipliersFFF.Add("MS", 0.1f);
                AddModCreatureToDictionary(modCreatureAncestorReplacements,
                    CreatureTemplate.Type.Vulture,
                    new ModCreatureReplacement(
                        new CreatureTemplate.Type("FatFireFly"),
                        options.GetModConfig("FatFireFlyChance"),
                        localMultipliersFFF,
                        localAdditionsFFF
                        ));

                //Surface swimmer
                Dictionary<string, float> localSSMultipliers = new Dictionary<string, float>();
                localSSMultipliers.Add("GW", 2f);
                localSSMultipliers.Add("LM", 2f);
                localSSMultipliers.Add("SU_A30", 2f);

                CreatureTemplate.Type ssType = new CreatureTemplate.Type("SurfaceSwimmer");

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    ssType, options.GetModConfig("SurfaceSwimmerChance"), localSSMultipliers));

                Dictionary<string, int> localSSAdds = new Dictionary<string, int>();
                localSSAdds.Add("DS", 20);
                localSSAdds.Add("SL", 20);
                localSSAdds.Add("LM", 20);

                modCreatureExtras.Add(ssType, new ModCreatureExtras(
                    options.GetModConfig("SurfaceSwimmerExtras"), true, null, localSSAdds));

                //Bouncing ball
                CreatureTemplate.Type bType = new CreatureTemplate.Type("BouncingBall");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, new ModCreatureReplacement(
                    bType, options.GetModConfig("BounceBallChance")));
                modCreatureExtras.Add(bType, new ModCreatureExtras(
                    options.GetModConfig("BounceBallExtras"), true));

                //Hoverfly
                CreatureTemplate.Type hoverType = new CreatureTemplate.Type("Hoverfly");
                ModCreatureReplacement hoverflyRep = new ModCreatureReplacement(hoverType, options.GetModConfig("CritterHoverflyChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallNeedleWorm, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.LanternMouse, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TubeWorm, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.Yeek, hoverflyRep);
                modCreatureExtras.Add(hoverType, new ModCreatureExtras(options.GetModConfig("HoverflyExtras")));

                //Noodle Eater
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallNeedleWorm, new ModCreatureReplacement(
                    new CreatureTemplate.Type("NoodleEater"), options.GetModConfig("NoodleEaterChance"), true, true));
                modCreatureExtras.Add(new CreatureTemplate.Type("NoodleEater"), new ModCreatureExtras(options.GetModConfig("NoodleEaterExtras")));

                //Thornbug
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    new CreatureTemplate.Type("ThornBug"), options.GetModConfig("ThornbugChance"), true));
                modCreatureExtras.Add(new CreatureTemplate.Type("ThornBug"), new ModCreatureExtras(options.GetModConfig("ThornbugExtras")));

                //Mini leviathan
                Dictionary<string, float> miniLeviDict = new Dictionary<string, float>();
                miniLeviDict.Add("SB", 1.5f);
                miniLeviDict.Add("MS", 1.5f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BigEel, new ModCreatureReplacement(
                    new CreatureTemplate.Type("MiniLeviathan"), options.GetModConfig("MiniLeviathanChance"), true, false, miniLeviDict));
                modCreatureExtras.Add(new CreatureTemplate.Type("MiniLeviathan"), new ModCreatureExtras(options.GetModConfig("MiniLeviathanExtras"), false));

                //Polliwog
                CreatureTemplate.Type polliType = new CreatureTemplate.Type("Polliwog");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, new ModCreatureReplacement(
                    polliType, options.GetModConfig("PolliwogChance")));
                modCreatureExtras.Add(polliType, new ModCreatureExtras(options.GetModConfig("PolliwogExtras")));

                //Hunter seeker (white-cyan)
                CreatureTemplate.Type hunType = new CreatureTemplate.Type("HunterSeeker");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, new ModCreatureReplacement(
                    hunType, options.GetModConfig("HunterSeekerCyanChance")));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.WhiteLizard, new ModCreatureReplacement(
                    hunType, options.GetModConfig("HunterSeekerWhiteChance")));
                modCreatureExtras.Add(hunType, new ModCreatureExtras(options.GetModConfig("HunterSeekerExtras")));

                //Silver lizard
                CreatureTemplate.Type silType = new CreatureTemplate.Type("SilverLizard");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, new ModCreatureReplacement(
                    silType, options.GetModConfig("SilverLizChance")));
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, new ModCreatureReplacement(
                    silType, options.GetModConfig("SilverLizChance")));
                modCreatureExtras.Add(silType, new ModCreatureExtras(options.GetModConfig("SilverLizExtras")));

                //Mole salamander, blizzor
                CreatureTemplate.Type salamoleType = new CreatureTemplate.Type("MoleSalamander");
                CreatureTemplate.Type blizzorType = new CreatureTemplate.Type("Blizzor");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.MirosBird, new ModCreatureReplacement(
                    blizzorType, options.GetModConfig("BlizzorChance")));
                modCreatureExtras.Add(blizzorType, new ModCreatureExtras(options.GetModConfig("BlizzorExtras"), false));

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, new ModCreatureReplacement(
                    salamoleType, options.GetModConfig("SalamanderSalamoleChance")));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlackLizard, new ModCreatureReplacement(
                    salamoleType, options.GetModConfig("BlackSalamolechance")));
                horizontalSpawnsList.Add(blizzorType);
            }
            if (activeMods.Contains("ShinyKelp.AngryInspectors"))
            {
                hasAngryInspectors = true;
            }
            if (activeMods.Contains("moredlls"))
            {
                Dictionary<string, int> localAdditionsExp = new Dictionary<string, int>();
                localAdditionsExp.Add("GWArtificer", 10);
                localAdditionsExp.Add("GWSpear", 10);
                ModCreatureReplacement expDLL = new ModCreatureReplacement(
                        new CreatureTemplate.Type("ExplosiveDaddyLongLegs"),
                        options.GetModConfig("MExplosiveLongLegsChance"),
                        null,
                        localAdditionsExp);

                Dictionary<string, int> localAdditionsZap = new Dictionary<string, int>();
                localAdditionsZap.Add("UW", 10);
                ModCreatureReplacement zapDLL = new ModCreatureReplacement(
                        new CreatureTemplate.Type("ZapDaddyLongLegs"),
                        options.GetModConfig("MZappyLongLegsChance"),
                        null,
                        localAdditionsZap);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, expDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, expDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, zapDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, zapDLL);
            }
            if (activeMods.Contains("ShinyKelp.LizardVariants"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("RyanLizard"),
                        options.GetModConfig("RyanLizardChance")));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("YellowLimeLizard"),
                        options.GetModConfig("YellowLimeLizardChance")));
                CreatureTemplate.Type mintType = new CreatureTemplate.Type("MintLizard");

                ModCreatureReplacement mintLiz = new ModCreatureReplacement(
                        mintType,
                        options.GetModConfig("MintLizardChance"));

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, mintLiz);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, mintLiz);
                modCreatureExtras.Add(mintType, new ModCreatureExtras(options.GetModConfig("MintLizardExtras"), true));

                hasLizardVariants = true;
            }            
            if (activeMods.Contains("thefriend"))
            {
                Dictionary<string, float> motherDic = new Dictionary<string, float>();
                motherDic.Add("OE", .5f);
                ModCreatureReplacement motherRep = new ModCreatureReplacement(new CreatureTemplate.Type("MotherLizard"), options.GetModConfig("MotherLizardChance"), motherDic);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, motherRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, motherRep);
                if(activeMods.Contains("ShinyKelp.LizardVariants"))
                    AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("MintLizard"), motherRep);

                Dictionary<string, float> snowSpiderModifs = new Dictionary<string, float>();
                snowSpiderModifs.Add("GWArtificer", .5f);
                snowSpiderModifs.Add("GWSpear", .5f);
                snowSpiderModifs.Add("Saint", 2f);
                snowSpiderModifs.Add("Bitter Aerie", 4f);
                Dictionary<string, int> snowSpiderInts = new Dictionary<string, int>();
                snowSpiderInts.Add("SH", 8);
                snowSpiderInts.Add("SB", 8);
                CreatureTemplate.Type snowType = new CreatureTemplate.Type("SnowSpider");
                CreatureTemplate.Type lizType = new CreatureTemplate.Type("YoungLizard");

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BigSpider, new ModCreatureReplacement(
                    snowType, options.GetModConfig("SnowSpiderChance"), snowSpiderModifs));

                Dictionary<string, float> lostYoungModif = new Dictionary<string, float>();
                lostYoungModif.Add("PinkLizard", .5f);

                ModCreatureReplacement youngLizRep = new ModCreatureReplacement(lizType,
                    options.GetModConfig("LostYoungLizardChance"), true, true, lostYoungModif);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.PinkLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.ZoopLizard, youngLizRep);

                modCreatureExtras.Add(lizType, new ModCreatureExtras(options.GetModConfig("YoungLizardExtras")));
                modCreatureExtras.Add(snowType, new ModCreatureExtras(options.GetModConfig("SnowSpiderExtras")));
            }
            if (activeMods.Contains("Outspector"))
            {
                CreatureTemplate.Type outType = new CreatureTemplate.Type("Outspector");
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.Inspector, new ModCreatureReplacement(
                    outType, options.GetModConfig("OutspectorChance")));
                modCreatureExtras.Add(outType, new ModCreatureExtras(
                    options.GetModConfig("OutspectorExtras"), true));
            }
            if (activeMods.Contains("theincandescent"))
            {
                //Cyanwing, IcyBlueLizard, FreezerLizard, InfantAquapede
                Dictionary<string, float> icyBlueBD = new Dictionary<string, float>();
                icyBlueBD.Add("Night", 1.25f);
                icyBlueBD.Add("PreCycle", 0.5f);
                icyBlueBD.Add("SUIncandescent", 0.5f);
                icyBlueBD.Add("Bitter Aerie", 2f);

                CreatureTemplate.Type icyType = new CreatureTemplate.Type("IcyBlueLizard");
                CreatureTemplate.Type freezerType = new CreatureTemplate.Type("FreezerLizard");

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                        icyType, options.GetModConfig("IcyBlueYellowChance"), icyBlueBD)
                    );

                AddModCreatureToDictionary(modCreatureReplacements, freezerType, new ModCreatureReplacement(
                        icyType, options.GetModConfig("IcyBlueFreezerInvChance"), true, true, icyBlueBD)
                    );

                Dictionary<string, float> icyBlueBD2 = new Dictionary<string, float>();
                icyBlueBD2.Add("Night", 1.25f);
                icyBlueBD2.Add("PreCycle", 0.5f);
                icyBlueBD2.Add("SUIncandescent", 0.5f);
                icyBlueBD2.Add("Saint", 0f);
                icyBlueBD2.Add("Bitter Aerie", 2f);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, new ModCreatureReplacement(
                        icyType, options.GetModConfig("IcyBlueBlueChance"), true, true, icyBlueBD2)
                    );
                Dictionary<string, float> icyBlueBD3 = new Dictionary<string, float>();
                icyBlueBD3.Add("Saint", 2f);
                icyBlueBD.Add("!", 0f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, new ModCreatureReplacement(
                        icyType, options.GetModConfig("IcyBlueBlueChance"), false, true, icyBlueBD3)
                    );

                modCreatureExtras.Add(icyType, new ModCreatureExtras(
                    options.GetModConfig("IcyBlueLizExtras"), true));


                Dictionary<string, float> freezerBD = new Dictionary<string, float>();
                freezerBD.Add("Night", 1.5f);
                freezerBD.Add("PreCycle", 0f);
                freezerBD.Add("SUIncandescent", 0.5f);
                freezerBD.Add("Bitter Aerie", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, new ModCreatureReplacement(
                        freezerType, options.GetModConfig("FreezerLizChance"), false, true, freezerBD)
                    );
                AddModCreatureToDictionary(modCreatureReplacements, icyType, new ModCreatureReplacement(
                        freezerType, options.GetModConfig("FreezerLizChance"), false, true, freezerBD)
                    );

                Dictionary<string, float> cyanwingBD = new Dictionary<string, float>();
                cyanwingBD.Add("Night", 1.25f);
                cyanwingBD.Add("PreCycle", 0.5f);
                cyanwingBD.Add("SISaint", 0.5f);
                cyanwingBD.Add("SIIncandescent", 0.5f);

                CreatureTemplate.Type cyanType = new CreatureTemplate.Type("Cyanwing");

                ModCreatureReplacement cyanwingRep = new ModCreatureReplacement(cyanType, options.GetModConfig("CyanwingChance"), false, true, cyanwingBD);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, cyanwingRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallCentipede, cyanwingRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centiwing, new ModCreatureReplacement(
                        cyanType, options.GetModConfig("WingCyanwingChance"), false, true, cyanwingBD
                    ));

                Dictionary<string, int> babyAquapedeBD = new Dictionary<string, int>();
                babyAquapedeBD.Add("Sump Tunnel", 45);

                CreatureTemplate.Type aquaType = new CreatureTemplate.Type("InfantAquapede");
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.AquaCenti, new ModCreatureReplacement(
                        aquaType, options.GetModConfig("BabyAquapedeInvChance"), true)
                    );
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.JetFish, new ModCreatureReplacement(
                        aquaType, options.GetModConfig("JetfishBabyAquapedeChance"), true, false, null, babyAquapedeBD)
                    );
                modCreatureExtras.Add(aquaType, new ModCreatureExtras(options.GetModConfig("AquapedeExtras"), true));
                //Chillipede

                CreatureTemplate.Type chillType = new CreatureTemplate.Type("Chillipede");
                ModCreatureReplacement chillRep = new ModCreatureReplacement(chillType, options.GetModConfig("ChillipedeChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, chillRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, chillRep);
                if(activeMods.Contains("ShinyKelp.LizardVariants"))
                    AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("MintLizard"), chillRep);

                Dictionary<string, float> subChillipedes = new Dictionary<string, float>();
                subChillipedes.Add("SB", 1f);
                subChillipedes.Add("!", 0f);
                ModCreatureReplacement subChillRep = new ModCreatureReplacement(chillType, options.GetModConfig("ChillipedeChance"), subChillipedes, null, true);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, subChillRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.WhiteLizard, subChillRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, subChillRep);

                bool updatedHailstorm = false;
                foreach(ModManager.Mod mod in ModManager.ActiveMods)
                {
                    if(mod.id == "theincandescent")
                    {
                        if (mod.version.Substring(0, 3) != "0.2")
                            updatedHailstorm = true;
                        break;
                    }
                }
                if (updatedHailstorm)
                {
                    ModCreatureReplacement kelpChillRep = new ModCreatureReplacement(chillType, options.GetModConfig("ChillipedeChance"), true);
                    AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TentaclePlant, kelpChillRep);

                }
            }
            if (activeMods.Contains("lurzard.pitchblack"))
            {
                //Night Terror, Little Longlegs
                Dictionary<string, int> nightTerrorDict = new Dictionary<string, int>();
                nightTerrorDict.Add("SH", 15);
                nightTerrorDict.Add("VS_F01", 20);
                nightTerrorDict.Add("SB", 4);
                nightTerrorDict.Add("Filtration", 4);
                nightTerrorDict.Add("Night", 20);
                CreatureTemplate.Type nightType = new CreatureTemplate.Type("NightTerror");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, new ModCreatureReplacement(
                    nightType, options.GetModConfig("NightTerrorChance"), false, false, null, nightTerrorDict));

                //Night terror replace red cent in shaded/night
                Dictionary<string, int> redCentRepDict = new Dictionary<string, int>();
                redCentRepDict.Add("SH", 30);
                redCentRepDict.Add("Night", 20);
                redCentRepDict.Add("!", -10);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.RedCentipede, new ModCreatureReplacement(
                        nightType, options.GetModConfig("NightTerrorChance"), false, false, null, redCentRepDict
                    ));

                //Little longlegs

                Dictionary<string, float> lllDict = new Dictionary<string, float>();
                lllDict.Add("VS", 2f);
                lllDict.Add("SL", 2f);
                lllDict.Add("LM", 2f);
                lllDict.Add("DS", 2f);
                lllDict.Add("OA", 2f);
                lllDict.Add("The Gutter", 2f);

                CreatureTemplate.Type littleType = new CreatureTemplate.Type("LMiniLongLegs");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, new ModCreatureReplacement(
                    littleType, options.GetModConfig("BrotherLittleLongLegChance"), true, false, lllDict));

                ModCreatureReplacement lllRep = new ModCreatureReplacement(
                    littleType, options.GetModConfig("CritterLittleLongLegsChance"), true, false, lllDict);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, lllRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.LanternMouse, lllRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TubeWorm, lllRep);

                //Extras
                modCreatureExtras.Add(littleType, new ModCreatureExtras(
                    options.GetModConfig("LittleLongLegsExtras"), true));

            }
            if (activeMods.Contains("Croken.bombardier-vulture"))
            {
                Dictionary<string, float> bombDict = new Dictionary<string, float>();
                bombDict.Add("GW", 1.2f);
                bombDict.Add("LF", 1.2f);
                bombDict.Add("UW", 0.6f);
                bombDict.Add("Saint", 0.2f);
                bombDict.Add("LM", 1.2f);
                bombDict.Add("GWArtificer", 1.2f);
                bombDict.Add("GWSpear", 1.2f);
                bombDict.Add("LFRed", 1.2f);
                bombDict.Add("SIRed", 1.2f);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Vulture, new ModCreatureReplacement(
                    new CreatureTemplate.Type("BombardierVulture"), options.GetModConfig("BombVultureChance"), bombDict));
            }
            if (activeMods.Contains("drainmites"))
            {
                Dictionary<string, float> drainmiteDict = new Dictionary<string, float>();
                drainmiteDict.Add("!", 1f);
                drainmiteDict.Add("PreCycle", 0f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Scavenger, new ModCreatureReplacement(
                    new CreatureTemplate.Type("DrainMite"), options.GetModConfig("DrainMiteChance"), true, true, drainmiteDict, null, true));
            }
            if (activeMods.Contains("myr.moss_fields") || activeMods.Contains("ShinyKelp.Udonfly"))
            {
                Dictionary<string, float> fatFlyDict = new Dictionary<string, float>();
                fatFlyDict.Add("SI", 1.5f);
                Dictionary<string, int> fatFlyDict2 = new Dictionary<string, int>();
                fatFlyDict2.Add("SI", 10);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.BigNeedleWorm, new ModCreatureReplacement(
                    new CreatureTemplate.Type("SnootShootNoot"), options.GetModConfig("FatNootChance"), fatFlyDict, fatFlyDict2));
            }
            if (activeMods.Contains("pkuyo.thevanguard"))
            {
                Dictionary<string, float> toxicDict = new Dictionary<string, float>();
                toxicDict.Add("GW", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SpitterSpider, new ModCreatureReplacement(
                    new CreatureTemplate.Type("ToxicSpider"), options.GetModConfig("ToxicSpiderChance")));
            }
            if (activeMods.Contains("shrimb.scroungers") || activeMods.Contains("shrimb.frostbite"))
            {
                Dictionary<string, float> scroungerDict = new Dictionary<string, float>();
                scroungerDict.Add("Artificer", 0.2f);
                scroungerDict.Add("Spear", 0.35f);
                scroungerDict.Add("Rivulet", 1.5f);
                scroungerDict.Add("Saint", 2f);
                scroungerDict.Add("UW", 2f);
                scroungerDict.Add("FR", 3f);
                scroungerDict.Add("OE", 3f);
                scroungerDict.Add("SI", 1.5f);
                scroungerDict.Add("SB", 0.5f);
                scroungerDict.Add("GW", 0.5f);
                CreatureTemplate.Type scrType = new CreatureTemplate.Type("Scrounger");
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Scavenger, new ModCreatureReplacement(
                    scrType, options.GetModConfig("ScroungerChance"), scroungerDict));
                modCreatureExtras.Add(scrType, new ModCreatureExtras(
                    options.GetModConfig("ScroungerExtras"), false));
            }
            if (activeMods.Contains("Croken.Mimicstarfish"))
            {
                Dictionary<string, int> starfishDict = new Dictionary<string, int>();
                starfishDict.Add("MS", 10);
                starfishDict.Add("SL", 10);
                starfishDict.Add("LM", 10);
                starfishDict.Add("DS", 10);
                starfishDict.Add("OA", 10);
                starfishDict.Add("Sump Tunnel", 10);
                starfishDict.Add("!", -5);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Mimicstar"), options.GetModConfig("BllMimicstarfishChance"), null, starfishDict));
                
                Dictionary<string, int> starfishDict2 = new Dictionary<string, int>();
                starfishDict.Add("JetFish", 3);
                starfishDict.Add("AquaCenti", 4);
                ModCreatureReplacement critterMimicRep = new ModCreatureReplacement(new CreatureTemplate.Type("Mimicstar"),
                    options.GetModConfig("CritterMimicstarfishChance"), false, true, null, starfishDict2);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.JetFish, critterMimicRep);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Snail, critterMimicRep);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Leech, critterMimicRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.AquaCenti, critterMimicRep);

            }
            if (activeMods.Contains("bebra.gregtech_lizard"))
            {
                CreatureTemplate.Type elecLiz = new CreatureTemplate.Type("GregTechLizard");
                Dictionary<string, float> elecFiltersL = new Dictionary<string, float>();
                Dictionary<string, float> elecFiltersC = new Dictionary<string, float>();
                elecFiltersL.Add("Salamander", 0f);
                elecFiltersL.Add("EelLizard", 0f);
                elecFiltersC.Add("AquaCenti", 0f);
                if(activeMods.Contains("theincandescent"))
                    elecFiltersC.Add("InfantAquapede", 0f);
                if(activeMods.Contains("lb-fgf-m4r-ik.water-spitter"))
                    elecFiltersL.Add("WaterSpitter", 0f);
                if(activeMods.Contains("lb-fgf-m4r-ik.coral-reef"))
                    elecFiltersL.Add("Polliwog", 0f);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.LizardTemplate,
                    new ModCreatureReplacement(elecLiz, options.GetModConfig("LizardElectricLizChance"), true, true, elecFiltersL, null, true));
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Centipede,
                    new ModCreatureReplacement(elecLiz, options.GetModConfig("LizardElectricLizChance"), true, true, elecFiltersC, null, true));
            }
            if (activeMods.Contains("bry.bubbleweavers"))
            {
                CreatureTemplate.Type bubbleType1 = new CreatureTemplate.Type("BubbleWeaver"),
                    bubbleType2 = new CreatureTemplate.Type("SapphiricWeaver");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SpitterSpider, new ModCreatureReplacement(
                    bubbleType1, options.GetModConfig("SpiderWeaverChance")));
                if (activeMods.Contains("lb-fgf-m4r-ik.swalkins"))
                {
                    AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("SurfaceSwimmer"), new ModCreatureReplacement(
                        bubbleType2, options.GetModConfig("SSwimmerWeaverChance")));
                }
                modCreatureExtras.Add(bubbleType1, new ModCreatureExtras(options.GetModConfig("BubbleWeaverExtras")));
                modCreatureExtras.Add(bubbleType2, new ModCreatureExtras(options.GetModConfig("BubbleWeaverExtras")));
            }
            if (ModManager.Watcher)
            {
                horizontalSpawnsList.Add(WatcherEnums.CreatureTemplateType.DrillCrab);
                horizontalSpawnsList.Add(WatcherEnums.CreatureTemplateType.Loach);
                horizontalSpawnsList.Add(WatcherEnums.CreatureTemplateType.RotLoach);
            }
            horizontalSpawns = horizontalSpawnsList.ToArray();
        }

        private void AddModCreatureToDictionary(Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>> dict, CreatureTemplate.Type key, ModCreatureReplacement modCreature)
        {
            if(dict.TryGetValue(key, out List<ModCreatureReplacement> list))
            {
                list.Add(modCreature);
            }
            else
            {
                List<ModCreatureReplacement> list2 = new List<ModCreatureReplacement>();
                list2.Add(modCreature);
                dict.Add(key, list2);
            }
        }

        #endregion

        #region Spawner Savestate handling
        private void CharacterSelectPage_AbandonButton_OnPressDone(On.Menu.CharacterSelectPage.orig_AbandonButton_OnPressDone orig, Menu.CharacterSelectPage self, Menu.Remix.MixedUI.UIfocusable trigger)
        {
            orig(self, trigger);
            ResetSpawnBufferOnDeath();
        }

        private void RainWorldGame_GoToStarveScreen(On.RainWorldGame.orig_GoToStarveScreen orig, RainWorldGame self)
        {
            orig(self);
            ResetSpawnBufferOnDeath();
        }

        private void RainWorldGame_GoToDeathScreen(On.RainWorldGame.orig_GoToDeathScreen orig, RainWorldGame self)
        {
            orig(self);
            ResetSpawnBufferOnDeath();
        }

        private void ResetSpawnBufferOnDeath()
        {
            foreach (List<World.Lineage> lin in lineagesToSave.Values)
                lin.Clear();
            lineagesToSave.Clear();
        }
        

        private void SaveSpawnersOnCycleComplete(On.WinState.orig_CycleCompleted orig, WinState self, RainWorldGame game)
        {
            orig(self, game);

            if (game.rainWorld.safariMode || (game.IsStorySession && game.GetStorySession.saveState.malnourished))
                return;

            if (!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns");
            if (!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates");

            SlugcatStats.Name player = null;
            int saveSlot = Math.Abs(game.rainWorld.options.saveSlot);
            bool expedition = game.rainWorld.ExpeditionMode;
            if (game.IsStorySession)
                player = game.GetStorySession.saveStateNumber;
            string baseSaveFilePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates/" +
                (expedition ? "e" : "") + saveSlot + player.ToString();

            foreach (string regionSave in lineagesToSave.Keys)
            {
                string filePath = baseSaveFilePath + regionSave + ".txt";
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (World.Lineage lineage in lineagesToSave[regionSave])
                    {
                        string linStr = lineage.SpawnerID.ToString() + "|";
                        foreach (int critIndex in lineage.creatureTypes)
                        {
                            string critName = "Empty";
                            if(critIndex > -1 && critIndex < StaticWorld.creatureTemplates.Count())
                                critName = StaticWorld.creatureTemplates[critIndex].type.ToString();
                            linStr += critName + ";";
                        }
                        linStr = linStr.Substring(0, linStr.Length - 1);

                        linStr += "|";
                        for (int i = 0; i < lineage.spawnData.Length; ++i)
                        {
                            if (lineage.spawnData[i] != null)
                                linStr += lineage.spawnData[i];
                            linStr += ";";
                        }
                        linStr = linStr.Substring(0, linStr.Length - 1);
                        writer.WriteLine(linStr);
                    }

                }
                lineagesToSave[regionSave].Clear();
            }
            lineagesToSave.Clear();
        }
        

        private bool LoadSaveState(WorldLoader worldLoader)
        {
            bool needsFresh = false;
            Dictionary<int, KeyValuePair<int[], string[]>> linSaveState = SavedLineagesToDictionary(worldLoader);

            if (logSpawners)
            {
                Debug.Log("READ DICTIONARY:");
                foreach (int i in linSaveState.Keys)
                {
                    Debug.Log("\n   ===========\n");
                    Debug.Log("ID: " + i);
                    foreach (int j in linSaveState[i].Key)
                        Debug.Log(j);

                }
                Debug.Log("\n");
            }
            
            foreach (World.Lineage lineage in worldLoader.spawners.OfType<World.Lineage>())
            {
                if (linSaveState.ContainsKey(lineage.SpawnerID))
                {
                    lineage.creatureTypes = linSaveState[lineage.SpawnerID].Key;
                    lineage.spawnData = linSaveState[lineage.SpawnerID].Value;

                }
            }
            linSaveState.Clear();

            /*
             * Offscreen dens
            List<World.SimpleSpawner> offScreenSaveState = SavedSpawnersToList(worldLoader);
            if(offScreenSaveState.Count > 0)
            {
                WorldCoordinate offScreenDen = FindOffScreenDen(worldLoader);
                foreach (World.SimpleSpawner spawner in worldLoader.spawners.OfType<World.SimpleSpawner>())
                {
                    if (spawner.den == offScreenDen)
                    {
                        spawner.creatureType = offScreenSaveState[0].creatureType;
                        spawner.amount = offScreenSaveState[0].amount;
                        spawner.spawnDataString = offScreenSaveState[0].spawnDataString;
                        offScreenSaveState.RemoveAt(0);
                        if (offScreenSaveState.Count == 0)
                            break;
                    }
                }
                if (offScreenSaveState.Count > 0)
                    needsFresh = true;
                //
                while (offScreenSaveState.Count > 0)
                {
                    offScreenSaveState[0].inRegionSpawnerIndex = worldLoader.spawners.Count;
                    worldLoader.spawners.Add(offScreenSaveState[0]);
                    offScreenSaveState.RemoveAt(0);
                }
            }*/
            return needsFresh;
        }
        
        private void AddSpawnersToBuffer(WorldLoader loader)
        {
            List<World.Lineage> lineages = new List<World.Lineage>();
            //WorldCoordinate offScreenDen = FindOffScreenDen(loader);
            foreach (World.CreatureSpawner spawner in loader.spawners)
            {
                if (spawner is World.Lineage lin)
                    lineages.Add(lin);
            }
            if (lineages.Count > 0)
            {
                if (lineagesToSave.ContainsKey(loader.worldName))
                {
                    Debug.Log("WARNING: Trying to save state of region twice: " + loader.worldName);
                    lineagesToSave[loader.worldName] = lineages;
                }
                else
                    lineagesToSave.Add(loader.worldName, lineages);
            }
        }

        private Dictionary<int, KeyValuePair<int[], string[]>> SavedLineagesToDictionary(WorldLoader loader)
        {
            Dictionary<int, KeyValuePair<int[], string[]>> lineagesDict = new Dictionary<int, KeyValuePair<int[], string[]>>();
            string filePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates/";
            SlugcatStats.Name player = null;
            int saveSlot = Math.Abs(loader.game.rainWorld.options.saveSlot);
            bool expedition = loader.game.rainWorld.ExpeditionMode;
            if (loader.game.IsStorySession)
                player = loader.game.GetStorySession.saveStateNumber;
            filePath += (expedition ? "e" : "") + saveSlot + player.ToString() + loader.worldName + ".txt";
            StreamReader sr = null;
            try
            {
                if (File.Exists(filePath))
                {
                    sr = new StreamReader(filePath);
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] baseSplits = line.Split('|');
                        string[] creatureSplits = baseSplits[1].Split(';');
                        int[] critArray = new int[creatureSplits.Length];
                        for (int i = 0; i < creatureSplits.Length; ++i)
                        {
                            if (creatureSplits[i] == "Empty")
                                critArray[i] = -1;
                            else
                                critArray[i] = new CreatureTemplate.Type(creatureSplits[i]).index;
                        }
                        string[] spawnDataSplit = baseSplits[2].Split(';');
                        if (spawnDataSplit.Length != creatureSplits.Length)
                            spawnDataSplit = new string[creatureSplits.Length];
                        for (int i = 0; i < spawnDataSplit.Length; ++i)
                        {
                            if (spawnDataSplit[i] != null && (spawnDataSplit[i].Length < 3 ||
                                !spawnDataSplit[i].StartsWith("{") || !spawnDataSplit[i].EndsWith("}")))
                                spawnDataSplit[i] = null;
                        }
                        lineagesDict.Add(int.Parse(baseSplits[0]), new KeyValuePair<int[], string[]>(critArray, spawnDataSplit));

                        line = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR WHILE READING FROM FILE.");
                Debug.Log(e.Message);
            }
            finally
            {
                if(sr != null)
                    sr.Close();
            }
            return lineagesDict;
        }

        private List<World.SimpleSpawner> SavedSpawnersToList(WorldLoader loader)
        {
            List<World.SimpleSpawner> savedSpawners = new List<World.SimpleSpawner>();
            string filePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates/";
            SlugcatStats.Name player = null;
            int saveSlot = Math.Abs(loader.game.rainWorld.options.saveSlot);
            bool expedition = loader.game.rainWorld.ExpeditionMode;
            if (loader.game.IsStorySession)
                player = loader.game.GetStorySession.saveStateNumber;
            filePath += (expedition ? "e" : "") + saveSlot + player.ToString() + loader.worldName + ".txt";

            if (File.Exists(filePath))
            {
                WorldCoordinate offScreenDen = FindOffScreenDen(loader);
                StreamReader sr = new StreamReader(filePath);
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (!line.StartsWith("===="))
                    {
                        line = sr.ReadLine();
                        continue;
                    }
                    else
                    {
                        line = sr.ReadLine();
                        break;
                    }
                }
                while (line != null)
                {

                    World.SimpleSpawner spawner = new World.SimpleSpawner(0, 0, offScreenDen, CreatureTemplate.Type.StandardGroundCreature,
                        null, 0);
                    string[] splitLine = line.Split('|');
                    string[] splitID = splitLine[0].Split(':');
                    spawner.region = int.Parse(splitID[0]);
                    spawner.inRegionSpawnerIndex = int.Parse(splitID[1]);
                    string[] spawnData = splitLine[1].Split(',');

                    if (!spawnData[0].Contains(":"))
                    {
                        int index = int.Parse(spawnData[0]);
                        spawner.creatureType = StaticWorld.creatureTemplates[index].type;
                    }
                    else
                    {
                        string[] splitType = spawnData[0].Split(':');
                        int index = int.Parse(splitType[1]);
                        if (index > -1 && index < StaticWorld.creatureTemplates.Length &&
                            StaticWorld.creatureTemplates[index].type.value == splitType[0])
                            spawner.creatureType = StaticWorld.creatureTemplates[index].type;
                        else
                            spawner.creatureType = new CreatureTemplate.Type(splitType[0]);
                    }

                    spawner.amount = int.Parse(spawnData[1]);
                    if (spawnData[2].Length > 2 && spawnData[2].StartsWith("{") && spawnData[2].EndsWith("}"))
                        spawner.spawnDataString = spawnData[2];
                    savedSpawners.Add(spawner);

                    line = sr.ReadLine();
                }
                sr.Close();
            }
            return savedSpawners;
        }
        
        #endregion

        #region HunterLongLegs functions

        private void ReplaceSlugpupForHLLRoom(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(2),
                x => x.MatchLdfld<AbstractCreature>("state"),
                x => x.MatchIsinst<PlayerNPCState>(),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<PlayerState>("foodInStomach")
                );
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 2);
            c.EmitDelegate<Action<AbstractRoom, AbstractCreature>>((absRoom, pupAbstract) =>
            {
                UnityEngine.Debug.Log("Created Pup from AbstractRoom.RealizeRoom!");
                if (ApexUpYourSpawnsMod.HunterLongLegsChance > UnityEngine.Random.value)
                {
                    AbstractCreature hllReplacement = new AbstractCreature(pupAbstract.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy),
                            null, pupAbstract.pos, pupAbstract.ID);
                    absRoom.RemoveEntity(pupAbstract);
                    absRoom.AddEntity(hllReplacement);
                }
            });
        }
        
        private void ReplaceSlugpupForHLL(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(13),
                x => x.MatchLdfld<AbstractCreature>("state"),
                x => x.MatchIsinst<PlayerNPCState>(),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<PlayerState>("foodInStomach")
                );
            c.Emit(OpCodes.Ldloc, 6);
            c.Emit(OpCodes.Ldloc, 13);
            c.EmitDelegate<Action<AbstractRoom, AbstractCreature>>((absRoom, pupAbstract) =>
            {
                UnityEngine.Debug.Log("Created Pup from World.SpawnNPCs!");
                if (ApexUpYourSpawnsMod.HunterLongLegsChance > UnityEngine.Random.value)
                {
                    AbstractCreature hllReplacement = new AbstractCreature(pupAbstract.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy),
                            null, pupAbstract.pos, pupAbstract.ID);
                    absRoom.RemoveEntity(pupAbstract);
                    absRoom.AddEntity(hllReplacement);
                    if (absRoom.realizedRoom != null)
                        hllReplacement.RealizeInRoom();
                    else
                        absRoom.RealizeRoom(pupAbstract.world, pupAbstract.world.game);
                }
            });
        }

        private void GiveHunterDaddyPupColor(On.DaddyLongLegs.orig_ctor orig, DaddyLongLegs self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.isHD)
            {
                if((game.IsStorySession && self.abstractCreature.Room.shelter) || (!game.IsStorySession && UnityEngine.Random.value < .8f))
                {
                    UnityEngine.Random.InitState(self.abstractCreature.ID.RandomSeed);
                    float Met = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 1.5f);
                    float Stealth = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 1.5f);

                    float H = Mathf.Lerp(UnityEngine.Random.Range(0.15f, 0.58f), UnityEngine.Random.value, Mathf.Pow(UnityEngine.Random.value, 1.5f - Met));
                    float S = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 0.3f + Stealth * 0.3f);
                    bool Dark = (UnityEngine.Random.Range(0f, 1f) <= 0.3f + Stealth * 0.2f);
                    float L = Mathf.Pow(UnityEngine.Random.Range(Dark ? 0.9f : 0.75f, 1f), 1.5f - Stealth);
                    //float EyeColor = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 2f - Stealth * 1.5f);

                    self.effectColor = RWCustom.Custom.HSL2RGB(H, S, Mathf.Clamp(Dark ? (1f - L) : L, 0.01f, 1f), 1f);
                    //self.eyeColor = Dark? Color.white : Color.black;
                    self.eyeColor = Color.Lerp(self.effectColor, Dark ? Color.white : Color.black, 0.8f);
                }
                else
                {
                    self.effectColor = PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Red);
                    self.eyeColor = new Color(0.57255f, 0.11373f, 0.22745f);
                }
            }
        }

        private void GiveHunterDaddyDummyPupTailSize(On.DaddyGraphics.HunterDummy.orig_ctor orig, DaddyGraphics.HunterDummy self, DaddyGraphics dg, int startSprite)
        {
            orig(self, dg, startSprite);
            if (self.owner.EffectColor != PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Red))
            {
                for (int i = 0; i < self.tail.Length; ++i)
                {
                    self.tail[i].connectionRad /= 2;
                    self.tail[i].affectPrevious /= 2;

                }
            }
        }

        private void GiveHunterDaddyPupPalette(On.DaddyGraphics.orig_ApplyPalette orig, DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.daddy.HDmode)
            {
                self.blackColor = palette.blackColor;
                for (int i = 0; i < self.daddy.bodyChunks.Length; i++)
                {
                    sLeaser.sprites[self.BodySprite(i)].color = self.EffectColor;//Change here
                }
                for (int j = 0; j < self.legGraphics.Length; j++)
                {
                    self.legGraphics[j].ApplyPalette(sLeaser, rCam, palette);
                }
                for (int k = 0; k < self.deadLegs.Length; k++)
                {
                    self.deadLegs[k].ApplyPalette(sLeaser, rCam, palette);
                }
                for (int l = 0; l < self.danglers.Length; l++)
                {
                    self.danglers[l].ApplyPalette(sLeaser, rCam, palette);
                }
                self.dummy.ApplyPalette(sLeaser, rCam, palette);
            }
            else orig(self, sLeaser, rCam, palette);
        }

        private void GiveHunterDaddyDummyPupPalette(On.DaddyGraphics.HunterDummy.orig_ApplyPalette orig, DaddyGraphics.HunterDummy self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);//Change here
            Color blackColor = palette.blackColor;
            for (int i = 0; i < self.numberOfSprites - 1; i++)
            {
                sLeaser.sprites[self.startSprite + i].color = color;
            }
            sLeaser.sprites[self.startSprite + 5].color = blackColor;
        }

        private void GiveHunterDaddyDummyPupSprites(On.DaddyGraphics.HunterDummy.orig_DrawSprites orig, DaddyGraphics.HunterDummy self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);

            if (self.darkenFactor > 0f)
            {
                for (int j = 0; j < self.numberOfSprites; j++)
                {
                    Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);
                    sLeaser.sprites[self.startSprite + j].color = new Color(Mathf.Min(1f, color.r * (1f - self.darkenFactor) + 0.01f), Mathf.Min(1f, color.g * (1f - self.darkenFactor) + 0.01f), Mathf.Min(1f, color.b * (1f - self.darkenFactor) + 0.01f));
                }
            }
        }
        /*
        private void GiveHunterDaddyDangleTubePupPallete(On.DaddyGraphics.DaddyDangleTube.orig_ApplyPalette orig, DaddyGraphics.DaddyDangleTube self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.owner.daddy.HDmode)
            {
                Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);//Change here
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? self.owner.EffectColor : color);
                        num++;
                    }
                }
            }
            else orig(self, sLeaser, rCam, palette);
        }

        private void GiveHunterDaddyTubePupPallete(On.DaddyGraphics.DaddyTubeGraphic.orig_ApplyPalette orig, DaddyGraphics.DaddyTubeGraphic self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.owner.daddy.HDmode)
            {
                Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);//Change here

                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? self.owner.EffectColor : color);
                        num++;
                    }
                }
            }
            else orig(self, sLeaser, rCam, palette);
        }
        
        private void GiveHunterDaddyDeadLegPupPallete(On.DaddyGraphics.DaddyDeadLeg.orig_ApplyPalette orig, DaddyGraphics.DaddyDeadLeg self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.owner.daddy.HDmode)
            {
                Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? (self.owner.EffectColor * Mathf.Lerp(0.5f, 0.2f, self.deadness)) : color);
                        num++;
                    }
                }
            }
            else orig(self, sLeaser, rCam, palette);
            
        }*/

        private void GiveHunterDaddyDummyPupSize(On.DaddyGraphics.HunterDummy.orig_Update orig, DaddyGraphics.HunterDummy self)
        {
            if (self.owner.EffectColor != PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Red))
            {
                Vector2 origPos = self.owner.daddy.bodyChunks[0].pos;
                self.owner.daddy.bodyChunks[0].pos = Vector2.Lerp(self.owner.daddy.bodyChunks[0].pos, self.owner.daddy.bodyChunks[1].pos, 0.95f);
                orig(self);
                self.owner.daddy.bodyChunks[0].pos = origPos;
            }
            else orig(self);
        }
        #endregion

        #region Non-spawner creature replacements

        private void PreventSandGrubPullCrash(On.Watcher.SandGrubNetwork.orig_MarkConsumed orig, SandGrubNetwork self)
        {
            if (SlugcatName != WatcherEnums.SlugcatStatsName.Watcher)
                return;
            else
                orig(self);
        }

        private void ReplaceVultureWithSandGrub(On.VultureGrub.orig_PlaceInRoom orig, VultureGrub self, Room placeRoom)
        {
            if(ModManager.Watcher && !game.IsArenaSession && UnityEngine.Random.value < GrubSandGrubChance)
            {
                AddSandWorm(placeRoom, self.abstractPhysicalObject.pos, false);

            }
            else
                orig(self, placeRoom);
        }

        private void AddSandTrapToPopcorn(On.SeedCob.orig_PlaceInRoom orig, SeedCob self, Room placeRoom)
        {
            if (ModManager.Watcher && !game.IsArenaSession && !self.AbstractCob.dead && UnityEngine.Random.value < PopcornSandWormTrapChance)
            {
                AddSandWorm(placeRoom, self.abstractPhysicalObject.pos, true);
            }
            orig(self, placeRoom);
        }

        private void AddSandWorm(Room placeRoom, WorldCoordinate objPos, bool isBig)
        {
            SandGrubBurrow sandGrubBurrow = new SandGrubBurrow(null);
            placeRoom.AddObject(sandGrubBurrow);
            Vector2 origPos = new Vector2(objPos.x, objPos.y + 2);
            Vector2 newPos = new Vector2(((float)origPos.x + 1f) * 20f - 20f, ((float)origPos.y + 1f) * 20f - 20f);
            sandGrubBurrow.pos = placeRoom.FindGroundBelow(newPos, out sandGrubBurrow.dir, 200f);
            SandGrubNetwork sgNet = null;
            foreach (UpdatableAndDeletable udel in placeRoom.updateList)
            {
                if (udel is SandGrubNetwork sgn)
                {
                    sgNet = sgn; break;
                }
            }

            if (sgNet is null)
                placeRoom.AddObject(new SandGrubNetwork(sandGrubBurrow.pos, 1f, 1f, isBig? 1f:0f, true));
            else
            {
                Vector2 netPos = Vector2.zero;
                int count = 0;

                foreach (UpdatableAndDeletable udel in placeRoom.updateList)
                {
                    if (udel is SandGrubBurrow burrow)
                    {
                        count++;
                        netPos += burrow.pos;
                    }
                }
                netPos /= count;
                sgNet.pos = netPos;
            }
        }


        private void AddBoxWormsOnPearls(On.Room.orig_ReadyForAI orig, Room self)
        {
            orig(self);
            if (self.abstractRoom.shelter)
                return;
            float localChance = PearlFireSpriteChance;
            foreach(PlacedObject pobj in self.roomSettings.placedObjects)
            {
                if(pobj.type == PlacedObject.Type.ScavengerTreasury)
                {
                    localChance *= 0.5f;
                    break;
                }
            }
            List<AbstractPhysicalObject> list = new List<AbstractPhysicalObject>();
            foreach (AbstractWorldEntity ent in self.abstractRoom.entities)
            {
                if (ent is AbstractPhysicalObject phob && phob.type == AbstractPhysicalObject.AbstractObjectType.DataPearl && UnityEngine.Random.value < (BalancedSpawns ? localChance : PearlFireSpriteChance))
                    list.Add(phob);
            }
            foreach (AbstractPhysicalObject phob in list)
            {
                AbstractCreature boxWrm = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(WatcherEnums.CreatureTemplateType.BoxWorm), null, new WorldCoordinate(self.abstractRoom.index, phob.pos.x, phob.pos.y - 1, 0), game.GetNewID());
                self.abstractRoom.AddEntity(boxWrm);
                boxWrm.RealizeInRoom();

                AbstractCreature fireSpr = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(WatcherEnums.CreatureTemplateType.FireSprite), null, new WorldCoordinate(self.abstractRoom.index, phob.pos.x, phob.pos.y - 1, 0), game.GetNewID());
                self.abstractRoom.AddEntity(fireSpr);
                fireSpr.RealizeInRoom();
            }
        }

        private void ReplaceHazerWithTardigrade(On.Hazer.orig_PlaceInRoom orig, Hazer self, Room room)
        {
            if (ModManager.Watcher && !game.IsArenaSession && UnityEngine.Random.value < HazerTardigradeChance)
            {
                AbstractCreature tardAbs = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(WatcherEnums.CreatureTemplateType.Tardigrade), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), game.GetNewID());
                tardAbs.state = new Tardigrade.TardigradeState(tardAbs);
                if(UnityEngine.Random.value < 0.1f)
                    (tardAbs.state as Tardigrade.TardigradeState).slayer = true;
                Tardigrade tardi = new Tardigrade(tardAbs, game.world);
                tardAbs.realizedCreature = tardi;
                tardAbs.abstractAI.RealAI = new TardigradeAI(tardAbs, game.world);

                tardi.PlaceInRoom(room);
            }
            else
                orig(self, room);
        }

        private void JellyfishSpawn(On.JellyFish.orig_PlaceInRoom orig, JellyFish self, Room room)
        {
            if (hasSharedDLC && !game.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < GiantJellyfishChance)
            {
                AbstractCreature myBigJelly = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(DLCSharedEnums.CreatureTemplateType.BigJelly), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), game.GetNewID());
                BigJellyFish myJelly = new BigJellyFish(myBigJelly, game.world);
                myJelly.PlaceInRoom(room);
            }
            else
                orig(self, room);
        }

        private void ReplaceStowawayBugBlueFruit(On.DangleFruit.orig_PlaceInRoom orig, DangleFruit self, Room room)
        {
            if(hasSharedDLC && !game.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < StowawayChance)
            {
                self.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(self, room, self.firstChunk.pos);

                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(DLCSharedEnums.CreatureTemplateType.StowawayBug), 
                    null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), game.GetNewID());

                Vector2 pos = new Vector2((self.abstractPhysicalObject.pos.x+1) * 20f - 10f, (self.abstractPhysicalObject.pos.y+1) * 20f - 20f + stalk.ropeLength);
                
                (myStowawayAbstract.state as StowawayBugState).HomePos = new Vector2(pos.x, pos.y);
                pos.y -= 60f;
                (myStowawayAbstract.state as StowawayBugState).aimPos = pos;
                (myStowawayAbstract.state as StowawayBugState).debugForceAwake = true;
                myStowawayAbstract.pos.abstractNode = 0;

                StowawayBug myBug = new StowawayBug(myStowawayAbstract, game.world);

                myBug.AI = new StowawayBugAI(myStowawayAbstract, game.world);

                myBug.PlaceInRoom(room);
            }
            
            orig(self, room);
        }

        private void ReplaceStowawayBugGooieDuck(On.MoreSlugcats.GooieDuck.orig_PlaceInRoom orig, GooieDuck self, Room room)
        {
            if (hasSharedDLC && game.IsStorySession && !room.abstractRoom.shelter && UnityEngine.Random.value < (BalancedSpawns? StowawayChance * 2 : StowawayChance))
            {
                DangleFruit fruit = new DangleFruit(self.abstractPhysicalObject);
                fruit.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(fruit, room, fruit.firstChunk.pos);
                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(DLCSharedEnums.CreatureTemplateType.StowawayBug), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), game.GetNewID());
                Vector2 pos = new Vector2((self.abstractPhysicalObject.pos.x + 1) * 20f - 10f, (self.abstractPhysicalObject.pos.y + 1) * 20f - 20f + stalk.ropeLength);
                
                (myStowawayAbstract.state as StowawayBugState).HomePos = new Vector2(pos.x, pos.y);
                pos.y -= 60f;
                (myStowawayAbstract.state as StowawayBugState).aimPos = pos;
                (myStowawayAbstract.state as StowawayBugState).debugForceAwake = true;
                myStowawayAbstract.pos.abstractNode = 0;

                StowawayBug myBug = new StowawayBug(myStowawayAbstract, game.world);
                myBug.AI = new StowawayBugAI(myStowawayAbstract, game.world);
                myBug.PlaceInRoom(room);
            }
            orig(self, room);
        }

        private void AddScavKings(On.Room.orig_ReadyForAI orig, Room self)
        {
            if (ModManager.MSC && KingScavengerChance > 0 && self.abstractRoom.name != "LC_FINAL" && activeMods.Contains("ShinyKelp.ScavengerTweaks"))
            {
                List<AbstractCreature> elitesList = new List<AbstractCreature>();
                List<AbstractCreature> removedElitesList = new List<AbstractCreature>();
                if (self.game != null)
                {
                    foreach (AbstractCreature abstractCreature in self.abstractRoom.creatures)
                    {
                        if (abstractCreature.realizedCreature == null && abstractCreature.creatureTemplate.type == DLCSharedEnums.CreatureTemplateType.ScavengerElite)
                            elitesList.Add(abstractCreature);

                    }
                }

                float localChance = KingScavengerChance;
                if (self.abstractRoom.scavengerOutpost)
                    localChance *= 4f;
                else if (self.abstractRoom.scavengerTrader)
                    localChance *= 0.5f;

                foreach (AbstractCreature eliteAbstract in elitesList)
                {
                    float value = UnityEngine.Random.value;
                    if (value < (options.balancedSpawns.Value? localChance : KingScavengerChance))
                    {
                        AbstractCreature kingAbstract = new AbstractCreature(eliteAbstract.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing),
                            null, eliteAbstract.pos, eliteAbstract.ID);
                        self.abstractRoom.creatures.Remove(eliteAbstract);
                        self.abstractRoom.creatures.Add(kingAbstract);
                        removedElitesList.Add(eliteAbstract);
                        if (self.abstractRoom.scavengerOutpost)
                            localChance = (KingScavengerChance - 0.04f);
                    }
                }
                orig(self);
                foreach (AbstractCreature removedElite in removedElitesList)
                    self.abstractRoom.creatures.Add(removedElite);
            }
            else
                orig(self);
        }

        #endregion

        #region Helper Spawner Variables

        private int spawnerIndex, originalSpawnCount; 
        private WorldLoader wLoader;
        private int FirstRoomIndex
        {
            get => wLoader.world.firstRoomIndex;
        }
        private int NumberOfRooms
        {
            get => wLoader.world.NumberOfRooms;
        }
        private string Region 
        {
            get => wLoader.worldName;
        }
        private string Subregion
        {
            get => (wLoader.spawners[spawnerIndex].den.room < FirstRoomIndex ||
                    wLoader.spawners[spawnerIndex].den.room >= FirstRoomIndex + NumberOfRooms)? "" : 
                wLoader.abstractRooms[wLoader.spawners[spawnerIndex].den.room - FirstRoomIndex].subregionName;
        }
        private string RoomName
        {
            get => wLoader.spawners[spawnerIndex].den.ResolveRoomName();
        }
        private int SpawnerCount
        {
            get => wLoader.spawners.Count;
        }
        private SlugcatStats.Name SlugcatName
        {
            get => (game is null || !game.IsStorySession) ? null :
                game.GetStorySession.saveState.saveStateNumber;
        }

        #endregion

        #region World Generation Functions

        private void ConfigureRoomAttractions(World self)
        {
            if (ModManager.Watcher)
            {
                //We previously search for the first creature with horizontal screen spawns (miros, deer, etc).
                //And we use that one as the template to set the room attractions for everyone else.
                if (vanillaHorizontalSpawn != null)
                {
                    foreach (AbstractRoom room in self.abstractRooms)
                    {
                        foreach(CreatureTemplate.Type hSpawn in horizontalSpawns)
                        {
                            if(hSpawn != CreatureTemplate.Type.Deer)
                                room.roomAttractions[hSpawn.Index] = room.roomAttractions[vanillaHorizontalSpawn.Index];
                        }
                        if(hasDeers)
                            room.roomAttractions[WatcherEnums.CreatureTemplateType.SkyWhale.Index] = room.roomAttractions[CreatureTemplate.Type.Deer.Index];
                    }
                }
            }
        }

        private void GenerateCustomPopulation(On.WorldLoader.orig_GeneratePopulation orig, WorldLoader worldLoader, bool fresh)
        {
            try
            {
                SetUpLocalVariables(worldLoader);
                if (ForceFreshSpawns && !fresh)
                {
                    fresh = true;
                    foreach (AbstractRoom abstractRoom in worldLoader.abstractRooms)
                    {
                        if (!abstractRoom.shelter)
                        {
                            abstractRoom.creatures.Clear();
                            abstractRoom.entitiesInDens.Clear();
                        }
                    }
                }
                if (fresh)
                {
                    if (!hasUpdatedRefs)
                    {
                        //UnityEngine.Debug.Log("\nUpdating modded creature references...");
                        RefreshModCreatures();
                        //UnityEngine.Debug.Log("Modded creature references updated.\n");
                    }


                    string storySession =  (SlugcatName is null) ? "null" : SlugcatName.ToString();
                    UnityEngine.Debug.Log("Starting spawn modifications for region: " + Region + " , character: " + 
                        storySession);
                    UnityEngine.Debug.Log("ORIGINAL SPAWN COUNT: " + SpawnerCount);
                    UnityEngine.Random.InitState(Mathf.RoundToInt(Time.time * 10f));
                    
                    HandleAllSpawners(worldLoader.spawners);
                    EnsureNormalScavengers(worldLoader);

                    wLoader = null;

                    
                    if (!worldLoader.game.rainWorld.safariMode)
                        AddSpawnersToBuffer(worldLoader);
                    else
                        ResetSpawnBufferOnDeath();

                    ConfigureRoomAttractions(worldLoader.world);
                }
                else
                {
                    ConfigureRoomAttractions(worldLoader.world);

                    if (!worldLoader.game.rainWorld.safariMode)
                    {
                        bool needsFresh = LoadSaveState(worldLoader);
                        //fresh = needsFresh;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }

            orig(worldLoader, fresh);

        }

        private void RefreshModCreatures()
        {
            foreach(CreatureTemplate.Type hType in horizontalSpawns)
            {
                if(hType.index == -1)
                {
                    hType.index = new CreatureTemplate.Type(hType.ToString()).index;
                    if (logSpawners)
                        Debug.Log("Updated Horizontal Spawn Creature: " + hType + " to Index " + hType.index);
                }

            }

            if (!(modCreatureReplacements is null))
            {
                foreach (KeyValuePair<CreatureTemplate.Type, List<ModCreatureReplacement>> pair in modCreatureReplacements)
                {
                    foreach (ModCreatureReplacement modRep in pair.Value)
                    {
                        if(modRep.type.index == -1)
                        {
                            //modRep.type = StaticWorld.GetCreatureTemplate(modRep.type.ToString()).type;
                            //UnityEngine.Debug.Log(modRep.type.ToString() + ":");
                            //UnityEngine.Debug.Log(new CreatureTemplate.Type(modRep.type.ToString()).index);
                            //UnityEngine.Debug.Log(StaticWorld.GetCreatureTemplate(modRep.type.ToString()));
                            modRep.type = new CreatureTemplate.Type(modRep.type.ToString());
                            if (logSpawners)
                                Debug.Log("Updated Mod Replacement creature " + modRep.type + " to Index " + modRep.type.index);
                        }
                    }
                }
            }

            if (!(modCreatureAncestorReplacements is null))
            {
                foreach (KeyValuePair<CreatureTemplate.Type, List<ModCreatureReplacement>> pair in modCreatureAncestorReplacements)
                {
                    foreach (ModCreatureReplacement modRep in pair.Value)
                    {
                        if (modRep.type.index == -1)
                        {
                            //UnityEngine.Debug.Log(modRep.type.ToString() + ":");
                            //UnityEngine.Debug.Log(new CreatureTemplate.Type(modRep.type.ToString()).index);
                            //UnityEngine.Debug.Log(StaticWorld.GetCreatureTemplate(modRep.type.ToString()));
                            modRep.type = new CreatureTemplate.Type(modRep.type.ToString());
                            if (logSpawners)
                                UnityEngine.Debug.Log("Refreshed Mod Replacement Ancestor creature " + modRep.type + " to index " + modRep.type.index);
                        }
                    }
                }
            }

            if(!(modCreatureExtras is null))
            {
                Dictionary<CreatureTemplate.Type, ModCreatureExtras> auxDix = new Dictionary<CreatureTemplate.Type, ModCreatureExtras>();
                foreach (KeyValuePair<CreatureTemplate.Type, ModCreatureExtras> pair in modCreatureExtras)
                {
                    if(pair.Key.index == -1)
                    {
                        auxDix.Add(new CreatureTemplate.Type(pair.Key.ToString()), pair.Value);
                    }
                }

                foreach (KeyValuePair<CreatureTemplate.Type, ModCreatureExtras> pair in auxDix)
                {
                    modCreatureExtras.Remove(pair.Key);
                    modCreatureExtras.Add(pair.Key, pair.Value);
                    if (logSpawners)
                        UnityEngine.Debug.Log("Updated Mod Extras creature " + pair.Key + " to index " + pair.Key.index);
                }
                auxDix.Clear();
            }
            hasUpdatedRefs = true;

            if (logSpawners)
            {
                foreach (CreatureTemplate ct in StaticWorld.creatureTemplates)
                    UnityEngine.Debug.Log(ct.type.ToString());
            }
        }

        private void HandleAllSpawners(List<World.CreatureSpawner> spawners)
        {
            for (int i = 0; i < spawners.Count; i++)
            {
                spawnerIndex = i;
                if (spawners[i].den.room < FirstRoomIndex ||
                    spawners[i].den.room >= FirstRoomIndex + NumberOfRooms)
                {
                    lastWasError = true;
                    if (logSpawners)
                    {
                        UnityEngine.Debug.Log("!!! ERROR SPAWNER FOUND !!!");
                        LogSpawner(spawners[i], i);
                    }
                    continue;
                }
                else
                    lastWasError = false;
                //Log Spawners
                if (logSpawners)
                {
                    if (!lastWasError)
                    {
                        if (i > 0)
                        {
                            UnityEngine.Debug.Log("==AFTER TRANSFORMATIONS==");
                            LogSpawner(spawners[i - 1], i - 1);
                        }
                        LogSpawner(spawners[i], i);
                    }
                }
                if (spawners[i] is World.SimpleSpawner simpleSpawner)
                {   
                    if(!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.EndsWith("<AUYS_SKIP>"))
                    {
                        simpleSpawner.spawnDataString = simpleSpawner.spawnDataString.Remove(simpleSpawner.spawnDataString.Length - 11);
                        if (simpleSpawner.spawnDataString.Length <= 1)
                            simpleSpawner.spawnDataString = null;
                        continue;
                    }

                    if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle"))
                        HandlePrecycleSpawns(simpleSpawner, spawners);

                    if ((simpleSpawner.nightCreature || (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("Night"))) 
                        && (simpleSpawner.spawnDataString is null || !simpleSpawner.spawnDataString.Contains("Ignorecycle")))
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraNightCreatures, true);
                        if (hasAngryInspectors && hasSharedDLC) 
                        {
                            if((Region == "LC" && RoomName != "LCOffScreenDen") || 
                                (Region == "UW" && RoomName != "UWOffScreenDen"))
                            {
                                bool addedSpawner =
                                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns? InspectorChance / 2 : InspectorChance, true);
                                if (addedSpawner)
                                    (spawners[spawners.Count - 1] as World.SimpleSpawner).spawnDataString = "{Ignorecycle}";
                            }
                        } 
                    }

                    if (IsLizard(simpleSpawner))
                    {
                        HandleLizardSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }
                    
                    if (IsCentipede(simpleSpawner))
                    {
                        if (hasAngryInspectors && Subregion == "Memory Crypts"
                            && simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                            AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns? InspectorChance * 4 : InspectorChance);
                        HandleCentipedeSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture || simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture
                        || simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.MirosVulture)
                    {
                        HandleVultureSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    {
                        if(Region == "GW")
                            IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns? ExtraSpiders*2 : ExtraSpiders, true);
                        else
                        {
                            IncreaseCreatureSpawner(simpleSpawner, (Region == "SB" && BalancedSpawns)? ExtraSpiders-10 : ExtraSpiders, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SpitterSpider, SpitterSpiderChance);
                        }
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.SpitterSpider)
                    {
                        if (Region == "GW" && BalancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, ExtraSpiders, true);
                        if (Region == "UW" || Region == "CL" || Region == "GW")
                            HandleLongLegsSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Spider)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraSmallSpiders);
                        AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.MotherSpider, MotherSpiderChance, true, true);
                        goto ModCreaturesSpawner;
                    }
                    if(simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.MotherSpider)
                    {
                        if (BalancedSpawns && Subregion == "The Gutter")
                        {
                            World.SimpleSpawner aSpawner = CopySpawner(simpleSpawner);
                            aSpawner.amount = 0;
                            aSpawner.creatureType = CreatureTemplate.Type.BigSpider;
                            spawners.Add(aSpawner);
                        }
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.DropBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraDropwigs, true);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigEel)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraLeviathans, true);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.MirosBird)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraMiros);
                        if (Region == "SB" && ExtraMiros > 0 && BalancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 2);
                        if (Region == "LC" && ExtraMiros > 0 && BalancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 4);

                        if (ModManager.Watcher)
                        {
                            if(simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                            {
                                float localMultiplierLoach = BalancedSpawns && Region == "SH" ? 0.5f : 1f;
                                float localMultiplierDrill = 1f;
                                if (BalancedSpawns)
                                {
                                    if (Region == "SH")
                                        localMultiplierDrill = 0.5f;
                                    else if (Region == "LC")
                                        localMultiplierDrill = 1.5f;
                                }
                                if (!AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.DrillCrab, MirosDrillCrabChance * localMultiplierDrill, true, false, true, true))
                                {
                                    AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Loach, MirosLoachChance * localMultiplierLoach, true, false, true, true);
                                }
                            }
                        }

                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Scavenger)
                    {
                        int localExtraScavs = ExtraScavengers;
                        if (BalancedSpawns)
                        {
                            if (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer || Region == "SB")
                                localExtraScavs /= 2;
                            else if (Region == "LC")
                                localExtraScavs = (int)(localExtraScavs * 1.5f);
                        }
                        IncreaseCreatureSpawner(simpleSpawner, localExtraScavs, false);
                        ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.ScavengerElite, EliteScavengerChance);
                        if(ModManager.Watcher)
                        {
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.ScavengerTemplar, ScavengerTemplarChance);
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.ScavengerDisciple, ScavengerDiscipleChance);
                        }
                        goto ModCreaturesSpawner;
                    }
                    
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                    {
                        HandleJetfishSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.TentaclePlant)
                    {
                        if (hasAngryInspectors && Region == "SB" && RoomName == "SB_G03")
                            AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns ? InspectorChance * 4 : InspectorChance, true);
                        IncreaseCreatureSpawner(simpleSpawner, ExtraKelp, true);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Leech || simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech 
                        || simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.JungleLeech)
                    {
                        HandleLeechSpawner(simpleSpawner, spawners);
                        if (ModManager.Watcher)
                            AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Frog, LeechFrogChance, true, true, true);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.EggBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraEggbugs, true);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.FireBug, FireBugChance);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.TubeWorm)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraTubeworms);
                        AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigSpider, TubeWormBigSpiderChance);
                        if (hasAngryInspectors && Region == "LC" && RoomName == "LC_station01")
                        {
                            AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns ? InspectorChance * 4 : InspectorChance, true);
                        }
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.CicadaA || simpleSpawner.creatureType == CreatureTemplate.Type.CicadaB)
                    {
                        HandleCicadaSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Snail)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (Region == "DS" && BalancedSpawns) ? ExtraSnails-10 : ExtraSnails, true);
                        HandleLongLegsSpawner(simpleSpawner, spawners);
                        if (ModManager.Watcher && simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Barnacle, SnailBarnacleChance, true);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.LanternMouse)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (Region == "SH" && BalancedSpawns) ? ExtraLMice - 10 : ExtraLMice, true);
                        HandleLongLegsSpawner(simpleSpawner, spawners);
                        if (ModManager.Watcher)
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Rat, MouseRatChance, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.BigNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= originalSpawnCount)
                            AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SmallNeedleWorm, 1f);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= originalSpawnCount && simpleSpawner.amount < 2)
                            IncreaseCreatureSpawner(simpleSpawner, 15, true);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.PoleMimic)
                    {
                        if(ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.TentaclePlant, MonsterKelpChance))
                            IncreaseCreatureSpawner(simpleSpawner, ExtraKelp, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.Yeek)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (Region == "OE" && BalancedSpawns) ? ExtraYeeks - 10 : ExtraYeeks, true);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, UnityEngine.Random.value < (Region == "OE" ? .8f : .5f)? 
                            DLCSharedEnums.CreatureTemplateType.ZoopLizard : DLCSharedEnums.CreatureTemplateType.SpitLizard, YeekLizardChance);
                        if (replacedFull)
                            HandleLizardSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.Inspector)
                    {
                        simpleSpawner.spawnDataString = "{Ignorecycle}";
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Deer)
                    {
                        int deerAmount = simpleSpawner.amount;
                        IncreaseCreatureSpawner(simpleSpawner, ExtraDeer);
                        if (ModManager.Watcher)
                        {
                            if(AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.DrillCrab, DeerDrillCrabInvChance, true, false, false, true))
                            {
                                World.SimpleSpawner drillSpawner = spawners[spawners.Count - 1] as World.SimpleSpawner;
                                if (BalancedSpawns)
                                    drillSpawner.amount -= 2;
                            }
                            else if(AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Loach, DeerLoachInvChance, true, false, false, true))
                            {
                                World.SimpleSpawner loachSpawner = spawners[spawners.Count - 1] as World.SimpleSpawner;
                                if (BalancedSpawns)
                                    loachSpawner.amount = Mathf.CeilToInt(loachSpawner.amount * 0.25f);
                            }
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.SkyWhale, DeerSkywhaleChance, true);
                            if (simpleSpawner.amount < 1)
                                simpleSpawner.amount = 2;
                        }
                        goto ModCreaturesSpawner;
                    }

                    if (ModManager.Watcher)
                    {
                        if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.BigMoth)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, BigMothExtras);
                            if(simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                            {
                                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Vulture, BigMothVultureChance, true);
                            }
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.SmallMoth)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, SmallMothExtras, true);
                            if (simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.CicadaA, SmallMothCicadaChance, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigNeedleWorm, SmallMothNoodleflyChance, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, SmallMothCentiwingChance, true);
                            goto ModCreaturesSpawner;
                        }
                        if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.DrillCrab)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, DrillCrabExtras);
                            if(simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                            {
                                if (!AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.MirosBird, DrillCrabMirosChance, true, false, true, true))
                                    AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Loach, DrillCrabLoachChance, true, false, true, true);
                            }
                            goto ModCreaturesSpawner;
                        }
                        if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Loach)
                        {
                            if(simpleSpawner.inRegionSpawnerIndex >= originalSpawnCount && BalancedSpawns && Region != "LC")
                                simpleSpawner.amount = Mathf.CeilToInt(simpleSpawner.amount * 0.35f);
                            IncreaseCreatureSpawner(simpleSpawner, LoachExtras);
                            if(simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                            {   
                                if (!AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.MirosBird, LoachMirosChance, true, false, true, true))
                                    AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.DrillCrab, LoachDrillCrabChance, true, false, true, true);
                            }
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.RotLoach, RotLoachChance);
                            goto ModCreaturesSpawner;
                        }
                        if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Barnacle)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, BarnacleExtras, true);
                            if (simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Snail, BarnacleSnailChance, true);
                            HandleLongLegsSpawner(simpleSpawner, spawners);
                            goto ModCreaturesSpawner;
                        }
                        if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.SkyWhale)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, SkywhaleExtras);
                            goto ModCreaturesSpawner;
                        }
                        if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Frog)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, FrogExtras); 
                            goto ModCreaturesSpawner;
                        }
                        if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Rat)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, RatExtras);
                            goto ModCreaturesSpawner;
                        }
                    }

                ModCreaturesSpawner:
                    CheckModCreatures(simpleSpawner, spawners);
                    AfterModAdjustments(simpleSpawner, spawners);

                }
                else if(spawners[i] is World.Lineage lineage)
                {

                    if(FillLineages)
                        FillLineage(lineage);
                    if (ForceFreshSpawns && FillLineages)
                        RandomizeLineageFirst(lineage);

                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, true))
                    {
                        HandleLizardLineage(lineage, spawners);
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, true))
                    {
                        HandleLongLegsLineage(lineage, spawners);
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, true))
                    {
                        HandleCentipedeLineage(lineage, spawners);
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.BigSpider))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, SpitterSpiderChance);
                        if(ForceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BigSpider.Index)
                        {
                            World.SimpleSpawner asimpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BigSpider, lineage.spawnData[0], 1);
                            spawners.Add(asimpleSpawner);
                        }
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.JetFish))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, DLCSharedEnums.CreatureTemplateType.AquaCenti, AquapedeChance);
                        if(Region == "SL")
                        {
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, CreatureTemplate.Type.BrotherLongLegs, AquapedeChance);
                            HandleLongLegsLineage(lineage, spawners);
                        }
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.EggBug))
                    {
                        if (BalancedSpawns && ModManager.MSC)
                        {
                            lineage.creatureTypes[0] = CreatureTemplate.Type.EggBug.Index;
                            for (int j = 1; j < lineage.creatureTypes.Length; ++j)
                            {
                                lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.FireBug.Index;
                            }
                        }
                        else
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.EggBug, MoreSlugcatsEnums.CreatureTemplateType.FireBug, FireBugChance);
                        goto ModCreaturesLineage;
                    }

                    if(IsCreatureInLineage(lineage, CreatureTemplate.Type.PoleMimic))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.PoleMimic, CreatureTemplate.Type.TentaclePlant, MonsterKelpChance);
                        goto ModCreaturesLineage;
                    }

                ModCreaturesLineage:
                    CheckModCreatures(lineage, spawners);
                    AfterModAdjustments(lineage, spawners);
                }
            }
        }

        private bool triedEchoLevi = false, hasBlackLizards = false;
        private void AfterModAdjustments(World.SimpleSpawner spawner, List<World.CreatureSpawner> spawners)
        {
            if (activeMods.Contains("Outspector") && spawner.creatureType == new CreatureTemplate.Type("Outspector"))
            {
                spawner.spawnDataString = "{IgnoreCycle}";
                if (hasAngryInspectors)
                    AddInvasionSpawner(spawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, options.GetConfigValue("InspectorOutspectorInvChance") / 100f);
                ReplaceMultiSpawner(spawner, spawners, new CreatureTemplate.Type("OutspectorB"), 0.3f);
            }

            if (activeMods.Contains("lb-fgf-m4r-ik.mini-levi") && spawner.creatureType == new CreatureTemplate.Type("MiniLeviathan"))
            {
                if (Region == "SL")
                    spawner.spawnDataString = "{AlternateForm}";
            }

            if (activeMods.Contains("lb-fgf-m4r-ik.cool-thorn-bug") && spawner.creatureType == new CreatureTemplate.Type("ThornBug"))
            {
                if (Region == "UW" || Region == "CC" || Region == "SH")
                    spawner.spawnDataString = "{AlternateForm}";
            }

            if(activeMods.Contains("drainmites") && spawner.creatureType == new CreatureTemplate.Type("DrainMite"))
            {
                if(spawner.spawnDataString is null || spawner.spawnDataString == "" || !DrainMiteStringValidFormat(spawner.spawnDataString))
                {
                    IncreaseCreatureSpawner(spawner, options.GetConfigValue("DrainMiteExtras"), false);
                    spawner.amount /= 4;
                    World.SimpleSpawner spawner2 = CopySpawner(spawner);
                    World.SimpleSpawner spawner3 = CopySpawner(spawner);
                    World.SimpleSpawner spawner4 = CopySpawner(spawner);
                    spawner2.inRegionSpawnerIndex += 1;
                    spawner3.inRegionSpawnerIndex += 2;
                    spawner4.inRegionSpawnerIndex += 3;
                    spawners.Add(spawner2);
                    spawners.Add(spawner3);
                    spawners.Add(spawner4);

                    float minSize = 0.5f, maxSize = 1.5f;

                    if (Region == "CC" || Region == "SI" || Region == "LF")
                    {
                        minSize = 0.5f;
                        maxSize = 1.0f;
                    }
                    else if(Region == "SB" || Region == "VS")
                    {
                        minSize = 1.0f;
                        maxSize = 1.5f;
                    }

                    float sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner.spawnDataString = "{BodySize:" + sizeMult + "}";
                    sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner2.spawnDataString = "{BodySize:" + sizeMult + "}<AUYS_SKIP>";
                    sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner3.spawnDataString = "{BodySize:" + sizeMult + "}<AUYS_SKIP>";
                    sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner4.spawnDataString = "{BodySize:" + sizeMult + "}<AUYS_SKIP>";
                }
            }

            if(activeMods.Contains("ShinyKelp.AlbinoKings") && (spawner.creatureType == CreatureTemplate.Type.Vulture ||
                spawner.creatureType == CreatureTemplate.Type.KingVulture || spawner.creatureType ==
                DLCSharedEnums.CreatureTemplateType.MirosVulture))
            {
                if(spawner.spawnDataString is null || !spawner.spawnDataString.Contains("AlternateForm"))
                {
                    int currentCount = spawners.Count;
                    CreatureTemplate.Type prevType = spawner.creatureType;
                    bool full =
                        ReplaceMultiSpawner(spawner, spawners, CreatureTemplate.Type.StandardGroundCreature, options.GetConfigValue("AlbinoVultureChance") * 0.01f);
                    if (full)
                    {
                        spawner.spawnDataString = "{AlternateForm}";
                        spawner.creatureType = prevType;
                    }
                    else if(currentCount < spawners.Count)
                    {
                        World.SimpleSpawner newSpawner = spawners[spawners.Count - 1] as World.SimpleSpawner;
                        newSpawner.spawnDataString = "{AlternateForm}<AUYS_SKIP>";
                        newSpawner.creatureType = prevType;
                    }
                }
            }
            if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                if (!triedEchoLevi && spawner.creatureType.index > -1 && spawner.creatureType.index < StaticWorld.creatureTemplates.Length && (
                    StaticWorld.creatureTemplates[spawner.creatureType.index].TopAncestor().type == CreatureTemplate.Type.Vulture ||
                    StaticWorld.creatureTemplates[spawner.creatureType.index].TopAncestor().type == DLCSharedEnums.CreatureTemplateType.MirosVulture))
                {
                    triedEchoLevi = true;
                    if(UnityEngine.Random.value < options.GetConfigValue("VultureEchoLeviChance") * 0.01f)
                    {
                        World.SimpleSpawner echoSpawner = CopySpawner(spawner);
                        echoSpawner.amount = 1;
                        IncreaseCreatureSpawner(spawner, options.GetConfigValue("EchoLeviExtras"), true);
                        echoSpawner.inRegionSpawnerIndex = spawners.Count;
                        echoSpawner.spawnDataString = "<AUYS_SKIP>";
                        echoSpawner.nightCreature = false;
                        echoSpawner.creatureType = new CreatureTemplate.Type("FlyingBigEel");
                        spawners.Add(echoSpawner);
                    }
                }
                
            }
        }

        private void AfterModAdjustments(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            if(activeMods.Contains("drainmites") && IsCreatureInLineage(lineage, new CreatureTemplate.Type("DrainMite")))
            {
                int miteIndex = new CreatureTemplate.Type("DrainMite").index;
                for(int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] == miteIndex)
                    {
                        lineage.creatureTypes[i] = -1;
                        lineage.spawnData[i] = "";
                    }
                }
                if (options.fillLineages.Value)
                    FillLineage(lineage);
            }
            if (activeMods.Contains("ShinyKelp.AlbinoKings") && (IsCreatureInLineage(lineage, CreatureTemplate.Type.Vulture)
                || IsCreatureInLineage(lineage, CreatureTemplate.Type.Vulture)
                || IsCreatureInLineage(lineage, DLCSharedEnums.CreatureTemplateType.MirosVulture)))
            {
                for(int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    int index = lineage.creatureTypes[i];
                    if(index > -1 && index < StaticWorld.creatureTemplates.Length)
                    {
                        if (StaticWorld.creatureTemplates[index].type ==
                            CreatureTemplate.Type.Vulture ||
                            IsCreatureInLineage(lineage, CreatureTemplate.Type.KingVulture) ||
                            StaticWorld.creatureTemplates[index].type ==
                            DLCSharedEnums.CreatureTemplateType.MirosVulture)
                        {
                            if (lineage.spawnData[i] is null || !lineage.spawnData[i].Contains("AlternateForm"))
                            {
                                if (UnityEngine.Random.value < options.GetConfigValue("AlbinoVultureChance") / 100f)
                                    lineage.spawnData[i] = "{AlternateForm}";
                            }
                        }
                    }
                }
            }
        }

        
        #endregion

        #region Vanilla spawner functions

        private bool IsLizard(World.SimpleSpawner spawner)
        {
            return StaticWorld.GetCreatureTemplate(spawner.creatureType)?.TopAncestor().type == 
                CreatureTemplate.Type.LizardTemplate;
        }
        
        private bool IsVanillaLizard(World.SimpleSpawner spawner)
        {
            return spawner.creatureType == CreatureTemplate.Type.GreenLizard || spawner.creatureType == CreatureTemplate.Type.PinkLizard ||
                spawner.creatureType == CreatureTemplate.Type.BlueLizard || spawner.creatureType == CreatureTemplate.Type.YellowLizard ||
                spawner.creatureType == CreatureTemplate.Type.WhiteLizard || spawner.creatureType == CreatureTemplate.Type.BlackLizard ||
                spawner.creatureType == CreatureTemplate.Type.RedLizard || spawner.creatureType == CreatureTemplate.Type.CyanLizard ||
                spawner.creatureType == CreatureTemplate.Type.Salamander || spawner.creatureType == DLCSharedEnums.CreatureTemplateType.SpitLizard ||
                spawner.creatureType == DLCSharedEnums.CreatureTemplateType.ZoopLizard || spawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard ||
                spawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard;
        }
        
        private bool IsCentipede(World.SimpleSpawner spawner)
        {
            return StaticWorld.GetCreatureTemplate(spawner.creatureType)?.TopAncestor().type ==
                CreatureTemplate.Type.Centipede;  
        }

        private void HandleLizardSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if(simpleSpawner.creatureType == CreatureTemplate.Type.Salamander
               || simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard)
            {
                HandleAxolotlSpawner(simpleSpawner, spawners);
                return;
            }


            //Info check before changes
            float localRedLizardChance = RedLizardChance;
            if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard || (
                (Region == "SU" || Region == "HI") && (
                SlugcatName == SlugcatStats.Name.Red || ( 
                hasSharedDLC && (
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear 
                )))))
            {
                if(localRedLizardChance < 1)
                    localRedLizardChance /= 2;
            }

            bool replaceForRyan = hasLizardVariants && RyanLizardChance > 0 && simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard;
            int currentCount = SpawnerCount;

            //Checks for normal lizards
            if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Region == "SU")? ExtraGreens/2 : ExtraGreens, true);
                bool replacedFull =
                    ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.SpitLizard, CaramelLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Region == "OE")? ExtraCaramels-10:ExtraCaramels, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraPinks, true);
                bool replacedFull =
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.ZoopLizard, StrawberryLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, ExtraZoops, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraBlues, true);
                bool replacedFull =
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.CyanLizard, CyanLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Region == "UW")? ExtraCyans/2 : ExtraCyans, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Subregion == "Filtration System") ? ExtraBlacks - 10 : ExtraBlacks, true);
                hasBlackLizards = true;
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.WhiteLizard)
                IncreaseCreatureSpawner(simpleSpawner, ExtraWhites, true);
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard)
                IncreaseCreatureSpawner(simpleSpawner, 
                    (BalancedSpawns && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Rivulet || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                    ? ExtraYellows/2 : ExtraYellows, true);
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Region == "UW") ? ExtraCyans / 2 : ExtraCyans, true);
            else if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.SpitLizard)
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Region == "OE") ? ExtraCaramels - 10 : ExtraCaramels, true);
            else if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.ZoopLizard)
                IncreaseCreatureSpawner(simpleSpawner, ExtraZoops, true);

            if (BalancedSpawns && Region == "GW" && ExtraWhites > 0 && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                    AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.WhiteLizard, 0.4f);
            }

            //Red&Train lizard
            if (IsVanillaLizard(simpleSpawner) && simpleSpawner.creatureType != CreatureTemplate.Type.RedLizard)
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedLizard, BalancedSpawns ? localRedLizardChance : RedLizardChance);
                
            if(simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard)
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, TrainLizardChance);

            //Watcher
            if (ModManager.Watcher)
            {
                if(simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard || simpleSpawner.creatureType ==
                    DLCSharedEnums.CreatureTemplateType.SpitLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.IndigoLizard, GroundIndigoLizChance);

                if (simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.BasiliskLizard, BlackBasiliskLizChance);

                if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.IndigoLizard)
                    IncreaseCreatureSpawner(simpleSpawner, IndigoLizExtras, true);

                if(simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.BasiliskLizard)
                    IncreaseCreatureSpawner(simpleSpawner, BasiliskLizExtras, true);

                float blizLizChance = BlizzardLizardChance;
                if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard)
                    blizLizChance /= 2;
                if (Subregion == "Bitter Aerie" || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                    blizLizChance *= 1.8f;

                AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.BlizzardLizard, BalancedSpawns ? blizLizChance : BlizzardLizardChance, true, false, true);
            }

            //Mods
            if (replaceForRyan)
            {
                if(simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard)
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), 1f);
                }
                if(SpawnerCount > currentCount)
                {
                    ReplaceMultiSpawner(spawners[spawners.Count-1] as World.SimpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), 1f);
                }
            }
            if (activeMods.Contains("thefriend"))
            {
                if(simpleSpawner.creatureType == StaticWorld.GetCreatureTemplate("MotherLizard").type)
                {
                    AddInvasionSpawner(simpleSpawner, spawners, StaticWorld.GetCreatureTemplate("YoungLizard").type, 1f);
                    (spawners[spawners.Count - 1] as World.SimpleSpawner).amount = (UnityEngine.Random.value > .5f ? 3 : 4); 
                }
            }
        }

        private void HandleAxolotlSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Salamander)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraSals, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.EelLizard, EelLizChance);
            }
            if (hasSharedDLC && simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraEellizs, true);
            }

        }

        private void HandleCentipedeSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if(simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.AquaCenti)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraAquapedes, true);
                return;
            }
            bool wasSmallCentipedes = false;
            if(simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
            {
                wasSmallCentipedes = true;
                IncreaseCreatureSpawner(simpleSpawner, ((Region == "OE" || Region == "SB" || Region == "VS") && BalancedSpawns)? ExtraSmallCents-10 : ExtraSmallCents);
                if(!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("AlternateForm"))
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, LargeCentipedeChance);
                else
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, LargeCentipedeChance);

            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
            {
                if(!wasSmallCentipedes)
                    IncreaseCreatureSpawner(simpleSpawner, ((Region == "SB" || Region == "VS") && BalancedSpawns)? ExtraCentipedes-10 : ExtraCentipedes, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, ((Region == "VS" || Region == "SB") && BalancedSpawns && RedCentipedeChance < 1) ? RedCentipedeChance / 2 : RedCentipedeChance);                                
            }
            bool isCentiwing = simpleSpawner.creatureType == CreatureTemplate.Type.Centiwing;
            if(isCentiwing)
            {
                if(!wasSmallCentipedes && BalancedSpawns)
                    IncreaseCreatureSpawner(simpleSpawner, ExtraCentiwings, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, (Region == "LC" && BalancedSpawns)? RedCentipedeChance*2 : RedCentipedeChance);
            }

            if(BalancedSpawns && Region == "GW" && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, LargeCentipedeChance * 2);
                if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, RedCentipedeChance);
            }
        }

        private void HandleVultureSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {

            if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture)
            {
                if (ExtraVultures > 0) 
                {
                    IncreaseCreatureSpawner(simpleSpawner, ExtraVultures);
                    if (Region == "OE" && BalancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 2);
                    if (Region == "SI" && BalancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 3);
                }
                float localKingVultureChance = KingVultureChance;
                if (BalancedSpawns && (
                    Region == "SI" || Region == "LC" ||
                    SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Rivulet ||
                    SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                    localKingVultureChance *= 2;
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.KingVulture, localKingVultureChance);
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture && Region == "UW")
                IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns? ExtraVultures+1 : ExtraVultures);

            float localMirosVultureChance = MirosVultureChance;
            if ((Region == "OE" || Region == "SI" || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint) && localMirosVultureChance < 1)
                localMirosVultureChance /= 2;
            else if (Region == "LM" || Region == "LC" || Region == "MS" || Region == "SD" ||
                (Region == "GW" && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear)))
                localMirosVultureChance *= 2;
            
            ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.MirosVulture, BalancedSpawns? localMirosVultureChance : MirosVultureChance);

            if (Region == "SH" && simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.MirosVulture && MirosVultureChance > 0)
                IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns? ExtraVultures+1 : ExtraVultures);

            //Watcher
            if (ModManager.Watcher)
            {
                if(simpleSpawner.inRegionSpawnerIndex < originalSpawnCount &&
                    (simpleSpawner.creatureType == CreatureTemplate.Type.Vulture || simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture))
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.BigMoth, VultureBigMothChance, true);
                }
            }

        }
        
        private void HandleLongLegsSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (!(StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs))
            {
                
                if (Region == "UW" || Region == "CL")
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, BalancedSpawns ? BrotherLongLegsChance * 2 : BrotherLongLegsChance);
                else if (BalancedSpawns && Region == "GW" && simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.TerrorLongLegs, 
                        (SlugcatName.ToString() == "Artificer" || SlugcatName.ToString() == "Spear")? BrotherLongLegsChance*2:BrotherLongLegsChance);
                else
                {
                    float localBrotherChance = BrotherLongLegsChance;
                    if (BalancedSpawns)
                    {
                        if (Subregion == "Sump Tunnel" || Subregion == "The Gutter")
                            localBrotherChance *= 2;
                        if (simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                            localBrotherChance *= 1.5f;
                        if (simpleSpawner.spawnDataString != null && simpleSpawner.spawnDataString.Contains("PreCycle"))
                            localBrotherChance *= 2;
                    }
                    if(activeMods.Contains("Croken.Mimicstarfish") && options.GetConfigValue("BllMimicstarfishChance") > 0 &&
                        (Region == "SL" || Region == "LM" || Region == "MS" || (Region == "DS" && UnityEngine.Random.value > 0.5f)))
                        ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Mimicstar"), localBrotherChance);
                    else
                        ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, localBrotherChance);
                }
            }

            if(simpleSpawner.creatureType == CreatureTemplate.Type.BrotherLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, DaddyLongLegsChance);
            }
            
            if(simpleSpawner.creatureType == CreatureTemplate.Type.DaddyLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.TerrorLongLegs, TerrorLongLegsChance);
            }

            if (hasAngryInspectors &&
                StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
            {
                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, InspectorChance);
            }


        }

        private void HandleJetfishSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (Region == "SL" && BalancedSpawns) ? ExtraJetfish - 10 : ExtraJetfish, true);
            if (AquapedeChance > 0 && (Region == "LM" || Region == "SB" || Region == "VS") && BalancedSpawns)
                IncreaseCreatureSpawner(simpleSpawner, 12, true);

            float localWaterPredatorChance = AquapedeChance;
            if (Region == "SB" || Region == "VS")
                localWaterPredatorChance *= 2f;
            else if (Region == "SL")
                localWaterPredatorChance *= 0.6f;

            bool replacedFull;
            replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, JetfishSalamanderChance, true);
            if(!replacedFull)
                replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.AquaCenti, BalancedSpawns? localWaterPredatorChance : AquapedeChance);

            if ((Region == "SL" || Region == "CL") && !replacedFull && AquapedeChance > 0)
                HandleLongLegsSpawner(simpleSpawner, spawners);
        }
        
        private void HandleCicadaSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (Region == "SI" || Region == "OE")? ExtraCicadas - 10 : ExtraCicadas, true);

            bool replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, (Region == "SI" || Region == "LC")? CicadaCentiwingChance / 2: CicadaCentiwingChance, true);
            if (!replacedFull)
            {
                int spawnCount = spawners.Count;
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigNeedleWorm, CicadaNoodleflyChance, true);
                if (ModManager.Watcher && simpleSpawner.inRegionSpawnerIndex < originalSpawnCount)
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.SmallMoth, CicadaSmallMothChance, true);
                }
            }
           
        }

        private void HandleLeechSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool wasRedLeech = false;
            if(simpleSpawner.creatureType == CreatureTemplate.Type.Leech)
            {
                wasRedLeech = true;
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Region == "DS")? (int)(ExtraLeeches*1.5f) : ExtraLeeches);
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.JungleLeech, JungleLeechChance);
                AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, (BalancedSpawns && Region == "DS")? LeechLizardChance*2 : LeechLizardChance, true, true);
            }
            if(simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.JungleLeech)
            {
                if(!wasRedLeech)
                    AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.EelLizard, LeechLizardChance, true, true);
            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraLeeches);
                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.AquaCenti, SeaLeechAquapedeChance, true, true);
                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.EelLizard, LeechLizardChance, true, true);
            }
        }

        private void HandlePrecycleSpawns(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            int extras = ExtraPrecycleSals;
            if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard ||
                StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                extras-= 10;
            IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns ? extras : ExtraPrecycleSals, true);
        }

        private void IncreaseCreatureSpawner(World.SimpleSpawner simpleSpawner, int amount = 1, bool divide = false)
        {
            if (amount <= 0)
                return;
            if (BalancedSpawns && bannedRooms.Contains(simpleSpawner.den.ResolveRoomName()))
                return;

            if (!divide)
            {
                simpleSpawner.amount += UnityEngine.Random.Range(0, amount + 1);
            }
            else
            {
                int loadedResult = UnityEngine.Random.Range(0, amount);
                simpleSpawner.amount += (int)Mathf.Floor(loadedResult / 10);
                if (UnityEngine.Random.value < (loadedResult % 10) / 10f)
                    simpleSpawner.amount++;
            }
        }

        private bool ReplaceMultiSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type replacement, float chance, bool forceNewSpawner = false)
        {
            if (replacement == null)
                return false;
            if (simpleSpawner.creatureType == replacement)
                return false;
            if (forceNewSpawner && SpawnerCount > originalSpawnCount * 4)
                return false;

            bool replacedFull = false;

            if (simpleSpawner.amount <= 1)
            {
                if (UnityEngine.Random.value < chance)
                {
                    if (!forceNewSpawner)
                    {
                        simpleSpawner.creatureType = replacement;
                        replacedFull = true;
                    }
                    else
                    {
                        World.SimpleSpawner replacementSpawner = CopySpawner(simpleSpawner);
                        replacementSpawner.creatureType = replacement;
                        replacementSpawner.amount = 1;
                        simpleSpawner.amount = 0;
                        replacementSpawner.inRegionSpawnerIndex = spawners.Count;
                        spawners.Add(replacementSpawner);

                    }
                }
            }
            else
            {
                int winningRolls = 0;
                for (int i = 0; i < simpleSpawner.amount; ++i)
                {
                    if (UnityEngine.Random.value < chance)
                        winningRolls++;
                }

                if (winningRolls > 0)
                {
                    if (winningRolls == simpleSpawner.amount && !forceNewSpawner)
                    {
                        simpleSpawner.creatureType = replacement;
                        replacedFull = true;
                    }
                    else
                    {
                        World.SimpleSpawner replacementSpawner = CopySpawner(simpleSpawner);
                        replacementSpawner.creatureType = replacement;
                        replacementSpawner.amount = winningRolls;
                        simpleSpawner.amount -= winningRolls;

                        replacementSpawner.inRegionSpawnerIndex = spawners.Count;
                        spawners.Add(replacementSpawner);
                    }
                }
            }
            return replacedFull;
        }
    
        private bool AddInvasionSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type invador, float chance, bool singleRoll = false, bool reduceInvaded = false, bool removeInvaded = false, bool mirrorAmount = false)
        {
            if (invador == null)
                return false;
            if (simpleSpawner.creatureType == invador)
                return false;
            if (SpawnerCount > originalSpawnCount * 4)
                return false;

            bool wonRoll = false;

            if (singleRoll)
                wonRoll = UnityEngine.Random.value < chance;
            else
            {
                for (int i = 0; i < simpleSpawner.amount; ++i)
                {
                    if (UnityEngine.Random.value < chance)
                    {
                        wonRoll = true;
                        break;
                    }
                }
            }

            if (wonRoll)
            {
                World.SimpleSpawner invasionSpawner = CopySpawner(simpleSpawner);
                invasionSpawner.creatureType = invador;
                if (mirrorAmount)
                    invasionSpawner.amount = simpleSpawner.amount;
                else
                    invasionSpawner.amount = 1;
                invasionSpawner.inRegionSpawnerIndex = spawners.Count;
                spawners.Add(invasionSpawner);
            }
            if(wonRoll && reduceInvaded)
                simpleSpawner.amount = (int)Mathf.Round(simpleSpawner.amount * 0.6f);
            if (wonRoll && removeInvaded)
                simpleSpawner.amount = 0;

            return wonRoll;
        }

        private World.SimpleSpawner CopySpawner(World.SimpleSpawner origSpawner)
        {
            World.SimpleSpawner newSpawner = new World.SimpleSpawner(origSpawner.region, origSpawner.inRegionSpawnerIndex, origSpawner.den,
                origSpawner.creatureType, origSpawner.spawnDataString, origSpawner.amount);
            newSpawner.nightCreature = origSpawner.nightCreature;
            return newSpawner;
        }

        private void SetSpawnSkip(World.SimpleSpawner simpleSpawner)
        {
            if (simpleSpawner.spawnDataString != null)
                simpleSpawner.spawnDataString += "<AUYS_SKIP>";
            else
                simpleSpawner.spawnDataString = "<AUYS_SKIP>";
        }

        #endregion

        #region Vanilla lineage functions

        private void FillLineage(World.Lineage lineage)
        {

            int fillCreature = -1;
            string fillCreatureData = "";
            int i = 0;
            while (fillCreature == -1 && i < lineage.creatureTypes.Length)
            {
                fillCreature = lineage.creatureTypes[i];
                fillCreatureData = lineage.spawnData[i];
                ++i;
            }
            if (fillCreature != -1)
            {
                for(int j = 0; j < lineage.creatureTypes.Length; ++j)
                {
                    if (lineage.creatureTypes[j] == -1) 
                    {
                        lineage.creatureTypes[j] = fillCreature;
                        lineage.spawnData[j] = fillCreatureData;
                    }
                    else
                    {
                        fillCreature = lineage.creatureTypes[j];
                        fillCreatureData = lineage.spawnData[j];
                    }
                }
            }
        }

        private void RandomizeLineageFirst(World.Lineage lineage)
        {
            int n = lineage.creatureTypes.Length;
            if (lineage.creatureTypes[n - 1] == CreatureTemplate.Type.RedLizard.Index ||
                lineage.creatureTypes[n - 1] == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard.Index ||
                lineage.creatureTypes[n - 1] == CreatureTemplate.Type.RedCentipede.Index)
                n--;
            int indexToCopy = UnityEngine.Random.Range(0, n);

            lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy];
            lineage.spawnData[0] = lineage.spawnData[indexToCopy];
            if(indexToCopy == n - 1 && n > 1)
            {
                if(lineage.creatureTypes[indexToCopy] == CreatureTemplate.Type.RedLizard.Index)
                {
                    if (UnityEngine.Random.value > RedLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard.Index)
                {
                    if (UnityEngine.Random.value > TrainLizardChance || UnityEngine.Random.value > RedLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == CreatureTemplate.Type.RedCentipede.Index)
                {
                    if (UnityEngine.Random.value > RedCentipedeChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
            }
        }

        private void HandleLizardLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            if(IsCreatureInLineage(lineage, CreatureTemplate.Type.Salamander) || IsCreatureInLineage(lineage, DLCSharedEnums.CreatureTemplateType.EelLizard))
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, DLCSharedEnums.CreatureTemplateType.EelLizard, EelLizChance);
                return;
            }

            bool replaceForRyanLiz = hasLizardVariants && IsCreatureInLineage(lineage, CreatureTemplate.Type.CyanLizard);

            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, CreatureTemplate.Type.RedLizard, RedLizardChance, true);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, DLCSharedEnums.CreatureTemplateType.SpitLizard, CaramelLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BlueLizard, CreatureTemplate.Type.CyanLizard, CyanLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.PinkLizard, DLCSharedEnums.CreatureTemplateType.ZoopLizard, StrawberryLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, TrainLizardChance);


            if (ForceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.YellowLizard.Index && ExtraYellows > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.YellowLizard, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (ForceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.Salamander.Index && ExtraSals > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.Salamander, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (ForceFreshSpawns && BalancedSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BlackLizard.Index && ExtraBlacks > 0 && Region == "SH" && UnityEngine.Random.value > .5f)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BlackLizard, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }

            //Watcher

            if (ModManager.Watcher)
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, WatcherEnums.CreatureTemplateType.IndigoLizard, GroundIndigoLizChance);
                ReplaceCreatureInLineage(lineage, DLCSharedEnums.CreatureTemplateType.SpitLizard, WatcherEnums.CreatureTemplateType.IndigoLizard, GroundIndigoLizChance);
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BlackLizard, WatcherEnums.CreatureTemplateType.BasiliskLizard, BlackBasiliskLizChance);
                float blizLizChance = BlizzardLizardChance;
                if (Subregion == "Bitter Aerie" || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                    blizLizChance *= 1.8f;
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, WatcherEnums.CreatureTemplateType.BlizzardLizard, BalancedSpawns? blizLizChance : BlizzardLizardChance, true);
            }

            //Mods
            if (replaceForRyanLiz)
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, new CreatureTemplate.Type("RyanLizard"), 1f);

            if (activeMods.Contains("thefriend"))
            {
                if(ForceFreshSpawns && lineage.creatureTypes[0] > -1 && lineage.creatureTypes[0] < StaticWorld.creatureTemplates.Length && 
                    StaticWorld.creatureTemplates[lineage.creatureTypes[0]].type == StaticWorld.GetCreatureTemplate("MotherLizard").type)
                {
                    World.SimpleSpawner invasionSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den,
                        StaticWorld.GetCreatureTemplate("YoungLizard").type, lineage.spawnData[0], UnityEngine.Random.value > .5f? 3:4);
                    
                    spawners.Add(invasionSpawner);
                }
            }
        }
        
        private void HandleCentipedeLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.SmallCentipede, CreatureTemplate.Type.Centipede, LargeCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede, RedCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, RedCentipedeChance);
        }

        private void HandleLongLegsLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, DaddyLongLegsChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, DLCSharedEnums.CreatureTemplateType.TerrorLongLegs, TerrorLongLegsChance);

            //Inspector invasion
            if (hasAngryInspectors)
            {
                for(int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if(lineage.creatureTypes[i] > -1 && lineage.creatureTypes[i] < StaticWorld.creatureTemplates.Length && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                    {
                        if(UnityEngine.Random.value < ((Region == "UW" && BalancedSpawns)? InspectorChance*2 : InspectorChance))
                        {
                            World.SimpleSpawner inspectorSpawner = new World.SimpleSpawner
                                (lineage.region, spawners.Count, lineage.den, DLCSharedEnums.CreatureTemplateType.Inspector, "{Ignorecycle}", 1);
                            spawners.Add(inspectorSpawner);
                            break;
                        }
                    }
                }
            }
        }

        private bool IsCreatureInLineage(World.Lineage lineage, CreatureTemplate.Type creatureType, bool useAncestor = false)
        {
            if (creatureType == null)
                return false;
            bool foundCreature = false;
            if (!useAncestor)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] == creatureType.Index)
                    {
                        foundCreature = true;
                        break;
                    }
                }
            }
            else if(creatureType.index > -1 && creatureType.index < StaticWorld.creatureTemplates.Length)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] > -1 && lineage.creatureTypes[i] < StaticWorld.creatureTemplates.Length && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type == 
                        StaticWorld.creatureTemplates[creatureType.Index].TopAncestor().type)
                    {
                        foundCreature = true;
                        break;
                    }
                }
            }

            return foundCreature;
        }
    
        private bool ReplaceCreatureInLineage(World.Lineage lineage, CreatureTemplate.Type replacee, CreatureTemplate.Type replacement, float chance, bool useAncestors = false)
        {
            if (replacement is null || replacee is null)
                return false;
            bool replacedCreature = false;

            if (!useAncestors)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] == replacee.Index && UnityEngine.Random.value < chance)
                    {
                        replacedCreature = true;
                        lineage.creatureTypes[i] = replacement.Index;
                    }
                }
            }
            else if(replacee.index > -1 && replacee.index < StaticWorld.creatureTemplates.Length)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] > -1 && lineage.creatureTypes[i] < StaticWorld.creatureTemplates.Length && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type ==
                        StaticWorld.creatureTemplates[replacee.Index].TopAncestor().type && UnityEngine.Random.value < chance)
                    {
                        replacedCreature = true;
                        lineage.creatureTypes[i] = replacement.Index;
                    }
                }
            }
           
            return replacedCreature;
        }

        private bool ReplaceCreatureInLineageIndex(World.Lineage lineage, CreatureTemplate.Type replacement, float chance, int index)
        {
            if (index < 0 || index >= lineage.creatureTypes.Length)
                return false;
            bool replacedCreature = false;
            if (UnityEngine.Random.value < chance)
            {
                replacedCreature = true;
                lineage.creatureTypes[index] = replacement.Index;
            }
            return replacedCreature;
        }

        #endregion

        #region Mod Creature Edits functions

        private void CheckModCreatures(World.SimpleSpawner spawner, List<World.CreatureSpawner> spawners)
        {
            if (spawner is null || spawner.creatureType is null)
                return;
            if(!(modCreatureReplacements is null) && modCreatureReplacements.TryGetValue(spawner.creatureType, out List<ModCreatureReplacement> list))
            {
                ApplyModCreatureEdits(spawner, spawners, list);
            }

            if (!(modCreatureAncestorReplacements is null) && !(StaticWorld.GetCreatureTemplate(spawner.creatureType) is null) &&
                modCreatureAncestorReplacements.TryGetValue(
                StaticWorld.GetCreatureTemplate(spawner.creatureType).TopAncestor().type, out List<ModCreatureReplacement> list2))
            {

                bool isInList = false;
                for(int i = 0; i < list2.Count; ++i)
                {
                    if (list2[i].type == spawner.creatureType)
                    {
                        isInList = true;
                        break;
                    }
                }
                if(!isInList)
                    ApplyModCreatureEdits(spawner, spawners, list2);
            }

            if (!(modCreatureExtras is null) && modCreatureExtras.TryGetValue(spawner.creatureType, out ModCreatureExtras modExtras))
            {
                ApplyModCreatureEdits(spawner, modExtras);
            }
        }

        private void ApplyModCreatureEdits(World.SimpleSpawner spawner, List<World.CreatureSpawner> spawners, List<ModCreatureReplacement> list)
        {
            int i = 0;

            while (i < list.Count)
            {
                if (list[i].type == spawner.creatureType || list[i].type.index == -1)
                {
                    i++;
                    continue;
                }

                float localRepChance = list[i].repChance;

                if ((BalancedSpawns && localRepChance > 0 && localRepChance < 1) || list[i].overrideBalance)
                {
                    if (!(list[i].localMultipliers is null))
                    {
                        localRepChance *= GetLocalMultipliers(list[i].localMultipliers, spawner);
                    }
                    if (!(list[i].localAdditions is null))
                    {
                        localRepChance += (float)GetLocalAdditions(list[i].localAdditions, spawner) / 100;
                    }
                }

                if (list[i].isInvasion || list[i].perDenReplacement)
                    AddInvasionSpawner(spawner, spawners, list[i].type, localRepChance, list[i].perDenReplacement, list[i].isInvasion, (list[i].perDenReplacement && !list[i].isInvasion));
                else
                    ReplaceMultiSpawner(spawner, spawners, list[i].type, localRepChance, true);
                i++;
            }
        }

        private void ApplyModCreatureEdits(World.SimpleSpawner spawner, ModCreatureExtras modExtras)
        {
            int localAmount = modExtras.amount;
            if ((BalancedSpawns && localAmount > 0) || modExtras.overrideBalance)
            {
                if (!(modExtras.localMultipliers is null))
                {
                    localAmount = (int)(localAmount * GetLocalMultipliers(modExtras.localMultipliers, spawner));
                }
                if (!(modExtras.localAdditions is null))
                {
                    localAmount += GetLocalAdditions(modExtras.localAdditions, spawner);
                }
            }
            IncreaseCreatureSpawner(spawner, localAmount, modExtras.divideByTen);
        }

        private void CheckModCreatures(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            for(int k = 0; k < lineage.creatureTypes.Length; ++k)
            {
                if (lineage.creatureTypes[k] < 0 || lineage.creatureTypes[k] >= StaticWorld.creatureTemplates.Length)
                    continue;
                if (!(modCreatureReplacements is null) && modCreatureReplacements.TryGetValue(
                    StaticWorld.creatureTemplates[lineage.creatureTypes[k]].type, out List<ModCreatureReplacement> list))
                {
                    ApplyModCreatureEdits(lineage, k, spawners, list);
                }
                if (lineage.creatureTypes[k] < 0 || lineage.creatureTypes[k] >= StaticWorld.creatureTemplates.Length)
                    continue;
                if (!(modCreatureAncestorReplacements is null) && modCreatureAncestorReplacements.TryGetValue(
                    StaticWorld.creatureTemplates[lineage.creatureTypes[k]].TopAncestor().type, out List<ModCreatureReplacement> list2))
                {
                    ApplyModCreatureEdits(lineage, k, spawners, list2);
                }
            }
            if (lineage.creatureTypes[0] > -1 && lineage.creatureTypes[0] < StaticWorld.creatureTemplates.Length && ForceFreshSpawns && FillLineages)
            {
                if (!(modCreatureReplacements is null) && modCreatureReplacements.TryGetValue(
                    StaticWorld.creatureTemplates[lineage.creatureTypes[0]].type, out List<ModCreatureReplacement> list))
                {
                    int i = 0;
                    while(i < list.Count)
                    {
                        if (list[i].isInvasion)
                        {
                            if(UnityEngine.Random.value < list[i].repChance)
                            {
                                World.SimpleSpawner newSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den,
                                list[i].type, lineage.spawnData[0], 1);
                                spawners.Add(newSpawner);
                            }
                        }
                        i++;
                    }
                }
                if (!(modCreatureExtras is null) && modCreatureExtras.ContainsKey(
                    StaticWorld.creatureTemplates[lineage.creatureTypes[0]].type))
                {
                    modCreatureExtras.TryGetValue(StaticWorld.creatureTemplates[lineage.creatureTypes[0]].type, out ModCreatureExtras value);
                    if(value.amount > 0)
                    {
                        World.SimpleSpawner newSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den,
                        StaticWorld.creatureTemplates[lineage.creatureTypes[0]].type, lineage.spawnData[0], 0);
                        spawners.Add(newSpawner);
                    }
                }
            }
        }

        private void ApplyModCreatureEdits(World.Lineage lineage, int index, List<World.CreatureSpawner> spawners, List<ModCreatureReplacement> list)
        {
            int i = 0;

            while (i < list.Count)
            {
                if (list[i].isInvasion || list[i].type.index == -1)
                {
                    i++;
                    continue;
                }
                float localRepChance = list[i].repChance; 
                if ((BalancedSpawns && localRepChance > 0 && localRepChance < 1) || list[i].overrideBalance)
                {
                    if (!(list[i].localMultipliers is null))
                    {
                        localRepChance *= GetLocalMultipliers(list[i].localMultipliers, lineage, index);
                    }
                    if (!(list[i].localAdditions is null))
                    {
                        localRepChance += (float)GetLocalAdditions(list[i].localAdditions, lineage, index) / 100;
                    }
                }
                ReplaceCreatureInLineageIndex(lineage, list[i].type, localRepChance, index);
                i++;
            }
        }

        private float GetLocalMultipliers(Dictionary<string, float> localMultipliers, World.SimpleSpawner spawner)
        {
            float multiplier;
            float totalMult = 1f;
            bool foundModifier = false;
            if (localMultipliers.TryGetValue(Region, out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (!(Subregion is null) && localMultipliers.TryGetValue(Subregion, out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (localMultipliers.TryGetValue(SlugcatName.ToString(), out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (localMultipliers.TryGetValue(RoomName, out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (localMultipliers.TryGetValue(String.Concat(Region, SlugcatName.ToString()), out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if ((spawner.nightCreature || (!(spawner.spawnDataString is null) && spawner.spawnDataString.Contains("Night"))) &&
                localMultipliers.TryGetValue("Night", out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (!(spawner.spawnDataString is null) && spawner.spawnDataString.Contains("PreCycle") &&
                localMultipliers.TryGetValue("PreCycle", out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (!(spawner.spawnDataString is null) && localMultipliers.TryGetValue(spawner.spawnDataString, out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (localMultipliers.TryGetValue(spawner.creatureType.ToString(), out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            
            
            if (!foundModifier && localMultipliers.TryGetValue("!", out multiplier))
                    totalMult *= multiplier;
            
            return totalMult;
        }
        
        private int GetLocalAdditions(Dictionary<string, int> localAdditions, World.SimpleSpawner spawner)
        {
            int addition;
            int totalAdd = 0;
            bool foundModifier = false;
            if (localAdditions.TryGetValue(Region, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (!(Subregion is null) && localAdditions.TryGetValue(Subregion, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (localAdditions.TryGetValue(SlugcatName.ToString(), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(RoomName, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (localAdditions.TryGetValue(String.Concat(Region, SlugcatName.ToString()), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if ((spawner.nightCreature || (!(spawner.spawnDataString is null) && spawner.spawnDataString.Contains("Night"))) &&
                localAdditions.TryGetValue("Night", out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (!(spawner.spawnDataString is null) && spawner.spawnDataString.Contains("PreCycle") &&
                localAdditions.TryGetValue("PreCycle", out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (!(spawner.spawnDataString is null) && localAdditions.TryGetValue(spawner.spawnDataString, out addition))
            {
                foundModifier = true;
                totalAdd *= addition;
            }
            if (localAdditions.TryGetValue(spawner.creatureType.ToString(), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }

            if (!foundModifier && localAdditions.TryGetValue("!", out addition))
                totalAdd += addition;

            return totalAdd;
        }

        private float GetLocalMultipliers(Dictionary<string, float> localMultipliers, World.Lineage lineage, int index)
        {
            float multiplier;
            float totalMult = 1f;
            bool foundModifier = false;
            if (localMultipliers.TryGetValue(Region, out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (!(Subregion is null) && localMultipliers.TryGetValue(Subregion, out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (localMultipliers.TryGetValue(SlugcatName.ToString(), out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (localMultipliers.TryGetValue(RoomName, out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (localMultipliers.TryGetValue(String.Concat(Region, SlugcatName.ToString()), out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (!(lineage.spawnData[index] is null) && localMultipliers.TryGetValue(lineage.spawnData[index], out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }

            /*
             * No instances of night or precycle lineages in-game.
            if ((lineage.nightCreature || (!(lineage.spawnData[index] is null) && lineage.spawnData[index].Contains("Night"))) &&
                localMultipliers.TryGetValue("Night", out multiplier))
                totalMult *= multiplier;
            if (!(lineage.spawnData[index] is null) && lineage.spawnData[index].Contains("PreCycle") &&
                localMultipliers.TryGetValue("PreCycle", out multiplier))
                totalMult *= multiplier;
            */


            if (lineage.creatureTypes[index] > -1 && lineage.creatureTypes[index] < StaticWorld.creatureTemplates.Length && 
                localMultipliers.TryGetValue(StaticWorld.creatureTemplates[lineage.creatureTypes[index]].type.ToString(), out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }

            if (!foundModifier && localMultipliers.TryGetValue("!", out multiplier))
                totalMult *= multiplier;

            return totalMult;
        }

        private int GetLocalAdditions(Dictionary<string, int> localAdditions, World.Lineage lineage, int index)
        {
            int addition;
            int totalAdd = 0;
            bool foundModifier = false;
            if (localAdditions.TryGetValue(Region, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (!(Subregion is null) && localAdditions.TryGetValue(Subregion, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(SlugcatName.ToString(), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(RoomName, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(String.Concat(Region, SlugcatName.ToString()), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (!(lineage.spawnData[index] is null) && localAdditions.TryGetValue(lineage.spawnData[index], out addition))
            {
                foundModifier = true;
                totalAdd *= addition;
            }

            /*
             * Again, no instances of night or precycle lineages in game.
            if ((lineage.nightCreature || (!(lineage.spawnData[index] is null) && lineage.spawnData[index].Contains("Night"))) &&
                localAdditionsRed.TryGetValue("Night", out addition))
                totalAdd += addition;
            if (!(lineage.spawnData[index] is null) && lineage.spawnData[index].Contains("PreCycle") &&
                localAdditionsRed.TryGetValue("PreCycle", out addition))
                totalAdd += addition;
            */

            if (lineage.creatureTypes[index] > -1 && lineage.creatureTypes[index] < StaticWorld.creatureTemplates.Length &&
                localAdditions.TryGetValue(StaticWorld.creatureTemplates[lineage.creatureTypes[index]].type.ToString(), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }

            if (!foundModifier && localAdditions.TryGetValue("!", out addition))
                totalAdd += addition;

            return totalAdd;
        }

        #endregion

        #region Other functions

        private void LogSpawner(World.CreatureSpawner spawner, int arrayIndex = -1)
        {
            if (spawner is null)
            {
                UnityEngine.Debug.Log("Received null spawner with index: " + arrayIndex);
            }
            else if (spawner is World.SimpleSpawner simpleSpawner)
            {
                UnityEngine.Debug.Log("Simple Spawner data:");
                UnityEngine.Debug.Log("ID: " + simpleSpawner.SpawnerID);
                UnityEngine.Debug.Log("Creature: " + simpleSpawner.creatureType);
                UnityEngine.Debug.Log("Amount: " + simpleSpawner.amount);
                UnityEngine.Debug.Log("Subregion: " + Subregion);
                UnityEngine.Debug.Log("In region index: " + simpleSpawner.inRegionSpawnerIndex);
                if (arrayIndex != -1)
                    UnityEngine.Debug.Log("Spawner array index: " + arrayIndex);
                UnityEngine.Debug.Log("Den: " + simpleSpawner.den.ToString());
                UnityEngine.Debug.Log("Den room: " + simpleSpawner.den.ResolveRoomName());
                UnityEngine.Debug.Log("Spawn data string: " + simpleSpawner.spawnDataString);
                UnityEngine.Debug.Log("Night creature: " + simpleSpawner.nightCreature.ToString());
            }
            else if (spawner is World.Lineage lineage)
            {
                string auxStr;
                UnityEngine.Debug.Log("Lineage data:");// + lineage.ToString());
                UnityEngine.Debug.Log("ID: " + lineage.SpawnerID);
                for (int j = 0; j < lineage.creatureTypes.Length; ++j)
                {
                    if (lineage.creatureTypes[j] > -1)
                        auxStr = StaticWorld.creatureTemplates[lineage.creatureTypes[j]].type.ToString();
                    else auxStr = "Null";
                    UnityEngine.Debug.Log("Creature " + (j + 1) + " : " + lineage.creatureTypes[j] + " (" +
                        auxStr + ")");
                }
                UnityEngine.Debug.Log("Subregion: " + Subregion);
                UnityEngine.Debug.Log("In region index: " + lineage.inRegionSpawnerIndex);
                if (arrayIndex != -1)
                    UnityEngine.Debug.Log("Spawner array index: " + arrayIndex);
                UnityEngine.Debug.Log("Den: " + lineage.den.ToString());
                UnityEngine.Debug.Log("Den room: " + lineage.den.ResolveRoomName());
                UnityEngine.Debug.Log("Spawn data strings: ");
                for (int j = 0; j < lineage.spawnData.Length; ++j)
                {
                    UnityEngine.Debug.Log("Creature " + j + " : " + lineage.spawnData[j]);
                }
            }
            UnityEngine.Debug.Log("\n");

        }

        private WorldCoordinate FindOffScreenDen(WorldLoader loader)
        {
            foreach(World.CreatureSpawner spawner in loader.spawners)
            {
                if(spawner.den.ResolveRoomName().ToLower().Contains("offscreenden"))
                    return spawner.den;
            }

            return new WorldCoordinate(loader.world.firstRoomIndex + loader.roomAdder.Count, -1, -1, 0);
        }

        private void EnsureNormalScavengers(WorldLoader loader)
        {
            int scavs = 0, totalScavs = 0;
            World.SimpleSpawner scavSpawner = null;
            foreach (World.CreatureSpawner spawner in loader.spawners)
            {
                if (spawner is World.SimpleSpawner sspawner && (sspawner.spawnDataString == "" || sspawner.spawnDataString is null) && !sspawner.nightCreature
                    && StaticWorld.GetCreatureTemplate(sspawner.creatureType).TopAncestor().type == CreatureTemplate.Type.Scavenger)
                {
                    if (sspawner.creatureType == CreatureTemplate.Type.Scavenger)
                    {
                        scavs += sspawner.amount;
                        totalScavs += sspawner.amount;
                        scavSpawner = sspawner;
                    }
                    else
                    {
                        totalScavs += sspawner.amount;
                        if (scavSpawner is null)
                            scavSpawner = sspawner;
                    }
                }
            }
            if (scavs < 2 && totalScavs > 0)
            {
                World.SimpleSpawner invasionSpawner = CopySpawner(scavSpawner);
                invasionSpawner.creatureType = CreatureTemplate.Type.Scavenger;
                invasionSpawner.amount = (2 - scavs);
                invasionSpawner.inRegionSpawnerIndex = loader.spawners.Count;
                loader.spawners.Add(invasionSpawner);
            }
        }

        private bool DrainMiteStringValidFormat(string spawnDataString)
        {
            try
            {
                if (spawnDataString.Length < 3)
                    return false;
                string cutstr = spawnDataString.Substring(1, spawnDataString.Length - 2);
                string[] splitstr = cutstr.Split(',');
                if (splitstr.Length == 0)
                    return false;
                foreach (string field in splitstr)
                {
                    string[] splitField = field.Split(':');

                    if (splitField.Length < 2)
                        return false;
                    else
                    {
                        if (splitField[0] != "BodySize" && splitField[0] != "SizeMult" && splitField[0] != "FoodPips")
                            return false;
                        if (!float.TryParse(splitField[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                            return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void SetUpLocalVariables(WorldLoader worldLoader)
        {
            triedEchoLevi = false;
            hasBlackLizards = false;
            hasDeers = false;
            vanillaHorizontalSpawn = null;
            wLoader = worldLoader;
            originalSpawnCount = SpawnerCount;

            if (activeMods.Contains("lb-fgf-m4r-ik.tronsx-region-code") || activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                foreach (World.SimpleSpawner simpleSpawner in worldLoader.spawners.OfType<World.SimpleSpawner>())
                {
                    if (simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard)
                    {
                        hasBlackLizards = true;
                        break;
                    }
                }
            }

            if (ModManager.Watcher)
            {
                //Need to check for deers for skywhale replacements.
                foreach (World.SimpleSpawner simpleSpawner in worldLoader.spawners.OfType<World.SimpleSpawner>())
                {
                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Deer)
                    {
                        hasDeers = true;
                        break;
                    }
                }
                //We find the first instance of a horizontal spawn.
                //Vanilla creatures should come before modded ones in the list.
                for (int i = 0; i < worldLoader.spawners.Count; ++i)
                {
                    if(worldLoader.spawners[i] is World.SimpleSpawner sSpawner)
                    {
                        if(horizontalSpawns.Contains(sSpawner.creatureType))
                        { 
                            vanillaHorizontalSpawn = sSpawner.creatureType;
                            break;
                        }
                    }
                }
            }

        }
        
        private void ForceBlackMoleSalamander(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (hasBlackLizards && self != null && self.lizard != null && self.lizard.Template.type == new CreatureTemplate.Type("MoleSalamander"))
            {
                self.blackSalamander = true;
            }
        }
        #endregion

        #region Game Manager functions
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            triedEchoLevi = false;
            hasBlackLizards = false;
            orig(self, game);
            this.game = game;
            SetStaticOptions();
        }

        void SetStaticOptions()
        {
            HunterLongLegsChance = (float)options.GetConfigValue("HunterLongLegsChance") / 100f;
        }

        void OnDestroy()
        {
            options = null;
        }

        #endregion
    }
}
﻿using BepInEx;
using System.Security.Permissions;
using System.Security;
using System;
using UnityEngine;
using MoreSlugcats;
using System.Collections.Generic;
using System.Security.AccessControl;
using Steamworks;
using System.Runtime.Serialization;
using MonoMod.Cil;
using Mono.Cecil.Cil;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ApexUpYourSpawns
{
    [BepInPlugin("ShinyKelp.ApexUpYourSpawns", "ApexUpYourSpawns", "1.4.2")]

    public class ApexUpYourSpawnsMod : BaseUnityPlugin
    {
        #region Vanilla Creature Variables
        private bool forceFreshSpawns, fillLineages, balancedSpawns;

        private float redLizardChance, trainLizardChance, largeCentipedeChance, redCentipedeChance, spitterSpiderChance, kingVultureChance,
            mirosVultureChance, eliteScavengerChance, brotherLongLegsChance, daddyLongLegsChance, terrorLongLegsChance, cicadaCentiwingChance, cicadaNoodleflyChance,
            fireBugChance, giantJellyfishChance, leechLizardChance, seaLeechAquapedeChance, yeekLizardChance, aquapedeChance, caramelLizChance,
            strawberryLizChance, cyanLizChance, eelLizChance, jungleLeechChance, motherSpiderChance, tubeWormBigSpiderChance,
            stowawayChance, kingScavengerChance, hunterLongLegsChance, jetfishSalamanderChance;

        private int extraGreens, extraPinks, extraBlues, extraWhites, extraBlacks, extraYellows, extraCyans, extraSals, extraCaramels, extraZoops, extraEellizs,
            extraSpiders, extraVultures, extraScavengers, extraSmallCents, extraCentipedes,
            extraCentiwings, extraAquapedes, extraPrecycleSals, extraDropwigs, extraMiros, extraSmallSpiders, extraLeeches, extraKelp, extraLeviathans,
            extraEggbugs, extraCicadas, extraLMice, extraSnails, extraJetfish, extraTubeworms, extraYeeks, extraNightCreatures;
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
        private float inspectorChance, ryanLizardChance;

        #endregion

        #region Mod Setup Functions

        private bool IsInit;

        private bool hasUpdatedRefs;

        private bool logSpawners;

        private bool lastWasError;

        private HashSet<string> bannedRooms;

        private ApexUpYourSpawnsOptions options;

        private RainWorldGame game;

        private static HashSet<string> supportedMods = new HashSet<string>(
                new string[]
                {
                    "lb-fgf-m4r-ik.bl-crit",
                    "ShinyKelp.AngryInspectors",
                    "lb-fgf-m4r-ik.scutigera-creature",
                    "lb-fgf-m4r-ik.red-horror-centi",
                    "lb-fgf-m4r-ik.water-spitter",
                    "moredlls",
                    "niko.explodingdlls",
                    "lb-fgf-m4r-ik.fat-fire-fly-creature",
                    "ShinyKelp.LizardVariants",
                    "sludgeliz",
                    "mymod",
                    "lb-fgf-m4r-ik.swalkins",
                    "lb-fgf-m4r-ik.bouncing-ball-creature",
                    "rainbowlonglegs",
                    "epiclizards",
                    "thefriend",
                    "cherrylizard",
                    "jadeliz",
                    "crazylizard",
                    "Outspector",
                    "theincandescent",
                    "ShinyKelp.ScavengerTweaks",

                    "lurzard.pitchblack",
                    "spearsnail",
                    "Croken.bombardier-vulture",
                    "pkuyo.thevanguard",
                    "lb-fgf-m4r-ik.hvfly-tm",
                    "themast",
                    "drainmites",
                    "myr.moss_fields",
                    "ShinyKelp.FatNoodleFly",
                    "shrimb.scroungers",
                    "lb-fgf-m4r-ik.noodle-eater",
                    "lb-fgf-m4r-ik.cool-thorn-bug",
                    "lb-fgf-m4r-ik.mini-levi",
                    "Croken.Mimicstarfish"

                }
            );
        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;

            options = new ApexUpYourSpawnsOptions(this, Logger);
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

            logSpawners = false;
        }

        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;

                //hooks go here
                On.GameSession.ctor += GameSessionOnctor;
                //On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
                On.WorldLoader.GeneratePopulation += GenerateCustomPopulation;
                On.JellyFish.PlaceInRoom += ReplaceGiantJellyfish;
                On.DangleFruit.PlaceInRoom += ReplaceStowawayBugBlueFruit;
                On.MoreSlugcats.GooieDuck.PlaceInRoom += ReplaceStowawayBugGooieDuck;
                On.AbstractCreature.ctor += ReplaceSlugpupForHLL;
                On.Scavenger.ctor += KingCtor;
                On.Room.ReadyForAI += AddScavKings;
                
                On.ScavengerAI.Update += TrickKingIntoSquad;
                On.ScavengerAI.DecideBehavior += TrickKingIntoSquad1;
                On.ScavengerAbstractAI.AbstractBehavior += ScavKingAbsAICanGoIntoPipes;
                On.StaticWorld.InitStaticWorld += ScavKingCanTravelLikeElites;
                IL.Scavenger.Act += ScavKingActCanGoIntoPipes;
                
                IL.World.SpawnPupNPCs += PreventPupCrash;
                IL.AbstractRoom.RealizeRoom += AbstractRoom_RealizeRoom;

                On.DaddyLongLegs.ctor += GiveHunterDaddyPupColor;
                On.DaddyGraphics.ApplyPalette += GiveHunterDaddyPupPalette;
                On.DaddyGraphics.HunterDummy.ApplyPalette += GiveHunterDaddyDummyPupPalette;
                On.DaddyGraphics.HunterDummy.DrawSprites += GiveHunterDaddyDummyPupSprites;
                On.DaddyGraphics.HunterDummy.Update += GiveHunterDaddyDummyPupSize;
                On.DaddyGraphics.HunterDummy.ctor += GiveHunterDaddyDummyPupTailSize;
                On.DaddyGraphics.DaddyDangleTube.ApplyPalette += GiveHunterDaddyDangleTubePupPallete;
                On.DaddyGraphics.DaddyTubeGraphic.ApplyPalette += GiveHunterDaddyTubePupPallete;
                On.DaddyGraphics.DaddyDeadLeg.ApplyPalette += GiveHunterDaddyDeadLegPupPallete;
                //*/
                ClearDictionaries();
                SetUpModDependencies();
                hasUpdatedRefs = false;

                if(activeMods.Contains("ShinyKelp.ScavengerTweaks"))
                {
                    On.Scavenger.ctor -= KingCtor;
                    On.ScavengerAI.Update -= TrickKingIntoSquad;
                    On.ScavengerAI.DecideBehavior -= TrickKingIntoSquad1;
                    On.ScavengerAbstractAI.AbstractBehavior -= ScavKingAbsAICanGoIntoPipes;
                    On.StaticWorld.InitStaticWorld -= ScavKingCanTravelLikeElites;
                    IL.Scavenger.Act -= ScavKingActCanGoIntoPipes;
                }

                MachineConnector.SetRegisteredOI("ShinyKelp.ApexUpYourSpawns", this.options);
                IsInit = true;
                UnityEngine.Debug.Log("Apex Up Your Spawns setup finished successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        private void SetOptions()
        {
            fillLineages = options.fillLineages.Value;
            forceFreshSpawns = options.forceFreshSpawns.Value;
            balancedSpawns = options.balancedSpawns.Value;

            redLizardChance = (float)options.redLizardChance.Value / 100;
            trainLizardChance = (float)options.trainLizardChance.Value / 100;
            largeCentipedeChance = (float)options.largeCentipedeChance.Value / 100;
            redCentipedeChance = (float)options.redCentipedeChance.Value / 100;
            spitterSpiderChance = (float)options.spitterSpiderChance.Value / 100;
            kingVultureChance = (float)options.kingVultureChance.Value / 100;
            mirosVultureChance = (float)options.mirosVultureChance.Value / 100;
            eliteScavengerChance = (float)options.eliteScavengerChance.Value / 100;
            brotherLongLegsChance = (float)options.brotherLongLegsChance.Value / 100;
            daddyLongLegsChance = (float)options.daddyLongLegsChance.Value / 100;
            terrorLongLegsChance = (float)options.terrorLongLegsChance.Value / 100;
            fireBugChance = (float)options.fireBugChance.Value / 100;
            cicadaCentiwingChance = (float)options.cicadaCentiwingChance.Value / 100;
            leechLizardChance = (float)options.leechLizardChance.Value / 100;
            seaLeechAquapedeChance = (float)options.seaLeechAquapedeChance.Value / 100;
            yeekLizardChance = (float)options.yeekLizardChance.Value / 100;
            aquapedeChance = (float)options.jetfishAquapedeChance.Value / 100;
            giantJellyfishChance = (float)options.giantJellyfishChance.Value / 100;
            caramelLizChance = (float)options.caramelLizChance.Value / 100;
            strawberryLizChance = (float)options.strawberryLizChance.Value / 100;
            cyanLizChance = (float)options.cyanLizChance.Value / 100;
            eelLizChance = (float)options.eelLizChance.Value / 100;
            jungleLeechChance = (float)options.jungleLeechChance.Value / 100;
            motherSpiderChance = (float)options.motherSpiderChance.Value / 100;
            tubeWormBigSpiderChance = (float)options.tubeWormSpiderInv.Value / 100;
            stowawayChance = (float)options.stowawayChance.Value / 100;
            kingScavengerChance = (float)options.kingScavengerChance.Value / 100;
            hunterLongLegsChance = (float)options.hunterLongLegsChance.Value / 100;
            jetfishSalamanderChance = (float)options.jetfishSalamanderChance.Value / 100;
            cicadaNoodleflyChance = (float)options.cicadaNoodleFlyChance.Value / 100;

            //Mod dependant
            if (activeMods.Contains("ShinyKelp.AngryInspectors"))
                inspectorChance = (float)options.inspectorChance.Value / 100;
            if (activeMods.Contains("ShinyKelp.LizardVariants"))
                ryanLizardChance = (float)options.ryanLizardChance.Value / 100;

            extraGreens = options.greenLizExtras.Value;
            extraPinks = options.pinkLizExtras.Value;
            extraBlues = options.blueLizExtras.Value;
            extraWhites = options.whiteLizExtras.Value;
            extraBlacks = options.blackLizExtras.Value;
            extraYellows = options.yellowLizExtras.Value;
            extraSals = options.salExtras.Value;
            extraCyans = options.cyanLizExtras.Value;
            extraCaramels = options.caramelLizExtras.Value;
            extraEellizs = options.eelLizExtras.Value;
            extraZoops = options.zoopLizExtras.Value;
            extraSpiders = options.bigSpiderExtras.Value;
            extraVultures = options.vultureExtras.Value;
            extraScavengers = options.scavengerExtras.Value;
            extraSmallCents = options.smallCentExtras.Value;
            extraEggbugs = options.eggbugExtras.Value;
            extraCicadas = options.cicadaExtras.Value;
            extraSnails = options.snailExtras.Value;
            extraJetfish = options.jetfishExtras.Value;
            extraYeeks = options.yeekExtras.Value;
            extraLMice = options.lmiceExtras.Value;
            extraCentipedes = options.centipedeExtras.Value;
            extraCentiwings = options.centiWingExtras.Value;
            extraAquapedes = options.aquapedeExtras.Value;
            extraPrecycleSals = options.precycleCreatureExtras.Value;
            extraDropwigs = options.dropwigExtras.Value;
            extraMiros = options.mirosExtras.Value;
            extraSmallSpiders = options.spiderExtras.Value;
            extraLeeches = options.leechExtras.Value;
            extraTubeworms = options.tubeWormExtras.Value;
            extraKelp = options.kelpExtras.Value;
            extraLeviathans = options.leviathanExtras.Value;
            extraNightCreatures = options.nightCreatureExtras.Value;
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
        }

        private void SetUpModDependencies()
        {
            hasAngryInspectors = hasLizardVariants = false;
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

            if (activeMods.Contains("lb-fgf-m4r-ik.bl-crit"))
            {
                Dictionary<string, float> localMultipliers = new Dictionary<string, float>();
                localMultipliers.Add("SmallNeedleWorm", .5f);
                localMultipliers.Add("BigNeedleWorm", .5f);
                localMultipliers.Add("Centiwing", .5f);
                localMultipliers.Add("CicadaA", .5f);
                localMultipliers.Add("CicadaB", .5f);
                localMultipliers.Add("AquaCenti", 0f);
                ModCreatureReplacement sporantulaRep = new ModCreatureReplacement(
                    new CreatureTemplate.Type("Sporantula"),
                    options.sporantulaChance,
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
                modCreatureExtras.Add(new CreatureTemplate.Type("Sporantula"), new ModCreatureExtras(
                    options.sporantulaExtras
                    ));
            }
            if (activeMods.Contains("ShinyKelp.AngryInspectors"))
            {
                hasAngryInspectors = true;
            }
            if (activeMods.Contains("lb-fgf-m4r-ik.scutigera-creature"))
            {
                Dictionary<string, float> localMultipliers = new Dictionary<string, float>();
                localMultipliers.Add("GWArtificer", 2f);
                localMultipliers.Add("GWSpear", 2f);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede,
                    new ModCreatureReplacement(
                        new CreatureTemplate.Type("Scutigera"),
                        options.scutigeraChance,
                        localMultipliers
                    )   
                    );
                modCreatureExtras.Add(new CreatureTemplate.Type("Scutigera"), new ModCreatureExtras(
                    options.scutigeraExtras
                    ));

            }
            if (activeMods.Contains("lb-fgf-m4r-ik.red-horror-centi"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.RedCentipede,
                    new ModCreatureReplacement(
                        new CreatureTemplate.Type("RedHorrorCenti"),
                        options.redRedHorrorCentiChance
                ));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centiwing,
                    new ModCreatureReplacement(
                        new CreatureTemplate.Type("RedHorrorCenti"),
                        options.wingRedHorrorCentiChance
                ));
            }
            if (activeMods.Contains("lb-fgf-m4r-ik.water-spitter"))
            {
                ModCreatureReplacement wSpitter = new ModCreatureReplacement(
                    new CreatureTemplate.Type("WaterSpitter"), options.waterSpitterChance);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, wSpitter);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, wSpitter);
                modCreatureExtras.Add(new CreatureTemplate.Type("WaterSpitter"), new ModCreatureExtras(
                    options.waterSpitterExtras, true));
            }
            if (activeMods.Contains("moredlls"))
            {
                Dictionary<string, int> localAdditionsExp = new Dictionary<string, int>();
                localAdditionsExp.Add("GWArtificer", 10);
                localAdditionsExp.Add("GWSpear", 10);
                ModCreatureReplacement expDLL = new ModCreatureReplacement(
                        new CreatureTemplate.Type("ExplosiveDaddyLongLegs"),
                        options.mExplosiveLongLegsChance,
                        null,
                        localAdditionsExp);

                Dictionary<string, int> localAdditionsZap = new Dictionary<string, int>();
                localAdditionsZap.Add("UW", 10);
                ModCreatureReplacement zapDLL = new ModCreatureReplacement(
                        new CreatureTemplate.Type("ZapDaddyLongLegs"),
                        options.mZappyLongLegsChance,
                        null,
                        localAdditionsZap);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, expDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, expDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, zapDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, zapDLL);
            }
            if (activeMods.Contains("niko.explodingdlls"))
            {
                Dictionary<string, int> localAdditionsExp = new Dictionary<string, int>();
                localAdditionsExp.Add("GWArtificer", 15);
                localAdditionsExp.Add("GWSpear", 15);
                localAdditionsExp.Add("GW", 15);
                ModCreatureReplacement expDLL = new ModCreatureReplacement(
                        new CreatureTemplate.Type("ExplosiveDLL"),
                        options.explosionLongLegsChance,
                        null,
                        localAdditionsExp);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, expDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, expDLL);

            }
            if (activeMods.Contains("lb-fgf-m4r-ik.fat-fire-fly-creature"))
            {
                Dictionary<string, float> localMultipliers = new Dictionary<string, float>();
                localMultipliers.Add("GW", 2f);
                localMultipliers.Add("Saint", .5f);
                AddModCreatureToDictionary(modCreatureAncestorReplacements,
                    CreatureTemplate.Type.Vulture,
                    new ModCreatureReplacement(
                        new CreatureTemplate.Type("FatFireFly"),
                        options.fatFireFlyChance,
                        localMultipliers
                        ));
            }
            if (activeMods.Contains("ShinyKelp.LizardVariants"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("RyanLizard"),
                        options.ryanLizardChance
                ));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("YellowLimeLizard"),
                        options.yellowLimeLizardChance
                ));

                ModCreatureReplacement mintLiz = new ModCreatureReplacement(
                        new CreatureTemplate.Type("MintLizard"),
                        options.mintLizardChance
                );

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, mintLiz);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, mintLiz);
                modCreatureExtras.Add(new CreatureTemplate.Type("MintLizard"), new ModCreatureExtras(options.mintLizardExtras, true));

                hasLizardVariants = true;
            }
            if (activeMods.Contains("sludgeliz"))
            {
                Dictionary<string, float> localMultipliers = new Dictionary<string, float>();
                localMultipliers.Add("The Gutter", 5f);
                localMultipliers.Add("SL", .5f);
                Dictionary<string, int> localAdditions = new Dictionary<string, int>();
                localAdditions.Add("The Gutter", 25);

                ModCreatureReplacement sludLiz = new ModCreatureReplacement(new CreatureTemplate.Type("SludgeLizard"),
                    options.sludgeLizardChance, localMultipliers);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, sludLiz);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, sludLiz);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, 
                    new ModCreatureReplacement(
                        new CreatureTemplate.Type("SludgeLizard"),
                        options.snailSludgeLizardChance,
                        false,
                        true,
                        localMultipliers
                    ));
                modCreatureExtras.Add(new CreatureTemplate.Type("SludgeLizard"), new ModCreatureExtras(
                        options.sludgeLizardExtras, 
                        true,
                        null,
                        localAdditions
                    ));

            }
            if (activeMods.Contains("mymod"))
            {
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.LizardTemplate, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Lizor"), options.lizorInvChance, true));

                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.Inspector, new ModCreatureReplacement(
                    new CreatureTemplate.Type("VoltLizard"), options.voltLizardChance, true));


                Dictionary<string, float> localMultipliersMagenta = new Dictionary<string, float>();
                localMultipliersMagenta.Add("Rivulet", 2f);
                Dictionary<string, int> localAdditionsMagenta = new Dictionary<string, int>();
                localAdditionsMagenta.Add("OE", 50);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.PinkLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("MagentaLizard"), options.magentaLizardChance, localMultipliersMagenta, localAdditionsMagenta));

                Dictionary<string, int> localAdditionsTangerine = new Dictionary<string, int>();
                localAdditionsTangerine.Add("UW", 10);
                localAdditionsTangerine.Add("SI", 10);
                localAdditionsTangerine.Add("Spear", 10);
                localAdditionsTangerine.Add("Rivulet", 10);
                localAdditionsTangerine.Add("Saint", 10);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("TangerineLizard"), options.yellowTangerineLizardInvChance, false, true, null, localAdditionsTangerine));

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("TangerineLizard"), options.cyanTangerineLizardInvChance, false, true, null, localAdditionsTangerine));

                Dictionary<string, float> localMultipliersChameleon = new Dictionary<string, float>();
                localMultipliersChameleon.Add("OE", 3f);
                localMultipliersChameleon.Add("Rivulet", 0.7f);

                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, new ModCreatureReplacement(
                   new CreatureTemplate.Type("CameleonLizard"), options.chameleonLizarcChance, localMultipliersChameleon));

                Dictionary<string, float> localMultipliersSkyBlue = new Dictionary<string, float>();
                localMultipliersSkyBlue.Add("HI", 1.5f);
                localMultipliersSkyBlue.Add("Rivulet", 1.5f);

                Dictionary<string, int> localAdditionsSkyBlue = new Dictionary<string, int>();
                localAdditionsSkyBlue.Add("SI", 50);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("SkyBlueLizard"), options.skyBlueLizardChance, localMultipliersSkyBlue, localAdditionsSkyBlue));

                Dictionary<string, float> localMultipliersCherry = new Dictionary<string, float>();
                localMultipliersCherry.Add("CL", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("CherryLizard"), options.cherryLizardChance, localMultipliersCherry));

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.RedLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("RaspberryLizard"), options.redRaspberryLizardChance));

                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("RaspberryLizard"), options.strawberryRaspberryLizardChance));

                modCreatureExtras.Add(new CreatureTemplate.Type("Lizor"), new ModCreatureExtras(
                    options.lizorsExtras));

                modCreatureExtras.Add(new CreatureTemplate.Type("CherryLizard"), new ModCreatureExtras(
                    options.cherryLizExtras));

                modCreatureExtras.Add(new CreatureTemplate.Type("TangerineLizard"), new ModCreatureExtras(
                    options.tangerineLizExtras));
            }
            if (activeMods.Contains("lb-fgf-m4r-ik.swalkins"))
            {
                Dictionary<string, float> localSSMultipliers = new Dictionary<string, float>();
                localSSMultipliers.Add("GW", 2f);
                localSSMultipliers.Add("LM", 2f);
                localSSMultipliers.Add("SU_A30", 2f);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    new CreatureTemplate.Type("SurfaceSwimmer"), options.surfaceSwimmerChance, localSSMultipliers));

                Dictionary<string, int> localSSAdds = new Dictionary<string, int>();
                localSSAdds.Add("DS", 20);
                localSSAdds.Add("SL", 20);
                localSSAdds.Add("LM", 20);

                modCreatureExtras.Add(new CreatureTemplate.Type("SurfaceSwimmer"), new ModCreatureExtras(
                    options.surfaceSwimmerExtras, true, null, localSSAdds));
            }
            if (activeMods.Contains("lb-fgf-m4r-ik.bouncing-ball-creature"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, new ModCreatureReplacement(
                    new CreatureTemplate.Type("BouncingBall"), options.bounceBallChance));
                modCreatureExtras.Add(new CreatureTemplate.Type("BouncingBall"), new ModCreatureExtras(
                    options.bounceBallExtras, true));
            }
            if (activeMods.Contains("rainbowlonglegs"))
            {
                Dictionary<string, float> localMultipliers = new Dictionary<string, float>();
                localMultipliers.Add("UW", 1f);
                localMultipliers.Add("SS", 1f);
                localMultipliers.Add("!", .5f);
                ModCreatureReplacement rainbowLongLegsRep = new ModCreatureReplacement(new CreatureTemplate.Type("RainbowLongLegs"), options.rainbowLongLegsChance, localMultipliers);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, rainbowLongLegsRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, rainbowLongLegsRep);
            }
            if (activeMods.Contains("epiclizards"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Spider, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Brown"), options.brownLizardChance, false, true));

                ModCreatureReplacement rotZardrep = new ModCreatureReplacement(
                    new CreatureTemplate.Type("Rotzard"), options.rotzardChance);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, rotZardrep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, rotZardrep);

                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.LizardTemplate, new ModCreatureReplacement(
                    new CreatureTemplate.Type("FPLizards"), options.universalLizardChance));

                Dictionary<string, float> gildedLizDic = new Dictionary<string, float>();
                gildedLizDic.Add("{AlternateForm}", 0f);
                ModCreatureReplacement gildedLizRep = new ModCreatureReplacement(
                    new CreatureTemplate.Type("GildedLizard"), options.gildedLizardChance, gildedLizDic);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, gildedLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallCentipede, gildedLizRep);

                Dictionary<string, float> scalizardDic = new Dictionary<string, float>();
                scalizardDic.Add("VS", 2f);
                ModCreatureReplacement scalizRep = new ModCreatureReplacement(
                    new CreatureTemplate.Type("Scalizard"), options.scalizardSchance, scalizardDic);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, scalizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallCentipede, scalizRep);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlackLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Nightmare"), options.nightmareLizardChance));

                Dictionary<string, float> turquoiseRepDic = new Dictionary<string, float>();
                turquoiseRepDic.Add("SL", 0f);
                Dictionary<string, float> turquoiseInvDic = new Dictionary<string, float>();
                turquoiseInvDic.Add("SL", 1f);
                turquoiseInvDic.Add("!", 0f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Turqoise"), options.turquoiseLizardChance, false, false, turquoiseRepDic));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Turqoise"), options.turquoiseLizardChance, true, false, turquoiseInvDic));

                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Amoeba"), options.amoebaLizardChance));
            }
            if (activeMods.Contains("thefriend"))
            {
                Dictionary<string, float> motherDic = new Dictionary<string, float>();
                motherDic.Add("OE", .5f);
                ModCreatureReplacement motherRep = new ModCreatureReplacement(new CreatureTemplate.Type("MotherLizard"), options.motherLizardChance, motherDic);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, motherRep);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, motherRep);
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
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BigSpider, new ModCreatureReplacement(
                    new CreatureTemplate.Type("SnowSpider"), options.snowSpiderChance, snowSpiderModifs));

                Dictionary<string, float> lostYoungModif = new Dictionary<string, float>();
                lostYoungModif.Add("PinkLizard", .5f);

                ModCreatureReplacement youngLizRep = new ModCreatureReplacement(new CreatureTemplate.Type("YoungLizard"),
                    options.lostYoungLizardChance, true, true, lostYoungModif);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.PinkLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, youngLizRep);

                modCreatureExtras.Add(new CreatureTemplate.Type("YoungLizard"), new ModCreatureExtras(options.youngLizardExtras));
                modCreatureExtras.Add(new CreatureTemplate.Type("SnowSpider"), new ModCreatureExtras(options.snowSpiderExtras));
            }
            if (activeMods.Contains("cherrylizard"))
            {
                Dictionary<string, float> cherryBD = new Dictionary<string, float>();
                cherryBD.Add("Artificer", 4f);
                cherryBD.Add("Hunter", 4f);
                ModCreatureReplacement modRepInv = new ModCreatureReplacement(new CreatureTemplate.Type("CherrybombLizard"), options.cherryBombLizInvChance, true, false, cherryBD);
                ModCreatureReplacement modRepDen = new ModCreatureReplacement(new CreatureTemplate.Type("CherrybombLizard"), options.cherryBombLizDenChance, false, true, cherryBD);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, modRepInv);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, modRepDen);
            }
            if (activeMods.Contains("jadeliz"))
            {
                Dictionary<string, float> jadeBD = new Dictionary<string, float>();
                jadeBD.Add("Rivulet", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, new ModCreatureReplacement
                    (
                        new CreatureTemplate.Type("JadeLizard"),
                        options.jadeLizInvChance,
                        true,
                        false,
                        jadeBD
                    ));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, new ModCreatureReplacement
                    (
                        new CreatureTemplate.Type("JadeLizard"),
                        options.jadeLizDenChance,
                        false,
                        true,
                        jadeBD
                    ));
                modCreatureExtras.Add(new CreatureTemplate.Type("JadeLizard"), new ModCreatureExtras(
                    options.jadeLizExtras, true));
            }
            if (activeMods.Contains("crazylizard"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement
                (
                    new CreatureTemplate.Type("YellowCrazy"), options.yellowCrazyLizDenChance, false, true
                ));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement
                (
                    new CreatureTemplate.Type("YellowCrazy"), options.yellowCrazyLizInvChance, true, false
                ));
                modCreatureExtras.Add(new CreatureTemplate.Type("YellowCrazy"), new ModCreatureExtras(
                    options.yellowCrazyLizExtras, true));
            }
            if (activeMods.Contains("Outspector"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.Inspector, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Outspector"), options.outspectorChance));
                modCreatureExtras.Add(new CreatureTemplate.Type("Outspector"), new ModCreatureExtras(
                    options.outspectorExtras, true));
            }
            if (activeMods.Contains("theincandescent"))
            {
                //Cyanwing, IcyBlueLizard, FreezerLizard, InfantAquapede
                Dictionary<string, float> icyBlueBD = new Dictionary<string, float>();
                icyBlueBD.Add("Night", 1.25f);
                icyBlueBD.Add("PreCycle", 0.5f);
                icyBlueBD.Add("SUIncandescent", 0.5f);
                icyBlueBD.Add("Bitter Aerie", 2f);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("IcyBlueLizard"), options.icyBlueYellowChance, icyBlueBD)
                    );

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("IcyBlueLizard"), options.icyBlueFreezerInvChance, true, true, icyBlueBD)
                    );

                Dictionary<string, float> icyBlueBD2 = new Dictionary<string, float>();
                icyBlueBD2.Add("Night", 1.25f);
                icyBlueBD2.Add("PreCycle", 0.5f);
                icyBlueBD2.Add("SUIncandescent", 0.5f);
                icyBlueBD2.Add("Saint", 0f);
                icyBlueBD2.Add("Bitter Aerie", 2f);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("IcyBlueLizard"), options.icyBlueBlueChance, true, true, icyBlueBD2)
                    );
                Dictionary<string, float> icyBlueBD3 = new Dictionary<string, float>();
                icyBlueBD3.Add("Saint", 2f);
                icyBlueBD.Add("!", 0f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("IcyBlueLizard"), options.icyBlueBlueChance, false, true, icyBlueBD3)
                    );

                modCreatureExtras.Add(new CreatureTemplate.Type("IcyBlueLizard"), new ModCreatureExtras(
                    options.icyBlueLizExtras, true));


                Dictionary<string, float> freezerBD = new Dictionary<string, float>();
                freezerBD.Add("Night", 1.5f);
                freezerBD.Add("PreCycle", 0f);
                freezerBD.Add("SUIncandescent", 0.5f);
                freezerBD.Add("Bitter Aerie", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("FreezerLizard"), options.freezerLizChance, false, true, freezerBD)
                    );
                AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("IcyBlueLizard"), new ModCreatureReplacement(
                        new CreatureTemplate.Type("FreezerLizard"), options.freezerLizChance, false, true, freezerBD)
                    );

                Dictionary<string, float> cyanwingBD = new Dictionary<string, float>();
                cyanwingBD.Add("Night", 1.25f);
                cyanwingBD.Add("PreCycle", 0.5f);
                cyanwingBD.Add("SISaint", 0.5f);
                cyanwingBD.Add("SIIncandescent", 0.5f);
                ModCreatureReplacement cyanwingRep = new ModCreatureReplacement(new CreatureTemplate.Type("Cyanwing"), options.cyanwingChance, false, true, cyanwingBD);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, cyanwingRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallCentipede, cyanwingRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centiwing, new ModCreatureReplacement(
                        new CreatureTemplate.Type("Cyanwing"), options.wingCyanwingChance, false, true, cyanwingBD
                    ));

                Dictionary<string, int> babyAquapedeBD = new Dictionary<string, int>();
                babyAquapedeBD.Add("Sump Tunnel", 45);

                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, new ModCreatureReplacement(
                        new CreatureTemplate.Type("InfantAquapede"), options.babyAquapedeInvChance, true)
                    );
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.JetFish, new ModCreatureReplacement(
                        new CreatureTemplate.Type("InfantAquapede"), options.jetfishBabyAquapedeChance, true, false, null, babyAquapedeBD)
                    );
                modCreatureExtras.Add(new CreatureTemplate.Type("InfantAquapede"), new ModCreatureExtras(options.aquapedeExtras, true));
                //Chillipede
                ModCreatureReplacement chillRep = new ModCreatureReplacement(new CreatureTemplate.Type("Chillipede"), options.chillipedeChance);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, chillRep);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, chillRep);
                if(activeMods.Contains("ShinyKelp.LizardVariants"))
                    AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("MintLizard"), chillRep);

                Dictionary<string, float> subChillipedes = new Dictionary<string, float>();
                subChillipedes.Add("SB", 1f);
                subChillipedes.Add("!", 0f);
                ModCreatureReplacement subChillRep = new ModCreatureReplacement(new CreatureTemplate.Type("Chillipede"), options.chillipedeChance, subChillipedes, null, true);
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
                    ModCreatureReplacement kelpChillRep = new ModCreatureReplacement(new CreatureTemplate.Type("Chillipede"), options.chillipedeChance, true);
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
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, new ModCreatureReplacement(
                    new CreatureTemplate.Type("NightTerror"), options.nightTerrorChance, false, false, null, nightTerrorDict));

                //Night terror replace red cent in shaded/night
                Dictionary<string, int> redCentRepDict = new Dictionary<string, int>();
                redCentRepDict.Add("SH", 30);
                redCentRepDict.Add("Night", 20);
                redCentRepDict.Add("!", -10);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.RedCentipede, new ModCreatureReplacement(
                        new CreatureTemplate.Type("NightTerror"), options.nightTerrorChance, false, false, null, redCentRepDict
                    ));

                //Little longlegs

                Dictionary<string, float> lllDict = new Dictionary<string, float>();
                lllDict.Add("VS", 2f);
                lllDict.Add("SL", 2f);
                lllDict.Add("LM", 2f);
                lllDict.Add("DS", 2f);
                lllDict.Add("OA", 2f);
                lllDict.Add("The Gutter", 2f);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, new ModCreatureReplacement(
                    new CreatureTemplate.Type("LMiniLongLegs"), options.brotherLittleLongLegChance, true, false, lllDict));

                ModCreatureReplacement lllRep = new ModCreatureReplacement(
                    new CreatureTemplate.Type("LMiniLongLegs"), options.critterLittleLongLegsChance, true, false, lllDict);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, lllRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.LanternMouse, lllRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TubeWorm, lllRep);

                //Extras
                modCreatureExtras.Add(new CreatureTemplate.Type("LMiniLongLegs"), new ModCreatureExtras(
                    options.littleLongLegsExtras, true));

            }
            if (activeMods.Contains("spearsnail"))
            {
                Dictionary<string, float> spearSnailDict = new Dictionary<string, float>();
                spearSnailDict.Add("GW", 1.5f);
                spearSnailDict.Add("PreCycle", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, new ModCreatureReplacement(
                    new CreatureTemplate.Type("SpearSnail"), options.spearSnailChance, spearSnailDict));

            }
            if (activeMods.Contains("lb-fgf-m4r-ik.hvfly-tm") || activeMods.Contains("themast"))
            {
                ModCreatureReplacement hoverflyRep = new ModCreatureReplacement(new CreatureTemplate.Type("Hoverfly"), options.critterHoverflyChance);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallNeedleWorm, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.LanternMouse, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TubeWorm, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.Yeek, hoverflyRep);
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
                    new CreatureTemplate.Type("BombardierVulture"), options.bombVultureChance, bombDict));
            }
            if (activeMods.Contains("drainmites"))
            {
                Dictionary<string, float> drainmiteDict = new Dictionary<string, float>();
                drainmiteDict.Add("!", 100f);
                drainmiteDict.Add("PreCycle", 0f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Scavenger, new ModCreatureReplacement(
                    new CreatureTemplate.Type("DrainMite"), options.drainMiteChance, true, true, drainmiteDict, null, true));
                Dictionary<string, int> drainmiteAmountDict = new Dictionary<string, int>();
                drainmiteAmountDict.Add("SizeMult", 0);
                drainmiteAmountDict.Add("PreCycle", 0);
                drainmiteAmountDict.Add("!", 10);
                Dictionary<string, float> drainmiteAmountDict2 = new Dictionary<string, float>();
                drainmiteAmountDict2.Add("SI", 0.3f);
                drainmiteAmountDict2.Add("SU", 0.3f);
                drainmiteAmountDict2.Add("SC", 0.3f);
                drainmiteAmountDict2.Add("LC", 0.3f);
                drainmiteAmountDict2.Add("PreCycle", 0f);
                drainmiteAmountDict2.Add("SizeMult", 0f);

                modCreatureExtras.Add(new CreatureTemplate.Type("DrainMite"), new ModCreatureExtras(options.drainMiteExtras, false, drainmiteAmountDict2, drainmiteAmountDict));
            }
            if (activeMods.Contains("myr.moss_fields") || activeMods.Contains("ShinyKelp.FatNoodleFly"))
            {
                Dictionary<string, float> fatFlyDict = new Dictionary<string, float>();
                fatFlyDict.Add("SI", 1.5f);
                Dictionary<string, int> fatFlyDict2 = new Dictionary<string, int>();
                fatFlyDict2.Add("SI", 10);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.BigNeedleWorm, new ModCreatureReplacement(
                    new CreatureTemplate.Type("SnootShootNoot"), options.fatNootChance, fatFlyDict, fatFlyDict2));
            }
            if (activeMods.Contains("pkuyo.thevanguard"))
            {
                Dictionary<string, float> toxicDict = new Dictionary<string, float>();
                toxicDict.Add("GW", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SpitterSpider, new ModCreatureReplacement(
                    new CreatureTemplate.Type("ToxicSpider"), options.toxicSpiderChance));
            }
            if (activeMods.Contains("shrimb.scroungers"))
            {
                //Mimicstar, ThornBug, MiniLeviathan, NoodleEater, Scrounger, Chillipede,
                Dictionary<string, float> scroungerDict = new Dictionary<string, float>();
                scroungerDict.Add("Artificer", 0.2f);
                scroungerDict.Add("Spear", 0.35f);
                scroungerDict.Add("Rivulet", 1.5f);
                scroungerDict.Add("Saint", 2f);
                scroungerDict.Add("UW", 2f);
                scroungerDict.Add("LC", 2f);
                scroungerDict.Add("OE", 1.5f);
                scroungerDict.Add("SI", 1.5f);
                scroungerDict.Add("SB", 0.5f);
                scroungerDict.Add("GW", 0.5f);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Scavenger, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Scrounger"), options.scroungerChance, scroungerDict));
                modCreatureExtras.Add(new CreatureTemplate.Type("Scrounger"), new ModCreatureExtras(
                    options.scroungerExtras, false));
            }
            if (activeMods.Contains("Croken.Mimicstarfish"))
            {
                Dictionary<string, int> starfishDict = new Dictionary<string, int>();
                starfishDict.Add("MS", 20);
                starfishDict.Add("SL", 20);
                starfishDict.Add("LM", 20);
                starfishDict.Add("DS", 5);
                starfishDict.Add("OA", 5);
                starfishDict.Add("Sump Tunnel", 30);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Mimicstar"), options.bllMimicstarfishChance, null, starfishDict));
                
                Dictionary<string, int> starfishDict2 = new Dictionary<string, int>();
                starfishDict.Add("JetFish", 3);
                starfishDict.Add("AquaCenti", 4);
                ModCreatureReplacement critterMimicRep = new ModCreatureReplacement(new CreatureTemplate.Type("Mimicstar"),
                    options.critterMimicstarfishChance, false, true, null, starfishDict2);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.JetFish, critterMimicRep);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Snail, critterMimicRep);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Leech, critterMimicRep);
                AddModCreatureToDictionary(modCreatureReplacements, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, critterMimicRep);

            }
            if (activeMods.Contains("lb-fgf-m4r-ik.noodle-eater"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallNeedleWorm, new ModCreatureReplacement(
                    new CreatureTemplate.Type("NoodleEater"), options.noodleEaterChance, true, true));
                modCreatureExtras.Add(new CreatureTemplate.Type("NoodleEater"), new ModCreatureExtras(options.noodleEaterExtras));
            }
            if (activeMods.Contains("lb-fgf-m4r-ik.cool-thorn-bug"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    new CreatureTemplate.Type("ThornBug"), options.thornbugChance, true));
                modCreatureExtras.Add(new CreatureTemplate.Type("ThornBug"), new ModCreatureExtras(options.thornbugExtras));
            }
            if(activeMods.Contains("lb-fgf-m4r-ik.mini-levi"))
            {
                Dictionary<string, float> miniLeviDict = new Dictionary<string, float>();
                miniLeviDict.Add("SB", 1.5f);
                miniLeviDict.Add("MS", 1.5f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BigEel, new ModCreatureReplacement(
                    new CreatureTemplate.Type("MiniLeviathan"), options.miniLeviathanChance, true, false, miniLeviDict));
                modCreatureExtras.Add(new CreatureTemplate.Type("MiniLeviathan"), new ModCreatureExtras(options.miniLeviathanExtras, false));
            }

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

        #region HunterLongLegs functions

        private void AbstractRoom_RealizeRoom(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(2),
                x => x.MatchLdfld<AbstractCreature>("state"),
                x => x.MatchIsinst<PlayerNPCState>(),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<PlayerState>("foodInStomach")
                );
            c.RemoveRange(5);
            c.Emit(OpCodes.Ldloc, 2);
            c.EmitDelegate<Action<AbstractCreature>>((abstractCreature) =>
            {
                if (!(abstractCreature.state is null) && abstractCreature.state is PlayerNPCState npcState)
                {
                    npcState.foodInStomach = 1;
                }
            });
        }
        
        private void PreventPupCrash(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(13),
                x => x.MatchLdfld<AbstractCreature>("state"),
                x => x.MatchIsinst<PlayerNPCState>(),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<PlayerState>("foodInStomach")
                );

            c.Index++;
            c.RemoveRange(4);
            
            c.EmitDelegate<Action<AbstractCreature>>((abstractCreature) => 
            {
                if (!(abstractCreature.state is null) && abstractCreature.state is PlayerNPCState npcState)
                {
                    npcState.foodInStomach = 1;
                }
            });//*/
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
            
        }

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

        #region ScavKing Behaviour

        private void ScavKingActCanGoIntoPipes(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Creature>("shortcutDelay"),
                x => x.MatchLdcI4(1));
            c.Index += 6;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
        }

        private void ScavKingCanTravelLikeElites(On.StaticWorld.orig_InitStaticWorld orig)
        {
            orig();
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).doesNotUseDens = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).forbidStandardShortcutEntry = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).usesCreatureHoles = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).usesRegionTransportation = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).roamBetweenRoomsChance = -1f;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).usesNPCTransportation =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).usesNPCTransportation;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).shortcutAversion =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).shortcutAversion;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).mappedNodeTypes =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).mappedNodeTypes;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).pathingPreferencesConnections =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).pathingPreferencesConnections;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).NPCTravelAversion =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).NPCTravelAversion;

        }

        private void ScavKingAbsAICanGoIntoPipes(On.ScavengerAbstractAI.orig_AbstractBehavior orig, ScavengerAbstractAI self, int time)
        {
            if(self is null || self.parent is null || self.parent.creatureTemplate is null)
            {
                orig(self, time);
                return;
            }

            if (self.parent.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing && 
                !(self.parent.realizedCreature is null) && !(self.parent.realizedCreature.room is null) &&
                self.parent.realizedCreature.room.abstractRoom.name != "LC_FINAL")
            {
                self.parent.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite;
                orig(self, time);
                self.parent.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing;
            }
            else
                orig(self, time);
        }

        private void TrickKingIntoSquad1(On.ScavengerAI.orig_DecideBehavior orig, ScavengerAI self)
        {
            if (self.scavenger.King && !(self.scavenger.room is null) && self.scavenger.room.abstractRoom.name != "LC_FINAL")
            {
                try
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite;
                    orig(self);
                }
                finally
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing;
                }
            }
            else
                orig(self);
        }

        private void TrickKingIntoSquad(On.ScavengerAI.orig_Update orig, ScavengerAI self)
        {
            if (self.scavenger.King && !(self.scavenger.room is null) && self.scavenger.room.abstractRoom.name != "LC_FINAL")
            {
                try
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite;
                    orig(self);
                }
                finally
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing;
                }

            }
            else
                orig(self);
        }

        #endregion
        
        #region Non-spawner creature replacements

        private void ReplaceGiantJellyfish(On.JellyFish.orig_PlaceInRoom orig, JellyFish self, Room room)
        {
            if (!game.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < giantJellyfishChance)
            {
                AbstractCreature myBigJelly = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.BigJelly), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), game.GetNewID());
                BigJellyFish myJelly = new BigJellyFish(myBigJelly, game.world);
                myJelly.PlaceInRoom(room);
            }
            else
                orig(self, room);
        }

        private void ReplaceStowawayBugBlueFruit(On.DangleFruit.orig_PlaceInRoom orig, DangleFruit self, Room room)
        {
            if(!game.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < stowawayChance)
            {
                self.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(self, room, self.firstChunk.pos);

                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.StowawayBug), 
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
            if (game.IsStorySession && !room.abstractRoom.shelter && UnityEngine.Random.value < (balancedSpawns? stowawayChance * 2 : stowawayChance))
            {
                DangleFruit fruit = new DangleFruit(self.abstractPhysicalObject);
                fruit.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(fruit, room, fruit.firstChunk.pos);
                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.StowawayBug), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), game.GetNewID());
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
            if (kingScavengerChance > 0 && self.abstractRoom.name != "LC_FINAL")
            {
                List<AbstractCreature> elitesList = new List<AbstractCreature>();
                List<AbstractCreature> removedElitesList = new List<AbstractCreature>();
                if (self.game != null)
                {
                    foreach (AbstractCreature abstractCreature in self.abstractRoom.creatures)
                    {
                        if (abstractCreature.realizedCreature == null && abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite)
                            elitesList.Add(abstractCreature);

                    }
                }

                float localChance = kingScavengerChance;
                if (self.abstractRoom.scavengerOutpost)
                    localChance *= 4f;
                else if (self.abstractRoom.scavengerTrader)
                    localChance *= 0.5f;

                foreach (AbstractCreature eliteAbstract in elitesList)
                {
                    float value = UnityEngine.Random.value;
                    if (value < (options.balancedSpawns.Value? localChance : kingScavengerChance))
                    {
                        AbstractCreature kingAbstract = new AbstractCreature(eliteAbstract.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing),
                            null, eliteAbstract.pos, eliteAbstract.ID);
                        self.abstractRoom.creatures.Remove(eliteAbstract);
                        self.abstractRoom.creatures.Add(kingAbstract);
                        removedElitesList.Add(eliteAbstract);
                        if (self.abstractRoom.scavengerOutpost)
                            localChance = (kingScavengerChance - 0.04f);
                    }
                }
                orig(self);
                foreach (AbstractCreature removedElite in removedElitesList)
                    self.abstractRoom.creatures.Add(removedElite);
            }
            else
                orig(self);
        }
        private void KingCtor(On.Scavenger.orig_ctor orig, Scavenger self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if (self.King && abstractCreature.Room.name != "LC_FINAL")
            {
                self.kingWaiting = false;
                UnityEngine.Random.InitState(self.abstractCreature.ID.RandomSeed);
                self.armorPieces = UnityEngine.Random.Range(1, 4);
            }
        }

        private void ReplaceSlugpupForHLL(On.AbstractCreature.orig_ctor orig, AbstractCreature self, World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
        {
            if (!(world is null) && !(world.game is null) && !world.game.IsArenaSession && creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
            {
                AbstractRoom aRoom = world.abstractRooms[pos.room - world.firstRoomIndex];
                bool hasPlayer = false;
                foreach(AbstractCreature creature in aRoom.creatures)
                {
                    if (creature.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
                        hasPlayer = true;
                }
                if (!hasPlayer && UnityEngine.Random.value < hunterLongLegsChance)
                {
                    creatureTemplate = StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy);
                }
            }
            orig(self, world, creatureTemplate, realizedCreature, pos, ID);
        }

        #endregion

        #region Helper Spawner Variables

        private int spawnerIndex, originalSpawnCount; 
        private WorldLoader wLoader;
        private int firstRoomIndex
        {
            get => wLoader.world.firstRoomIndex;
        }
        private int numberOfRooms
        {
            get => wLoader.world.NumberOfRooms;
        }
        private string region 
        {
            get => wLoader.worldName;
        }
        private string subregion
        {
            get => (wLoader.spawners[spawnerIndex].den.room < firstRoomIndex ||
                    wLoader.spawners[spawnerIndex].den.room >= firstRoomIndex + numberOfRooms)? "" : 
                wLoader.abstractRooms[wLoader.spawners[spawnerIndex].den.room - firstRoomIndex].subregionName;
        }
        private string roomName
        {
            get => wLoader.spawners[spawnerIndex].den.ResolveRoomName();
        }
        private int spawnerCount
        {
            get => wLoader.spawners.Count;
        }
        private SlugcatStats.Name slugcatName
        {
            get => (!game.IsStorySession) ? null :
                game.GetStorySession.saveState.saveStateNumber;
        }

        #endregion

        #region World Generation Functions
        private void GenerateCustomPopulation(On.WorldLoader.orig_GeneratePopulation orig, WorldLoader worldLoader, bool fresh)
        {
            if (forceFreshSpawns && !fresh)
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

            try
            {
                if (fresh)
                {
                    if (!hasUpdatedRefs)
                    {
                        //UnityEngine.Debug.Log("\nUpdating modded creature references...");
                        RefreshModCreatures();
                        //UnityEngine.Debug.Log("Modded creature references updated.\n");
                    }

                    wLoader = worldLoader;
                    string storySession =  (slugcatName is null) ? "null" : slugcatName.ToString();
                    UnityEngine.Debug.Log("Starting spawn modifications for region: " + region + " , character: " + 
                        storySession);
                    UnityEngine.Debug.Log("ORIGINAL SPAWN COUNT: " + spawnerCount);
                    originalSpawnCount = spawnerCount;
                    UnityEngine.Random.InitState(Mathf.RoundToInt(Time.time * 10f));
                    HandleAllSpawners(worldLoader.spawners);

                    EnsureNormalScavengers(worldLoader);

                    UnityEngine.Debug.Log("\nFinished setting up spawns.");
                    UnityEngine.Debug.Log("FINAL SPAWN COUNT: " + spawnerCount + "\n");
                    wLoader = null;
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }

            orig(worldLoader, fresh);

        }
        
        //Region must have at least 2 normal scavengers; or scavengers won't show up at all in tolls.
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

        private void RefreshModCreatures()
        {
            if(!(modCreatureReplacements is null))
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
                                UnityEngine.Debug.Log("Updated Mod Replacement creature " + modRep.type + " to index " + modRep.type.index);
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
            int originalSpawnerCount = spawners.Count;
            for (int i = 0; i < spawners.Count; i++)
            {
                spawnerIndex = i;
                if (spawners[i].den.room < firstRoomIndex ||
                    spawners[i].den.room >= firstRoomIndex + numberOfRooms)
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
                    if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle"))
                        HandlePrecycleSpawns(simpleSpawner, spawners);

                    if ((simpleSpawner.nightCreature || (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("Night"))) 
                        && (simpleSpawner.spawnDataString is null || !simpleSpawner.spawnDataString.Contains("Ignorecycle")))
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraNightCreatures, true);
                        if (hasAngryInspectors) 
                        {
                            if((region == "LC" && roomName != "LCOffScreenDen") || 
                                (region == "UW" && roomName != "UWOffScreenDen"))
                            {
                                bool addedSpawner =
                                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns? inspectorChance / 2 : inspectorChance, true);
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
                        if (hasAngryInspectors && subregion == "Memory Crypts"
                            && simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns? inspectorChance * 4 : inspectorChance);
                        HandleCentipedeSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture || simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture
                        || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture)
                    {
                        HandleVultureSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    {
                        if(region == "GW")
                            IncreaseCreatureSpawner(simpleSpawner, balancedSpawns? extraSpiders*2 : extraSpiders, true);
                        else
                        {
                            IncreaseCreatureSpawner(simpleSpawner, (region == "SB" && balancedSpawns)? extraSpiders-10 : extraSpiders, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                        }
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.SpitterSpider)
                    {
                        if (region == "GW" && balancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, extraSpiders, true);
                        if (region == "UW" || region == "CL" || region == "GW")
                            HandleLongLegsSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Spider)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraSmallSpiders);
                        AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.MotherSpider, motherSpiderChance, true, true);
                        goto ModCreaturesSpawner;
                    }
                    if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MotherSpider)
                    {
                        if (balancedSpawns && subregion == "The Gutter")
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
                        IncreaseCreatureSpawner(simpleSpawner, extraDropwigs, true);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigEel)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraLeviathans, true);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.MirosBird)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraMiros);
                        if (region == "SB" && extraMiros > 0 && balancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 2);
                        if (region == "LC" && extraMiros > 0 && balancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 4);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Scavenger)
                    {
                        int localExtraScavs = extraScavengers;
                        if (balancedSpawns)
                        {
                            if (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer || region == "SB")
                                localExtraScavs /= 2;
                            else if (region == "LC")
                                localExtraScavs = (int)(localExtraScavs * 1.5f);
                        }
                        IncreaseCreatureSpawner(simpleSpawner, localExtraScavs, false);
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite, eliteScavengerChance);
                        goto ModCreaturesSpawner;
                    }
                    
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                    {
                        HandleJetfishSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.TentaclePlant)
                    {
                        if (hasAngryInspectors && region == "SB" && roomName == "SB_G03")
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns ? inspectorChance * 4 : inspectorChance, true);
                        IncreaseCreatureSpawner(simpleSpawner, extraKelp, true);
                        goto ModCreaturesSpawner;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Leech || simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech 
                        || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.JungleLeech)
                    {
                        HandleLeechSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.EggBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraEggbugs, true);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.FireBug, fireBugChance);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.TubeWorm)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraTubeworms);
                        AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigSpider, tubeWormBigSpiderChance);
                        if (hasAngryInspectors && region == "LC" && roomName == "LC_station01")
                        {
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns ? inspectorChance * 4 : inspectorChance, true);
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
                        IncreaseCreatureSpawner(simpleSpawner, (region == "DS" && balancedSpawns) ? extraSnails-10 : extraSnails, true);
                        HandleLongLegsSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.LanternMouse)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (region == "SH" && balancedSpawns) ? extraLMice - 10 : extraLMice, true);
                        HandleLongLegsSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.BigNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= originalSpawnerCount)
                            AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SmallNeedleWorm, 1f);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= originalSpawnerCount && simpleSpawner.amount < 2)
                            IncreaseCreatureSpawner(simpleSpawner, 15, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.Yeek)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (region == "OE" && balancedSpawns) ? extraYeeks - 10 : extraYeeks, true);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, UnityEngine.Random.value < (region == "OE" ? .8f : .5f)? 
                            MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard : MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, yeekLizardChance);
                        if (replacedFull)
                            HandleLizardSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.Inspector)
                    {
                        simpleSpawner.spawnDataString = "{Ignorecycle}";
                        goto ModCreaturesSpawner;
                    }


                ModCreaturesSpawner:
                    CheckModCreatures(simpleSpawner, spawners);
                    AfterModAdjustments(simpleSpawner, spawners);

                }
                else if(spawners[i] is World.Lineage lineage)
                {

                    if(fillLineages)
                        FillLineage(lineage);
                    if (forceFreshSpawns && fillLineages)
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
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                        if(forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BigSpider.Index)
                        {
                            World.SimpleSpawner asimpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BigSpider, lineage.spawnData[0], 1);
                            spawners.Add(asimpleSpawner);
                        }
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.JetFish))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, aquapedeChance);
                        if(region == "SL")
                        {
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, CreatureTemplate.Type.BrotherLongLegs, aquapedeChance);
                            HandleLongLegsLineage(lineage, spawners);
                        }
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.EggBug))
                    {
                        if (balancedSpawns)
                        {
                            lineage.creatureTypes[0] = CreatureTemplate.Type.EggBug.Index;
                            for (int j = 1; j < lineage.creatureTypes.Length; ++j)
                            {
                                lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.FireBug.Index;
                            }
                        }
                        else
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.EggBug, MoreSlugcatsEnums.CreatureTemplateType.FireBug, fireBugChance);
                        goto ModCreaturesLineage;
                    }

                ModCreaturesLineage:
                    CheckModCreatures(lineage, spawners);
                }
            }
        }

        private void AfterModAdjustments(World.SimpleSpawner spawner, List<World.CreatureSpawner> spawners)
        {
            if (activeMods.Contains("Outspector") && spawner.creatureType == new CreatureTemplate.Type("Outspector"))
            {
                spawner.spawnDataString = "{IgnoreCycle}";
                if (hasAngryInspectors)
                    AddInvasionSpawner(spawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, options.inspectorOutspectorInvChance.Value / 100f);
                ReplaceMultiSpawner(spawner, spawners, new CreatureTemplate.Type("OutspectorB"), 0.3f);
            }

            if (activeMods.Contains("lb-fgf-m4r-ik.mini-levi") && spawner.creatureType == new CreatureTemplate.Type("MiniLeviathan"))
            {
                if (region == "SL")
                    spawner.spawnDataString = "{AlternateForm}";
            }

            if (activeMods.Contains("lb-fgf-m4r-ik.cool-thorn-bug") && spawner.creatureType == new CreatureTemplate.Type("ThornBug"))
            {
                if (region == "UW" || region == "CC" || region == "SH")
                    spawner.spawnDataString = "{AlternateForm}";
            }

            if(activeMods.Contains("drainmites") && spawner.creatureType == new CreatureTemplate.Type("DrainMite"))
            {
                if (region == "CC" || region == "SI" || region == "LF")
                {
                    if(spawner.spawnDataString is null || spawner.spawnDataString == "")
                    {
                        spawner.spawnDataString = "SizeMult";
                        spawner.amount /= 4;
                        spawner.amount++;
                        World.SimpleSpawner spawner2 = CopySpawner(spawner);
                        World.SimpleSpawner spawner3 = CopySpawner(spawner);
                        World.SimpleSpawner spawner4 = CopySpawner(spawner);
                        float sizeMult = UnityEngine.Random.Range(0.5f, 1.0f);
                        spawner.spawnDataString = "{SizeMult:" + sizeMult + "}";
                        spawner2.inRegionSpawnerIndex += 1;
                        spawner3.inRegionSpawnerIndex += 2;
                        spawner4.inRegionSpawnerIndex += 3;
                        spawners.Add(spawner2);
                        spawners.Add(spawner3);
                        spawners.Add(spawner4);
                    }
                    else if(spawner.spawnDataString == "SizeMult")
                    {
                        float sizeMult = UnityEngine.Random.Range(0.5f, 1.0f);
                        spawner.spawnDataString = "{SizeMult:" + sizeMult + "}";
                    }
                }
                else if (region == "SB" || region == "VS")
                {
                    if (spawner.spawnDataString is null || spawner.spawnDataString == "")
                    {
                        spawner.spawnDataString = "SizeMult";
                        spawner.amount /= 4;
                        spawner.amount++;
                        World.SimpleSpawner spawner2 = CopySpawner(spawner);
                        World.SimpleSpawner spawner3 = CopySpawner(spawner);
                        World.SimpleSpawner spawner4 = CopySpawner(spawner);
                        float sizeMult = UnityEngine.Random.Range(1.0f, 1.5f);
                        spawner.spawnDataString = "{SizeMult:" + sizeMult + "}";
                        spawner2.inRegionSpawnerIndex += 1;
                        spawner3.inRegionSpawnerIndex += 2;
                        spawner4.inRegionSpawnerIndex += 3;
                        spawners.Add(spawner2);
                        spawners.Add(spawner3);
                        spawners.Add(spawner4);
                    }
                    else if (spawner.spawnDataString == "SizeMult")
                    {
                        float sizeMult = UnityEngine.Random.Range(1.0f, 1.5f);
                        spawner.spawnDataString = "{SizeMult:" + sizeMult + "}";
                    }
                }
            }
        }

        private void LogSpawner(World.CreatureSpawner spawner, int arrayIndex = -1)
        {
            if(spawner is null)
            {
                UnityEngine.Debug.Log("Received null spawner with index: " + arrayIndex);
            }
            else if (spawner is World.SimpleSpawner simpleSpawner)
            {
                UnityEngine.Debug.Log("Simple Spawner data:");
                UnityEngine.Debug.Log("ID: " + simpleSpawner.SpawnerID);
                UnityEngine.Debug.Log("Creature: " + simpleSpawner.creatureType);
                UnityEngine.Debug.Log("Amount: " + simpleSpawner.amount);
                UnityEngine.Debug.Log("Subregion: " + subregion);
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
                UnityEngine.Debug.Log("Subregion: " + subregion);
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
                spawner.creatureType == CreatureTemplate.Type.Salamander || spawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard ||
                spawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard || spawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard ||
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
               || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard)
            {
                HandleAxolotlSpawner(simpleSpawner, spawners);
                return;
            }

            //Info check before changes
            float localRedLizardChance = redLizardChance;
            if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard || (
                (region == "SU" || region == "HI") && (
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear ||
                slugcatName == SlugcatStats.Name.Red)))
            {
                if(localRedLizardChance < 1)
                    localRedLizardChance /= 2;
            }

            bool replaceForRyan = hasLizardVariants && ryanLizardChance > 0 && simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard;
            int currentCount = spawnerCount;

            //Checks for normal lizards
            if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && region == "SU")? extraGreens/2 : extraGreens, true);
                bool replacedFull =
                    ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, caramelLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && region == "OE")? extraCaramels-10:extraCaramels, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraPinks, true);
                bool replacedFull =
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, strawberryLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, extraZoops, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraBlues, true);
                bool replacedFull =
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.CyanLizard, cyanLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && region == "UW")? extraCyans/2 : extraCyans, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard)
                IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && subregion == "Filtration System")? extraBlacks - 10 : extraBlacks, true);
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.WhiteLizard)
                IncreaseCreatureSpawner(simpleSpawner, extraWhites, true);
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard)
                IncreaseCreatureSpawner(simpleSpawner, 
                    (balancedSpawns && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Rivulet || slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                    ? extraYellows/2 : extraYellows, true);
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && region == "UW") ? extraCyans / 2 : extraCyans, true);
            else if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard)
                IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && region == "OE") ? extraCaramels - 10 : extraCaramels, true);
            else if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard)
                IncreaseCreatureSpawner(simpleSpawner, extraZoops, true);

            if (balancedSpawns && region == "GW" && extraWhites > 0 && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                    AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.WhiteLizard, 0.4f);
            }

            //Red&Train lizard
            if (IsVanillaLizard(simpleSpawner) && simpleSpawner.creatureType != CreatureTemplate.Type.RedLizard)
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedLizard, balancedSpawns ? localRedLizardChance : redLizardChance);
                
            if(simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard)
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);


            //Mods
            if (replaceForRyan)
            {
                if(simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard)
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), 1f);
                }
                if(spawnerCount > currentCount)
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
                IncreaseCreatureSpawner(simpleSpawner, extraSals, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, eelLizChance);
            }
            if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraEellizs, true);
            }


            

        }

        private void HandleCentipedeSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.AquaCenti)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraAquapedes, true);
                return;
            }
            bool wasSmallCentipedes = false;
            if(simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
            {
                wasSmallCentipedes = true;
                IncreaseCreatureSpawner(simpleSpawner, ((region == "OE" || region == "SB" || region == "VS") && balancedSpawns)? extraSmallCents-10 : extraSmallCents);
                if(!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("AlternateForm"))
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, largeCentipedeChance);
                else
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, largeCentipedeChance);

            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
            {
                if(!wasSmallCentipedes)
                    IncreaseCreatureSpawner(simpleSpawner, ((region == "SB" || region == "VS") && balancedSpawns)? extraCentipedes-10 : extraCentipedes, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, ((region == "VS" || region == "SB") && balancedSpawns && redCentipedeChance < 1) ? redCentipedeChance / 2 : redCentipedeChance);                                
            }
            bool isCentiwing = simpleSpawner.creatureType == CreatureTemplate.Type.Centiwing;
            if(isCentiwing)
            {
                if(!wasSmallCentipedes && balancedSpawns)
                    IncreaseCreatureSpawner(simpleSpawner, extraCentiwings, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, (region == "LC" && balancedSpawns)? redCentipedeChance*2 : redCentipedeChance);
            }

            if(balancedSpawns && region == "GW" && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, largeCentipedeChance * 2);
                if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            }
        }

        private void HandleVultureSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {

            if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture)
            {
                if (extraVultures > 0) 
                {
                    IncreaseCreatureSpawner(simpleSpawner, extraVultures);
                    if (region == "OE" && balancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 2);
                    if (region == "SI" && balancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 3);
                }
                float localKingVultureChance = kingVultureChance;
                if (balancedSpawns && (
                    region == "SI" || region == "LC" ||
                    slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Rivulet ||
                    slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                    localKingVultureChance *= 2;
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.KingVulture, (region == "SI" && balancedSpawns)? localKingVultureChance : kingVultureChance);
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture && region == "UW")
                IncreaseCreatureSpawner(simpleSpawner, balancedSpawns? extraVultures+1 : extraVultures);

            float localMirosVultureChance = mirosVultureChance;
            if ((region == "OE" || region == "SI" || slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint) && localMirosVultureChance < 1)
                localMirosVultureChance /= 2;
            else if (region == "SL" || region == "LM" || region == "LC" || region == "MS" ||
                (region == "GW" && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear)))
                localMirosVultureChance *= 2;
            
            ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture, balancedSpawns? localMirosVultureChance : mirosVultureChance);

            if (region == "SH" && simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture && mirosVultureChance > 0)
                IncreaseCreatureSpawner(simpleSpawner, balancedSpawns? extraVultures+1 : extraVultures);

        }
        
        private void HandleLongLegsSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (!(StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs))
            {
                
                if (region == "UW" || region == "CL")
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, balancedSpawns ? brotherLongLegsChance * 2 : brotherLongLegsChance);
                else if (balancedSpawns && region == "GW" && simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, 
                        (slugcatName.ToString() == "Artificer" || slugcatName.ToString() == "Spear")? brotherLongLegsChance*2:brotherLongLegsChance);
                else
                {
                    float localBrotherChance = simpleSpawner.creatureType == CreatureTemplate.Type.JetFish ? aquapedeChance : brotherLongLegsChance;
                    if (balancedSpawns && (subregion == "Sump Tunnel" || subregion == "The Gutter" || region == "LM" ||
                        (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle"))))
                        localBrotherChance *= 2;
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, localBrotherChance);
                }
            }

            if(simpleSpawner.creatureType == CreatureTemplate.Type.BrotherLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance);
            }
            
            if(simpleSpawner.creatureType == CreatureTemplate.Type.DaddyLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
            }

            if (hasAngryInspectors &&
                StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
            {
                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, inspectorChance);
            }


        }

        private void HandleJetfishSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (region == "SL" && balancedSpawns) ? extraJetfish - 10 : extraJetfish, true);
            if (aquapedeChance > 0 && (region == "LM" || region == "SB" || region == "VS") && balancedSpawns)
                IncreaseCreatureSpawner(simpleSpawner, 12, true);

            float localWaterPredatorChance = aquapedeChance;
            if (region == "SB" || region == "VS")
                localWaterPredatorChance *= 2f;
            else if (region == "SL")
                localWaterPredatorChance *= 0.6f;

            bool replacedFull;
            replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, jetfishSalamanderChance, true);
            if(!replacedFull)
                replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, balancedSpawns? localWaterPredatorChance : aquapedeChance);

            if ((region == "SL" || region == "CL") && !replacedFull && aquapedeChance > 0)
                HandleLongLegsSpawner(simpleSpawner, spawners);
        }
        
        private void HandleCicadaSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (region == "SI" || region == "OE")? extraCicadas - 10 : extraCicadas, true);

            bool replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, (region == "SI" || region == "LC")? cicadaCentiwingChance / 2: cicadaCentiwingChance, true);
            if (!replacedFull)
            {
                replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, cicadaNoodleflyChance);
                if (replacedFull)
                    AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SmallNeedleWorm, 1f);
            }
           
        }

        private void HandleLeechSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool wasRedLeech = false;
            if(simpleSpawner.creatureType == CreatureTemplate.Type.Leech)
            {
                wasRedLeech = true;
                IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && region == "DS")? (int)(extraLeeches*1.5f) : extraLeeches);
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.JungleLeech, jungleLeechChance);
                AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, (balancedSpawns && region == "DS")? leechLizardChance*2 : leechLizardChance, true, true);
            }
            if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.JungleLeech)
            {
                if(!wasRedLeech)
                    AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, leechLizardChance, true, true);
            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraLeeches);
                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, seaLeechAquapedeChance, true, true);
                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, leechLizardChance, true, true);
            }
        }

        private void HandlePrecycleSpawns(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            int extras = extraPrecycleSals;
            if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard ||
                StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                extras-= 10;
            IncreaseCreatureSpawner(simpleSpawner, balancedSpawns ? extras : extraPrecycleSals, true);
        }

        private void IncreaseCreatureSpawner(World.SimpleSpawner simpleSpawner, int amount = 1, bool divide = false)
        {
            if (amount <= 0)
                return;
            if (balancedSpawns && bannedRooms.Contains(simpleSpawner.den.ResolveRoomName()))
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

            if (simpleSpawner.creatureType == replacement)
                return false;
            if (forceNewSpawner && spawnerCount > originalSpawnCount * 4)
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
    
        private bool AddInvasionSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type invador, float chance, bool singleRoll = false, bool reduceInvaded = false, bool removeInvaded = false)
        {
            if (simpleSpawner.creatureType == invador)
                return false;
            if (spawnerCount > originalSpawnCount * 4)
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
                    if (UnityEngine.Random.value > redLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard.Index)
                {
                    if (UnityEngine.Random.value > trainLizardChance || UnityEngine.Random.value > redLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == CreatureTemplate.Type.RedCentipede.Index)
                {
                    if (UnityEngine.Random.value > redCentipedeChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
            }
        }

        private void HandleLizardLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            if(IsCreatureInLineage(lineage, CreatureTemplate.Type.Salamander) || IsCreatureInLineage(lineage, MoreSlugcatsEnums.CreatureTemplateType.EelLizard))
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, eelLizChance);
                return;
            }

            bool replaceForRyanLiz = hasLizardVariants && IsCreatureInLineage(lineage, CreatureTemplate.Type.CyanLizard);

            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, CreatureTemplate.Type.RedLizard, redLizardChance, true);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, caramelLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BlueLizard, CreatureTemplate.Type.CyanLizard, cyanLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.PinkLizard, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, strawberryLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);


            if (forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.YellowLizard.Index && extraYellows > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.YellowLizard, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.Salamander.Index && extraSals > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.Salamander, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (forceFreshSpawns && balancedSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BlackLizard.Index && extraBlacks > 0 && region == "SH" && UnityEngine.Random.value > .5f)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BlackLizard, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }

            //Mods
            if (replaceForRyanLiz)
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, new CreatureTemplate.Type("RyanLizard"), 1f);

            if (activeMods.Contains("thefriend"))
            {
                if(forceFreshSpawns && lineage.creatureTypes[0] > -1 && lineage.creatureTypes[0] < StaticWorld.creatureTemplates.Length && 
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
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.SmallCentipede, CreatureTemplate.Type.Centipede, largeCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
        }

        private void HandleLongLegsLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);

            //Inspector invasion
            if (hasAngryInspectors)
            {
                for(int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if(lineage.creatureTypes[i] > -1 && lineage.creatureTypes[i] < StaticWorld.creatureTemplates.Length && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                    {
                        if(UnityEngine.Random.value < ((region == "UW" && balancedSpawns)? inspectorChance*2 : inspectorChance))
                        {
                            World.SimpleSpawner inspectorSpawner = new World.SimpleSpawner
                                (lineage.region, spawners.Count, lineage.den, MoreSlugcatsEnums.CreatureTemplateType.Inspector, "{Ignorecycle}", 1);
                            spawners.Add(inspectorSpawner);
                            break;
                        }
                    }
                }
            }
        }

        private bool IsCreatureInLineage(World.Lineage lineage, CreatureTemplate.Type creatureType, bool useAncestor = false)
        {
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

                if ((balancedSpawns && localRepChance > 0 && localRepChance < 1) || list[i].overrideBalance)
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
            if ((balancedSpawns && localAmount > 0) || modExtras.overrideBalance)
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
            if (lineage.creatureTypes[0] > -1 && lineage.creatureTypes[0] < StaticWorld.creatureTemplates.Length && forceFreshSpawns && fillLineages)
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
                if ((balancedSpawns && localRepChance > 0 && localRepChance < 1) || list[i].overrideBalance)
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
            if (localMultipliers.TryGetValue(region, out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (!(subregion is null) && localMultipliers.TryGetValue(subregion, out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (localMultipliers.TryGetValue(slugcatName.ToString(), out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (localMultipliers.TryGetValue(roomName, out multiplier))
            {
                foundModifier = true;
                totalMult *= multiplier;
            }
            if (localMultipliers.TryGetValue(String.Concat(region, slugcatName.ToString()), out multiplier))
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
            if (localAdditions.TryGetValue(region, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (!(subregion is null) && localAdditions.TryGetValue(subregion, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (localAdditions.TryGetValue(slugcatName.ToString(), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(roomName, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (localAdditions.TryGetValue(String.Concat(region, slugcatName.ToString()), out addition))
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
            if (localMultipliers.TryGetValue(region, out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (!(subregion is null) && localMultipliers.TryGetValue(subregion, out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (localMultipliers.TryGetValue(slugcatName.ToString(), out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (localMultipliers.TryGetValue(roomName, out multiplier))
            {
                totalMult *= multiplier;
                foundModifier = true;
            }
            if (localMultipliers.TryGetValue(String.Concat(region, slugcatName.ToString()), out multiplier))
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
            if (localAdditions.TryGetValue(region, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
            if (!(subregion is null) && localAdditions.TryGetValue(subregion, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(slugcatName.ToString(), out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(roomName, out addition))
            {
                totalAdd += addition;
                foundModifier = true;
            }
                totalAdd += addition;
            if (localAdditions.TryGetValue(String.Concat(region, slugcatName.ToString()), out addition))
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
                localAdditions.TryGetValue("Night", out addition))
                totalAdd += addition;
            if (!(lineage.spawnData[index] is null) && lineage.spawnData[index].Contains("PreCycle") &&
                localAdditions.TryGetValue("PreCycle", out addition))
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

        #region Game Manager functions
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            this.game = game;
            SetOptions();
        }

        #endregion
    }
}
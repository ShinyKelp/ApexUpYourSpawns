
namespace ApexUpYourSpawns
{
    using System;
    using System.Collections.Generic;
    using Watcher;
    using UnityEngine;
    using static ApexUtils;
    using static ApexGameInfo;
    using static SpawnerHelperFunctions;

    //Structure for the modded creature system.
    public class ModCreatureLogic
    {

        private Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>> modCreatureReplacements;
        private Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>> modCreatureAncestorReplacements;
        private Dictionary<CreatureTemplate.Type, ModCreatureExtras> modCreatureExtras;

        public ModCreatureLogic()
        {
            if (modCreatureReplacements is null)
                modCreatureReplacements = new Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>>();
            if (modCreatureAncestorReplacements is null)
                modCreatureAncestorReplacements = new Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>>();
            if (modCreatureExtras is null)
                modCreatureExtras = new Dictionary<CreatureTemplate.Type, ModCreatureExtras>();
        }

        #region Creature representation classes
        //Struct with object references. They cannot be primitives since they must access the values in-game, not during mod init.
        protected class ModCreatureReplacement
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

            public void Print()
            {
                Debug.Log("-- MOD CREATURE REPLACEMENT --");
                Debug.Log("Target Type: " + type);
                Debug.Log("Configurable: " + repChanceConfig);
                Debug.Log("Is per den: " + perDenReplacement);
                Debug.Log("Is invasion: " + isInvasion);
                Debug.Log("Override balance: " + overrideBalance);
                if (localMultipliers != null)
                {
                    Debug.Log("Local multipliers:");
                    foreach(var pair in localMultipliers)
                    {
                        Debug.Log("  " +  pair.Key + " -> " + pair.Value);
                    }
                }
                if (localAdditions != null)
                {
                    Debug.Log("Local additions:");
                    foreach (var pair in localAdditions)
                    {
                        Debug.Log("  " + pair.Key + " -> " + pair.Value);
                    }
                }
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

        protected class ModCreatureExtras
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

        #endregion

        #region Mod Creatures Setup
        public void SetUpModDependencies()
        {
            HasAngryInspectors = HasLizardVariants = false;
            List<CreatureTemplate.Type> horizontalSpawnsList = new List<CreatureTemplate.Type>()
            {
                CreatureTemplate.Type.MirosBird,
                CreatureTemplate.Type.Deer
            };
            if(ModManager.Watcher)
            {
                horizontalSpawnsList.Add(WatcherEnums.CreatureTemplateType.Loach);
                horizontalSpawnsList.Add(WatcherEnums.CreatureTemplateType.RotLoach);
                horizontalSpawnsList.Add(WatcherEnums.CreatureTemplateType.DrillCrab);
            }

            //DO NOT TRY TO USE StaticWorld.GetCreatureTemplate. IT DOES NOT WORK AT MOD LOADING TIME. USE new CreatureTemplate.Type("name")
            if (ActiveMods.Contains("lb-fgf-m4r-ik.modpack"))
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
                    OptionConfigs.Instance.GetOptionConfig("SporantulaChance"),
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
                    OptionConfigs.Instance.GetOptionConfig("SporantulaExtras")
                    ));

                //Scutigera
                Dictionary<string, float> localMultipliersS = new Dictionary<string, float>();
                localMultipliersS.Add("GWArtificer", 2f);
                localMultipliersS.Add("GWSpear", 2f);
                CreatureTemplate.Type scutType = new CreatureTemplate.Type("Scutigera");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede,
                    new ModCreatureReplacement(
                        scutType,
                        OptionConfigs.Instance.GetOptionConfig("ScutigeraChance"),
                        localMultipliersS
                    )
                    );
                modCreatureExtras.Add(scutType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("ScutigeraExtras")
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
                        OptionConfigs.Instance.GetOptionConfig("RedRedHorrorCentiChance"), null, localAdditionsRed
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
                        OptionConfigs.Instance.GetOptionConfig("WingRedHorrorCentiChance"), null, localAdditionsWing
                ));

                //Water spitter
                CreatureTemplate.Type watType = new CreatureTemplate.Type("WaterSpitter");
                ModCreatureReplacement wSpitter = new ModCreatureReplacement(
                    watType, OptionConfigs.Instance.GetOptionConfig("WaterSpitterChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, wSpitter);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.EelLizard, wSpitter);
                modCreatureExtras.Add(watType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("WaterSpitterExtras"), true));

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
                        OptionConfigs.Instance.GetOptionConfig("FatFireFlyChance"),
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
                    ssType, OptionConfigs.Instance.GetOptionConfig("SurfaceSwimmerChance"), localSSMultipliers));

                Dictionary<string, int> localSSAdds = new Dictionary<string, int>();
                localSSAdds.Add("DS", 20);
                localSSAdds.Add("SL", 20);
                localSSAdds.Add("LM", 20);

                modCreatureExtras.Add(ssType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("SurfaceSwimmerExtras"), true, null, localSSAdds));

                //Bouncing ball
                CreatureTemplate.Type bType = new CreatureTemplate.Type("BouncingBall");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, new ModCreatureReplacement(
                    bType, OptionConfigs.Instance.GetOptionConfig("BounceBallChance")));
                modCreatureExtras.Add(bType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("BounceBallExtras"), true));

                //Hoverfly
                CreatureTemplate.Type hoverType = new CreatureTemplate.Type("Hoverfly");
                ModCreatureReplacement hoverflyRep = new ModCreatureReplacement(hoverType, OptionConfigs.Instance.GetOptionConfig("CritterHoverflyChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallNeedleWorm, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.LanternMouse, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TubeWorm, hoverflyRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.Yeek, hoverflyRep);
                modCreatureExtras.Add(hoverType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("HoverflyExtras")));

                //Tailfly
                CreatureTemplate.Type tailflyType = new CreatureTemplate.Type("Tailfly");
                ModCreatureReplacement tailFlyRep = new ModCreatureReplacement(tailflyType, OptionConfigs.Instance.GetOptionConfig("TailflyChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaA, tailFlyRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaB, tailFlyRep);
                modCreatureExtras.Add(tailflyType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("TailflyExtras")));

                //Noodle Eater
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallNeedleWorm, new ModCreatureReplacement(
                    new CreatureTemplate.Type("NoodleEater"), OptionConfigs.Instance.GetOptionConfig("NoodleEaterChance"), true, true));
                modCreatureExtras.Add(new CreatureTemplate.Type("NoodleEater"), new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("NoodleEaterExtras")));

                //Thornbug, TintedBeetle, Chipchop, Mamabug
                CreatureTemplate.Type thornType = new CreatureTemplate.Type("ThornBug");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    thornType, OptionConfigs.Instance.GetOptionConfig("ThornbugChance"), true));
                modCreatureExtras.Add(thornType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("ThornbugExtras")));

                CreatureTemplate.Type tintedType = new CreatureTemplate.Type("TintedBeetle");
                Dictionary<string, float> tintedLocals = new Dictionary<string, float>();
                tintedLocals.Add("LF", 2f);
                tintedLocals.Add("SB", 2f);
                tintedLocals.Add("OE", 1.5f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    tintedType, OptionConfigs.Instance.GetOptionConfig("TintedBeetleChance"), tintedLocals));
                modCreatureExtras.Add(tintedType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("TintedBeetleExtras")));

                CreatureTemplate.Type chipchopType = new CreatureTemplate.Type("ChipChop");
                Dictionary<string, float> chipchopLocals = new Dictionary<string, float>();
                chipchopLocals.Add("SH", 2f);
                chipchopLocals.Add("SL", 1.5f);
                chipchopLocals.Add("UW", 2f);
                Dictionary<string, int> chipchopLocals2 = new Dictionary<string, int>();
                chipchopLocals2.Add("SH", 20);
                chipchopLocals2.Add("SL", 10);
                chipchopLocals2.Add("UW", 20);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    chipchopType, OptionConfigs.Instance.GetOptionConfig("ChipChopChance")));
                modCreatureExtras.Add(chipchopType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("ChipChopExtras"), chipchopLocals, chipchopLocals2));

                CreatureTemplate.Type mamaType = new CreatureTemplate.Type("MamaBug");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.EggBug, new ModCreatureReplacement(
                    mamaType, OptionConfigs.Instance.GetOptionConfig("MamaBugChance")));
                modCreatureExtras.Add(mamaType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("MamaBugExtras")));

                //Mini leviathan
                Dictionary<string, float> miniLeviDict = new Dictionary<string, float>();
                miniLeviDict.Add("SB", 1.5f);
                miniLeviDict.Add("MS", 1.5f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BigEel, new ModCreatureReplacement(
                    new CreatureTemplate.Type("MiniLeviathan"), OptionConfigs.Instance.GetOptionConfig("MiniLeviathanChance"), true, false, miniLeviDict));
                modCreatureExtras.Add(new CreatureTemplate.Type("MiniLeviathan"), new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("MiniLeviathanExtras"), false));

                //Polliwog
                CreatureTemplate.Type polliType = new CreatureTemplate.Type("Polliwog");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, new ModCreatureReplacement(
                    polliType, OptionConfigs.Instance.GetOptionConfig("PolliwogChance")));
                modCreatureExtras.Add(polliType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("PolliwogExtras")));

                //Hunter seeker (white-cyan)
                CreatureTemplate.Type hunType = new CreatureTemplate.Type("HunterSeeker");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, new ModCreatureReplacement(
                    hunType, OptionConfigs.Instance.GetOptionConfig("HunterSeekerCyanChance")));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.WhiteLizard, new ModCreatureReplacement(
                    hunType, OptionConfigs.Instance.GetOptionConfig("HunterSeekerWhiteChance")));
                modCreatureExtras.Add(hunType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("HunterSeekerExtras")));

                //Silver lizard
                CreatureTemplate.Type silType = new CreatureTemplate.Type("SilverLizard");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, new ModCreatureReplacement(
                    silType, OptionConfigs.Instance.GetOptionConfig("SilverLizChance")));
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, new ModCreatureReplacement(
                    silType, OptionConfigs.Instance.GetOptionConfig("SilverLizChance")));
                modCreatureExtras.Add(silType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("SilverLizExtras")));

                //Mole salamander, blizzor
                CreatureTemplate.Type salamoleType = new CreatureTemplate.Type("MoleSalamander");
                CreatureTemplate.Type blizzorType = new CreatureTemplate.Type("Blizzor");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.MirosBird, new ModCreatureReplacement(
                    blizzorType, OptionConfigs.Instance.GetOptionConfig("BlizzorChance")));
                modCreatureExtras.Add(blizzorType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("BlizzorExtras"), false));

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Salamander, new ModCreatureReplacement(
                    salamoleType, OptionConfigs.Instance.GetOptionConfig("SalamanderSalamoleChance")));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlackLizard, new ModCreatureReplacement(
                    salamoleType, OptionConfigs.Instance.GetOptionConfig("BlackSalamoleChance")));
                horizontalSpawnsList.Add(blizzorType);

                //Mini echo leviathan
                CreatureTemplate.Type miniEchoLeviType = new CreatureTemplate.Type("MiniFlyingBigEel");
                CreatureTemplate.Type echoLeviType = new CreatureTemplate.Type("FlyingBigEel");

                AddModCreatureToDictionary(modCreatureReplacements, echoLeviType, new ModCreatureReplacement(
                        miniEchoLeviType, OptionConfigs.Instance.GetOptionConfig("MiniEchoLeviChance"), true, true));
                modCreatureExtras.Add(miniEchoLeviType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("MiniEchoLeviExtras"), true));

                //Alpha orange
                CreatureTemplate.Type alphaOType = new CreatureTemplate.Type("AlphaOrange");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                    alphaOType, OptionConfigs.Instance.GetOptionConfig("AlphaOrangeChance"), true, true));

                //Scavenger Sentinel
                Dictionary<string, float> sentinelLocals = new Dictionary<string, float>();
                sentinelLocals.Add("SU", 2f);
                sentinelLocals.Add("IC", 2f);
                sentinelLocals.Add("SL", 1.5f);
                sentinelLocals.Add("DS", 1.5f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Scavenger, new ModCreatureReplacement(
                    new CreatureTemplate.Type("ScavengerSentinel"), OptionConfigs.Instance.GetOptionConfig("ScavengerSentinelChance"), sentinelLocals));

                //Miniscuti
                Dictionary<string, float> miniscutLocals = new Dictionary<string, float>();
                miniscutLocals.Add("SH", 2f);
                miniscutLocals.Add("UW", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallCentipede, new ModCreatureReplacement(
                    new CreatureTemplate.Type("MiniScutigera"), OptionConfigs.Instance.GetOptionConfig("MiniScutigeraChance"), miniscutLocals));

                //Pillars
                CreatureTemplate.Type killerType = new CreatureTemplate.Type("Killerpillar");
                ModCreatureReplacement killerRep = new ModCreatureReplacement(killerType, OptionConfigs.Instance.GetOptionConfig("KillerpillarChance"), false, true);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.PinkLizard, killerRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, killerRep);
                modCreatureExtras.Add(killerType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("KillerpillarExtras")));

                CreatureTemplate.Type glowType = new CreatureTemplate.Type("Glowpillar");
                Dictionary<string, float> glowLocals = new Dictionary<string, float>();
                glowLocals.Add("YellowLizard", 0.5f);
                ModCreatureReplacement glowRep = new ModCreatureReplacement(glowType, OptionConfigs.Instance.GetOptionConfig("GlowpillarChance"), false, true, glowLocals);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlackLizard, glowRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, glowRep);
                modCreatureExtras.Add(glowType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("GlowpillarExtras")));

                //Spark Eye
                CreatureTemplate.Type sparkType = new CreatureTemplate.Type("SparkEye");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.MirosBird, new ModCreatureReplacement(
                    sparkType, OptionConfigs.Instance.GetOptionConfig("SparkEyeChance")));
                horizontalSpawnsList.Add(sparkType);


                //Diving beetle
                CreatureTemplate.Type divingType = new CreatureTemplate.Type("DivingBeetle");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.JetFish, new ModCreatureReplacement(
                    divingType, OptionConfigs.Instance.GetOptionConfig("DivingBeetleChance"), true));
                modCreatureExtras.Add(divingType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("DivingBeetleExtras")));

                //Common Eel
                CreatureTemplate.Type eelType = new CreatureTemplate.Type("CommonEel");
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.EelLizard, new ModCreatureReplacement(
                    eelType, OptionConfigs.Instance.GetOptionConfig("CommonEelChance")));
                modCreatureExtras.Add(eelType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("CommonEelExtras")));

                //Denture
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.PoleMimic, new ModCreatureReplacement(
                    new CreatureTemplate.Type("Denture"), OptionConfigs.Instance.GetOptionConfig("DentureChance")));

                //MiniBlackLeech
                CreatureTemplate.Type miniLeechType = new CreatureTemplate.Type("MiniBlackLeech");
                Dictionary<string, int> blackLocals = new Dictionary<string, int>();
                blackLocals.Add("!", 3);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Leech, new ModCreatureReplacement(
                    miniLeechType, OptionConfigs.Instance.GetOptionConfig("MiniBlackLeechChance"), null, blackLocals));
                modCreatureExtras.Add(miniLeechType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("MiniBlackLeechExtras")));
            }
            if (ActiveMods.Contains("ShinyKelp.AngryInspectors"))
            {
                HasAngryInspectors = true;
            }
            if (ActiveMods.Contains("moredlls"))
            {
                Dictionary<string, int> localAdditionsExp = new Dictionary<string, int>();
                localAdditionsExp.Add("GWArtificer", 10);
                localAdditionsExp.Add("GWSpear", 10);
                ModCreatureReplacement expDLL = new ModCreatureReplacement(
                        new CreatureTemplate.Type("ExplosiveDaddyLongLegs"),
                        OptionConfigs.Instance.GetOptionConfig("MExplosiveLongLegsChance"),
                        null,
                        localAdditionsExp);

                Dictionary<string, int> localAdditionsZap = new Dictionary<string, int>();
                localAdditionsZap.Add("UW", 10);
                ModCreatureReplacement zapDLL = new ModCreatureReplacement(
                        new CreatureTemplate.Type("ZapDaddyLongLegs"),
                        OptionConfigs.Instance.GetOptionConfig("MZappyLongLegsChance"),
                        null,
                        localAdditionsZap);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, expDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, expDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BrotherLongLegs, zapDLL);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.DaddyLongLegs, zapDLL);
            }
            if (ActiveMods.Contains("ShinyKelp.LizardVariants"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("RyanLizard"),
                        OptionConfigs.Instance.GetOptionConfig("RyanLizardChance")));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, new ModCreatureReplacement(
                        new CreatureTemplate.Type("YellowLimeLizard"),
                        OptionConfigs.Instance.GetOptionConfig("YellowLimeLizardChance")));
                CreatureTemplate.Type mintType = new CreatureTemplate.Type("MintLizard");

                ModCreatureReplacement mintLiz = new ModCreatureReplacement(
                        mintType,
                        OptionConfigs.Instance.GetOptionConfig("MintLizardChance"));

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, mintLiz);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, mintLiz);
                modCreatureExtras.Add(mintType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("MintLizardExtras"), true));

                HasLizardVariants = true;
            }
            if (ActiveMods.Contains("thefriend"))
            {
                Dictionary<string, float> motherDic = new Dictionary<string, float>();
                motherDic.Add("OE", .5f);
                ModCreatureReplacement motherRep = new ModCreatureReplacement(new CreatureTemplate.Type("MotherLizard"), OptionConfigs.Instance.GetOptionConfig("MotherLizardChance"), motherDic);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, motherRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, motherRep);
                if (ActiveMods.Contains("ShinyKelp.LizardVariants"))
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
                    snowType, OptionConfigs.Instance.GetOptionConfig("SnowSpiderChance"), snowSpiderModifs));

                Dictionary<string, float> lostYoungModif = new Dictionary<string, float>();
                lostYoungModif.Add("PinkLizard", .5f);

                ModCreatureReplacement youngLizRep = new ModCreatureReplacement(lizType,
                    OptionConfigs.Instance.GetOptionConfig("LostYoungLizardChance"), true, true, lostYoungModif);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.YellowLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.PinkLizard, youngLizRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.ZoopLizard, youngLizRep);

                modCreatureExtras.Add(lizType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("YoungLizardExtras")));
                modCreatureExtras.Add(snowType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("SnowSpiderExtras")));
            }
            if (ActiveMods.Contains("Outspector"))
            {
                CreatureTemplate.Type outType = new CreatureTemplate.Type("Outspector");
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.Inspector, new ModCreatureReplacement(
                    outType, OptionConfigs.Instance.GetOptionConfig("OutspectorChance")));
                modCreatureExtras.Add(outType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("OutspectorExtras"), true));
            }
            if (ActiveMods.Contains("theincandescent"))
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
                        icyType, OptionConfigs.Instance.GetOptionConfig("IcyBlueYellowChance"), icyBlueBD)
                    );

                AddModCreatureToDictionary(modCreatureReplacements, freezerType, new ModCreatureReplacement(
                        icyType, OptionConfigs.Instance.GetOptionConfig("IcyBlueFreezerInvChance"), true, true, icyBlueBD)
                    );

                Dictionary<string, float> icyBlueBD2 = new Dictionary<string, float>();
                icyBlueBD2.Add("Night", 1.25f);
                icyBlueBD2.Add("PreCycle", 0.5f);
                icyBlueBD2.Add("SUIncandescent", 0.5f);
                icyBlueBD2.Add("Saint", 0f);
                icyBlueBD2.Add("Bitter Aerie", 2f);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, new ModCreatureReplacement(
                        icyType, OptionConfigs.Instance.GetOptionConfig("IcyBlueBlueChance"), true, true, icyBlueBD2)
                    );
                Dictionary<string, float> icyBlueBD3 = new Dictionary<string, float>();
                icyBlueBD3.Add("Saint", 2f);
                icyBlueBD.Add("!", 0f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, new ModCreatureReplacement(
                        icyType, OptionConfigs.Instance.GetOptionConfig("IcyBlueBlueChance"), false, true, icyBlueBD3)
                    );

                modCreatureExtras.Add(icyType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("IcyBlueLizExtras"), true));


                Dictionary<string, float> freezerBD = new Dictionary<string, float>();
                freezerBD.Add("Night", 1.5f);
                freezerBD.Add("PreCycle", 0f);
                freezerBD.Add("SUIncandescent", 0.5f);
                freezerBD.Add("Bitter Aerie", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, new ModCreatureReplacement(
                        freezerType, OptionConfigs.Instance.GetOptionConfig("FreezerLizChance"), false, true, freezerBD)
                    );
                AddModCreatureToDictionary(modCreatureReplacements, icyType, new ModCreatureReplacement(
                        freezerType, OptionConfigs.Instance.GetOptionConfig("FreezerLizChance"), false, true, freezerBD)
                    );

                Dictionary<string, float> cyanwingBD = new Dictionary<string, float>();
                cyanwingBD.Add("Night", 1.25f);
                cyanwingBD.Add("PreCycle", 0.5f);
                cyanwingBD.Add("SISaint", 0.5f);
                cyanwingBD.Add("SIIncandescent", 0.5f);

                CreatureTemplate.Type cyanType = new CreatureTemplate.Type("Cyanwing");

                ModCreatureReplacement cyanwingRep = new ModCreatureReplacement(cyanType, OptionConfigs.Instance.GetOptionConfig("CyanwingChance"), false, true, cyanwingBD);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centipede, cyanwingRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SmallCentipede, cyanwingRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Centiwing, new ModCreatureReplacement(
                        cyanType, OptionConfigs.Instance.GetOptionConfig("WingCyanwingChance"), false, true, cyanwingBD
                    ));

                Dictionary<string, int> babyAquapedeBD = new Dictionary<string, int>();
                babyAquapedeBD.Add("Sump Tunnel", 45);

                CreatureTemplate.Type aquaType = new CreatureTemplate.Type("InfantAquapede");
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.AquaCenti, new ModCreatureReplacement(
                        aquaType, OptionConfigs.Instance.GetOptionConfig("BabyAquapedeInvChance"), true)
                    );
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.JetFish, new ModCreatureReplacement(
                        aquaType, OptionConfigs.Instance.GetOptionConfig("JetfishBabyAquapedeChance"), true, false, null, babyAquapedeBD)
                    );
                modCreatureExtras.Add(aquaType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("AquapedeExtras"), true));
                //Chillipede

                CreatureTemplate.Type chillType = new CreatureTemplate.Type("Chillipede");
                ModCreatureReplacement chillRep = new ModCreatureReplacement(chillType, OptionConfigs.Instance.GetOptionConfig("ChillipedeChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.GreenLizard, chillRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.SpitLizard, chillRep);
                if (ActiveMods.Contains("ShinyKelp.LizardVariants"))
                    AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("MintLizard"), chillRep);

                Dictionary<string, float> subChillipedes = new Dictionary<string, float>();
                subChillipedes.Add("SB", 1f);
                subChillipedes.Add("!", 0f);
                ModCreatureReplacement subChillRep = new ModCreatureReplacement(chillType, OptionConfigs.Instance.GetOptionConfig("ChillipedeChance"), subChillipedes, null, true);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BlueLizard, subChillRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.WhiteLizard, subChillRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CyanLizard, subChillRep);

                bool updatedHailstorm = false;
                foreach (ModManager.Mod mod in ModManager.ActiveMods)
                {
                    if (mod.id == "theincandescent")
                    {
                        if (mod.version.Substring(0, 3) != "0.2")
                            updatedHailstorm = true;
                        break;
                    }
                }
                if (updatedHailstorm)
                {
                    ModCreatureReplacement kelpChillRep = new ModCreatureReplacement(chillType, OptionConfigs.Instance.GetOptionConfig("ChillipedeChance"), true);
                    AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TentaclePlant, kelpChillRep);

                }
            }
            if (ActiveMods.Contains("lurzard.pitchblack"))
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
                    nightType, OptionConfigs.Instance.GetOptionConfig("NightTerrorChance"), false, false, null, nightTerrorDict));

                //Night terror replace red cent in shaded/night
                Dictionary<string, int> redCentRepDict = new Dictionary<string, int>();
                redCentRepDict.Add("SH", 30);
                redCentRepDict.Add("Night", 20);
                redCentRepDict.Add("!", -10);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.RedCentipede, new ModCreatureReplacement(
                        nightType, OptionConfigs.Instance.GetOptionConfig("NightTerrorChance"), false, false, null, redCentRepDict
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
                    littleType, OptionConfigs.Instance.GetOptionConfig("BrotherLittleLongLegChance"), true, false, lllDict));

                ModCreatureReplacement lllRep = new ModCreatureReplacement(
                    littleType, OptionConfigs.Instance.GetOptionConfig("CritterLittleLongLegsChance"), true, false, lllDict);

                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Snail, lllRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.LanternMouse, lllRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.TubeWorm, lllRep);

                //Extras
                modCreatureExtras.Add(littleType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("LittleLongLegsExtras"), true));

            }
            if (ActiveMods.Contains("Croken.bombardier-vulture"))
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
                    new CreatureTemplate.Type("BombardierVulture"), OptionConfigs.Instance.GetOptionConfig("BombVultureChance"), bombDict));
            }
            if (ActiveMods.Contains("drainmites"))
            {
                Dictionary<string, float> drainmiteDict = new Dictionary<string, float>();
                drainmiteDict.Add("!", 1f);
                drainmiteDict.Add("PreCycle", 0f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Scavenger, new ModCreatureReplacement(
                    new CreatureTemplate.Type("DrainMite"), OptionConfigs.Instance.GetOptionConfig("DrainMiteChance"), true, true, drainmiteDict, null, true));
            }
            if (ActiveMods.Contains("myr.moss_fields") || ActiveMods.Contains("ShinyKelp.Udonfly"))
            {
                Dictionary<string, float> fatFlyDict = new Dictionary<string, float>();
                fatFlyDict.Add("SI", 1.5f);
                Dictionary<string, int> fatFlyDict2 = new Dictionary<string, int>();
                fatFlyDict2.Add("SI", 10);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.BigNeedleWorm, new ModCreatureReplacement(
                    new CreatureTemplate.Type("SnootShootNoot"), OptionConfigs.Instance.GetOptionConfig("FatNootChance"), fatFlyDict, fatFlyDict2));
            }
            if (ActiveMods.Contains("pkuyo.thevanguard"))
            {
                Dictionary<string, float> toxicDict = new Dictionary<string, float>();
                toxicDict.Add("GW", 2f);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SpitterSpider, new ModCreatureReplacement(
                    new CreatureTemplate.Type("ToxicSpider"), OptionConfigs.Instance.GetOptionConfig("ToxicSpiderChance")));
            }
            if (ActiveMods.Contains("shrimb.scroungers") || ActiveMods.Contains("shrimb.frostbite"))
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
                    scrType, OptionConfigs.Instance.GetOptionConfig("ScroungerChance"), scroungerDict));
                modCreatureExtras.Add(scrType, new ModCreatureExtras(
                    OptionConfigs.Instance.GetOptionConfig("ScroungerExtras"), false));
            }
            if (ActiveMods.Contains("Croken.Mimicstarfish"))
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
                    new CreatureTemplate.Type("Mimicstar"), OptionConfigs.Instance.GetOptionConfig("BllMimicstarfishChance"), null, starfishDict));

                Dictionary<string, int> starfishDict2 = new Dictionary<string, int>();
                starfishDict.Add("JetFish", 3);
                starfishDict.Add("AquaCenti", 4);
                ModCreatureReplacement critterMimicRep = new ModCreatureReplacement(new CreatureTemplate.Type("Mimicstar"),
                    OptionConfigs.Instance.GetOptionConfig("CritterMimicstarfishChance"), false, true, null, starfishDict2);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.JetFish, critterMimicRep);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Snail, critterMimicRep);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Leech, critterMimicRep);
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.AquaCenti, critterMimicRep);

            }
            if (ActiveMods.Contains("bebra.gregtech_lizard"))
            {
                CreatureTemplate.Type elecLiz = new CreatureTemplate.Type("GregTechLizard");
                Dictionary<string, float> elecFiltersL = new Dictionary<string, float>();
                Dictionary<string, float> elecFiltersC = new Dictionary<string, float>();
                elecFiltersL.Add("Salamander", 0f);
                elecFiltersL.Add("EelLizard", 0f);
                elecFiltersC.Add("AquaCenti", 0f);
                if (ActiveMods.Contains("theincandescent"))
                    elecFiltersC.Add("InfantAquapede", 0f);
                if (ActiveMods.Contains("lb-fgf-m4r-ik.water-spitter"))
                    elecFiltersL.Add("WaterSpitter", 0f);
                if (ActiveMods.Contains("lb-fgf-m4r-ik.coral-reef"))
                    elecFiltersL.Add("Polliwog", 0f);
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.LizardTemplate,
                    new ModCreatureReplacement(elecLiz, OptionConfigs.Instance.GetOptionConfig("LizardElectricLizChance"), true, true, elecFiltersL, null, true));
                AddModCreatureToDictionary(modCreatureAncestorReplacements, CreatureTemplate.Type.Centipede,
                    new ModCreatureReplacement(elecLiz, OptionConfigs.Instance.GetOptionConfig("LizardElectricLizChance"), true, true, elecFiltersC, null, true));
            }
            if (ActiveMods.Contains("bry.bubbleweavers"))
            {
                CreatureTemplate.Type bubbleType1 = new CreatureTemplate.Type("BubbleWeaver"),
                    bubbleType2 = new CreatureTemplate.Type("SapphiricWeaver");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SpitterSpider, new ModCreatureReplacement(
                    bubbleType1, OptionConfigs.Instance.GetOptionConfig("SpiderWeaverChance")));
                if (ActiveMods.Contains("lb-fgf-m4r-ik.modpack"))
                {
                    AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("SurfaceSwimmer"), new ModCreatureReplacement(
                        bubbleType2, OptionConfigs.Instance.GetOptionConfig("SSwimmerWeaverChance")));
                }
                modCreatureExtras.Add(bubbleType1, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("BubbleWeaverExtras")));
                modCreatureExtras.Add(bubbleType2, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("BubbleWeaverExtras")));
            }
            if (ActiveMods.Contains("com.rainworldgame.shroudedassembly.plugin"))
            {
                //Geckos
                CreatureTemplate.Type geckoType = new CreatureTemplate.Type("Gecko");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.PinkLizard, new ModCreatureReplacement(
                    geckoType, OptionConfigs.Instance.GetOptionConfig("GeckoChance")));
                modCreatureExtras.Add(geckoType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("GeckoExtras")));

                //Maraca spiders
                CreatureTemplate.Type maracaType = new CreatureTemplate.Type("MaracaSpider");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.SpitterSpider, new ModCreatureReplacement(
                    maracaType, OptionConfigs.Instance.GetOptionConfig("MaracaSpiderChance")));
                modCreatureExtras.Add(maracaType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("MaracaSpiderExtras")));


                //Baby croakers
                CreatureTemplate.Type croakerType = new CreatureTemplate.Type("BabyCroaker");
                ModCreatureReplacement croakerRep = new ModCreatureReplacement(croakerType, OptionConfigs.Instance.GetOptionConfig("BabyCroakerChance"), true);
                ModCreatureReplacement croakerRep2 = new ModCreatureReplacement(croakerType, OptionConfigs.Instance.GetOptionConfig("MosquitoBabyCroakerChance"), true, true);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaA, croakerRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaB, croakerRep);
                modCreatureExtras.Add(croakerType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("BabyCroakerExtras")));
                if(ActiveMods.Contains("mosquitoes"))
                    AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("Mosquito"), croakerRep2);
            }
            if (ActiveMods.Contains("sequoia.luminouscode"))
            {
                CreatureTemplate.Type teuType = new CreatureTemplate.Type("Teuthicada");
                ModCreatureReplacement teuRep = new ModCreatureReplacement(teuType, OptionConfigs.Instance.GetOptionConfig("TeuthicadaChance"));
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaA, teuRep);
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.CicadaB, teuRep);
                modCreatureExtras.Add(teuType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("TeuthicadaExtras")));
            }
            if (ActiveMods.Contains("sequoia.parascutigera-creature"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, new CreatureTemplate.Type("Scutigera"), new ModCreatureReplacement(
                    new CreatureTemplate.Type("Parascutigera"), OptionConfigs.Instance.GetOptionConfig("ParascutigeraChance")));
            }
            if (ActiveMods.Contains("mosquitoes"))
            {
                CreatureTemplate.Type mosquitoType = new CreatureTemplate.Type("Mosquito");
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.BigNeedleWorm, new ModCreatureReplacement(
                    mosquitoType, OptionConfigs.Instance.GetOptionConfig("MosquitoChance"), false, true));
                AddModCreatureToDictionary(modCreatureReplacements, mosquitoType, new ModCreatureReplacement(
                    new CreatureTemplate.Type("ExplodingMosquito"), OptionConfigs.Instance.GetOptionConfig("ExplodingMosquitoChance")));
                AddModCreatureToDictionary(modCreatureReplacements, mosquitoType, new ModCreatureReplacement(
                    new CreatureTemplate.Type("AngryMosquito"), OptionConfigs.Instance.GetOptionConfig("AngryMosquitoChance")));
                Dictionary<string, int> mosLocals = new Dictionary<string, int>();
                mosLocals.Add("!", 20);
                modCreatureExtras.Add(mosquitoType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("MosquitoExtras"), null, mosLocals, true));
            }
            if (ActiveMods.Contains("SnowBee.Snow"))
            {
                CreatureTemplate.Type crypticType = new CreatureTemplate.Type("CrypticCenti");
                AddModCreatureToDictionary(modCreatureReplacements, DLCSharedEnums.CreatureTemplateType.Inspector, new ModCreatureReplacement(
                    crypticType, OptionConfigs.Instance.GetOptionConfig("CrypticCentiChance"), true));
                modCreatureExtras.Add(crypticType, new ModCreatureExtras(OptionConfigs.Instance.GetOptionConfig("CrypticCentiExtras")));
            }
            if (ActiveMods.Contains("com.rainworldgame.shroudedassembly"))
            {
                AddModCreatureToDictionary(modCreatureReplacements, CreatureTemplate.Type.Vulture, new ModCreatureReplacement(
                    new CreatureTemplate.Type("FogVulture"), OptionConfigs.Instance.GetOptionConfig("FogVultureChance")));
            }

            HorizontalSpawns = horizontalSpawnsList.ToArray();
        }


        #endregion

        public void ClearDictionaries()
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
        }

        private void AddModCreatureToDictionary(Dictionary<CreatureTemplate.Type, List<ModCreatureReplacement>> dict, CreatureTemplate.Type key, ModCreatureReplacement modCreature)
        {
            if (dict.TryGetValue(key, out List<ModCreatureReplacement> list))
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

        public void RefreshModCreatures()
        {
            foreach (CreatureTemplate.Type hType in HorizontalSpawns)
            {
                if (hType.index == -1)
                {
                    hType.index = new CreatureTemplate.Type(hType.ToString()).index;
                    if (DebugLogs)
                    {
                        Debug.Log("Updated Horizontal Spawn Creature: " + hType + " to Index " + hType.index);
                    }
                }

            }

            if (!(modCreatureReplacements is null))
            {
                foreach (KeyValuePair<CreatureTemplate.Type, List<ModCreatureReplacement>> pair in modCreatureReplacements)
                {
                    foreach (ModCreatureReplacement modRep in pair.Value)
                    {
                        if (modRep.type.index == -1)
                        {
                            //modRep.type = StaticWorld.GetCreatureTemplate(modRep.type.ToString()).type;
                            //UnityEngine.Debug.Log(modRep.type.ToString() + ":");
                            //UnityEngine.Debug.Log(new CreatureTemplate.Type(modRep.type.ToString()).index);
                            //UnityEngine.Debug.Log(StaticWorld.GetCreatureTemplate(modRep.type.ToString()));
                            modRep.type = new CreatureTemplate.Type(modRep.type.ToString());
                            if (DebugLogs)
                            {
                                if(modRep.type.index < 0)
                                    Debug.Log("WARNING: Could not update index of creature type: " +  modRep.type);
                                else
                                    Debug.Log("Updated Mod Replacement creature " + modRep.type + " to Index " + modRep.type.index);
                            }
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
                            if (DebugLogs)
                            {
                                if (modRep.type.index < 0)
                                    Debug.Log("WARNING: Could not update index of creature type: " + modRep.type);
                                else
                                    Debug.Log("Updated Mod Replacement Ancestor creature " + modRep.type + " to Index " + modRep.type.index);
                            }
                        }
                    }
                }
            }

            if (!(modCreatureExtras is null))
            {
                Dictionary<CreatureTemplate.Type, ModCreatureExtras> auxDix = new Dictionary<CreatureTemplate.Type, ModCreatureExtras>();
                foreach (KeyValuePair<CreatureTemplate.Type, ModCreatureExtras> pair in modCreatureExtras)
                {
                    if (pair.Key.index == -1)
                    {
                        auxDix.Add(new CreatureTemplate.Type(pair.Key.ToString()), pair.Value);
                    }
                }

                foreach (KeyValuePair<CreatureTemplate.Type, ModCreatureExtras> pair in auxDix)
                {
                    modCreatureExtras.Remove(pair.Key);
                    modCreatureExtras.Add(pair.Key, pair.Value);
                    if (DebugLogs)
                    {
                        if (pair.Key.index < 0)
                            Debug.Log("WARNING: Could not update index of creature type: " + pair.Key.ToString());
                        else
                            Debug.Log("Updated Mod Extras creature " + pair.Key + " to index " + pair.Key.index);
                    }
                }
                auxDix.Clear();
            }
            HasUpdatedRefs = true;
            if (DebugLogs)
                Debug.Log("Finished updating mod creature indexes.");
        }

        #region Mod Creature Edits functions

        public void CheckModCreatures(World.SimpleSpawner spawner, List<World.CreatureSpawner> spawners)
        {
            if (spawner is null || spawner.creatureType is null)
                return;
            if (!(modCreatureReplacements is null) && modCreatureReplacements.TryGetValue(spawner.creatureType, out List<ModCreatureReplacement> list))
            {
                ApplyModCreatureEdits(spawner, spawners, list);
            }

            if (!(modCreatureAncestorReplacements is null) && !(StaticWorld.GetCreatureTemplate(spawner.creatureType) is null) &&
                modCreatureAncestorReplacements.TryGetValue(
                StaticWorld.GetCreatureTemplate(spawner.creatureType).TopAncestor().type, out List<ModCreatureReplacement> list2))
            {

                bool isInList = false;
                for (int i = 0; i < list2.Count; ++i)
                {
                    if (list2[i].type == spawner.creatureType)
                    {
                        isInList = true;
                        break;
                    }
                }
                if (!isInList)
                    ApplyModCreatureEdits(spawner, spawners, list2);
            }

            if (!(modCreatureExtras is null) && modCreatureExtras.TryGetValue(spawner.creatureType, out ModCreatureExtras modExtras))
            {
                ApplyModCreatureEdits(spawner, modExtras);
            }
        }

        protected void ApplyModCreatureEdits(World.SimpleSpawner spawner, List<World.CreatureSpawner> spawners, List<ModCreatureReplacement> list)
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

        protected void ApplyModCreatureEdits(World.SimpleSpawner spawner, ModCreatureExtras modExtras)
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

        public void CheckModCreatures(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            for (int k = 0; k < lineage.creatureTypes.Length; ++k)
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
                    while (i < list.Count)
                    {
                        if (list[i].isInvasion)
                        {
                            if (UnityEngine.Random.value < list[i].repChance)
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
                    if (value.amount > 0)
                    {
                        World.SimpleSpawner newSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den,
                        StaticWorld.creatureTemplates[lineage.creatureTypes[0]].type, lineage.spawnData[0], 0);
                        spawners.Add(newSpawner);
                    }
                }
            }
        }

        protected void ApplyModCreatureEdits(World.Lineage lineage, int index, List<World.CreatureSpawner> spawners, List<ModCreatureReplacement> list)
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

        protected float GetLocalMultipliers(Dictionary<string, float> localMultipliers, World.SimpleSpawner spawner)
        {
            float multiplier;
            float totalMult = 1f;
            bool foundModifier = false;
            if (localMultipliers.TryGetValue(CurrentRegion, out multiplier))
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
            if (localMultipliers.TryGetValue(String.Concat(CurrentRegion, SlugcatName.ToString()), out multiplier))
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
            if (localAdditions.TryGetValue(CurrentRegion, out addition))
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
            if (localAdditions.TryGetValue(String.Concat(CurrentRegion, SlugcatName.ToString()), out addition))
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
            if (localMultipliers.TryGetValue(CurrentRegion, out multiplier))
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
            if (localMultipliers.TryGetValue(String.Concat(CurrentRegion, SlugcatName.ToString()), out multiplier))
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
            if (localAdditions.TryGetValue(CurrentRegion, out addition))
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
            if (localAdditions.TryGetValue(String.Concat(CurrentRegion, SlugcatName.ToString()), out addition))
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
    }
}
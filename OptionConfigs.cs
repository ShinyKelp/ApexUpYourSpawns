
namespace ApexUpYourSpawns
{
    using System.Collections.Generic;
    using static OptionInterface;
    
    //Storage and handling of creature replacement/increase config variables.
    public class OptionConfigs
    {
        ConfigHolder config;
        private bool appliedVanilla = false;
        private bool appliedSharedDLC = false;
        private bool appliedMSC = false;
        private bool appliedWatcher = false;
        public readonly Dictionary<string, Configurable<int>> VanillaRepConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> VanillaExtraConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> DLCRepConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> DLCExtraConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> WatcherRepConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> WatcherExtraConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> ModRepConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> ModExtraConfigs = new Dictionary<string, Configurable<int>>();

        public readonly Dictionary<Configurable<int>, string> LabelsMap = new Dictionary<Configurable<int>, string>();

        public static OptionConfigs Instance { get; private set; } = null;

        public OptionConfigs(ConfigHolder config)
        {
            UnityEngine.Debug.Log("OPTION CONFIGS CTOR.");
            Instance = this;
            this.config = config;
        }

        public void UpdateBindings()
        {
            UnityEngine.Debug.Log("UPDATE BINDINGS.");
            if (!appliedVanilla)
            {
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
                appliedVanilla = true;
            }
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

            if (ModManager.MSC && ApexUtils.ActiveMods.Contains("ShinyKelp.ScavengerTweaks"))
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

        public void UpdateModBindings()
        {
            UnityEngine.Debug.Log("UPDATING MOD BINDINGS.");
            HashSet<string> activeMods = ApexUtils.ActiveMods;

            if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                UnityEngine.Debug.Log("CREATING BINDINGS FOR MARBLE PACK.");
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
                AddConfigOption(ModExtraConfigs, "SurfaceSwimmerExtras", 5, "Surface Swimmer (/10)");
                AddConfigOption(ModRepConfigs, "BounceBallChance", 10, "Snail > Bouncing Ball");
                AddConfigOption(ModExtraConfigs, "BounceBallExtras", 10, "Bouncing Ball (/10)");
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

            if (activeMods.Contains("Croken.Mimic-Long-Legs"))
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
            if (!LabelsMap.ContainsKey(dictionary[name]))
            {
                LabelsMap.Add(dictionary[name], labelDesc);
            }
        }

        public int GetOptionConfigValue(string configName)
        {
            Configurable<int> config = GetOptionConfig(configName);
            if (config == null)
                return 0;
            else
                return config.Value;

        }

        public Configurable<int> GetOptionConfig(string configName)
        {
            if (VanillaRepConfigs.ContainsKey(configName))
                return VanillaRepConfigs[configName];
            else if (VanillaExtraConfigs.ContainsKey(configName))
                return VanillaExtraConfigs[configName];
            else if (DLCRepConfigs.ContainsKey(configName))
                return DLCRepConfigs[configName];
            else if (DLCExtraConfigs.ContainsKey(configName))
                return DLCExtraConfigs[configName];
            else if (WatcherRepConfigs.ContainsKey(configName))
                return WatcherRepConfigs[configName];
            else if (WatcherExtraConfigs.ContainsKey(configName))
                return WatcherExtraConfigs[configName];
            else if (ModRepConfigs.ContainsKey(configName))
                return ModRepConfigs[configName];
            else if (ModExtraConfigs.ContainsKey(configName))
                return ModExtraConfigs[configName];
            else
                UnityEngine.Debug.Log("--AUYS-- Warning, key not found in options dictionary: " + configName);
            return null;
        }
    }
}
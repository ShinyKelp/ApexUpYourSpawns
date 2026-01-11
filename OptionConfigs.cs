
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
        public readonly Dictionary<string, Configurable<int>> M4rbleRepConfigs = new Dictionary<string, Configurable<int>>();
        public readonly Dictionary<string, Configurable<int>> M4rbleExtraConfigs = new Dictionary<string, Configurable<int>>();


        public readonly Dictionary<Configurable<int>, string> LabelsMap = new Dictionary<Configurable<int>, string>();

        public static OptionConfigs Instance { get; private set; } = null;

        public OptionConfigs(ConfigHolder config)
        {
            Instance = this;
            this.config = config;
        }

        public void UpdateBindings()
        {
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
            HashSet<string> activeMods = ApexUtils.ActiveMods;

            if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                AddConfigOption(M4rbleRepConfigs, "SporantulaChance", 4, "Small Insects > Sporantula (Inv)");
                AddConfigOption(M4rbleExtraConfigs, "SporantulaExtras", 25, "Sporantulas (/10)");
                AddConfigOption(M4rbleRepConfigs, "ScutigeraChance", 15, "Centipede > Scutigera");
                AddConfigOption(M4rbleExtraConfigs, "ScutigeraExtras", 0, "Scutigeras (/10)");
                AddConfigOption(M4rbleRepConfigs, "RedRedHorrorCentiChance", 10, "Red Centipede > Red Horror Centi");
                AddConfigOption(M4rbleRepConfigs, "WingRedHorrorCentiChance", 4, "Centiwing > Red Horror Centi");
                AddConfigOption(M4rbleRepConfigs, "WaterSpitterChance", 10, "Aquatic Lizards > Water Spitter");
                AddConfigOption(M4rbleExtraConfigs, "WaterSpitterExtras", 0, "Water Spitters (/10)");
                AddConfigOption(M4rbleRepConfigs, "FatFireFlyChance", 10, "Vultures > Fat Firefly");
                AddConfigOption(M4rbleRepConfigs, "SurfaceSwimmerChance", 15, "EggBug > Surface Swimmer");
                AddConfigOption(M4rbleExtraConfigs, "SurfaceSwimmerExtras", 5, "Surface Swimmer (/10)");

                AddConfigOption(M4rbleRepConfigs, "TintedBeetleChance", 20, "EggBug > Tinted Beetle");
                AddConfigOption(M4rbleExtraConfigs, "TintedBeetleExtras", 30, "Tinted Beetle (/10)");

                AddConfigOption(M4rbleRepConfigs, "ChipChopChance", 10, "EggBug > Chip Chop");
                AddConfigOption(M4rbleExtraConfigs, "ChipChopExtras", 25, "Chip Chop (/10)");

                AddConfigOption(M4rbleRepConfigs, "MamaBugChance", 5, "EggBug > MamaBug");
                AddConfigOption(M4rbleExtraConfigs, "MamaBugExtras", 2, "MamaBug (/10)");


                AddConfigOption(M4rbleRepConfigs, "BounceBallChance", 10, "Snail > Bouncing Ball");
                AddConfigOption(M4rbleExtraConfigs, "BounceBallExtras", 10, "Bouncing Ball (/10)");
                AddConfigOption(M4rbleRepConfigs, "CritterHoverflyChance", 7, "Critters > Hoverfly (Inv)");
                AddConfigOption(M4rbleExtraConfigs, "HoverflyExtras", 15, "Hoverfly (/10)");

                AddConfigOption(M4rbleRepConfigs, "TailflyChance", 12, "Cicada > Tailfly");
                AddConfigOption(M4rbleExtraConfigs, "TailflyExtras", 15, "Tailfly (/10)");
                

                AddConfigOption(M4rbleRepConfigs, "NoodleEaterChance", 10, "Noodlefly > Noodle Eater (Inv)");
                AddConfigOption(M4rbleExtraConfigs, "NoodleEaterExtras", 6, "Noodle Eater (/10)");
                AddConfigOption(M4rbleRepConfigs, "ThornbugChance", 20, "Eggbug > Thornbug (Inv)");
                AddConfigOption(M4rbleExtraConfigs, "ThornbugExtras", 4, "Thornbug (/10)");
                AddConfigOption(M4rbleRepConfigs, "MiniLeviathanChance", 25, "Leviathan > Mini Leviathan (Inv)");
                AddConfigOption(M4rbleExtraConfigs, "MiniLeviathanExtras", 3, "Mini Leviathan", false);
                AddConfigOption(M4rbleRepConfigs, "PolliwogChance", 10, "Salamander > Polliwog");
                AddConfigOption(M4rbleExtraConfigs, "PolliwogExtras", 8, "Polliwog (/10)");
                AddConfigOption(M4rbleRepConfigs, "HunterSeekerCyanChance", 6, "Cyan Liz > Hunter Seeker");
                AddConfigOption(M4rbleRepConfigs, "HunterSeekerWhiteChance", 6, "White Liz > Hunter Seeker");
                AddConfigOption(M4rbleExtraConfigs, "HunterSeekerExtras", 2, "Hunter Seeker (/10)");
                AddConfigOption(M4rbleRepConfigs, "SilverLizChance", 15, "Grounded Lizards > Silver Liz");
                AddConfigOption(M4rbleExtraConfigs, "SilverLizExtras", 2, "Silver Lizard (/10)");
                AddConfigOption(M4rbleRepConfigs, "VultureEchoLeviChance", 10, "Vultures > Echo Leviathan (Den)");
                AddConfigOption(M4rbleExtraConfigs, "EchoLeviExtras", 0, "Echo Leviathans (/10)");
                AddConfigOption(M4rbleRepConfigs, "BlizzorChance", 7, "Miros Bird > Blizzor");
                AddConfigOption(M4rbleRepConfigs, "SalamanderSalamoleChance", 5, "Salamander > Mole Salamander");
                AddConfigOption(M4rbleRepConfigs, "BlackSalamoleChance", 5, "Black liz > Mole Salamander");
                AddConfigOption(M4rbleExtraConfigs, "BlizzorExtras", 2, "Blizzor", false);
                AddConfigOption(M4rbleExtraConfigs, "SalamoleExtras", 10, "Mole Salamander (/10)");

                AddConfigOption(M4rbleRepConfigs, "BigrubChance", 10, "Grappleworm > Big Grappleworm");
                AddConfigOption(M4rbleRepConfigs, "WaterBlobChance", 10, "Bubble Fruit > Water Blob");
                AddConfigOption(M4rbleRepConfigs, "HazerMomChance", 10, "Hazer > Hazer Mom");
                AddConfigOption(M4rbleRepConfigs, "SeedBatChance", 10, "Batfly > Seedbat");
                AddConfigOption(M4rbleRepConfigs, "MiniEchoLeviChance", 50, "Echo Levi > Mini Echo Levi (Inv)");
                AddConfigOption(M4rbleExtraConfigs, "MiniEchoLeviExtras", 20, "Mini Echo Levi (/10)");

                AddConfigOption(M4rbleRepConfigs, "AlphaOrangeChance", 25, "Orange > Alpha Orange Liz (Den)");
                AddConfigOption(M4rbleRepConfigs, "ScavengerSentinelChance", 6, "Scavenger > Sentinel");
                AddConfigOption(M4rbleRepConfigs, "MiniScutigeraChance", 16, "Small Cent > Mini Scutigera");

                AddConfigOption(M4rbleRepConfigs, "KillerpillarChance", 7, "Basic Lizards > Killerpillar");
                AddConfigOption(M4rbleRepConfigs, "GlowpillarChance", 14, "Dark Lizards > Glowpillar");

                AddConfigOption(M4rbleExtraConfigs, "KillerpillarExtras", 15, "Killerpillars (/10)");
                AddConfigOption(M4rbleExtraConfigs, "GlowpillarExtras", 15, "Glowpillars (/10)");

                AddConfigOption(M4rbleRepConfigs, "SparkEyeChance", 8, "Miros Bird > Spark Eye");
                
                
                AddConfigOption(M4rbleRepConfigs, "DivingBeetleChance", 10, "Jetfish > Diving Beetle (Inv)");
                AddConfigOption(M4rbleExtraConfigs, "DivingBeetleExtras", 4, "Diving Beetles (/10)");

                AddConfigOption(M4rbleRepConfigs, "CommonEelChance", 10, "Eel Lizard > Common Eel");
                AddConfigOption(M4rbleExtraConfigs, "CommonEelExtras", 4, "Common Eels (/10)");

                AddConfigOption(M4rbleRepConfigs, "MiniBlackLeechChance", 20, "Red Leech > Mini Black Leech");
                AddConfigOption(M4rbleExtraConfigs, "MiniBlackLeechExtras", 4, "Mini Black Leeches", false);

                AddConfigOption(M4rbleRepConfigs, "DentureChance", 20, "Pole Plant > Denture");

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

            if (activeMods.Contains("com.rainworldgame.shroudedassembly.plugin"))
            {
                AddConfigOption(ModRepConfigs, "GeckoChance", 15, "Pink Lizard > Gecko");
                AddConfigOption(ModExtraConfigs, "GeckoExtras", 3, "Geckos (/10)");
                AddConfigOption(ModRepConfigs, "MaracaSpiderChance", 9, "Spitter Spider > Maraca Spider");
                AddConfigOption(ModExtraConfigs, "MaracaSpiderExtras", 0, "Maraca Spiders (/10)");
                AddConfigOption(ModRepConfigs, "BabyCroakerChance", 6, "Cicada > Baby Croaker (Inv)");
                AddConfigOption(ModExtraConfigs, "BabyCroakerExtras", 30, "Baby Croakers (/10)");
                if (activeMods.Contains("mosquitoes"))
                    AddConfigOption(ModRepConfigs, "MosquitoBabyCroakerChance", 20, "Mosquitoes > Baby Croaker (Inv)");
            }

            if (activeMods.Contains("sequoia.luminouscode"))
            {
                AddConfigOption(ModRepConfigs, "TeuthicadaChance", 6, "Cicada > Teuthicada");
                AddConfigOption(ModExtraConfigs, "TeuthicadaExtras", 0, "Teuthicadas (/10)");
            }
            if (activeMods.Contains("sequoia.parascutigera-creature"))
            {
                AddConfigOption(ModRepConfigs, "ParascutigeraChance", 6, "Scutigera > Parascutigera");
            }
            if (activeMods.Contains("mosquitoes"))
            {
                AddConfigOption(ModRepConfigs, "MosquitoChance", 5, "Noodle Fly > Mosquito");
                AddConfigOption(ModRepConfigs, "ExplodingMosquitoChance", 16, "Mosquito > Exploding Mosquito");
                AddConfigOption(ModRepConfigs, "AngryMosquitoChance", 16, "Mosquito > Angry Mosquito");
                AddConfigOption(ModExtraConfigs, "MosquitoExtras", 25, "Mosquitoes (/10)");
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
            else if (M4rbleRepConfigs.ContainsKey(configName))
                return M4rbleRepConfigs[configName];
            else if(M4rbleExtraConfigs.ContainsKey(configName))
                return M4rbleExtraConfigs[configName];
            else
                UnityEngine.Debug.Log("--AUYS-- Warning, key not found in options dictionary: " + configName);
            return null;
        }
    }
}
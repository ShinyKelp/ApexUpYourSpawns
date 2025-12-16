namespace ApexUpYourSpawns
{
    public static class ConfigShortcuts
    {
        #region Vanilla Creature Variables

        // Vanilla - Replacements
        public static float RedLizardChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("RedLizardChance") / 100; } }
        public static float LargeCentipedeChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("LargeCentipedeChance") / 100; } }
        public static float RedCentipedeChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("RedCentipedeChance") / 100; } }
        public static float SpitterSpiderChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("SpitterSpiderChance") / 100; } }
        public static float KingVultureChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("KingVultureChance") / 100; } }
        public static float BrotherLongLegsChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("BrotherLongLegsChance") / 100; } }
        public static float DaddyLongLegsChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("DaddyLongLegsChance") / 100; } }
        public static float CicadaCentiwingChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("CicadaCentiwingChance") / 100; } }
        public static float LeechLizardChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("LeechLizardChance") / 100; } }
        public static float CyanLizChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("CyanLizChance") / 100; } }
        public static float TubeWormBigSpiderChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("TubeWormSpiderInv") / 100; } }
        public static float JetfishSalamanderChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("JetfishSalamanderChance") / 100; } }
        public static float CicadaNoodleflyChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("CicadaNoodleFlyChance") / 100; } }
        public static float MonsterKelpChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("MonsterKelpChance") / 100; } }

        // Vanilla - Extras
        public static int ExtraGreens { get { return OptionConfigs.Instance.GetOptionConfigValue("GreenLizExtras"); } }
        public static int ExtraPinks { get { return OptionConfigs.Instance.GetOptionConfigValue("PinkLizExtras"); } }
        public static int ExtraBlues { get { return OptionConfigs.Instance.GetOptionConfigValue("BlueLizExtras"); } }
        public static int ExtraWhites { get { return OptionConfigs.Instance.GetOptionConfigValue("WhiteLizExtras"); } }
        public static int ExtraBlacks { get { return OptionConfigs.Instance.GetOptionConfigValue("BlackLizExtras"); } }
        public static int ExtraYellows { get { return OptionConfigs.Instance.GetOptionConfigValue("YellowLizExtras"); } }
        public static int ExtraSals { get { return OptionConfigs.Instance.GetOptionConfigValue("SalExtras"); } }
        public static int ExtraCyans { get { return OptionConfigs.Instance.GetOptionConfigValue("CyanLizExtras"); } }
        public static int ExtraSpiders { get { return OptionConfigs.Instance.GetOptionConfigValue("BigSpiderExtras"); } }
        public static int ExtraVultures { get { return OptionConfigs.Instance.GetOptionConfigValue("VultureExtras"); } }
        public static int ExtraScavengers { get { return OptionConfigs.Instance.GetOptionConfigValue("ScavengerExtras"); } }
        public static int ExtraSmallCents { get { return OptionConfigs.Instance.GetOptionConfigValue("SmallCentExtras"); } }
        public static int ExtraEggbugs { get { return OptionConfigs.Instance.GetOptionConfigValue("EggbugExtras"); } }
        public static int ExtraCicadas { get { return OptionConfigs.Instance.GetOptionConfigValue("CicadaExtras"); } }
        public static int ExtraSnails { get { return OptionConfigs.Instance.GetOptionConfigValue("SnailExtras"); } }
        public static int ExtraJetfish { get { return OptionConfigs.Instance.GetOptionConfigValue("JetfishExtras"); } }
        public static int ExtraLMice { get { return OptionConfigs.Instance.GetOptionConfigValue("LmiceExtras"); } }
        public static int ExtraCentipedes { get { return OptionConfigs.Instance.GetOptionConfigValue("CentipedeExtras"); } }
        public static int ExtraCentiwings { get { return OptionConfigs.Instance.GetOptionConfigValue("CentiWingExtras"); } }
        public static int ExtraPrecycleSals { get { return OptionConfigs.Instance.GetOptionConfigValue("PrecycleCreatureExtras"); } }
        public static int ExtraDropwigs { get { return OptionConfigs.Instance.GetOptionConfigValue("DropwigExtras"); } }
        public static int ExtraMiros { get { return OptionConfigs.Instance.GetOptionConfigValue("MirosExtras"); } }
        public static int ExtraSmallSpiders { get { return OptionConfigs.Instance.GetOptionConfigValue("SpiderExtras"); } }
        public static int ExtraLeeches { get { return OptionConfigs.Instance.GetOptionConfigValue("LeechExtras"); } }
        public static int ExtraTubeworms { get { return OptionConfigs.Instance.GetOptionConfigValue("TubeWormExtras"); } }
        public static int ExtraKelp { get { return OptionConfigs.Instance.GetOptionConfigValue("KelpExtras"); } }
        public static int ExtraLeviathans { get { return OptionConfigs.Instance.GetOptionConfigValue("LeviathanExtras"); } }
        public static int ExtraNightCreatures { get { return OptionConfigs.Instance.GetOptionConfigValue("NightCreatureExtras"); } }
        public static int ExtraDeer { get { return OptionConfigs.Instance.GetOptionConfigValue("DeerExtras"); } }

        // MSC - Replacements
        public static float SeaLeechAquapedeChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("SeaLeechAquapedeChance") / 100; } }
        public static float MirosVultureChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("MirosVultureChance") / 100; } }
        public static float EliteScavengerChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("EliteScavengerChance") / 100; } }
        public static float TerrorLongLegsChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("TerrorLongLegsChance") / 100; } }
        public static float YeekLizardChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("YeekLizardChance") / 100; } }
        public static float AquapedeChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("JetfishAquapedeChance") / 100; } }
        public static float CaramelLizChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("CaramelLizChance") / 100; } }
        public static float StrawberryLizChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("StrawberryLizChance") / 100; } }
        public static float EelLizChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("EelLizChance") / 100; } }
        public static float JungleLeechChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("JungleLeechChance") / 100; } }
        public static float MotherSpiderChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("MotherSpiderChance") / 100; } }
        public static float TrainLizardChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("TrainLizardChance") / 100; } }
        public static float FireBugChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("FireBugChance") / 100; } }

        // MSC - Extras
        public static int ExtraCaramels { get { return OptionConfigs.Instance.GetOptionConfigValue("CaramelLizExtras"); } }
        public static int ExtraEellizs { get { return OptionConfigs.Instance.GetOptionConfigValue("EelLizExtras"); } }
        public static int ExtraZoops { get { return OptionConfigs.Instance.GetOptionConfigValue("ZoopLizExtras"); } }
        public static int ExtraYeeks { get { return OptionConfigs.Instance.GetOptionConfigValue("YeekExtras"); } }
        public static int ExtraAquapedes { get { return OptionConfigs.Instance.GetOptionConfigValue("AquapedeExtras"); } }

        // Watcher - Replacements
        public static float ScavengerDiscipleChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("ScavengerDiscipleChance") / 100; } }
        public static float ScavengerTemplarChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("ScavengerTemplarChance") / 100; } }
        public static float BlizzardLizardChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("BlizzardLizardChance") / 100; } }
        public static float MirosLoachChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("MirosLoachChance") / 100; } }
        public static float DeerLoachInvChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("DeerLoachInvChance") / 100; } }
        public static float LoachMirosChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("LoachMirosChance") / 100; } }
        public static float RotLoachChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("RotLoachChance") / 100; } }
        public static float VultureBigMothChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("VultureBigMothChance") / 100; } }
        public static float BigMothVultureChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("BigMothVultureChance") / 100; } }
        public static float CicadaSmallMothChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("CicadaSmallMothChance") / 100; } }
        public static float SmallMothCicadaChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("CicadaSmallMothChance") / 100; } }
        public static float SmallMothNoodleflyChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("SmallMothNoodleflyChance") / 100; } }
        public static float SmallMothCentiwingChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("SmallMothCentiwingChance") / 100; } }
        public static float DeerSkywhaleChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("DeerSkywhaleChance") / 100; } }
        public static float SnailBarnacleChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("SnailBarnacleChance") / 100; } }
        public static float BarnacleSnailChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("BarnacleSnailChance") / 100; } }
        public static float BlackBasiliskLizChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("BlackBasiliskLizChance") / 100; } }
        public static float GroundIndigoLizChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("GroundIndigoLizChance") / 100; } }
        public static float DrillCrabMirosChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("DrillCrabMirosChance") / 100; } }
        public static float MirosDrillCrabChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("MirosDrillCrabChance") / 100; } }
        public static float DrillCrabLoachChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("DrillCrabLoachChance") / 100; } }
        public static float LoachDrillCrabChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("LoachDrillCrabChance") / 100; } }
        public static float DeerDrillCrabInvChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("DeerDrillCrabInvChance") / 100; } }
        public static float LeechFrogChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("LeechFrogChance") / 100; } }
        public static float MouseRatChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("MouseRatChance") / 100; } }

        // Watcher - Extras
        public static int LoachExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("LoachExtras"); } }
        public static int BigMothExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("BigMothExtras"); } }
        public static int SmallMothExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("SmallMothExtras"); } }
        public static int SkywhaleExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("SkywhaleExtras"); } }
        public static int BasiliskLizExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("BasiliskLizExtras"); } }
        public static int IndigoLizExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("IndigoLizExtras"); } }
        public static int BarnacleExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("BarnacleExtras"); } }
        public static int DrillCrabExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("DrillCrabExtras"); } }
        public static int FrogExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("FrogExtras"); } }
        public static int RatExtras { get { return OptionConfigs.Instance.GetOptionConfigValue("RatExtras"); } }

        // Mods
        public static float InspectorChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("InspectorChance") / 100; } }
        public static float RyanLizardChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("RyanLizardChance") / 100; } }


        //Non-spawner variables
        public static float GrubSandGrubChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("GrubSandGrubChance") / 100; } }
        public static float PopcornSandWormTrapChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("PopcornSandWormTrapChance") / 100; } }
        public static float HazerTardigradeChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("HazerTardigradeChance") / 100; } }
        public static float PearlFireSpriteChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("PearlFireSpriteChance") / 100; } }
        public static float KingScavengerChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("KingScavengerChance") / 100; } }
        public static float GiantJellyfishChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("GiantJellyfishChance") / 100; } }
        public static float StowawayChance { get { return (float)OptionConfigs.Instance.GetOptionConfigValue("StowawayChance") / 100; } }

        #endregion
    }
}
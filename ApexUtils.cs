namespace ApexUpYourSpawns
{
    using System.Collections.Generic;

    //Global utility variables.
    public static class ApexUtils
    {
        //Development only
        public static bool DebugLogs { get; private set; } = false;

        //User options
        public static bool BalancedSpawns = false;

        public static bool FillLineages = false;

        public static bool ForceFreshSpawns = false;

        //Enabled mods

        public static HashSet<string> ActiveMods = new HashSet<string>();

        public static bool HasSharedDLC = false;

        public static bool HasAngryInspectors = false;

        public static bool HasLizardVariants = false;

        //Other
        public static CreatureTemplate.Type[] HorizontalSpawns;

        public static bool HasUpdatedRefs = false;

        //Constants
        public static readonly HashSet<string> BannedRooms = new()
        {
            "SB_GOR01",
            "SB_H03",
            "SB_H02",
            "SB_E04",
            "SB_C06"
        };

        public static readonly HashSet<string> SupportedMods = new HashSet<string>(
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
                "Croken.Mimic-Long-Legs",
                "ShinyKelp.AlbinoKings",
                "bebra.gregtech_lizard",
                "bry.bubbleweavers",
                "lb-fgf-m4r-ik.modpack",
                "sequoia.luminouscode",
                "com.rainworldgame.shroudedassembly.plugin",
                "sequoia.parascutigera-creature",
                "mosquitoes"
            }
        );
    }
}
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace ApexUpYourSpawns
{
    public class ApexUpYourSpawnsOptions : OptionInterface
    {
        private readonly ManualLogSource Logger;

        public ApexUpYourSpawnsOptions(ApexUpYourSpawnsMod modInstance, ManualLogSource loggerSource)
        {
            Logger = loggerSource;
            mod = modInstance;

            redLizardChance = this.config.Bind<int>("RedLizardChance", 8, new ConfigAcceptableRange<int>(0, 100));
            trainLizardChance = this.config.Bind<int>("TrainLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            largeCentipedeChance = this.config.Bind<int>("LargeCentipedeChance", 0, new ConfigAcceptableRange<int>(0, 100));
            redCentipedeChance = this.config.Bind<int>("RedCentipedeChance", 8, new ConfigAcceptableRange<int>(0, 100));
            mirosVultureChance = this.config.Bind<int>("MirosVultureChance", 15, new ConfigAcceptableRange<int>(0, 100));
            spitterSpiderChance = this.config.Bind<int>("SpitterSpiderChance", 15, new ConfigAcceptableRange<int>(0, 100));
            noodleFlyChance = this.config.Bind<int>("NoodleFlyChance", 12, new ConfigAcceptableRange<int>(0, 100));
            fireBugChance = this.config.Bind<int>("FireBugChance", 30, new ConfigAcceptableRange<int>(0, 100));
            eliteScavengerChance = this.config.Bind<int>("EliteScavengerChance", 12, new ConfigAcceptableRange<int>(0, 100));
            brotherLongLegsChance = this.config.Bind<int>("BrotherLongLegsChance", 4, new ConfigAcceptableRange<int>(0, 100));
            daddyLongLegsChance = this.config.Bind<int>("DaddyLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
            terrorLongLegsChance = this.config.Bind<int>("MotherLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
            giantJellyfishChance = this.config.Bind<int>("GiantJellyfishChance", 10, new ConfigAcceptableRange<int>(0, 100));
            leechLizardChance = this.config.Bind<int>("LeechLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            aquapedeChance = this.config.Bind<int>("AquapedeChance", 20, new ConfigAcceptableRange<int>(0, 100));
            yeekLizardChance = this.config.Bind<int>("YeekLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            caramelLizChance = this.config.Bind<int>("CaramelLizChance", 15, new ConfigAcceptableRange<int>(0, 100));
            strawberryLizChance = this.config.Bind<int>("StrawberryLizChance", 5, new ConfigAcceptableRange<int>(0, 100));
            cyanLizChance = this.config.Bind<int>("CyanLizChance", 0, new ConfigAcceptableRange<int>(0, 100));
            jungleLeechChance = this.config.Bind<int>("JungleLeechChance", 0, new ConfigAcceptableRange<int>(0, 100));
            motherSpiderChance = this.config.Bind<int>("MotherSpiderChance", 10, new ConfigAcceptableRange<int>(0, 100));
            stowawayChance = this.config.Bind<int>("StowawayChance", 4, new ConfigAcceptableRange<int>(0, 100));
            sporantulaChance = this.config.Bind<int>("SporantulaChance", 4, new ConfigAcceptableRange<int>(0, 100));

            yellowLizExtras = this.config.Bind<int>("ExtraYellowLizs", 2, new ConfigAcceptableRange<int>(0, 10));
            cyanLizExtras = this.config.Bind<int>("ExtraCyanLizs", 0, new ConfigAcceptableRange<int>(0, 10));
            genericLizExtras = this.config.Bind<int>("ExtraLizards", 0, new ConfigAcceptableRange<int>(0, 10));
            waterLizExtras = this.config.Bind<int>("ExtraWaterLizs", 0, new ConfigAcceptableRange<int>(0, 10));
            precycleSalExtras = this.config.Bind<int>("ExtraPrecycleSalamanders", 2, new ConfigAcceptableRange<int>(0, 10));
            scavengerExtras = this.config.Bind<int>("ExtraScavengers", 4, new ConfigAcceptableRange<int>(0, 100));
            vultureExtras = this.config.Bind<int>("ExtraVultures", 1, new ConfigAcceptableRange<int>(0, 10));
            vultureKingExtras = this.config.Bind<int>("ExtraKingVultures", 1, new ConfigAcceptableRange<int>(0, 10));
            centipedeExtras = this.config.Bind<int>("ExtraKCentipedes", 0, new ConfigAcceptableRange<int>(0, 10));
            centiWingExtras = this.config.Bind<int>("ExtraCentiwings", 0, new ConfigAcceptableRange<int>(0, 10));
            aquapedeExtras = this.config.Bind<int>("ExtraAquaPedes", 0, new ConfigAcceptableRange<int>(0, 10));
            bigSpiderExtras = this.config.Bind<int>("ExtraBigSpiders", 2, new ConfigAcceptableRange<int>(0, 10));
            dropwigExtras = this.config.Bind<int>("ExtraDropwigs", 1, new ConfigAcceptableRange<int>(0, 10));
            eggbugExtras = this.config.Bind<int>("ExtraEggbugs", 1, new ConfigAcceptableRange<int>(0, 10));
            mirosExtras = this.config.Bind<int>("ExtraMirosBirds", 4, new ConfigAcceptableRange<int>(0, 10));
            spiderExtras = this.config.Bind<int>("ExtraSpiders", 0, new ConfigAcceptableRange<int>(0, 10));
            leechExtras = this.config.Bind<int>("ExtraLeeches", 0, new ConfigAcceptableRange<int>(0, 10));
            kelpExtras = this.config.Bind<int>("ExtraKelps", 1, new ConfigAcceptableRange<int>(0, 10));
            leviathanExtras = this.config.Bind<int>("ExtraLeviathans", 1, new ConfigAcceptableRange<int>(0, 10));
            sporantulaExtras = this.config.Bind<int>("ExtraSporantulas", 3, new ConfigAcceptableRange<int>(0, 10));
        }


        public readonly Configurable<int> redLizardChance;
        public readonly Configurable<int> redCentipedeChance;
        public readonly Configurable<int> largeCentipedeChance;
        public readonly Configurable<int> mirosVultureChance;
        public readonly Configurable<int> spitterSpiderChance;
        public readonly Configurable<int> trainLizardChance;
        public readonly Configurable<int> noodleFlyChance;
        public readonly Configurable<int> fireBugChance;
        public readonly Configurable<int> eliteScavengerChance;
        public readonly Configurable<int> brotherLongLegsChance;
        public readonly Configurable<int> daddyLongLegsChance;
        public readonly Configurable<int> terrorLongLegsChance;
        public readonly Configurable<int> giantJellyfishChance;
        public readonly Configurable<int> leechLizardChance;
        public readonly Configurable<int> aquapedeChance;
        public readonly Configurable<int> yeekLizardChance;
        public readonly Configurable<int> caramelLizChance;
        public readonly Configurable<int> strawberryLizChance;
        public readonly Configurable<int> cyanLizChance;
        public readonly Configurable<int> jungleLeechChance;
        public readonly Configurable<int> motherSpiderChance;
        public readonly Configurable<int> stowawayChance;
        public readonly Configurable<int> sporantulaChance;

        public readonly Configurable<int> yellowLizExtras;
        public readonly Configurable<int> cyanLizExtras;
        public readonly Configurable<int> genericLizExtras;
        public readonly Configurable<int> waterLizExtras;
        public readonly Configurable<int> precycleSalExtras;
        public readonly Configurable<int> scavengerExtras;
        public readonly Configurable<int> vultureExtras;
        public readonly Configurable<int> vultureKingExtras;
        public readonly Configurable<int> centipedeExtras;
        public readonly Configurable<int> centiWingExtras;
        public readonly Configurable<int> aquapedeExtras;
        public readonly Configurable<int> bigSpiderExtras;
        public readonly Configurable<int> dropwigExtras;
        public readonly Configurable<int> eggbugExtras;
        public readonly Configurable<int> mirosExtras;
        public readonly Configurable<int> spiderExtras;
        public readonly Configurable<int> leechExtras;
        public readonly Configurable<int> kelpExtras;
        public readonly Configurable<int> leviathanExtras;
        public readonly Configurable<int> sporantulaExtras;


        private OpSimpleButton defaultsSimpleButton;
        private OpSimpleButton nullsSimpleButton;

        private OpScrollBox scrollBox;

        private UIelement[] UIArrPlayerOptions, UITitles, UIDependentOptions;

        private ApexUpYourSpawnsMod mod;

        public override void Initialize()
        {
            float sbs = 830f;   //Size of scrollbox.
            var opTab = new OpTab(this, "Options");
            this.Tabs = new[]
            {
                opTab
            };

            defaultsSimpleButton = new OpSimpleButton(new Vector2(50f, 10f), new Vector2(60, 30), "Defaults");
            nullsSimpleButton = new OpSimpleButton(new Vector2(250f, 10f), new Vector2(60, 30), "Nulls");

            //defaultsSimpleButton.OnClick += setDefaults(); //Currently bugged. Dual-Iron's dlls won't work either.

            UITitles = new UIelement[]
            {
                new OpLabel(10f, 570f, "Options", true),
                new OpLabel(-2f, 543f, "Apex Replacements (% chance)", true),
                new OpLabel(325f, 543f, "Extra spawns (up to, per den)", true),

                defaultsSimpleButton,
            };

            UIArrPlayerOptions = new UIelement[]
            {
                new OpLabel(80f, sbs-30f, "Lizard > Red lizard"),
                new OpUpdown(redLizardChance, new Vector2(10f, sbs-35f), 60f),
                new OpLabel(80f, sbs-65f, "Red lizard > Train lizard"),
                new OpUpdown(trainLizardChance, new Vector2(10f, sbs-70f), 60f),
                new OpLabel(80f, sbs-100f, "Small Cent > Large Centipede"),
                new OpUpdown(largeCentipedeChance, new Vector2(10f, sbs-105f), 60f),
                new OpLabel(80f, sbs-135f, "Large Cent > Red centipede"),
                new OpUpdown(redCentipedeChance, new Vector2(10f, sbs-140f), 60f),
                new OpLabel(80f, sbs-170f, "Vulture > Miros Vulture"),
                new OpUpdown(mirosVultureChance, new Vector2(10f, sbs-175f), 60f),
                new OpLabel(80f, sbs-205f, "Scavenger > Elite Scavenger"),
                new OpUpdown(eliteScavengerChance, new Vector2(10f, sbs-210f), 60f),
                new OpLabel(80f, sbs-240f, "Eggbug > Firebug"),
                new OpUpdown(fireBugChance, new Vector2(10f, sbs-245f), 60f),
                new OpLabel(80f, sbs-275f, "Cicada > Noodlefly"),
                new OpUpdown(noodleFlyChance, new Vector2(10f, sbs-280f), 60f),
                new OpLabel(80f, sbs-310f, "L.Mice/Snail > BrotherLongLegs"),
                new OpUpdown(brotherLongLegsChance, new Vector2(10f, sbs-315f), 60f),
                new OpLabel(80f, sbs-345f, "BrotherLongLegs > DaddyLongLegs"),
                new OpUpdown(daddyLongLegsChance, new Vector2(10f, sbs-350f), 60f),
                new OpLabel(80f, sbs-380f, "DaddyLongLegs > MotherLongLegs"),
                new OpUpdown(terrorLongLegsChance, new Vector2(10f, sbs-385f), 60f),
                new OpLabel(80f, sbs-415f, "Jellyfish > Giant Jellyfish"),
                new OpUpdown(giantJellyfishChance, new Vector2(10f, sbs-420f), 60f),
                new OpLabel(80f, sbs-450f, "Leeches > Water lizard"),
                new OpUpdown(leechLizardChance, new Vector2(10f, sbs-455f), 60f),
                new OpLabel(80f, sbs-485f, "Yeek > Caramel/Strawberry liz"),
                new OpUpdown(yeekLizardChance, new Vector2(10f, sbs-490f), 60f),
                new OpLabel(80f, sbs-520f, "Jetfish > Aquapede"),
                new OpUpdown(aquapedeChance, new Vector2(10f, sbs-525f), 60f),
                new OpLabel(80f, sbs-555f, "Pink > Strawberry Lizard"),
                new OpUpdown(strawberryLizChance, new Vector2(10f, sbs-560f), 60f),
                new OpLabel(80f, sbs-590f, "Green > Caramel Lizard"),
                new OpUpdown(caramelLizChance, new Vector2(10f, sbs-595f), 60f),
                new OpLabel(80f, sbs-625f, "Blue > Cyan Lizard"),
                new OpUpdown(cyanLizChance, new Vector2(10f, sbs-630f), 60f),
                new OpLabel(80f, sbs-660f, "Big Spider > Spitter Spider"),
                new OpUpdown(spitterSpiderChance, new Vector2(10f, sbs-665f), 60f),
                new OpLabel(80f, sbs-695f, "Spiders > Mother Spider"),
                new OpUpdown(motherSpiderChance, new Vector2(10f, sbs-700f), 60f),
                new OpLabel(80f, sbs-730f, "Leech > Jungle Leech"),
                new OpUpdown(jungleLeechChance, new Vector2(10f, sbs-735f), 60f),
                new OpLabel(80f, sbs-765f, "Ceiling Fruits > Stowawaybug trap"),
                new OpUpdown(stowawayChance, new Vector2(10f, sbs-770f), 60f),
                
                

                new OpLabel(400f, sbs-30f, "Lizards"),
                new OpUpdown(genericLizExtras, new Vector2(330f, sbs-35f), 60f),
                new OpLabel(400f, sbs-65f, "Yellow lizards"),
                new OpUpdown(yellowLizExtras, new Vector2(330f, sbs-70f), 60f),
                new OpLabel(400f, sbs-100f, "Cyan lizards"),
                new OpUpdown(cyanLizExtras, new Vector2(330f, sbs-105f), 60f),
                new OpLabel(400f, sbs-135f, "Water lizards"),
                new OpUpdown(waterLizExtras, new Vector2(330f, sbs-140f), 60f),
                new OpLabel(400f, sbs-170f, "Shelter failure spawns"),
                new OpUpdown(precycleSalExtras, new Vector2(330f, sbs-175f), 60f),
                new OpLabel(400f, sbs-205f, "Scavengers"),
                new OpUpdown(scavengerExtras, new Vector2(330f, sbs-210f), 60f),
                new OpLabel(400f, sbs-240f, "Vultures"),
                new OpUpdown(vultureExtras, new Vector2(330f, sbs-245f), 60f),
                new OpLabel(400f, sbs-275f, "King vultures"),
                new OpUpdown(vultureKingExtras, new Vector2(330f, sbs-280f), 60f),
                new OpLabel(400f, sbs-310f, "Centipedes"),
                new OpUpdown(centipedeExtras, new Vector2(330f, sbs-315f), 60f),
                new OpLabel(400f, sbs-345f, "Centiwings"),
                new OpUpdown(centiWingExtras, new Vector2(330f, sbs-350f), 60f),
                new OpLabel(400f, sbs-380f, "Aquapedes"),
                new OpUpdown(aquapedeExtras, new Vector2(330f, sbs-385f), 60f),
                new OpLabel(400f, sbs-415f, "Big Spiders"),
                new OpUpdown(bigSpiderExtras, new Vector2(330f, sbs-420f), 60f),
                new OpLabel(400f, sbs-450f, "Spiders"),
                new OpUpdown(spiderExtras, new Vector2(330f, sbs-455f), 60f),
                new OpLabel(400f, sbs-485f, "Dropwigs"),
                new OpUpdown(dropwigExtras, new Vector2(330f, sbs-490f), 60f),
                new OpLabel(400f, sbs-520f, "Eggbugs"),
                new OpUpdown(eggbugExtras, new Vector2(330f, sbs-525f), 60f),
                new OpLabel(400f, sbs-555f, "Monster Kelp"),
                new OpUpdown(kelpExtras, new Vector2(330f, sbs-560f), 60f),
                new OpLabel(400f, sbs-590f, "Leeches"),
                new OpUpdown(leechExtras, new Vector2(330f, sbs-595f), 60f),
                new OpLabel(400f, sbs-625f, "Miros Birds"),
                new OpUpdown(mirosExtras, new Vector2(330f, sbs-630f), 60f),
                new OpLabel(400f, sbs-660f, "Leviathans"),
                new OpUpdown(leviathanExtras, new Vector2(330f, sbs-665f), 60f),

            };

            UIDependentOptions = new UIelement[]
            {
                new OpLabel(80f, sbs-800f, "Small Insects > Sporantulas"),
                new OpUpdown(sporantulaChance, new Vector2(10f, sbs-805f), 60f),
                new OpLabel(400f, sbs-695f, "Extra Sporantulas"),
                new OpUpdown(sporantulaExtras, new Vector2(330f, sbs-700f), 60f),
            };
            scrollBox = new OpScrollBox(new Vector2(0f, 55f), new Vector2(580f, 480f), sbs, false, false, true);
            opTab.AddItems(UITitles);
            opTab._AddItem(scrollBox);
            scrollBox.AddItems(UIArrPlayerOptions);
            scrollBox.AddItems(UIDependentOptions);

            if (!mod.hasSporantulaMod)
            {
                for (int i = 0; i < UIDependentOptions.Length; i++)
                {
                    UIDependentOptions[i].Hide();
                }
            }
        }

        private void setDefaults()
        {
            for(int i = 0; i < UIArrPlayerOptions.Length; i++)
                if (UIArrPlayerOptions[i] is OpUpdown op)
                    op.Reset();

            for(int i = 0; i < UIDependentOptions.Length; i++)
                if (UIDependentOptions[i] is OpUpdown op)
                    op.Reset();

        }

        private void setNulls()
        {
            for (int i = 0; i < UIArrPlayerOptions.Length; i++)
                if (UIArrPlayerOptions[i] is OpUpdown op)
                    op.ForceValue("0");

            for (int i = 0; i < UIDependentOptions.Length; i++)
                if (UIDependentOptions[i] is OpUpdown op)
                    op.ForceValue("0");

        }

        public override void Update()
        {
            
            //Has to be replaced with a hook to defaultsSimpleButton.OnClick; currently the PUBLIC assembly is bugged and the
            //hook is not possible.
            if (defaultsSimpleButton._held)
            {
                setDefaults();
            }
        }

    }
}
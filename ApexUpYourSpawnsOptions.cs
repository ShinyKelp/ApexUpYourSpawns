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
            redLizardChance = this.config.Bind<int>("RedLizardChance", 8, new ConfigAcceptableRange<int>(0, 100));
            trainLizardChance = this.config.Bind<int>("TrainLizardChance", 20, new ConfigAcceptableRange<int>(0, 100));
            redCentipedeChance = this.config.Bind<int>("RedCentipedeChance", 8, new ConfigAcceptableRange<int>(0, 100));
            mirosVultureChance = this.config.Bind<int>("MirosVultureChance", 15, new ConfigAcceptableRange<int>(0, 100));
            spitterSpiderChance = this.config.Bind<int>("SpitterSpiderChance", 15, new ConfigAcceptableRange<int>(0, 100));
            noodleFlyChance = this.config.Bind<int>("NoodleFlyChance", 12, new ConfigAcceptableRange<int>(0, 100));
            fireBugChance = this.config.Bind<int>("FireBugChance", 40, new ConfigAcceptableRange<int>(0, 100));
            eliteScavengerChance = this.config.Bind<int>("EliteScavengerChance", 12, new ConfigAcceptableRange<int>(0, 100));
            brotherLongLegsChance = this.config.Bind<int>("BrotherLongLegsChance", 6, new ConfigAcceptableRange<int>(0, 100));
            daddyLongLegsChance = this.config.Bind<int>("DaddyLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
            terrorLongLegsChance = this.config.Bind<int>("MotherLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
            giantJellyfishChance = this.config.Bind<int>("GiantJellyfishChance", 10, new ConfigAcceptableRange<int>(0, 100));

            yellowLizExtras = this.config.Bind<int>("ExtraYellowLizs", 3, new ConfigAcceptableRange<int>(0, 10));
            cyanLizExtras = this.config.Bind<int>("ExtraCyanLizs", 0, new ConfigAcceptableRange<int>(0, 10));
            genericLizExtras = this.config.Bind<int>("ExtraLizards", 1, new ConfigAcceptableRange<int>(0, 10));
            waterLizExtras = this.config.Bind<int>("ExtraWaterLizs", 1, new ConfigAcceptableRange<int>(0, 10));
            precycleSalExtras = this.config.Bind<int>("ExtraPrecycleSalamanders", 4, new ConfigAcceptableRange<int>(0, 10));
            scavengerExtras = this.config.Bind<int>("ExtraScavengers", 4, new ConfigAcceptableRange<int>(0, 100));
            vultureExtras = this.config.Bind<int>("ExtraVultures", 1, new ConfigAcceptableRange<int>(0, 10));
            vultureKingExtras = this.config.Bind<int>("ExtraKingVultures", 1, new ConfigAcceptableRange<int>(0, 10));
            centipedeExtras = this.config.Bind<int>("ExtraKCentipedes", 1, new ConfigAcceptableRange<int>(0, 10));
            centiWingExtras = this.config.Bind<int>("ExtraCentiwings", 1, new ConfigAcceptableRange<int>(0, 10));
            aquapedeExtras = this.config.Bind<int>("ExtraAquaPedes", 1, new ConfigAcceptableRange<int>(0, 10));
            bigSpiderExtras = this.config.Bind<int>("ExtraBigSpiders", 3, new ConfigAcceptableRange<int>(0, 10));
            dropwigExtras = this.config.Bind<int>("ExtraDropwigs", 1, new ConfigAcceptableRange<int>(0, 10));
            eggbugExtras = this.config.Bind<int>("ExtraEggbugs", 1, new ConfigAcceptableRange<int>(0, 10));
            mirosExtras = this.config.Bind<int>("ExtraMirosBirds", 4, new ConfigAcceptableRange<int>(0, 10));
        }


        public readonly Configurable<int> redLizardChance;
        public readonly Configurable<int> redCentipedeChance;
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


        private OpSimpleButton defaultsSimpleButton;


        private UIelement[] UIArrPlayerOptions;

        public override void Initialize()
        {
            var opTab = new OpTab(this, "Options");
            this.Tabs = new[]
            {
                opTab
            };


            defaultsSimpleButton = new OpSimpleButton(new Vector2(100f, 50f), new Vector2(60, 30), "Defaults");
            UIArrPlayerOptions = new UIelement[]
            {
                new OpLabel(10f, 570f, "Options", true),
                new OpLabel(-2f, 543f, "Apex Replacements (% chance)", true),
                new OpLabel(80f, 515f, "Lizard > Red lizard"),
                new OpUpdown(redLizardChance, new Vector2(10f, 510f), 60f),
                new OpLabel(80f, 480f, "Red lizard > Train lizard"),
                new OpUpdown(trainLizardChance, new Vector2(10f, 475f), 60f),
                new OpLabel(80f, 445f, "Centipede > Red centipede"),
                new OpUpdown(redCentipedeChance, new Vector2(10f, 440f), 60f),
                new OpLabel(80f, 410f, "Vulture > Miros Vulture"),
                new OpUpdown(mirosVultureChance, new Vector2(10f, 405f), 60f),
                new OpLabel(80f, 375f, "Big Spider > Spitter Spider"),
                new OpUpdown(spitterSpiderChance, new Vector2(10f, 370f), 60f),
                new OpLabel(80f, 340f, "Scavenger > Elite Scavenger"),
                new OpUpdown(eliteScavengerChance, new Vector2(10f, 335f), 60f),
                new OpLabel(80f, 305f, "Eggbug > Firebug"),
                new OpUpdown(fireBugChance, new Vector2(10f, 300f), 60f),
                new OpLabel(80f, 270f, "Cicada > Noodlefly"),
                new OpUpdown(noodleFlyChance, new Vector2(10f, 265f), 60f),
                new OpLabel(80f, 235f, "Harmless critters > BrotherLongLegs"),
                new OpUpdown(brotherLongLegsChance, new Vector2(10f, 230f), 60f),
                new OpLabel(80f, 200f, "BrotherLongLegs > DaddyLongLegs"),
                new OpUpdown(daddyLongLegsChance, new Vector2(10f, 195f), 60f),
                new OpLabel(80f, 165f, "DaddyLongLegs > MotherLongLegs"),
                new OpUpdown(terrorLongLegsChance, new Vector2(10f, 160f), 60f),
                new OpLabel(80f, 130f, "Jellyfish > Giant Jellyfish"),
                new OpUpdown(giantJellyfishChance, new Vector2(10f, 125f), 60f),


                new OpLabel(325f, 543f, "Extra spawns (up to, per den)", true),
                new OpLabel(400f, 515f, "Lizards"),
                new OpUpdown(genericLizExtras, new Vector2(330f, 510f), 60f),
                new OpLabel(400f, 480f, "Yellow lizards"),
                new OpUpdown(yellowLizExtras, new Vector2(330f, 475f), 60f),
                new OpLabel(400f, 445f, "Cyan lizards"),
                new OpUpdown(cyanLizExtras, new Vector2(330f, 440f), 60f),
                new OpLabel(400f, 410f, "Water lizards"),
                new OpUpdown(waterLizExtras, new Vector2(330f, 405f), 60f),
                new OpLabel(400f, 375f, "Shelter failure salamanders"),
                new OpUpdown(precycleSalExtras, new Vector2(330f, 370f), 60f),
                new OpLabel(400f, 340f, "Scavengers"),
                new OpUpdown(scavengerExtras, new Vector2(330f, 335f), 60f),
                new OpLabel(400f, 305f, "Vultures"),
                new OpUpdown(vultureExtras, new Vector2(330f, 300f), 60f),
                new OpLabel(400f, 270f, "King vultures"),
                new OpUpdown(vultureKingExtras, new Vector2(330f, 265f), 60f),
                new OpLabel(400f, 235f, "Centipedes"),
                new OpUpdown(centipedeExtras, new Vector2(330f, 230f), 60f),
                new OpLabel(400f, 200f, "Centiwings"),
                new OpUpdown(centiWingExtras, new Vector2(330f, 195f), 60f),
                new OpLabel(400f, 165f, "Aquapedes"),
                new OpUpdown(aquapedeExtras, new Vector2(330f, 160f), 60f),
                new OpLabel(400f, 130f, "Big Spiders"),
                new OpUpdown(bigSpiderExtras, new Vector2(330f, 125f), 60f),
                new OpLabel(400f, 95f, "Dropwigs"),
                new OpUpdown(dropwigExtras, new Vector2(330f, 90f), 60f),
                new OpLabel(400f, 60f, "Eggbugs"),
                new OpUpdown(eggbugExtras, new Vector2(330f, 55f), 60f),
                new OpLabel(400f, 25f, "Miros Birds"),
                new OpUpdown(mirosExtras, new Vector2(330f, 20f), 60f),

                defaultsSimpleButton,

            };
            opTab.AddItems(UIArrPlayerOptions);
        }

        private void setDefaults()
        {
            for(int i = 0; i < UIArrPlayerOptions.Length; i++)
            {
                if (UIArrPlayerOptions[i] is OpUpdown op)
                    op.Reset();

            }

        }

        private void setNulls()
        {
            for (int i = 0; i < UIArrPlayerOptions.Length; i++)
            {
                if (UIArrPlayerOptions[i] is OpUpdown op)
                {
                    op._value = "0";
                }
            }
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
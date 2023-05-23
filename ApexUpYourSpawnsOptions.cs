using BepInEx.Logging;
using Menu.Remix.MixedUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ApexUpYourSpawns
{
    public class ApexUpYourSpawnsOptions : OptionInterface
    {
        private readonly ManualLogSource Logger;

        public readonly Configurable<bool> fillLineages;
        public readonly Configurable<bool> forceFreshSpawns;
        public readonly Configurable<bool> balancedSpawns;

        //Replacements
        public readonly Configurable<int> redLizardChance;
        public readonly Configurable<int> redCentipedeChance;
        public readonly Configurable<int> largeCentipedeChance;
        public readonly Configurable<int> mirosVultureChance;
        public readonly Configurable<int> kingVultureChance;
        public readonly Configurable<int> spitterSpiderChance;
        public readonly Configurable<int> trainLizardChance;
        public readonly Configurable<int> flyingPredatorChance;
        public readonly Configurable<int> fireBugChance;
        public readonly Configurable<int> eliteScavengerChance;
        public readonly Configurable<int> brotherLongLegsChance;
        public readonly Configurable<int> daddyLongLegsChance;
        public readonly Configurable<int> terrorLongLegsChance;
        public readonly Configurable<int> giantJellyfishChance;
        public readonly Configurable<int> leechLizardChance;
        public readonly Configurable<int> waterPredatorChance;
        public readonly Configurable<int> yeekLizardChance;
        public readonly Configurable<int> caramelLizChance;
        public readonly Configurable<int> strawberryLizChance;
        public readonly Configurable<int> cyanLizChance;
        public readonly Configurable<int> eelLizChance;
        public readonly Configurable<int> jungleLeechChance;
        public readonly Configurable<int> motherSpiderChance;
        public readonly Configurable<int> stowawayChance;
        //Mod dependent
        public readonly Configurable<int> inspectorChance;
        public readonly Configurable<int> sporantulaChance;
        public readonly Configurable<int> scutigeraChance;
        public readonly Configurable<int> redHorrorCentiChance;
        public readonly Configurable<int> longlegsVariantChance;
        public readonly Configurable<int> waterSpitterChance;
        public readonly Configurable<int> fatFireFlyChance;
        public readonly Configurable<int> sludgeLizardChance;
        public readonly Configurable<int> snailSludgeLizardChance;
        public readonly Configurable<int> mintLizardChance;
        public readonly Configurable<int> ryanLizardChance;
        public readonly Configurable<int> yellowLimeLizardChance;

        //Extra spawns
        public readonly Configurable<int> yellowLizExtras;
        public readonly Configurable<int> cyanLizExtras;
        public readonly Configurable<int> genericLizExtras;
        public readonly Configurable<int> waterLizExtras;
        public readonly Configurable<int> precycleSalExtras;
        public readonly Configurable<int> scavengerExtras;
        public readonly Configurable<int> vultureExtras;
        public readonly Configurable<int> centipedeExtras;
        public readonly Configurable<int> centiWingExtras;
        public readonly Configurable<int> aquapedeExtras;
        public readonly Configurable<int> bigSpiderExtras;
        public readonly Configurable<int> dropwigExtras;
        public readonly Configurable<int> eggbugExtras;
        public readonly Configurable<int> cicadaExtras;
        public readonly Configurable<int> lmiceExtras;
        public readonly Configurable<int> snailExtras;
        public readonly Configurable<int> jetfishExtras;
        public readonly Configurable<int> smallCentExtras;
        public readonly Configurable<int> yeekExtras;
        public readonly Configurable<int> mirosExtras;
        public readonly Configurable<int> spiderExtras;
        public readonly Configurable<int> leechExtras;
        public readonly Configurable<int> kelpExtras;
        public readonly Configurable<int> leviathanExtras;
        public readonly Configurable<int> nightCreatureExtras;
        //Mod dependent
        public readonly Configurable<int> sporantulaExtras;
        public readonly Configurable<int> scutigeraExtras;
        public readonly Configurable<int> waterSpitterExtras;
        public readonly Configurable<int> sludgeLizardExtras;

        private OpSimpleButton defaultsSimpleButton, nullsSimpleButton;
        private OpCheckBox fillLineageCheck, forceFreshCheck, balancedSpawnsCheck;
        private OpScrollBox scrollBox;

        private UIelement[] UIFixed, UIBaseGameOptions, UIDependentOptions;

        private ApexUpYourSpawnsMod apexMod;

        public ApexUpYourSpawnsOptions(ApexUpYourSpawnsMod modInstance, ManualLogSource loggerSource)
        {
            Logger = loggerSource;
            apexMod = modInstance;

            fillLineages = this.config.Bind<bool>("FillLineages", false);
            forceFreshSpawns = this.config.Bind<bool>("ForceFreshSpawns", false);
            balancedSpawns = this.config.Bind<bool>("BalancedSpawns", true);

            //Replacements
            redLizardChance = this.config.Bind<int>("RedLizardChance", 6, new ConfigAcceptableRange<int>(0, 100));
            trainLizardChance = this.config.Bind<int>("TrainLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            largeCentipedeChance = this.config.Bind<int>("LargeCentipedeChance", 0, new ConfigAcceptableRange<int>(0, 100));
            redCentipedeChance = this.config.Bind<int>("RedCentipedeChance", 8, new ConfigAcceptableRange<int>(0, 100));
            mirosVultureChance = this.config.Bind<int>("MirosVultureChance", 15, new ConfigAcceptableRange<int>(0, 100));
            kingVultureChance = this.config.Bind<int>("KingVultureChance", 10, new ConfigAcceptableRange<int>(0, 100));
            spitterSpiderChance = this.config.Bind<int>("SpitterSpiderChance", 15, new ConfigAcceptableRange<int>(0, 100));
            flyingPredatorChance = this.config.Bind<int>("FlyingPredatorChance", 12, new ConfigAcceptableRange<int>(0, 100));
            fireBugChance = this.config.Bind<int>("FireBugChance", 30, new ConfigAcceptableRange<int>(0, 100));
            eliteScavengerChance = this.config.Bind<int>("EliteScavengerChance", 12, new ConfigAcceptableRange<int>(0, 100));
            brotherLongLegsChance = this.config.Bind<int>("BrotherLongLegsChance", 4, new ConfigAcceptableRange<int>(0, 100));
            daddyLongLegsChance = this.config.Bind<int>("DaddyLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
            terrorLongLegsChance = this.config.Bind<int>("MotherLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
            giantJellyfishChance = this.config.Bind<int>("GiantJellyfishChance", 10, new ConfigAcceptableRange<int>(0, 100));
            leechLizardChance = this.config.Bind<int>("LeechLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            waterPredatorChance = this.config.Bind<int>("AquapedeChance", 20, new ConfigAcceptableRange<int>(0, 100));
            yeekLizardChance = this.config.Bind<int>("YeekLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            caramelLizChance = this.config.Bind<int>("CaramelLizChance", 15, new ConfigAcceptableRange<int>(0, 100));
            strawberryLizChance = this.config.Bind<int>("StrawberryLizChance", 5, new ConfigAcceptableRange<int>(0, 100));
            cyanLizChance = this.config.Bind<int>("CyanLizChance", 0, new ConfigAcceptableRange<int>(0, 100));
            eelLizChance = this.config.Bind<int>("EelLizChance", 10, new ConfigAcceptableRange<int>(0, 100));
            jungleLeechChance = this.config.Bind<int>("JungleLeechChance", 0, new ConfigAcceptableRange<int>(0, 100));
            motherSpiderChance = this.config.Bind<int>("MotherSpiderChance", 10, new ConfigAcceptableRange<int>(0, 100));
            stowawayChance = this.config.Bind<int>("StowawayChance", 3, new ConfigAcceptableRange<int>(0, 100));

            //Mod dependent
            inspectorChance = this.config.Bind<int>("InspectorChance", 8, new ConfigAcceptableRange<int>(0, 100));
            sporantulaChance = this.config.Bind<int>("SporantulaChance", 4, new ConfigAcceptableRange<int>(0, 100));
            scutigeraChance = this.config.Bind<int>("ScutigeraChance", 15, new ConfigAcceptableRange<int>(0, 100));
            redHorrorCentiChance = this.config.Bind<int>("RedHorrorCentiChance", 10, new ConfigAcceptableRange<int>(0, 100));
            longlegsVariantChance = this.config.Bind<int>("LongLegsVariantChance", 5, new ConfigAcceptableRange<int>(0, 100));
            waterSpitterChance = this.config.Bind<int>("WaterSpitterChance", 10, new ConfigAcceptableRange<int>(0, 100));
            fatFireFlyChance = this.config.Bind<int>("FatFireFlyChance", 10, new ConfigAcceptableRange<int>(0, 100));
            sludgeLizardChance = this.config.Bind<int>("SludgeLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
            snailSludgeLizardChance = this.config.Bind<int>("SnailSludgeLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            mintLizardChance = this.config.Bind<int>("MintLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            ryanLizardChance = this.config.Bind<int>("RyanLizardChance", 4, new ConfigAcceptableRange<int>(0, 100));
            yellowLimeLizardChance = this.config.Bind<int>("YellowLimeLizardChance", 16, new ConfigAcceptableRange<int>(0, 100));

            //Extras
            yellowLizExtras = this.config.Bind<int>("ExtraYellowLizs", 25, new ConfigAcceptableRange<int>(0, 100));
            cyanLizExtras = this.config.Bind<int>("ExtraCyanLizs", 2, new ConfigAcceptableRange<int>(0, 100));
            genericLizExtras = this.config.Bind<int>("ExtraLizards", 4, new ConfigAcceptableRange<int>(0, 100));
            waterLizExtras = this.config.Bind<int>("ExtraWaterLizs", 10, new ConfigAcceptableRange<int>(0, 100));
            precycleSalExtras = this.config.Bind<int>("ExtraPrecycleSalamanders", 10, new ConfigAcceptableRange<int>(0, 100));
            scavengerExtras = this.config.Bind<int>("ExtraScavengers", 4, new ConfigAcceptableRange<int>(0, 20));
            vultureExtras = this.config.Bind<int>("ExtraVultures", 1, new ConfigAcceptableRange<int>(0, 20));
            centipedeExtras = this.config.Bind<int>("ExtraCentipedes", 0, new ConfigAcceptableRange<int>(0, 100));
            centiWingExtras = this.config.Bind<int>("ExtraCentiwings", 0, new ConfigAcceptableRange<int>(0, 100));
            aquapedeExtras = this.config.Bind<int>("ExtraAquaPedes", 5, new ConfigAcceptableRange<int>(0, 100));
            bigSpiderExtras = this.config.Bind<int>("ExtraBigSpiders", 25, new ConfigAcceptableRange<int>(0, 100));
            dropwigExtras = this.config.Bind<int>("ExtraDropwigs", 10, new ConfigAcceptableRange<int>(0, 100));
            eggbugExtras = this.config.Bind<int>("ExtraEggbugs", 10, new ConfigAcceptableRange<int>(0, 100));
            cicadaExtras = this.config.Bind<int>("ExtraCicadas", 10, new ConfigAcceptableRange<int>(0, 100));
            snailExtras = this.config.Bind<int>("ExtraSnails", 3, new ConfigAcceptableRange<int>(0, 100));
            jetfishExtras = this.config.Bind<int>("ExtraJetfish", 6, new ConfigAcceptableRange<int>(0, 100));
            lmiceExtras = this.config.Bind<int>("ExtraLMice", 3, new ConfigAcceptableRange<int>(0, 100));
            smallCentExtras = this.config.Bind<int>("ExtraSmallCents", 8, new ConfigAcceptableRange<int>(0, 100));
            yeekExtras = this.config.Bind<int>("ExtraYeek", 8, new ConfigAcceptableRange<int>(0, 100));
            mirosExtras = this.config.Bind<int>("ExtraMirosBirds", 2, new ConfigAcceptableRange<int>(0, 20));
            spiderExtras = this.config.Bind<int>("ExtraSpiders", 0, new ConfigAcceptableRange<int>(0, 20));
            leechExtras = this.config.Bind<int>("ExtraLeeches", 0, new ConfigAcceptableRange<int>(0, 20));
            kelpExtras = this.config.Bind<int>("ExtraKelps", 6, new ConfigAcceptableRange<int>(0, 100));
            leviathanExtras = this.config.Bind<int>("ExtraLeviathans", 4, new ConfigAcceptableRange<int>(0, 100));
            nightCreatureExtras = this.config.Bind<int>("ExtraNightCreatures", 10, new ConfigAcceptableRange<int>(0, 100));
            //Mod dependent
            sporantulaExtras = this.config.Bind<int>("ExtraSporantulas", 25, new ConfigAcceptableRange<int>(0, 100));
            scutigeraExtras = this.config.Bind<int>("ExtraScutigeras", 0, new ConfigAcceptableRange<int>(0, 100));
            waterSpitterExtras = this.config.Bind<int>("ExtraWaterSpitters", 0, new ConfigAcceptableRange<int>(0, 100));
            sludgeLizardExtras = this.config.Bind<int>("ExtraSludgeLizards", 8, new ConfigAcceptableRange<int>(0, 100));
        }

        private void SetDictionary(Dictionary<Configurable<int>, string> labelsMap)
        {
            //Replacements
            labelsMap.Add(redLizardChance, "Lizard > Red Lizard");
            labelsMap.Add(trainLizardChance, "Red Lizard > Train Lizard");
            labelsMap.Add(largeCentipedeChance, "Small Cent > Large Centipede");
            labelsMap.Add(redCentipedeChance, "Large Cent > Red Centipede");
            labelsMap.Add(kingVultureChance, "Vulture > King Vulture");
            labelsMap.Add(mirosVultureChance, "Vultures > Miros Vulture");
            labelsMap.Add(eliteScavengerChance, "Scavenger > Elite Scavenger");
            labelsMap.Add(fireBugChance, "Eggbug > Firebug");
            labelsMap.Add(flyingPredatorChance, "Cicada > Flying Predators");
            labelsMap.Add(brotherLongLegsChance, "L.Mice/Snail/??? > LongLegs");
            labelsMap.Add(daddyLongLegsChance, "Brother > DaddyLongLegs");
            labelsMap.Add(terrorLongLegsChance, "Daddy > MotherLongLegs");
            labelsMap.Add(giantJellyfishChance, "Jellyfish > Giant Jellyfish");
            labelsMap.Add(leechLizardChance, "Leeches > Aquatic Lizard (Mult)");
            labelsMap.Add(yeekLizardChance, "Yeek > Caramel/Strawberry Liz");
            labelsMap.Add(waterPredatorChance, "Jetfish > Water Predators");
            labelsMap.Add(strawberryLizChance, "Pink > Strawberry Lizard");
            labelsMap.Add(caramelLizChance, "Green > Caramel Lizard");
            labelsMap.Add(cyanLizChance, "Blue > Cyan Lizard");
            labelsMap.Add(eelLizChance, "Salamander > Eel Lizard");
            labelsMap.Add(spitterSpiderChance, "Big Spider > Spitter Spider");
            labelsMap.Add(motherSpiderChance, "Small Spiders > Mother Spider (Mult)");
            labelsMap.Add(jungleLeechChance, "Leech > Jungle Leech");
            labelsMap.Add(stowawayChance, "Ceiling Fruits > Stowawaybug Trap");

            //Mod dependent replacements
            labelsMap.Add(inspectorChance, "LongLegs/??? > Inspector (Inv)");
            labelsMap.Add(sporantulaChance, "Small Insects > Sporantula (Inv)");
            labelsMap.Add(scutigeraChance, "Centipede > Scutigera");
            labelsMap.Add(redHorrorCentiChance, "Red Centipede > Red Horror Centi");
            labelsMap.Add(longlegsVariantChance, "LongLegs > LongLegs Variant");
            labelsMap.Add(waterSpitterChance, "Aquatic Lizards > Water Spitter");
            labelsMap.Add(fatFireFlyChance, "Vultures > Fat Firefly");
            labelsMap.Add(sludgeLizardChance, "Water Lizards > Sludge Lizard");
            labelsMap.Add(snailSludgeLizardChance, "Snails > Sludge Lizard (Mult)");
            labelsMap.Add(mintLizardChance, "Ground Lizards > Mint Lizard");
            labelsMap.Add(ryanLizardChance, "Cyan Lizard > Ryan Lizard");
            labelsMap.Add(yellowLimeLizardChance, "Yellow Lizard > YellowLime Lizard");

            //Extras
            labelsMap.Add(yellowLizExtras, "Yellow Lizards (/10)");
            labelsMap.Add(cyanLizExtras, "Cyan Lizards (/10)");
            labelsMap.Add(genericLizExtras, "Lizards (/10)");
            labelsMap.Add(waterLizExtras, "Aquatic Lizards (/10)");
            labelsMap.Add(precycleSalExtras, "Shelter Failure Spawns (/10)");
            labelsMap.Add(scavengerExtras, "Scavengers");
            labelsMap.Add(vultureExtras, "Vultures");
            labelsMap.Add(centipedeExtras, "Centipedes (/10)");
            labelsMap.Add(centiWingExtras, "Centiwings (/10)");
            labelsMap.Add(aquapedeExtras, "Aquapedes (/10)");
            labelsMap.Add(bigSpiderExtras, "Big Spiders (/10)");
            labelsMap.Add(dropwigExtras, "Dropwigs (/10)");
            labelsMap.Add(eggbugExtras, "Eggbugs (/10)");
            labelsMap.Add(cicadaExtras, "Cicadas (/10)");
            labelsMap.Add(smallCentExtras, "Small Centipedes (/10)");
            labelsMap.Add(snailExtras, "Snails (/10)");
            labelsMap.Add(jetfishExtras, "Jetfish (/10)");
            labelsMap.Add(lmiceExtras, "Lantern Mice (/10)");
            labelsMap.Add(yeekExtras, "Yeeks (/10)");
            labelsMap.Add(mirosExtras, "Miros Birds");
            labelsMap.Add(spiderExtras, "Small Spiders");
            labelsMap.Add(leechExtras, "Leeches");
            labelsMap.Add(kelpExtras, "Monster Kelp (/10)");
            labelsMap.Add(leviathanExtras, "Leviathans");
            labelsMap.Add(nightCreatureExtras, "Night Creatures (/10)");
            //Mod-dependant extras
            labelsMap.Add(sporantulaExtras, "Sporantulas (/10)");
            labelsMap.Add(scutigeraExtras, "Scutigeras (/10)");
            labelsMap.Add(waterSpitterExtras, "Water Spitters (/10)");
            labelsMap.Add(sludgeLizardExtras, "Sludge Lizards (/10)");

        }


        public override void Initialize()
        {
            //Constants
            float sbs = 900f;   //Base size of scrollbox. Manually adjusted.

            var opTab = new OpTab(this, "Options");
            this.Tabs = new[]
            {
                opTab
            };

            defaultsSimpleButton = new OpSimpleButton(new Vector2(10f, 10f), new Vector2(60, 30), "Defaults")
            {
                description = "Set options to default (just a recommended template)"
            };
            //defaultsSimpleButton.OnClick += setDefaults(); //Currently bugged. Dual-Iron's PUBLIC library fix won't work for me either.
            nullsSimpleButton = new OpSimpleButton(new Vector2(80f, 10f), new Vector2(60, 30), "Nulls")
            {
                description = "Set options to null (no changes)"
            };

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

            

            UIFixed = new UIelement[]
            {
                new OpLabel(10f, 570f, "Options", true),
                new OpLabel(-2f, 543f, "Apex Replacements (% chance)", true){
                    description = "Mult: Each DEN has a chance to be replaced instead of each individual creature.\n" +
                    "Inv: Invasion. Original creature is not replaced."
                },
                new OpLabel(325f, 543f, "Extra spawns (up to, per den)", true){
                    description = "\\10: Value is divided by 10. Decimals determine chance of one extra addition.\n" +
                    "Example: 11 becomes 1.1 => either 0 or 1, with a 10% chance of one extra."
                },

                defaultsSimpleButton,
                nullsSimpleButton,
                fillLineageCheck,
                new OpLabel(187f, 16f, "Fill Lineages"),
                balancedSpawnsCheck,
                new OpLabel(289f, 16f, "Balanced spawns"),
                forceFreshCheck,
                new OpLabel(421f, 16f, "Randomize every cycle"),  
            };



            //Set the base game configs
            Dictionary<Configurable<int>, string> labelsMap = new Dictionary<Configurable<int>, string>();
            SetDictionary(labelsMap);

            Configurable<int>[] UIReplacementConfigs = new Configurable<int>[]
            {
                redLizardChance, trainLizardChance, strawberryLizChance, caramelLizChance, cyanLizChance, eelLizChance, 
                leechLizardChance, yeekLizardChance, largeCentipedeChance, redCentipedeChance, kingVultureChance, mirosVultureChance, 
                eliteScavengerChance, spitterSpiderChance, motherSpiderChance, jungleLeechChance, fireBugChance, 
                brotherLongLegsChance, daddyLongLegsChance, terrorLongLegsChance, 
                flyingPredatorChance, waterPredatorChance, giantJellyfishChance, stowawayChance
            };

            Configurable<int>[] UIExtraConfigs = new Configurable<int>[]
            {
                genericLizExtras, yellowLizExtras, cyanLizExtras, waterLizExtras,  
                smallCentExtras, centipedeExtras, centiWingExtras, aquapedeExtras, bigSpiderExtras, dropwigExtras, 
                kelpExtras, leviathanExtras, eggbugExtras, cicadaExtras, 
                lmiceExtras, snailExtras, jetfishExtras, yeekExtras, precycleSalExtras, nightCreatureExtras,
                scavengerExtras, vultureExtras, mirosExtras, spiderExtras, leechExtras
            };

            //Set the mod configs
            List<Configurable<int>> enabledModsRepConfigs = new List<Configurable<int>>();
            List<Configurable<int>> enabledModsExtraConfigs = new List<Configurable<int>>();
            if (apexMod.hasSporantula)
            {
                enabledModsRepConfigs.Add(sporantulaChance);
                enabledModsExtraConfigs.Add(sporantulaExtras);
            }
            if (apexMod.hasAngryInspectors)
                enabledModsRepConfigs.Add(inspectorChance);
            if (apexMod.hasScutigera)
            {
                enabledModsRepConfigs.Add(scutigeraChance);
                enabledModsExtraConfigs.Add(scutigeraExtras);
            }
            if (apexMod.hasRedHorrorCentipede)
                enabledModsRepConfigs.Add(redHorrorCentiChance);
            if (apexMod.hasExplosiveDLL || apexMod.hasMoreDLLs)
                enabledModsRepConfigs.Add(longlegsVariantChance);
            if (apexMod.hasWaterSpitter)
            {
                enabledModsRepConfigs.Add(waterSpitterChance);
                enabledModsExtraConfigs.Add(waterSpitterExtras);
            }
            if (apexMod.hasFatFirefly)
                enabledModsRepConfigs.Add(fatFireFlyChance);
            if (apexMod.hasSludgeLizard)
            {
                enabledModsRepConfigs.Add(sludgeLizardChance);
                enabledModsRepConfigs.Add(snailSludgeLizardChance);
                enabledModsExtraConfigs.Add(sludgeLizardExtras);
            }
            if (apexMod.hasLizardVariants)
            {
                enabledModsRepConfigs.Add(mintLizardChance);
                enabledModsRepConfigs.Add(ryanLizardChance);
                enabledModsRepConfigs.Add(yellowLimeLizardChance);
            }
                

            //Adjust scrollbox's size accordingly
            if (enabledModsRepConfigs.Count + UIReplacementConfigs.Length > enabledModsExtraConfigs.Count + UIExtraConfigs.Length)
                sbs += enabledModsRepConfigs.Count * 45f;
            else sbs += enabledModsExtraConfigs.Count * 45f;

            int replaceLength = UIReplacementConfigs.Length * 2;
            int extraLength = UIExtraConfigs.Length * 2;

            UIBaseGameOptions = new UIelement[extraLength + replaceLength];
            
            //Set the base replacements
            string auxString;
            for(int i = 0; i < UIReplacementConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(UIReplacementConfigs[i], out auxString);
                UIBaseGameOptions[i*2] = new OpLabel(80f, sbs-30f-(35f*i), auxString);
                UIBaseGameOptions[i * 2 + 1] = new OpUpdown(UIReplacementConfigs[i], new Vector2(10f, sbs-35f-(35f*i)), 60f);
            }

            //Set the base extras
            for(int i = 0; i < UIExtraConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(UIExtraConfigs[i], out auxString);
                UIBaseGameOptions[replaceLength + i*2] = new OpLabel(400f, sbs-30f-(35f*i), auxString);
                UIBaseGameOptions[replaceLength + i*2+1] = new OpUpdown(UIExtraConfigs[i], new Vector2(330f, sbs-35f-(35f*i)), 60f);
            }

            //Set the mod-dependant configs
            int modReplaceLength = enabledModsRepConfigs.Count * 2;
            int modExtraLength = enabledModsExtraConfigs.Count * 2;
            UIDependentOptions = new UIelement[modReplaceLength + modExtraLength];

            //Set the mod dependant replacement configs
            for(int i = 0; i < enabledModsRepConfigs.Count; ++i)
            {
                labelsMap.TryGetValue(enabledModsRepConfigs[i], out auxString);
                UIDependentOptions[i*2] = new OpLabel(80f, sbs-30f-(35f*(i+UIReplacementConfigs.Length)), auxString);
                UIDependentOptions[i*2+1] = new OpUpdown(enabledModsRepConfigs[i], new Vector2(10f, sbs-35f-(35f*(i+UIReplacementConfigs.Length))), 60f);
            }

            //Set the mod dependent extra configs
            for (int i = 0; i < enabledModsExtraConfigs.Count; ++i)
            {
                labelsMap.TryGetValue(enabledModsExtraConfigs[i], out auxString);
                UIDependentOptions[modReplaceLength+i*2] = new OpLabel(400f, sbs-30f-(35f*(i+UIExtraConfigs.Length)), auxString);
                UIDependentOptions[modReplaceLength+i*2+1] = new OpUpdown(enabledModsExtraConfigs[i], new Vector2(330f, sbs-35f-(35f*(i+UIExtraConfigs.Length))), 60f);
            }

            scrollBox = new OpScrollBox(new Vector2(0f, 55f), new Vector2(580f, 480f), sbs, false, false, true);
            opTab.AddItems(UIFixed);
            opTab._AddItem(scrollBox);
            scrollBox.AddItems(UIBaseGameOptions);
            scrollBox.AddItems(UIDependentOptions);

            labelsMap.Clear();
            enabledModsRepConfigs.Clear();
            enabledModsExtraConfigs.Clear();

        }


        private void SetDefaults()
        {
            for(int i = 0; i < UIBaseGameOptions.Length; i++)
                if (UIBaseGameOptions[i] is OpUpdown op)
                {
                    op.Reset();
                }

            for (int i = 0; i < UIDependentOptions.Length; i++)
                if (UIDependentOptions[i] is OpUpdown op)
                    op.Reset();

        }

        private void SetNulls()
        {
            string aux;
            for (int i = 0; i < UIBaseGameOptions.Length; i++)
                if (UIBaseGameOptions[i] is OpUpdown op)
                {
                    aux = op.defaultValue;
                    op.defaultValue = "0";
                    op.Reset();
                    op.defaultValue = aux;
                }

            for (int i = 0; i < UIDependentOptions.Length; i++)
                if (UIDependentOptions[i] is OpUpdown op)
                {
                    aux = op.defaultValue;
                    op.defaultValue = "0";
                    op.Reset();
                    op.defaultValue = aux;
                }
        }

        public override void Update()
        {
            
            //Has to be replaced with a hook to defaultsSimpleButton.OnClick; currently the PUBLIC assembly is bugged and the
            //hook is not possible.
            if (defaultsSimpleButton._held)
            {
                SetDefaults();
            }
            if (nullsSimpleButton._held)
            {
                SetNulls();
            }
        }

    }
}
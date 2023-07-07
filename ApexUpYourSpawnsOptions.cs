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
        public readonly Configurable<int> seaLeechAquapedeChance;
        public readonly Configurable<int> waterPredatorChance;
        public readonly Configurable<int> yeekLizardChance;
        public readonly Configurable<int> caramelLizChance;
        public readonly Configurable<int> strawberryLizChance;
        public readonly Configurable<int> cyanLizChance;
        public readonly Configurable<int> eelLizChance;
        public readonly Configurable<int> jungleLeechChance;
        public readonly Configurable<int> motherSpiderChance;
        public readonly Configurable<int> stowawayChance;
        public readonly Configurable<int> kingScavengerChance;
        public readonly Configurable<int> hunterLongLegsChance;

        //Extra spawns
        public readonly Configurable<int> greenLizExtras;
        public readonly Configurable<int> pinkLizExtras;
        public readonly Configurable<int> blueLizExtras;
        public readonly Configurable<int> whiteLizExtras;
        public readonly Configurable<int> blackLizExtras;
        public readonly Configurable<int> yellowLizExtras;
        public readonly Configurable<int> salExtras;
        public readonly Configurable<int> cyanLizExtras;
        public readonly Configurable<int> zoopLizExtras;
        public readonly Configurable<int> caramelLizExtras;
        public readonly Configurable<int> eelLizExtras;
        public readonly Configurable<int> precycleCreatureExtras;
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
        //Replacements
        public Configurable<int> inspectorChance, sporantulaChance, scutigeraChance, redRedHorrorCentiChance, wingRedHorrorCentiChance,
            mExplosiveLongLegsChance, explosionLongLegsChance, mZappyLongLegsChance, waterSpitterChance, fatFireFlyChance, sludgeLizardChance,
            snailSludgeLizardChance, mintLizardChance, ryanLizardChance, yellowLimeLizardChance, lizorInvChance, voltLizardChance, magentaLizardChance,
            yellowTangerineLizardInvChance, cyanTangerineLizardInvChance, chameleonLizarcChance, skyBlueLizardChance,
            cherryLizardChance, strawberryRaspberryLizardChance, redRaspberryLizardChance, surfaceSwimmerChance, bounceBallChance,
            rainbowLongLegsChance, brownLizardChance, rotzardChance, universalLizardChance, gildedLizardChance, scalizardSchance, 
            nightmareLizardChance, turquoiseLizardChance, amoebaLizardChance, gargolemLizardChance, motherLizardChance, lostYoungLizardChance,
            snowSpiderChance;

        //Extras
        public Configurable<int> sporantulaExtras, scutigeraExtras, waterSpitterExtras, sludgeLizardExtras, mintLizardExtras,
            lizorsExtras, tangerineLizExtras, cherryLizExtras, surfaceSwimmerExtras, bounceBallExtras, snowSpiderExtras,
            youngLizardExtras;


        private OpSimpleButton defaultsSimpleButton, nullsSimpleButton, replacementDescription, extrasDescription, replacementDescription2, extrasDescription2;
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
            kingScavengerChance = this.config.Bind<int>("KingScavengerChance", 5, new ConfigAcceptableRange<int>(0, 100));
            hunterLongLegsChance = this.config.Bind<int>("HunterLongLegsChance", 20, new ConfigAcceptableRange<int>(0, 100));

            //Extras
            greenLizExtras = this.config.Bind<int>("ExtraGreenLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            blueLizExtras = this.config.Bind<int>("ExtraBlueLizs", 6, new ConfigAcceptableRange<int>(0, 100));
            pinkLizExtras = this.config.Bind<int>("ExtraPinkLizs", 8, new ConfigAcceptableRange<int>(0, 100));
            whiteLizExtras = this.config.Bind<int>("ExtraWhiteLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            blackLizExtras = this.config.Bind<int>("ExtraBlackLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            yellowLizExtras = this.config.Bind<int>("ExtraYellowLizs", 25, new ConfigAcceptableRange<int>(0, 100));
            salExtras = this.config.Bind<int>("ExtraSals", 12, new ConfigAcceptableRange<int>(0, 100));
            cyanLizExtras = this.config.Bind<int>("ExtraCyanLizs", 2, new ConfigAcceptableRange<int>(0, 100));
            caramelLizExtras = this.config.Bind<int>("ExtraCaramelLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            zoopLizExtras = this.config.Bind<int>("ExtraZoopLizs", 8, new ConfigAcceptableRange<int>(0, 100));
            eelLizExtras = this.config.Bind<int>("ExtraEelLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            precycleCreatureExtras = this.config.Bind<int>("ExtraPrecycleCreatures", 10, new ConfigAcceptableRange<int>(0, 100));
            scavengerExtras = this.config.Bind<int>("ExtraScavengers", 4, new ConfigAcceptableRange<int>(0, 20));
            vultureExtras = this.config.Bind<int>("ExtraVultures", 1, new ConfigAcceptableRange<int>(0, 20));
            centipedeExtras = this.config.Bind<int>("ExtraCentipedes", 3, new ConfigAcceptableRange<int>(0, 100));
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
            labelsMap.Add(brotherLongLegsChance, "L.Mice/Snail/??? > LongLegs (Den)");
            labelsMap.Add(daddyLongLegsChance, "Brother > DaddyLongLegs");
            labelsMap.Add(terrorLongLegsChance, "Daddy > MotherLongLegs");
            labelsMap.Add(giantJellyfishChance, "Jellyfish > Giant Jellyfish (*)");
            labelsMap.Add(leechLizardChance, "Leeches > Aquatic Lizard (Den)");
            labelsMap.Add(leechLizardChance, "Sea Leeches > Aquapede (Den)");
            labelsMap.Add(yeekLizardChance, "Yeek > Caramel/Strawberry Liz");
            labelsMap.Add(waterPredatorChance, "Jetfish > Aquatic Predators");
            labelsMap.Add(strawberryLizChance, "Pink > Strawberry Lizard");
            labelsMap.Add(caramelLizChance, "Green > Caramel Lizard");
            labelsMap.Add(cyanLizChance, "Blue > Cyan Lizard");
            labelsMap.Add(eelLizChance, "Salamander > Eel Lizard");
            labelsMap.Add(spitterSpiderChance, "Big Spider > Spitter Spider");
            labelsMap.Add(motherSpiderChance, "Small Spiders > Mother Spider (Den)");
            labelsMap.Add(jungleLeechChance, "Leech > Jungle Leech");
            labelsMap.Add(stowawayChance, "Ceiling Fruits > Stowawaybug Trap (*)");
            labelsMap.Add(kingScavengerChance, "Elite > King Scavenger (*)");
            labelsMap.Add(hunterLongLegsChance, "Slugpup > HunterLongLegs (*)");

            //Extras
            labelsMap.Add(yellowLizExtras, "Yellow Lizards (/10)");
            labelsMap.Add(blueLizExtras, "Blue Lizards (/10)");
            labelsMap.Add(greenLizExtras, "Green Lizards (/10)");
            labelsMap.Add(pinkLizExtras, "Pink Lizards (/10)");
            labelsMap.Add(whiteLizExtras, "White Lizards (/10)");
            labelsMap.Add(blackLizExtras, "Black Lizards (/10)");
            labelsMap.Add(salExtras, "Salamanders (/10)");
            labelsMap.Add(cyanLizExtras, "Cyan Lizards (/10)");
            labelsMap.Add(caramelLizExtras, "Caramel Lizards (/10)");
            labelsMap.Add(eelLizExtras, "Eel Lizards (/10)");
            labelsMap.Add(zoopLizExtras, "Strawberry Lizards (/10)");
            labelsMap.Add(precycleCreatureExtras, "Shelter Failure Spawns (/10)");
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
            labelsMap.Add(leviathanExtras, "Leviathans (/10)");
            labelsMap.Add(nightCreatureExtras, "Night Creatures (/10)");
        }

        public override void Initialize()
        {
            float scrollBoxSize;


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


            replacementDescription = new OpSimpleButton(new Vector2(137, 543), new Vector2(16, 16), "i")
            {
                description = "% chance to replace a creature with another.\n" + 
                "*: Creature is not from a spawner. Replacements vary between cycles."
            };
            extrasDescription = new OpSimpleButton(new Vector2(457, 543), new Vector2(16, 16), "i")
            {
                description = "Increase the creature amount of each den by random value from 0 to X.\n" + 
                "Does not apply to lineages."
            };

            replacementDescription2 = new OpSimpleButton(new Vector2(167, 543), new Vector2(16, 16), "i")
            {
                description = "Den: Each DEN has a chance to be replaced instead of each individual creature.\n" +
                    "Inv: Invasion. Original creature is not replaced."
            };
            extrasDescription2 = new OpSimpleButton(new Vector2(487, 543), new Vector2(16, 16), "i")
            {
                description = "\\10: Value is divided by 10. Decimals determine chance of one extra addition.\n" +
                    "Example: 11 becomes 1.1 => 0, 1 or 2 extras; but 2 is very unlikely."
            };

            UIFixed = new UIelement[]
            {
                new OpLabel(10f, 574f, "Options", true),
                new OpLabel(-2f, 543f, "Replacements", true),
                new OpLabel(325f, 543f, "Extra spawns", true),
                replacementDescription,
                extrasDescription,
                replacementDescription2,
                extrasDescription2,
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

            //Set the mod configs
            List<Configurable<int>> enabledModsRepConfigs = new List<Configurable<int>>();
            List<Configurable<int>> enabledModsExtraConfigs = new List<Configurable<int>>();
            SetModConfigs(labelsMap, enabledModsRepConfigs, enabledModsExtraConfigs);

            Configurable<int>[] UIReplacementConfigs = new Configurable<int>[]
            {
                redLizardChance, trainLizardChance, strawberryLizChance, caramelLizChance, cyanLizChance, eelLizChance, 
                leechLizardChance, yeekLizardChance, largeCentipedeChance, redCentipedeChance, seaLeechAquapedeChance, 
                kingVultureChance, mirosVultureChance, eliteScavengerChance, kingScavengerChance, spitterSpiderChance, 
                motherSpiderChance, jungleLeechChance, fireBugChance, brotherLongLegsChance, daddyLongLegsChance, terrorLongLegsChance, 
                hunterLongLegsChance, flyingPredatorChance, waterPredatorChance, giantJellyfishChance, stowawayChance
            };

            Configurable<int>[] UIExtraConfigs = new Configurable<int>[]
            {
                greenLizExtras, pinkLizExtras, blueLizExtras, whiteLizExtras, blackLizExtras, yellowLizExtras, 
                salExtras, cyanLizExtras, caramelLizExtras, eelLizExtras, zoopLizExtras,  
                smallCentExtras, centipedeExtras, centiWingExtras, aquapedeExtras, bigSpiderExtras, dropwigExtras, 
                kelpExtras, leviathanExtras, eggbugExtras, cicadaExtras, 
                lmiceExtras, snailExtras, jetfishExtras, yeekExtras, precycleCreatureExtras, nightCreatureExtras,
                scavengerExtras, vultureExtras, mirosExtras, spiderExtras, leechExtras
            };


            //Adjust scrollbox's size accordingly
            if (enabledModsRepConfigs.Count + UIReplacementConfigs.Length > enabledModsExtraConfigs.Count + UIExtraConfigs.Length)
                scrollBoxSize = 60f +  (enabledModsRepConfigs.Count + UIReplacementConfigs.Length) * 35f;
            else scrollBoxSize = 60f + (enabledModsExtraConfigs.Count + UIExtraConfigs.Length) * 35f;

            int replaceLength = UIReplacementConfigs.Length * 2;
            int extraLength = UIExtraConfigs.Length * 2;

            UIBaseGameOptions = new UIelement[extraLength + replaceLength];
            
            //Set the base replacements
            string auxString;
            for(int i = 0; i < UIReplacementConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(UIReplacementConfigs[i], out auxString);
                UIBaseGameOptions[i*2] = new OpLabel(80f, scrollBoxSize-30f-(35f*i), auxString);
                UIBaseGameOptions[i * 2 + 1] = new OpUpdown(UIReplacementConfigs[i], new Vector2(10f, scrollBoxSize-35f-(35f*i)), 60f);
            }

            //Set the base extras
            for(int i = 0; i < UIExtraConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(UIExtraConfigs[i], out auxString);
                UIBaseGameOptions[replaceLength + i*2] = new OpLabel(400f, scrollBoxSize-30f-(35f*i), auxString);
                UIBaseGameOptions[replaceLength + i*2+1] = new OpUpdown(UIExtraConfigs[i], new Vector2(330f, scrollBoxSize-35f-(35f*i)), 60f);
            }

            //Set the mod-dependant configs
            int modReplaceLength = enabledModsRepConfigs.Count * 2;
            int modExtraLength = enabledModsExtraConfigs.Count * 2;
            UIDependentOptions = new UIelement[modReplaceLength + modExtraLength];

            //Set the mod dependant replacement configs
            for(int i = 0; i < enabledModsRepConfigs.Count; ++i)
            {
                labelsMap.TryGetValue(enabledModsRepConfigs[i], out auxString);
                UIDependentOptions[i*2] = new OpLabel(80f, scrollBoxSize-30f-(35f*(i+UIReplacementConfigs.Length)), auxString);
                UIDependentOptions[i*2+1] = new OpUpdown(enabledModsRepConfigs[i], new Vector2(10f, scrollBoxSize-35f-(35f*(i+UIReplacementConfigs.Length))), 60f);
            }

            //Set the mod dependent extra configs
            for (int i = 0; i < enabledModsExtraConfigs.Count; ++i)
            {
                labelsMap.TryGetValue(enabledModsExtraConfigs[i], out auxString);
                UIDependentOptions[modReplaceLength+i*2] = new OpLabel(400f, scrollBoxSize-30f-(35f*(i+UIExtraConfigs.Length)), auxString);
                UIDependentOptions[modReplaceLength+i*2+1] = new OpUpdown(enabledModsExtraConfigs[i], new Vector2(330f, scrollBoxSize-35f-(35f*(i+UIExtraConfigs.Length))), 60f);
            }

            scrollBox = new OpScrollBox(new Vector2(0f, 55f), new Vector2(580f, 480f), scrollBoxSize, false, false, true);
            opTab.AddItems(UIFixed);
            opTab._AddItem(scrollBox);
            scrollBox.AddItems(UIBaseGameOptions);
            scrollBox.AddItems(UIDependentOptions);

            labelsMap.Clear();
            enabledModsRepConfigs.Clear();
            enabledModsExtraConfigs.Clear();

        }

        public void InitModConfigs()
        {
            HashSet<string> activeMods = apexMod.activeMods;

            if (activeMods.Contains("Sporantula"))
            {
                sporantulaChance = this.config.Bind<int>("SporantulaChance", 4, new ConfigAcceptableRange<int>(0, 100));
                sporantulaExtras = this.config.Bind<int>("ExtraSporantulas", 25, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Angry Inspectors"))
            {
                inspectorChance = this.config.Bind<int>("InspectorChance", 8, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Scutigera"))
            {
                scutigeraChance = this.config.Bind<int>("ScutigeraChance", 15, new ConfigAcceptableRange<int>(0, 100));
                scutigeraExtras = this.config.Bind<int>("ExtraScutigeras", 0, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Red Horror Centipede"))
            {
                redRedHorrorCentiChance = this.config.Bind<int>("RedRedHorrorCentiChance", 10, new ConfigAcceptableRange<int>(0, 100));
                wingRedHorrorCentiChance = this.config.Bind<int>("WingRedHorrorCentiChance", 4, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Explosive DLLs"))
            {
                explosionLongLegsChance = this.config.Bind<int>("ExplosionLongLegsChance", 5, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("More Dlls"))
            {
                mExplosiveLongLegsChance = this.config.Bind<int>("MExplosiveLongLegsChance", 5, new ConfigAcceptableRange<int>(0, 100));
                mZappyLongLegsChance = this.config.Bind<int>("ZappyLongLegsChance", 5, new ConfigAcceptableRange<int>(0, 100));
                
            }

            if (activeMods.Contains("Water Spitter"))
            {
                waterSpitterChance = this.config.Bind<int>("WaterSpitterChance", 10, new ConfigAcceptableRange<int>(0, 100));
                waterSpitterExtras = this.config.Bind<int>("ExtraWaterSpitters", 0, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Fat Firefly"))
            {
                fatFireFlyChance = this.config.Bind<int>("FatFireFlyChance", 10, new ConfigAcceptableRange<int>(0, 100));
               
            }

            if (activeMods.Contains("Sludge Lizard"))
            {
                sludgeLizardChance = this.config.Bind<int>("SludgeLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                snailSludgeLizardChance = this.config.Bind<int>("SnailSludgeLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                sludgeLizardExtras = this.config.Bind<int>("ExtraSludgeLizards", 8, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Lizard Variants"))
            {
                mintLizardChance = this.config.Bind<int>("MintLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                ryanLizardChance = this.config.Bind<int>("RyanLizardChance", 4, new ConfigAcceptableRange<int>(0, 100));
                yellowLimeLizardChance = this.config.Bind<int>("YellowLimeLizardChance", 16, new ConfigAcceptableRange<int>(0, 100));
                mintLizardExtras = this.config.Bind<int>("ExtraSMintLizards", 4, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("The Lizard Mod"))
            {
                lizorInvChance = this.config.Bind<int>("LizorInvChance", 5, new ConfigAcceptableRange<int>(0, 100));
                voltLizardChance = this.config.Bind<int>("voltLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                magentaLizardChance = this.config.Bind<int>("magentaLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                yellowTangerineLizardInvChance = this.config.Bind<int>("yellowtangerineLizardInvChance", 5, new ConfigAcceptableRange<int>(0, 100));
                cyanTangerineLizardInvChance = this.config.Bind<int>("cyantangerineLizardInvChance", 5, new ConfigAcceptableRange<int>(0, 100));
                chameleonLizarcChance = this.config.Bind<int>("chameleonLizarcChance", 5, new ConfigAcceptableRange<int>(0, 100));
                skyBlueLizardChance = this.config.Bind<int>("skyBlueLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                cherryLizardChance = this.config.Bind<int>("cherryLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                strawberryRaspberryLizardChance = this.config.Bind<int>("strawberryRaspberryLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                redRaspberryLizardChance = this.config.Bind<int>("redRaspberryLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                lizorsExtras = this.config.Bind<int>("LizorExtras", 5, new ConfigAcceptableRange<int>(0, 100));
                tangerineLizExtras = this.config.Bind<int>("TangerineLizExtras", 5, new ConfigAcceptableRange<int>(0, 100));
                cherryLizExtras = this.config.Bind<int>("CherryLizExtras", 5, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Surface Swimmer"))
            {
                surfaceSwimmerChance = this.config.Bind<int>("SurfaceSwimmerChance", 20, new ConfigAcceptableRange<int>(0, 100));
                surfaceSwimmerExtras = this.config.Bind<int>("SurfaceSwimmerExtras", 5, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Bouncing Ball"))
            {
                bounceBallChance = this.config.Bind<int>("BouncingBallChance", 10, new ConfigAcceptableRange<int>(0, 100));
                bounceBallExtras = this.config.Bind<int>("BouncingBallExtras", 10, new ConfigAcceptableRange<int>(0, 100));
            }

            if (activeMods.Contains("Rainbow Long Legs"))
            {
                rainbowLongLegsChance = this.config.Bind<int>("RainbowLongLegs", 10, new ConfigAcceptableRange<int>(0, 100));
            }
            
            if(activeMods.Contains("Epic Lizards"))
            {
                brownLizardChance = this.config.Bind<int>("BrownLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                rotzardChance = this.config.Bind<int>("RotzardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                universalLizardChance = this.config.Bind<int>("UniversalLizardChance", 2, new ConfigAcceptableRange<int>(0, 100));
                gildedLizardChance = this.config.Bind<int>("GildedLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                scalizardSchance = this.config.Bind<int>("ScalizardSchance", 10, new ConfigAcceptableRange<int>(0, 100));
                nightmareLizardChance = this.config.Bind<int>("NightmareLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                turquoiseLizardChance = this.config.Bind<int>("TurquoiseLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                amoebaLizardChance = this.config.Bind<int>("AmoebaLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                gargolemLizardChance = this.config.Bind<int>("GargolemLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));

            }

            if (activeMods.Contains("Solace"))
            {
                motherLizardChance = this.config.Bind<int>("MotherLizardChance", 3, new ConfigAcceptableRange<int>(0, 100));
                youngLizardExtras = this.config.Bind<int>("YoungLizardExtras", 0, new ConfigAcceptableRange<int>(0, 100));
                lostYoungLizardChance = this.config.Bind<int>("LostYoungLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                snowSpiderChance = this.config.Bind<int>("SnowSpiderChance", 10, new ConfigAcceptableRange<int>(0, 100));
                snowSpiderExtras = this.config.Bind<int>("SnowSpiderExtras", 4, new ConfigAcceptableRange<int>(0, 100));

            }
        }

        private void SetModConfigs(Dictionary<Configurable<int>, string> labelsMap, List<Configurable<int>> enabledModsRepConfigs, List<Configurable<int>> enabledModsExtraConfigs)
        {
            HashSet<string> activeMods = apexMod.activeMods;

            if (activeMods.Contains("Sporantula"))
            {
                labelsMap.Add(sporantulaChance, "Small Insects > Sporantula (Inv)");
                labelsMap.Add(sporantulaExtras, "Sporantulas (/10)");
                enabledModsRepConfigs.Add(sporantulaChance);
                enabledModsExtraConfigs.Add(sporantulaExtras);
            }

            if (activeMods.Contains("Angry Inspectors"))
            {
                labelsMap.Add(inspectorChance, "LongLegs/??? > Inspector (Inv)");
                enabledModsRepConfigs.Add(inspectorChance);
            }

            if (activeMods.Contains("Scutigera"))
            {
                labelsMap.Add(scutigeraChance, "Centipede > Scutigera");
                labelsMap.Add(scutigeraExtras, "Scutigeras (/10)");
                enabledModsRepConfigs.Add(scutigeraChance);
                enabledModsExtraConfigs.Add(scutigeraExtras);
            }

            if (activeMods.Contains("Red Horror Centipede"))
            {
                labelsMap.Add(redRedHorrorCentiChance, "Red Centipede > Red Horror Centi");
                labelsMap.Add(wingRedHorrorCentiChance, "Centiwing > Red Horror Centi");
                enabledModsRepConfigs.Add(redRedHorrorCentiChance);
                enabledModsRepConfigs.Add(wingRedHorrorCentiChance);
            }

            if (activeMods.Contains("Explosive DLLs"))
            {
                labelsMap.Add(explosionLongLegsChance, "LongLegs > Explosion DLL");
                enabledModsRepConfigs.Add(explosionLongLegsChance);
            }

            if (activeMods.Contains("More Dlls"))
            {
                labelsMap.Add(mExplosiveLongLegsChance, "LongLegs > Explosive DLL");
                labelsMap.Add(mZappyLongLegsChance, "LongLegs > Zappy DLL");
                enabledModsRepConfigs.Add(mExplosiveLongLegsChance);
                enabledModsRepConfigs.Add(mZappyLongLegsChance);
            }

            if (activeMods.Contains("Water Spitter"))
            {
                labelsMap.Add(waterSpitterChance, "Aquatic Lizards > Water Spitter");
                labelsMap.Add(waterSpitterExtras, "Water Spitters (/10)");
                enabledModsRepConfigs.Add(waterSpitterChance);
                enabledModsExtraConfigs.Add(waterSpitterExtras);
            }

            if (activeMods.Contains("Fat Firefly"))
            {
                labelsMap.Add(fatFireFlyChance, "Vultures > Fat Firefly");
                enabledModsRepConfigs.Add(fatFireFlyChance);
            }

            if (activeMods.Contains("Sludge Lizard"))
            {
                labelsMap.Add(sludgeLizardChance, "Water Lizards > Sludge Lizard");
                labelsMap.Add(snailSludgeLizardChance, "Snails > Sludge Lizard (Den)");
                labelsMap.Add(sludgeLizardExtras, "Sludge Lizards (/10)");
                enabledModsRepConfigs.Add(sludgeLizardChance);
                enabledModsRepConfigs.Add(snailSludgeLizardChance);
                enabledModsExtraConfigs.Add(sludgeLizardExtras);

            }

            if (activeMods.Contains("Lizard Variants"))
            {
                labelsMap.Add(mintLizardChance, "Ground Lizards > Mint Lizard");
                labelsMap.Add(ryanLizardChance, "Cyan Lizard > Ryan Lizard");
                labelsMap.Add(yellowLimeLizardChance, "Yellow Lizard > YellowLime Lizard");
                labelsMap.Add(mintLizardExtras, "Mint Lizards (/10)");
                enabledModsRepConfigs.Add(mintLizardChance);
                enabledModsRepConfigs.Add(ryanLizardChance);
                enabledModsRepConfigs.Add(yellowLimeLizardChance);
                enabledModsExtraConfigs.Add(mintLizardExtras);
            }

            if (activeMods.Contains("The Lizard Mod"))
            {
                labelsMap.Add(lizorInvChance, "Lizards > Lizor (Inv)");
                labelsMap.Add(voltLizardChance, "Inspector > Volt Lizard (Inv)");
                labelsMap.Add(magentaLizardChance, "Pink Lizard > Magenta Lizard");
                labelsMap.Add(yellowTangerineLizardInvChance, "Yellow Liz > Tangerine Lizard (Den)");
                labelsMap.Add(cyanTangerineLizardInvChance, "Cyan Liz > Tangerine Lizard (Den)");
                labelsMap.Add(chameleonLizarcChance, "Caramel Liz > Chameleon Lizard");
                labelsMap.Add(skyBlueLizardChance, "Blue Lizard > Sky Blue Lizard");
                labelsMap.Add(cherryLizardChance, "Strawberry Liz > Cherry Lizard");
                labelsMap.Add(strawberryRaspberryLizardChance, "StrawBerry Liz > Raspberry Lizard");
                labelsMap.Add(redRaspberryLizardChance, "Red Lizard > Raspberry Lizard");
                labelsMap.Add(lizorsExtras, "Lizor (/10)");
                labelsMap.Add(tangerineLizExtras, "Tangerine Lizard (/10)");
                labelsMap.Add(cherryLizExtras, "Cherry Lizard (/10)");
                enabledModsRepConfigs.Add(lizorInvChance);
                enabledModsRepConfigs.Add(voltLizardChance);
                enabledModsRepConfigs.Add(magentaLizardChance);
                enabledModsRepConfigs.Add(yellowTangerineLizardInvChance);
                enabledModsRepConfigs.Add(cyanTangerineLizardInvChance);
                enabledModsRepConfigs.Add(chameleonLizarcChance);
                enabledModsRepConfigs.Add(skyBlueLizardChance);
                enabledModsRepConfigs.Add(strawberryRaspberryLizardChance);
                enabledModsRepConfigs.Add(redRaspberryLizardChance);
                enabledModsExtraConfigs.Add(lizorsExtras);
                enabledModsExtraConfigs.Add(tangerineLizExtras);
                enabledModsExtraConfigs.Add(cherryLizExtras);
            }

            if (activeMods.Contains("Surface Swimmer"))
            {
                labelsMap.Add(surfaceSwimmerChance, "EggBug > Surface Swimmer");
                labelsMap.Add(surfaceSwimmerExtras, "Surface Swimmer");
                enabledModsRepConfigs.Add(surfaceSwimmerChance);
                enabledModsExtraConfigs.Add(surfaceSwimmerExtras);
            }

            if (activeMods.Contains("Bouncing Ball"))
            {
                labelsMap.Add(bounceBallChance, "Snail > Bouncing Ball");
                labelsMap.Add(bounceBallExtras, "Bouncing Ball");
                enabledModsRepConfigs.Add(bounceBallChance);
                enabledModsExtraConfigs.Add(bounceBallExtras);
            }

            if(activeMods.Contains("Rainbow Long Legs"))
            {
                labelsMap.Add(rainbowLongLegsChance, "Longlegs > Rainbow Longlegs");
                enabledModsRepConfigs.Add(rainbowLongLegsChance);
            }

            if(activeMods.Contains("Epic Lizards"))
            {
                labelsMap.Add(brownLizardChance, "Small Spiders > Brown Lizard");
                labelsMap.Add(rotzardChance, "Longlegs > Rotzard");
                labelsMap.Add(universalLizardChance, "Lizards > Universal Lizard");
                labelsMap.Add(gildedLizardChance, "Centipedes > Gilded Lizard");
                labelsMap.Add(scalizardSchance, "Centipedes > Scalizard");
                labelsMap.Add(nightmareLizardChance, "Black Lizard > Nightmare Lizard");
                labelsMap.Add(turquoiseLizardChance, "Salamander > Turquoise Lizard");
                labelsMap.Add(amoebaLizardChance, "Strawberry > Amoeba Lizard");
                labelsMap.Add(gargolemLizardChance, "White Lizard > Gargolem Lizard");

                enabledModsRepConfigs.Add(brownLizardChance);
                enabledModsRepConfigs.Add(rotzardChance);
                enabledModsRepConfigs.Add(universalLizardChance);
                enabledModsRepConfigs.Add(gildedLizardChance);
                enabledModsRepConfigs.Add(scalizardSchance);
                enabledModsRepConfigs.Add(nightmareLizardChance);
                enabledModsRepConfigs.Add(turquoiseLizardChance);
                enabledModsRepConfigs.Add(amoebaLizardChance);
                enabledModsRepConfigs.Add(gargolemLizardChance);
            }

            if (activeMods.Contains("Solace"))
            {
                labelsMap.Add(youngLizardExtras, "Young Lizards (/10)");
                labelsMap.Add(lostYoungLizardChance, "Small Lizards > Lost Young Lizard");
                labelsMap.Add(motherLizardChance, "Grounded Lizards > Mother Lizard (+ youngs)");
                labelsMap.Add(snowSpiderChance, "Big Spider > Snow Spider");
                labelsMap.Add(snowSpiderExtras, "Snow Spiders (/10)");

                enabledModsRepConfigs.Add(motherLizardChance);
                enabledModsRepConfigs.Add(lostYoungLizardChance);
                enabledModsRepConfigs.Add(snowSpiderChance);
                enabledModsExtraConfigs.Add(youngLizardExtras);
                enabledModsExtraConfigs.Add(snowSpiderExtras);
            }
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
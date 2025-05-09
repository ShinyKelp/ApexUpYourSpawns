using BepInEx.Logging;
using Menu.Remix.MixedUI;
using RWCustom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ApexUpYourSpawns
{
    public class ApexUpYourSpawnsOptions : OptionInterface
    {
        private readonly ManualLogSource Logger;

        public Configurable<bool> fillLineages;
        public Configurable<bool> forceFreshSpawns;
        public Configurable<bool> balancedSpawns;

        //Replacements
        public Configurable<int> redLizardChance;
        public Configurable<int> redCentipedeChance;
        public Configurable<int> largeCentipedeChance;
        public Configurable<int> mirosVultureChance;
        public Configurable<int> kingVultureChance;
        public Configurable<int> spitterSpiderChance;
        public Configurable<int> trainLizardChance;
        public Configurable<int> cicadaCentiwingChance;
        public Configurable<int> fireBugChance;
        public Configurable<int> eliteScavengerChance;
        public Configurable<int> brotherLongLegsChance;
        public Configurable<int> daddyLongLegsChance;
        public Configurable<int> terrorLongLegsChance;
        public Configurable<int> giantJellyfishChance;
        public Configurable<int> leechLizardChance;
        public Configurable<int> seaLeechAquapedeChance;
        public Configurable<int> jetfishAquapedeChance;
        public Configurable<int> yeekLizardChance;
        public Configurable<int> caramelLizChance;
        public Configurable<int> strawberryLizChance;
        public Configurable<int> cyanLizChance;
        public Configurable<int> eelLizChance;
        public Configurable<int> jungleLeechChance;
        public Configurable<int> motherSpiderChance;
        public Configurable<int> tubeWormSpiderInv;
        public Configurable<int> stowawayChance;
        public Configurable<int> kingScavengerChance;
        public Configurable<int> hunterLongLegsChance;
        public Configurable<int> jetfishSalamanderChance;
        public Configurable<int> cicadaNoodleFlyChance;
        public Configurable<int> monsterKelpChance;

        //Extra spawns
        public Configurable<int> greenLizExtras;
        public Configurable<int> pinkLizExtras;
        public Configurable<int> blueLizExtras;
        public Configurable<int> whiteLizExtras;
        public Configurable<int> blackLizExtras;
        public Configurable<int> yellowLizExtras;
        public Configurable<int> salExtras;
        public Configurable<int> cyanLizExtras;
        public Configurable<int> zoopLizExtras;
        public Configurable<int> caramelLizExtras;
        public Configurable<int> eelLizExtras;
        public Configurable<int> precycleCreatureExtras;
        public Configurable<int> scavengerExtras;
        public Configurable<int> vultureExtras;
        public Configurable<int> centipedeExtras;
        public Configurable<int> centiWingExtras;
        public Configurable<int> aquapedeExtras;
        public Configurable<int> bigSpiderExtras;
        public Configurable<int> dropwigExtras;
        public Configurable<int> eggbugExtras;
        public Configurable<int> cicadaExtras;
        public Configurable<int> lmiceExtras;
        public Configurable<int> snailExtras;
        public Configurable<int> jetfishExtras;
        public Configurable<int> smallCentExtras;
        public Configurable<int> yeekExtras;
        public Configurable<int> mirosExtras;
        public Configurable<int> spiderExtras;
        public Configurable<int> leechExtras;
        public Configurable<int> kelpExtras;
        public Configurable<int> tubeWormExtras;
        public Configurable<int> leviathanExtras;
        public Configurable<int> nightCreatureExtras;
        public Configurable<int> deerExtras;

        //Watcher DLC
        public Configurable<int> scavengerDiscipleChance;
        public Configurable<int> scavengerTemplarChance;
        public Configurable<int> blizzardLizardChance;
        public Configurable<int> mirosLoachChance;
        public Configurable<int> deerLoachInvChance;
        public Configurable<int> loachMirosChance;
        public Configurable<int> rotLoachChance;
        public Configurable<int> vultureBigMothChance;
        public Configurable<int> bigMothVultureChance;
        public Configurable<int> cicadaSmallMothChance;
        public Configurable<int> smallMothNoodleflyChance;
        public Configurable<int> smallMothCentiwingChance;
        public Configurable<int> deerSkywhaleChance;
        public Configurable<int> snailBarnacleChance;
        public Configurable<int> barnacleSnailChance;
        public Configurable<int> blackBasiliskLizChance;
        public Configurable<int> groundIndigoLizChance;
        public Configurable<int> drillCrabMirosChance;
        public Configurable<int> mirosDrillCrabChance;
        public Configurable<int> drillCrabLoachChance;
        public Configurable<int> loachDrillCrabChance;
        public Configurable<int> deerDrillCrabInvChance;

        public Configurable<int> loachExtras;
        public Configurable<int> bigMothExtras;
        public Configurable<int> smallMothExtras;
        public Configurable<int> skywhaleExtras;
        public Configurable<int> basiliskLizExtras;
        public Configurable<int> indigoLizExtras;
        public Configurable<int> barnacleExtras;
        public Configurable<int> drillCrabExtras;


        //Mod dependent
        //Replacements
        public Configurable<int> inspectorChance, sporantulaChance, scutigeraChance, redRedHorrorCentiChance, wingRedHorrorCentiChance,
            mExplosiveLongLegsChance, mZappyLongLegsChance, waterSpitterChance, fatFireFlyChance,
            mintLizardChance, ryanLizardChance, yellowLimeLizardChance, surfaceSwimmerChance, bounceBallChance,
            motherLizardChance, lostYoungLizardChance, snowSpiderChance,
            outspectorChance, inspectorOutspectorInvChance, icyBlueFreezerInvChance,
            icyBlueBlueChance, icyBlueYellowChance, freezerLizChance, cyanwingChance, wingCyanwingChance,
            jetfishBabyAquapedeChance, babyAquapedeInvChance, aquapedeBabyAquaChance, nightTerrorChance, brotherLittleLongLegChance,
            critterLittleLongLegsChance, critterHoverflyChance, drainMiteChance, bombVultureChance, toxicSpiderChance,
            fatNootChance, scroungerChance, bllMimicstarfishChance, critterMimicstarfishChance, chillipedeChance, noodleEaterChance, thornbugChance, miniLeviathanChance,
            polliwogChance, silverLizChance, hunterSeekerWhiteChance, hunterSeekerCyanChance, centiElectricLizChance,
            lizardElectricLizChance, albinoVultureChance, vultureEchoLeviChance, spiderWeaverChance, sSwimmerWeaverChance, blizzorChance, salamanderSalamoleChance,
            blackSalamolechance;

        //Extras
        public Configurable<int> sporantulaExtras, scutigeraExtras, waterSpitterExtras, mintLizardExtras,
            surfaceSwimmerExtras, bounceBallExtras, snowSpiderExtras,
            youngLizardExtras, outspectorExtras, icyBlueLizExtras, babyAquapedeExtras,
            littleLongLegsExtras, hoverflyExtras, drainMiteExtras, scroungerExtras, noodleEaterExtras, thornbugExtras, miniLeviathanExtras,
            polliwogExtras, silverLizExtras, hunterSeekerExtras, echoLeviExtras, bubbleWeaverExtras, blizzorExtras, salamoleExtras;

        private Configurable<string> comboBoxConfig, presetTextConfig;

        private OpSimpleButton replacementDescription, extrasDescription, replacementDescription2, extrasDescription2,
            savePresetButton, loadPresetButton, removePresetButton;
        private OpCheckBox fillLineageCheck, forceFreshCheck, balancedSpawnsCheck;
        private OpTextBox presetText;
        private OpComboBox presetsComboBox;
        private UIelement[] UIFixed;
        private Dictionary<string, OpUpdown> optionsRefs;

        private ApexUpYourSpawnsMod apexMod;
        private bool appliedSharedDLC = false;
        private bool appliedMSC = false;
        private bool appliedWatcher = false;

        public ApexUpYourSpawnsOptions(ApexUpYourSpawnsMod modInstance, ManualLogSource loggerSource)
        {
            Logger = loggerSource;
            apexMod = modInstance;

            optionsRefs = new Dictionary<string, OpUpdown>();

            fillLineages = this.config.Bind<bool>("FillLineages", false);
            forceFreshSpawns = this.config.Bind<bool>("ForceFreshSpawns", false);
            balancedSpawns = this.config.Bind<bool>("BalancedSpawns", true);

            ConfigurableInfo info = null;
            comboBoxConfig = this.config.Bind<string>("PresetComboBox", "Default", info);
            presetTextConfig = this.config.Bind<string>(null, "", info);

            //Vanilla configs
            //Replacements
            redLizardChance = this.config.Bind<int>("RedLizardChance", 6, new ConfigAcceptableRange<int>(0, 100));
            largeCentipedeChance = this.config.Bind<int>("LargeCentipedeChance", 0, new ConfigAcceptableRange<int>(0, 100));
            redCentipedeChance = this.config.Bind<int>("RedCentipedeChance", 8, new ConfigAcceptableRange<int>(0, 100));
            kingVultureChance = this.config.Bind<int>("KingVultureChance", 10, new ConfigAcceptableRange<int>(0, 100));
            spitterSpiderChance = this.config.Bind<int>("SpitterSpiderChance", 15, new ConfigAcceptableRange<int>(0, 100));
            cicadaCentiwingChance = this.config.Bind<int>("FlyingPredatorChance", 12, new ConfigAcceptableRange<int>(0, 100));
            brotherLongLegsChance = this.config.Bind<int>("BrotherLongLegsChance", 4, new ConfigAcceptableRange<int>(0, 100));
            daddyLongLegsChance = this.config.Bind<int>("DaddyLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
            leechLizardChance = this.config.Bind<int>("LeechLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
            cyanLizChance = this.config.Bind<int>("CyanLizChance", 0, new ConfigAcceptableRange<int>(0, 100));
            tubeWormSpiderInv = this.config.Bind<int>("TubeWormSpiderInv", 20, new ConfigAcceptableRange<int>(0, 100));
            jetfishSalamanderChance = this.config.Bind<int>("JetFishSalamanderChance", 6, new ConfigAcceptableRange<int>(0, 100));
            cicadaNoodleFlyChance = this.config.Bind<int>("CicadaNoodleflyChance", 4, new ConfigAcceptableRange<int>(0, 100));
            monsterKelpChance = this.config.Bind<int>("MonsterKelpChance", 5, new ConfigAcceptableRange<int>(0, 100));

            //Extras
            greenLizExtras = this.config.Bind<int>("ExtraGreenLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            blueLizExtras = this.config.Bind<int>("ExtraBlueLizs", 6, new ConfigAcceptableRange<int>(0, 100));
            pinkLizExtras = this.config.Bind<int>("ExtraPinkLizs", 8, new ConfigAcceptableRange<int>(0, 100));
            whiteLizExtras = this.config.Bind<int>("ExtraWhiteLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            blackLizExtras = this.config.Bind<int>("ExtraBlackLizs", 4, new ConfigAcceptableRange<int>(0, 100));
            yellowLizExtras = this.config.Bind<int>("ExtraYellowLizs", 25, new ConfigAcceptableRange<int>(0, 100));
            salExtras = this.config.Bind<int>("ExtraSals", 12, new ConfigAcceptableRange<int>(0, 100));
            cyanLizExtras = this.config.Bind<int>("ExtraCyanLizs", 2, new ConfigAcceptableRange<int>(0, 100));
            precycleCreatureExtras = this.config.Bind<int>("ExtraPrecycleCreatures", 10, new ConfigAcceptableRange<int>(0, 100));
            scavengerExtras = this.config.Bind<int>("ExtraScavengers", 4, new ConfigAcceptableRange<int>(0, 20));
            vultureExtras = this.config.Bind<int>("ExtraVultures", 1, new ConfigAcceptableRange<int>(0, 20));
            centipedeExtras = this.config.Bind<int>("ExtraCentipedes", 3, new ConfigAcceptableRange<int>(0, 100));
            centiWingExtras = this.config.Bind<int>("ExtraCentiwings", 0, new ConfigAcceptableRange<int>(0, 100));
            bigSpiderExtras = this.config.Bind<int>("ExtraBigSpiders", 25, new ConfigAcceptableRange<int>(0, 100));
            dropwigExtras = this.config.Bind<int>("ExtraDropwigs", 10, new ConfigAcceptableRange<int>(0, 100));
            eggbugExtras = this.config.Bind<int>("ExtraEggbugs", 10, new ConfigAcceptableRange<int>(0, 100));
            cicadaExtras = this.config.Bind<int>("ExtraCicadas", 10, new ConfigAcceptableRange<int>(0, 100));
            snailExtras = this.config.Bind<int>("ExtraSnails", 3, new ConfigAcceptableRange<int>(0, 100));
            jetfishExtras = this.config.Bind<int>("ExtraJetfish", 6, new ConfigAcceptableRange<int>(0, 100));
            lmiceExtras = this.config.Bind<int>("ExtraLMice", 3, new ConfigAcceptableRange<int>(0, 100));
            smallCentExtras = this.config.Bind<int>("ExtraSmallCents", 8, new ConfigAcceptableRange<int>(0, 100));
            mirosExtras = this.config.Bind<int>("ExtraMirosBirds", 2, new ConfigAcceptableRange<int>(0, 20));
            spiderExtras = this.config.Bind<int>("ExtraSpiders", 0, new ConfigAcceptableRange<int>(0, 20));
            leechExtras = this.config.Bind<int>("ExtraLeeches", 0, new ConfigAcceptableRange<int>(0, 20));
            tubeWormExtras = this.config.Bind<int>("ExtraTubeworms", 4, new ConfigAcceptableRange<int>(0, 20));
            kelpExtras = this.config.Bind<int>("ExtraKelps", 6, new ConfigAcceptableRange<int>(0, 100));
            leviathanExtras = this.config.Bind<int>("ExtraLeviathans", 4, new ConfigAcceptableRange<int>(0, 100));
            nightCreatureExtras = this.config.Bind<int>("ExtraNightCreatures", 10, new ConfigAcceptableRange<int>(0, 100));
            deerExtras = this.config.Bind<int>("ExtraDeer", 2, new ConfigAcceptableRange<int>(0, 20));
        }

        public void SetExpansionBindings()
        {   

            //Shared DLC
            if((ModManager.Watcher || ModManager.MSC) && !appliedSharedDLC)
            {
                appliedSharedDLC = true;
                mirosVultureChance = this.config.Bind<int>("MirosVultureChance", 15, new ConfigAcceptableRange<int>(0, 100));
                eliteScavengerChance = this.config.Bind<int>("EliteScavengerChance", 12, new ConfigAcceptableRange<int>(0, 100));
                terrorLongLegsChance = this.config.Bind<int>("MotherLongLegsChance", 10, new ConfigAcceptableRange<int>(0, 100));
                seaLeechAquapedeChance = this.config.Bind<int>("SeaLeechAquapedeChance", 10, new ConfigAcceptableRange<int>(0, 100));
                jetfishAquapedeChance = this.config.Bind<int>("AquapedeChance", 20, new ConfigAcceptableRange<int>(0, 100));
                yeekLizardChance = this.config.Bind<int>("YeekLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                caramelLizChance = this.config.Bind<int>("CaramelLizChance", 15, new ConfigAcceptableRange<int>(0, 100));
                strawberryLizChance = this.config.Bind<int>("StrawberryLizChance", 5, new ConfigAcceptableRange<int>(0, 100));
                eelLizChance = this.config.Bind<int>("EelLizChance", 10, new ConfigAcceptableRange<int>(0, 100));
                jungleLeechChance = this.config.Bind<int>("JungleLeechChance", 0, new ConfigAcceptableRange<int>(0, 100));
                motherSpiderChance = this.config.Bind<int>("MotherSpiderChance", 10, new ConfigAcceptableRange<int>(0, 100));
                stowawayChance = this.config.Bind<int>("StowawayChance", 3, new ConfigAcceptableRange<int>(0, 100));
                giantJellyfishChance = this.config.Bind<int>("GiantJellyfishChance", 10, new ConfigAcceptableRange<int>(0, 100));

                caramelLizExtras = this.config.Bind<int>("ExtraCaramelLizs", 4, new ConfigAcceptableRange<int>(0, 100));
                zoopLizExtras = this.config.Bind<int>("ExtraZoopLizs", 8, new ConfigAcceptableRange<int>(0, 100));
                eelLizExtras = this.config.Bind<int>("ExtraEelLizs", 4, new ConfigAcceptableRange<int>(0, 100));
                aquapedeExtras = this.config.Bind<int>("ExtraAquaPedes", 5, new ConfigAcceptableRange<int>(0, 100));
                yeekExtras = this.config.Bind<int>("ExtraYeek", 8, new ConfigAcceptableRange<int>(0, 100));
            }

            //MSC
            if (ModManager.MSC && !appliedMSC){
                appliedMSC = true;
                trainLizardChance = this.config.Bind<int>("TrainLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                fireBugChance = this.config.Bind<int>("FireBugChance", 30, new ConfigAcceptableRange<int>(0, 100));
                kingScavengerChance = this.config.Bind<int>("KingScavengerChance", 5, new ConfigAcceptableRange<int>(0, 100));
                hunterLongLegsChance = this.config.Bind<int>("HunterLongLegsChance", 20, new ConfigAcceptableRange<int>(0, 100));
            }

            //Watcher
            if (ModManager.Watcher && !appliedWatcher)
            {
                appliedWatcher = true;
                scavengerDiscipleChance = this.config.Bind<int>("ScavengerDiscipleChance", 4, new ConfigAcceptableRange<int>(0, 100));
                scavengerTemplarChance = this.config.Bind<int>("ScavengerTemplarChance", 10, new ConfigAcceptableRange<int>(0, 100));
                blizzardLizardChance = this.config.Bind<int>("BlizzardLizardChance", 2, new ConfigAcceptableRange<int>(0, 100));
                mirosLoachChance = this.config.Bind<int>("MirosLoachChance", 20, new ConfigAcceptableRange<int>(0, 100));
                deerLoachInvChance = this.config.Bind<int>("DeerLoachInvChance", 5, new ConfigAcceptableRange<int>(0, 100));
                loachMirosChance = this.config.Bind<int>("LoachMirosChance", 0, new ConfigAcceptableRange<int>(0, 100));
                rotLoachChance = this.config.Bind<int>("RotLoachChance", 10, new ConfigAcceptableRange<int>(0, 100));
                vultureBigMothChance = this.config.Bind<int>("VultureBigMothChance", 12, new ConfigAcceptableRange<int>(0, 100));
                bigMothVultureChance = this.config.Bind<int>("BigMothVultureChance", 0, new ConfigAcceptableRange<int>(0, 100));
                cicadaSmallMothChance = this.config.Bind<int>("CicadaSmallMothChance", 15, new ConfigAcceptableRange<int>(0, 100));
                smallMothNoodleflyChance = this.config.Bind<int>("SmallMothNoodleflyChance", 0, new ConfigAcceptableRange<int>(0, 100));
                smallMothCentiwingChance = this.config.Bind<int>("SmallMothCentiwingChance", 0, new ConfigAcceptableRange<int>(0, 100));
                deerSkywhaleChance = this.config.Bind<int>("DeerSkywhaleChance", 15, new ConfigAcceptableRange<int>(0, 100));
                snailBarnacleChance = this.config.Bind<int>("SnailBarnacleChance", 30, new ConfigAcceptableRange<int>(0, 100));
                barnacleSnailChance = this.config.Bind<int>("BarnacleSnailChance", 8, new ConfigAcceptableRange<int>(0, 100));
                blackBasiliskLizChance = this.config.Bind<int>("BlackBasiliskLizChance", 10, new ConfigAcceptableRange<int>(0, 100));
                groundIndigoLizChance = this.config.Bind<int>("GroundIndigoLizChance", 0, new ConfigAcceptableRange<int>(0, 100));
                drillCrabMirosChance = this.config.Bind<int>("DrillCrabMirosChance", 0, new ConfigAcceptableRange<int>(0, 100));
                mirosDrillCrabChance = this.config.Bind<int>("MirosDrillCrabChance", 25, new ConfigAcceptableRange<int>(0, 100));
                drillCrabLoachChance = this.config.Bind<int>("DrillCrabLoachChance", 0, new ConfigAcceptableRange<int>(0, 100));
                loachDrillCrabChance = this.config.Bind<int>("LoachDrillCrabChance", 0, new ConfigAcceptableRange<int>(0, 100));
                deerDrillCrabInvChance = this.config.Bind<int>("DeerDrillCrabChance", 18, new ConfigAcceptableRange<int>(0, 100));

                loachExtras = this.config.Bind<int>("LoachExtras", 2, new ConfigAcceptableRange<int>(0, 20));
                bigMothExtras = this.config.Bind<int>("BigMothExtras", 3, new ConfigAcceptableRange<int>(0, 100));
                smallMothExtras = this.config.Bind<int>("SmallMothExtras", 6, new ConfigAcceptableRange<int>(0, 100));
                basiliskLizExtras = this.config.Bind<int>("BasiliskLizExtras", 4, new ConfigAcceptableRange<int>(0, 100));
                indigoLizExtras = this.config.Bind<int>("IndigoLizExtras", 8, new ConfigAcceptableRange<int>(0, 100));
                barnacleExtras = this.config.Bind<int>("BarnacleExtras", 0, new ConfigAcceptableRange<int>(0, 100));
                drillCrabExtras = this.config.Bind<int>("DrillCrabExtras", 3, new ConfigAcceptableRange<int>(0, 20));
                skywhaleExtras = this.config.Bind<int>("SkywhaleExtras", 2, new ConfigAcceptableRange<int>(0, 20));
            }
        }

        private void SetVanillaDictionary(Dictionary<Configurable<int>, string> labelsMap)
        {
            //Replacements
            labelsMap.Add(redLizardChance, "Lizard > Red Lizard");
            labelsMap.Add(largeCentipedeChance, "Small Cent > Large Centipede");
            labelsMap.Add(redCentipedeChance, "Large Cent > Red Centipede");
            labelsMap.Add(kingVultureChance, "Vulture > King Vulture");
            labelsMap.Add(cicadaCentiwingChance, "Cicada > Centiwing");
            labelsMap.Add(cicadaNoodleFlyChance, "Cicada > Noodlefly");
            labelsMap.Add(brotherLongLegsChance, "L.Mice/Snail/??? > LongLegs (Den)");
            labelsMap.Add(daddyLongLegsChance, "Brother > DaddyLongLegs");
            labelsMap.Add(leechLizardChance, "Leeches > Aquatic Lizard (Den)");
            labelsMap.Add(jetfishSalamanderChance, "Jetfish > Salamander");
            labelsMap.Add(cyanLizChance, "Blue > Cyan Lizard");
            labelsMap.Add(spitterSpiderChance, "Big Spider > Spitter Spider");
            labelsMap.Add(tubeWormSpiderInv, "Grappleworm > Big Spider (Inv)");
            labelsMap.Add(monsterKelpChance, "Pole Plant > Monster Kelp");

            //Extras
            labelsMap.Add(yellowLizExtras, "Yellow Lizards (/10)");
            labelsMap.Add(blueLizExtras, "Blue Lizards (/10)");
            labelsMap.Add(greenLizExtras, "Green Lizards (/10)");
            labelsMap.Add(pinkLizExtras, "Pink Lizards (/10)");
            labelsMap.Add(whiteLizExtras, "White Lizards (/10)");
            labelsMap.Add(blackLizExtras, "Black Lizards (/10)");
            labelsMap.Add(salExtras, "Salamanders (/10)");
            labelsMap.Add(cyanLizExtras, "Cyan Lizards (/10)");
            labelsMap.Add(precycleCreatureExtras, "Shelter Failure Spawns (/10)");
            labelsMap.Add(scavengerExtras, "Scavengers");
            labelsMap.Add(vultureExtras, "Vultures");
            labelsMap.Add(centipedeExtras, "Centipedes (/10)");
            labelsMap.Add(centiWingExtras, "Centiwings (/10)");
            labelsMap.Add(bigSpiderExtras, "Big Spiders (/10)");
            labelsMap.Add(dropwigExtras, "Dropwigs (/10)");
            labelsMap.Add(eggbugExtras, "Eggbugs (/10)");
            labelsMap.Add(cicadaExtras, "Cicadas (/10)");
            labelsMap.Add(smallCentExtras, "Small Centipedes (/10)");
            labelsMap.Add(snailExtras, "Snails (/10)");
            labelsMap.Add(jetfishExtras, "Jetfish (/10)");
            labelsMap.Add(lmiceExtras, "Lantern Mice (/10)");
            labelsMap.Add(mirosExtras, "Miros Birds");
            labelsMap.Add(spiderExtras, "Small Spiders");
            labelsMap.Add(leechExtras, "Leeches");
            labelsMap.Add(tubeWormExtras, "Grappleworms (/10)");
            labelsMap.Add(kelpExtras, "Monster Kelp (/10)");
            labelsMap.Add(leviathanExtras, "Leviathans (/10)");
            labelsMap.Add(nightCreatureExtras, "Night Creatures (/10)");
            labelsMap.Add(deerExtras, "Deer");

            if (ModManager.MSC || ModManager.Watcher)
            {
                labelsMap.Add(mirosVultureChance, "Vultures > Miros Vulture");
                labelsMap.Add(eliteScavengerChance, "Scavenger > Elite Scavenger");
                labelsMap.Add(terrorLongLegsChance, "Daddy > MotherLongLegs");
                labelsMap.Add(giantJellyfishChance, "Jellyfish > Giant Jellyfish (*)");
                labelsMap.Add(strawberryLizChance, "Pink > Strawberry Lizard");
                labelsMap.Add(caramelLizChance, "Green > Caramel Lizard");
                labelsMap.Add(eelLizChance, "Salamander > Eel Lizard");
                labelsMap.Add(motherSpiderChance, "Small Spiders > Mother Spider (Den)");
                labelsMap.Add(jungleLeechChance, "Leech > Jungle Leech");
                labelsMap.Add(stowawayChance, "Ceiling Fruits > Stowawaybug Trap (*)");
                labelsMap.Add(seaLeechAquapedeChance, "Sea Leeches > Aquapede (Den)");
                labelsMap.Add(yeekLizardChance, "Yeek > Caramel/Strawberry Liz");
                labelsMap.Add(jetfishAquapedeChance, "Jetfish > Aquapede");

                labelsMap.Add(caramelLizExtras, "Caramel Lizards (/10)");
                labelsMap.Add(eelLizExtras, "Eel Lizards (/10)");
                labelsMap.Add(zoopLizExtras, "Strawberry Lizards (/10)");
                labelsMap.Add(aquapedeExtras, "Aquapedes (/10)");
                labelsMap.Add(yeekExtras, "Yeeks (/10)");
            }

            //MSC
            if (ModManager.MSC)
            {
                labelsMap.Add(trainLizardChance, "Red Lizard > Train Lizard");
                labelsMap.Add(fireBugChance, "Eggbug > Firebug");
                labelsMap.Add(kingScavengerChance, "Elite > King Scavenger (*)");
                labelsMap.Add(hunterLongLegsChance, "Slugpup > HunterLongLegs (*)");
            }

            //Watcher
            if (ModManager.Watcher)
            {
                labelsMap.Add(scavengerDiscipleChance, "Scavenger > Disciple");
                labelsMap.Add(scavengerTemplarChance, "Scavenger > Templar");
                labelsMap.Add(blizzardLizardChance, "Lizards > Blizzard");
                labelsMap.Add(mirosLoachChance, "Miros Bird > Loach (Den)");
                labelsMap.Add(deerLoachInvChance, "Deer > Loach (Inv)");
                labelsMap.Add(loachMirosChance, "Loach > Miros Bird (Den)");
                labelsMap.Add(rotLoachChance, "Loach > Rot Loach");
                labelsMap.Add(vultureBigMothChance, "Vulture > Big Moth");
                labelsMap.Add(bigMothVultureChance, "Big Moth > Vulture");
                labelsMap.Add(cicadaSmallMothChance, "Cicada > Small Moth");
                labelsMap.Add(smallMothNoodleflyChance, "Small Moth > Noodlefly");
                labelsMap.Add(smallMothCentiwingChance, "Small Moth > Centiwing");
                labelsMap.Add(deerSkywhaleChance, "Deer > Skywhale");
                labelsMap.Add(snailBarnacleChance, "Snail > Barnacle");
                labelsMap.Add(barnacleSnailChance, "Barnacle > Snail");
                labelsMap.Add(blackBasiliskLizChance, "Black Lizard > Basilisk Lizard");
                labelsMap.Add(groundIndigoLizChance, "Grounded Lizards > Indigo lizard");
                labelsMap.Add(drillCrabMirosChance, "Drill Crab > Miros Bird (Den)");
                labelsMap.Add(mirosDrillCrabChance, "Miros Bird > Drill Crab (Den)");
                labelsMap.Add(drillCrabLoachChance, "Drill Crab > Loach (Den)");
                labelsMap.Add(loachDrillCrabChance, "Loach > Drill Crab (Den)");
                labelsMap.Add(deerDrillCrabInvChance, "Deer > Drill Crab (Inv)");
                labelsMap.Add(loachExtras, "Loaches");
                labelsMap.Add(bigMothExtras, "Big Moths");
                labelsMap.Add(smallMothExtras, "Small Moths (/10)");
                labelsMap.Add(basiliskLizExtras, "Basilisk Lizards (/10)");
                labelsMap.Add(indigoLizExtras, "Indigo Lizards  (/10)");
                labelsMap.Add(barnacleExtras, "Barnacles  (/10)");
                labelsMap.Add(drillCrabExtras, "Drill Crabs");
                labelsMap.Add(skywhaleExtras, "Skywhales");
            }
        }

        public override void Initialize()
        {
            // === CREATE BASE UI ELEMENTS
            optionsRefs.Clear();
            List<ListItem> boxList = new List<ListItem>();
            ListItem def = new ListItem("Default");
            ListItem nulls = new ListItem("Nulls");

            boxList.Add(def);
            boxList.Add(nulls);

            if (Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
            {
                foreach (string filename in Directory.GetFiles(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
                {
                    string[] splitted = filename.Split('\\');
                    string aloneName = splitted[splitted.Length - 1].Split('.')[0];
                    boxList.Add(new ListItem(aloneName));
                }
            }
            else
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns");

            if(!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates");


            Vector2 presetsPos = new Vector2(220, 535);

            presetsComboBox = new OpComboBox(comboBoxConfig, presetsPos, 90, boxList);
            presetsPos.y += 32;
            presetText = new OpTextBox(presetTextConfig, presetsPos, 90f);

            presetsPos.x -= 95f;
            presetsPos.y -= 2f;

            savePresetButton = new OpSimpleButton(presetsPos, new Vector2(88, 28), "SAVE PRESET")
            {
                description = "Save preset (if text is empty, saves currently selected one)"
            };

            presetsPos.y -= 32f;
            loadPresetButton = new OpSimpleButton(presetsPos, new Vector2(88, 28), "LOAD PRESET")
            {
                description = "Load selected preset"
            };

            presetsPos.y += 16;
            presetsPos.x += 204;
            removePresetButton = new OpSimpleButton(presetsPos, new Vector2(108, 30), "REMOVE PRESET")
            {
                description = "Remove selected preset"
            };
            loadPresetButton.OnClick += LoadPreset;
            savePresetButton.OnClick += SavePreset;
            removePresetButton.OnClick += RemovePreset;

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


            replacementDescription = new OpSimpleButton(new Vector2(137, 483), new Vector2(16, 16), "i")
            {
                description = "% chance to replace a creature with another.\n" + 
                "*: Creature is not from a spawner. Replacements vary between cycles."
            };
            extrasDescription = new OpSimpleButton(new Vector2(457, 483), new Vector2(16, 16), "i")
            {
                description = "Increase the creature amount of each den by random value from 0 to X.\n" + 
                "Does not apply to lineages."
            };

            replacementDescription2 = new OpSimpleButton(new Vector2(167, 483), new Vector2(16, 16), "i")
            {
                description = "Den: Each DEN has a chance to be replaced instead of each individual creature.\n" +
                    "Inv: Invasion. Original creature is not replaced."
            };
            extrasDescription2 = new OpSimpleButton(new Vector2(487, 483), new Vector2(16, 16), "i")
            {
                description = "\\10: Value is divided by 10. Decimals determine chance of one extra addition.\n" +
                    "Example: 11 becomes 1.1 => 0, 1 or 2 extras; but 2 is very unlikely."
            };

            UIFixed = new UIelement[]
            {
                new OpLabel(10f, 524f, "Options", true),
                new OpLabel(-2f, 483f, "Replacements", true),
                new OpLabel(325f, 483f, "Extra spawns", true),
                replacementDescription,
                extrasDescription,
                replacementDescription2,
                extrasDescription2,
                fillLineageCheck,
                new OpLabel(187f, 16f, "Fill Lineages"),
                balancedSpawnsCheck,
                new OpLabel(289f, 16f, "Balanced spawns"),
                forceFreshCheck,
                new OpLabel(421f, 16f, "Randomize every cycle"),
                presetsComboBox,
                presetText,
                loadPresetButton,
                savePresetButton,
                removePresetButton
            };

            //Set the base game configs
            SetExpansionBindings();
            Dictionary<Configurable<int>, string> labelsMap = new Dictionary<Configurable<int>, string>();
            SetVanillaDictionary(labelsMap);

            //TABS
            List<OpTab> tabs = new List<OpTab>();

            //VANILLA TAB

            Configurable<int>[] UIVanillaReplacementConfigs = new Configurable<int>[]
            {
                redLizardChance, cyanLizChance,
                leechLizardChance, largeCentipedeChance, redCentipedeChance,
                kingVultureChance,
                cicadaNoodleFlyChance, cicadaCentiwingChance, jetfishSalamanderChance,
                spitterSpiderChance,  tubeWormSpiderInv,
                brotherLongLegsChance, daddyLongLegsChance, monsterKelpChance,

            };

            Configurable<int>[] UIVanillaExtraConfigs = new Configurable<int>[]
            {
                greenLizExtras, pinkLizExtras, blueLizExtras, whiteLizExtras, blackLizExtras, yellowLizExtras,
                salExtras, cyanLizExtras,
                smallCentExtras, centipedeExtras, centiWingExtras,  bigSpiderExtras, dropwigExtras,
                kelpExtras, eggbugExtras, cicadaExtras,
                lmiceExtras, snailExtras, jetfishExtras, tubeWormExtras, leviathanExtras, precycleCreatureExtras, nightCreatureExtras,
                scavengerExtras, vultureExtras, mirosExtras, spiderExtras, leechExtras, deerExtras
            };

            OpTab vanillaTab = CreateTab(UIVanillaReplacementConfigs, UIVanillaExtraConfigs, labelsMap, " Vanilla");
            tabs.Add(vanillaTab);

            //MSC / DLC TAB
            if(ModManager.MSC || ModManager.Watcher)
            {
                Configurable<int>[] UIDLCExtraConfigs = new Configurable<int>[]
                {
                    caramelLizExtras, eelLizExtras, zoopLizExtras,
                    aquapedeExtras,
                    yeekExtras,
                };


                List<Configurable<int>> dlcRepConfigsList = new List<Configurable<int>>
                {
                    strawberryLizChance, caramelLizChance, eelLizChance, yeekLizardChance,
                    jetfishAquapedeChance, seaLeechAquapedeChance,
                    motherSpiderChance,
                    jungleLeechChance,
                    mirosVultureChance,
                    eliteScavengerChance,
                    terrorLongLegsChance,
                    giantJellyfishChance,
                    stowawayChance
                };

                List<Configurable<int>> mscRepConfigList = new List<Configurable<int>>
                {
                    trainLizardChance,
                    fireBugChance,
                    hunterLongLegsChance
                };
                if (apexMod.activeMods.Contains("ShinyKelp.ScavengerTweaks"))
                {
                    mscRepConfigList.Add(kingScavengerChance);
                }

                if (ModManager.MSC)
                    dlcRepConfigsList.AddRange(mscRepConfigList);

                Configurable<int>[] UIDLCReplacementConfigs = dlcRepConfigsList.ToArray();

                OpTab mscTab = CreateTab(UIDLCReplacementConfigs, UIDLCExtraConfigs, labelsMap, ModManager.MSC ? " More Slugcats" : " DLC Expansion");
                tabs.Add(mscTab);
            }

            //WATCHER TAB
            if (ModManager.Watcher)
            {
                Configurable<int>[] UIWatcherRepConfigs =
                {
                    blackBasiliskLizChance,
                    groundIndigoLizChance,
                    blizzardLizardChance,
                    scavengerTemplarChance,
                    scavengerDiscipleChance,
                    vultureBigMothChance,
                    bigMothVultureChance,
                    cicadaSmallMothChance,
                    smallMothNoodleflyChance,
                    smallMothCentiwingChance,
                    snailBarnacleChance,
                    barnacleSnailChance,
                    deerSkywhaleChance,
                    deerDrillCrabInvChance,
                    deerLoachInvChance,
                    rotLoachChance,
                    drillCrabMirosChance,
                    mirosDrillCrabChance,
                    drillCrabLoachChance,
                    loachDrillCrabChance,
                    loachMirosChance,
                    mirosLoachChance,
                };

                Configurable<int>[] UIWatcherExtraConfigs =
                {
                    basiliskLizExtras,
                    indigoLizExtras,
                    bigMothExtras,
                    smallMothExtras,
                    drillCrabExtras,
                    loachExtras,
                    barnacleExtras,
                    skywhaleExtras
                };

                OpTab watcherTab = CreateTab(UIWatcherRepConfigs, UIWatcherExtraConfigs, labelsMap, " The Watcher");
                tabs.Add(watcherTab);
            }
            
            //MODS TAB
            //Set the mod configs
            List<Configurable<int>> enabledModsRepConfigs = new List<Configurable<int>>();
            List<Configurable<int>> enabledModsExtraConfigs = new List<Configurable<int>>();
            SetModConfigs(labelsMap, enabledModsRepConfigs, enabledModsExtraConfigs);

            if(enabledModsExtraConfigs.Count > 0 || enabledModsRepConfigs.Count > 0)
            {
                OpTab modsTab = CreateTab(enabledModsRepConfigs.ToArray(), enabledModsExtraConfigs.ToArray(), labelsMap, " Mods");
                tabs.Add(modsTab);
            }

            //END

            this.Tabs = tabs.ToArray();

            labelsMap.Clear();
            enabledModsRepConfigs.Clear();
            enabledModsExtraConfigs.Clear();
            
        }

        private OpTab CreateTab(Configurable<int>[] replacementConfigs, Configurable<int>[] extraConfigs, Dictionary<Configurable<int>, string> labelsMap, string tabName)
        {
            string auxString;
            float scrollBoxSize = 60f + 35f * (Mathf.Max(replacementConfigs.Length, extraConfigs.Length));
            int replaceLength = replacementConfigs.Length * 2;
            int extraLength = extraConfigs.Length * 2;

            UIelement[] totalElements = new UIelement[extraLength + replaceLength];

            //Replacements
            for (int i = 0; i < replacementConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(replacementConfigs[i], out auxString);
                totalElements[i * 2] = new OpLabel(80f, scrollBoxSize - 30f - (35f * i), auxString);
                totalElements[i * 2 + 1] = new OpUpdown(replacementConfigs[i], new Vector2(10f, scrollBoxSize - 35f - (35f * i)), 60f);
            }

            //Extras
            for (int i = 0; i < extraConfigs.Length; ++i)
            {
                labelsMap.TryGetValue(extraConfigs[i], out auxString);
                totalElements[replaceLength + i * 2] = new OpLabel(400f, scrollBoxSize - 30f - (35f * i), auxString);
                totalElements[replaceLength + i * 2 + 1] = new OpUpdown(extraConfigs[i], new Vector2(330f, scrollBoxSize - 35f - (35f * i)), 60f);
            }


            OpScrollBox scrollBox = new OpScrollBox(new Vector2(0f, 55f), new Vector2(580f, 420f), scrollBoxSize, false, false, true);
            OpTab tab = new OpTab(this, tabName);
            tab._AddItem(scrollBox);
            scrollBox.AddItems(totalElements);
            foreach (UIelement u in tab.items)
                if (u is OpUpdown op)
                    optionsRefs.Add(op.Key, op);

            
            tab.OnPreActivate += () => {
                tab.AddItems(UIFixed);
            };
            //NOTE: Do not remove items on PostDeactivate, it causes reload bug. (The game does it automatically)
            return tab;
        }

        public void InitModConfigs()
        {
            HashSet<string> activeMods = apexMod.activeMods;

            if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                sporantulaChance = this.config.Bind<int>("SporantulaChance", 4, new ConfigAcceptableRange<int>(0, 100));
                sporantulaExtras = this.config.Bind<int>("ExtraSporantulas", 25, new ConfigAcceptableRange<int>(0, 100));
                scutigeraChance = this.config.Bind<int>("ScutigeraChance", 15, new ConfigAcceptableRange<int>(0, 100));
                scutigeraExtras = this.config.Bind<int>("ExtraScutigeras", 0, new ConfigAcceptableRange<int>(0, 100));
                redRedHorrorCentiChance = this.config.Bind<int>("RedRedHorrorCentiChance", 10, new ConfigAcceptableRange<int>(0, 100));
                wingRedHorrorCentiChance = this.config.Bind<int>("WingRedHorrorCentiChance", 4, new ConfigAcceptableRange<int>(0, 100));
                waterSpitterChance = this.config.Bind<int>("WaterSpitterChance", 10, new ConfigAcceptableRange<int>(0, 100));
                waterSpitterExtras = this.config.Bind<int>("ExtraWaterSpitters", 0, new ConfigAcceptableRange<int>(0, 100));
                fatFireFlyChance = this.config.Bind<int>("FatFireFlyChance", 10, new ConfigAcceptableRange<int>(0, 100));
                surfaceSwimmerChance = this.config.Bind<int>("SurfaceSwimmerChance", 20, new ConfigAcceptableRange<int>(0, 100));
                surfaceSwimmerExtras = this.config.Bind<int>("SurfaceSwimmerExtras", 5, new ConfigAcceptableRange<int>(0, 100));
                bounceBallChance = this.config.Bind<int>("BouncingBallChance", 10, new ConfigAcceptableRange<int>(0, 100));
                bounceBallExtras = this.config.Bind<int>("BouncingBallExtras", 10, new ConfigAcceptableRange<int>(0, 100));
                critterHoverflyChance = this.config.Bind<int>("CritterHoverflyChance", 7, new ConfigAcceptableRange<int>(0, 100));
                hoverflyExtras = this.config.Bind<int>("HoverflyExtras", 15, new ConfigAcceptableRange<int>(0, 100));
                noodleEaterChance = this.config.Bind<int>("NoodleEaterChance", 10, new ConfigAcceptableRange<int>(0, 100));
                noodleEaterExtras = this.config.Bind<int>("NoodleEaterExtras", 6, new ConfigAcceptableRange<int>(0, 20));
                thornbugChance = this.config.Bind<int>("ThornbugChance", 20, new ConfigAcceptableRange<int>(0, 100));
                thornbugExtras = this.config.Bind<int>("ThornbugExtras", 4, new ConfigAcceptableRange<int>(0, 20));
                miniLeviathanChance = this.config.Bind<int>("MiniLeviChance", 25, new ConfigAcceptableRange<int>(0, 100));
                miniLeviathanExtras = this.config.Bind<int>("MiniLeviExtras", 3, new ConfigAcceptableRange<int>(0, 20));
                polliwogChance = this.config.Bind<int>("PolliwogChance", 10, new ConfigAcceptableRange<int>(0, 100));
                polliwogExtras = this.config.Bind<int>("PolliwogExtras", 8, new ConfigAcceptableRange<int>(0, 100));
                hunterSeekerCyanChance = this.config.Bind<int>("HunterSeekerCyanChance", 6, new ConfigAcceptableRange<int>(0, 100));
                hunterSeekerWhiteChance = this.config.Bind<int>("HunterSeekerWhiteChance", 6, new ConfigAcceptableRange<int>(0, 100));
                hunterSeekerExtras = this.config.Bind<int>("HunterSeekerExtras", 2, new ConfigAcceptableRange<int>(0, 100));
                silverLizChance = this.config.Bind<int>("SilverLizardChance", 15, new ConfigAcceptableRange<int>(0, 100));
                silverLizExtras = this.config.Bind<int>("SilverLizardExtras", 2, new ConfigAcceptableRange<int>(0, 100));
                vultureEchoLeviChance = this.config.Bind<int>("VultureEchoLeviChance", 10, new ConfigAcceptableRange<int>(0, 100));
                echoLeviExtras = this.config.Bind<int>("EchoLeviExtras", 0, new ConfigAcceptableRange<int>(0, 100));
                blizzorChance = this.config.Bind<int>("BlizzorChance", 7, new ConfigAcceptableRange<int>(0, 100));
                salamanderSalamoleChance = this.config.Bind<int>("SalamanderSalamoleChance", 5, new ConfigAcceptableRange<int>(0, 100));
                blackSalamolechance = this.config.Bind<int>("BlackSalamoleChance", 5, new ConfigAcceptableRange<int>(0, 100));
                blizzorExtras = this.config.Bind<int>("BlizzorExtras", 2, new ConfigAcceptableRange<int>(0, 20));
                salamoleExtras = this.config.Bind<int>("SalamoleExtras", 10, new ConfigAcceptableRange<int>(0, 20));

            }
            if (activeMods.Contains("ShinyKelp.AngryInspectors"))
            {
                inspectorChance = this.config.Bind<int>("InspectorChance", 8, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("moredlls"))
            {
                mExplosiveLongLegsChance = this.config.Bind<int>("MExplosiveLongLegsChance", 5, new ConfigAcceptableRange<int>(0, 100));
                mZappyLongLegsChance = this.config.Bind<int>("ZappyLongLegsChance", 5, new ConfigAcceptableRange<int>(0, 100));
                
            }
            if (activeMods.Contains("ShinyKelp.LizardVariants"))
            {
                mintLizardChance = this.config.Bind<int>("MintLizardChance", 10, new ConfigAcceptableRange<int>(0, 100));
                ryanLizardChance = this.config.Bind<int>("RyanLizardChance", 4, new ConfigAcceptableRange<int>(0, 100));
                yellowLimeLizardChance = this.config.Bind<int>("YellowLimeLizardChance", 16, new ConfigAcceptableRange<int>(0, 100));
                mintLizardExtras = this.config.Bind<int>("ExtraSMintLizards", 4, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("thefriend"))
            {
                motherLizardChance = this.config.Bind<int>("MotherLizardChance", 3, new ConfigAcceptableRange<int>(0, 100));
                youngLizardExtras = this.config.Bind<int>("YoungLizardExtras", 0, new ConfigAcceptableRange<int>(0, 100));
                lostYoungLizardChance = this.config.Bind<int>("LostYoungLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                snowSpiderChance = this.config.Bind<int>("SnowSpiderChance", 10, new ConfigAcceptableRange<int>(0, 100));
                snowSpiderExtras = this.config.Bind<int>("SnowSpiderExtras", 4, new ConfigAcceptableRange<int>(0, 100));

            }
            if (activeMods.Contains("Outspector"))
            {
                outspectorChance = this.config.Bind<int>("OutspectorChance", 20, new ConfigAcceptableRange<int>(0, 100));
                outspectorExtras = this.config.Bind<int>("OutspectorExtras", 12, new ConfigAcceptableRange<int>(0, 100));
                if(activeMods.Contains("ShinyKelp.AngryInspectors"))
                    inspectorOutspectorInvChance = this.config.Bind<int>("InspectorOutspectorInvChance", 10, new ConfigAcceptableRange<int>(0, 100));
                
            }
            if (activeMods.Contains("theincandescent"))
            {
                icyBlueFreezerInvChance = this.config.Bind<int>("IcyBlueFreezerInvChance", 5, new ConfigAcceptableRange<int>(0, 100));
                icyBlueBlueChance = this.config.Bind<int>("IcyBlueBlueChance", 10, new ConfigAcceptableRange<int>(0, 100));
                icyBlueYellowChance = this.config.Bind<int>("IcyBlueYellowChance", 15, new ConfigAcceptableRange<int>(0, 100));
                freezerLizChance = this.config.Bind<int>("FreezerLizChance", 5, new ConfigAcceptableRange<int>(0, 100));
                cyanwingChance = this.config.Bind<int>("CyanwingChance", 4, new ConfigAcceptableRange<int>(0, 100));
                wingCyanwingChance = this.config.Bind<int>("WingCyanwingChance", 10, new ConfigAcceptableRange<int>(0, 100));
                jetfishBabyAquapedeChance = this.config.Bind<int>("JetfishBabyAquapedeChance", 7, new ConfigAcceptableRange<int>(0, 100));
                babyAquapedeInvChance = this.config.Bind<int>("BabyAquapedeInvChance", 20, new ConfigAcceptableRange<int>(0, 100));
                babyAquapedeExtras = this.config.Bind<int>("BabyAquapedeExtras", 22, new ConfigAcceptableRange<int>(0, 100));
                icyBlueLizExtras = this.config.Bind<int>("IcyBlueLizExtras", 6, new ConfigAcceptableRange<int>(0, 100));
                aquapedeBabyAquaChance = this.config.Bind<int>("AquapedeBabyAquaChance", 5, new ConfigAcceptableRange<int>(0, 100));
                chillipedeChance = this.config.Bind<int>("ChillipedeChance", 10, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("Pitch Black") || activeMods.Contains("lurzard.pitchblack"))
            {
                critterLittleLongLegsChance = this.config.Bind<int>("CritterLittleLongLegsChance", 5, new ConfigAcceptableRange<int>(0, 100));
                brotherLittleLongLegChance = this.config.Bind<int>("BrotherLittleLongLegsChance", 20, new ConfigAcceptableRange<int>(0, 100));
                nightTerrorChance = this.config.Bind<int>("NightTerrorChance", 2, new ConfigAcceptableRange<int>(0, 100));

                littleLongLegsExtras = this.config.Bind<int>("LittleLongLegsExtras", 5, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("drainmites"))
            {
                drainMiteChance = this.config.Bind<int>("DrainMiteChance", 25, new ConfigAcceptableRange<int>(0, 100));
                drainMiteExtras = this.config.Bind<int>("DrainMiteExtras", 30, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("Croken.bombardier-vulture"))
            {
                bombVultureChance = this.config.Bind<int>("BombardierVultureChance", 6, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("pkuya.thevanguard"))
            {
                toxicSpiderChance = this.config.Bind<int>("ToxicSpiderChance", 20, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("myr.moss_fields") || activeMods.Contains("ShinyKelp.Udonfly"))
            {
                fatNootChance = this.config.Bind<int>("FatNoodleflyChance", 10, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("shrimb.scroungers"))
            {
                //Mimicstar, Thorn Bug, 
                scroungerChance = this.config.Bind<int>("ScroungerChance", 10, new ConfigAcceptableRange<int>(0, 100));
                scroungerExtras = this.config.Bind<int>("ScroungerExtras", 4, new ConfigAcceptableRange<int>(0, 20));
            }
            if (activeMods.Contains("Croken.Mimicstarfish"))
            {
                bllMimicstarfishChance = this.config.Bind<int>("BLLMimicChance", 7, new ConfigAcceptableRange<int>(0, 100));
                critterMimicstarfishChance = this.config.Bind<int>("CritterMimicChance", 3, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("ShinyKelp.AlbinoKings"))
            {
                albinoVultureChance = this.config.Bind<int>("AlbinoVultureChance", 10, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("bebra.gregtech_lizard"))
            {
                centiElectricLizChance = this.config.Bind<int>("CentiElectricLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
                lizardElectricLizChance = this.config.Bind<int>("LizElectricLizardChance", 5, new ConfigAcceptableRange<int>(0, 100));
            }
            if (activeMods.Contains("bry.bubbleweavers"))
            {
                spiderWeaverChance = this.config.Bind<int>("SpiderWeaverChance", 10, new ConfigAcceptableRange<int>(0, 100));
                if(activeMods.Contains("lb-fgf-m4r-ik.swalkins"))
                    sSwimmerWeaverChance = this.config.Bind<int>("SSwimmerWeaverChance", 15, new ConfigAcceptableRange<int>(0, 100));
                bubbleWeaverExtras = this.config.Bind<int>("SpiderWeaverExtras", 4, new ConfigAcceptableRange<int>(0, 100));
            }
        }

        private void SetModConfigs(Dictionary<Configurable<int>, string> labelsMap, List<Configurable<int>> enabledModsRepConfigs, List<Configurable<int>> enabledModsExtraConfigs)
        {
            HashSet<string> activeMods = apexMod.activeMods;

            if (activeMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                labelsMap.Add(sporantulaChance, "Small Insects > Sporantula (Inv)");
                labelsMap.Add(sporantulaExtras, "Sporantulas (/10)");
                enabledModsRepConfigs.Add(sporantulaChance);
                enabledModsExtraConfigs.Add(sporantulaExtras);
                labelsMap.Add(scutigeraChance, "Centipede > Scutigera");
                labelsMap.Add(scutigeraExtras, "Scutigeras (/10)");
                enabledModsRepConfigs.Add(scutigeraChance);
                enabledModsExtraConfigs.Add(scutigeraExtras);
                labelsMap.Add(redRedHorrorCentiChance, "Red Centipede > Red Horror Centi");
                labelsMap.Add(wingRedHorrorCentiChance, "Centiwing > Red Horror Centi");
                enabledModsRepConfigs.Add(redRedHorrorCentiChance);
                enabledModsRepConfigs.Add(wingRedHorrorCentiChance);
                labelsMap.Add(waterSpitterChance, "Aquatic Lizards > Water Spitter");
                labelsMap.Add(waterSpitterExtras, "Water Spitters (/10)");
                enabledModsRepConfigs.Add(waterSpitterChance);
                enabledModsExtraConfigs.Add(waterSpitterExtras);
                labelsMap.Add(fatFireFlyChance, "Vultures > Fat Firefly");
                enabledModsRepConfigs.Add(fatFireFlyChance);
                labelsMap.Add(surfaceSwimmerChance, "EggBug > Surface Swimmer");
                labelsMap.Add(surfaceSwimmerExtras, "Surface Swimmer");
                enabledModsRepConfigs.Add(surfaceSwimmerChance);
                enabledModsExtraConfigs.Add(surfaceSwimmerExtras);
                labelsMap.Add(bounceBallChance, "Snail > Bouncing Ball");
                labelsMap.Add(bounceBallExtras, "Bouncing Ball");
                enabledModsRepConfigs.Add(bounceBallChance);
                enabledModsExtraConfigs.Add(bounceBallExtras);
                labelsMap.Add(noodleEaterChance, "Noodlefly > Noodle Eater (Inv)");
                labelsMap.Add(noodleEaterExtras, "Noodle Eater (/10)");
                enabledModsRepConfigs.Add(noodleEaterChance);
                enabledModsExtraConfigs.Add(noodleEaterExtras);
                labelsMap.Add(thornbugChance, "Eggbug > Thornbug (Inv)");
                labelsMap.Add(thornbugExtras, "Thornbug (/10)");
                enabledModsRepConfigs.Add(thornbugChance);
                enabledModsExtraConfigs.Add(thornbugExtras);
                labelsMap.Add(miniLeviathanChance, "Leviathan > Mini Leviathan (Inv)");
                labelsMap.Add(miniLeviathanExtras, "Mini Leviathan");
                enabledModsRepConfigs.Add(miniLeviathanChance);
                enabledModsExtraConfigs.Add(miniLeviathanExtras); labelsMap.Add(polliwogChance, "Salamander > Polliwog");
                labelsMap.Add(polliwogExtras, "Polliwog (/10)");
                enabledModsRepConfigs.Add(polliwogChance);
                enabledModsExtraConfigs.Add(polliwogExtras);
                labelsMap.Add(hunterSeekerCyanChance, "Cyan Liz > Hunter Seeker");
                labelsMap.Add(hunterSeekerWhiteChance, "White Liz > Hunter Seeker");
                labelsMap.Add(hunterSeekerExtras, "Hunter Seeker (/10)");
                enabledModsRepConfigs.Add(hunterSeekerCyanChance);
                enabledModsRepConfigs.Add(hunterSeekerWhiteChance);
                enabledModsExtraConfigs.Add(hunterSeekerExtras);
                labelsMap.Add(silverLizChance, "Grounded Lizards > Silver Liz");
                labelsMap.Add(silverLizExtras, "Silver Lizard (/10)");
                enabledModsRepConfigs.Add(silverLizChance);
                enabledModsExtraConfigs.Add(silverLizExtras);
                labelsMap.Add(vultureEchoLeviChance, "Vultures > Echo Leviathan (Den)");
                enabledModsRepConfigs.Add(vultureEchoLeviChance);
                labelsMap.Add(echoLeviExtras, "Echo Leviathans (/10)");
                enabledModsExtraConfigs.Add(echoLeviExtras);
                labelsMap.Add(blizzorChance, "Miros Bird > Blizzor");
                labelsMap.Add(blizzorExtras, "Blizzor");
                labelsMap.Add(salamanderSalamoleChance, "Salamander > Mole Salamander");
                labelsMap.Add(blackSalamolechance, "Black liz > Mole Salamander");
                labelsMap.Add(salamoleExtras, "Mole Salamander (/10)");
                enabledModsRepConfigs.Add(salamanderSalamoleChance);
                enabledModsRepConfigs.Add(blackSalamolechance);
                enabledModsRepConfigs.Add(blizzorChance);
                enabledModsExtraConfigs.Add(blizzorExtras);
                enabledModsExtraConfigs.Add(salamoleExtras);
                labelsMap.Add(critterHoverflyChance, "Critters > Hoverfly (Inv)");
                labelsMap.Add(hoverflyExtras, "Hoverfly (/10)");
                enabledModsRepConfigs.Add(critterHoverflyChance);
                enabledModsExtraConfigs.Add(hoverflyExtras);
            }
            if (activeMods.Contains("ShinyKelp.AngryInspectors"))
            {
                labelsMap.Add(inspectorChance, "LongLegs/??? > Inspector (Inv)");
                enabledModsRepConfigs.Add(inspectorChance);
            }
            if (activeMods.Contains("moredlls"))
            {
                labelsMap.Add(mExplosiveLongLegsChance, "LongLegs > Explosive DLL");
                labelsMap.Add(mZappyLongLegsChance, "LongLegs > Zappy DLL");
                enabledModsRepConfigs.Add(mExplosiveLongLegsChance);
                enabledModsRepConfigs.Add(mZappyLongLegsChance);
            }
            if (activeMods.Contains("ShinyKelp.LizardVariants"))
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
            if (activeMods.Contains("thefriend"))
            {
                labelsMap.Add(youngLizardExtras, "Young Lizards (/10)");
                labelsMap.Add(lostYoungLizardChance, "Small Lizards > Lost Young Lizard");
                labelsMap.Add(motherLizardChance, "Ground Lizards > Mother Lizard (+ youngs)");
                labelsMap.Add(snowSpiderChance, "Big Spider > Snow Spider");
                labelsMap.Add(snowSpiderExtras, "Snow Spiders (/10)");

                enabledModsRepConfigs.Add(motherLizardChance);
                enabledModsRepConfigs.Add(lostYoungLizardChance);
                enabledModsRepConfigs.Add(snowSpiderChance);
                enabledModsExtraConfigs.Add(youngLizardExtras);
                enabledModsExtraConfigs.Add(snowSpiderExtras);
            }
            if (activeMods.Contains("Outspector"))
            {
                labelsMap.Add(outspectorChance, "Inspector > Outspector");
                labelsMap.Add(outspectorExtras, "Outspectors (/10)");
                enabledModsRepConfigs.Add(outspectorChance);
                enabledModsExtraConfigs.Add(outspectorExtras);
                if(activeMods.Contains("ShinyKelp.AngryInspectors"))
                {
                    labelsMap.Add(inspectorOutspectorInvChance, "Outspector > Inspector (Inv)");
                    enabledModsRepConfigs.Add(inspectorOutspectorInvChance);
                }
            }
            if (activeMods.Contains("theincandescent"))
            {
                labelsMap.Add(icyBlueFreezerInvChance, "Freezer > Icy Blue Liz (Den, Inv)");
                labelsMap.Add(icyBlueBlueChance, "Blue Liz > Icy Blue Liz (Den)");
                labelsMap.Add(icyBlueYellowChance, "Yellow Liz > Icy Blue Liz");
                labelsMap.Add(freezerLizChance, "Icy Blue / Caramel > Freezer Liz (Den)");
                labelsMap.Add(wingCyanwingChance, "Centiwing > Cyanwing (Den)");
                labelsMap.Add(cyanwingChance, "Yellow Centipedes > Cyanwing (Den)");
                labelsMap.Add(jetfishBabyAquapedeChance, "Jetfish > Infant Aquapede (Inv)");
                labelsMap.Add(aquapedeBabyAquaChance, "Infant Aquapede > Aquapede");
                labelsMap.Add(babyAquapedeInvChance, "Aquapede > Infant Aquapede (Inv)");
                labelsMap.Add(icyBlueLizExtras, "Icy Blue Lizards (/10)");
                labelsMap.Add(babyAquapedeExtras, "Infant Aquapedes (/10)");
                labelsMap.Add(chillipedeChance, "Ground Lizards > Chillipede");

                enabledModsRepConfigs.Add(icyBlueFreezerInvChance);
                enabledModsRepConfigs.Add(icyBlueBlueChance);
                enabledModsRepConfigs.Add(icyBlueYellowChance);
                enabledModsRepConfigs.Add(freezerLizChance);
                enabledModsRepConfigs.Add(cyanwingChance);
                enabledModsRepConfigs.Add(wingCyanwingChance);
                enabledModsRepConfigs.Add(jetfishBabyAquapedeChance);
                enabledModsRepConfigs.Add(aquapedeBabyAquaChance);
                enabledModsRepConfigs.Add(babyAquapedeInvChance);
                enabledModsRepConfigs.Add(chillipedeChance);
                enabledModsExtraConfigs.Add(icyBlueLizExtras);
                enabledModsExtraConfigs.Add(babyAquapedeExtras);
            }
            if (activeMods.Contains("lurzard.pitchblack"))
            {
                labelsMap.Add(critterLittleLongLegsChance, "Snail/LMice/??? > Little LongLegs (Inv)");
                labelsMap.Add(brotherLittleLongLegChance, "Brother > Little LongLegs (Inv)");
                labelsMap.Add(nightTerrorChance, "Centipedes > Night Terror");
                labelsMap.Add(littleLongLegsExtras, "Little LongLegs (/10)");

                enabledModsRepConfigs.Add(critterLittleLongLegsChance);
                enabledModsRepConfigs.Add(brotherLittleLongLegChance);
                enabledModsRepConfigs.Add(nightTerrorChance);
                enabledModsExtraConfigs.Add(littleLongLegsExtras);
            }
            if (activeMods.Contains("drainmites"))
            {
                labelsMap.Add(drainMiteChance, "Scavengers > Drain Mites (Den)(Inv)");
                labelsMap.Add(drainMiteExtras, "Drain Mites");
                enabledModsRepConfigs.Add(drainMiteChance);
                enabledModsExtraConfigs.Add(drainMiteExtras);
            }
            if (activeMods.Contains("Croken.bombardier-vulture"))
            {
                labelsMap.Add(bombVultureChance, "Vultures > Bombardier Vulture");
                enabledModsRepConfigs.Add(bombVultureChance);
            }
            if (activeMods.Contains("pkuya.thevanguard"))
            {
                labelsMap.Add(toxicSpiderChance, "Spitter > Toxic Spider");
                enabledModsRepConfigs.Add(toxicSpiderChance);
            }
            if (activeMods.Contains("ShinyKelp.Udonfly") || activeMods.Contains("myr.moss_fields"))
            {
                labelsMap.Add(fatNootChance, "Noodlefly > Fat Noodlefly");
                enabledModsRepConfigs.Add(fatNootChance);
            }
            if (activeMods.Contains("shrimb.scroungers"))
            {
                labelsMap.Add(scroungerChance, "Scavenger > Scrounger");
                labelsMap.Add(scroungerExtras, "Scroungers");
                enabledModsRepConfigs.Add(scroungerChance);
                enabledModsExtraConfigs.Add(scroungerExtras);
            }
            if (activeMods.Contains("Croken.Mimicstarfish"))
            {
                labelsMap.Add(bllMimicstarfishChance, "BLL > Mimic Starfish");
                labelsMap.Add(critterMimicstarfishChance, "Aquatic creatures > Mimic Starfish");
                enabledModsRepConfigs.Add(bllMimicstarfishChance);
                enabledModsRepConfigs.Add(critterMimicstarfishChance);
            }
            if (activeMods.Contains("ShinyKelp.AlbinoKings"))
            {
                labelsMap.Add(albinoVultureChance, "Vultures > Albino Vultures");
                enabledModsRepConfigs.Add(albinoVultureChance);
            }
            if (activeMods.Contains("bebra.gregtech_lizard"))
            {
                labelsMap.Add(centiElectricLizChance, "Centipedes > Electric Lizard (Inv)");
                labelsMap.Add(lizardElectricLizChance, "Lizards > Electric Lizard (Inv)");
                enabledModsRepConfigs.Add(centiElectricLizChance);
                enabledModsRepConfigs.Add(lizardElectricLizChance);
            }
            if (activeMods.Contains("bry.bubbleweavers"))
            {
                labelsMap.Add(spiderWeaverChance, "Spitter Spider > Bubble Weaver");
                labelsMap.Add(bubbleWeaverExtras, "Bubble Weaver (/10)");
                enabledModsRepConfigs.Add(spiderWeaverChance);
                enabledModsExtraConfigs.Add(bubbleWeaverExtras);
                if (activeMods.Contains("lb-fgf-m4r-ik.swalkins"))
                {
                    labelsMap.Add(sSwimmerWeaverChance, "Surface Swimmer > Spider Weaver (Inv)");
                    enabledModsRepConfigs.Add(sSwimmerWeaverChance);
                }

            }

        }

        private void SavePreset(UIfocusable trigger)
        {
            string presetName = presetText.value;
            if (presetName == "" || presetName is null)
                presetName = presetsComboBox.GetItemList()[presetsComboBox.GetIndex()].name;
            if (presetName == "Nulls" || presetName == "Default")
                return;

            if (!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns");

            string filePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/" + presetName + ".txt";
            Dictionary<string, int> valuesToSave = new Dictionary<string, int>();
            bool alreadyExists;
            if (alreadyExists = File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                string line = sr.ReadLine();
                while (line != null)
                {
                    string[] splits = line.Split('|');
                    valuesToSave.Add(splits[0], Int32.Parse(splits[1]));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
                
            foreach(KeyValuePair<string, OpUpdown> pair in optionsRefs)
            {
                if (valuesToSave.ContainsKey(pair.Key))
                    valuesToSave[pair.Key] = Int32.Parse(pair.Value.value);
                else
                    valuesToSave.Add(pair.Key, Int32.Parse(pair.Value.value));
            }

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                foreach (KeyValuePair<string, int> pair in valuesToSave)
                    writer.WriteLine(pair.Key + "|" + pair.Value);
            }

            if (!alreadyExists)
                presetsComboBox.AddItems(false, new ListItem(presetName));
            presetText.Reset();
        }

        private void LoadPreset(UIfocusable trigger)
        {
            string presetName = presetsComboBox.GetItemList()[presetsComboBox.GetIndex()].name;
            if (presetName == "Nulls")
                SetNulls();
            else if (presetName == "Default")
                SetDefaults();
            else
            {
                StreamReader sr = new StreamReader(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/" + presetName + ".txt");
                string line = sr.ReadLine();
                while(line != null)
                {
                    string[] splits = line.Split('|');
                    if(optionsRefs.ContainsKey(splits[0]))
                        SetValue(optionsRefs[splits[0]], Int32.Parse(splits[1]));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }

        private void RemovePreset(UIfocusable trigger)
        {
            string presetName = presetsComboBox.GetItemList()[presetsComboBox.GetIndex()].name;
            if (presetName == "Nulls" || presetName == "Default")
                return;
            presetsComboBox.RemoveItems(true, presetName);
            File.Delete(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/" + presetName + ".txt");
        }

        private void SetDefaults()
        {
            foreach(OpUpdown op in optionsRefs.Values)
                op.Reset();
        }

        private void SetNulls()
        {
            string aux;
            foreach (OpUpdown op in optionsRefs.Values)
            {
                aux = op.defaultValue;
                op.defaultValue = "0";
                op.Reset();
                op.defaultValue = aux;
            }
        }

        private void SetValue(OpUpdown op, int value)
        {
            string aux = op.defaultValue;
            op.defaultValue = value.ToString();
            op.Reset();
            op.defaultValue = aux;
        }


    }
}
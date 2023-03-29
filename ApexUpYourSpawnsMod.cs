using BepInEx;
using System.Security.Permissions;
using System.Security;
using System;
using UnityEngine;
using MoreSlugcats;
using System.Collections.Generic;
using System.Security.AccessControl;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ApexUpYourSpawns
{
    [BepInPlugin("ShinyKelp.ApexUpYourSpawns", "ApexUpYourSpawns", "1.1.4")]

    public class ApexUpYourSpawnsMod : BaseUnityPlugin
    {
        private float redLizardChance, trainLizardChance, largeCentipedeChance, redCentipedeChance, spitterSpiderChance, mirosVultureChance,
            eliteScavengerChance, brotherLongLegsChance, daddyLongLegsChance, terrorLongLegsChance, noodleFlyChance, fireBugChance,
            giantJellyfishChance, leechLizardChance, yeekLizardChance, waterPredatorChance, caramelLizChance, strawberryLizChance,
            cyanLizChance, jungleLeechChance, motherSpiderChance, stowawayChance, inspectorChance, sporantulaChance;

        private int extraYellows, extraLizards, extraCyans, extraWaterLiz, extraSpiders, extraVultures, extraKings, extraScavengers, extraEggbugs, extraCentipedes,
            extraCentiwings, extraAquapedes, extraPrecycleSals, extraDropwigs, extraMiros, extraSmallSpiders, extraLeeches, extraKelp, extraLeviathans, extraSporantulas;

        private bool IsInit;

        public bool hasSporantulaMod;

        private ApexUpYourSpawnsOptions options;

        private RainWorldGame game;

        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;            
        }

        public ApexUpYourSpawnsMod()
        {
            try
            {
                options = new ApexUpYourSpawnsOptions(this, Logger);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;

                //hooks go here

                On.GameSession.ctor += GameSessionOnctor;
                On.WorldLoader.GeneratePopulation += GenerateCustomPopulation;
                On.JellyFish.PlaceInRoom += replaceGiantJellyfish;
                On.DangleFruit.PlaceInRoom += replaceStowawayBugBlueFruit;
                On.MoreSlugcats.GooieDuck.PlaceInRoom += replaceStowawayBugGooieDuck;
                hasSporantulaMod = false;
                foreach(ModManager.Mod mod in ModManager.ActiveMods)
                {
                    if (mod.name == "Sporantula")
                        hasSporantulaMod = true;
                }

                //On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
                MachineConnector.SetRegisteredOI("ShinyKelp.ApexUpYourSpawns", this.options);
                IsInit = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        private void setOptions()
        {
            redLizardChance = (float)options.redLizardChance.Value / 100;
            trainLizardChance = (float)options.trainLizardChance.Value / 100;
            largeCentipedeChance = (float)options.largeCentipedeChance.Value / 100;
            redCentipedeChance = (float)options.redCentipedeChance.Value / 100;
            spitterSpiderChance = (float)options.spitterSpiderChance.Value / 100;
            mirosVultureChance = (float)options.mirosVultureChance.Value / 100;
            eliteScavengerChance = (float)options.eliteScavengerChance.Value / 100;
            brotherLongLegsChance = (float)options.brotherLongLegsChance.Value / 100;
            daddyLongLegsChance = (float)options.daddyLongLegsChance.Value / 100;
            terrorLongLegsChance = (float)options.terrorLongLegsChance.Value / 100;
            fireBugChance = (float)options.fireBugChance.Value / 100;
            noodleFlyChance = (float) options.noodleFlyChance.Value / 100;
            leechLizardChance = (float)options.leechLizardChance.Value / 100;
            yeekLizardChance = (float)options.yeekLizardChance.Value / 100;
            waterPredatorChance = (float)options.waterPredatorChance.Value / 100;
            giantJellyfishChance = (float)options.giantJellyfishChance.Value / 100;
            caramelLizChance = (float)options.caramelLizChance.Value / 100;
            strawberryLizChance = (float)options.strawberryLizChance.Value / 100;
            cyanLizChance = (float)options.cyanLizChance.Value / 100;
            jungleLeechChance = (float)options.jungleLeechChance.Value / 100;
            motherSpiderChance = (float)options.motherSpiderChance.Value / 100;
            stowawayChance = (float)options.stowawayChance.Value / 100;
            inspectorChance = (float)options.inspectorChance.Value / 100;
            sporantulaChance = (float)options.sporantulaChance.Value / 100;

            extraYellows = options.yellowLizExtras.Value;
            extraLizards = options.genericLizExtras.Value;
            extraCyans = options.cyanLizExtras.Value;
            extraWaterLiz = options.waterLizExtras.Value;
            extraSpiders = options.bigSpiderExtras.Value;
            extraVultures = options.vultureExtras.Value;
            extraKings = options.vultureKingExtras.Value;
            extraScavengers = options.scavengerExtras.Value;
            extraEggbugs = options.eggbugExtras.Value;
            extraCentipedes = options.centipedeExtras.Value;
            extraCentiwings = options.centiWingExtras.Value;
            extraAquapedes = options.aquapedeExtras.Value;
            extraPrecycleSals = options.precycleSalExtras.Value;
            extraDropwigs = options.dropwigExtras.Value;
            extraMiros = options.mirosExtras.Value;
            extraSmallSpiders = options.spiderExtras.Value;
            extraLeeches = options.leechExtras.Value;
            extraKelp = options.kelpExtras.Value;
            extraLeviathans = options.leviathanExtras.Value;
            extraSporantulas = options.sporantulaExtras.Value;
        }
        private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            ClearMemory();
        }
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            ClearMemory();
            this.game = game;
        }

        #region Helper Methods

        private void ClearMemory()
        {
            //If you have any collections (lists, dictionaries, etc.)
            //Clear them here to prevent a memory leak
            //YourList.Clear();
        }
        #endregion

        private void replaceGiantJellyfish(On.JellyFish.orig_PlaceInRoom orig, JellyFish self, Room room)
        {

            if (!room.abstractRoom.shelter && UnityEngine.Random.value < giantJellyfishChance)
            {
                AbstractCreature myBigJelly = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.BigJelly), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), game.GetNewID());
                BigJellyFish myJelly = new BigJellyFish(myBigJelly, game.world);
                myJelly.PlaceInRoom(room);
            }
            else
                orig(self, room);
        }

        private void replaceStowawayBugBlueFruit(On.DangleFruit.orig_PlaceInRoom orig, DangleFruit self, Room room)
        {
            if(!room.abstractRoom.shelter && UnityEngine.Random.value < stowawayChance)
            {
                self.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(self, room, self.firstChunk.pos);

                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.StowawayBug), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), game.GetNewID());

                Vector2 pos = new Vector2(self.abstractPhysicalObject.pos.x * 20.1f, (self.abstractPhysicalObject.pos.y) * 20.1f + stalk.ropeLength);

                (myStowawayAbstract.state as StowawayBugState).HomePos = new Vector2(pos.x, pos.y);
                pos.y -= 60f;
                (myStowawayAbstract.state as StowawayBugState).aimPos = pos;
                (myStowawayAbstract.state as StowawayBugState).debugForceAwake = true;
                myStowawayAbstract.pos.abstractNode = 0;

                StowawayBug myBug = new StowawayBug(myStowawayAbstract, game.world);

                myBug.AI = new StowawayBugAI(myStowawayAbstract, game.world);

                myBug.PlaceInRoom(room);
            }
            
            orig(self, room);
        }

        private void replaceStowawayBugGooieDuck(On.MoreSlugcats.GooieDuck.orig_PlaceInRoom orig, GooieDuck self, Room room)
        {
            if (!room.abstractRoom.shelter && UnityEngine.Random.value < stowawayChance * 2)
            {
                DangleFruit fruit = new DangleFruit(self.abstractPhysicalObject);
                fruit.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(fruit, room, fruit.firstChunk.pos);

                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.StowawayBug), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), game.GetNewID());
                Vector2 pos = new Vector2(self.abstractPhysicalObject.pos.x * 20.1f, (self.abstractPhysicalObject.pos.y) * 20.1f + stalk.ropeLength);
                (myStowawayAbstract.state as StowawayBugState).HomePos = new Vector2(pos.x, pos.y);
                pos.y -= 60f;
                (myStowawayAbstract.state as StowawayBugState).aimPos = pos;
                (myStowawayAbstract.state as StowawayBugState).debugForceAwake = true;
                myStowawayAbstract.pos.abstractNode = 0;

                StowawayBug myBug = new StowawayBug(myStowawayAbstract, game.world);
                myBug.AI = new StowawayBugAI(myStowawayAbstract, game.world);
                myBug.PlaceInRoom(room);
            }
            orig(self, room);
        }

        private void GenerateCustomPopulation(On.WorldLoader.orig_GeneratePopulation orig, WorldLoader worldLoader, bool fresh)
        {

            try
            {
                if (fresh)
                {
                    setOptions();

                    for (int i = 0; i < worldLoader.spawners.Count; i++)
                    {
                        bool foundType;

                        //Debug.Log("SPAWNER INDEX: " + i);
                        if (worldLoader.spawners[i] is World.SimpleSpawner simpleSpawner)
                        {
                            /*
                            Debug.Log("\n");
                            Debug.Log("SPAWNER DATA: " + simpleSpawner.ToString());
                            Debug.Log("CREATURE: " + simpleSpawner.creatureType);
                            Debug.Log("INDEX: " + simpleSpawner.inRegionSpawnerIndex);
                            Debug.Log("DEN: " + simpleSpawner.den.ToString());
                            Debug.Log("DEN ROOM: " + simpleSpawner.den.ResolveRoomName());
                            Debug.Log("AMOUNT: " + simpleSpawner.amount);
                            Debug.Log("SPAWN DATA STRING: " + simpleSpawner.spawnDataString);
                            if (simpleSpawner.nightCreature)
                                    Debug.Log("IS NIGHT CREATURE.");
                            //*/

                            //Precycle spawns
                            if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle"))
                            {
                                handlePrecycleSpawns(simpleSpawner, worldLoader.spawners);
                            }

                            //Lizards
                            foundType = handleLizardSpawner(simpleSpawner, worldLoader.spawners);

                            //Centipedes
                            foundType = replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.SmallCentipede, CreatureTemplate.Type.Centipede, largeCentipedeChance);
                            if (foundType)
                            {
                                if (hasSporantulaMod && simpleSpawner.amount > 1)
                                {
                                    addSporantulaSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.SmallCentipede, sporantulaChance);
                                    addSporantulaSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Centipede, sporantulaChance);

                                }
                                continue;
                            }
                            
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Centipede, extraCentipedes);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede,
                                    (worldLoader.worldName == "VS" || worldLoader.worldName == "SB") ? redCentipedeChance / 2 : redCentipedeChance);
                                if (hasSporantulaMod && simpleSpawner.amount > 1)
                                {
                                    addSporantulaSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Centipede, sporantulaChance);
                                }
                                if (worldLoader.worldName == "SH" && simpleSpawner.den.ResolveRoomName() == "SH_H01" && UnityEngine.Random.value < inspectorChance * 10f)
                                {
                                    World.SimpleSpawner i_spawner = CopySpawner(simpleSpawner);
                                    i_spawner.amount = UnityEngine.Random.value < inspectorChance * 10 ? 2 : 1;
                                    i_spawner.creatureType = MoreSlugcatsEnums.CreatureTemplateType.Inspector;
                                    i_spawner.inRegionSpawnerIndex = worldLoader.spawners.Count;
                                    i_spawner.spawnDataString = "{Ignorecycle}";
                                    worldLoader.spawners.Add(i_spawner);
                                }
                                    
                                continue;
                            }
                            else foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Centiwing, extraCentiwings);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, (worldLoader.worldName == "SI") ? redCentipedeChance / 2 : redCentipedeChance);
                                continue;
                            }
                            else foundType = increaseCreatureSpawner(simpleSpawner, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, extraAquapedes); 
                            if (foundType) 
                                continue;

                            //Jetfish replacements (aquapedes, longlegs in shoreline)
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.JetFish, ((worldLoader.worldName == "LM" || worldLoader.worldName == "SB" || worldLoader.worldName == "VS") && waterPredatorChance > 0f) ? 1 : 0);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.JetFish, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti,
                                         (worldLoader.worldName == "SB" || worldLoader.worldName == "VS") ? waterPredatorChance * 2 : waterPredatorChance);
                                continue;
                            }

                            //(Big)Spiders
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.BigSpider, extraSpiders);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                                if (hasSporantulaMod && simpleSpawner.amount > 1)
                                    addSporantulaSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.BigSpider, sporantulaChance);
                                continue;
                            }

                            //Misc
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.DropBug, extraDropwigs);
                            if (foundType)
                                continue;
                            
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.MirosBird, extraMiros);
                            if (foundType)
                            {
                                if(worldLoader.worldName == "SB" && extraMiros > 0)
                                    increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.MirosBird, 2);
                                else if (worldLoader.worldName == "LC" && extraMiros > 0)
                                {
                                    increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.MirosBird, 4);   
                                }
                                continue;
                            }
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Spider, extraSmallSpiders);
                            if (foundType)
                            {
                                replaceSpiderSpawner(simpleSpawner, worldLoader.spawners, motherSpiderChance);
                                continue;
                            }
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.TentaclePlant, extraKelp);
                            if (foundType)
                            {
                                if (inspectorChance > 0 && worldLoader.worldName == "SB" && simpleSpawner.den.ResolveRoomName() == "SB_G03" && UnityEngine.Random.value < inspectorChance * 5)
                                {
                                    //Placing inspector in room with miros
                                    World.SimpleSpawner inspectorSpawner = CopySpawner(simpleSpawner);
                                    inspectorSpawner.amount = 1;
                                    inspectorSpawner.creatureType = MoreSlugcatsEnums.CreatureTemplateType.Inspector;
                                    inspectorSpawner.spawnDataString = "{Ignorecycle}";
                                    inspectorSpawner.inRegionSpawnerIndex = worldLoader.spawners.Count;
                                    worldLoader.spawners.Add(inspectorSpawner);
                                }
                                continue;
                            }
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.BigEel, extraLeviathans);
                            if (foundType)
                                continue;

                            //Yeeks (for caramel/strawberry lizs)
                            foundType = replaceMultiSpawner(simpleSpawner, worldLoader.spawners, MoreSlugcatsEnums.CreatureTemplateType.Yeek, 
                                UnityEngine.Random.value < .75f ? MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard : MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, yeekLizardChance);

                            //Leeches
                            foundType = handleLeechSpawner(simpleSpawner, worldLoader.spawners);
                            if (foundType)
                                continue;
                                
                            //Vultures
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Vulture, extraVultures);
                            if (foundType)
                            {
                                if ((extraVultures > 0 || extraKings > 0) && (worldLoader.worldName == "OE" || worldLoader.worldName == "SI"))
                                    increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Vulture, 2);

                                if(worldLoader.worldName == "OE" || worldLoader.worldName == "SI")
                                    replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Vulture, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture,
                                        mirosVultureChance / 2);
                                else if(worldLoader.worldName == "GW" || worldLoader.worldName == "SL" || worldLoader.worldName == "LM" || worldLoader.worldName == "LC" || worldLoader.worldName == "MS")
                                    replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Vulture, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture,
                                        mirosVultureChance * 2);
                                else replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Vulture, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture,
                                        mirosVultureChance);
                                continue;
                            }
                            else
                                foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.KingVulture, extraKings);
                            if (foundType)
                            {
                                if ((extraVultures > 0 || extraKings > 0) && worldLoader.worldName == "SI")
                                    increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.KingVulture, 1);
                                if (worldLoader.worldName == "OE" || worldLoader.worldName == "SI")
                                    replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.KingVulture, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture,
                                        mirosVultureChance / 2);
                                else if (worldLoader.worldName == "GW" || worldLoader.worldName == "SL" || worldLoader.worldName == "LM" || worldLoader.worldName == "LC")
                                    replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.KingVulture, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture,
                                        mirosVultureChance * 2);
                                else replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.KingVulture, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture,
                                        mirosVultureChance);
                                continue;
                            }

                            if (worldLoader.worldName == "SH" && mirosVultureChance > 0)
                            {
                                foundType = increaseCreatureSpawner(simpleSpawner, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture, 2, 1);
                                if (foundType)
                                    continue;
                            }

                            //Scavengers
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Scavenger, extraScavengers);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Scavenger, MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite, eliteScavengerChance);
                                continue;
                            }

                            //Longlegs
                            foundType = replaceLongLegsSpawner(simpleSpawner, worldLoader.spawners, worldLoader.worldName);
                            if (foundType)
                                continue;

                            //Noodleflies
                            foundType = replaceNoodleFlySpawner(simpleSpawner, worldLoader.spawners);
                            if (foundType)
                                continue;

                            //Eggbugs
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.EggBug, extraEggbugs);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.EggBug, MoreSlugcatsEnums.CreatureTemplateType.FireBug, fireBugChance);
                                if (hasSporantulaMod)
                                    addSporantulaSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.EggBug, sporantulaChance);
                                continue;
                            }

                            //Tubeworms (check for inspector room)
                            if(simpleSpawner.creatureType == CreatureTemplate.Type.TubeWorm)
                            {
                                if (worldLoader.worldName == "LC" && simpleSpawner.den.ResolveRoomName() == "LC_station01" 
                                    && simpleSpawner.amount == 2 && UnityEngine.Random.value < inspectorChance * 10)
                                {
                                    World.SimpleSpawner spawner = CopySpawner(simpleSpawner);
                                    spawner.amount = 1;
                                    spawner.spawnDataString = "{Ignorecycle}";
                                    spawner.creatureType = MoreSlugcatsEnums.CreatureTemplateType.Inspector;
                                    spawner.inRegionSpawnerIndex = worldLoader.spawners.Count;
                                    worldLoader.spawners.Add(spawner);
                                }
                                continue;
                            }


                            //Mods
                            //Sporantula
                            if (hasSporantulaMod && simpleSpawner.amount > 1)
                            {
                                foundType = addSporantulaSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.SmallNeedleWorm, sporantulaChance);
                                if (foundType)
                                    continue;
                                foundType = increaseCreatureSpawner(simpleSpawner, new CreatureTemplate.Type("Sporantula"), extraSporantulas);
                            }

                        }

                        else if (worldLoader.spawners[i] is World.Lineage lineage)
                        {
                            foundType = replaceLizardLineage(lineage);
                            if (foundType)
                                continue;
                            foundType = replaceLongLegsLineage(lineage, worldLoader.spawners);
                            if (foundType)
                                continue;
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.SmallCentipede, CreatureTemplate.Type.Centipede, largeCentipedeChance);
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede, redCentipedeChance) || foundType;
                            if (foundType)
                                continue;
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, worldLoader.worldName == "SI" ? redCentipedeChance/2 : redCentipedeChance);
                            if (foundType)
                                continue;
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                            if (foundType)
                                continue;
                            foundType = replaceFireBugLineage(lineage);
                            if (foundType)
                                continue;
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.JetFish, (worldLoader.worldName == "SL")? CreatureTemplate.Type.BrotherLongLegs : MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, (worldLoader.worldName == "SB" || worldLoader.worldName == "VS")? waterPredatorChance*2 : (worldLoader.worldName == "SL")? waterPredatorChance/2 : waterPredatorChance);
                            if (foundType)
                                continue;
                            
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }

            Debug.Log("FINISHED SETTING UP SPAWNS.");
            Debug.Log("SPAWN AMOUNT: " + worldLoader.spawners.Count);

            /*
            Debug.Log("FINAL SPAWNS:");
            foreach(World.CreatureSpawner cSpawner in worldLoader.spawners)
            {
                if(cSpawner is World.SimpleSpawner simpleSpawner)
                {
                    Debug.Log("\n");
                    Debug.Log("SPAWNER DATA: " + simpleSpawner.ToString());
                    Debug.Log("CREATURE: " + simpleSpawner.creatureType);
                    Debug.Log("INDEX: " + simpleSpawner.inRegionSpawnerIndex);
                    Debug.Log("DEN: " + simpleSpawner.den.ToString());
                    Debug.Log("DEN ROOM: " + simpleSpawner.den.ResolveRoomName());
                    Debug.Log("AMOUNT: " + simpleSpawner.amount);
                    Debug.Log("SPAWN DATA STRING: " + simpleSpawner.spawnDataString);
                    if (simpleSpawner.nightCreature)
                        Debug.Log("IS NIGHT CREATURE.");
                }
            }//*/

            orig(worldLoader, fresh);

        }

        private void handlePrecycleSpawns(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.AquaCenti)
            {
                if (UnityEngine.Random.value < waterPredatorChance * 2)
                {
                    World.SimpleSpawner spawner = CopySpawner(simpleSpawner);
                    spawner.spawnDataString = " ";
                    spawner.amount = 1;
                    spawner.inRegionSpawnerIndex = spawners.Count;
                    spawners.Add(spawner);
                }
            }
            int extras = extraPrecycleSals;
            if (extras > 0 && simpleSpawner.creatureType == CreatureTemplate.Type.Salamander)
                extras++;
            increaseCreatureSpawner(simpleSpawner, simpleSpawner.creatureType, extras);
        }
        
        private bool handleLizardSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool isLizard = false, isYellow, isCyan, isAxo = false;

            if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard || simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard
                || simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard 
                || simpleSpawner.creatureType == CreatureTemplate.Type.WhiteLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard 
                || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard)
            {
                isLizard = true;
                increaseCreatureSpawner(simpleSpawner, simpleSpawner.creatureType, extraLizards);

                replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BlueLizard, CreatureTemplate.Type.CyanLizard, cyanLizChance);
                replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.GreenLizard, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, caramelLizChance);
                replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.PinkLizard, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, strawberryLizChance);
            }

            isYellow = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.YellowLizard, extraYellows);
            isCyan = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.CyanLizard, extraCyans);

            if (isYellow || isCyan || isLizard)
                replaceMultiSpawner(simpleSpawner, spawners, simpleSpawner.creatureType, CreatureTemplate.Type.RedLizard, isYellow ? redLizardChance / 2 : redLizardChance);

            else
            {
                isAxo = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Salamander, extraWaterLiz) ||
                        increaseCreatureSpawner(simpleSpawner, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, extraWaterLiz > 0 ? extraWaterLiz - 1 : 0);
            }

            if (!isLizard && !isYellow && !isCyan && !isAxo)
                isLizard = replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedLizard, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);
            
            return isLizard || isYellow || isCyan || isAxo;
        }

        private bool handleLeechSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool isLeech;

            isLeech = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Leech, extraLeeches);

            if (isLeech)
            {
                replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Leech, MoreSlugcatsEnums.CreatureTemplateType.JungleLeech, jungleLeechChance);
                if(UnityEngine.Random.value < leechLizardChance)
                {
                    World.SimpleSpawner replacementSpawner = CopySpawner(simpleSpawner);
                    replacementSpawner.creatureType = CreatureTemplate.Type.Salamander;
                    replacementSpawner.amount = 1;
                    simpleSpawner.amount = (int) (simpleSpawner.amount / 2);

                    replacementSpawner.inRegionSpawnerIndex = spawners.Count;
                    spawners.Add(replacementSpawner);
                }

            }
            else if(increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.SeaLeech, extraLeeches) ||
                increaseCreatureSpawner(simpleSpawner, MoreSlugcatsEnums.CreatureTemplateType.JungleLeech, extraLeeches))
            {
                isLeech = true;
                if (UnityEngine.Random.value < leechLizardChance)
                {
                    World.SimpleSpawner replacementSpawner = CopySpawner(simpleSpawner);
                    replacementSpawner.creatureType = MoreSlugcatsEnums.CreatureTemplateType.EelLizard;
                    replacementSpawner.amount = 1;
                    simpleSpawner.amount = (int)(simpleSpawner.amount / 2);

                    replacementSpawner.inRegionSpawnerIndex = spawners.Count;
                    spawners.Add(replacementSpawner);
                }
            }

            return isLeech;
        }

        private bool replaceLongLegsSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region)
        {
            if (simpleSpawner.creatureType == CreatureTemplate.Type.LanternMouse || simpleSpawner.creatureType == CreatureTemplate.Type.Snail)
            {
                float brotherChance = brotherLongLegsChance;
                if (region == "VS" || region == "CC" || region == "LM")
                    brotherChance *= 2;

                replaceMultiSpawner(simpleSpawner, spawners, simpleSpawner.creatureType, CreatureTemplate.Type.BrotherLongLegs, brotherChance);
                if(replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance))
                {
                    replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                    addInspectorSpawner(simpleSpawner, spawners, (region == "CC" || region == "SH" || region == "UW")? inspectorChance * 2 : inspectorChance);
                }
                return true;
            }
            else if(region == "SL" && replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.JetFish, CreatureTemplate.Type.BrotherLongLegs, brotherLongLegsChance))
            {
                if (replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance))
                {
                    replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                    addInspectorSpawner(simpleSpawner, spawners, inspectorChance);
                }

                return true;
            }
            else if(region == "UW" && replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SpitterSpider, CreatureTemplate.Type.DaddyLongLegs, brotherLongLegsChance))
            {
                if (increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.DaddyLongLegs, 1))
                {
                    replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                    addInspectorSpawner(simpleSpawner, spawners, inspectorChance * 2);
                }
                return true;
            }
            else if (replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance))
            {
                replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                addInspectorSpawner(simpleSpawner, spawners, (region == "CC" || region == "SH" || region == "UW") ? inspectorChance * 2 : inspectorChance);
                return true;
            }
            else if(increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.DaddyLongLegs, (brotherLongLegsChance > 0 && region == "UW")? 1 : 0))
            {
                replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                addInspectorSpawner(simpleSpawner, spawners, (region == "CC" || region == "SH" || region == "UW") ? inspectorChance * 2 : inspectorChance);
                return true;
            }
            else
            {
                return addInspectorSpawner(simpleSpawner, spawners, (region == "CC" || region == "SH" || region == "UW") ? inspectorChance * 2 : inspectorChance);
            }
        }
        
        private bool replaceNoodleFlySpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool isCicada = false;
            if (simpleSpawner.creatureType == CreatureTemplate.Type.CicadaA || simpleSpawner.creatureType == CreatureTemplate.Type.CicadaB)
            {
                isCicada = true;
                int rolls = 0;
                for (int i = 0; i < simpleSpawner.amount; ++i)
                {
                    if (UnityEngine.Random.value < noodleFlyChance)
                        rolls++;
                }

                if(rolls > 0)
                {
                    if (rolls == simpleSpawner.amount)
                        simpleSpawner.creatureType = CreatureTemplate.Type.BigNeedleWorm;
                    else
                    {
                        simpleSpawner.amount -= rolls;

                        World.SimpleSpawner adultSpawner = CopySpawner(simpleSpawner);
                        adultSpawner.amount = rolls;
                        adultSpawner.creatureType = CreatureTemplate.Type.BigNeedleWorm;
                        adultSpawner.inRegionSpawnerIndex = spawners.Count;
                        spawners.Add(adultSpawner);
                    }

                    World.SimpleSpawner infantSpawner = CopySpawner(simpleSpawner);
                    infantSpawner.amount = UnityEngine.Random.value < .5 ? 2 : 3;
                    infantSpawner.creatureType = CreatureTemplate.Type.SmallNeedleWorm;
                    infantSpawner.inRegionSpawnerIndex = spawners.Count;
                    spawners.Add(infantSpawner);

                }

            }

            return isCicada;
        }

        private bool replaceLizardLineage(World.Lineage lineage)
        {
            bool isLizard = false;

            for (int j = 0; j < lineage.creatureTypes.Length; j++)
            {
                //Red lizard replacement
                if (lineage.creatureTypes[j] == CreatureTemplate.Type.GreenLizard.Index || lineage.creatureTypes[j] == CreatureTemplate.Type.BlueLizard.Index
                    || lineage.creatureTypes[j] == CreatureTemplate.Type.PinkLizard.Index || lineage.creatureTypes[j] == CreatureTemplate.Type.WhiteLizard.Index
                    || lineage.creatureTypes[j] == CreatureTemplate.Type.YellowLizard.Index || lineage.creatureTypes[j] == CreatureTemplate.Type.BlackLizard.Index
                    || lineage.creatureTypes[j] == CreatureTemplate.Type.CyanLizard.Index || lineage.creatureTypes[j] == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard.Index
                    || lineage.creatureTypes[j] == MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard.Index)
                {
                    isLizard = true;
                    if (UnityEngine.Random.value < redLizardChance)
                    {
                        lineage.creatureTypes[j] = CreatureTemplate.Type.RedLizard.Index;
                    }
                    else if (lineage.creatureTypes[j] == CreatureTemplate.Type.GreenLizard.Index && UnityEngine.Random.value < caramelLizChance)
                        lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.SpitLizard.Index;
                    else if (lineage.creatureTypes[j] == CreatureTemplate.Type.PinkLizard.Index && UnityEngine.Random.value < strawberryLizChance)
                        lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard.Index;
                    else if (lineage.creatureTypes[j] == CreatureTemplate.Type.BlueLizard.Index && UnityEngine.Random.value < cyanLizChance)
                        lineage.creatureTypes[j] = CreatureTemplate.Type.CyanLizard.Index;
                }
                //Train lizard replacement
                if (lineage.creatureTypes[j] == CreatureTemplate.Type.RedLizard.Index)
                {
                    isLizard = true;
                    if (UnityEngine.Random.value < trainLizardChance)
                    {
                        lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.TrainLizard.Index;
                    }
                }
                
            }

            return isLizard;
        }

        private bool replaceLongLegsLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            bool isLongLegs;
            isLongLegs = replaceCreatureLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance);
            isLongLegs = replaceCreatureLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance) || isLongLegs;
            if (isLongLegs)
                addInspectorSpawner(lineage, spawners, inspectorChance);
            
            return isLongLegs;
        }

        private bool replaceFireBugLineage(World.Lineage lineage)
        {
            bool isEggBug = false;

            int i = 0;
            while(i < lineage.creatureTypes.Length && !isEggBug)
            {
                if (lineage.creatureTypes[i] == CreatureTemplate.Type.EggBug.Index)
                    isEggBug = true;
                i++;
            }

            if (isEggBug && fireBugChance > 0)
            {
                for (int j = i + 1; j < lineage.creatureTypes.Length; j++)
                    lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.FireBug.Index;
            }

            return isEggBug;
        }

        private bool replaceCreatureLineage(World.Lineage lineage, CreatureTemplate.Type replacee, CreatureTemplate.Type replacement, float chance)
        {
            bool isCreature = false;

            for(int i = 0; i < lineage.creatureTypes.Length; i++)
            {
                if (lineage.creatureTypes[i] == replacee.Index)
                {
                    isCreature = true;
                    if (UnityEngine.Random.value < chance)
                        lineage.creatureTypes[i] = replacement.Index;
                }
            }

            return isCreature;
        }
        
        private bool increaseCreatureSpawner(World.SimpleSpawner simpleSpawner, CreatureTemplate.Type creatureType, int amount = 1, int lesserAmount = 0)
        {
            bool isCreature = false;

            int finalAmount = (int)Mathf.Round(UnityEngine.Random.Range((float)lesserAmount - .5f, (float)amount + .5f));

            if (simpleSpawner.creatureType == creatureType)
            {
                isCreature = true;
                if (simpleSpawner.amount > 0)
                    simpleSpawner.amount += (int)Mathf.Round(UnityEngine.Random.Range((float)lesserAmount - .5f, (float)amount + .5f));
                  
            }

            return isCreature;
        }

        private bool replaceMultiSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type replacee, CreatureTemplate.Type replacement, float chance)
        {
            bool isCreature = false;

            if(simpleSpawner.creatureType == replacee)
            {
                isCreature = true;

                if(simpleSpawner.amount <= 1)
                {
                    if(UnityEngine.Random.value < chance)
                    {
                        simpleSpawner.creatureType = replacement;
                    }
                }
                else
                {
                    int winningRolls = 0;
                    for(int i = 0; i < simpleSpawner.amount; ++i)
                    {
                        if(UnityEngine.Random.value < chance)
                            winningRolls++;
                    }

                    if(winningRolls > 0)
                    {
                        if (winningRolls == simpleSpawner.amount)
                            simpleSpawner.creatureType = replacement;
                        else
                        {
                            World.SimpleSpawner replacementSpawner = CopySpawner(simpleSpawner);
                            replacementSpawner.creatureType = replacement;
                            replacementSpawner.amount = winningRolls;
                            simpleSpawner.amount -= winningRolls;

                            replacementSpawner.inRegionSpawnerIndex = spawners.Count;
                            spawners.Add(replacementSpawner);
                        }
                    }
                }
            }
            return isCreature;
        }

        private bool replaceSpiderSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, float chance)
        {
            bool isSpider = false;

            if(simpleSpawner.creatureType == CreatureTemplate.Type.Spider)
            {
                isSpider = true;
                if(UnityEngine.Random.value < chance)
                {
                    simpleSpawner.amount = (int)simpleSpawner.amount / 2;
                    World.SimpleSpawner motherSpawner = CopySpawner(simpleSpawner);
                    motherSpawner.amount = 1;
                    motherSpawner.creatureType = MoreSlugcatsEnums.CreatureTemplateType.MotherSpider;
                    motherSpawner.inRegionSpawnerIndex = spawners.Count;
                    spawners.Add(motherSpawner);
                }
            }

            return isSpider;
        }
       
        private bool addInspectorSpawner(World.CreatureSpawner creatureSpawner, List<World.CreatureSpawner> spawners, float chance)
        {
            if (creatureSpawner is World.SimpleSpawner spawner)
            {
                bool foundType = spawner.creatureType == CreatureTemplate.Type.BrotherLongLegs || spawner.creatureType == CreatureTemplate.Type.DaddyLongLegs
                                || spawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs;
                if (foundType && UnityEngine.Random.value < chance)
                {
                    World.SimpleSpawner inspectorSpawner = CopySpawner(spawner);
                    inspectorSpawner.amount = 1;
                    inspectorSpawner.creatureType = MoreSlugcatsEnums.CreatureTemplateType.Inspector;
                    inspectorSpawner.inRegionSpawnerIndex = spawners.Count;
                    spawners.Add(inspectorSpawner);
                }
                return foundType;
            }
            else if (creatureSpawner is World.Lineage lineage)
            {
                bool foundType = false;
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (foundType = lineage.creatureTypes[i] == CreatureTemplate.Type.BrotherLongLegs.Index || lineage.creatureTypes[i] == CreatureTemplate.Type.DaddyLongLegs.Index
                        || lineage.creatureTypes[i] == MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs.Index)
                        break;
                }
                if (foundType && UnityEngine.Random.value < chance)
                {
                    World.SimpleSpawner inSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, MoreSlugcatsEnums.CreatureTemplateType.Inspector, "{Ignorecycle}", 1);
                    inSpawner.nightCreature = false;
                    spawners.Add(inSpawner);
                }
                return foundType;
            }
            else return false;
            
        }
        private bool addSporantulaSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type replacee, float chance)
        {
            bool isCreature = false;
            bool winningRoll = false;

            if(simpleSpawner.creatureType == replacee)
            {
                isCreature = true;
                for (int i = 0; i < simpleSpawner.amount; ++i)
                {
                    if (UnityEngine.Random.value < chance)
                        winningRoll = true;
                }
            }
            
            if (winningRoll)
            {
                World.SimpleSpawner sporantulaSpawner = CopySpawner(simpleSpawner);
                sporantulaSpawner.amount = 1;
                sporantulaSpawner.creatureType = new CreatureTemplate.Type("Sporantula");
                sporantulaSpawner.inRegionSpawnerIndex = spawners.Count;
                spawners.Add(sporantulaSpawner);
            }
            
            return isCreature;
        }
        private World.SimpleSpawner CopySpawner(World.SimpleSpawner origSpawner)
        {
            World.SimpleSpawner newSpawner = new World.SimpleSpawner(origSpawner.region, origSpawner.inRegionSpawnerIndex, origSpawner.den,
                origSpawner.creatureType, origSpawner.spawnDataString, origSpawner.amount);

            return newSpawner;
        }
        


    }
}
using BepInEx;
using System.Security.Permissions;
using System.Security;
using System;
using UnityEngine;
using MoreSlugcats;
using System.Collections.Generic;
using System.Security.AccessControl;
using Steamworks;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ApexUpYourSpawns
{
    [BepInPlugin("ShinyKelp.ApexUpYourSpawns", "ApexUpYourSpawns", "1.2.2")]

    public class ApexUpYourSpawnsMod : BaseUnityPlugin
    {
        private bool forceFreshSpawns, fillLineages;

        private float redLizardChance, trainLizardChance, largeCentipedeChance, redCentipedeChance, spitterSpiderChance, kingVultureChance,
            mirosVultureChance, eliteScavengerChance, brotherLongLegsChance, daddyLongLegsChance, terrorLongLegsChance, flyingPredatorChance,
            fireBugChance, giantJellyfishChance, leechLizardChance, yeekLizardChance, waterPredatorChance, caramelLizChance, strawberryLizChance,
            cyanLizChance, eelLizChance, jungleLeechChance, motherSpiderChance, stowawayChance;

        private int extraYellows, extraLizards, extraCyans, extraWaterLiz, extraSpiders, extraVultures, extraScavengers, extraSmallCents, extraCentipedes,
            extraCentiwings, extraAquapedes, extraPrecycleSals, extraDropwigs, extraMiros, extraSmallSpiders, extraLeeches, extraKelp, extraLeviathans,
            extraEggbugs, extraCicadas, extraLMice, extraSnails, extraJetfish, extraYeeks, extraNightCreatures;

        //Mod dependent
        private float inspectorChance, sporantulaChance, scutigeraChance, redHorrorCentiChance, longlegsVariantChance, waterSpitterChance, fatFireFlyChance;
        private int extraSporantulas, extraScutigeras, extraWaterSpitters;

        private bool IsInit;

        public bool hasSporantula, hasAngryInspectors, hasRedHorrorCentipede, hasScutigera, hasWaterSpitter, hasExplosiveDLL, hasMoreDLLs, hasFatFirefly;

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
                On.JellyFish.PlaceInRoom += ReplaceGiantJellyfish;
                On.DangleFruit.PlaceInRoom += ReplaceStowawayBugBlueFruit;
                On.MoreSlugcats.GooieDuck.PlaceInRoom += ReplaceStowawayBugGooieDuck;

                hasSporantula = hasAngryInspectors = hasRedHorrorCentipede = hasScutigera = hasWaterSpitter = hasMoreDLLs = hasExplosiveDLL = false;
                foreach(ModManager.Mod mod in ModManager.ActiveMods)
                {
                    if (mod.name == "Sporantula")
                    {
                        hasSporantula = true;
                        continue;
                    }
                    if(mod.name == "Angry Inspectors")
                    {
                        hasAngryInspectors = true;
                        continue;
                    }
                    if(mod.name == "Scutigera")
                    {
                        hasScutigera = true;
                        continue;
                    }
                    if(mod.name == "Red Horror Centipede")
                    {
                        hasRedHorrorCentipede = true;
                        continue;
                    }
                    if(mod.name == "Water Spitter")
                    {
                        hasWaterSpitter = true;
                        continue;
                    }
                    if(mod.name == "More Dlls")
                    {
                        hasMoreDLLs = true;
                        continue;
                    }
                    if(mod.name == "Explosive DLLs")
                    {
                        hasExplosiveDLL = true;
                        continue;
                    }
                    if(mod.name == "Fat Firefly")
                    {
                        hasFatFirefly = true;
                        continue;
                    }
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

        private void SetOptions()
        {
            fillLineages = options.fillLineages.Value;
            forceFreshSpawns = options.forceFreshSpawns.Value;

            redLizardChance = (float)options.redLizardChance.Value / 100;
            trainLizardChance = (float)options.trainLizardChance.Value / 100;
            largeCentipedeChance = (float)options.largeCentipedeChance.Value / 100;
            redCentipedeChance = (float)options.redCentipedeChance.Value / 100;
            spitterSpiderChance = (float)options.spitterSpiderChance.Value / 100;
            kingVultureChance = (float)options.kingVultureChance.Value / 100;
            mirosVultureChance = (float)options.mirosVultureChance.Value / 100;
            eliteScavengerChance = (float)options.eliteScavengerChance.Value / 100;
            brotherLongLegsChance = (float)options.brotherLongLegsChance.Value / 100;
            daddyLongLegsChance = (float)options.daddyLongLegsChance.Value / 100;
            terrorLongLegsChance = (float)options.terrorLongLegsChance.Value / 100;
            fireBugChance = (float)options.fireBugChance.Value / 100;
            flyingPredatorChance = (float) options.flyingPredatorChance.Value / 100;
            leechLizardChance = (float)options.leechLizardChance.Value / 100;
            yeekLizardChance = (float)options.yeekLizardChance.Value / 100;
            waterPredatorChance = (float)options.waterPredatorChance.Value / 100;
            giantJellyfishChance = (float)options.giantJellyfishChance.Value / 100;
            caramelLizChance = (float)options.caramelLizChance.Value / 100;
            strawberryLizChance = (float)options.strawberryLizChance.Value / 100;
            cyanLizChance = (float)options.cyanLizChance.Value / 100;
            eelLizChance = (float)options.eelLizChance.Value / 100;
            jungleLeechChance = (float)options.jungleLeechChance.Value / 100;
            motherSpiderChance = (float)options.motherSpiderChance.Value / 100;
            stowawayChance = (float)options.stowawayChance.Value / 100;
            //Mod dependant
            inspectorChance = (float)options.inspectorChance.Value / 100;
            sporantulaChance = (float)options.sporantulaChance.Value / 100;
            scutigeraChance = (float)options.scutigeraChance.Value / 100;
            redHorrorCentiChance = (float)options.redHorrorCentiChance.Value / 100;
            longlegsVariantChance = (float)options.longlegsVariantChance.Value / 100;
            waterSpitterChance = (float)options.waterSpitterChance.Value / 100;
            fatFireFlyChance = (float)options.fatFireFlyChance.Value / 100;

            extraYellows = options.yellowLizExtras.Value;
            extraLizards = options.genericLizExtras.Value;
            extraCyans = options.cyanLizExtras.Value;
            extraWaterLiz = options.waterLizExtras.Value;
            extraSpiders = options.bigSpiderExtras.Value;
            extraVultures = options.vultureExtras.Value;
            extraScavengers = options.scavengerExtras.Value;
            extraSmallCents = options.smallCentExtras.Value;
            extraEggbugs = options.eggbugExtras.Value;
            extraCicadas = options.cicadaExtras.Value;
            extraSnails = options.snailExtras.Value;
            extraJetfish = options.jetfishExtras.Value;
            extraYeeks = options.yeekExtras.Value;
            extraLMice = options.lmiceExtras.Value;
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
            extraNightCreatures = options.nightCreatureExtras.Value;
            //Mod dependant
            extraSporantulas = options.sporantulaExtras.Value;
            extraScutigeras = options.scutigeraExtras.Value;
            extraWaterSpitters = options.waterSpitterExtras.Value;
        }
        private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            ClearMemory();
        }
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            this.game = game;
            SetOptions();
            //ClearMemory();
        }

        #region Helper Methods

        private void ClearMemory()
        {
            //Clear lists, dictionaries, etc. No such thing here, yet.
        }
        #endregion

        private void ReplaceGiantJellyfish(On.JellyFish.orig_PlaceInRoom orig, JellyFish self, Room room)
        {

            if (game.IsStorySession && !room.abstractRoom.shelter && UnityEngine.Random.value < giantJellyfishChance)
            {
                AbstractCreature myBigJelly = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.BigJelly), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), game.GetNewID());
                BigJellyFish myJelly = new BigJellyFish(myBigJelly, game.world);
                myJelly.PlaceInRoom(room);
            }
            else
                orig(self, room);
        }

        private void ReplaceStowawayBugBlueFruit(On.DangleFruit.orig_PlaceInRoom orig, DangleFruit self, Room room)
        {
            if(game.IsStorySession && !room.abstractRoom.shelter && UnityEngine.Random.value < stowawayChance)
            {
                self.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(self, room, self.firstChunk.pos);

                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.StowawayBug), 
                    null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), game.GetNewID());

                Vector2 pos = new Vector2((self.abstractPhysicalObject.pos.x+1) * 20f - 10f, (self.abstractPhysicalObject.pos.y+1) * 20f - 20f + stalk.ropeLength);
                
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

        private void ReplaceStowawayBugGooieDuck(On.MoreSlugcats.GooieDuck.orig_PlaceInRoom orig, GooieDuck self, Room room)
        {
            if (game.IsStorySession && !room.abstractRoom.shelter && UnityEngine.Random.value < stowawayChance * 2)
            {
                DangleFruit fruit = new DangleFruit(self.abstractPhysicalObject);
                fruit.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(fruit, room, fruit.firstChunk.pos);
                AbstractCreature myStowawayAbstract = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.StowawayBug), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), game.GetNewID());
                Vector2 pos = new Vector2((self.abstractPhysicalObject.pos.x + 1) * 20f - 10f, (self.abstractPhysicalObject.pos.y + 1) * 20f - 20f + stalk.ropeLength);
                
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

            if (forceFreshSpawns && !fresh)
            {
                fresh = true;
                foreach (AbstractRoom abstractRoom in worldLoader.abstractRooms)
                {
                    if (!abstractRoom.shelter)
                    {
                        abstractRoom.creatures.Clear();
                        abstractRoom.entitiesInDens.Clear();
                        Debug.Log(abstractRoom.index);
                    }
                }
            }

            try
            {
                if (fresh)
                {
                    string storySession = "null";
                    if (game.IsStorySession)
                        storySession = game.GetStorySession.saveState.saveStateNumber.ToString();
                    Debug.Log("Starting spawns for region: " + worldLoader.worldName + " , character: " + 
                        storySession);
                    Debug.Log("ORIGINAL SPAWN COUNT: " + worldLoader.spawners.Count);
                    Debug.Log("\n");
                    UnityEngine.Random.InitState(Mathf.RoundToInt(Time.time * 10f));
                    HandleAllSpawners(worldLoader, worldLoader.spawners, worldLoader.worldName, worldLoader.world.firstRoomIndex);
                    Debug.Log("Finished setting up spawns.");
                    Debug.Log("FINAL SPAWN COUNT: " + worldLoader.spawners.Count);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }

            orig(worldLoader, fresh);

        }
        
        private void HandleAllSpawners(WorldLoader worldLoader, List<World.CreatureSpawner> spawners, string region, int firstRoomIndex)
        {
            int originalSpawnerCount = spawners.Count;
            for (int i = 0; i < spawners.Count; i++)
            {
                if(spawners[i].den.room < worldLoader.world.firstRoomIndex || 
                    spawners[i].den.room >= worldLoader.world.firstRoomIndex + worldLoader.world.NumberOfRooms){
                    Debug.Log("!!! ERROR SPAWNER FOUND !!!");
                    LogSpawner(spawners[i], i);
                    continue;
                }
                //Log Spawners
                if (i > 0)
                {
                    Debug.Log("==AFTER TRANSFORMATIONS==");
                    LogSpawner(spawners[i - 1], i - 1, worldLoader.abstractRooms[spawners[i - 1].den.room - firstRoomIndex].subregionName);
                }
                LogSpawner(spawners[i], i, worldLoader.abstractRooms[spawners[i].den.room - firstRoomIndex].subregionName);
                //*/
                if (spawners[i] is World.SimpleSpawner simpleSpawner)
                {   
                    if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle"))
                        HandlePrecycleSpawns(simpleSpawner);

                    if ((simpleSpawner.nightCreature || (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("Night"))) 
                        && (simpleSpawner.spawnDataString is null || !simpleSpawner.spawnDataString.Contains("Ignorecycle")))
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraNightCreatures);
                        if (hasAngryInspectors) 
                        {
                            if((region == "LC" && simpleSpawner.den.ResolveRoomName() != "LCOffScreenDen") || 
                                (region == "UW" && simpleSpawner.den.ResolveRoomName() != "UWOffScreenDen"))
                            {
                                bool addedSpawner =
                                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, inspectorChance / 2, true);
                                if (addedSpawner)
                                    (spawners[spawners.Count - 1] as World.SimpleSpawner).spawnDataString = "{Ignorecycle}";
                            }
                        } 
                    }

                    if (IsLizard(simpleSpawner))
                    {
                        HandleLizardSpawner(simpleSpawner, spawners, region);
                        continue;
                    }

                    if (IsCentipede(simpleSpawner))
                    {
                        if (hasAngryInspectors && worldLoader.abstractRooms[spawners[i].den.room - firstRoomIndex].subregionName == "Memory Crypts"
                            && simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, inspectorChance * 4);
                        HandleCentipedeSpawner(simpleSpawner, spawners, region);
                        if (hasSporantula && (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede || 
                            simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede))
                            AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Sporantula"), sporantulaChance);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture || simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture
                        || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture)
                    {
                        HandleVultureSpawner(simpleSpawner, spawners, region);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    {
                        if(region == "GW")
                            IncreaseCreatureSpawner(simpleSpawner, extraSpiders*2);
                        else
                        {
                            IncreaseCreatureSpawner(simpleSpawner, region == "SB"? extraSpiders-1 : extraSpiders);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                        }
                        //Sporantula
                        if (hasSporantula)
                            AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Sporantula"), sporantulaChance);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.SpitterSpider)
                    {
                        if (region == "GW")
                            IncreaseCreatureSpawner(simpleSpawner, extraSpiders);
                        if (region == "UW" || region == "CL" || region == "GW")
                            HandleLongLegsSpawner(simpleSpawner, spawners, region);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Spider)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraSmallSpiders);
                        AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.MotherSpider, motherSpiderChance, true);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.DropBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraDropwigs);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigEel)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraLeviathans);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.MirosBird)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraMiros);
                        if (region == "SB" && extraMiros > 0)
                            IncreaseCreatureSpawner(simpleSpawner, 2);
                        if (region == "LC" && extraMiros > 0)
                            IncreaseCreatureSpawner(simpleSpawner, 4);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Scavenger)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (game.IsStorySession && 
                            game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer) ?
                            extraScavengers/2 : extraScavengers);
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite, eliteScavengerChance);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, region == "SL" ? extraJetfish-1 : extraJetfish);
                        HandleJetfishSpawner(simpleSpawner, spawners, region);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.TentaclePlant)
                    {
                        if (hasAngryInspectors && region == "SB" && simpleSpawner.den.ResolveRoomName() == "SB_G03")
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, inspectorChance * 4);
                        IncreaseCreatureSpawner(simpleSpawner, extraKelp);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Leech || simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech 
                        || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.JungleLeech)
                    {
                        HandleLeechSpawner(simpleSpawner, spawners);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.EggBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraEggbugs);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.FireBug, fireBugChance);
                        if (hasSporantula && !replacedFull)
                            AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Sporantula"), sporantulaChance);
                        continue;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.TubeWorm)
                    {
                        if (hasAngryInspectors && region == "LC" && simpleSpawner.den.ResolveRoomName() == "LC_station01")
                        {
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, inspectorChance * 4);
                        }
                        continue;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.CicadaA || simpleSpawner.creatureType == CreatureTemplate.Type.CicadaB)
                    {
                        HandleCicadaSpawner(simpleSpawner, spawners, region);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Snail)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, region == "DS" ? extraSnails-1 : extraSnails);
                        HandleLongLegsSpawner(simpleSpawner, spawners, region);
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.LanternMouse)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, region == "SH" ? extraLMice - 1 : extraLMice);
                        HandleLongLegsSpawner(simpleSpawner, spawners, region);
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= originalSpawnerCount)
                            AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SmallNeedleWorm, 1f);
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.SmallNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= originalSpawnerCount && simpleSpawner.amount < 2)
                            IncreaseCreatureSpawner(simpleSpawner, 2);
                    }

                    if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.Yeek)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, region == "OE" ? extraYeeks - 1 : extraYeeks);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, UnityEngine.Random.value < (region == "OE" ? .8f : .5f)? 
                            MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard : MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, yeekLizardChance);
                        if (replacedFull)
                            HandleLizardSpawner(simpleSpawner, spawners, region);
                        continue;
                    }

                    if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.Inspector)
                    {
                        if (hasAngryInspectors)
                            simpleSpawner.spawnDataString = "{Ignorecycle}";
                        continue;
                    }

                    if (hasSporantula)
                    {
                        if(simpleSpawner.creatureType == new CreatureTemplate.Type("Sporantula"))
                        {
                            IncreaseCreatureSpawner(simpleSpawner, extraSporantulas);
                        }
                    }
                }
                else if(spawners[i] is World.Lineage lineage)
                {

                    if(fillLineages)
                        FillLineage(lineage);
                    if (forceFreshSpawns && fillLineages)
                        RandomizeLineageFirst(lineage);

                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, true))
                    {
                        HandleLizardLineage(lineage, spawners, region);
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, true))
                    {
                        HandleLongLegsLineage(lineage, spawners, region);
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, true))
                    {
                        HandleCentipedeLineage(lineage);
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.BigSpider))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                        if(forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BigSpider.Index)
                        {
                            World.SimpleSpawner asimpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BigSpider, lineage.spawnData[0], 1);
                            spawners.Add(asimpleSpawner);
                        }
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.JetFish))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, waterPredatorChance);
                        if(region == "SL")
                        {
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, CreatureTemplate.Type.BrotherLongLegs, waterPredatorChance);
                            HandleLongLegsLineage(lineage, spawners, region);
                        }
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.EggBug))
                    {
                        lineage.creatureTypes[0] = CreatureTemplate.Type.EggBug.Index;
                        for(int j = 1; j < lineage.creatureTypes.Length; ++j)
                        {
                            lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.FireBug.Index;
                        }
                        continue;
                    }
                }
            }
        }

        private bool IsLizard(World.SimpleSpawner spawner)
        {

            return StaticWorld.GetCreatureTemplate(spawner.creatureType).TopAncestor().type == 
                CreatureTemplate.Type.LizardTemplate;
        }
        
        private bool IsCentipede(World.SimpleSpawner spawner)
        {
            return StaticWorld.GetCreatureTemplate(spawner.creatureType).TopAncestor().type ==
                CreatureTemplate.Type.Centipede;
                
        }

        private void HandleLizardSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region)
        {
            if(simpleSpawner.creatureType == CreatureTemplate.Type.Salamander
               || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard
               || (hasWaterSpitter && simpleSpawner.creatureType == new CreatureTemplate.Type("WaterSpitter")))
            {
                HandleAxolotlSpawner(simpleSpawner, spawners);
                return;
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard || simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard
                || simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard
                || simpleSpawner.creatureType == CreatureTemplate.Type.WhiteLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard
                || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraLizards);
                if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, caramelLizChance);
                else if(simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, caramelLizChance);
                else if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.CyanLizard, caramelLizChance);
                }

            float localRedLizardChance = redLizardChance;

            if(simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard)
            {
                localRedLizardChance /= 2;
                IncreaseCreatureSpawner(simpleSpawner, extraYellows);
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                IncreaseCreatureSpawner(simpleSpawner, extraCyans);
            
            if(simpleSpawner.creatureType != CreatureTemplate.Type.RedLizard)
            {
                bool replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedLizard, localRedLizardChance);
                if (replacedFull)
                    ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);
            }
            else
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);

            if (region == "GW" && game.IsStorySession && (game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                    AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.WhiteLizard, redLizardChance * 4);
            }
        }

        private void HandleAxolotlSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Salamander)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraWaterLiz);
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, eelLizChance);
            }
            if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraWaterLiz > 0 ? extraWaterLiz - 1 : 0);
            }

            if (hasWaterSpitter)
            {
                if (simpleSpawner.creatureType != new CreatureTemplate.Type("WaterSpitter"))
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("WaterSpitter"), waterSpitterChance);
                if (simpleSpawner.creatureType == new CreatureTemplate.Type("WaterSpitter"))
                    IncreaseCreatureSpawner(simpleSpawner, extraWaterSpitters);
            }

        }

        private void HandleCentipedeSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region)
        {
            if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.AquaCenti)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraAquapedes);
                return;
            }
            bool wasSmallCentipedes = false;
            if(simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
            {
                wasSmallCentipedes = true;
                IncreaseCreatureSpawner(simpleSpawner, (region == "OE" || region == "SB" || region == "VS")? extraSmallCents-1 : extraSmallCents);
                if(!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("AlternateForm"))
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, largeCentipedeChance);
                else
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, largeCentipedeChance);

            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
            {
                if(!wasSmallCentipedes)
                    IncreaseCreatureSpawner(simpleSpawner, (region == "SB" || region == "VS")? extraCentipedes-1 : extraCentipedes);
                bool replacedFull =
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, (region == "VS" || region == "SB") ? redCentipedeChance / 2 : redCentipedeChance);
                //Scugitera chance
                if (hasScutigera && !replacedFull)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Scutigera"), scutigeraChance);
            }
            bool isCentiwing = simpleSpawner.creatureType == CreatureTemplate.Type.Centiwing;
            if(isCentiwing)
            {
                if(!wasSmallCentipedes)
                    IncreaseCreatureSpawner(simpleSpawner, extraCentiwings);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, region == "LC"? redCentipedeChance*2 : redCentipedeChance);
            }

            if(hasScutigera && simpleSpawner.creatureType == new CreatureTemplate.Type("Scutigera"))
            {
                IncreaseCreatureSpawner(simpleSpawner, extraScutigeras);
            }

            if(hasRedHorrorCentipede && simpleSpawner.creatureType == CreatureTemplate.Type.RedCentipede)
            {
                float localRedHorrorChance = redHorrorCentiChance;
                if (region == "SI" || region == "LC" || isCentiwing)
                    localRedHorrorChance *= 2f;
                ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("RedHorrorCenti"), localRedHorrorChance);

            }

            if(region == "GW" && game.IsStorySession && (game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, largeCentipedeChance * 2);
                if (hasScutigera && simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Scutigera"), scutigeraChance * 2);
                if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            }
        }

        private void HandleVultureSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region)
        {

            if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture)
            {
                if (extraVultures > 0) 
                {
                    IncreaseCreatureSpawner(simpleSpawner, extraVultures);
                    if (region == "OE")
                        IncreaseCreatureSpawner(simpleSpawner, 2);
                    if (region == "SI")
                        IncreaseCreatureSpawner(simpleSpawner, 3);
                }
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.KingVulture, region == "SI"? kingVultureChance*2 : kingVultureChance);
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture && region == "UW")
                IncreaseCreatureSpawner(simpleSpawner, extraVultures);

            float localMirosVultureChance = mirosVultureChance;
            if (region == "OE" || region == "SI")
                localMirosVultureChance /= 2;
            else if (region == "SL" || region == "LM" || region == "LC" || region == "MS" ||
                (region == "GW" && game.IsStorySession && (game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Spear)))
                localMirosVultureChance *= 2;
            
            ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture, localMirosVultureChance);

            if (region == "SH" && simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture && mirosVultureChance > 0)
                IncreaseCreatureSpawner(simpleSpawner, 2);

            if (hasFatFirefly && (simpleSpawner.creatureType == CreatureTemplate.Type.Vulture ||
                simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture))
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("FatFireFly"), fatFireFlyChance);
            }

        }
        
        private void HandleLongLegsSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region, string subregion = "")
        {
            if (!(StaticWorld.creatureTemplates[simpleSpawner.creatureType.Index].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs))
            {
                if(region == "UW" || region == "CL")
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, brotherLongLegsChance * 2);
                }
                else if (region == "GW" && simpleSpawner.creatureType == CreatureTemplate.Type.SpitterSpider && game.IsStorySession &&
                    (game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Spear))
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, brotherLongLegsChance * 2);
                }
                else
                {
                    float localBrotherChance = brotherLongLegsChance;
                    if (subregion == "Sump Tunnel" || subregion == "The Gutter" || region == "LM" || 
                        (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle")) || 
                        simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                        localBrotherChance *= 2;
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, localBrotherChance);
                }
            }

            if(simpleSpawner.creatureType == CreatureTemplate.Type.BrotherLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance);
            }
            
            if(simpleSpawner.creatureType == CreatureTemplate.Type.DaddyLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
            }

            if ((hasMoreDLLs || hasExplosiveDLL) && (simpleSpawner.creatureType == CreatureTemplate.Type.BrotherLongLegs
                    || simpleSpawner.creatureType == CreatureTemplate.Type.DaddyLongLegs))
            {
                if (!hasMoreDLLs)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("ExplosiveDLL"), longlegsVariantChance);
                else
                {
                    if (UnityEngine.Random.value < 0.5f)
                    {
                        if (hasExplosiveDLL)
                            ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("ExplosiveDLL"), longlegsVariantChance);
                        else
                            ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("ExplosiveDaddyLongLegs"), longlegsVariantChance);
                    }
                    else
                        ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("ZapDaddyLongLegs"), longlegsVariantChance);
                }
            }
            if (hasAngryInspectors &&
                StaticWorld.creatureTemplates[simpleSpawner.creatureType.Index].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
            {
                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, inspectorChance);
            }


        }

        private void HandleJetfishSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region)
        {
            if (waterPredatorChance > 0 && (region == "LM" || region == "SB" || region == "VS"))
                IncreaseCreatureSpawner(simpleSpawner, 1);

            float localWaterPredatorChance = waterPredatorChance;
            if (region == "SB" || region == "VS")
                localWaterPredatorChance *= 2f;
            else if (region == "SL")
                localWaterPredatorChance *= 0.8f;

            bool replacedFull;
            replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, localWaterPredatorChance);

            if ((region == "SL" || region == "CL") && !replacedFull && waterPredatorChance > 0)
                HandleLongLegsSpawner(simpleSpawner, spawners, region);


        }
        
        private void HandleCicadaSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region)
        {
            IncreaseCreatureSpawner(simpleSpawner, (region == "SI" || region == "OE")? extraCicadas - 1 : extraCicadas);
            if (flyingPredatorChance > 0)
            {
                float localSelector = .5f;
                if (region == "SI" || region == "LC")
                    localSelector = .75f;
                if (UnityEngine.Random.value < localSelector)
                {
                    bool replacedFull =
                        ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigNeedleWorm, flyingPredatorChance);
                    if (replacedFull)
                        AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SmallNeedleWorm, 1f);
                }
                else
                {
                    bool replacedFull =
                        ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, flyingPredatorChance);
                    if (replacedFull)
                        HandleCentipedeSpawner(simpleSpawner, spawners, region);
                }
            }
            if(hasSporantula && (simpleSpawner.creatureType == CreatureTemplate.Type.CicadaA 
                || simpleSpawner.creatureType == CreatureTemplate.Type.CicadaB))
            {
                AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Sporantula"), sporantulaChance);
            }
        }

        private void HandleLeechSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool wasRedLeech = false;
            if(simpleSpawner.creatureType == CreatureTemplate.Type.Leech)
            {
                wasRedLeech = true;
                IncreaseCreatureSpawner(simpleSpawner, extraLeeches);
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.JungleLeech, jungleLeechChance);
                AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, leechLizardChance, true);
            }
            if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.JungleLeech)
            {
                if(!wasRedLeech)
                    AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, leechLizardChance, true);
            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraLeeches);
                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, leechLizardChance, true);
            }
        }

        private void HandlePrecycleSpawns(World.SimpleSpawner simpleSpawner)
        {
            int extras = extraPrecycleSals;
            if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard ||
                StaticWorld.creatureTemplates[simpleSpawner.creatureType.Index].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                extras--;
            IncreaseCreatureSpawner(simpleSpawner, extras);
        }

        private void IncreaseCreatureSpawner(World.SimpleSpawner simpleSpawner, int amount = 1, int lesserAmount = 0)
        {
            if (amount <= 0)
                return;
            if (simpleSpawner.amount > 0)
                simpleSpawner.amount += (int)Mathf.Round(UnityEngine.Random.Range((float)lesserAmount - .5f, (float)amount + .5f));

        }

        private bool ReplaceMultiSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type replacement, float chance)
        {
            bool replacedFull = false;

            if (simpleSpawner.amount <= 1)
            {
                if (UnityEngine.Random.value < chance)
                {
                    simpleSpawner.creatureType = replacement;
                    replacedFull = true;
                }
            }
            else
            {
                int winningRolls = 0;
                for (int i = 0; i < simpleSpawner.amount; ++i)
                {
                    if (UnityEngine.Random.value < chance)
                        winningRolls++;
                }

                if (winningRolls > 0)
                {
                    if (winningRolls == simpleSpawner.amount)
                    {
                        simpleSpawner.creatureType = replacement;
                        replacedFull = true;
                    }
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
            return replacedFull;
        }
    
        private bool AddInvasionSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type invador, float chance, bool singleRoll = false)
        {
            bool wonRoll = false;

            if (singleRoll)
                wonRoll = UnityEngine.Random.value < chance;
            else
            {
                for (int i = 0; i < simpleSpawner.amount; ++i)
                {
                    if (UnityEngine.Random.value < chance)
                    {
                        wonRoll = true;
                        break;
                    }
                }
            }

            if (wonRoll)
            {
                World.SimpleSpawner invasionSpawner = CopySpawner(simpleSpawner);
                invasionSpawner.creatureType = invador;
                invasionSpawner.amount = 1;
                invasionSpawner.inRegionSpawnerIndex = spawners.Count;
                spawners.Add(invasionSpawner);
                if (singleRoll)
                    simpleSpawner.amount = (int)Mathf.Round(simpleSpawner.amount * 0.75f);
            }
            return wonRoll;
        }

        private void FillLineage(World.Lineage lineage)
        {

            int fillCreature = -1;
            int i = 0;
            while (fillCreature == -1 && i < lineage.creatureTypes.Length)
            {
                fillCreature = lineage.creatureTypes[i];
                ++i;
            }

            if (fillCreature != -1)
            {
                for(int j = 0; j < lineage.creatureTypes.Length; ++j)
                {
                    if (lineage.creatureTypes[j] == -1)
                        lineage.creatureTypes[j] = fillCreature;
                    else
                        fillCreature = lineage.creatureTypes[j];
                }
            }
        }

        private void RandomizeLineageFirst(World.Lineage lineage)
        {
            int n = lineage.creatureTypes.Length;
            int indexToCopy = (int)Mathf.Round(UnityEngine.Random.Range(-0.5f, n - 0.5f));
            lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy];
            lineage.spawnData[0] = lineage.spawnData[indexToCopy];
            if(indexToCopy == n - 1 && n > 1)
            {
                if(lineage.creatureTypes[indexToCopy] == CreatureTemplate.Type.RedLizard.Index)
                {
                    if (UnityEngine.Random.value > redLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard.Index)
                {
                    if (UnityEngine.Random.value > trainLizardChance || UnityEngine.Random.value > redLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == CreatureTemplate.Type.RedCentipede.Index)
                {
                    if (UnityEngine.Random.value > redCentipedeChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
            }
        }

        private void HandleLizardLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners, string region)
        {
            if(IsCreatureInLineage(lineage, CreatureTemplate.Type.Salamander) || IsCreatureInLineage(lineage, MoreSlugcatsEnums.CreatureTemplateType.EelLizard))
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, eelLizChance);
                if (hasWaterSpitter)
                {
                    ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, new CreatureTemplate.Type("WaterSpitter"), waterSpitterChance);
                    ReplaceCreatureInLineage(lineage, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, new CreatureTemplate.Type("WaterSpitter"), waterSpitterChance);
                }
                return;
            }
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, CreatureTemplate.Type.RedLizard, redLizardChance, true);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, caramelLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BlueLizard, CreatureTemplate.Type.CyanLizard, cyanLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.PinkLizard, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, strawberryLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);
            
            if(forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.YellowLizard.Index && extraYellows > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.YellowLizard, lineage.spawnData[0], 1);
                spawners.Add(simpleSpawner);
            }
            else if (forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.Salamander.Index && extraWaterLiz > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.YellowLizard, lineage.spawnData[0], 1);
                spawners.Add(simpleSpawner);
            }
            else if (forceFreshSpawns && fillLineages && lineage.creatureTypes[0] == CreatureTemplate.Type.BlackLizard.Index && region == "SH" && UnityEngine.Random.value > .5f)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.YellowLizard, lineage.spawnData[0], 1);
                spawners.Add(simpleSpawner);
            }
        }
        
        private void HandleCentipedeLineage(World.Lineage lineage)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.SmallCentipede, CreatureTemplate.Type.Centipede, largeCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            if (hasScutigera)
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, new CreatureTemplate.Type("Scutigera"), scutigeraChance);
            if (hasRedHorrorCentipede)
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedCentipede, new CreatureTemplate.Type("RedHorrorCenti"), redHorrorCentiChance);
        }

        private void HandleLongLegsLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners, string region)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);

            //DLL variants
            if(hasExplosiveDLL || hasMoreDLLs)
            {
                if (!hasMoreDLLs)
                {
                    ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, new CreatureTemplate.Type("ExplosiveDLL"), longlegsVariantChance);
                    ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Type("ExplosiveDLL"), longlegsVariantChance);
                }
                else
                {
                    if(UnityEngine.Random.value < 0.5f)
                    {
                        if (hasExplosiveDLL)
                        {
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, new CreatureTemplate.Type("ExplosiveDLL"), longlegsVariantChance);
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Type("ExplosiveDLL"), longlegsVariantChance);
                        }
                        else
                        {
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, new CreatureTemplate.Type("ExplosiveDaddyLongLegs"), longlegsVariantChance);
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Type("ExplosiveDaddyLongLegs"), longlegsVariantChance);
                        }
                    }
                    else
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, new CreatureTemplate.Type("ZapDaddyLongLegs"), longlegsVariantChance);
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, new CreatureTemplate.Type("ZapDaddyLongLegs"), longlegsVariantChance);
                    }
                }
            }

            //Inspector invasion
            if (hasAngryInspectors)
            {
                for(int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if(lineage.creatureTypes[i] >= 0 && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                    {
                        if(UnityEngine.Random.value < ((region == "UW")? inspectorChance*2 : inspectorChance))
                        {
                            World.SimpleSpawner inspectorSpawner = new World.SimpleSpawner
                                (lineage.region, spawners.Count, lineage.den, MoreSlugcatsEnums.CreatureTemplateType.Inspector, "{Ignorecycle}", 1);
                            spawners.Add(inspectorSpawner);
                            break;
                        }
                    }
                }
            }
        }

        private bool IsCreatureInLineage(World.Lineage lineage, CreatureTemplate.Type creatureType, bool useAncestor = false)
        {
            bool foundCreature = false;

            if (!useAncestor)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] == creatureType.Index)
                    {
                        foundCreature = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] >= 0 && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type == 
                        StaticWorld.creatureTemplates[creatureType.Index].TopAncestor().type)
                    {
                        foundCreature = true;
                        break;
                    }
                }
            }
           

            return foundCreature;
        }
    
        private bool ReplaceCreatureInLineage(World.Lineage lineage, CreatureTemplate.Type replacee, CreatureTemplate.Type replacement, float chance, bool useAncestors = false)
        {
            bool replacedCreature = false;
            if (!useAncestors)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] == replacee.Index && UnityEngine.Random.value < chance)
                    {
                        replacedCreature = true;
                        lineage.creatureTypes[i] = replacement.Index;
                    }
                }
            }
            else
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] >= 0 && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type ==
                        StaticWorld.creatureTemplates[replacee.Index].TopAncestor().type && UnityEngine.Random.value < chance)
                    {
                        replacedCreature = true;
                        lineage.creatureTypes[i] = replacement.Index;
                    }
                }
            }
           
            return replacedCreature;
        }

        private World.SimpleSpawner CopySpawner(World.SimpleSpawner origSpawner)
        {
            World.SimpleSpawner newSpawner = new World.SimpleSpawner(origSpawner.region, origSpawner.inRegionSpawnerIndex, origSpawner.den,
                origSpawner.creatureType, origSpawner.spawnDataString, origSpawner.amount);
            newSpawner.nightCreature = origSpawner.nightCreature;
            return newSpawner;
        }
        
        private void LogSpawner(World.CreatureSpawner spawner, int arrayIndex = -1, string subregion = "")
        {
            if(spawner is World.SimpleSpawner simpleSpawner)
            {
                Debug.Log("Simple Spawner data: " + simpleSpawner.ToString());
                Debug.Log("ID: " + simpleSpawner.SpawnerID);
                Debug.Log("Creature: " + simpleSpawner.creatureType);
                Debug.Log("Amount: " + simpleSpawner.amount);
                Debug.Log("Subregion: " + subregion);
                Debug.Log("In region index: " + simpleSpawner.inRegionSpawnerIndex);
                if(arrayIndex != -1)
                    Debug.Log("Spawner array index: " + arrayIndex);
                Debug.Log("Den: " + simpleSpawner.den.ToString());
                Debug.Log("Den room: " + simpleSpawner.den.ResolveRoomName());
                Debug.Log("Spawn data string: " + simpleSpawner.spawnDataString);
                Debug.Log("Night creature: " + simpleSpawner.nightCreature.ToString());    
            }
            else if(spawner is World.Lineage lineage)
            {
                string auxStr;
                Debug.Log("Lineage data: " + lineage.ToString());
                Debug.Log("ID: " + lineage.SpawnerID);
                for (int j = 0; j < lineage.creatureTypes.Length; ++j)
                {
                    if (lineage.creatureTypes[j] > -1)
                        auxStr = StaticWorld.creatureTemplates[lineage.creatureTypes[j]].type.ToString();
                    else auxStr = "Null";
                    Debug.Log("Creature " + (j + 1) + " : " + lineage.creatureTypes[j] + " (" +
                        auxStr + ")");
                }
                Debug.Log("Subregion: " + subregion);
                Debug.Log("In region index: " + lineage.inRegionSpawnerIndex);
                if (arrayIndex != -1)
                    Debug.Log("Spawner array index: " + arrayIndex);
                Debug.Log("Den: " + lineage.den.ToString());
                Debug.Log("Den room: " + lineage.den.ResolveRoomName());
                Debug.Log("Spawn data strings: ");
                for (int j = 0; j < lineage.spawnData.Length; ++j)
                {
                    Debug.Log("Creature " + j + " : " + lineage.spawnData[j]);
                }
            }
            Debug.Log("\n");

        }

    }    
}
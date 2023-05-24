using BepInEx;
using System.Security.Permissions;
using System.Security;
using System;
using UnityEngine;
using MoreSlugcats;
using System.Collections.Generic;
using System.Security.AccessControl;
using Steamworks;
using System.Runtime.Serialization;
using MonoMod.Cil;
using Mono.Cecil.Cil;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ApexUpYourSpawns
{
    [BepInPlugin("ShinyKelp.ApexUpYourSpawns", "ApexUpYourSpawns", "1.2.3")]

    public class ApexUpYourSpawnsMod : BaseUnityPlugin
    {
        private bool forceFreshSpawns, fillLineages, balancedSpawns;

        private float redLizardChance, trainLizardChance, largeCentipedeChance, redCentipedeChance, spitterSpiderChance, kingVultureChance,
            mirosVultureChance, eliteScavengerChance, brotherLongLegsChance, daddyLongLegsChance, terrorLongLegsChance, flyingPredatorChance,
            fireBugChance, giantJellyfishChance, leechLizardChance, yeekLizardChance, waterPredatorChance, caramelLizChance, strawberryLizChance,
            cyanLizChance, eelLizChance, jungleLeechChance, motherSpiderChance, stowawayChance, kingScavengerChance;

        private int extraYellows, extraLizards, extraCyans, extraWaterLiz, extraSpiders, extraVultures, extraScavengers, extraSmallCents, extraCentipedes,
            extraCentiwings, extraAquapedes, extraPrecycleSals, extraDropwigs, extraMiros, extraSmallSpiders, extraLeeches, extraKelp, extraLeviathans,
            extraEggbugs, extraCicadas, extraLMice, extraSnails, extraJetfish, extraYeeks, extraNightCreatures;

        //Mod dependent
        private float inspectorChance, sporantulaChance, scutigeraChance, redHorrorCentiChance, longlegsVariantChance, waterSpitterChance, fatFireFlyChance,
            sludgeLizardChance, snailSludgeLizardChance, mintLizardChance, ryanLizardChance, yellowLimeLizardChance;
        private int extraSporantulas, extraScutigeras, extraWaterSpitters, extraSludgeLizards, extraMintLizards;

        private bool IsInit;

        public bool hasSporantula, hasAngryInspectors, hasRedHorrorCentipede, hasScutigera, hasWaterSpitter, hasExplosiveDLL, hasMoreDLLs, hasFatFirefly,
            hasSludgeLizard, hasLizardVariants;

        private bool lastWasError;

        private HashSet<string> bannedRooms;

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
                On.Scavenger.ctor += ReplaceEliteForKing;

                On.ScavengerAI.Update += TrickKingIntoSquad;
                On.ScavengerAI.DecideBehavior += TrickKingIntoSquad1;
                On.ScavengerAbstractAI.AbstractBehavior += ScavKingAbsAICanGoIntoPipes;
                On.StaticWorld.InitStaticWorld += ScavKingCanTravelLikeElites;
                IL.Scavenger.Act += ScavKingActCanGoIntoPipes;

                if(bannedRooms is null)
                    bannedRooms = new HashSet<string>();
                bannedRooms.Clear();
                bannedRooms.Add("SB_GOR01");
                bannedRooms.Add("SB_H03");
                bannedRooms.Add("SB_H02");
                bannedRooms.Add("SB_E04");
                bannedRooms.Add("SB_C06");

                lastWasError = hasSporantula = hasAngryInspectors = hasRedHorrorCentipede = hasScutigera = hasWaterSpitter = hasMoreDLLs = hasExplosiveDLL = false;
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
                    if(mod.name == "Lizard Variants")
                    {
                        hasLizardVariants = true;
                        continue;
                    }
                    if(mod.name == "Sludge Lizard")
                    {
                        hasSludgeLizard = true;
                        continue;
                    }
                }

                //On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
                MachineConnector.SetRegisteredOI("ShinyKelp.ApexUpYourSpawns", this.options);
                IsInit = true;
                Debug.Log("Apex Up Your Spawns hooks finished successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }



        #region Experimental ScavKing Behaviour

        private void ScavKingActCanGoIntoPipes(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Creature>("shortcutDelay"),
                x => x.MatchLdcI4(1));
            c.Index += 6;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
        }

        private void ScavKingCanTravelLikeElites(On.StaticWorld.orig_InitStaticWorld orig)
        {
            orig();
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).doesNotUseDens = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).forbidStandardShortcutEntry = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).usesCreatureHoles = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).usesRegionTransportation = false;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).roamBetweenRoomsChance = -1f;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).usesNPCTransportation =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).usesNPCTransportation;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).shortcutAversion =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).shortcutAversion;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).mappedNodeTypes =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).mappedNodeTypes;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).pathingPreferencesConnections =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).pathingPreferencesConnections;
            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing).NPCTravelAversion =
                StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite).NPCTravelAversion;

        }

        private void ScavKingAbsAICanGoIntoPipes(On.ScavengerAbstractAI.orig_AbstractBehavior orig, ScavengerAbstractAI self, int time)
        {
            if(self is null || self.parent is null || self.parent.creatureTemplate is null)
            {
                orig(self, time);
                return;
            }

            if (self.parent.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing && 
                !(self.parent.realizedCreature is null) && !(self.parent.realizedCreature.room is null) &&
                self.parent.realizedCreature.room.abstractRoom.name != "LC_FINAL")
            {
                self.parent.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite;
                orig(self, time);
                self.parent.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing;
            }
            else
                orig(self, time);
        }

        private void TrickKingIntoSquad1(On.ScavengerAI.orig_DecideBehavior orig, ScavengerAI self)
        {
            if (self.scavenger.King && !(self.scavenger.room is null) && self.scavenger.room.abstractRoom.name != "LC_FINAL")
            {
                try
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite;
                    orig(self);
                }
                finally
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing;
                }
            }
            else
                orig(self);
        }

        private void TrickKingIntoSquad(On.ScavengerAI.orig_Update orig, ScavengerAI self)
        {
            if (self.scavenger.King && !(self.scavenger.room is null) && self.scavenger.room.abstractRoom.name != "LC_FINAL")
            {
                try
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite;
                    orig(self);
                }
                finally
                {
                    self.scavenger.abstractCreature.creatureTemplate.type = MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing;
                }

            }
            else
                orig(self);
        }

        #endregion

        private void SetOptions()
        {
            fillLineages = options.fillLineages.Value;
            forceFreshSpawns = options.forceFreshSpawns.Value;
            balancedSpawns = options.balancedSpawns.Value;

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
            kingScavengerChance = (float)options.kingScavengerChance.Value / 100;

            //Mod dependant
            inspectorChance = (float)options.inspectorChance.Value / 100;
            sporantulaChance = (float)options.sporantulaChance.Value / 100;
            scutigeraChance = (float)options.scutigeraChance.Value / 100;
            redHorrorCentiChance = (float)options.redHorrorCentiChance.Value / 100;
            longlegsVariantChance = (float)options.longlegsVariantChance.Value / 100;
            waterSpitterChance = (float)options.waterSpitterChance.Value / 100;
            fatFireFlyChance = (float)options.fatFireFlyChance.Value / 100;
            sludgeLizardChance = (float)options.sludgeLizardChance.Value / 100;
            snailSludgeLizardChance = (float)options.snailSludgeLizardChance.Value / 100;
            mintLizardChance = (float)options.mintLizardChance.Value / 100;
            ryanLizardChance = (float)options.ryanLizardChance.Value / 100;
            yellowLimeLizardChance = (float)options.yellowLimeLizardChance.Value / 100;

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
            extraSludgeLizards = options.sludgeLizardExtras.Value;
            extraMintLizards = options.mintLizardExtras.Value;
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

            if (!game.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < giantJellyfishChance)
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
            if(!game.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < stowawayChance)
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
            if (game.IsStorySession && !room.abstractRoom.shelter && UnityEngine.Random.value < (balancedSpawns? stowawayChance * 2 : stowawayChance))
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

        private void ReplaceEliteForKing(On.Scavenger.orig_ctor orig, Scavenger self, AbstractCreature abstractCreature, World world)
        {
            if (!game.IsArenaSession && abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite
                && abstractCreature.Room.name != "LC_FINAL" && UnityEngine.Random.value < kingScavengerChance)
            {
                abstractCreature.creatureTemplate = StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing);
            }
            orig(self, abstractCreature, world);
            if (self.King && abstractCreature.Room.name != "LC_FINAL")
            {
                self.kingWaiting = false;
                self.armorPieces = UnityEngine.Random.Range(1, 4);
            }

        }

        private int spawnerIndex; 
        private WorldLoader wLoader;
        private int firstRoomIndex
        {
            get => wLoader.world.firstRoomIndex;
        }
        private int numberOfRooms
        {
            get => wLoader.world.NumberOfRooms;
        }
        private string region 
        {
            get => wLoader.worldName;
        }
        private string subregion
        {
            get => (wLoader.spawners[spawnerIndex].den.room < firstRoomIndex ||
                    wLoader.spawners[spawnerIndex].den.room >= firstRoomIndex + numberOfRooms)? "" : 
                wLoader.abstractRooms[wLoader.spawners[spawnerIndex].den.room - firstRoomIndex].subregionName;
        }
        private string roomName
        {
            get => wLoader.spawners[spawnerIndex].den.ResolveRoomName();
        }
        private int spawnerCount
        {
            get => wLoader.spawners.Count;
        }
        private SlugcatStats.Name slugcatName
        {
            get => (game.IsStorySession) ? null :
                game.GetStorySession.saveState.saveStateNumber;
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
                    wLoader = worldLoader;
                    string storySession =  (slugcatName is null) ? "null" : slugcatName.ToString();
                    Debug.Log("Starting spawns for region: " + region + " , character: " + 
                        storySession);
                    Debug.Log("ORIGINAL SPAWN COUNT: " + spawnerCount);
                    Debug.Log("\n");
                    UnityEngine.Random.InitState(Mathf.RoundToInt(Time.time * 10f));
                    HandleAllSpawners(worldLoader, worldLoader.spawners);
                    Debug.Log("\nFinished setting up spawns.");
                    Debug.Log("FINAL SPAWN COUNT: " + spawnerCount + "\n");
                    wLoader = null;
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }

            orig(worldLoader, fresh);

        }
        
        private void HandleAllSpawners(WorldLoader worldLoader, List<World.CreatureSpawner> spawners)
        {
            int originalSpawnerCount = spawners.Count;
            for (int i = 0; i < spawners.Count; i++)
            {
                spawnerIndex = i;
                if (spawners[i].den.room < firstRoomIndex ||
                    spawners[i].den.room >= firstRoomIndex + numberOfRooms)
                {
                    lastWasError = true;
                    Debug.Log("!!! ERROR SPAWNER FOUND !!!");
                    LogSpawner(spawners[i], i);
                    continue;
                }
                //Log Spawners
                if (!lastWasError)
                {
                    if (i > 0)
                    {
                        Debug.Log("==AFTER TRANSFORMATIONS==");
                        LogSpawner(spawners[i - 1], i - 1);
                    }
                    LogSpawner(spawners[i], i);
                }
                else
                    lastWasError = false;
                //*/
                if (spawners[i] is World.SimpleSpawner simpleSpawner)
                {   
                    if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle"))
                        HandlePrecycleSpawns(simpleSpawner, spawners);

                    if ((simpleSpawner.nightCreature || (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("Night"))) 
                        && (simpleSpawner.spawnDataString is null || !simpleSpawner.spawnDataString.Contains("Ignorecycle")))
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraNightCreatures, true);
                        if (hasAngryInspectors) 
                        {
                            if((region == "LC" && roomName != "LCOffScreenDen") || 
                                (region == "UW" && roomName != "UWOffScreenDen"))
                            {
                                bool addedSpawner =
                                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns? inspectorChance / 2 : inspectorChance, true);
                                if (addedSpawner)
                                    (spawners[spawners.Count - 1] as World.SimpleSpawner).spawnDataString = "{Ignorecycle}";
                            }
                        } 
                    }

                    if (IsLizard(simpleSpawner))
                    {
                        HandleLizardSpawner(simpleSpawner, spawners);
                        continue;
                    }

                    if (IsCentipede(simpleSpawner))
                    {
                        if (hasAngryInspectors && subregion == "Memory Crypts"
                            && simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns? inspectorChance * 4 : inspectorChance);
                        HandleCentipedeSpawner(simpleSpawner, spawners);
                        if (hasSporantula && (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede || 
                            simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede))
                            AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Sporantula"), sporantulaChance);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture || simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture
                        || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture)
                    {
                        HandleVultureSpawner(simpleSpawner, spawners);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    {
                        if(region == "GW")
                            IncreaseCreatureSpawner(simpleSpawner, balancedSpawns? extraSpiders*2 : extraSpiders, true);
                        else
                        {
                            IncreaseCreatureSpawner(simpleSpawner, (region == "SB" && balancedSpawns)? extraSpiders-10 : extraSpiders, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                        }
                        //Sporantula
                        if (hasSporantula)
                            AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Sporantula"), sporantulaChance);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.SpitterSpider)
                    {
                        if (region == "GW" && balancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, extraSpiders, true);
                        if (region == "UW" || region == "CL" || region == "GW")
                            HandleLongLegsSpawner(simpleSpawner, spawners);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Spider)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraSmallSpiders);
                        AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.MotherSpider, motherSpiderChance, true);
                        continue;
                    }
                    if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MotherSpider)
                    {
                        if (balancedSpawns && subregion == "The Gutter")
                        {
                            World.SimpleSpawner aSpawner = CopySpawner(simpleSpawner);
                            aSpawner.amount = 0;
                            aSpawner.creatureType = CreatureTemplate.Type.BigSpider;
                            spawners.Add(aSpawner);
                        }
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.DropBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraDropwigs, true);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.BigEel)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraLeviathans, true);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.MirosBird)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, extraMiros);
                        if (region == "SB" && extraMiros > 0 && balancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 2);
                        if (region == "LC" && extraMiros > 0 && balancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 4);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Scavenger)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer || region == "SB")) ?
                            extraScavengers/2 : extraScavengers);
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite, eliteScavengerChance);
                        continue;
                    }
                    
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                    {
                        HandleJetfishSpawner(simpleSpawner, spawners);
                        continue;
                    }
                    
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.TentaclePlant)
                    {
                        if (hasAngryInspectors && region == "SB" && roomName == "SB_G03")
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns ? inspectorChance * 4 : inspectorChance);
                        IncreaseCreatureSpawner(simpleSpawner, extraKelp, true);
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
                        IncreaseCreatureSpawner(simpleSpawner, extraEggbugs, true);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.FireBug, fireBugChance);
                        if (hasSporantula && !replacedFull)
                            AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Sporantula"), sporantulaChance);
                        continue;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.TubeWorm)
                    {
                        if (hasAngryInspectors && region == "LC" && roomName == "LC_station01")
                        {
                            AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.Inspector, balancedSpawns ? inspectorChance * 4 : inspectorChance);
                        }
                        continue;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.CicadaA || simpleSpawner.creatureType == CreatureTemplate.Type.CicadaB)
                    {
                        HandleCicadaSpawner(simpleSpawner, spawners);
                        continue;
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.Snail)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (region == "DS" && balancedSpawns) ? extraSnails-10 : extraSnails, true);
                        if (hasSludgeLizard)
                        {
                            AddInvasionSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("SludgeLizard"), snailSludgeLizardChance, true);
                        }
                        HandleLongLegsSpawner(simpleSpawner, spawners);
                    }

                    if(simpleSpawner.creatureType == CreatureTemplate.Type.LanternMouse)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (region == "SH" && balancedSpawns) ? extraLMice - 10 : extraLMice, true);
                        HandleLongLegsSpawner(simpleSpawner, spawners);
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
                        IncreaseCreatureSpawner(simpleSpawner, (region == "OE" && balancedSpawns) ? extraYeeks - 10 : extraYeeks, true);
                        bool replacedFull = 
                        ReplaceMultiSpawner(simpleSpawner, spawners, UnityEngine.Random.value < (region == "OE" ? .8f : .5f)? 
                            MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard : MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, yeekLizardChance);
                        if (replacedFull)
                            HandleLizardSpawner(simpleSpawner, spawners);
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
                            IncreaseCreatureSpawner(simpleSpawner, extraSporantulas, true);
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
                        HandleLizardLineage(lineage, spawners);
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, true))
                    {
                        HandleLongLegsLineage(lineage, spawners);
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, true))
                    {
                        HandleCentipedeLineage(lineage, spawners);
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
                            HandleLongLegsLineage(lineage, spawners);
                        }
                        continue;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.EggBug))
                    {
                        if (balancedSpawns)
                        {
                            lineage.creatureTypes[0] = CreatureTemplate.Type.EggBug.Index;
                            for (int j = 1; j < lineage.creatureTypes.Length; ++j)
                            {
                                lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.FireBug.Index;
                            }
                        }
                        else
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.EggBug, MoreSlugcatsEnums.CreatureTemplateType.FireBug, fireBugChance);
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

        private void HandleLizardSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if(simpleSpawner.creatureType == CreatureTemplate.Type.Salamander
               || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard
               || (hasWaterSpitter && simpleSpawner.creatureType == new CreatureTemplate.Type("WaterSpitter"))
               || (hasSludgeLizard && simpleSpawner.creatureType == new CreatureTemplate.Type("SludgeLizard")))
            {
                HandleAxolotlSpawner(simpleSpawner, spawners);
                return;
            }

            bool replaceForRyan = hasLizardVariants && ryanLizardChance > 0 && 
                (simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard || simpleSpawner.creatureType == new CreatureTemplate.Type("RyanLizard"));
            int currentCount = spawnerCount;

            if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard || simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard
                || simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard
                || simpleSpawner.creatureType == CreatureTemplate.Type.WhiteLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard
                || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraLizards, true);
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
                IncreaseCreatureSpawner(simpleSpawner, extraYellows, true);
            }
            else if((region == "SU" || region == "HI") && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear ||
                slugcatName == SlugcatStats.Name.Red))
            {
                    localRedLizardChance /= 2;
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraCyans, true);
            }
            
            if(simpleSpawner.creatureType != CreatureTemplate.Type.RedLizard 
                && simpleSpawner.creatureType != MoreSlugcatsEnums.CreatureTemplateType.TrainLizard)
            {
                bool replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedLizard, balancedSpawns? localRedLizardChance : redLizardChance);
                if (replacedFull)
                    ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);
            }
            if(simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);
            }

            if (balancedSpawns && region == "GW" && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                    AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.WhiteLizard, redLizardChance * 4);
            }

            //Mods
            if (hasLizardVariants)
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard
                    || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("MintLizard"), mintLizardChance);
                if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("YellowLimeLizard"), yellowLimeLizardChance);
                if(simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), ryanLizardChance);
                if (replaceForRyan) 
                {
                    if(simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard)
                    {
                        ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), 1f);
                    }
                    if(spawnerCount > currentCount)
                    {
                        ReplaceMultiSpawner(spawners[spawners.Count-1] as World.SimpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), 1f);
                    }
                    if(simpleSpawner.creatureType == new CreatureTemplate.Type("RyanLizard"))
                    {
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);
                    }
                }
                if(simpleSpawner.creatureType == new CreatureTemplate.Type("MintLizard"))
                {
                    IncreaseCreatureSpawner(simpleSpawner, extraMintLizards, true);
                }
            }

        }

        private void HandleAxolotlSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Salamander)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraWaterLiz, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, eelLizChance);
            }
            if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, (extraWaterLiz > 0 && balancedSpawns) ? extraWaterLiz - 10 : extraWaterLiz, true);
            }

            if (hasSludgeLizard)
            {
                if (simpleSpawner.creatureType != new CreatureTemplate.Type("SludgeLizard"))
                {
                    float localSludgeLizardChance = sludgeLizardChance;
                    if (region == "CC")
                        localSludgeLizardChance = sludgeLizardChance * 2;
                    if (subregion == "The Gutter")
                        localSludgeLizardChance = sludgeLizardChance * 5;

                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("SludgeLizard"), balancedSpawns ? localSludgeLizardChance : sludgeLizardChance);
                }
                if (simpleSpawner.creatureType == new CreatureTemplate.Type("SludgeLizard"))
                    IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && subregion == "The Gutter")? extraSludgeLizards*2 : extraSludgeLizards, true);
            }

            if (hasWaterSpitter)
            {
                if (simpleSpawner.creatureType != new CreatureTemplate.Type("WaterSpitter"))
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("WaterSpitter"), waterSpitterChance);
                if (simpleSpawner.creatureType == new CreatureTemplate.Type("WaterSpitter"))
                    IncreaseCreatureSpawner(simpleSpawner, extraWaterSpitters, true);
            }

        }

        private void HandleCentipedeSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.AquaCenti)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraAquapedes, true);
                return;
            }
            bool wasSmallCentipedes = false;
            if(simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
            {
                wasSmallCentipedes = true;
                IncreaseCreatureSpawner(simpleSpawner, ((region == "OE" || region == "SB" || region == "VS") && balancedSpawns)? extraSmallCents-10 : extraSmallCents);
                if(!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("AlternateForm"))
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, largeCentipedeChance);
                else
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, largeCentipedeChance);

            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
            {
                if(!wasSmallCentipedes)
                    IncreaseCreatureSpawner(simpleSpawner, ((region == "SB" || region == "VS") && balancedSpawns)? extraCentipedes-10 : extraCentipedes, true);
                bool replacedFull =
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, ((region == "VS" || region == "SB") && balancedSpawns) ? redCentipedeChance / 2 : redCentipedeChance);
                //Scugitera chance
                if (hasScutigera && !replacedFull)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Scutigera"), scutigeraChance);
            }
            bool isCentiwing = simpleSpawner.creatureType == CreatureTemplate.Type.Centiwing;
            if(isCentiwing)
            {
                if(!wasSmallCentipedes && balancedSpawns)
                    IncreaseCreatureSpawner(simpleSpawner, extraCentiwings, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, (region == "LC" && balancedSpawns)? redCentipedeChance*2 : redCentipedeChance);
            }

            if(hasScutigera && simpleSpawner.creatureType == new CreatureTemplate.Type("Scutigera"))
            {
                IncreaseCreatureSpawner(simpleSpawner, extraScutigeras, true);
            }

            if(hasRedHorrorCentipede && simpleSpawner.creatureType == CreatureTemplate.Type.RedCentipede)
            {
                float localRedHorrorChance = redHorrorCentiChance;
                if (region == "SI" || region == "LC" || isCentiwing)
                    localRedHorrorChance *= 2f;
                ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("RedHorrorCenti"), balancedSpawns? localRedHorrorChance : redHorrorCentiChance);

            }

            if(balancedSpawns && region == "GW" && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, largeCentipedeChance * 2);
                if (hasScutigera && simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Scutigera"), scutigeraChance * 2);
                if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            }
        }

        private void HandleVultureSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {

            if(simpleSpawner.creatureType == CreatureTemplate.Type.Vulture)
            {
                if (extraVultures > 0) 
                {
                    IncreaseCreatureSpawner(simpleSpawner, extraVultures);
                    if (region == "OE" && balancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 2);
                    if (region == "SI" && balancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 3);
                }
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.KingVulture, (region == "SI" && balancedSpawns)? kingVultureChance*2 : kingVultureChance);
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture && region == "UW")
                IncreaseCreatureSpawner(simpleSpawner, balancedSpawns? extraVultures+1 : extraVultures);

            float localMirosVultureChance = mirosVultureChance;
            if (region == "OE" || region == "SI")
                localMirosVultureChance /= 2;
            else if (region == "SL" || region == "LM" || region == "LC" || region == "MS" ||
                (region == "GW" && (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear)))
                localMirosVultureChance *= 2;
            
            ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.MirosVulture, balancedSpawns? localMirosVultureChance : mirosVultureChance);

            if (region == "SH" && simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture && mirosVultureChance > 0)
                IncreaseCreatureSpawner(simpleSpawner, balancedSpawns? extraVultures+1 : extraVultures);

            if (hasFatFirefly && (simpleSpawner.creatureType == CreatureTemplate.Type.Vulture ||
                simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture))
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("FatFireFly"), fatFireFlyChance);
            }

        }
        
        private void HandleLongLegsSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (!(StaticWorld.creatureTemplates[simpleSpawner.creatureType.Index].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs))
            {
                if(region == "UW" || region == "CL")
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, balancedSpawns ? brotherLongLegsChance * 2 : brotherLongLegsChance);
                }
                else if (balancedSpawns && region == "GW" && simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider &&
                    (slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                    slugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
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
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, balancedSpawns? brotherLongLegsChance : localBrotherChance);
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

        private void HandleJetfishSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (region == "SL" && balancedSpawns) ? extraJetfish - 10 : extraJetfish, true);
            if (waterPredatorChance > 0 && (region == "LM" || region == "SB" || region == "VS") && balancedSpawns)
                IncreaseCreatureSpawner(simpleSpawner, 10, true);

            float localWaterPredatorChance = waterPredatorChance;
            if (region == "SB" || region == "VS")
                localWaterPredatorChance *= 2f;
            else if (region == "SL")
                localWaterPredatorChance *= 0.8f;

            bool replacedFull;
            replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, balancedSpawns? localWaterPredatorChance : waterPredatorChance);

            if ((region == "SL" || region == "CL") && !replacedFull && waterPredatorChance > 0)
                HandleLongLegsSpawner(simpleSpawner, spawners);
        }
        
        private void HandleCicadaSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (region == "SI" || region == "OE")? extraCicadas - 10 : extraCicadas, true);
            if (flyingPredatorChance > 0)
            {
                float localSelector = .4f;
                if (region == "SI" || region == "LC")
                    localSelector = .65f;
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
                        HandleCentipedeSpawner(simpleSpawner, spawners);
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
                IncreaseCreatureSpawner(simpleSpawner, (balancedSpawns && region == "DS")? (int)(extraLeeches*1.5f) : extraLeeches);
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.JungleLeech, jungleLeechChance);
                AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, (balancedSpawns && region == "DS")? leechLizardChance*2 : leechLizardChance, true);
            }
            if(simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.JungleLeech)
            {
                if(!wasRedLeech)
                    AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, leechLizardChance, true);
            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech)
            {
                IncreaseCreatureSpawner(simpleSpawner, extraLeeches);
                AddInvasionSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, leechLizardChance, true);
            }
        }

        private void HandlePrecycleSpawns(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            int extras = extraPrecycleSals;
            if (simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.EelLizard ||
                StaticWorld.creatureTemplates[simpleSpawner.creatureType.Index].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                extras-= 10;
            IncreaseCreatureSpawner(simpleSpawner, balancedSpawns ? extras : extraPrecycleSals, true);
        }

        private void IncreaseCreatureSpawner(World.SimpleSpawner simpleSpawner, int amount = 1, bool divide = false)
        {
            if (amount <= 0)
                return;
            if (balancedSpawns && bannedRooms.Contains(simpleSpawner.den.ResolveRoomName()))
                return;

            if (!divide)
            {
                simpleSpawner.amount += UnityEngine.Random.Range(0, amount + 1);
            }
            else
            {
                int loadedResult = UnityEngine.Random.Range(0, amount);
                simpleSpawner.amount += (int)Mathf.Floor(loadedResult / 10);
                if (UnityEngine.Random.value < (loadedResult % 10) / 10f)
                    simpleSpawner.amount++;
            }
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
            string fillCreatureData = "";
            int i = 0;
            while (fillCreature == -1 && i < lineage.creatureTypes.Length)
            {
                fillCreature = lineage.creatureTypes[i];
                fillCreatureData = lineage.spawnData[i];
                ++i;
            }
            if (fillCreature != -1)
            {
                for(int j = 0; j < lineage.creatureTypes.Length; ++j)
                {
                    if (lineage.creatureTypes[j] == -1) 
                    {
                        lineage.creatureTypes[j] = fillCreature;
                        lineage.spawnData[j] = fillCreatureData;
                    }
                    else
                    {
                        fillCreature = lineage.creatureTypes[j];
                        fillCreatureData = lineage.spawnData[j];
                    }
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

        private void HandleLizardLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            if(IsCreatureInLineage(lineage, CreatureTemplate.Type.Salamander) || IsCreatureInLineage(lineage, MoreSlugcatsEnums.CreatureTemplateType.EelLizard))
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, eelLizChance);
                if (hasWaterSpitter)
                {
                    ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, new CreatureTemplate.Type("WaterSpitter"), waterSpitterChance);
                    ReplaceCreatureInLineage(lineage, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, new CreatureTemplate.Type("WaterSpitter"), waterSpitterChance);
                }
                if (hasSludgeLizard)
                {
                    ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, new CreatureTemplate.Type("SludgeLizard"), sludgeLizardChance);
                    ReplaceCreatureInLineage(lineage, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, new CreatureTemplate.Type("SludgeLizard"), sludgeLizardChance);
                }
                return;
            }

            bool replaceForRyanLiz = hasLizardVariants && IsCreatureInLineage(lineage, CreatureTemplate.Type.CyanLizard);

            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, CreatureTemplate.Type.RedLizard, redLizardChance, true);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, caramelLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BlueLizard, CreatureTemplate.Type.CyanLizard, cyanLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.PinkLizard, MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard, strawberryLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);

            if (replaceForRyanLiz && IsCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard))
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, new CreatureTemplate.Type("RyanLizard"), 1f);


            if (forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.YellowLizard.Index && extraYellows > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.YellowLizard, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (forceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.Salamander.Index && extraWaterLiz > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.Salamander, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (forceFreshSpawns && balancedSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BlackLizard.Index && region == "SH" && UnityEngine.Random.value > .5f)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BlackLizard, lineage.spawnData[0], 1);
                spawners.Add(simpleSpawner);
            }
            else if (hasSludgeLizard && forceFreshSpawns && lineage.creatureTypes[0] == new CreatureTemplate.Type("SludgeLizard").index && extraSludgeLizards > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, new CreatureTemplate.Type("SludgeLizard"), lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }

            if (hasLizardVariants)
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.CyanLizard, new CreatureTemplate.Type("RyanLizard"), ryanLizardChance);
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, new CreatureTemplate.Type("MintLizard"), mintLizardChance);
                ReplaceCreatureInLineage(lineage, MoreSlugcatsEnums.CreatureTemplateType.SpitLizard, new CreatureTemplate.Type("MintLizard"), mintLizardChance);
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.YellowLizard, new CreatureTemplate.Type("YellowLimeLizard"), yellowLimeLizardChance);
            }
        }
        
        private void HandleCentipedeLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.SmallCentipede, CreatureTemplate.Type.Centipede, largeCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
            if (hasScutigera)
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, new CreatureTemplate.Type("Scutigera"), scutigeraChance);
            if (hasRedHorrorCentipede)
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedCentipede, new CreatureTemplate.Type("RedHorrorCenti"), redHorrorCentiChance);
        }

        private void HandleLongLegsLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
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
                        if(UnityEngine.Random.value < ((region == "UW" && balancedSpawns)? inspectorChance*2 : inspectorChance))
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
        
        private void LogSpawner(World.CreatureSpawner spawner, int arrayIndex = -1)
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
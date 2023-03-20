using BepInEx;
using System.Security.Permissions;
using System.Security;
using System;
using UnityEngine;
using MoreSlugcats;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using On;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ApexUpYourSpawns
{
    [BepInPlugin("ShinyKelp.ApexUpYourSpawns", "ApexUpYourSpawns", "1.0.1")]

    public class ApexUpYourSpawnsMod : BaseUnityPlugin
    {
        private float redLizardChance, trainLizardChance, redCentipedeChance, spitterSpiderChance, mirosVultureChance, 
            eliteScavengerChance, brotherLongLegsChance, daddyLongLegsChance, terrorLongLegsChance, noodleFlyChance, fireBugChance, giantJellyfishChance;

        private int extraYellows, extraLizards, extraCyans, extraWaterLiz, extraSpiders, extraVultures, extraKings, extraScavengers, extraEggbugs, extraCentipedes,
            extraCentiwings, extraAquapedes, extraPrecycleSals, extraDropwigs, extraMiros;

        private bool IsInit;

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
                //On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
                global::MachineConnector.SetRegisteredOI("ShinyKelp.ApexUpYourSpawns", this.options);
                
                
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
            redCentipedeChance = (float)options.redCentipedeChance.Value / 100;
            spitterSpiderChance = (float)options.spitterSpiderChance.Value / 100;
            mirosVultureChance = (float)options.mirosVultureChance.Value / 100;
            eliteScavengerChance = (float)options.eliteScavengerChance.Value / 100;
            brotherLongLegsChance = (float)options.brotherLongLegsChance.Value / 100;
            daddyLongLegsChance = (float)options.daddyLongLegsChance.Value / 100;
            terrorLongLegsChance = (float)options.terrorLongLegsChance.Value / 100;
            fireBugChance = (float)options.fireBugChance.Value / 100;
            noodleFlyChance = (float) options.noodleFlyChance.Value / 100;
            giantJellyfishChance = (float)options.giantJellyfishChance.Value / 100;

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

        private void GenerateCustomPopulation(On.WorldLoader.orig_GeneratePopulation orig, WorldLoader worldLoader, bool fresh)
        {
            try
            {
                if (fresh)
                {
                    setOptions();
                    int origCount = worldLoader.spawners.Count;
                    for (int i = 0; i < worldLoader.spawners.Count; i++)
                    {
                        bool foundType;

                        if (worldLoader.spawners[i] is World.SimpleSpawner simpleSpawner)
                        {

                            //Lizards
                            foundType = handleLizardSpawner(simpleSpawner, worldLoader.spawners);

                            //Centipedes
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Centipede, extraCentipedes);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede,
                                    (worldLoader.worldName == "VS" || worldLoader.worldName == "SB") ? redCentipedeChance / 2 : redCentipedeChance);
                                continue;
                            }
                            else foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Centiwing, extraCentiwings);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, redCentipedeChance / 2);
                                continue;
                            }
                            else foundType = increaseCreatureSpawner(simpleSpawner, MoreSlugcatsEnums.CreatureTemplateType.AquaCenti, extraAquapedes); 
                            if (foundType) 
                                continue;

                            //Spiders
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.BigSpider, extraSpiders);
                            if (foundType)
                            {
                                replaceMultiSpawner(simpleSpawner, worldLoader.spawners, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                                continue;
                            }

                            //Misc
                            foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.DropBug, extraDropwigs);
                            if (foundType)
                                continue;
                            else
                                foundType = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.MirosBird, extraMiros);
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
                                else if(worldLoader.worldName == "GW" || worldLoader.worldName == "SL" || worldLoader.worldName == "LM" || worldLoader.worldName == "LC")
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
                                continue;
                            }   

                            //Testing
                            //Full info on any spawner
                            /*
                            if (simpleSpawner.creatureType == CreatureTemplate.Type.Salamander)
                            {
                                Debug.Log("FOUND SALAMANDER SPAWNER: " + simpleSpawner.ToString());
                                Debug.Log("INDEX: " + simpleSpawner.inRegionSpawnerIndex);
                                Debug.Log("DEN: " + simpleSpawner.den.ToString());
                                Debug.Log("AMOUNT: " + simpleSpawner.amount);
                                Debug.Log("STRING DATA: " + simpleSpawner.spawnDataString);
                                if (simpleSpawner.nightCreature)
                                    Debug.Log("IS NIGHT CREATURE.");
                                foundType = true;
                            }//*/
                        }

                        else if (worldLoader.spawners[i] is World.Lineage lineage)
                        {
                            foundType = replaceLizardLineage(lineage);
                            if (foundType)
                                continue;
                            foundType = replaceLongLegsLineage(lineage);
                            if (foundType)
                                continue;
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede, redCentipedeChance);
                            if (foundType)
                                continue;
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, redCentipedeChance/2);
                            if (foundType)
                                continue;
                            foundType = replaceCreatureLineage(lineage, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, spitterSpiderChance);
                            if (foundType)
                                continue;
                            foundType = replaceFireBugLineage(lineage);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }

            orig(worldLoader, fresh);

        }

        private bool handleLizardSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool isLizard = false, isYellow, isCyan, isAxo = false;

            //Increase lizard amounts
            isYellow = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.YellowLizard, extraYellows);

            isCyan = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.CyanLizard, extraCyans);

            if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard || simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard
                || simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard 
                || simpleSpawner.creatureType == CreatureTemplate.Type.WhiteLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard 
                || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard)
            {
                isLizard = true;
                increaseCreatureSpawner(simpleSpawner, simpleSpawner.creatureType, extraLizards);
            }


            if (isYellow || isCyan || isLizard)
                replaceMultiSpawner(simpleSpawner, spawners, simpleSpawner.creatureType, CreatureTemplate.Type.RedLizard, isYellow ? redLizardChance / 2 : redLizardChance);

            else
            {
                if (simpleSpawner.spawnDataString == "{PreCycle}")
                    isAxo = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Salamander, extraPrecycleSals);
                else
                    isAxo = increaseCreatureSpawner(simpleSpawner, CreatureTemplate.Type.Salamander, extraWaterLiz) ||
                        increaseCreatureSpawner(simpleSpawner, MoreSlugcatsEnums.CreatureTemplateType.EelLizard, extraWaterLiz);
            }

            if (!isLizard && !isYellow && !isCyan && !isAxo)
                isLizard = replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedLizard, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, trainLizardChance);
            
            return isLizard || isYellow || isCyan || isAxo;
        }
        private bool replaceLongLegsSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, string region)
        {
            bool foundType;
            if (simpleSpawner.creatureType == CreatureTemplate.Type.JetFish || simpleSpawner.creatureType == CreatureTemplate.Type.LanternMouse
                || simpleSpawner.creatureType == CreatureTemplate.Type.Snail)
            {
                float brotherChance = brotherLongLegsChance;
                if (region == "VS")
                    brotherChance = brotherChance * 2;
                if (simpleSpawner.creatureType != CreatureTemplate.Type.JetFish)
                    brotherChance = brotherChance / 2;
                foundType = replaceMultiSpawner(simpleSpawner, spawners, simpleSpawner.creatureType, CreatureTemplate.Type.BrotherLongLegs, brotherChance);
                if(replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance))
                    replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                return foundType;
            }
            else if (foundType = replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance))
            {
                replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                return foundType;
            }
            else
            {
                foundType = replaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance);
                return foundType;
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


        private bool replaceLizardLineage (World.Lineage lineage)
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

        private bool replaceLongLegsLineage(World.Lineage lineage)
        {
            bool isLongLegs;
            isLongLegs = replaceCreatureLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, daddyLongLegsChance);
            isLongLegs = replaceCreatureLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs, terrorLongLegsChance) || isLongLegs;
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

        private World.SimpleSpawner CopySpawner(World.SimpleSpawner origSpawner)
        {
            World.SimpleSpawner newSpawner = new World.SimpleSpawner(origSpawner.region, origSpawner.inRegionSpawnerIndex, origSpawner.den,
                origSpawner.creatureType, origSpawner.spawnDataString, origSpawner.amount);

            return newSpawner;
        }
        


    }
}
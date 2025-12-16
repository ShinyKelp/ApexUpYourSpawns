namespace ApexUpYourSpawns
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static ApexUtils;
    using static ApexGameInfo;
    using static SpawnerHelperFunctions;
    using static ConfigShortcuts;
    using Watcher;
    using MoreSlugcats;
    using System.Globalization;
    using System.Linq;

    //Logic and flow for all spawner-based vanilla or mod-hardcoded creatures.
    public class SpawnerCreatureLogic
    {

        private ModCreatureLogic modHandler;
        public SpawnerCreatureLogic(ModCreatureLogic modHandler)
        {
            this.modHandler = modHandler;
        }

        public void HandleAllSpawners()
        {
            List<World.CreatureSpawner> spawners = WLoader.spawners;

            for (int i = 0; i < spawners.Count; i++)
            {
                CurrentSpawnerIndex = i;
                if (spawners[i].den.room < FirstRoomIndex ||
                    spawners[i].den.room >= FirstRoomIndex + NumberOfRooms)
                {
                    LastWasError = true;
                    if (DebugLogs)
                    {
                        UnityEngine.Debug.Log("!!! ERROR SPAWNER FOUND !!!");
                        LogSpawner(spawners[i], i);
                    }
                    continue;
                }
                else
                    LastWasError = false;
                //Log Spawners
                if (DebugLogs)
                {
                    if (!LastWasError)
                    {
                        if (i > 0)
                        {
                            UnityEngine.Debug.Log("==AFTER TRANSFORMATIONS==");
                            LogSpawner(spawners[i - 1], i - 1);
                        }
                        LogSpawner(spawners[i], i);
                    }
                }
                if (spawners[i] is World.SimpleSpawner simpleSpawner)
                {
                    if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.EndsWith("<AUYS_SKIP>"))
                    {
                        simpleSpawner.spawnDataString = simpleSpawner.spawnDataString.Remove(simpleSpawner.spawnDataString.Length - 11);
                        if (simpleSpawner.spawnDataString.Length <= 1)
                            simpleSpawner.spawnDataString = null;
                        continue;
                    }

                    if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("PreCycle"))
                        HandlePrecycleSpawns(simpleSpawner, spawners);

                    if ((simpleSpawner.nightCreature || (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("Night")))
                        && (simpleSpawner.spawnDataString is null || !simpleSpawner.spawnDataString.Contains("Ignorecycle")))
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraNightCreatures, true);
                        if (HasAngryInspectors && HasSharedDLC)
                        {
                            if ((CurrentRegion == "LC" && RoomName != "LCOffScreenDen") ||
                                (CurrentRegion == "UW" && RoomName != "UWOffScreenDen"))
                            {
                                bool addedSpawner =
                                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns ? InspectorChance / 2 : InspectorChance, true);
                                if (addedSpawner)
                                    (spawners[spawners.Count - 1] as World.SimpleSpawner).spawnDataString = "{Ignorecycle}";
                            }
                        }
                    }

                    if (IsLizard(simpleSpawner))
                    {
                        HandleLizardSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (IsCentipede(simpleSpawner))
                    {
                        if (HasAngryInspectors && Subregion == "Memory Crypts"
                            && simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                            AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns ? InspectorChance * 4 : InspectorChance);
                        HandleCentipedeSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Vulture || simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture
                        || simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.MirosVulture)
                    {
                        HandleVultureSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    {
                        if (CurrentRegion == "GW")
                            IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns ? ExtraSpiders * 2 : ExtraSpiders, true);
                        else
                        {
                            IncreaseCreatureSpawner(simpleSpawner, (CurrentRegion == "SB" && BalancedSpawns) ? ExtraSpiders - 10 : ExtraSpiders, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SpitterSpider, SpitterSpiderChance);
                        }
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.SpitterSpider)
                    {
                        if (CurrentRegion == "GW" && BalancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, ExtraSpiders, true);
                        if (CurrentRegion == "UW" || CurrentRegion == "CL" || CurrentRegion == "GW")
                            HandleLongLegsSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Spider)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraSmallSpiders);
                        AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.MotherSpider, MotherSpiderChance, true, true);
                        goto ModCreaturesSpawner;
                    }
                    if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.MotherSpider)
                    {
                        if (BalancedSpawns && Subregion == "The Gutter")
                        {
                            World.SimpleSpawner aSpawner = CopySpawner(simpleSpawner);
                            aSpawner.amount = 0;
                            aSpawner.creatureType = CreatureTemplate.Type.BigSpider;
                            spawners.Add(aSpawner);
                        }
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.DropBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraDropwigs, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.BigEel)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraLeviathans, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.MirosBird)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraMiros);
                        if (CurrentRegion == "SB" && ExtraMiros > 0 && BalancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 2);
                        if (CurrentRegion == "LC" && ExtraMiros > 0 && BalancedSpawns)
                            IncreaseCreatureSpawner(simpleSpawner, 4);

                        if (ModManager.Watcher)
                        {
                            if (simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                            {
                                float localMultiplierLoach = BalancedSpawns && CurrentRegion == "SH" ? 0.5f : 1f;
                                float localMultiplierDrill = 1f;
                                if (BalancedSpawns)
                                {
                                    if (CurrentRegion == "SH")
                                        localMultiplierDrill = 0.5f;
                                    else if (CurrentRegion == "LC")
                                        localMultiplierDrill = 1.5f;
                                }
                                if (!AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.DrillCrab, MirosDrillCrabChance * localMultiplierDrill, true, false, true, true))
                                {
                                    AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Loach, MirosLoachChance * localMultiplierLoach, true, false, true, true);
                                }
                            }
                        }

                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Scavenger)
                    {
                        int localExtraScavs = ExtraScavengers;
                        if (BalancedSpawns)
                        {
                            if (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer || CurrentRegion == "SB")
                                localExtraScavs /= 2;
                            else if (CurrentRegion == "LC")
                                localExtraScavs = (int)(localExtraScavs * 1.5f);
                        }
                        IncreaseCreatureSpawner(simpleSpawner, localExtraScavs, false);
                        ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.ScavengerElite, EliteScavengerChance);
                        if (ModManager.Watcher)
                        {
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.ScavengerTemplar, ScavengerTemplarChance);
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.ScavengerDisciple, ScavengerDiscipleChance);
                        }
                        goto ModCreaturesSpawner;
                    }


                    if (simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                    {
                        HandleJetfishSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.TentaclePlant)
                    {
                        if (HasAngryInspectors && CurrentRegion == "SB" && RoomName == "SB_G03")
                            AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns ? InspectorChance * 4 : InspectorChance, true);
                        IncreaseCreatureSpawner(simpleSpawner, ExtraKelp, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Leech || simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech
                        || simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.JungleLeech)
                    {
                        HandleLeechSpawner(simpleSpawner, spawners);
                        if (ModManager.Watcher)
                            AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Frog, LeechFrogChance, true, true, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.EggBug)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraEggbugs, true);
                        bool replacedFull =
                        ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.FireBug, FireBugChance);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.TubeWorm)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, ExtraTubeworms);
                        AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigSpider, TubeWormBigSpiderChance);
                        if (HasAngryInspectors && CurrentRegion == "LC" && RoomName == "LC_station01")
                        {
                            AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, BalancedSpawns ? InspectorChance * 4 : InspectorChance, true);
                        }
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.CicadaA || simpleSpawner.creatureType == CreatureTemplate.Type.CicadaB)
                    {
                        HandleCicadaSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Snail)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (CurrentRegion == "DS" && BalancedSpawns) ? ExtraSnails - 10 : ExtraSnails, true);
                        HandleLongLegsSpawner(simpleSpawner, spawners);
                        if (ModManager.Watcher && simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Barnacle, SnailBarnacleChance, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.LanternMouse)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (CurrentRegion == "SH" && BalancedSpawns) ? ExtraLMice - 10 : ExtraLMice, true);
                        HandleLongLegsSpawner(simpleSpawner, spawners);
                        if (ModManager.Watcher)
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Rat, MouseRatChance, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.BigNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= OriginalSpawnCount)
                            AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.SmallNeedleWorm, 1f);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallNeedleWorm)
                    {
                        if (simpleSpawner.inRegionSpawnerIndex >= OriginalSpawnCount && simpleSpawner.amount < 2)
                            IncreaseCreatureSpawner(simpleSpawner, 15, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.PoleMimic)
                    {
                        if (ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.TentaclePlant, MonsterKelpChance))
                            IncreaseCreatureSpawner(simpleSpawner, ExtraKelp, true);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.Yeek)
                    {
                        IncreaseCreatureSpawner(simpleSpawner, (CurrentRegion == "OE" && BalancedSpawns) ? ExtraYeeks - 10 : ExtraYeeks, true);
                        bool replacedFull =
                        ReplaceMultiSpawner(simpleSpawner, spawners, UnityEngine.Random.value < (CurrentRegion == "OE" ? .8f : .5f) ?
                            DLCSharedEnums.CreatureTemplateType.ZoopLizard : DLCSharedEnums.CreatureTemplateType.SpitLizard, YeekLizardChance);
                        if (replacedFull)
                            HandleLizardSpawner(simpleSpawner, spawners);
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.Inspector)
                    {
                        simpleSpawner.spawnDataString = "{Ignorecycle}";
                        goto ModCreaturesSpawner;
                    }

                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Deer)
                    {
                        int deerAmount = simpleSpawner.amount;
                        IncreaseCreatureSpawner(simpleSpawner, ExtraDeer);
                        if (ModManager.Watcher)
                        {
                            if (AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.DrillCrab, DeerDrillCrabInvChance, true, false, false, true))
                            {
                                World.SimpleSpawner drillSpawner = spawners[spawners.Count - 1] as World.SimpleSpawner;
                                if (BalancedSpawns)
                                    drillSpawner.amount -= 2;
                            }
                            else if (AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Loach, DeerLoachInvChance, true, false, false, true))
                            {
                                World.SimpleSpawner loachSpawner = spawners[spawners.Count - 1] as World.SimpleSpawner;
                                if (BalancedSpawns)
                                    loachSpawner.amount = Mathf.CeilToInt(loachSpawner.amount * 0.25f);
                            }
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.SkyWhale, DeerSkywhaleChance, true);
                            if (simpleSpawner.amount < 1)
                                simpleSpawner.amount = 2;
                        }
                        goto ModCreaturesSpawner;
                    }

                    if (ModManager.Watcher)
                    {
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.BigMoth)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, BigMothExtras);
                            if (simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                            {
                                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Vulture, BigMothVultureChance, true);
                            }
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.SmallMoth)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, SmallMothExtras, true);
                            if (simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.CicadaA, SmallMothCicadaChance, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigNeedleWorm, SmallMothNoodleflyChance, true);
                            ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, SmallMothCentiwingChance, true);
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.DrillCrab)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, DrillCrabExtras);
                            if (simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                            {
                                if (!AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.MirosBird, DrillCrabMirosChance, true, false, true, true))
                                    AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.Loach, DrillCrabLoachChance, true, false, true, true);
                            }
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Loach)
                        {
                            if (simpleSpawner.inRegionSpawnerIndex >= OriginalSpawnCount && BalancedSpawns && CurrentRegion != "LC")
                                simpleSpawner.amount = Mathf.CeilToInt(simpleSpawner.amount * 0.35f);
                            IncreaseCreatureSpawner(simpleSpawner, LoachExtras);
                            if (simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                            {
                                if (!AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.MirosBird, LoachMirosChance, true, false, true, true))
                                    AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.DrillCrab, LoachDrillCrabChance, true, false, true, true);
                            }
                            ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.RotLoach, RotLoachChance);
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Barnacle)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, BarnacleExtras, true);
                            if (simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Snail, BarnacleSnailChance, true);
                            HandleLongLegsSpawner(simpleSpawner, spawners);
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.SkyWhale)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, SkywhaleExtras);
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Frog)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, FrogExtras);
                            goto ModCreaturesSpawner;
                        }
                        if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.Rat)
                        {
                            IncreaseCreatureSpawner(simpleSpawner, RatExtras);
                            goto ModCreaturesSpawner;
                        }
                    }

                ModCreaturesSpawner:
                    modHandler.CheckModCreatures(simpleSpawner, spawners);
                    AfterModAdjustments(simpleSpawner, spawners);

                }
                else if (spawners[i] is World.Lineage lineage)
                {

                    if (FillLineages)
                        FillLineage(lineage);
                    if (ForceFreshSpawns && FillLineages)
                        RandomizeLineageFirst(lineage);

                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, true))
                    {
                        HandleLizardLineage(lineage, spawners);
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, true))
                    {
                        HandleLongLegsLineage(lineage, spawners);
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, true))
                    {
                        HandleCentipedeLineage(lineage, spawners);
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.BigSpider))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BigSpider, CreatureTemplate.Type.SpitterSpider, SpitterSpiderChance);
                        if (ForceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BigSpider.Index)
                        {
                            World.SimpleSpawner asimpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BigSpider, lineage.spawnData[0], 1);
                            spawners.Add(asimpleSpawner);
                        }
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.JetFish))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, DLCSharedEnums.CreatureTemplateType.AquaCenti, AquapedeChance);
                        if (CurrentRegion == "SL")
                        {
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.JetFish, CreatureTemplate.Type.BrotherLongLegs, AquapedeChance);
                            HandleLongLegsLineage(lineage, spawners);
                        }
                        goto ModCreaturesLineage;
                    }
                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.EggBug))
                    {
                        if (BalancedSpawns && ModManager.MSC)
                        {
                            lineage.creatureTypes[0] = CreatureTemplate.Type.EggBug.Index;
                            for (int j = 1; j < lineage.creatureTypes.Length; ++j)
                            {
                                lineage.creatureTypes[j] = MoreSlugcatsEnums.CreatureTemplateType.FireBug.Index;
                            }
                        }
                        else
                            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.EggBug, MoreSlugcatsEnums.CreatureTemplateType.FireBug, FireBugChance);
                        goto ModCreaturesLineage;
                    }

                    if (IsCreatureInLineage(lineage, CreatureTemplate.Type.PoleMimic))
                    {
                        ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.PoleMimic, CreatureTemplate.Type.TentaclePlant, MonsterKelpChance);
                        goto ModCreaturesLineage;
                    }

                ModCreaturesLineage:
                    modHandler.CheckModCreatures(lineage, spawners);
                    AfterModAdjustments(lineage, spawners);
                }
            }
            EnsureNormalScavengers(WLoader);
            WLoader = null;
        }

        private void AfterModAdjustments(World.SimpleSpawner spawner, List<World.CreatureSpawner> spawners)
        {
            if (ActiveMods.Contains("Outspector") && spawner.creatureType == new CreatureTemplate.Type("Outspector"))
            {
                spawner.spawnDataString = "{IgnoreCycle}";
                if (HasAngryInspectors)
                    AddInvasionSpawner(spawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, OptionConfigs.Instance.GetOptionConfigValue("InspectorOutspectorInvChance") / 100f);
                ReplaceMultiSpawner(spawner, spawners, new CreatureTemplate.Type("OutspectorB"), 0.3f);
            }

            if (ActiveMods.Contains("lb-fgf-m4r-ik.mini-levi") && spawner.creatureType == new CreatureTemplate.Type("MiniLeviathan"))
            {
                if (CurrentRegion == "SL")
                    spawner.spawnDataString = "{AlternateForm}";
            }

            if (ActiveMods.Contains("lb-fgf-m4r-ik.cool-thorn-bug") && spawner.creatureType == new CreatureTemplate.Type("ThornBug"))
            {
                if (CurrentRegion == "UW" || CurrentRegion == "CC" || CurrentRegion == "SH")
                    spawner.spawnDataString = "{AlternateForm}";
            }

            if (ActiveMods.Contains("drainmites") && spawner.creatureType == new CreatureTemplate.Type("DrainMite"))
            {
                if (spawner.spawnDataString is null || spawner.spawnDataString == "" || !DrainMiteStringValidFormat(spawner.spawnDataString))
                {
                    IncreaseCreatureSpawner(spawner, OptionConfigs.Instance.GetOptionConfigValue("DrainMiteExtras"), false);
                    spawner.amount /= 4;
                    World.SimpleSpawner spawner2 = CopySpawner(spawner);
                    World.SimpleSpawner spawner3 = CopySpawner(spawner);
                    World.SimpleSpawner spawner4 = CopySpawner(spawner);
                    spawner2.inRegionSpawnerIndex += 1;
                    spawner3.inRegionSpawnerIndex += 2;
                    spawner4.inRegionSpawnerIndex += 3;
                    spawners.Add(spawner2);
                    spawners.Add(spawner3);
                    spawners.Add(spawner4);

                    float minSize = 0.5f, maxSize = 1.5f;

                    if (CurrentRegion == "CC" || CurrentRegion == "SI" || CurrentRegion == "LF")
                    {
                        minSize = 0.5f;
                        maxSize = 1.0f;
                    }
                    else if (CurrentRegion == "SB" || CurrentRegion == "VS")
                    {
                        minSize = 1.0f;
                        maxSize = 1.5f;
                    }

                    float sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner.spawnDataString = "{BodySize:" + sizeMult + "}";
                    sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner2.spawnDataString = "{BodySize:" + sizeMult + "}<AUYS_SKIP>";
                    sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner3.spawnDataString = "{BodySize:" + sizeMult + "}<AUYS_SKIP>";
                    sizeMult = UnityEngine.Random.Range(minSize, maxSize);
                    spawner4.spawnDataString = "{BodySize:" + sizeMult + "}<AUYS_SKIP>";
                }
            }

            if (ActiveMods.Contains("ShinyKelp.AlbinoKings") && (spawner.creatureType == CreatureTemplate.Type.Vulture ||
                spawner.creatureType == CreatureTemplate.Type.KingVulture || spawner.creatureType ==
                DLCSharedEnums.CreatureTemplateType.MirosVulture))
            {
                if (spawner.spawnDataString is null || !spawner.spawnDataString.Contains("AlternateForm"))
                {
                    int currentCount = spawners.Count;
                    CreatureTemplate.Type prevType = spawner.creatureType;
                    bool full =
                        ReplaceMultiSpawner(spawner, spawners, CreatureTemplate.Type.StandardGroundCreature, OptionConfigs.Instance.GetOptionConfigValue("AlbinoVultureChance") * 0.01f);
                    if (full)
                    {
                        spawner.spawnDataString = "{AlternateForm}";
                        spawner.creatureType = prevType;
                    }
                    else if (currentCount < spawners.Count)
                    {
                        World.SimpleSpawner newSpawner = spawners[spawners.Count - 1] as World.SimpleSpawner;
                        newSpawner.spawnDataString = "{AlternateForm}<AUYS_SKIP>";
                        newSpawner.creatureType = prevType;
                    }
                }
            }
            if (ActiveMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                if (!TriedEchoLevi && spawner.creatureType.index > -1 && spawner.creatureType.index < StaticWorld.creatureTemplates.Length && (
                    StaticWorld.creatureTemplates[spawner.creatureType.index].TopAncestor().type == CreatureTemplate.Type.Vulture ||
                    StaticWorld.creatureTemplates[spawner.creatureType.index].TopAncestor().type == DLCSharedEnums.CreatureTemplateType.MirosVulture))
                {
                    TriedEchoLevi = true;
                    if (UnityEngine.Random.value < OptionConfigs.Instance.GetOptionConfigValue("VultureEchoLeviChance") * 0.01f)
                    {
                        World.SimpleSpawner echoSpawner = CopySpawner(spawner);
                        echoSpawner.amount = 1;
                        IncreaseCreatureSpawner(spawner, OptionConfigs.Instance.GetOptionConfigValue("EchoLeviExtras"), true);
                        echoSpawner.inRegionSpawnerIndex = spawners.Count;
                        echoSpawner.spawnDataString = "<AUYS_SKIP>";
                        echoSpawner.nightCreature = false;
                        echoSpawner.creatureType = new CreatureTemplate.Type("FlyingBigEel");
                        spawners.Add(echoSpawner);
                    }
                }

            }
        }

        private void AfterModAdjustments(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            if (ActiveMods.Contains("drainmites") && IsCreatureInLineage(lineage, new CreatureTemplate.Type("DrainMite")))
            {
                int miteIndex = new CreatureTemplate.Type("DrainMite").index;
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] == miteIndex)
                    {
                        lineage.creatureTypes[i] = -1;
                        lineage.spawnData[i] = "";
                    }
                }
                if (FillLineages)
                    FillLineage(lineage);
            }
            if (ActiveMods.Contains("ShinyKelp.AlbinoKings") && (IsCreatureInLineage(lineage, CreatureTemplate.Type.Vulture)
                || IsCreatureInLineage(lineage, CreatureTemplate.Type.Vulture)
                || IsCreatureInLineage(lineage, DLCSharedEnums.CreatureTemplateType.MirosVulture)))
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    int index = lineage.creatureTypes[i];
                    if (index > -1 && index < StaticWorld.creatureTemplates.Length)
                    {
                        if (StaticWorld.creatureTemplates[index].type ==
                            CreatureTemplate.Type.Vulture ||
                            IsCreatureInLineage(lineage, CreatureTemplate.Type.KingVulture) ||
                            StaticWorld.creatureTemplates[index].type ==
                            DLCSharedEnums.CreatureTemplateType.MirosVulture)
                        {
                            if (lineage.spawnData[i] is null || !lineage.spawnData[i].Contains("AlternateForm"))
                            {
                                if (UnityEngine.Random.value < OptionConfigs.Instance.GetOptionConfigValue("AlbinoVultureChance") / 100f)
                                    lineage.spawnData[i] = "{AlternateForm}";
                            }
                        }
                    }
                }
            }
        }

        private bool IsLizard(World.SimpleSpawner spawner)
        {
            return StaticWorld.GetCreatureTemplate(spawner.creatureType)?.TopAncestor().type ==
                CreatureTemplate.Type.LizardTemplate;
        }

        private bool IsVanillaLizard(World.SimpleSpawner spawner)
        {
            return spawner.creatureType == CreatureTemplate.Type.GreenLizard || spawner.creatureType == CreatureTemplate.Type.PinkLizard ||
                spawner.creatureType == CreatureTemplate.Type.BlueLizard || spawner.creatureType == CreatureTemplate.Type.YellowLizard ||
                spawner.creatureType == CreatureTemplate.Type.WhiteLizard || spawner.creatureType == CreatureTemplate.Type.BlackLizard ||
                spawner.creatureType == CreatureTemplate.Type.RedLizard || spawner.creatureType == CreatureTemplate.Type.CyanLizard ||
                spawner.creatureType == CreatureTemplate.Type.Salamander || spawner.creatureType == DLCSharedEnums.CreatureTemplateType.SpitLizard ||
                spawner.creatureType == DLCSharedEnums.CreatureTemplateType.ZoopLizard || spawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard ||
                spawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard;
        }

        private bool IsCentipede(World.SimpleSpawner spawner)
        {
            return StaticWorld.GetCreatureTemplate(spawner.creatureType)?.TopAncestor().type ==
                CreatureTemplate.Type.Centipede;
        }

        private void HandleLizardSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Salamander
               || simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard)
            {
                HandleAxolotlSpawner(simpleSpawner, spawners);
                return;
            }


            //Info check before changes
            float localRedLizardChance = RedLizardChance;
            if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard || (
                (CurrentRegion == "SU" || CurrentRegion == "HI") && (
                SlugcatName == SlugcatStats.Name.Red || (
                HasSharedDLC && (
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear
                )))))
            {
                if (localRedLizardChance < 1)
                    localRedLizardChance /= 2;
            }

            bool replaceForRyan = HasLizardVariants && RyanLizardChance > 0 && simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard;
            int currentCount = SpawnerCount;

            //Checks for normal lizards
            if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && CurrentRegion == "SU") ? ExtraGreens / 2 : ExtraGreens, true);
                bool replacedFull =
                    ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.SpitLizard, CaramelLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && CurrentRegion == "OE") ? ExtraCaramels - 10 : ExtraCaramels, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.PinkLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraPinks, true);
                bool replacedFull =
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.ZoopLizard, StrawberryLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, ExtraZoops, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraBlues, true);
                bool replacedFull =
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.CyanLizard, CyanLizChance);
                if (replacedFull)
                    IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && CurrentRegion == "UW") ? ExtraCyans / 2 : ExtraCyans, true);
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && Subregion == "Filtration System") ? ExtraBlacks - 10 : ExtraBlacks, true);
                HasBlackLizards = true;
            }
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.WhiteLizard)
                IncreaseCreatureSpawner(simpleSpawner, ExtraWhites, true);
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard)
                IncreaseCreatureSpawner(simpleSpawner,
                    (BalancedSpawns && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Rivulet || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                    ? ExtraYellows / 2 : ExtraYellows, true);
            else if (simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && CurrentRegion == "UW") ? ExtraCyans / 2 : ExtraCyans, true);
            else if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.SpitLizard)
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && CurrentRegion == "OE") ? ExtraCaramels - 10 : ExtraCaramels, true);
            else if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.ZoopLizard)
                IncreaseCreatureSpawner(simpleSpawner, ExtraZoops, true);

            if (BalancedSpawns && CurrentRegion == "GW" && ExtraWhites > 0 && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.BlueLizard || simpleSpawner.creatureType == CreatureTemplate.Type.CyanLizard)
                    AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.WhiteLizard, 0.4f);
            }

            //Red&Train lizard
            if (IsVanillaLizard(simpleSpawner) && simpleSpawner.creatureType != CreatureTemplate.Type.RedLizard)
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedLizard, BalancedSpawns ? localRedLizardChance : RedLizardChance);

            if (simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard)
                ReplaceMultiSpawner(simpleSpawner, spawners, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, TrainLizardChance);

            //Watcher
            if (ModManager.Watcher)
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.GreenLizard || simpleSpawner.creatureType ==
                    DLCSharedEnums.CreatureTemplateType.SpitLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.IndigoLizard, GroundIndigoLizChance);

                if (simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard)
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.BasiliskLizard, BlackBasiliskLizChance);

                if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.IndigoLizard)
                    IncreaseCreatureSpawner(simpleSpawner, IndigoLizExtras, true);

                if (simpleSpawner.creatureType == WatcherEnums.CreatureTemplateType.BasiliskLizard)
                    IncreaseCreatureSpawner(simpleSpawner, BasiliskLizExtras, true);

                float blizLizChance = BlizzardLizardChance;
                if (simpleSpawner.creatureType == CreatureTemplate.Type.YellowLizard)
                    blizLizChance /= 2;
                if (Subregion == "Bitter Aerie" || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                    blizLizChance *= 1.8f;

                AddInvasionSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.BlizzardLizard, BalancedSpawns ? blizLizChance : BlizzardLizardChance, true, false, true);
            }

            //Mods
            if (replaceForRyan)
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.RedLizard || simpleSpawner.creatureType == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard)
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), 1f);
                }
                if (SpawnerCount > currentCount)
                {
                    ReplaceMultiSpawner(spawners[spawners.Count - 1] as World.SimpleSpawner, spawners, new CreatureTemplate.Type("RyanLizard"), 1f);
                }
            }
            if (ActiveMods.Contains("thefriend"))
            {
                if (simpleSpawner.creatureType == StaticWorld.GetCreatureTemplate("MotherLizard").type)
                {
                    AddInvasionSpawner(simpleSpawner, spawners, StaticWorld.GetCreatureTemplate("YoungLizard").type, 1f);
                    (spawners[spawners.Count - 1] as World.SimpleSpawner).amount = (UnityEngine.Random.value > .5f ? 3 : 4);
                }
            }
        }

        private void HandleAxolotlSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Salamander)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraSals, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.EelLizard, EelLizChance);
            }
            if (HasSharedDLC && simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraEellizs, true);
            }

        }

        private void HandleCentipedeSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.AquaCenti)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraAquapedes, true);
                return;
            }
            bool wasSmallCentipedes = false;
            if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
            {
                wasSmallCentipedes = true;
                IncreaseCreatureSpawner(simpleSpawner, ((CurrentRegion == "OE" || CurrentRegion == "SB" || CurrentRegion == "VS") && BalancedSpawns) ? ExtraSmallCents - 10 : ExtraSmallCents);
                if (!(simpleSpawner.spawnDataString is null) && simpleSpawner.spawnDataString.Contains("AlternateForm"))
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, LargeCentipedeChance);
                else
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, LargeCentipedeChance);

            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
            {
                if (!wasSmallCentipedes)
                    IncreaseCreatureSpawner(simpleSpawner, ((CurrentRegion == "SB" || CurrentRegion == "VS") && BalancedSpawns) ? ExtraCentipedes - 10 : ExtraCentipedes, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, ((CurrentRegion == "VS" || CurrentRegion == "SB") && BalancedSpawns && RedCentipedeChance < 1) ? RedCentipedeChance / 2 : RedCentipedeChance);
            }
            bool isCentiwing = simpleSpawner.creatureType == CreatureTemplate.Type.Centiwing;
            if (isCentiwing)
            {
                if (!wasSmallCentipedes && BalancedSpawns)
                    IncreaseCreatureSpawner(simpleSpawner, ExtraCentiwings, true);
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, (CurrentRegion == "LC" && BalancedSpawns) ? RedCentipedeChance * 2 : RedCentipedeChance);
            }

            if (BalancedSpawns && CurrentRegion == "GW" && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear))
            {
                if (simpleSpawner.creatureType == CreatureTemplate.Type.SmallCentipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centipede, LargeCentipedeChance * 2);
                if (simpleSpawner.creatureType == CreatureTemplate.Type.Centipede)
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.RedCentipede, RedCentipedeChance);
            }
        }

        private void HandleVultureSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {

            if (simpleSpawner.creatureType == CreatureTemplate.Type.Vulture)
            {
                if (ExtraVultures > 0)
                {
                    IncreaseCreatureSpawner(simpleSpawner, ExtraVultures);
                    if (CurrentRegion == "OE" && BalancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 2);
                    if (CurrentRegion == "SI" && BalancedSpawns)
                        IncreaseCreatureSpawner(simpleSpawner, 3);
                }
                float localKingVultureChance = KingVultureChance;
                if (BalancedSpawns && (
                    CurrentRegion == "SI" || CurrentRegion == "LC" ||
                    SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Rivulet ||
                    SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                    localKingVultureChance *= 2;
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.KingVulture, localKingVultureChance);
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture && CurrentRegion == "UW")
                IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns ? ExtraVultures + 1 : ExtraVultures);

            float localMirosVultureChance = MirosVultureChance;
            if ((CurrentRegion == "OE" || CurrentRegion == "SI" || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint) && localMirosVultureChance < 1)
                localMirosVultureChance /= 2;
            else if (CurrentRegion == "LM" || CurrentRegion == "LC" || CurrentRegion == "MS" || CurrentRegion == "SD" ||
                (CurrentRegion == "GW" && (SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Artificer ||
                SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Spear)))
                localMirosVultureChance *= 2;

            ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.MirosVulture, BalancedSpawns ? localMirosVultureChance : MirosVultureChance);

            if (CurrentRegion == "SH" && simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.MirosVulture && MirosVultureChance > 0)
                IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns ? ExtraVultures + 1 : ExtraVultures);

            //Watcher
            if (ModManager.Watcher)
            {
                if (simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount &&
                    (simpleSpawner.creatureType == CreatureTemplate.Type.Vulture || simpleSpawner.creatureType == CreatureTemplate.Type.KingVulture))
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.BigMoth, VultureBigMothChance, true);
                }
            }

        }

        private void HandleLongLegsSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            if (!(StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs))
            {

                if (CurrentRegion == "UW" || CurrentRegion == "CL")
                    ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, BalancedSpawns ? BrotherLongLegsChance * 2 : BrotherLongLegsChance);
                else if (BalancedSpawns && CurrentRegion == "GW" && simpleSpawner.creatureType == CreatureTemplate.Type.BigSpider)
                    ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.TerrorLongLegs,
                        (SlugcatName.ToString() == "Artificer" || SlugcatName.ToString() == "Spear") ? BrotherLongLegsChance * 2 : BrotherLongLegsChance);
                else
                {
                    float localBrotherChance = BrotherLongLegsChance;
                    if (BalancedSpawns)
                    {
                        if (Subregion == "Sump Tunnel" || Subregion == "The Gutter")
                            localBrotherChance *= 2;
                        if (simpleSpawner.creatureType == CreatureTemplate.Type.JetFish)
                            localBrotherChance *= 1.5f;
                        if (simpleSpawner.spawnDataString != null && simpleSpawner.spawnDataString.Contains("PreCycle"))
                            localBrotherChance *= 2;
                    }
                    if (ActiveMods.Contains("Croken.Mimicstarfish") && OptionConfigs.Instance.GetOptionConfigValue("BllMimicstarfishChance") > 0 &&
                        (CurrentRegion == "SL" || CurrentRegion == "LM" || CurrentRegion == "MS" || (CurrentRegion == "DS" && UnityEngine.Random.value > 0.5f)))
                        ReplaceMultiSpawner(simpleSpawner, spawners, new CreatureTemplate.Type("Mimicstar"), localBrotherChance);
                    else
                        ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BrotherLongLegs, localBrotherChance);
                }
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.BrotherLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.DaddyLongLegs, DaddyLongLegsChance);
            }

            if (simpleSpawner.creatureType == CreatureTemplate.Type.DaddyLongLegs)
            {
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.TerrorLongLegs, TerrorLongLegsChance);
            }

            if (HasAngryInspectors &&
                StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
            {
                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.Inspector, InspectorChance);
            }


        }

        private void HandleJetfishSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (CurrentRegion == "SL" && BalancedSpawns) ? ExtraJetfish - 10 : ExtraJetfish, true);
            if (AquapedeChance > 0 && (CurrentRegion == "LM" || CurrentRegion == "SB" || CurrentRegion == "VS") && BalancedSpawns)
                IncreaseCreatureSpawner(simpleSpawner, 12, true);

            float localWaterPredatorChance = AquapedeChance;
            if (CurrentRegion == "SB" || CurrentRegion == "VS")
                localWaterPredatorChance *= 2f;
            else if (CurrentRegion == "SL")
                localWaterPredatorChance *= 0.6f;

            bool replacedFull;
            replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, JetfishSalamanderChance, true);
            if (!replacedFull)
                replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.AquaCenti, BalancedSpawns ? localWaterPredatorChance : AquapedeChance);

            if ((CurrentRegion == "SL" || CurrentRegion == "CL") && !replacedFull && AquapedeChance > 0)
                HandleLongLegsSpawner(simpleSpawner, spawners);
        }

        private void HandleCicadaSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            IncreaseCreatureSpawner(simpleSpawner, (CurrentRegion == "SI" || CurrentRegion == "OE") ? ExtraCicadas - 10 : ExtraCicadas, true);

            bool replacedFull = ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Centiwing, (CurrentRegion == "SI" || CurrentRegion == "LC") ? CicadaCentiwingChance / 2 : CicadaCentiwingChance, true);
            if (!replacedFull)
            {
                int spawnCount = spawners.Count;
                ReplaceMultiSpawner(simpleSpawner, spawners, CreatureTemplate.Type.BigNeedleWorm, CicadaNoodleflyChance, true);
                if (ModManager.Watcher && simpleSpawner.inRegionSpawnerIndex < OriginalSpawnCount)
                {
                    ReplaceMultiSpawner(simpleSpawner, spawners, WatcherEnums.CreatureTemplateType.SmallMoth, CicadaSmallMothChance, true);
                }
            }

        }

        private void HandleLeechSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            bool wasRedLeech = false;
            if (simpleSpawner.creatureType == CreatureTemplate.Type.Leech)
            {
                wasRedLeech = true;
                IncreaseCreatureSpawner(simpleSpawner, (BalancedSpawns && CurrentRegion == "DS") ? (int)(ExtraLeeches * 1.5f) : ExtraLeeches);
                ReplaceMultiSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.JungleLeech, JungleLeechChance);
                AddInvasionSpawner(simpleSpawner, spawners, CreatureTemplate.Type.Salamander, (BalancedSpawns && CurrentRegion == "DS") ? LeechLizardChance * 2 : LeechLizardChance, true, true);
            }
            if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.JungleLeech)
            {
                if (!wasRedLeech)
                    AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.EelLizard, LeechLizardChance, true, true);
            }
            if (simpleSpawner.creatureType == CreatureTemplate.Type.SeaLeech)
            {
                IncreaseCreatureSpawner(simpleSpawner, ExtraLeeches);
                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.AquaCenti, SeaLeechAquapedeChance, true, true);
                AddInvasionSpawner(simpleSpawner, spawners, DLCSharedEnums.CreatureTemplateType.EelLizard, LeechLizardChance, true, true);
            }
        }

        private void HandlePrecycleSpawns(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners)
        {
            int extras = ExtraPrecycleSals;
            if (simpleSpawner.creatureType == DLCSharedEnums.CreatureTemplateType.EelLizard ||
                StaticWorld.GetCreatureTemplate(simpleSpawner.creatureType)?.TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                extras -= 10;
            IncreaseCreatureSpawner(simpleSpawner, BalancedSpawns ? extras : ExtraPrecycleSals, true);
        }


        private void HandleLizardLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            if (IsCreatureInLineage(lineage, CreatureTemplate.Type.Salamander) || IsCreatureInLineage(lineage, DLCSharedEnums.CreatureTemplateType.EelLizard))
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Salamander, DLCSharedEnums.CreatureTemplateType.EelLizard, EelLizChance);
                return;
            }

            bool replaceForRyanLiz = HasLizardVariants && IsCreatureInLineage(lineage, CreatureTemplate.Type.CyanLizard);

            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, CreatureTemplate.Type.RedLizard, RedLizardChance, true);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, DLCSharedEnums.CreatureTemplateType.SpitLizard, CaramelLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BlueLizard, CreatureTemplate.Type.CyanLizard, CyanLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.PinkLizard, DLCSharedEnums.CreatureTemplateType.ZoopLizard, StrawberryLizChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, MoreSlugcatsEnums.CreatureTemplateType.TrainLizard, TrainLizardChance);


            if (ForceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.YellowLizard.Index && ExtraYellows > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.YellowLizard, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (ForceFreshSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.Salamander.Index && ExtraSals > 0)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.Salamander, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }
            else if (ForceFreshSpawns && BalancedSpawns && lineage.creatureTypes[0] == CreatureTemplate.Type.BlackLizard.Index && ExtraBlacks > 0 && CurrentRegion == "SH" && UnityEngine.Random.value > .5f)
            {
                World.SimpleSpawner simpleSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den, CreatureTemplate.Type.BlackLizard, lineage.spawnData[0], 0);
                spawners.Add(simpleSpawner);
            }

            //Watcher

            if (ModManager.Watcher)
            {
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, WatcherEnums.CreatureTemplateType.IndigoLizard, GroundIndigoLizChance);
                ReplaceCreatureInLineage(lineage, DLCSharedEnums.CreatureTemplateType.SpitLizard, WatcherEnums.CreatureTemplateType.IndigoLizard, GroundIndigoLizChance);
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BlackLizard, WatcherEnums.CreatureTemplateType.BasiliskLizard, BlackBasiliskLizChance);
                float blizLizChance = BlizzardLizardChance;
                if (Subregion == "Bitter Aerie" || SlugcatName == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                    blizLizChance *= 1.8f;
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.GreenLizard, WatcherEnums.CreatureTemplateType.BlizzardLizard, BalancedSpawns ? blizLizChance : BlizzardLizardChance, true);
            }

            //Mods
            if (replaceForRyanLiz)
                ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.RedLizard, new CreatureTemplate.Type("RyanLizard"), 1f);

            if (ActiveMods.Contains("thefriend"))
            {
                if (ForceFreshSpawns && lineage.creatureTypes[0] > -1 && lineage.creatureTypes[0] < StaticWorld.creatureTemplates.Length &&
                    StaticWorld.creatureTemplates[lineage.creatureTypes[0]].type == StaticWorld.GetCreatureTemplate("MotherLizard").type)
                {
                    World.SimpleSpawner invasionSpawner = new World.SimpleSpawner(lineage.region, spawners.Count, lineage.den,
                        StaticWorld.GetCreatureTemplate("YoungLizard").type, lineage.spawnData[0], UnityEngine.Random.value > .5f ? 3 : 4);

                    spawners.Add(invasionSpawner);
                }
            }
        }

        private void HandleCentipedeLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.SmallCentipede, CreatureTemplate.Type.Centipede, LargeCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centipede, CreatureTemplate.Type.RedCentipede, RedCentipedeChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.Centiwing, CreatureTemplate.Type.RedCentipede, RedCentipedeChance);
        }

        private void HandleLongLegsLineage(World.Lineage lineage, List<World.CreatureSpawner> spawners)
        {
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.BrotherLongLegs, CreatureTemplate.Type.DaddyLongLegs, DaddyLongLegsChance);
            ReplaceCreatureInLineage(lineage, CreatureTemplate.Type.DaddyLongLegs, DLCSharedEnums.CreatureTemplateType.TerrorLongLegs, TerrorLongLegsChance);

            //Inspector invasion
            if (HasAngryInspectors)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] > -1 && lineage.creatureTypes[i] < StaticWorld.creatureTemplates.Length && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type == CreatureTemplate.Type.DaddyLongLegs)
                    {
                        if (UnityEngine.Random.value < ((CurrentRegion == "UW" && BalancedSpawns) ? InspectorChance * 2 : InspectorChance))
                        {
                            World.SimpleSpawner inspectorSpawner = new World.SimpleSpawner
                                (lineage.region, spawners.Count, lineage.den, DLCSharedEnums.CreatureTemplateType.Inspector, "{Ignorecycle}", 1);
                            spawners.Add(inspectorSpawner);
                            break;
                        }
                    }
                }
            }
        }

        private void RandomizeLineageFirst(World.Lineage lineage)
        {
            int n = lineage.creatureTypes.Length;
            if (lineage.creatureTypes[n - 1] == CreatureTemplate.Type.RedLizard.Index ||
                lineage.creatureTypes[n - 1] == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard.Index ||
                lineage.creatureTypes[n - 1] == CreatureTemplate.Type.RedCentipede.Index)
                n--;
            int indexToCopy = UnityEngine.Random.Range(0, n);

            lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy];
            lineage.spawnData[0] = lineage.spawnData[indexToCopy];
            if (indexToCopy == n - 1 && n > 1)
            {
                if (lineage.creatureTypes[indexToCopy] == CreatureTemplate.Type.RedLizard.Index)
                {
                    if (UnityEngine.Random.value > RedLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard.Index)
                {
                    if (UnityEngine.Random.value > TrainLizardChance || UnityEngine.Random.value > RedLizardChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
                else if (lineage.creatureTypes[indexToCopy] == CreatureTemplate.Type.RedCentipede.Index)
                {
                    if (UnityEngine.Random.value > RedCentipedeChance)
                    {
                        lineage.creatureTypes[0] = lineage.creatureTypes[indexToCopy - 1];
                        lineage.spawnData[0] = lineage.spawnData[indexToCopy - 1];
                    }
                }
            }
        }

        public void SetUpLocalVariables(WorldLoader worldLoader)
        {
            TriedEchoLevi = false;
            HasBlackLizards = false;
            RegionHasDeers = false;
            VanillaHorizontalSpawn = null;
            WLoader = worldLoader;
            OriginalSpawnCount = SpawnerCount;

            if (ActiveMods.Contains("lb-fgf-m4r-ik.tronsx-region-code") || ActiveMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                foreach (World.SimpleSpawner simpleSpawner in worldLoader.spawners.OfType<World.SimpleSpawner>())
                {
                    if (simpleSpawner.creatureType == CreatureTemplate.Type.BlackLizard)
                    {
                        HasBlackLizards = true;
                        break;
                    }
                }
            }

            if (ModManager.Watcher)
            {
                //Need to check for deers for skywhale replacements.
                foreach (World.SimpleSpawner simpleSpawner in worldLoader.spawners.OfType<World.SimpleSpawner>())
                {
                    if (simpleSpawner.creatureType == CreatureTemplate.Type.Deer)
                    {
                        RegionHasDeers = true;
                        break;
                    }
                }
                //We find the first instance of a horizontal spawn.
                //Vanilla creatures should come before modded ones in the list.
                for (int i = 0; i < worldLoader.spawners.Count; ++i)
                {
                    if (worldLoader.spawners[i] is World.SimpleSpawner sSpawner)
                    {
                        if (HorizontalSpawns.Contains(sSpawner.creatureType))
                        {
                            VanillaHorizontalSpawn = sSpawner.creatureType;
                            break;
                        }
                    }
                }
            }
        }

        private void EnsureNormalScavengers(WorldLoader loader)
        {
            int scavs = 0, totalScavs = 0;
            World.SimpleSpawner scavSpawner = null;
            foreach (World.CreatureSpawner spawner in loader.spawners)
            {
                if (spawner is World.SimpleSpawner sspawner && (sspawner.spawnDataString == "" || sspawner.spawnDataString is null) && !sspawner.nightCreature
                    && StaticWorld.GetCreatureTemplate(sspawner.creatureType).TopAncestor().type == CreatureTemplate.Type.Scavenger)
                {
                    if (sspawner.creatureType == CreatureTemplate.Type.Scavenger)
                    {
                        scavs += sspawner.amount;
                        totalScavs += sspawner.amount;
                        scavSpawner = sspawner;
                    }
                    else
                    {
                        totalScavs += sspawner.amount;
                        if (scavSpawner is null)
                            scavSpawner = sspawner;
                    }
                }
            }
            if (scavs < 2 && totalScavs > 0)
            {
                World.SimpleSpawner invasionSpawner = CopySpawner(scavSpawner);
                invasionSpawner.creatureType = CreatureTemplate.Type.Scavenger;
                invasionSpawner.amount = (2 - scavs);
                invasionSpawner.inRegionSpawnerIndex = loader.spawners.Count;
                loader.spawners.Add(invasionSpawner);
            }
        }

        private bool DrainMiteStringValidFormat(string spawnDataString)
        {
            try
            {
                if (spawnDataString.Length < 3)
                    return false;
                string cutstr = spawnDataString.Substring(1, spawnDataString.Length - 2);
                string[] splitstr = cutstr.Split(',');
                if (splitstr.Length == 0)
                    return false;
                foreach (string field in splitstr)
                {
                    string[] splitField = field.Split(':');

                    if (splitField.Length < 2)
                        return false;
                    else
                    {
                        if (splitField[0] != "BodySize" && splitField[0] != "SizeMult" && splitField[0] != "FoodPips")
                            return false;
                        if (!float.TryParse(splitField[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                            return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
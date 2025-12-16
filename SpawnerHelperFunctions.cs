namespace ApexUpYourSpawns
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static ApexUtils;
    using static ApexGameInfo;

    //Hooks for all spawner-based vanilla/hardcoded creatures.
    public static class SpawnerHelperFunctions
    {
        public static void IncreaseCreatureSpawner(World.SimpleSpawner simpleSpawner, int amount = 1, bool divide = false)
        {
            if (amount <= 0)
                return;
            if (BalancedSpawns && BannedRooms.Contains(simpleSpawner.den.ResolveRoomName()))
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

        public static bool ReplaceMultiSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type replacement, float chance, bool forceNewSpawner = false)
        {
            if (replacement == null)
                return false;
            if (simpleSpawner.creatureType == replacement)
                return false;
            if (forceNewSpawner && SpawnerCount > OriginalSpawnCount * 4)
                return false;

            bool replacedFull = false;

            if (simpleSpawner.amount <= 1)
            {
                if (UnityEngine.Random.value < chance)
                {
                    if (!forceNewSpawner)
                    {
                        simpleSpawner.creatureType = replacement;
                        replacedFull = true;
                    }
                    else
                    {
                        World.SimpleSpawner replacementSpawner = CopySpawner(simpleSpawner);
                        replacementSpawner.creatureType = replacement;
                        replacementSpawner.amount = 1;
                        simpleSpawner.amount = 0;
                        replacementSpawner.inRegionSpawnerIndex = spawners.Count;
                        spawners.Add(replacementSpawner);

                    }
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
                    if (winningRolls == simpleSpawner.amount && !forceNewSpawner)
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

        public static bool AddInvasionSpawner(World.SimpleSpawner simpleSpawner, List<World.CreatureSpawner> spawners, CreatureTemplate.Type invador, float chance, bool singleRoll = false, bool reduceInvaded = false, bool removeInvaded = false, bool mirrorAmount = false)
        {
            if (invador == null)
                return false;
            if (simpleSpawner.creatureType == invador)
                return false;
            if (SpawnerCount > OriginalSpawnCount * 4)
                return false;

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
                if (mirrorAmount)
                    invasionSpawner.amount = simpleSpawner.amount;
                else
                    invasionSpawner.amount = 1;
                invasionSpawner.inRegionSpawnerIndex = spawners.Count;
                spawners.Add(invasionSpawner);
            }
            if (wonRoll && reduceInvaded)
                simpleSpawner.amount = (int)Mathf.Round(simpleSpawner.amount * 0.6f);
            if (wonRoll && removeInvaded)
                simpleSpawner.amount = 0;

            return wonRoll;
        }

        public static World.SimpleSpawner CopySpawner(World.SimpleSpawner origSpawner)
        {
            World.SimpleSpawner newSpawner = new World.SimpleSpawner(origSpawner.region, origSpawner.inRegionSpawnerIndex, origSpawner.den,
                origSpawner.creatureType, origSpawner.spawnDataString, origSpawner.amount);
            newSpawner.nightCreature = origSpawner.nightCreature;
            return newSpawner;
        }

        public static bool IsCreatureInLineage(World.Lineage lineage, CreatureTemplate.Type creatureType, bool useAncestor = false)
        {
            if (creatureType == null)
                return false;
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
            else if (creatureType.index > -1 && creatureType.index < StaticWorld.creatureTemplates.Length)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] > -1 && lineage.creatureTypes[i] < StaticWorld.creatureTemplates.Length && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type ==
                        StaticWorld.creatureTemplates[creatureType.Index].TopAncestor().type)
                    {
                        foundCreature = true;
                        break;
                    }
                }
            }

            return foundCreature;
        }

        public static bool ReplaceCreatureInLineage(World.Lineage lineage, CreatureTemplate.Type replacee, CreatureTemplate.Type replacement, float chance, bool useAncestors = false)
        {
            if (replacement is null || replacee is null)
                return false;
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
            else if (replacee.index > -1 && replacee.index < StaticWorld.creatureTemplates.Length)
            {
                for (int i = 0; i < lineage.creatureTypes.Length; ++i)
                {
                    if (lineage.creatureTypes[i] > -1 && lineage.creatureTypes[i] < StaticWorld.creatureTemplates.Length && StaticWorld.creatureTemplates[lineage.creatureTypes[i]].TopAncestor().type ==
                        StaticWorld.creatureTemplates[replacee.Index].TopAncestor().type && UnityEngine.Random.value < chance)
                    {
                        replacedCreature = true;
                        lineage.creatureTypes[i] = replacement.Index;
                    }
                }
            }

            return replacedCreature;
        }

        public static bool ReplaceCreatureInLineageIndex(World.Lineage lineage, CreatureTemplate.Type replacement, float chance, int index)
        {
            if (index < 0 || index >= lineage.creatureTypes.Length)
                return false;
            bool replacedCreature = false;
            if (UnityEngine.Random.value < chance)
            {
                replacedCreature = true;
                lineage.creatureTypes[index] = replacement.Index;
            }
            return replacedCreature;
        }

        public static void LogSpawner(World.CreatureSpawner spawner, int arrayIndex = -1)
        {
            if (spawner is null)
            {
                UnityEngine.Debug.Log("Received null spawner with index: " + arrayIndex);
            }
            else if (spawner is World.SimpleSpawner simpleSpawner)
            {
                UnityEngine.Debug.Log("Simple Spawner data:");
                UnityEngine.Debug.Log("ID: " + simpleSpawner.SpawnerID);
                UnityEngine.Debug.Log("Creature: " + simpleSpawner.creatureType);
                UnityEngine.Debug.Log("Amount: " + simpleSpawner.amount);
                UnityEngine.Debug.Log("Subregion: " + Subregion);
                UnityEngine.Debug.Log("In region index: " + simpleSpawner.inRegionSpawnerIndex);
                if (arrayIndex != -1)
                    UnityEngine.Debug.Log("Spawner array index: " + arrayIndex);
                UnityEngine.Debug.Log("Den: " + simpleSpawner.den.ToString());
                UnityEngine.Debug.Log("Den room: " + simpleSpawner.den.ResolveRoomName());
                UnityEngine.Debug.Log("Spawn data string: " + simpleSpawner.spawnDataString);
                UnityEngine.Debug.Log("Night creature: " + simpleSpawner.nightCreature.ToString());
            }
            else if (spawner is World.Lineage lineage)
            {
                string auxStr;
                UnityEngine.Debug.Log("Lineage data:");// + lineage.ToString());
                UnityEngine.Debug.Log("ID: " + lineage.SpawnerID);
                for (int j = 0; j < lineage.creatureTypes.Length; ++j)
                {
                    if (lineage.creatureTypes[j] > -1)
                        auxStr = StaticWorld.creatureTemplates[lineage.creatureTypes[j]].type.ToString();
                    else auxStr = "Null";
                    UnityEngine.Debug.Log("Creature " + (j + 1) + " : " + lineage.creatureTypes[j] + " (" +
                        auxStr + ")");
                }
                UnityEngine.Debug.Log("Subregion: " + Subregion);
                UnityEngine.Debug.Log("In region index: " + lineage.inRegionSpawnerIndex);
                if (arrayIndex != -1)
                    UnityEngine.Debug.Log("Spawner array index: " + arrayIndex);
                UnityEngine.Debug.Log("Den: " + lineage.den.ToString());
                UnityEngine.Debug.Log("Den room: " + lineage.den.ResolveRoomName());
                UnityEngine.Debug.Log("Spawn data strings: ");
                for (int j = 0; j < lineage.spawnData.Length; ++j)
                {
                    UnityEngine.Debug.Log("Creature " + j + " : " + lineage.spawnData[j]);
                }
            }
            UnityEngine.Debug.Log("\n");

        }

        public static WorldCoordinate FindOffScreenDen(WorldLoader loader)
        {
            foreach (World.CreatureSpawner spawner in loader.spawners)
            {
                if (spawner.den.ResolveRoomName().ToLower().Contains("offscreenden"))
                    return spawner.den;
            }

            return new WorldCoordinate(loader.world.firstRoomIndex + loader.roomAdder.Count, -1, -1, 0);
        }

        public static void FillLineage(World.Lineage lineage)
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
                for (int j = 0; j < lineage.creatureTypes.Length; ++j)
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
    }
}
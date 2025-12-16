
namespace ApexUpYourSpawns
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using RWCustom;
    using UnityEngine;
    using static ApexUtils;

    //Logic for game save states (preserving lineages)
    public class SaveStateHandler
    {
        Dictionary<string, List<World.Lineage>> lineagesToSave;

        public SaveStateHandler()
        {
            lineagesToSave = new Dictionary<string, List<World.Lineage>>();
        }

        public void ApplyHooks()
        {
            On.WinState.CycleCompleted += SaveSpawnersOnCycleComplete;
            On.RainWorldGame.GoToDeathScreen += RainWorldGame_GoToDeathScreen;
            On.RainWorldGame.GoToStarveScreen += RainWorldGame_GoToStarveScreen;
            On.Menu.CharacterSelectPage.AbandonButton_OnPressDone += CharacterSelectPage_AbandonButton_OnPressDone;
        }

        public void CharacterSelectPage_AbandonButton_OnPressDone(On.Menu.CharacterSelectPage.orig_AbandonButton_OnPressDone orig, Menu.CharacterSelectPage self, Menu.Remix.MixedUI.UIfocusable trigger)
        {
            orig(self, trigger);
            ResetSpawnBufferOnDeath();
        }

        public void RainWorldGame_GoToStarveScreen(On.RainWorldGame.orig_GoToStarveScreen orig, RainWorldGame self)
        {
            orig(self);
            ResetSpawnBufferOnDeath();
        }

        public void RainWorldGame_GoToDeathScreen(On.RainWorldGame.orig_GoToDeathScreen orig, RainWorldGame self)
        {
            orig(self);
            ResetSpawnBufferOnDeath();
        }

        public void ResetSpawnBufferOnDeath()
        {
            foreach (List<World.Lineage> lin in lineagesToSave.Values)
                lin.Clear();
            lineagesToSave.Clear();
        }

        public void SaveSpawnersOnCycleComplete(On.WinState.orig_CycleCompleted orig, WinState self, RainWorldGame game)
        {
            orig(self, game);

            if (game.rainWorld.safariMode || (game.IsStorySession && game.GetStorySession.saveState.malnourished))
                return;

            if (!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns");
            if (!Directory.Exists(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates"))
                Directory.CreateDirectory(Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates");

            SlugcatStats.Name player = null;
            int saveSlot = Math.Abs(game.rainWorld.options.saveSlot);
            bool expedition = game.rainWorld.ExpeditionMode;
            if (game.IsStorySession)
                player = game.GetStorySession.saveStateNumber;
            string baseSaveFilePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates/" +
                (expedition ? "e" : "") + saveSlot + player.ToString();

            foreach (string regionSave in lineagesToSave.Keys)
            {
                string filePath = baseSaveFilePath + regionSave + ".txt";
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (World.Lineage lineage in lineagesToSave[regionSave])
                    {
                        string linStr = lineage.SpawnerID.ToString() + "|";
                        foreach (int critIndex in lineage.creatureTypes)
                        {
                            string critName = "Empty";
                            if (critIndex > -1 && critIndex < StaticWorld.creatureTemplates.Count())
                                critName = StaticWorld.creatureTemplates[critIndex].type.ToString();
                            linStr += critName + ";";
                        }
                        linStr = linStr.Substring(0, linStr.Length - 1);

                        linStr += "|";
                        for (int i = 0; i < lineage.spawnData.Length; ++i)
                        {
                            if (lineage.spawnData[i] != null)
                                linStr += lineage.spawnData[i];
                            linStr += ";";
                        }
                        linStr = linStr.Substring(0, linStr.Length - 1);
                        writer.WriteLine(linStr);
                    }

                }
                lineagesToSave[regionSave].Clear();
            }
            lineagesToSave.Clear();
        }

        public bool LoadSaveState(WorldLoader worldLoader)
        {
            bool needsFresh = false;
            Dictionary<int, KeyValuePair<int[], string[]>> linSaveState = SavedLineagesToDictionary(worldLoader);

            if (DebugLogs)
            {
                Debug.Log("READ DICTIONARY:");
                foreach (int i in linSaveState.Keys)
                {
                    Debug.Log("\n   ===========\n");
                    Debug.Log("ID: " + i);
                    foreach (int j in linSaveState[i].Key)
                        Debug.Log(j);

                }
                Debug.Log("\n");
            }

            foreach (World.Lineage lineage in worldLoader.spawners.OfType<World.Lineage>())
            {
                if (linSaveState.ContainsKey(lineage.SpawnerID))
                {
                    lineage.creatureTypes = linSaveState[lineage.SpawnerID].Key;
                    lineage.spawnData = linSaveState[lineage.SpawnerID].Value;

                }
            }
            linSaveState.Clear();

            /*
             * Offscreen dens
            List<World.SimpleSpawner> offScreenSaveState = SavedSpawnersToList(worldLoader);
            if(offScreenSaveState.Count > 0)
            {
                WorldCoordinate offScreenDen = FindOffScreenDen(worldLoader);
                foreach (World.SimpleSpawner spawner in worldLoader.spawners.OfType<World.SimpleSpawner>())
                {
                    if (spawner.den == offScreenDen)
                    {
                        spawner.creatureType = offScreenSaveState[0].creatureType;
                        spawner.amount = offScreenSaveState[0].amount;
                        spawner.spawnDataString = offScreenSaveState[0].spawnDataString;
                        offScreenSaveState.RemoveAt(0);
                        if (offScreenSaveState.Count == 0)
                            break;
                    }
                }
                if (offScreenSaveState.Count > 0)
                    needsFresh = true;
                //
                while (offScreenSaveState.Count > 0)
                {
                    offScreenSaveState[0].inRegionSpawnerIndex = worldLoader.spawners.Count;
                    worldLoader.spawners.Add(offScreenSaveState[0]);
                    offScreenSaveState.RemoveAt(0);
                }
            }*/
            return needsFresh;
        }

        public void AddSpawnersToBuffer(WorldLoader loader)
        {
            List<World.Lineage> lineages = new List<World.Lineage>();
            //WorldCoordinate offScreenDen = FindOffScreenDen(loader);
            foreach (World.CreatureSpawner spawner in loader.spawners)
            {
                if (spawner is World.Lineage lin)
                    lineages.Add(lin);
            }
            if (lineages.Count > 0)
            {
                if (lineagesToSave.ContainsKey(loader.worldName))
                {
                    Debug.Log("WARNING: Trying to save state of region twice: " + loader.worldName);
                    lineagesToSave[loader.worldName] = lineages;
                }
                else
                    lineagesToSave.Add(loader.worldName, lineages);
            }
        }

        public Dictionary<int, KeyValuePair<int[], string[]>> SavedLineagesToDictionary(WorldLoader loader)
        {
            Dictionary<int, KeyValuePair<int[], string[]>> lineagesDict = new Dictionary<int, KeyValuePair<int[], string[]>>();
            string filePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates/";
            SlugcatStats.Name player = null;
            int saveSlot = Math.Abs(loader.game.rainWorld.options.saveSlot);
            bool expedition = loader.game.rainWorld.ExpeditionMode;
            if (loader.game.IsStorySession)
                player = loader.game.GetStorySession.saveStateNumber;
            filePath += (expedition ? "e" : "") + saveSlot + player.ToString() + loader.worldName + ".txt";
            StreamReader sr = null;
            try
            {
                if (File.Exists(filePath))
                {
                    sr = new StreamReader(filePath);
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] baseSplits = line.Split('|');
                        string[] creatureSplits = baseSplits[1].Split(';');
                        int[] critArray = new int[creatureSplits.Length];
                        for (int i = 0; i < creatureSplits.Length; ++i)
                        {
                            if (creatureSplits[i] == "Empty")
                                critArray[i] = -1;
                            else
                                critArray[i] = new CreatureTemplate.Type(creatureSplits[i]).index;
                        }
                        string[] spawnDataSplit = baseSplits[2].Split(';');
                        if (spawnDataSplit.Length != creatureSplits.Length)
                            spawnDataSplit = new string[creatureSplits.Length];
                        for (int i = 0; i < spawnDataSplit.Length; ++i)
                        {
                            if (spawnDataSplit[i] != null && (spawnDataSplit[i].Length < 3 ||
                                !spawnDataSplit[i].StartsWith("{") || !spawnDataSplit[i].EndsWith("}")))
                                spawnDataSplit[i] = null;
                        }
                        lineagesDict.Add(int.Parse(baseSplits[0]), new KeyValuePair<int[], string[]>(critArray, spawnDataSplit));

                        line = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR WHILE READING FROM FILE.");
                Debug.Log(e.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            return lineagesDict;
        }

        private List<World.SimpleSpawner> SavedSpawnersToList(WorldLoader loader)
        {
            List<World.SimpleSpawner> savedSpawners = new List<World.SimpleSpawner>();
            string filePath = Custom.RootFolderDirectory() + "/ApexUpYourSpawns/Savestates/";
            SlugcatStats.Name player = null;
            int saveSlot = Math.Abs(loader.game.rainWorld.options.saveSlot);
            bool expedition = loader.game.rainWorld.ExpeditionMode;
            if (loader.game.IsStorySession)
                player = loader.game.GetStorySession.saveStateNumber;
            filePath += (expedition ? "e" : "") + saveSlot + player.ToString() + loader.worldName + ".txt";

            if (File.Exists(filePath))
            {
                WorldCoordinate offScreenDen = SpawnerHelperFunctions.FindOffScreenDen(loader);
                StreamReader sr = new StreamReader(filePath);
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (!line.StartsWith("===="))
                    {
                        line = sr.ReadLine();
                        continue;
                    }
                    else
                    {
                        line = sr.ReadLine();
                        break;
                    }
                }
                while (line != null)
                {

                    World.SimpleSpawner spawner = new World.SimpleSpawner(0, 0, offScreenDen, CreatureTemplate.Type.StandardGroundCreature,
                        null, 0);
                    string[] splitLine = line.Split('|');
                    string[] splitID = splitLine[0].Split(':');
                    spawner.region = int.Parse(splitID[0]);
                    spawner.inRegionSpawnerIndex = int.Parse(splitID[1]);
                    string[] spawnData = splitLine[1].Split(',');

                    if (!spawnData[0].Contains(":"))
                    {
                        int index = int.Parse(spawnData[0]);
                        spawner.creatureType = StaticWorld.creatureTemplates[index].type;
                    }
                    else
                    {
                        string[] splitType = spawnData[0].Split(':');
                        int index = int.Parse(splitType[1]);
                        if (index > -1 && index < StaticWorld.creatureTemplates.Length &&
                            StaticWorld.creatureTemplates[index].type.value == splitType[0])
                            spawner.creatureType = StaticWorld.creatureTemplates[index].type;
                        else
                            spawner.creatureType = new CreatureTemplate.Type(splitType[0]);
                    }

                    spawner.amount = int.Parse(spawnData[1]);
                    if (spawnData[2].Length > 2 && spawnData[2].StartsWith("{") && spawnData[2].EndsWith("}"))
                        spawner.spawnDataString = spawnData[2];
                    savedSpawners.Add(spawner);

                    line = sr.ReadLine();
                }
                sr.Close();
            }
            return savedSpawners;
        }

        public void ClearDictionaries()
        {
            foreach (List<World.Lineage> lins in lineagesToSave.Values)
                lins.Clear();
            lineagesToSave.Clear();
        }
    }
}
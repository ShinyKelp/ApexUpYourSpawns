

using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace ApexUpYourSpawns
{
    using BepInEx;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using RWCustom;
    using UnityEngine;
    using Watcher;
    using static ApexUtils;     //Not global to differentiate which classes do or don't have access to these.
    using static ApexGameInfo;
    using static SpawnerHelperFunctions;

    [BepInPlugin("ShinyKelp.ApexUpYourSpawns", "ApexUpYourSpawns", "1.6.0")]

    public class ApexUpYourSpawnsMod : BaseUnityPlugin
    {
        #region Mod Setup

        private bool IsInit;

        private ApexUpYourSpawnsOptions ApexOptions;

        private ModCreatureLogic ModHandler;

        private MiscCreatureLogic MiscHandler;

        private SpawnerCreatureLogic SpawnerHandler;

        SaveStateHandler SaveStateHandler;
        
        //Dictionary<string, List<World.SimpleSpawner>> spawnersToSave = new Dictionary<string, List<World.SimpleSpawner>>();
        
        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
            Debug.Log("ON ENABLE. CREATING OPTIONS.");
            ApexOptions = new ApexUpYourSpawnsOptions();
        }

        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;
                if (ModManager.MSC || ModManager.Watcher)
                    HasSharedDLC = true;
                //hooks go here
                On.GameSession.ctor += GameSessionOnctor;
                //On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
                On.WorldLoader.GeneratePopulation += GenerateCustomPopulation;

                //*/

                ActiveMods.Clear();
                foreach (ModManager.Mod mod in ModManager.ActiveMods)
                {
                    if (SupportedMods.Contains(mod.id))
                        ActiveMods.Add(mod.id);
                }
                if (DebugLogs)
                {
                    Debug.Log("Mods detected: ");
                    foreach (string s in ActiveMods)
                        Debug.Log(s);
                }

                ApexOptions.UpdateAllBindings();

                ModHandler = new ModCreatureLogic();
                ModHandler.SetUpModDependencies();

                SpawnerHandler = new SpawnerCreatureLogic(ModHandler);

                MiscHandler = new MiscCreatureLogic();
                MiscHandler.ApplyHooks();

                SaveStateHandler = new SaveStateHandler();
                SaveStateHandler.ApplyHooks();

                MachineConnector.SetRegisteredOI("ShinyKelp.ApexUpYourSpawns", ApexOptions);
                IsInit = true;
                UnityEngine.Debug.Log("Apex Up Your Spawns setup finished successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
        
        #endregion

        #region World Generation
        private void GenerateCustomPopulation(On.WorldLoader.orig_GeneratePopulation orig, WorldLoader worldLoader, bool fresh)
        {
            try
            {
                if (ForceFreshSpawns && !fresh)
                {
                    fresh = true;
                    foreach (AbstractRoom abstractRoom in worldLoader.abstractRooms)
                    {
                        if (!abstractRoom.shelter)
                        {
                            abstractRoom.creatures.Clear();
                            abstractRoom.entitiesInDens.Clear();
                        }
                    }
                }
                if (fresh)
                {
                    if (!HasUpdatedRefs)
                    {
                        //UnityEngine.Debug.Log("\nUpdating modded creature references...");
                        ModHandler.RefreshModCreatures();
                        //UnityEngine.Debug.Log("Modded creature references updated.\n");
                    }

                    SpawnerHandler.SetUpLocalVariables(worldLoader);
                    string storySession =  (SlugcatName is null) ? "null" : SlugcatName.ToString();
                    Debug.Log("Starting spawn modifications for region: " + CurrentRegion + " , character: " + 
                        storySession);
                    Debug.Log("ORIGINAL SPAWN COUNT: " + SpawnerCount);
                    UnityEngine.Random.InitState(Mathf.RoundToInt(Time.time * 10f));
                    
                    SpawnerHandler.HandleAllSpawners();
                    
                    if (!worldLoader.game.rainWorld.safariMode)
                        SaveStateHandler.AddSpawnersToBuffer(worldLoader);
                    else
                        SaveStateHandler.ResetSpawnBufferOnDeath();
                }
                else
                {

                    if (!worldLoader.game.rainWorld.safariMode)
                    {
                        bool needsFresh = SaveStateHandler.LoadSaveState(worldLoader);
                        //fresh = needsFresh;
                    }
                }

                ConfigureRoomAttractions(worldLoader.world);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }

            orig(worldLoader, fresh);
        }

        private void ConfigureRoomAttractions(World self)
        {
            if (ModManager.Watcher)
            {
                //We previously search for the first creature with horizontal screen spawns (miros, deer, etc).
                //And we use that one as the template to set the room attractions for everyone else.
                if (VanillaHorizontalSpawn != null)
                {
                    foreach (AbstractRoom room in self.abstractRooms)
                    {
                        foreach (CreatureTemplate.Type hSpawn in HorizontalSpawns)
                        {
                            if (hSpawn != CreatureTemplate.Type.Deer)
                                room.roomAttractions[hSpawn.Index] = room.roomAttractions[VanillaHorizontalSpawn.Index];
                        }
                        if (RegionHasDeers)
                            room.roomAttractions[WatcherEnums.CreatureTemplateType.SkyWhale.Index] = room.roomAttractions[CreatureTemplate.Type.Deer.Index];
                    }
                }
            }
        }
        #endregion

        #region Game Manager functions
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            CurrentGame = game;
        }

        void OnDestroy()
        {
            ClearDictionaries();
            ApexOptions = null;
        }

        private void ClearDictionaries()
        {
            SaveStateHandler.ClearDictionaries();
            ModHandler.ClearDictionaries();
        }
        #endregion
    }
}
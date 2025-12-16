namespace ApexUpYourSpawns
{
    //All runtime variables
    public static class ApexGameInfo
    {
        public static RainWorldGame CurrentGame = null;
        public static SlugcatStats.Name SlugcatName
        {
            get => (ApexGameInfo.CurrentGame is null || !ApexGameInfo.CurrentGame.IsStorySession) ? null :
                ApexGameInfo.CurrentGame.GetStorySession.saveState.saveStateNumber;
        }

        public static int CurrentSpawnerIndex, OriginalSpawnCount;
        public static WorldLoader WLoader;
        public static int FirstRoomIndex
        {
            get => WLoader.world.firstRoomIndex;
        }
        public static int NumberOfRooms
        {
            get => WLoader.world.NumberOfRooms;
        }
        public static string CurrentRegion
        {
            get => WLoader.worldName;
        }
        public static string Subregion
        {
            get => (WLoader.spawners[CurrentSpawnerIndex].den.room < FirstRoomIndex ||
                    WLoader.spawners[CurrentSpawnerIndex].den.room >= FirstRoomIndex + NumberOfRooms) ? "" :
                WLoader.abstractRooms[WLoader.spawners[CurrentSpawnerIndex].den.room - FirstRoomIndex].subregionName;
        }
        public static string RoomName
        {
            get => WLoader.spawners[CurrentSpawnerIndex].den.ResolveRoomName();
        }
        public static int SpawnerCount
        {
            get => WLoader.spawners.Count;
        }

        public static bool LastWasError;

        public static bool RegionHasDeers = false;

        public static CreatureTemplate.Type VanillaHorizontalSpawn = null;

        public static bool TriedEchoLevi = false;

        public static bool HasBlackLizards = false;

    }
}
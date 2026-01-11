namespace ApexUpYourSpawns
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil.Cil;
    using MonoMod.Cil;
    using MoreSlugcats;
    using UnityEngine;
    using Watcher;
    using static ApexUtils;
    using static ApexGameInfo;
    using static ConfigShortcuts;

    //Hooks for all creatures that do not use the traditional spawner system.
    public class MiscCreatureLogic
    {
        public MiscCreatureLogic()
        {

        }
        public void ApplyHooks()
        {

            if (HasSharedDLC)
            {
                On.JellyFish.PlaceInRoom += JellyfishSpawn;
                On.DangleFruit.PlaceInRoom += ReplaceStowawayBugBlueFruit;
                On.MoreSlugcats.GooieDuck.PlaceInRoom += ReplaceStowawayBugGooieDuck;
            }
            if (ModManager.Watcher)
            {
                On.VultureGrub.PlaceInRoom += ReplaceVultureWithSandGrub;
                On.SeedCob.PlaceInRoom += AddSandTrapToPopcorn;
                On.Watcher.SandGrubNetwork.MarkConsumed += PreventSandGrubPullCrash;
                On.Hazer.PlaceInRoom += ReplaceHazerWithTardigrade;
                On.Room.ReadyForAI += AddBoxWormsOnPearls;
                On.SkyWhaleAbstractAI.CheckBlacklist += SkyWhale_ExtraBlacklist;
            }
            if (ActiveMods.Contains("ShinyKelp.ScavengerTweaks"))
            {
                On.Room.ReadyForAI += AddScavKings;
            }
            if (ActiveMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                On.LizardGraphics.ctor += ForceBlackMoleSalamander;
                On.WaterNut.PlaceInRoom += ReplaceWaterNutWithWaterBlob;
                On.Room.ReadyForAI += ReplaceHazersWithMoms;

                On.AbstractCreature.ctor += ForceFlagsOnBatfly;
                On.AbstractCreature.setCustomFlags += PutSeedBatFlagOnFly;
            }
            //PupLongLegs
            if (ModManager.MSC)
            {
                IL.AbstractRoom.RealizeRoom += ReplaceSlugpupForHLLRoom;
                IL.World.SpawnPupNPCs += ReplaceSlugpupForHLL;
                On.DaddyLongLegs.ctor += GiveHunterDaddyPupColor;
                On.DaddyGraphics.ApplyPalette += GiveHunterDaddyPupPalette;
                On.DaddyGraphics.HunterDummy.ApplyPalette += GiveHunterDaddyDummyPupPalette;
                On.DaddyGraphics.HunterDummy.DrawSprites += GiveHunterDaddyDummyPupSprites;
                On.DaddyGraphics.HunterDummy.Update += GiveHunterDaddyDummyPupSize;
                On.DaddyGraphics.HunterDummy.ctor += GiveHunterDaddyDummyPupTailSize;
                //On.DaddyGraphics.DaddyDangleTube.ApplyPalette += GiveHunterDaddyDangleTubePupPallete;
                //On.DaddyGraphics.DaddyTubeGraphic.ApplyPalette += GiveHunterDaddyTubePupPallete;
                //On.DaddyGraphics.DaddyDeadLeg.ApplyPalette += GiveHunterDaddyDeadLegPupPallete;
            }
        }



        private void ForceFlagsOnBatfly(On.AbstractCreature.orig_ctor orig, AbstractCreature self, World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
        {
            orig(self, world, creatureTemplate, realizedCreature, pos, ID);
            if (creatureTemplate.type == CreatureTemplate.Type.Fly && !CurrentGame.IsArenaSession)
                self.setCustomFlags();
        }

        private void PutSeedBatFlagOnFly(On.AbstractCreature.orig_setCustomFlags orig, AbstractCreature self)
        {
            if (self.creatureTemplate.type == CreatureTemplate.Type.Fly)
            {
                if(OptionConfigs.Instance.GetOptionConfigValue("SeedBatChance")/100f > UnityEngine.Random.value)
                {
                    if (self.spawnData != null && self.spawnData.Length > 0)
                    {
                        string spawnData = self.spawnData.Remove(self.spawnData.Length - 1);
                        spawnData += ":seedbat}";
                        self.spawnData = spawnData;
                    }
                    else
                        self.spawnData = "{seedbat}";
                }
            }
            orig(self);
        }


        #region Creature Replacement functions
        private void ReplaceVultureWithSandGrub(On.VultureGrub.orig_PlaceInRoom orig, VultureGrub self, Room placeRoom)
        {
            if (ModManager.Watcher && !CurrentGame.IsArenaSession && UnityEngine.Random.value < GrubSandGrubChance)
            {
                AddSandWorm(placeRoom, self.abstractPhysicalObject.pos, false);

            }
            else
                orig(self, placeRoom);
        }

        private void AddSandTrapToPopcorn(On.SeedCob.orig_PlaceInRoom orig, SeedCob self, Room placeRoom)
        {
            if (ModManager.Watcher && !CurrentGame.IsArenaSession && !self.AbstractCob.dead && UnityEngine.Random.value < PopcornSandWormTrapChance)
            {
                AddSandWorm(placeRoom, self.abstractPhysicalObject.pos, true);
            }
            orig(self, placeRoom);
        }

        private void AddSandWorm(Room placeRoom, WorldCoordinate objPos, bool isBig)
        {
            SandGrubBurrow sandGrubBurrow = new SandGrubBurrow(null, null);
            placeRoom.AddObject(sandGrubBurrow);
            Vector2 origPos = new Vector2(objPos.x, objPos.y + 2);
            Vector2 newPos = new Vector2(((float)origPos.x + 1f) * 20f - 20f, ((float)origPos.y + 1f) * 20f - 20f);
            sandGrubBurrow.pos = placeRoom.FindGroundBelow(newPos, out sandGrubBurrow.dir, 200f);
            SandGrubNetwork sgNet = null;
            foreach (UpdatableAndDeletable udel in placeRoom.updateList)
            {
                if (udel is SandGrubNetwork sgn)
                {
                    sgNet = sgn; break;
                }
            }

            if (sgNet is null)
                placeRoom.AddObject(new SandGrubNetwork(sandGrubBurrow.pos, 1f, 1f, isBig ? 1f : 0f, true));
            else
            {
                Vector2 netPos = Vector2.zero;
                int count = 0;

                foreach (UpdatableAndDeletable udel in placeRoom.updateList)
                {
                    if (udel is SandGrubBurrow burrow)
                    {
                        count++;
                        netPos += burrow.pos;
                    }
                }
                netPos /= count;
                sgNet.pos = netPos;
            }
        }

        private void AddBoxWormsOnPearls(On.Room.orig_ReadyForAI orig, Room self)
        {
            orig(self);
            if (self.abstractRoom.shelter || self.abstractRoom.scavengerOutpost)
                return;
            foreach (UpdatableAndDeletable obj in self.updateList)
                if (obj is ScavengerTreasury)
                    return;
            float localChance = PearlFireSpriteChance;
            foreach (PlacedObject pobj in self.roomSettings.placedObjects)
            {
                if (pobj.type == PlacedObject.Type.ScavengerTreasury)
                {
                    localChance *= 0.5f;
                    break;
                }
            }
            List<AbstractPhysicalObject> list = new List<AbstractPhysicalObject>();
            foreach (AbstractWorldEntity ent in self.abstractRoom.entities)
            {
                if (ent is AbstractPhysicalObject phob && phob.type == AbstractPhysicalObject.AbstractObjectType.DataPearl && UnityEngine.Random.value < (BalancedSpawns ? localChance : PearlFireSpriteChance))
                    list.Add(phob);
            }
            foreach (AbstractPhysicalObject phob in list)
            {
                AbstractCreature boxWrm = new AbstractCreature(CurrentGame.world, StaticWorld.GetCreatureTemplate(WatcherEnums.CreatureTemplateType.BoxWorm), null, new WorldCoordinate(self.abstractRoom.index, phob.pos.x, phob.pos.y - 1, 0), CurrentGame.GetNewID());
                self.abstractRoom.AddEntity(boxWrm);
                boxWrm.RealizeInRoom();

                AbstractCreature fireSpr = new AbstractCreature(CurrentGame.world, StaticWorld.GetCreatureTemplate(WatcherEnums.CreatureTemplateType.FireSprite), null, new WorldCoordinate(self.abstractRoom.index, phob.pos.x, phob.pos.y - 1, 0), CurrentGame.GetNewID());
                self.abstractRoom.AddEntity(fireSpr);
                fireSpr.RealizeInRoom();
            }
        }

        private void ReplaceWaterNutWithWaterBlob(On.WaterNut.orig_PlaceInRoom orig, WaterNut self, Room placeRoom)
        {
            if (OptionConfigs.Instance.GetOptionConfigValue("WaterBlobChance")/100f > UnityEngine.Random.value)
            {
                AbstractCreature blobAbs = new AbstractCreature(placeRoom.world, StaticWorld.GetCreatureTemplate("WaterBlob"), null, new WorldCoordinate(placeRoom.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), CurrentGame.GetNewID());
                placeRoom.abstractRoom.AddEntity(blobAbs);
                blobAbs.RealizeInRoom();
            }
            else orig(self, placeRoom);
        }

        private void ReplaceHazerWithTardigrade(On.Hazer.orig_PlaceInRoom orig, Hazer self, Room room)
        {
            if (ModManager.Watcher && !CurrentGame.IsArenaSession && UnityEngine.Random.value < HazerTardigradeChance)
            {
                AbstractCreature tardAbs = new AbstractCreature(CurrentGame.world, StaticWorld.GetCreatureTemplate(WatcherEnums.CreatureTemplateType.Tardigrade), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), CurrentGame.GetNewID());
                tardAbs.state = new Tardigrade.TardigradeState(tardAbs);
                if (UnityEngine.Random.value < 0.1f)
                    (tardAbs.state as Tardigrade.TardigradeState).slayer = true;
                Tardigrade tardi = new Tardigrade(tardAbs, CurrentGame.world);
                tardAbs.realizedCreature = tardi;
                tardAbs.abstractAI.RealAI = new TardigradeAI(tardAbs, CurrentGame.world);

                tardi.PlaceInRoom(room);
            }
            else
                orig(self, room);
        }

        private void JellyfishSpawn(On.JellyFish.orig_PlaceInRoom orig, JellyFish self, Room room)
        {
            if (HasSharedDLC && !CurrentGame.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < GiantJellyfishChance)
            {
                AbstractCreature myBigJelly = new AbstractCreature(CurrentGame.world, StaticWorld.GetCreatureTemplate(DLCSharedEnums.CreatureTemplateType.BigJelly), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y - 1, 0), CurrentGame.GetNewID());
                BigJellyFish myJelly = new BigJellyFish(myBigJelly, CurrentGame.world);
                myJelly.PlaceInRoom(room);
            }
            else
                orig(self, room);
        }

        private void ReplaceStowawayBugBlueFruit(On.DangleFruit.orig_PlaceInRoom orig, DangleFruit self, Room room)
        {
            if (HasSharedDLC && !CurrentGame.IsArenaSession && !room.abstractRoom.shelter && UnityEngine.Random.value < StowawayChance)
            {
                self.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(self, room, self.firstChunk.pos);

                AbstractCreature myStowawayAbstract = new AbstractCreature(CurrentGame.world, StaticWorld.GetCreatureTemplate(DLCSharedEnums.CreatureTemplateType.StowawayBug),
                    null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), CurrentGame.GetNewID());

                Vector2 pos = new Vector2((self.abstractPhysicalObject.pos.x + 1) * 20f - 10f, (self.abstractPhysicalObject.pos.y + 1) * 20f - 20f + stalk.ropeLength);

                (myStowawayAbstract.state as StowawayBugState).HomePos = new Vector2(pos.x, pos.y);
                pos.y -= 60f;
                (myStowawayAbstract.state as StowawayBugState).aimPos = pos;
                (myStowawayAbstract.state as StowawayBugState).debugForceAwake = true;
                myStowawayAbstract.pos.abstractNode = 0;

                StowawayBug myBug = new StowawayBug(myStowawayAbstract, CurrentGame.world);

                myBug.AI = new StowawayBugAI(myStowawayAbstract, CurrentGame.world);

                myBug.PlaceInRoom(room);
            }

            orig(self, room);
        }

        private void ReplaceStowawayBugGooieDuck(On.MoreSlugcats.GooieDuck.orig_PlaceInRoom orig, GooieDuck self, Room room)
        {
            if (HasSharedDLC && CurrentGame.IsStorySession && !room.abstractRoom.shelter && UnityEngine.Random.value < (BalancedSpawns ? StowawayChance * 2 : StowawayChance))
            {
                DangleFruit fruit = new DangleFruit(self.abstractPhysicalObject);
                fruit.firstChunk.HardSetPosition(room.MiddleOfTile(self.abstractPhysicalObject.pos));
                DangleFruit.Stalk stalk = new DangleFruit.Stalk(fruit, room, fruit.firstChunk.pos);
                AbstractCreature myStowawayAbstract = new AbstractCreature(CurrentGame.world, StaticWorld.GetCreatureTemplate(DLCSharedEnums.CreatureTemplateType.StowawayBug), null, new WorldCoordinate(room.abstractRoom.index, self.abstractPhysicalObject.pos.x, self.abstractPhysicalObject.pos.y + 3, 0), CurrentGame.GetNewID());
                Vector2 pos = new Vector2((self.abstractPhysicalObject.pos.x + 1) * 20f - 10f, (self.abstractPhysicalObject.pos.y + 1) * 20f - 20f + stalk.ropeLength);

                (myStowawayAbstract.state as StowawayBugState).HomePos = new Vector2(pos.x, pos.y);
                pos.y -= 60f;
                (myStowawayAbstract.state as StowawayBugState).aimPos = pos;
                (myStowawayAbstract.state as StowawayBugState).debugForceAwake = true;
                myStowawayAbstract.pos.abstractNode = 0;

                StowawayBug myBug = new StowawayBug(myStowawayAbstract, CurrentGame.world);
                myBug.AI = new StowawayBugAI(myStowawayAbstract, CurrentGame.world);
                myBug.PlaceInRoom(room);
            }
            orig(self, room);
        }

        CreatureTemplate.Type hazMomType;
        private void ReplaceHazersWithMoms(On.Room.orig_ReadyForAI orig, Room self)
        {
            if (CurrentGame != null && !CurrentGame.IsArenaSession && ActiveMods.Contains("lb-fgf-m4r-ik.modpack"))
            {
                List<AbstractCreature> hazers = new List<AbstractCreature>();
                foreach (AbstractCreature abstractCreature in self.abstractRoom.creatures)
                {
                    if(abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Hazer)
                        hazers.Add(abstractCreature);
                }
                foreach (AbstractCreature hazer in hazers)
                {
                    if (OptionConfigs.Instance.GetOptionConfigValue("HazerMomChance") / 100f > UnityEngine.Random.value)
                    {
                        if (hazMomType == null)
                            hazMomType = new CreatureTemplate.Type("HazerMom");
                        self.abstractRoom.creatures.Remove(hazer);
                        //For some reason, StaticWorld.GetCreatureTemplate does not work for HazerMom. Using the template type index as work-around.
                        AbstractCreature hazMomAbs = new AbstractCreature(self.world, StaticWorld.creatureTemplates[hazMomType.Index], null, new WorldCoordinate(self.abstractRoom.index, hazer.pos.x, hazer.pos.y - 1, 0), CurrentGame.GetNewID());
                        hazMomAbs.superSizeMe = true;
                        self.abstractRoom.creatures.Add(hazMomAbs);
                    }
                }
            }
            orig(self);
        }

        private void AddScavKings(On.Room.orig_ReadyForAI orig, Room self)
        {
            if (ModManager.MSC && KingScavengerChance > 0 && self.abstractRoom.name != "LC_FINAL" && ActiveMods.Contains("ShinyKelp.ScavengerTweaks"))
            {
                List<AbstractCreature> elitesList = new List<AbstractCreature>();
                List<AbstractCreature> removedElitesList = new List<AbstractCreature>();
                if (self.game != null)
                {
                    foreach (AbstractCreature abstractCreature in self.abstractRoom.creatures)
                    {
                        if (abstractCreature.realizedCreature == null && abstractCreature.creatureTemplate.type == DLCSharedEnums.CreatureTemplateType.ScavengerElite)
                            elitesList.Add(abstractCreature);

                    }
                }

                float localChance = KingScavengerChance;
                if (self.abstractRoom.scavengerOutpost)
                    localChance *= 4f;
                else if (self.abstractRoom.scavengerTrader)
                    localChance *= 0.5f;

                foreach (AbstractCreature eliteAbstract in elitesList)
                {
                    float value = UnityEngine.Random.value;
                    if (value < (BalancedSpawns ? localChance : KingScavengerChance))
                    {
                        AbstractCreature kingAbstract = new AbstractCreature(eliteAbstract.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing),
                            null, eliteAbstract.pos, eliteAbstract.ID);
                        self.abstractRoom.creatures.Remove(eliteAbstract);
                        self.abstractRoom.creatures.Add(kingAbstract);
                        removedElitesList.Add(eliteAbstract);
                        if (self.abstractRoom.scavengerOutpost)
                            localChance = (KingScavengerChance - 0.04f);
                    }
                }
                orig(self);
                foreach (AbstractCreature removedElite in removedElitesList)
                    self.abstractRoom.creatures.Add(removedElite);
            }
            else
                orig(self);
        }

        #endregion

        #region HunterLongLegs functions

        private void ReplaceSlugpupForHLLRoom(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(2),
                x => x.MatchLdfld<AbstractCreature>("state"),
                x => x.MatchIsinst<PlayerNPCState>(),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<PlayerState>("foodInStomach")
                );
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 2);
            c.EmitDelegate<Action<AbstractRoom, AbstractCreature>>((absRoom, pupAbstract) =>
            {
                float hunterLongLegsChance = (float)OptionConfigs.Instance.GetOptionConfigValue("HunterLongLegsChance") / 100f;
                if (hunterLongLegsChance > UnityEngine.Random.value)
                {
                    AbstractCreature hllReplacement = new AbstractCreature(pupAbstract.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy),
                            null, pupAbstract.pos, pupAbstract.ID);
                    absRoom.RemoveEntity(pupAbstract);
                    absRoom.AddEntity(hllReplacement);
                }
            });
        }

        private void ReplaceSlugpupForHLL(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(13),
                x => x.MatchLdfld<AbstractCreature>("state"),
                x => x.MatchIsinst<PlayerNPCState>(),
                x => x.MatchLdcI4(1),
                x => x.MatchStfld<PlayerState>("foodInStomach")
                );
            c.Emit(OpCodes.Ldloc, 6);
            c.Emit(OpCodes.Ldloc, 13);
            c.EmitDelegate<Action<AbstractRoom, AbstractCreature>>((absRoom, pupAbstract) =>
            {
                float hunterLongLegsChance = (float)OptionConfigs.Instance.GetOptionConfigValue("HunterLongLegsChance") / 100f;
                if (hunterLongLegsChance > UnityEngine.Random.value)
                {
                    AbstractCreature hllReplacement = new AbstractCreature(pupAbstract.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy),
                            null, pupAbstract.pos, pupAbstract.ID);
                    absRoom.RemoveEntity(pupAbstract);
                    absRoom.AddEntity(hllReplacement);
                    if (absRoom.realizedRoom != null)
                        hllReplacement.RealizeInRoom();
                    else
                        absRoom.RealizeRoom(pupAbstract.world, pupAbstract.world.game);
                }
            });
        }

        private void GiveHunterDaddyPupColor(On.DaddyLongLegs.orig_ctor orig, DaddyLongLegs self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.isHD)
            {
                if ((CurrentGame.IsStorySession && self.abstractCreature.Room.shelter) || (!CurrentGame.IsStorySession && UnityEngine.Random.value < .8f))
                {
                    UnityEngine.Random.InitState(self.abstractCreature.ID.RandomSeed);
                    float Met = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 1.5f);
                    float Stealth = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 1.5f);

                    float H = Mathf.Lerp(UnityEngine.Random.Range(0.15f, 0.58f), UnityEngine.Random.value, Mathf.Pow(UnityEngine.Random.value, 1.5f - Met));
                    float S = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 0.3f + Stealth * 0.3f);
                    bool Dark = (UnityEngine.Random.Range(0f, 1f) <= 0.3f + Stealth * 0.2f);
                    float L = Mathf.Pow(UnityEngine.Random.Range(Dark ? 0.9f : 0.75f, 1f), 1.5f - Stealth);
                    //float EyeColor = Mathf.Pow(UnityEngine.Random.Range(0f, 1f), 2f - Stealth * 1.5f);

                    self.effectColor = RWCustom.Custom.HSL2RGB(H, S, Mathf.Clamp(Dark ? (1f - L) : L, 0.01f, 1f), 1f);
                    //self.eyeColor = Dark? Color.white : Color.black;
                    self.eyeColor = Color.Lerp(self.effectColor, Dark ? Color.white : Color.black, 0.8f);
                }
                else
                {
                    self.effectColor = PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Red);
                    self.eyeColor = new Color(0.57255f, 0.11373f, 0.22745f);
                }
            }
        }

        private void GiveHunterDaddyDummyPupTailSize(On.DaddyGraphics.HunterDummy.orig_ctor orig, DaddyGraphics.HunterDummy self, DaddyGraphics dg, int startSprite)
        {
            orig(self, dg, startSprite);
            if (self.owner.EffectColor != PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Red))
            {
                for (int i = 0; i < self.tail.Length; ++i)
                {
                    self.tail[i].connectionRad /= 2;
                    self.tail[i].affectPrevious /= 2;

                }
            }
        }

        private void GiveHunterDaddyPupPalette(On.DaddyGraphics.orig_ApplyPalette orig, DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.daddy.HDmode)
            {
                self.blackColor = palette.blackColor;
                for (int i = 0; i < self.daddy.bodyChunks.Length; i++)
                {
                    sLeaser.sprites[self.BodySprite(i)].color = self.EffectColor;//Change here
                }
                for (int j = 0; j < self.legGraphics.Length; j++)
                {
                    self.legGraphics[j].ApplyPalette(sLeaser, rCam, palette);
                }
                for (int k = 0; k < self.deadLegs.Length; k++)
                {
                    self.deadLegs[k].ApplyPalette(sLeaser, rCam, palette);
                }
                for (int l = 0; l < self.danglers.Length; l++)
                {
                    self.danglers[l].ApplyPalette(sLeaser, rCam, palette);
                }
                self.dummy.ApplyPalette(sLeaser, rCam, palette);
            }
            else orig(self, sLeaser, rCam, palette);
        }

        private void GiveHunterDaddyDummyPupPalette(On.DaddyGraphics.HunterDummy.orig_ApplyPalette orig, DaddyGraphics.HunterDummy self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);//Change here
            Color blackColor = palette.blackColor;
            for (int i = 0; i < self.numberOfSprites - 1; i++)
            {
                sLeaser.sprites[self.startSprite + i].color = color;
            }
            sLeaser.sprites[self.startSprite + 5].color = blackColor;
        }

        private void GiveHunterDaddyDummyPupSprites(On.DaddyGraphics.HunterDummy.orig_DrawSprites orig, DaddyGraphics.HunterDummy self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);

            if (self.darkenFactor > 0f)
            {
                for (int j = 0; j < self.numberOfSprites; j++)
                {
                    Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);
                    sLeaser.sprites[self.startSprite + j].color = new Color(Mathf.Min(1f, color.r * (1f - self.darkenFactor) + 0.01f), Mathf.Min(1f, color.g * (1f - self.darkenFactor) + 0.01f), Mathf.Min(1f, color.b * (1f - self.darkenFactor) + 0.01f));
                }
            }
        }
        /*
         * OBSOLETE: Part of dll coloration code. Must update.
        private void GiveHunterDaddyDangleTubePupPallete(On.DaddyGraphics.DaddyDangleTube.orig_ApplyPalette orig, DaddyGraphics.DaddyDangleTube self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.owner.daddy.HDmode)
            {
                Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);//Change here
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? self.owner.EffectColor : color);
                        num++;
                    }
                }
            }
            else orig(self, sLeaser, rCam, palette);
        }

        private void GiveHunterDaddyTubePupPallete(On.DaddyGraphics.DaddyTubeGraphic.orig_ApplyPalette orig, DaddyGraphics.DaddyTubeGraphic self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.owner.daddy.HDmode)
            {
                Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);//Change here

                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? self.owner.EffectColor : color);
                        num++;
                    }
                }
            }
            else orig(self, sLeaser, rCam, palette);
        }

        private void GiveHunterDaddyDeadLegPupPallete(On.DaddyGraphics.DaddyDeadLeg.orig_ApplyPalette orig, DaddyGraphics.DaddyDeadLeg self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self.owner.daddy.HDmode)
            {
                Color color = Color.Lerp(self.owner.EffectColor, Color.gray, 0.4f);
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? (self.owner.EffectColor * Mathf.Lerp(0.5f, 0.2f, self.deadness)) : color);
                        num++;
                    }
                }
            }
            else orig(self, sLeaser, rCam, palette);

        }*/

        private void GiveHunterDaddyDummyPupSize(On.DaddyGraphics.HunterDummy.orig_Update orig, DaddyGraphics.HunterDummy self)
        {
            if (self.owner.EffectColor != PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Red))
            {
                Vector2 origPos = self.owner.daddy.bodyChunks[0].pos;
                self.owner.daddy.bodyChunks[0].pos = Vector2.Lerp(self.owner.daddy.bodyChunks[0].pos, self.owner.daddy.bodyChunks[1].pos, 0.95f);
                orig(self);
                self.owner.daddy.bodyChunks[0].pos = origPos;
            }
            else orig(self);
        }
        #endregion

        #region Other fixes
        private void PreventSandGrubPullCrash(On.Watcher.SandGrubNetwork.orig_MarkConsumed orig, SandGrubNetwork self)
        {
            if (SlugcatName != WatcherEnums.SlugcatStatsName.Watcher)
                return;
            else
                orig(self);
        }

        private void ForceBlackMoleSalamander(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (HasBlackLizards && self != null && self.lizard != null && self.lizard.Template.type == new CreatureTemplate.Type("MoleSalamander"))
            {
                self.blackSalamander = true;
            }
        }

        //Apparently, limiting skywhales to deer's room attractions is not enough. Idk why.
        readonly HashSet<string> LF_Skywhale_Blacklist = new HashSet<string>
        {
            "LF_M04",
            "LF_M02",
            "LF_A01",
            "LF_A10",
            "LF_C01",
            "LF_C02",
            "LF_B01",
            "LF_B02",
            "LF_D04",
            "LF_D01",
            "LF_D02",
            "LF_D03",
            "LF_E01",
            "LF_E02",
            "LF_E03",
            "LF_E04",
            "LF_F02",
        };
        private bool SkyWhale_ExtraBlacklist(On.SkyWhaleAbstractAI.orig_CheckBlacklist orig, SkyWhaleAbstractAI self, string room)
        {
            return orig(self, room) || LF_Skywhale_Blacklist.Contains(room);
        }
        #endregion
    }
}
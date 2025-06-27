using BFPlus.Extensions.Maps.NewPowerPlant;
using BFPlus.Extensions.Maps.PitMaps;
using BFPlus.Extensions.Maps.SandCastleDepths;
using BFPlus.Extensions.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using HarmonyLib;
using BFPlus.Extensions.Events.DarkTeamSnakemouthEvents;
using static BFPlus.Extensions.PitData;
using UnityEngine;
using BFPlus.Extensions.Events.PitEvents;
using BFPlus.Extensions.Events.SeedlingMinigameEvents;
using BFPlus.Extensions.Events.NewDungeonsEvents;
using BFPlus.Extensions.Events.NewBossesEvents;
using System.IO.Ports;
using BFPlus.Extensions.Events.GourmetRaceEvents;
using BFPlus.Extensions.Events.HoaxeIntermissionEvents;
using BFPlus.Extensions.Events.NewDungeonsEvents.GiantLairPlayroomEvents;

namespace BFPlus.Extensions.Events
{
    public abstract class NewEvent
    {
        protected bool endEvent = true;
        public IEnumerator StartEvent(NPCControl caller, EventControl instance)
        {
            yield return DoEvent(caller, instance);

            if (endEvent) 
            {
                EventControl.EndEvent();
            }
            yield break;
        }

        protected IEnumerator SetupPlayerHoaxe(Vector3 position, int animid)
        {
            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.following = null;
                MainManager.map.chompy.transform.position = new Vector3(0, -30);
            }

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                UnityEngine.Object.Destroy(MainManager.instance.playerdata[i].entity.gameObject);
            }

            MainManager.ChangeParty(new int[1] { 0 }, true, true);
            MainManager.SetPlayers();
            yield return EventControl.tenthsec;

            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.gameObject.SetActive(true);
            hoaxe.LockRigid(true);
            hoaxe.transform.position = position;
            hoaxe.animid = animid;
        }

        protected void SetupHoaxeFlags()
        {
            MainManager.player.basespeed = 5;
            MainManager.player.canpause = false;
            MainManager.instance.flags[11] = false; //can use beemrang
            MainManager.instance.playerdata[0].animid = (int)NewAnimID.Hoaxe;
            MainManager.instance.playerdata[0].entity.emoticonoffset = new Vector3(0, 2.2f, -0.1f);
        }

        protected abstract IEnumerator DoEvent(NPCControl caller, EventControl instance);

        protected IEnumerator LoadPitRoom(EventControl instance)
        {
            EntityControl[] party = MainManager.GetPartyEntities(true);
            SpriteRenderer dimmer = MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>();
            while (dimmer.color.a < 0.95f)
            {
                yield return null;
            }
            dimmer.color = Color.black;

            MainManager.instance.flagvar[(int)NewFlagVar.Pit_Floor]++;

            int currentFloor = MainManager.instance.flagvar[(int)NewFlagVar.Pit_Floor];

            MainManager.UpdateArea((int)MainManager.Areas.BugariaOutskirts);
            bool marsBudCutscene = currentFloor == 90 && !MainManager.instance.flags[868];

            if (currentFloor == 100)
            {
                MainManager.LoadMap((int)NewMaps.PitBossRoom);
            }
            else
            {
                if ((currentFloor % 10) != 0)
                {
                    MainManager.LoadMap((int)NewMaps.Pit100BaseRoom);
                }
                else
                {
                    MainManager.FadeMusic(0.05f);
                    MainManager.LoadMap((int)NewMaps.Pit100Reward);
                }
            }
            yield return new WaitForSeconds(0.75f);
            if(!marsBudCutscene && ((int)MainManager.map.mapid == (int)NewMaps.Pit100Reward || (int)MainManager.map.mapid == (int)NewMaps.PitBossRoom) || currentFloor == 1)
                MainManager.ChangeMusic();

            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-1.5f, 8f, 6f),
                new Vector3(0f, 8f, 6f),
                new Vector3(1.5f, 8f, 6f)
            };

            if ((int)MainManager.map.mapid == (int)NewMaps.PitBossRoom)
            {
                posArray = new Vector3[]
                {
                    new Vector3(-1.5f, 8.46f, 45f),
                    new Vector3(0f, 8.46f, 45f),
                    new Vector3(1.5f, 8.46f, 45f)
                };
            }

            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = posArray[i];
            }
            MainManager.SetCamera(MainManager.player.transform, null, 1, MainManager.defaultcamoffset, new Vector3(20, 0, 0));
            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = new Vector3(2.5f, 8f, 6f);

                if ((int)MainManager.map.mapid == (int)NewMaps.PitBossRoom)
                {
                    MainManager.map.chompy.transform.position = new Vector3(2.5f, 8.46f, 45f);
                }
            }

            EntityControl lieutenant = MainManager.map.tempfollowers?.FirstOrDefault(f => f?.animid == (int)MainManager.AnimIDs.AntCapitain - 1);

            if (lieutenant != null)
            {
                lieutenant.transform.position = new Vector3(2.5f, 8f, 7f);
            }
            var platform = GameObject.Find("AncientPlatform");

            Vector3 initialPos = platform.transform.position;
            platform.transform.position = initialPos + new Vector3(0, 8);


            bool lieutenantCutscene = lieutenant != null && (NewMaps)MainManager.map.mapid == NewMaps.Pit100Reward && MainManager.HasFollower(MainManager.AnimIDs.AntCapitain) && MainManager.instance.flags[834] && !MainManager.instance.flags[835];
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].entity.transform.parent = platform.transform;
                MainManager.instance.playerdata[i].entity.LockRigid(true);
                MainManager.instance.playerdata[i].entity.ccol.enabled = false;

                if (lieutenantCutscene || marsBudCutscene)
                {
                    MainManager.instance.playerdata[i].entity.animstate = (int)MainManager.Animations.WeakBattleIdle;
                }
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.parent = platform.transform;
            }

            if (lieutenant != null)
            {
                lieutenant.transform.transform.parent = platform.transform;
            }

            AudioClip platformMove = Resources.Load<AudioClip>("Audio/Sounds/PlatformMove");
            MainManager.PlayTransition(1, 0, 0.075f, Color.black);
            float a = 0;
            float b = 240;
            Vector3 platPos = platform.transform.position;
            do
            {
                platform.transform.position = MainManager.SmoothLerp(platPos, initialPos, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a <= b);

            MainManager.StopSound(platformMove);
            yield return null;
            for (int j = 0; j < MainManager.instance.playerdata.Length; j++)
            {
                MainManager.instance.playerdata[j].entity.transform.parent = null;
                MainManager.instance.playerdata[j].entity.LockRigid(false);
                MainManager.instance.playerdata[j].entity.ccol.enabled = true;
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.parent = MainManager.map.transform;
            }

            if (lieutenant != null)
            {
                lieutenant.transform.parent = MainManager.map.transform;
            }

            dimmer = MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>();
            while (dimmer.color.a > 0.1f)
            {
                yield return null;
            }

            if (lieutenantCutscene)
            {
                for (int j = 0; j < MainManager.instance.playerdata.Length; j++)
                {
                    MainManager.instance.playerdata[j].entity.FaceTowards(lieutenant.transform.position);
                }

                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[18], true, Vector3.zero, lieutenant.transform, lieutenant.npcdata));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
            }

            if(marsBudCutscene)
            {
                yield return DoMarsBudEvent(party);

            }
            MainManager.ResetCamera();
        }

        IEnumerator DoMarsBudEvent(EntityControl[] party)
        {
            foreach(var p in party)
            {
                p.FaceTowards(party[1].transform.position);
            }
            MainManager.FadeMusic(0.05f);
            MainManager.SetCamera(party[1].transform.position, 0.045f);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[20], true, Vector3.zero, party[0].transform, party[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            EntityControl bud = MainManager.GetEntity(4);
            MainManager.SetCamera(bud.transform.position, 0.045f);

            bud.flip = false;
            Vector3 ss = bud.startscale;
            bud.spin = new Vector3(0f, 20f, 0f);
            bud.sprite.transform.localScale = Vector3.zero;

            bud.transform.position = new Vector3(5f, 0, 6.7f);
            Vector3 camPos = bud.transform.position + Vector3.left;
            MainManager.SetCamera(camPos, 0.045f);
            MainManager.DeathSmoke(bud.transform.position);
            foreach(var p in party)
            {
                p.animstate = (int)MainManager.Animations.Surprized;
                p.Emoticon(2, 50);
                p.FaceTowards(bud.transform.position);
                p.backsprite = false;
            }

            float a = 0f;
            float b = 35f;
            MainManager.PlaySound("Charge");
            do
            {
                bud.sprite.transform.localScale = Vector3.Lerp(Vector3.zero, ss, a / b);
                a += MainManager.framestep;
                yield return null;
            }
            while (a < b);
            bud.spin = Vector3.zero;

            yield return EventControl.quartersec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[21], true, Vector3.zero, bud.transform, bud.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            bud.animstate = 100;
            foreach (var p in party)
            {
                p.animstate = 13;
            }

            yield return EventControl.sec;
            MainManager.instance.StartCoroutine(BattleControl.StartBattle(new int[] { (int)NewEnemies.MarsBud, (int)NewEnemies.MarsBud }, -1, -1, "Battle6", null, false));
            yield return null;
            while (MainManager.battle != null)
            {
                yield return null;
            }
            MainManager.SetCamera(camPos, 1);
            yield return null;
            bud.animstate = (int)MainManager.Animations.Hurt;
            foreach (var p in party)
            {
                p.animstate = (int)MainManager.Animations.WeakBattleIdle;
            }
            MainManager.FadeOut();
            yield return EventControl.sec;

            a = 0f;
            MainManager.PlaySound("ChargeDown");
            bud.StartCoroutine(bud.ChangeScale(Vector3.zero, b, true));
            do
            {
                a += MainManager.framestep;
                yield return null;
            }
            while (a < b);

            MainManager.DeathSmoke(bud.transform.position);
            bud.transform.position = bud.startpos.Value;
            bud.spin = Vector3.zero;
            MainManager.ResetCamera(false);
            yield return EventControl.sec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[22], true, Vector3.zero, party[0].transform, party[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.instance.flags[868] = true;
            MainManager.ChangeMusic();
        }

        public static Audience CreateAudience(Transform parent, int amount, Audience.Type type, Vector2 constantjump, Vector2 spawnArea, Vector3 pos, Vector3 rotation)
        {
            Audience audience = new GameObject("audience").AddComponent<Audience>();
            audience.ammount = amount;
            audience.animtype = type;
            audience.constantjump = constantjump;
            audience.spawnarea = spawnArea;
            audience.transform.parent = parent;
            audience.transform.position = pos;
            audience.transform.localEulerAngles = rotation;
            return audience;
        }


        protected IEnumerator EndIntermissionPostgame(EventControl instance, int eventToLoad, int loadMap =-1)
        {
            MainManager.instance.flags[916] = false;

            //not intermission 7
            if(eventToLoad != 204)
            {
                MainManager.DestroyPlayers(false, false);
                MainManager.ChangeParty(new int[3] { 0, 1, 2 }, true, true);
                MainManager.SetPlayers();
            }

            if (MainManager.instance.flags[555] && !MainManager.instance.flags[947])
            {
                yield return null;
                MainManager.LoadMap((int)MainManager.Maps.BugariaMainPlaza);
                yield return EventControl.tenthsec;

                //Just keep getting the same random bug where the inside house is not rendered and its all blacked out. 
                //dont really want to lose more time on it so for the time being ill just respawn the player in front of the house.
                //instance.ChangeInside(1, MainManager.GetEntity(4).npcdata);

                yield return EventControl.halfsec;
                //inside house pos
                /*instance.SetPartyPos(new Vector3[]
                {
                    new Vector3(-16.76f, 0.0197f, 7.57f),
                    new Vector3(-14f, 0.0197f, 7.57f),
                    new Vector3(-12.5f, 0.0197f, 7.57f),
                    new Vector3(-12f, 0.0197f, 7.57f)
                });*/

                instance.SetPartyPos(new Vector3[]
                {
                    new Vector3(-14.67f, 0, 3.28f),
                    new Vector3(-13f, 0f, 3.28f),
                    new Vector3(-12.5f, 0f, 3.28f),
                    new Vector3(-12f, 0f, 3.28f)
                });
                yield return EventControl.halfsec;
                MainManager.ResetCamera(true);
                MainManager.FadeOut();
                MainManager.ChangeMusic();
                yield return EventControl.sec;
                endEvent = true;
                yield return null;
            }
            else
            {
                yield return null;
                if(loadMap != -1)
                    MainManager.LoadMap(loadMap);
                yield return EventControl.sec;
                endEvent = false;
                instance.StartEvent(eventToLoad, null);
            }
        }
    }

    public static class EventFactory
    {
        private static readonly Dictionary<NewEvents, Type> EventTypeToClass = new Dictionary<NewEvents, Type>
        {
            { NewEvents.DarkSnekFight, typeof(DarkTeamSnakemouthFight) },
            { NewEvents.DarkSnekZaryant, typeof(DarkTeamSnakemouthZaryant) },
            { NewEvents.MusicPlayerUse, typeof(MusicPlayerEvent) },
            { NewEvents.PitStart, typeof(PitStartEvent) },
            { NewEvents.PitNextFloor, typeof(PitNextFloorEvent) },
            { NewEvents.PitEnemyDead, typeof(PitEnemyDeadEvent) },
            { NewEvents.LeavePit, typeof(LeavePitEvent) },
            { NewEvents.DarkSnekSearch, typeof(DarkTeamSnakemouthSearch) },
            { NewEvents.TangyFight, typeof(TanjyFightEvent) },
            { NewEvents.PitFinalBoss, typeof(PitFinalBossEvent) },
            { NewEvents.StartSeedlingGame, typeof(StartSeedlingGameEvent) },
            { NewEvents.EndSeedlingGame, typeof(EndSeedlingGameEvent) },
            { NewEvents.PowerPlantExtraSwitchDoor, typeof(PowerPlantSwitchDoorEvent) },
            { NewEvents.PowerPlantBoss, typeof(PowerPlantBossEvent) },
            { NewEvents.SandCastleDepthsIcePuzzle, typeof(SandCastleDepthsIcePuzzleEvent) },
            { NewEvents.SandCastleDepthsBoss, typeof(SandCastleDepthsBossEvent) },
            { NewEvents.LeavePitBossRoomEvent, typeof(LeavePitBossRoomEvent) },
            { NewEvents.LeviCeliaFight, typeof(LeviCeliaFightEvent) },
            { NewEvents.DeepCave1VineDoor, typeof(DeepCaveVineDoorEvent) },
            { NewEvents.DeepCaveBoss, typeof(DeepCaveBossEvent) },
            { NewEvents.MoverEvent, typeof(MoverEvent) },
            { NewEvents.StartGourmetMinigame, typeof(StartGourmetRaceEvent) },
            { NewEvents.EndGourmetMinigame, typeof(EndGourmetRaceEvent) },
            { NewEvents.CheckMedal, typeof(CheckMedalEvent) },
            { NewEvents.HBLeifEvent, typeof(LeifFungalLeechEvent) },
            { NewEvents.LeafbugVillageLift, typeof(LeafbugVillageLiftEvent) },
            { NewEvents.ApproachLeafbugShaman, typeof(LeafbugShamanFightEvent) },
            { NewEvents.LeafbugCooking, typeof(LeafbugCookingEvent) },
            { NewEvents.ConflictingVisions, typeof(ConflictingVisionEvent) },
            { NewEvents.AbandonedTower3WindSwitch, typeof(AncientTowerWindSwitchEvent) },
            { NewEvents.IronTowerBoss, typeof(IronTowerBossEvent) },
            { NewEvents.HoaxeIntermission1, typeof(HoaxeIntermission1Event) },
            { NewEvents.HoaxeIntermission1End, typeof(HoaxeIntermission1EndEvent) },
            { NewEvents.PlayroomTrain, typeof(PlayroomTrainEvent) },
            { NewEvents.ToyTruckPlayroom, typeof(ToyTruckEvent) },
            { NewEvents.PlayroomRaceStart, typeof(PlayroomRaceStartEvent) },
            { NewEvents.PlayroomRaceEnd, typeof(PlayroomRaceEndEvent) },
            { NewEvents.PlayroomBoss, typeof(PlayroomBossEvent) },
            { NewEvents.HoaxeIntermission2, typeof(HoaxeIntermission2Event) },
            { NewEvents.HoaxeIntermission3, typeof(HoaxeIntermission3Event) },
            { NewEvents.HoaxeIntermission3End, typeof(HoaxeIntermission3EndEvent) },
            { NewEvents.HerFavoriteSweet, typeof(HerFavoriteSweetEvent) },
            { NewEvents.CollectMagicPouch, typeof(CollectMagicPouchEvent) },
            { NewEvents.HoaxeIntermission4, typeof(HoaxeIntermission4Event) },
            { NewEvents.HoaxeIntermission4End, typeof(HoaxeIntermission4EventEnd) },
            { NewEvents.HoaxeIntermission5, typeof(HoaxeIntermission5Event) },
            { NewEvents.HoaxeIntermission5Talk, typeof(HoaxeIntermission5NpcTalkEvent) },
            { NewEvents.HoaxeIntermission5End, typeof(HoaxeIntermission5EndEvent) },
            { NewEvents.HoaxeIntermission5EatDish, typeof(HoaxeIntermission5EatDishEvent) },
            { NewEvents.HoaxeIntermission6, typeof(HoaxeIntermission6Event) },
            { NewEvents.HoaxeIntermission6MainHall, typeof(HoaxeIntermission6MainHallEvent) },
            { NewEvents.HoaxeIntermission6End, typeof(HoaxeIntermission6EndEvent) },
            { NewEvents.HoaxeIntermission7, typeof(HoaxeIntermission7Event) },
        };

        public static NewEvent GetEventClass(NewEvents eventType)
        {
            if (EventTypeToClass.TryGetValue(eventType, out var eventClassType))
            {
                return (NewEvent)Activator.CreateInstance(eventClassType);
            }
            throw new ArgumentException($"No event class found for event type {eventType}");
        }
    }
}

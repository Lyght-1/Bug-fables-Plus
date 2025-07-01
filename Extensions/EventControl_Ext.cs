using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BFPlus.Extensions.PitData;
using BFPlus.Extensions.EnemyAI;
using static MainManager;

namespace BFPlus.Extensions
{
    public enum NewEvents
    {
        DarkSnekFight=400,MusicPlayerUse=401,PitStart=402,PitNextFloor=403,PitEnemyDead=404,LeavePit=405,DarkSnekZaryant =406, DarkSnekSearch=407,
        TangyFight=408,PitFinalBoss=409,StartSeedlingGame=410, EndSeedlingGame=411, PowerPlantExtraSwitchDoor=412,PowerPlantBoss=413,SandCastleDepthsIcePuzzle=414,
        SandCastleDepthsBoss=415,LeavePitBossRoomEvent=416,LeviCeliaFight=417,DeepCave1VineDoor=418,DeepCaveBoss=419,MoverEvent=420,
        StartGourmetMinigame=421, EndGourmetMinigame = 422, CheckMedal=423, HBLeifEvent=424,LeafbugVillageLift=425,ApproachLeafbugShaman=426,
        LeafbugCooking=427, ConflictingVisions=428, AbandonedTower3WindSwitch, IronTowerBoss, HoaxeIntermission1, HoaxeIntermission1End,
        PlayroomTrain, ToyTruckPlayroom, PlayroomRaceStart, PlayroomRaceEnd, PlayroomBoss, HoaxeIntermission2, HoaxeIntermission3,
        HoaxeIntermission3End,HerFavoriteSweet,CollectMagicPouch, HoaxeIntermission4, HoaxeIntermission4End, HoaxeIntermission5,
        HoaxeIntermission5Talk, HoaxeIntermission5End, HoaxeIntermission5EatDish, HoaxeIntermission6, HoaxeIntermission6MainHall,
        HoaxeIntermission6End, HoaxeIntermission7
    }
    class EventControl_Ext
    {
        public static IEnumerator WaitForPitEnemyDeath(NPCControl caller)
        {
            MainManager.instance.inevent = true;
            yield return null;
            yield return new WaitUntil(() => caller == null || !caller.gameObject.activeSelf || caller.entity.deathcoroutine == null);

            int floor = PitData.GetCurrentFloor();
            if (floor == 99)
            {
                MainManager.instance.flagvar[(int)NewFlagVar.PitEnemyDeadLastFloor]++;
            }

            if (MainManager.instance.flagvar[(int)NewFlagVar.PitEnemyDeadLastFloor] >= 4 || floor != 99)
                MainManager.events.StartEvent((int)NewEvents.PitEnemyDead, caller);
            else
            {
                MainManager.instance.inevent = false;
                MainManager.instance.minipause = false;
            }
        }


        static void CheckDestinyDreamGet()
        {
            if (!MainManager.instance.flags[774])
            {
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[119] + "|break||giveitem,2," + (int)Medal.DestinyDream+",120|", true, Vector3.zero, EventControl.call.transform, EventControl.call));
                MainManager.instance.flags[774] = true;
            }
            else
            {
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[121], true, Vector3.zero, EventControl.call.transform, EventControl.call));
            }
        }

        static IEnumerator TermiteKnightEvent()
        {
            Audience[] audience = MainManager.map.transform.GetChild(1).GetComponentsInChildren<Audience>();
            EntityControl[] party = MainManager.GetPartyEntities(true);
            EntityControl[] entities = MainManager.GetEntities(new int[] { 0, 1, 2, 3, 4 });
            EntityControl[] enemies = MainManager.GetEntities(new int[] { 13,14 });

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[52], true, Vector3.zero, party[2].transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 14f, -12.25f)), 0.025f);

            yield return new WaitForSeconds(0.75f);
            for (int i = 0; i < audience.Length; i++)
            {
                audience[i].Jump();
            }

            MainManager.instance.StartCoroutine(MainManager.SetText("|sound,CrowdCheer||boxstyle,-1||hide||minibubble,18,7||fwait,0.5||minibubble,19,8||fwait,0.5||minibubble,20,9||fwait,0.5||minibubble,17,10||waitminibubble||end|", true, Vector3.zero, null, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 14f, 30.8f)), 1f);
            enemies[0].LockRigid(true);
            enemies[0].transform.position = new Vector3(29.5f, -10f, -2f);

            yield return EventControl.halfsec;
            int dialogueId = MainManager.instance.flags[872] ? 69 : 70;

            if (MainManager.instance.flags[869] && !MainManager.instance.flags[871])
                dialogueId = 60;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueId], true, Vector3.zero, entities[0].transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.PlaySound("CrowdCheer3", -1, 1.1f, 0.8f);
            EntityControl termiteKnight = enemies[1];

            //First time doing the termite knight fight
            if (!MainManager.instance.flags[871])
            {
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 6f, -12.25f)), 0.025f);

                termiteKnight.LockRigid(true);
                termiteKnight.transform.position = new Vector3(29.5f, 30f, -2f);

                Vector3 targetPos = new Vector3(3f, 0f, -1.65f);
                termiteKnight.animstate = (int)MainManager.Animations.Jump;
                Vector3 startPos = termiteKnight.transform.position;
                float a = 0;
                float b = 180;
                do
                {
                    termiteKnight.transform.position = MainManager.BeizierCurve3(startPos, targetPos, 30, a / b);
                    a += MainManager.TieFramerate(1f);
                    yield return null;
                } while (a < b);

                termiteKnight.LockRigid(false);
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 1.5f, -2f)), 0.1f);

                termiteKnight.animstate = (int)MainManager.Animations.KO;
                MainManager.PlaySound("Thud3");
                MainManager.PlayParticle("impactsmoke", termiteKnight.transform.position);
                MainManager.ShakeScreen(0.1f, 0.75f);

                for(int i = 0; i < party.Length; i++)
                {
                    party[i].animstate = (int)MainManager.Animations.Hurt;
                    party[i].Jump();
                }
                yield return EventControl.halfsec;

                for (int i = 0; i < party.Length; i++)
                {
                    party[i].animstate = (int)MainManager.Animations.BattleIdle;
                }

                termiteKnight.animstate = (int)MainManager.Animations.ItemGet;
                MainManager.ChangeMusic("MiteKnight");
                Coroutine flipAround = MainManager.events.StartCoroutine(FlipAround(termiteKnight));

                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 14f, -12.25f)), 0.02f);
                yield return EventControl.sec;

                MainManager.instance.StartCoroutine(MainManager.SetText("|boxstyle,-1||sound,CrowdCheer3||hide||minibubble,61,7||fwait,0.5||minibubble,62,8||fwait,0.5||minibubble,63,9||fwait,0.5||minibubble,64,10||waitminibubble||end|", true, Vector3.zero, null, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 1f, -2f)), 0.02f);
                MainManager.events.StopCoroutine(flipAround);

                termiteKnight.flip = false;
                termiteKnight.animstate = (int)MainManager.Animations.Idle;

                MainManager.instance.StartCoroutine(MainManager.SetText("|define,14,knight|"+MainManager.map.dialogues[66], true, Vector3.zero, party[2].transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                MainManager.events.StartCoroutine(BattleControl.StartBattle(new int[] { (int)NewEnemies.TermiteKnight }, -1, -1, "MiteKnight", null, false));
                yield return EventControl.sec;
                while (MainManager.battle != null)
                {
                    yield return null;
                }

                termiteKnight.animstate = (int)MainManager.Animations.Sleep;

                for (int i = 0; i < party.Length; i++)
                {
                    party[i].flip = true;
                    party[i].animstate = (int)MainManager.Animations.BattleIdle;
                }
                yield return null;
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 1f, -2f)), 1f);
                if (MainManager.instance.flags[671] && !MainManager.battleresult)
                {
                    MainManager.events.StartCoroutine(MainManager.events.ColiseumEnd(false));
                    yield break;
                }
                yield return null;
                MainManager.FadeOut();
                yield return EventControl.sec;
                MainManager.instance.StartCoroutine(MainManager.SetText("|define,14,knight|" + MainManager.map.dialogues[67], true, Vector3.zero, party[2].transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 14f, -12.25f)), 0.025f);
                yield return new WaitForSeconds(0.75f);
                for (int i = 0; i < audience.Length; i++)
                {
                    audience[i].Jump();
                }
                MainManager.instance.StartCoroutine(MainManager.SetText("|sound,CrowdCheer||boxstyle,-1||hide||minibubble,18,7||fwait,0.5||minibubble,19,8||fwait,0.5||minibubble,20,9||fwait,0.5||minibubble,17,10||waitminibubble||end|", true, Vector3.zero, null, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 1f, -2f)), 0.1f);
                yield return EventControl.halfsec;
                MainManager.instance.StartCoroutine(MainManager.SetText("|define,14,knight|" + MainManager.map.dialogues[68], true, Vector3.zero, party[0].transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                List<Coroutine> coroutines = new List<Coroutine>();

                for(int i = 0; i < party.Length; i++)
                {
                    coroutines.Add(MainManager.events.StartCoroutine(FlipAround(party[i])));
                    party[i].animstate = (int)MainManager.Animations.ItemGet;
                }
                coroutines.Add(MainManager.events.StartCoroutine(FlipAround(termiteKnight)));
                termiteKnight.animstate = (int)MainManager.Animations.ItemGet;

                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 10f, -12.25f)), 0.025f);
                yield return new WaitForSeconds(0.75f);
                for (int i = 0; i < audience.Length; i++)
                {
                    audience[i].Jump();
                }
                MainManager.PlaySound("CrowdCheer3", -1, 1.1f, 0.8f);
                yield return EventControl.sec;
                MainManager.FadeIn(0.02f);


                yield return null;
                SpriteRenderer transition = MainManager.GetTransitionSprite();
                while (transition.color.a < 0.95f)
                {
                    yield return null;
                }
                MainManager.StopSound("CrowdChatter");

                for(int i=0;i<coroutines.Count;i++)
                    MainManager.events.StopCoroutine(coroutines[i]);

                yield return new WaitForSeconds(0.75f);
                MainManager.instance.flags[872] = true;
                MainManager.instance.flags[871] = true;

                MainManager.LoadMap(178);
                yield return null;

                termiteKnight = MainManager.GetEntity(14);
                MainManager.ChangeMusic();

                MainManager.events.SetPartyPos(new Vector3[]{
                        new Vector3(-16.45f, 0f, 2.65f),
                        new Vector3(-18f, 0f, 2.65f),
                        new Vector3(-19.55f, 0f, 2.65f),
                        new Vector3(-21f, 0f, 2.25f)
                    },
                    new bool[] { true, true, true, true }, new int[4]);

                MainManager.SetCamera(null, new Vector3?(new Vector3(-16f, 0f, 5f)), 1f);
                yield return null;
                termiteKnight.transform.position = new Vector3(-14.25f, 0f, 3f);
                
                yield return EventControl.halfsec;
                MainManager.FadeOut();
                yield return EventControl.sec;

                MainManager.instance.StartCoroutine(MainManager.SetText("|define,14,knight|"+MainManager.map.dialogues[37], true, Vector3.zero, termiteKnight.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                termiteKnight.gameObject.layer = 9;
                termiteKnight.alwaysactive = true;

                termiteKnight.MoveTowards(-11.2f, 0f, -1.5f);
                while (termiteKnight.forcemove)
                {
                    yield return null;
                }

                yield return EventControl.sec;
                termiteKnight.gameObject.SetActive(false);
                MainManager.CompleteQuest((int)NewQuest.AWorthyOpponent);
                MainManager.AddPrizeMedal((int)NewPrizeFlag.Termite);
                MainManager.ResetCamera();
                EventControl.EndEvent();
                yield break;
            }
            else
            {
                termiteKnight = MainManager.GetEntity(11);
                termiteKnight.animid = (int)NewAnimID.TermiteKnight;
                termiteKnight.CheckSpecialID();

                //termite knight fight but quest is already done
                termiteKnight.transform.position = new Vector3(29.5f, -1f, -2f);
                yield return null;
                MainManager.SetCamera(null, new Vector3?(new Vector3(17f, 1f, -3f)), 1f, MainManager.defaultcamoffset, new Vector3(10f, 10f));
                MainManager.ChangeMusic("MiteKnight");

                termiteKnight.LockRigid(false);
                while (!termiteKnight.onground)
                {
                    yield return null;
                }

                termiteKnight.forcejump = true;
                termiteKnight.MoveTowards(new Vector3(3f, 0f, -1.65f), 1.75f);
                yield return new WaitForSeconds(0.75f);
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 1f, -2f)), 0.025f);
                while (termiteKnight.forcemove)
                {
                    yield return null;
                }

                dialogueId = MainManager.instance.flags[872] ? 71 : 74;
                MainManager.instance.StartCoroutine(MainManager.SetText("|define,11,knight|" + MainManager.map.dialogues[dialogueId], true, Vector3.zero, termiteKnight.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                MainManager.instance.StartCoroutine(MainManager.SetText("|boxstyle,1||bleep,@0||halfline||size,1.5||shaky|" + MainManager.map.dialogues[72], true, Vector3.zero, null, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                MainManager.SaveCameraPosition();
                MainManager.events.StartCoroutine(BattleControl.StartBattle(new int[] { (int)NewEnemies.TermiteKnight }, -1, -1, "MiteKnight", null, false));
                yield return EventControl.sec;
                while (MainManager.battle != null)
                {
                    yield return null;
                }

                termiteKnight.animstate = (int)MainManager.Animations.Sleep;

                for (int i = 0; i < party.Length; i++)
                {
                    party[i].flip = true;
                    party[i].animstate = (int)MainManager.Animations.BattleIdle;
                }
                yield return null;
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 1f, -2f)), 1f);
                if (MainManager.instance.flags[671] && !MainManager.battleresult)
                {
                    MainManager.events.StartCoroutine(MainManager.events.ColiseumEnd(false));
                    yield break;
                }
                yield return null;
                MainManager.FadeOut();
                yield return EventControl.sec;

                dialogueId = MainManager.instance.flags[872] ? 73 : 75;


                MainManager.instance.StartCoroutine(MainManager.SetText("|define,11,knight|" + MainManager.map.dialogues[dialogueId], true, Vector3.zero, termiteKnight.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 14f, -12.25f)), 0.025f);
                yield return new WaitForSeconds(0.75f);
                for (int i = 0; i < audience.Length; i++)
                {
                    audience[i].Jump();
                }
                MainManager.instance.StartCoroutine(MainManager.SetText("|sound,CrowdCheer||boxstyle,-1||hide||minibubble,18,7||fwait,0.5||minibubble,19,8||fwait,0.5||minibubble,20,9||fwait,0.5||minibubble,17,10||waitminibubble||end|", true, Vector3.zero, null, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 14f, 30.8f)), 1f);
                yield return EventControl.halfsec;
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[53], true, Vector3.zero, entities[0].transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                MainManager.FadeIn();
                yield return EventControl.sec;
                MainManager.events.StartCoroutine(MainManager.events.ColiseumEnd(MainManager.battleresult));
                yield break;
            }
        }

        static IEnumerator PattonsQuestEvent()
        {
            yield return null;

            EntityControl patton = EventControl.call.entity;
            EntityControl[] party = MainManager.GetPartyEntities(true);
            if (!MainManager.instance.flags[935])
            {
                MainManager.DialogueText(MainManager.map.dialogues[24], EventControl.call.transform, EventControl.call);
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                MainManager.instance.flags[935] = true;
            }
            else
            {
                int[] requiredItems = new int[] { (int)MainManager.Items.LonglegSummoner, (int)NewItem.WebWad, (int)MainManager.Items.Abomihoney };
                bool gotAll = true;
                for (int i = 0; i < requiredItems.Length; i++)
                {
                    if (!MainManager.instance.items[0].Contains(requiredItems[i]))
                    {
                        gotAll = false;
                    }
                }

                if (gotAll || MainManager.instance.flags[936]) 
                {
                    Vector3 targetPos = new Vector3(5.21f, 0, 6.37f);
                    Vector3 basePos = patton.transform.position;

                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        Physics.IgnoreCollision(patton.ccol, MainManager.instance.playerdata[i].entity.ccol, true);
                    }

                    if (!MainManager.instance.flags[936])
                    {
                        MainManager.DialogueText(MainManager.map.dialogues[26], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }
                        MainManager.instance.flags[936] = true;

                        patton.MoveTowards(targetPos,2f);
                        yield return new WaitUntil(() => !patton.forcemove);

                        for (int i = 0; i < requiredItems.Length; i++)
                        {
                            MainManager.instance.items[0].Remove(requiredItems[i]);
                        }
                        CreatePattonsIngredients();
                        MainManager.PlaySound("WaterSplash2");
                        MainManager.PlaySound("Click2",1.2f,1);

                        patton.MoveTowards(basePos, 2f);
                        yield return new WaitUntil(() => !patton.forcemove);

                        patton.FacePlayer();
                        MainManager.DialogueText(MainManager.map.dialogues[27], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }

                    }
                    else
                    {
                        MainManager.DialogueText(MainManager.map.dialogues[29], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }
                    }


                    if(MainManager.instance.option == 0 && MainManager.instance.money >= 30)
                    {
                        MainManager.FadeMusic(0.1f);
                        MainManager.instance.money -= 30;
                        MainManager.DialogueText(MainManager.map.dialogues[30], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }

                        patton.MoveTowards(targetPos,2);
                        yield return new WaitUntil(() => !patton.forcemove);

                        int[] berriesId = { (int)MainManager.Items.MoneyBig, (int)MainManager.Items.MoneyMedium, (int)MainManager.Items.MoneySmall };
                        SpriteRenderer[] berries = new SpriteRenderer[berriesId.Length];
                        Vector3 berryPos = new Vector3(2.5f, 3f, 6f);

                        for (int i = 0; i < berriesId.Length; i++)
                        {
                            Vector3 positionOffset = new Vector3(i % 2 == 0 ? -0.5f : 0.5f, -1f * i, i % 2 == 0 ? 0f : 0.3f);
                            berries[i] = MainManager.NewSpriteObject(berryPos + positionOffset, MainManager.map.transform, MainManager.itemsprites[0, berriesId[i]]);
                            berries[i].name = "ingredient";
                        }

                        MainManager.PlaySound("WaterSplash2");
                        MainManager.PlaySound("Click2", 1.2f, 1);

                        MainManager.DialogueText(MainManager.map.dialogues[31], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }
                        MainManager.PlaySound("Click2");
                        yield return EventControl.tenthsec;
                        patton.MoveTowards(new Vector3(14.7f, 0, 7.5f), 3f, 1, 0, true);

                        yield return EventControl.halfsec;
                        MainManager.PlaySound("Bubbling");

                        Transform tube = MainManager.map.transform.Find("PattonsHouse").Find("Tube (2)").GetChild(0);
                        MainManager.instance.StartCoroutine(MainManager.ShakeObject(tube.parent, new Vector3(0.2f, 0.2f), 120, true));
                        GameObject smokePart =MainManager.PlayParticle("ContinuousSmokeCloud", tube.transform.position,-1);
                        smokePart.transform.localScale = Vector3.one * 3;

                        yield return EventControl.halfsec;

                        GameObject smokeSphere = MainManager.PlayParticle("OpaqueSmokeSphere", tube.transform.position + new Vector3(1,2,-1), -1);
                        smokeSphere.transform.localScale = Vector3.one * 0.5f;

                        foreach (var p in party)
                            p.animstate = (int)MainManager.Animations.Surprized;
                        yield return EventControl.sec;

                        //[Patton would then nudge the pod and you'd hear a click noise as it turns on. The pod would shake a bit,
                        //wilst making a wirring sound, as smoke and pressure builds up inside the pod. it then shatters with an explosion
                        //and a Long Legs Spider and an Abombiberry burst out of the pod]
                        yield return new WaitUntil(() => !patton.forcemove);
                        patton.FacePlayer();

                        MainManager.SetCamera(null, new Vector3(9.43f, 1.17f, 4.65f), 0.1f, MainManager.defaultcamoffset, new Vector3(10, -15));

                        MainManager.PlaySound("WizardPot", 1.1f, 1);
                        yield return EventControl.halfsec;
                        MainManager.PlaySound("PotBreak", 1.2f, 1);
                        MainManager.ShakeScreen(0.05f, 0.5f);

                        foreach (Transform child in MainManager.map.transform)
                        {
                            if (child.name == "ingredient")
                                UnityEngine.Object.Destroy(child.gameObject);
                        }

                        GameObject tubeMesh = Resources.Load<GameObject>("prefabs/maps/UpperSnekMiddleRoom");
                        tube.gameObject.GetComponent<MeshFilter>().mesh = tubeMesh.transform.GetChild(0).Find("Tube (4)").GetChild(0).GetComponent<MeshFilter>().mesh;
                        EntityControl[] enemies = new EntityControl[2];
                        for(int i=0;i<enemies.Length; i++)
                        {
                            int animId = i == 0 ?(int)MainManager.AnimIDs.Abomihoney : (int)MainManager.AnimIDs.LongLegs;
                            enemies[i] = EntityControl.CreateNewEntity(i==0 ? NewEnemies.Abomiberry.ToString() : "spider", animId - 1, tube.transform.position);
                            enemies[i].transform.parent = MainManager.map.transform;

                            yield return null;
                            enemies[i].LockRigid(true);

                            if (i == 1)
                            {
                                enemies[i].startscale = Vector3.one * 0.7f;
                            }

                            enemies[i].gameObject.layer = 9;
                            enemies[i].StartCoroutine(MainManager.ArcMovement(enemies[i].gameObject, enemies[i].transform.position, patton.transform.position - new Vector3(2 + 2.5f *(1-i),0, i== 0 ? 1 : 0.5f),new Vector3(0,20,0),5,30,false)); 
                        }

                        UnityEngine.Object.Destroy(smokePart);
                        UnityEngine.Object.Destroy(smokeSphere);

                        MainManager.ChangeMusic("Tension");

                        foreach (var p in party)
                            p.animstate = (int)MainManager.Animations.BattleIdle;
                        yield return EventControl.sec;

                        MainManager.DialogueText(MainManager.map.dialogues[32], party[1].transform, party[1].npcdata);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }

                        patton.animstate = 101;
                        yield return EventControl.sec;
                        patton.animstate = 103;

                        MainManager.DialogueText(MainManager.map.dialogues[33], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }

                        MainManager.FadeMusic(1);
                        yield return EventControl.quartersec;

                        instance.StartCoroutine(BattleControl.StartBattle(new int[] { 
                            (int)NewEnemies.Abomiberry,(int)NewEnemies.LonglegsSpider,(int)NewEnemies.Patton }, 
                            -1, -1, NewMusic.PlusBosses.ToString(), null, false));
                        yield return EventControl.sec;
                        while (MainManager.battle != null)
                        {
                            yield return null;
                        }

                        NPCControl door = MainManager.GetEntity(2).npcdata;
                        MainManager.events.ChangeInside(0, door);
                        MainManager.SetCamera(null, new Vector3(9.43f, 1.17f, 4.65f), 1f, MainManager.defaultcamoffset, new Vector3(10, -15));

                        door.vectordata[4] = MainManager.defaultcamoffset;
                        door.vectordata[5] = MainManager.defaultcamangle;

                        MainManager.map.tcneg = MainManager.map.originallimitneg;
                        MainManager.map.tcpos = MainManager.map.originallimitpos;

                        patton.animstate = (int)MainManager.Animations.KO;
                        patton.spritetransform.localEulerAngles = new Vector3(0, 0, -90);
                        patton.LockRigid(true);
                        basePos = patton.spritetransform.position; 
                        patton.spritetransform.localPosition += new Vector3(-1, 0.5f);

                        yield return EventControl.halfsec;

                        foreach(var enemy in enemies)
                        {
                            enemy.animstate = (int)MainManager.Animations.Hurt;
                            enemy.destroytype = NPCControl.DeathType.SpinSmoke;
                            enemy.StartCoroutine(enemy.Death());
                        }

                        foreach (var p in party)
                            p.animstate = (int)MainManager.Animations.WeakBattleIdle;

                        MainManager.FadeOut();
                        yield return EventControl.sec;

                        MainManager.DialogueText(MainManager.map.dialogues[34], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }
                        patton.LockRigid(false);
                        patton.spritetransform.localEulerAngles = Vector3.zero;
                        patton.spritetransform.position = basePos;
                        patton.animstate = 0;
                        patton.PlaySound("Jump");
                        patton.Jump(10);
                        yield return null;
                        yield return new WaitUntil(() => patton.onground);

                        MainManager.DialogueText(MainManager.map.dialogues[35], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }

                        MainManager.AddPrizeMedal((int)NewPrizeFlag.Patton);
                        MainManager.CompleteQuest((int)NewQuest.MyNewestExperiment);
                        MainManager.instance.flags[934] = true;
                        MainManager.ChangeMusic();
                    }
                    else
                    {
                        //refused to give berries
                        MainManager.DialogueText(MainManager.map.dialogues[28], EventControl.call.transform, EventControl.call);
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }
                    }
                }
                else
                {
                    MainManager.DialogueText(MainManager.map.dialogues[25], EventControl.call.transform, EventControl.call);
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }
                }
            }

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                Physics.IgnoreCollision(patton.ccol, MainManager.instance.playerdata[i].entity.ccol, false);
            }
            MainManager.LoadCameraPosition();
            yield return null;
            EventControl.EndEvent();
        }

        public static void CreatePattonsIngredients()
        {
            int[] requiredItems = new int[] { (int)MainManager.Items.LonglegSummoner, (int)NewItem.WebWad, (int)MainManager.Items.Abomihoney };

            Vector3 itemPos = new Vector3(2.23f, 3f, 6.5f);
            SpriteRenderer[] items = new SpriteRenderer[requiredItems.Length];
            for (int i = 0; i < requiredItems.Length; i++)
            {
                Vector3 positionOffset = new Vector3(i % 2 == 0 ? 0.5f : 0, -1 * i, i % 2 == 0 ? 0 : -0.5f);
                items[i] = MainManager.NewSpriteObject(itemPos + positionOffset, MainManager.map.transform, MainManager.itemsprites[0, requiredItems[i]]);
                items[i].name = "ingredient";
            }
        }

        public static IEnumerator FlipAround(EntityControl entity)
        {
            do
            {
                entity.flip = !entity.flip;
                yield return EventControl.sec;
            } while (true);
        }

        public static void CheckEventBattleMusic()
        {

        }
    }
}

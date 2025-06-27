using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.HoaxeIntermissionEvents
{
    public class HoaxeIntermission5Event : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            if (MainManager.instance.flags[555])
                MainManager.FadeIn();
            MainManager.FadeMusic(0.02f);
            yield return new WaitForSeconds(2);

            MainManager.instance.flags[916] = true;

            int[] resetFlags = { 939, 940, 941, 943, 944 };

            foreach (int flag in resetFlags)
                MainManager.instance.flags[flag] = false;
            MainManager.instance.flagvar[(int)NewFlagVar.Intermission5RichNPCTalked] = 0;

            MainManager.instance.insideid = -1;

            MainManager.LoadMap((int)MainManager.Maps.BugariaPier);
            yield return null;
            yield return null;

            yield return SetupPlayerHoaxe(new Vector3(-20f, 0, 1.67f), (int)NewAnimID.Hoaxe);
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.animstate = 106;
            hoaxe.emoticonoffset = new Vector3(0, 2.2f, -0.1f);
            MainManager.SetCamera(hoaxe.transform.position + Vector3.down, 1);
            MainManager.FadeOut(0.01f);
            hoaxe.LockRigid(false);

            hoaxe.MoveTowards(new Vector3(-15f, 0, 1.67f));
            yield return new WaitForSeconds(2);

            yield return new WaitUntil(() => !hoaxe.forcemove);
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[71], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            //lz collision
            MainManager.map.entities[8].GetComponent<BoxCollider>().isTrigger = false;

            SetupHoaxeFlags();
            MainManager.ResetCamera();
            MainManager.ChangeMusic();
            yield return EventControl.tenthsec;
        }
    }

    public class HoaxeIntermission5NpcTalkEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            yield return new WaitUntil(() => !MainManager.instance.message);
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            EntityControl[] richEntities = MainManager.GetEntities(new int[] { 29, 30, 31 });
            if (MainManager.instance.insideid != 0)
            {
                //talking to rich npcs outside
                int dialogueId = 0;
                switch (caller.entity.animid)
                {
                    case (int)MainManager.AnimIDs.DragonflyLady - 1:
                        dialogueId = 80;
                        break;

                    case (int)MainManager.AnimIDs.TeaMoth - 1:
                        dialogueId = 85;
                        break;

                    case (int)MainManager.AnimIDs.RichKid - 1:
                        dialogueId = 87;
                        break;
                }

                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueId], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                List<Vector3> positions = new List<Vector3> { new Vector3(12.57f, 1.3f, -1.26f), new Vector3(12.78f, 2f, 4f) };
                switch (caller.entity.animid)
                {
                    case (int)MainManager.AnimIDs.DragonflyLady - 1:
                        positions.Add(new Vector3(18.20f, 2.0487f, 6.66f));
                        break;

                    case (int)MainManager.AnimIDs.TeaMoth - 1:
                        positions = new List<Vector3>() { new Vector3(7.91f, 0, -4.12f) };
                        break;

                    case (int)MainManager.AnimIDs.RichKid - 1:
                        positions = new List<Vector3>() { new Vector3(4.27f, 0, 2.24f), new Vector3(5.28f, 0, -4f) };
                        break;
                }
                caller.entity.alwaysactive = true;

                caller.StartCoroutine(caller.entity.TempIgnoreColision(hoaxe.ccol, 60));

                for(int i=0;i<positions.Count; i++)
                {
                    caller.entity.forcejump = i != positions.Count-1;
                    caller.entity.MoveTowards(positions[i], 1.5f);
                    yield return new WaitUntil(() => !caller.entity.forcemove);
                }
                yield return null;

                switch (caller.entity.animid)
                {
                    case (int)MainManager.AnimIDs.TeaMoth - 1:
                        caller.entity.SetPosition(new Vector3(20.10f, 2.0487f, 6.63f));
                        break;

                    case (int)MainManager.AnimIDs.RichKid - 1:
                        caller.entity.SetPosition(new Vector3(16.79f, 2.0487f, 6.36f));
                        break;
                }

                caller.insideid = 0;
                caller.dialogues = new Vector3[] { new Vector3(-1, 79, 0) };
                caller.behaviors = new NPCControl.ActionBehaviors[] { NPCControl.ActionBehaviors.TurnRandomly, NPCControl.ActionBehaviors.FacePlayer };
                
                MainManager.instance.flagvar[(int)NewFlagVar.Intermission5RichNPCTalked]++;
                if(MainManager.instance.flagvar[(int)NewFlagVar.Intermission5RichNPCTalked] == 3)
                {
                    MainManager.instance.flags[940] = true;
                }
            }
            else
            {
                foreach (var entity in richEntities)
                    entity.FacePlayer();

                if (MainManager.instance.flags[940])
                {
                    //talked to all rich npcs

                    MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[89], true, Vector3.zero, richEntities[0].transform, richEntities[0].npcdata));
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }

                    Vector3[] positions = new Vector3[] { new Vector3(28.80f, 2.0487f, 5.329f), new Vector3(27.20f, 2.0487f, 5.78f), new Vector3(26.32f, 2.0487f, 5f) };

                    MainManager.SetCamera(new Vector3(28.65f, 2, 5f),MainManager.defaultcamangle, new Vector3(0,2.25f,-8.25f), 0.1f);
                    MainManager.instance.insideid = -1;
                    for(int i=0;i<richEntities.Length;i++)
                    {
                        richEntities[i].npcdata.insideid = -1;
                        richEntities[i].gameObject.layer = 9;
                        richEntities[i].MoveTowards(positions[i]);
                    }
                    MainManager.map.RefreshInsides(false, null);
                    hoaxe.MoveTowards(positions[2] + Vector3.left * 4, 0.25f);
                    yield return null;
                    yield return new WaitUntil(() => MainManager.EntitiesAreNotMoving(richEntities));

                    EntityControl captain = MainManager.GetEntity(28);
                    captain.flip = false;

                    MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[88], true, Vector3.zero, richEntities[0].transform, richEntities[0].npcdata));
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }

                    MainManager.PlayTransition(4, 9999, 0.08f, Color.black);
                    while (MainManager.transition != null)
                    {
                        yield return null;
                    }

                    MainManager.SetCamera(null, new Vector3?(new Vector3(36f, 0f, -0.4f)), 1f);
                    Transform[] boat = new Transform[3];
                    boat[0] = MainManager.map.mainmesh.GetChild(4);
                    boat[0].GetComponent<StaticModelAnim>().enabled = false;
                    boat[0].transform.localPosition = new Vector3(38f, 0.4f, -2.7f);

                    boat[1] = boat[0].GetChild(0);
                    boat[2] = boat[0].GetChild(1);

                    List<EntityControl> boatEntities = new List<EntityControl>(richEntities);
                    boatEntities.AddRange(new EntityControl[] { captain, hoaxe });

                    positions = new Vector3[] { new Vector3(5.05f, 0f, 1.75f), new Vector3(2.95f, -1.3f, 1.88f), 
                        new Vector3(3.57f, 0.36f, 1.75f), new Vector3(-3.15f, -0.6f, 1.8f), new Vector3(0, 2.1316f, 1.7073f) };
                    for(int i=0;i<boatEntities.Count;i++)
                    {
                        boatEntities[i].LockRigid(true);
                        boatEntities[i].flip = true;
                        boatEntities[i].transform.parent = boat[0];
                        boatEntities[i].transform.localPosition = positions[i];
                    }

                    hoaxe.animstate = (int)MainManager.Animations.Sit;

                    captain.animstate = 100;
                    yield return EventControl.halfsec;
                    MainManager.PlayTransition(5, 9999, 0.08f, Color.black);
                    yield return new WaitUntil(() => MainManager.transition == null);

                    yield return EventControl.quartersec;
                    EventControl.call.entity.animstate = 101;
                    MainManager.PlaySound("Boat0");
                    yield return EventControl.halfsec;
                    MainManager.FadeMusic(0.05f);

                    instance.StartCoroutine(BattleControl_Ext.LerpPosition(100, boat[0].position, boat[0].position + Vector3.right * 20, boat[0]));

                    boat[1].gameObject.AddComponent<SpinAround>().itself = new Vector3(10f, 0f, 0f);
                    boat[2].gameObject.AddComponent<SpinAround>().itself = new Vector3(0f, 0f, -10f);

                    yield return new WaitForSeconds(1.5f);
                    MainManager.PlayTransition(4, 0, 0.08f, Color.black);

                    SpriteRenderer transitionSprite = MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>();
                    while (transitionSprite.color.a < 0.95f)
                    {
                        yield return null;
                    }

                    yield return EventControl.sec;
                    hoaxe.transform.parent = null;
                    yield return null;
                    MainManager.LoadMap((int)MainManager.Maps.MetalIsland1);
                    yield return EventControl.halfsec;

                    MainManager.SetCamera(null, new Vector3?(new Vector3(-20f, -1f, -40f)), 1f);
                    boat[0] = MainManager.map.mainmesh.GetChild(2);
                    boat[0].transform.localPosition = new Vector3(-43.7f, 36f, -2.7f);
                    boat[0].transform.localEulerAngles = Vector3.zero;

                    StaticModelAnim staticModelAnim = boat[0].GetComponent<StaticModelAnim>();
                    staticModelAnim.enabled = false;

                    boat[1] = boat[0].GetChild(0);
                    boat[2] = boat[0].GetChild(1);

                    SpinAround[] spins = { boat[1].gameObject.AddComponent<SpinAround>(), boat[2].gameObject.AddComponent<SpinAround>() };
                    spins[0].itself = new Vector3(10f, 0f, 0f);
                    spins[1].itself = new Vector3(0f, 0f, -10f);

                    hoaxe = MainManager.instance.playerdata[0].entity;
                    hoaxe.animstate = (int)MainManager.Animations.Sit;

                    boatEntities = new List<EntityControl>(MainManager.GetEntities(new int[] { 41, 42, 43, 40 }));
                    boatEntities.Add(hoaxe);

                    for (int i = 0; i < boatEntities.Count; i++)
                    {
                        boatEntities[i].LockRigid(true);
                        boatEntities[i].flip = true;
                        boatEntities[i].transform.parent = boat[0];
                        boatEntities[i].transform.localPosition = positions[i];
                    }
                    yield return EventControl.halfsec;

                    MainManager.PlaySound("Boat1");
                    MainManager.ChangeMusic();

                    MainManager.PlayTransition(5, 0, 0.08f, Color.black);
                    yield return BattleControl_Ext.LerpPosition(200, boat[0].position, boat[0].position + Vector3.right * 20, boat[0]);
                    yield return new WaitUntil(() => MainManager.transition == null);

                    foreach (var spin in spins)
                    {
                        UnityEngine.Object.Destroy(spin);
                    }

                    MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[125], true, Vector3.zero, boatEntities[3].transform, boatEntities[3].npcdata));
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }

                    yield return EventControl.quartersec;

                    for(int i = 0; i < boatEntities.Count-2; i++)
                    {
                        instance.StartCoroutine(GetOffBoat(boatEntities[i]));
                        yield return EventControl.tenthsec;
                    }

                    hoaxe.animstate = 108;
                    yield return EventControl.sec;
                    hoaxe.trail = true;
                    hoaxe.PlaySound("Flee");
                    hoaxe.animstate = 101;
                    instance.StartCoroutine(BattleControl_Ext.LerpPosition(80, hoaxe.transform.position, new Vector3(-0.33f, -0.57f, -37.44f), hoaxe.transform));
                    
                    MainManager.PlayTransition(4, 9999, 0.08f, Color.black);
                    yield return new WaitUntil(() => MainManager.transition == null);

                    yield return EventControl.halfsec;
                    yield return EventControl.sec;

                    MainManager.LoadMap();
                    yield return null;

                    foreach (var entity in MainManager.map.entities)
                    {
                        if (entity != null && entity.npcdata.interacttype == NPCControl.Interaction.Shop)
                        {
                            UnityEngine.Object.Destroy(entity.gameObject);
                        }
                    }

                    hoaxe.transform.position = new Vector3(-0.33f, -0.5781f, -37.44f);
                    hoaxe.animstate = (int)MainManager.Animations.Idle;
                    hoaxe.trail = false;
                    MainManager.ResetCamera(true);
                    yield return EventControl.halfsec;

                    MainManager.PlayTransition(5, 0, 0.08f, Color.black);
                    yield return new WaitUntil(() => MainManager.transition == null);

                    MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[126], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }
                }
                else
                {
                    //talking to rich npcs inside the house without having them all
                    if (MainManager.instance.flagvar[(int)NewFlagVar.Intermission5RichNPCTalked] == 1)
                    {
                        MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[81], true, Vector3.zero, caller.transform, caller));
                        while (MainManager.instance.message)
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        int dialogueId = 82;
                        EntityControl[] insideEntities = richEntities.Where(e => e.npcdata.insideid == 0).ToArray();
                        for (int i = 0; i < 2; i++)
                        {
                            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueId], true, Vector3.zero, insideEntities[i].transform, insideEntities[i].npcdata));
                            while (MainManager.instance.message)
                            {
                                yield return null;
                            }
                            dialogueId++;
                        }
                    }
                }
            }
        }

        IEnumerator GetOffBoat(EntityControl entity)
        {
            entity.animstate = 0;
            entity.PlaySound("Jump");
            yield return MainManager.ArcMovement(entity.gameObject, entity.transform.position,new Vector3(-15.21f, -0.5781f, -37.95f),Vector3.zero, 5, 30,false);
            entity.animstate = (int)MainManager.Animations.Walk;
            yield return BattleControl_Ext.LerpPosition(120, entity.transform.position, entity.transform.position + Vector3.right * 10, entity.transform);
        }
    }

    public class HoaxeIntermission5EndEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }
            EntityControl sailor = MainManager.GetEntity(22);
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;

            //holding bag anim
            hoaxe.animstate = (int)MainManager.Animations.Happy;

            MainManager.ChangeMusic("Inside1");

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[121], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            Sprite bagSprite = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("Hoaxe")[65];
            SpriteRenderer bag = MainManager.NewSpriteObject(sailor.transform.position + new Vector3(1.2f, 0, 0.1f), MainManager.map.transform, bagSprite);
            hoaxe.animstate = (int)MainManager.Animations.Idle;

            yield return EventControl.tenthsec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[122], true, Vector3.zero, sailor.transform, sailor.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.FadeMusic(0.1f);

            //Cant use artifactget cause it does a fadein at the end that we dont want :(
            Transform crown = MainManager.map.transform.Find("crown");
            SpriteRenderer backSprite = MainManager.NewSpriteObject("itemback", hoaxe.transform.position + Vector3.forward * 0.25f + Vector3.up * 2.75f, Vector3.zero, null, MainManager.guisprites[85], MainManager.spritedefaultunity);
            backSprite.color = new Color(0.75f, 0.75f, 0.75f);
            backSprite.gameObject.AddComponent<DialogueAnim>().SetUp(Vector3.zero, Vector3.one * 2f, Vector3.zero, 0f);
            crown.position = hoaxe.transform.position + Vector3.back * 0.05f + Vector3.up * 3f;

            GameObject temp = Resources.Load("prefabs/maps/SnakemouthTreasureRoom") as GameObject;
            temp.transform.Find("artifacts_0").GetChild(0);

            GameObject aura = UnityEngine.Object.Instantiate(temp.transform.Find("artifacts_0").GetChild(0).gameObject);
            aura.transform.parent = crown;
            aura.transform.localPosition = new Vector3(0, -0.2f, 0);
            hoaxe.animstate = (int)MainManager.Animations.ItemGet;

            MainManager.PlaySound("ItemGetS");
            yield return new WaitForSeconds(2f);
            MainManager.instance.StartCoroutine(MainManager.SetText("|boxstyle,5||noskip||spd,0.125||lockbacktrack||center|" + MainManager.map.dialogues[124] + "|fwait,0.75|", true, Vector3.zero, null, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            UnityEngine.Object.Destroy(backSprite.gameObject);
            UnityEngine.Object.Destroy(crown.gameObject);

            hoaxe.animstate = (int)MainManager.Animations.Idle;
            hoaxe.Emoticon(MainManager.Emoticons.DotsLong, 120);
            yield return new WaitForSeconds(2);


            hoaxe.animstate = (int)MainManager.Animations.Upset;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[123], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            sailor.Emoticon(MainManager.Emoticons.QuestionMark, 60);

            MainManager.FadeIn(0.05f);
            yield return new WaitForSeconds(2);

            MainManager.instance.playerdata[0].animid = 0;
            MainManager.instance.flags[11] = true;
            MainManager.player.basespeed = 5;
            MainManager.player.canpause = true;

            MainManager.instance.flags[942] = true;
            yield return EndIntermissionPostgame(instance, 148, (int)MainManager.Maps.FGOutsideSwamplands);
        }

    }

    public class HoaxeIntermission5EatDishEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.flip = true;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[128], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            GameObject dish = MainManager.map.transform.Find("DangerDish").gameObject;
            hoaxe.animstate = (int)MainManager.Animations.WeakBattleIdle;
            dish.transform.position = hoaxe.transform.position + new Vector3(0.5f, 1.6f, -0.1f);

            MainManager.PlaySound("Eat", -1, 0.9f, 1, true);
            yield return new WaitForSeconds(3);
            MainManager.StopSound("Eat");
            dish.SetActive(false);
            MainManager.PlayParticle("Mistake", hoaxe.transform.position + Vector3.up);
            hoaxe.PlaySound("Fail");

            caller.transform.position = new Vector3(0,-1000);
            caller.entity.startpos = new Vector3(0,-1000);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[129], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.emoticonoffset = new Vector3(0, 2.2f, -0.1f);
        }
    }
}

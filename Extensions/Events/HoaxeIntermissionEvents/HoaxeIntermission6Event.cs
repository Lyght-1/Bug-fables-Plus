using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.HoaxeIntermissionEvents
{
    public class HoaxeIntermission6Event : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            if (MainManager.instance.flags[555])
                MainManager.FadeIn();
            MainManager.FadeMusic(0.02f);

            yield return new WaitForSeconds(2);
            MainManager.ChangeMusic("Wind");
            MainManager.instance.flags[916] = true;
            MainManager.instance.flags[945] = true;
            MainManager.instance.insideid = -1;

            MainManager.LoadMap((int)MainManager.Maps.WaspKingdomOutside);
            yield return null;
            yield return null;

            yield return SetupPlayerHoaxe(new Vector3(0,0, -8), (int)NewAnimID.Hoaxe);
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.animstate = (int)MainManager.Animations.Upset;
            hoaxe.emoticonoffset = new Vector3(0, 2.2f, -0.1f);

            MainManager.SetCamera(hoaxe.transform.position + Vector3.down, 1);
            MainManager.FadeOut(0.01f);
            hoaxe.LockRigid(false);

            yield return EventControl.sec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[14], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.spin = new Vector3(0, 20);
            yield return EventControl.halfsec;
            hoaxe.spin = Vector3.zero;
            hoaxe.animid = (int)NewAnimID.HoaxeCrown;
            hoaxe.animstate = (int)MainManager.Animations.Idle;
            yield return EventControl.halfsec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[15], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            SetupHoaxeFlags();
            MainManager.instance.playerdata[0].animid = (int)NewAnimID.HoaxeCrown;

            MainManager.ResetCamera();
            yield return EventControl.tenthsec;
        }
    }

    public class HoaxeIntermission6MainHallEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            EntityControl[] entities = MainManager.GetEntities(new int[] { 28, 29, 30 });
            EntityControl hoaxe = MainManager.player.entity;

            MainManager.ChangeMusic("Tension3");

            hoaxe.MoveTowards(new Vector3(0.19f, 0, -6),1,(int)MainManager.Animations.Walk, (int)MainManager.Animations.Idle);
            yield return new WaitUntil(() => !hoaxe.forcemove);
            MainManager.SetCamera(null,MainManager.player.transform.position,0.1f, new Vector3(0,2.5f,-6));

            hoaxe.backsprite = false;

            foreach (var e in entities)
            {
                e.FaceTowards(hoaxe.transform.position);
                e.alwaysactive = true;
                e.LockRigid(false);
            }

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[31], true, Vector3.zero, entities[0].transform, entities[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.SetCamera(hoaxe.transform.position, MainManager.defaultcamangle, new Vector3(0, 4f, -12f), 0.1f);
            hoaxe.animstate = 100;
            MainManager.PlaySound("Lazer2", 0.8f);
            foreach (var entity in MainManager.map.entities)
            {
                if (entity != null && !entity.iskill && entity.npcdata != null && entity.npcdata.entitytype == NPCControl.NPCType.NPC)
                {
                    if(entity.animid != (int)MainManager.AnimIDs.WaspBoyfriend-1 && entity.animid != (int)MainManager.AnimIDs.TraitorWasp-1 && entity.animid != (int)MainManager.AnimIDs.Jayde - 1)
                    {
                        entity.animstate = (int)MainManager.Animations.Hurt;
                        instance.StartCoroutine(entity.ShakeSprite(0.2f, 120));
                    }
                    MainManager.PlayParticle("HoaxeDiamond", entity.transform.position + new Vector3(0,2f,-0.1f), 4);
                }
            }
            //MainManager.ChangeMusic("Tension3");
            yield return EventControl.sec;
            yield return null;
            hoaxe.animstate = (int)MainManager.Animations.Idle;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[32], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            foreach (var entity in MainManager.map.entities)
            {
                if (entity != null && !entity.iskill && entity.npcdata != null && entity.npcdata.entitytype == NPCControl.NPCType.NPC)
                {
                    if (entity.animid != (int)MainManager.AnimIDs.WaspBoyfriend - 1 && entity.animid != (int)MainManager.AnimIDs.TraitorWasp - 1)
                    {
                        entity.animstate = entity.basestate;
                    }
                }
            }

            MainManager.instance.StartCoroutine(MainManager.SetText("|sound,CrowdCheer||boxstyle,-1||hide||minibubble,39,28||fwait,0.5||minibubble,40,29||fwait,0.5||minibubble,41,31||fwait,0.5||minibubble,39,34||waitminibubble||end|", true, Vector3.zero, null, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.SetCamera(hoaxe.transform, null, 0.1f, new Vector3(0, 2.5f, -6));
            hoaxe.MoveTowards(0, 0f, 2.2f);
            yield return new WaitUntil(() => !hoaxe.forcemove);
            hoaxe.backsprite = false;

            entities[2].transform.position = new Vector3(-0.51f, 5f, 29f);
            entities[2].MoveTowards(new Vector3(0, 5, 15),2);
            yield return EventControl.tenthsec;

            MainManager.SetCamera(entities[2].transform, null, 0.1f, new Vector3(0, 2.5f, -6));
            yield return new WaitUntil(() => !entities[2].forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[33], true, Vector3.zero, entities[2].transform, entities[2].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[2].LockRigid(false);
            Vector3 ultimaxPos = entities[2].transform.position;
            MainManager.PlaySound("Jump");
            yield return MainManager.ArcMovement(entities[2].gameObject, ultimaxPos, hoaxe.transform.position + Vector3.left * 4, Vector3.zero, 5, 30, false);
            
            MainManager.PlaySound("Thud");
            MainManager.ShakeScreen(0.2f,1);

            hoaxe.FaceTowards(entities[2].transform.position);

            entities[2].FaceTowards(hoaxe.transform.position);
            entities[2].animstate = (int)MainManager.Animations.Flustered;

            MainManager.SetCamera(null, hoaxe.transform.position, 0.1f, new Vector3(-1, 2.5f, -7));

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[34], true, Vector3.zero, entities[2].transform, entities[2].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.animstate = 100;
            yield return EventControl.tenthsec;

            MainManager.PlaySound("Lazer2", 0.8f);
            entities[2].animstate = (int)MainManager.Animations.Woobly;
            MainManager.PlayParticle("HoaxeDiamond", entities[2].transform.position + new Vector3(0, 2f, -0.1f), 2);
            instance.StartCoroutine(entities[2].ShakeSprite(0.2f, 60));
            yield return EventControl.sec;

            hoaxe.animstate = (int)MainManager.Animations.Idle;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[35], true, Vector3.zero, entities[2].transform, entities[2].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[2].MoveTowards(hoaxe.transform.position + Vector3.left * 2, 1, (int)MainManager.Animations.Woobly, (int)MainManager.Animations.BattleIdle);
            yield return new WaitUntil(() => !entities[2].forcemove);

            hoaxe.animstate = (int)MainManager.Animations.ItemGet;
            MainManager.PlaySound("ItemHold");

            Sprite axeSprite = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("Hoaxe")[135];
            SpriteRenderer axe = MainManager.NewSpriteObject(hoaxe.transform.position + new Vector3(0, 4, -0.1f), MainManager.map.transform, axeSprite);

            yield return EventControl.sec;

            UnityEngine.Object.Destroy(axe.gameObject);
            hoaxe.animstate = (int)MainManager.Animations.Idle;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[36], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.animstate = 101;
            yield return EventControl.quartersec;
            MainManager.PlaySound("Slash");
            MainManager.ShakeScreen(0.2f, 0.5f);
            yield return EventControl.tenthsec;

            entities[2].LockRigid(true);
            entities[2].trail = true;
            entities[2].animstate = (int)MainManager.Animations.Hurt;
            MainManager.PlaySound("HugeHit2");
            MainManager.HitPart(entities[2].transform.position + Vector3.up);

            ultimaxPos = entities[2].transform.position;
            yield return MainManager.ArcMovement(entities[2].gameObject, ultimaxPos, ultimaxPos + Vector3.left * 4, new Vector3(0, 20), 5, 30, false);

            yield return EventControl.halfsec;
            hoaxe.animstate = (int)MainManager.Animations.Idle;
            entities[2].trail = false;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[37], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[2].MoveTowards(entities[2].transform.position + Vector3.left *5,2);
            yield return new WaitUntil(() => !entities[2].forcemove);

            entities[2].LockRigid(true);
            entities[2].SetPosition(new Vector3(0, -30));

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[38], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            /*foreach (var entity in MainManager.map.entities)
            {
                if (entity != null && !entity.iskill && entity.npcdata != null && entity.npcdata.entitytype == NPCControl.NPCType.NPC)
                {
                    entity.animstate = entity.basestate;
                }
            }*/

            MainManager.ResetCamera();    
            //south lz
            MainManager.map.entities[5].GetComponent<BoxCollider>().isTrigger = false;
        }
    }

    public class HoaxeIntermission6EndEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            EntityControl[] entities = MainManager.GetEntities(new int[] { 14, 16, 17, 19 });
            EntityControl hoaxe = MainManager.player.entity;


            MainManager.SetCamera(null, new Vector3?(new Vector3(3f, 2f, 8f)), 0.025f, MainManager.defaultcamoffset, new Vector3(5f, -20f));
            hoaxe.MoveTowards(0.4f, 0f, 7.5f);
            yield return new WaitUntil(() => !hoaxe.forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[35], true, Vector3.zero, entities[0].transform, entities[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.animstate = 100;
            MainManager.PlaySound("Lazer2", 0.8f);

            EntityControl[] troopers = { entities[1], entities[2] };
            Vector3[] troopersPos = { troopers[0].transform.position, troopers[1].transform.position };

            foreach (var entity in MainManager.map.entities)
            {
                if (entity != null && !entity.iskill && entity.npcdata != null && entity.npcdata.entitytype == NPCControl.NPCType.NPC)
                {
                    if (entity.animid != (int)MainManager.AnimIDs.WaspQueen - 1)
                    {
                        entity.animstate = (int)MainManager.Animations.Hurt;
                        instance.StartCoroutine(entity.ShakeSprite(0.2f, 120));
                    }
                    MainManager.PlayParticle("HoaxeDiamond", entity.transform.position + new Vector3(0, 2f, -0.1f), 1);
                }
            }

            yield return EventControl.sec;
            hoaxe.animstate = (int)MainManager.Animations.Idle;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[36], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MoveToQueen(troopers, entities[0]);
            entities[0].LockRigid(true);
            MainManager.SetCamera(null, new Vector3?(new Vector3(1f, 1f, 9f)), 0.025f, MainManager.defaultcamoffset, new Vector3(15f, -60));
            yield return EventControl.sec;
            yield return new WaitUntil(() => MainManager.EntitiesAreNotMoving(troopers));

            entities[0].transform.parent = troopers[0].transform;
            entities[0].animstate = (int)MainManager.Animations.Hurt;

            for(int i=0;i<troopers.Length;i++)
            {
                Vector3 targetPos = hoaxe.transform.position + new Vector3(-0.5f,0,3);

                if (i == 1)
                    targetPos += Vector3.right * 2;

                troopers[i].MoveTowards(targetPos);
            }
            yield return new WaitUntil(() => MainManager.EntitiesAreNotMoving(troopers));

            entities[0].transform.parent = MainManager.map.transform;
            entities[0].animstate = (int)MainManager.Animations.Angry;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[37], true, Vector3.zero, entities[0].transform, entities[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.animstate = 101;
            yield return EventControl.quartersec;
            MainManager.PlaySound("Slash");
            MainManager.ShakeScreen(0.2f, 0.5f);
            yield return EventControl.tenthsec;

            entities[0].LockRigid(true);
            entities[0].trail = true;
            entities[0].animstate = (int)MainManager.Animations.Hurt;
            MainManager.PlaySound("HugeHit2");
            MainManager.HitPart(entities[0].transform.position + Vector3.up);

            yield return MainManager.ArcMovement(entities[0].gameObject, entities[0].transform.position, new Vector3(0, 2f, 17.12f), new Vector3(0, 20), 5, 50, false);

            hoaxe.animstate = (int)MainManager.Animations.Idle;
            entities[0].trail = false;
            entities[0].animstate = (int)MainManager.Animations.KO;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[38], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MoveToQueen(troopers, entities[0]);
            MainManager.SetCamera(null, new Vector3?(new Vector3(0f, 2f, 18f)), 0.025f, MainManager.defaultcamoffset, new Vector3(15f, -20));
            yield return new WaitUntil(() => MainManager.EntitiesAreNotMoving(troopers));

            entities[0].transform.parent = troopers[0].transform;
            entities[0].transform.localPosition = Vector3.up;

            Vector3[] path = { new Vector3(3.2f, 2, 19.2f), new Vector3(0, 1, 25.6f) };

            foreach(var p in path)
            {
                for (int i = 0; i < troopers.Length; i++)
                {
                    troopers[i].MoveTowards(p);
                }
                yield return new WaitUntil(() => MainManager.EntitiesAreNotMoving(troopers));
            }

            entities[0].transform.parent = MainManager.map.transform;
            yield return MainManager.ArcMovement(entities[0].gameObject, entities[0].transform.position, new Vector3(0, 1, 29f), Vector3.zero, 5, 30, false);
            entities[0].PlaySound("Thud");

            yield return EventControl.tenthsec;
            entities[0].gameObject.SetActive(false);

            for (int i = 0; i < troopers.Length; i++)
            {
                troopers[i].forcejump = true;
                troopers[i].MoveTowards(troopersPos[i]);
            }

            hoaxe.forcejump = true;
            hoaxe.MoveTowards(new Vector3(0.22f, 2f, 16.9f));
            yield return new WaitUntil(()=>!hoaxe.forcemove);
            yield return EventControl.tenthsec;

            hoaxe.animstate = 100;
            yield return EventControl.tenthsec;

            MainManager.PlaySound("FirePillar");
            DialogueAnim pillar = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/FirePillar 1"), new Vector3(0, 1, 25.6f), Quaternion.identity) as GameObject).AddComponent<DialogueAnim>();
            pillar.transform.parent = MainManager.map.transform;
            pillar.transform.localScale = new Vector3(0f, 1f, 0f);
            pillar.targetscale = new Vector3(0.75f, 1f, 0.75f);
            pillar.shrink = false;
            pillar.shrinkspeed = 0.015f;
            yield return new WaitForSeconds(0.65f);
            pillar.shrinkspeed = 0.2f;
            yield return new WaitForSeconds(0.2f);
            MainManager.ShakeScreen(0.25f, 0.75f);
            yield return EventControl.sec;

            hoaxe.flip = false;
            hoaxe.backsprite = false;
            hoaxe.animstate = (int)MainManager.Animations.Idle;

            MainManager.SetCamera(null, new Vector3?(new Vector3(3f, 2f, 8f)), 0.025f, MainManager.defaultcamoffset, new Vector3(5f, -20f));

            entities[3].LockRigid(false);
            entities[3].alwaysactive = true;
            entities[3].transform.position = new Vector3(0f, 0, -1.46f);
            entities[3].animstate = 105;
            entities[3].MoveTowards(new Vector3(0, 0, 7.57f), 1, 106,105);
            yield return new WaitUntil(() => !entities[3].forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[39], true, Vector3.zero, entities[3].transform, entities[3].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            entities[3].forcejump = true;
            entities[3].MoveTowards(hoaxe.transform.position + new Vector3(2.5f,0,0.2f),1,106,105);
            yield return new WaitUntil(() => !entities[3].forcemove);
            MainManager.SetCamera(null, hoaxe.transform.position, 0.025f, new Vector3(0, 2, -7), new Vector3(15f, 0f));

            entities[3].FaceTowards(hoaxe.transform.position);

            hoaxe.animstate = 102;
            entities[3].animstate = (int)MainManager.Animations.Idle;
            yield return EventControl.halfsec;
            yield return EventControl.tenthsec;
            hoaxe.animid = (int)MainManager.AnimIDs.WaspKing - 1;
            hoaxe.animstate = (int)MainManager.Animations.Idle;
            hoaxe.FaceTowards(entities[3].transform.position);
            yield return EventControl.halfsec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[40], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[3].animstate = 101;
            yield return EventControl.halfsec;

            MainManager.FadeIn(0.05f,Color.white);
            MainManager.FadeMusic(0.05f);
            yield return new WaitForSeconds(2);

            MainManager.instance.playerdata[0].animid = 0;
            MainManager.instance.flags[11] = true;
            MainManager.player.basespeed = 5;
            MainManager.player.canpause = true;

            MainManager.instance.flags[945] = false;
            MainManager.instance.flags[946] = true;
            yield return EndIntermissionPostgame(instance, 194, (int)MainManager.Maps.RubberPrisonGiantLairBridge);

        }

        void MoveToQueen(EntityControl[] troopers, EntityControl queen)
        {
            for (int i = 0; i < troopers.Length; i++)
            {
                troopers[i].gameObject.layer = 9;
                troopers[i].forcejump = true;
                troopers[i].MoveTowards(queen.transform.position + (i == 0 ? Vector3.left : Vector3.right));
            }
        }
    }
}

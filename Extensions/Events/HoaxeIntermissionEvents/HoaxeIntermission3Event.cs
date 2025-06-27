using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.HoaxeIntermissionEvents
{
    public class HoaxeIntermission3Event : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            if (MainManager.instance.flags[555])
                MainManager.FadeIn();
            MainManager.FadeMusic(0.02f);
            yield return new WaitForSeconds(2);

            MainManager.instance.flags[916] = true;
            MainManager.instance.insideid = -1;
            MainManager.LoadMap(169);
            yield return null;
            yield return null;

            yield return SetupPlayerHoaxe(new Vector3(3.8f, 1.2f, -5.5f), (int)NewAnimID.Hoaxe);
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.animstate = 104;

            MainManager.SetCamera(hoaxe.transform.position + Vector3.down, 1);
            MainManager.FadeOut(0.01f);

            yield return new WaitForSeconds(2);

            for (int i = 0; i < 5; i++)
            {
                hoaxe.animstate = i % 2 == 0 ? 104 : 105;
                yield return EventControl.halfsec;
            }
            hoaxe.animstate = 105;

            hoaxe.StartCoroutine(hoaxe.ShakeSprite(0.3f, 60f));
            yield return EventControl.sec;

            MainManager.PlaySound("Jump", 1.2f);
            hoaxe.animstate = 108;

            EntityControl[] entities = MainManager.GetEntities(new int[] { 3, 4 });

            entities[1].transform.position = new Vector3(-0.30f, 0, -10.85f);
            entities[1].alwaysactive = true;
            yield return EventControl.sec;

            entities[0].Emoticon(MainManager.Emoticons.Exclamation, 60);
            yield return EventControl.sec;
            hoaxe.animstate = (int)MainManager.Animations.Sit;
            Vector3 basePos = entities[0].transform.position;

            entities[0].MoveTowards(new Vector3(3.39f, 0, -1.31f));
            yield return new WaitUntil(() => !entities[0].forcemove);
            entities[0].flip = true;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[16], true, Vector3.zero, entities[0].transform, entities[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[1].MoveTowards(new Vector3(0.48f, 0, -3.80f));
            yield return new WaitUntil(() => !entities[1].forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[17], true, Vector3.zero, entities[1].transform, entities[1].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[1].MoveTowards(new Vector3(-0.30f, 0, -10.85f));
            yield return new WaitUntil(() => !entities[1].forcemove);
            entities[1].gameObject.SetActive(false);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[18], true, Vector3.zero, entities[0].transform, entities[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[0].MoveTowards(basePos);
            yield return new WaitUntil(() => !entities[0].forcemove);

            MainManager.PlaySound("Jump");
            hoaxe.animstate = (int)MainManager.Animations.Jump;
            hoaxe.StartCoroutine(MainManager.ArcMovement(hoaxe.gameObject, new Vector3(2.82f, 0, -2.24f), 5, 30));
            yield return EventControl.halfsec;
            hoaxe.animstate = 0;

            hoaxe.LockRigid(false);
            SetupHoaxeFlags();
            MainManager.ResetCamera();
            MainManager.ChangeMusic();
            yield return EventControl.tenthsec;
        }
    }


    public class HoaxeIntermission3EndEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.FadeMusic(0.02f);
            MainManager.ChangeMusic("Tension2");
            EntityControl[] entities = MainManager.GetEntities(new int[] { 14,15,16,17});
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;

            MainManager.SetCamera(null, new Vector3?(new Vector3(3f, 2f, 8f)), 0.025f, MainManager.defaultcamoffset, new Vector3(5f, -20f));
            hoaxe.MoveTowards(0.4f, 0f, 7.5f);
            yield return new WaitUntil(() => !hoaxe.forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[34], true, Vector3.zero, entities[1].transform, entities[1].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.FadeIn();
            MainManager.FadeMusic(0.02f);
            hoaxe.MoveTowards(0.23f, 0f, 1.88f);
            yield return new WaitUntil(() => !hoaxe.forcemove);

            yield return EventControl.sec;

            MainManager.LoadMap(165);
            yield return EventControl.tenthsec;
            hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.transform.position = new Vector3(-31.54f, 0, 3.13f);
            hoaxe.animstate = 106;

            entities = MainManager.GetEntities(new int[] { 8,9,10,11,12 });

            for (int i = 0; i < entities.Length-1; i++) 
            {
                entities[i].alwaysactive = true;
                entities[i].transform.position = new Vector3(-10f+(0.1f*i), 0, 7f + (0.1f * i));
                entities[i].gameObject.layer = 9;
            }

            entities[4].transform.position = hoaxe.transform.position + Vector3.right * 3;
            entities[4].animstate = (int)MainManager.Animations.Angry;
            entities[4].flip = false;

            MainManager.SetCamera(hoaxe.transform.position,1);
            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[1], true, Vector3.zero, entities[4].transform, entities[4].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            entities[4].MoveTowards(-18.81f, 0, 3.13f);
            yield return new WaitUntil(() => !entities[4].forcemove);
            entities[4].gameObject.SetActive(false);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[2], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            Vector3 targetPos = hoaxe.transform.position + Vector3.up*2f;

            SpriteRenderer berry = MainManager.NewSpriteObject(new Vector3(-18.81f, 0, 3.13f), MainManager.map.transform, MainManager.itemsprites[0, 6]);
            yield return MainManager.ArcMovement(berry.gameObject, berry.transform.position, targetPos, new Vector3(0, 0, 20), 5, 40, true);


            MainManager.HitPart(targetPos);
            MainManager.PlaySound("Damage0");
            hoaxe.flip = true;

            Sprite broomSprite = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("Hoaxe")[32];
            SpriteRenderer broom = MainManager.NewSpriteObject(hoaxe.transform.position + new Vector3(0,0,-0.1f), MainManager.map.transform, broomSprite);
            hoaxe.animstate = (int)MainManager.Animations.Surprized;

            float a = 0;
            float b = 15;
            Vector3 startPos = broom.transform.position;
            do
            {
                broom.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 90, a / b));
                broom.transform.position = Vector3.Lerp(startPos, startPos+new Vector3(-1,0.1f,0),a/b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);


            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[3], true, Vector3.zero, entities[1].transform, entities[1].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.ChangeMusic("Battle3");

            Vector3[] waspPos = new Vector3[]
            {
                new Vector3(-27.53f, 0, 4f),
                new Vector3(-26f, 0, 3.23f),
                new Vector3(-28.83f, 0, 3.15f),
                new Vector3(-26.066f, 0, 4.72f),
            };

            for (int i = 0; i < entities.Length - 1; i++)
            {
                entities[i].MoveTowards(waspPos[i],2);
                yield return null;
            }
            yield return new WaitUntil(() => !entities[3].forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[4], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            waspPos = new Vector3[]
            {
                new Vector3(-29.8f, 0, 3.06f),
                new Vector3(-33.42f, 0, 2.8f),
                new Vector3(-30.05f, 0, 4.80f),
                new Vector3(-32.16f, 0, 4.01f),
            };

            for (int i = 0; i < entities.Length - 1; i++)
            {
                entities[i].MoveTowards(waspPos[i],2f);
                yield return null;
                yield return null;
            }

            while(entities.Any(e => e.forcemove))
            {
                yield return null;
            }

            hoaxe.animstate = 100;
            GameObject tiredPart = MainManager.PlayParticle("Tired", hoaxe.transform.position, -1);
            Coroutine shakeSprite = hoaxe.StartCoroutine(hoaxe.ShakeSprite(0.3f, 300));

            for (int i = 0; i < entities.Length - 1; i++)
            {
                entities[i].FaceTowards(hoaxe.transform.position);
            }
            Coroutine[] attacks = new Coroutine[entities.Length - 1];
            for (int i = 0; i < entities.Length - 1; i++) 
            {
                switch (entities[i].animid)
                {
                    case (int)MainManager.AnimIDs.WaspTrooper - 1:
                        attacks[i] = instance.StartCoroutine(DoWaspTrooperAttack(entities[i]));
                        break;

                    case (int)MainManager.AnimIDs.WaspScout - 1:
                        attacks[i] = instance.StartCoroutine(DoWaspScoutAttack(entities[i],hoaxe));
                        break;

                    case (int)MainManager.AnimIDs.WaspDriller - 1:
                        entities[i].animstate = 100;
                        break;
                }
            }

            MainManager.FadeMusic(0.01f);
            MainManager.FadeIn(0.02f);
            SpriteRenderer dimmer = MainManager.GetTransitionSprite();
            while (dimmer.color.a < 0.95f)
            {
                yield return null;
            }
            yield return EventControl.sec;

            for (int i = 0; i < attacks.Length; i++)
            {
                if(attacks[i] != null)
                    instance.StopCoroutine(attacks[i]);
            }
            UnityEngine.Object.Destroy(tiredPart);
            if(shakeSprite != null)
                hoaxe.StopCoroutine(shakeSprite);

            MainManager.LoadMap(169);
            yield return EventControl.tenthsec;
            hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.transform.position = new Vector3(0.2f, 0, -9);
            hoaxe.animstate = 103;

            MainManager.SetCamera(hoaxe.transform, null, 1);
            for(int i=0; i< MainManager.map.entities.Length; i++)
            {
                if(MainManager.map.entities[i] != null)
                    MainManager.map.entities[i].gameObject.SetActive(false);
            }
            yield return EventControl.sec;
            MainManager.FadeOut(0.01f);
            hoaxe.overrideanim = true;
            hoaxe.LockRigid(true);
            hoaxe.flip = false;
            hoaxe.backsprite = false;
            for(int i = 0; i < 4; i++)
            {
                hoaxe.animstate = 103;
                yield return BattleControl_Ext.LerpPosition(25, hoaxe.transform.position, new Vector3(0.2f, 0, -7 + i * 2), hoaxe.transform);
                yield return EventControl.halfsec;
            }

            hoaxe.animstate = (int)MainManager.Animations.KO;
            MainManager.SetCamera(null, hoaxe.transform.position + Vector3.forward * 2, 0.01f);
            MainManager.PlaySound("Death3");
            MainManager.PlayParticle("deathsmoke", hoaxe.transform.position + Vector3.left);

            yield return EventControl.halfsec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[19], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.PlaySound("Flame", 1.1f, 1);
            GameObject firePart = MainManager.PlayParticle("Flame", hoaxe.transform.position + new Vector3(-1.2f,0.2f,-0.1f),-1);
            firePart.transform.localScale = Vector3.one * 0.5f;
            firePart.transform.GetChild(0).gameObject.SetActive(false);
            for(int i = 0; i < 5; i++)
            {
                firePart.gameObject.SetActive(!firePart.activeSelf);
                yield return EventControl.halfsec;
            }

            MainManager.FadeIn();
            yield return EventControl.sec;

            UnityEngine.Object.Destroy(firePart);
            MainManager.instance.playerdata[0].animid = 0;
            MainManager.instance.flags[11] = true;
            MainManager.player.basespeed = 5;
            MainManager.player.canpause = true;

            MainManager.instance.flags[929] = true;
            yield return EndIntermissionPostgame(instance, 99,(int)MainManager.Maps.HoneyFactoryCore);
        }

        IEnumerator DoWaspTrooperAttack(EntityControl trooper)
        {
            while (true)
            {
                trooper.PlaySound("Woosh5");
                trooper.animstate = 101;
                yield return EventControl.halfsec;
                trooper.PlaySound("Woosh5");
                trooper.animstate = 102;
                yield return EventControl.halfsec;
                MainManager.PlaySound("Damage0", 1 + UnityEngine.Random.Range(-0.2f, 0.2f), 0.5f);
                yield return EventControl.halfsec;
            }
        }

        IEnumerator DoWaspScoutAttack(EntityControl scout, EntityControl hoaxe)
        {
            while (true)
            {
                scout.animstate = 100;
                yield return EventControl.quartersec;

                scout.animstate = 101;
                yield return EventControl.quartersec;
                scout.animstate = 103;
                MainManager.PlaySound("Damage0", 1 + UnityEngine.Random.Range(-0.2f, 0.2f), 0.5f);
                yield return EventControl.halfsec;
            }
        }
    }
}

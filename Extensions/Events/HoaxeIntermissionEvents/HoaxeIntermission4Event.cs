using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.HoaxeIntermissionEvents
{
    public class HoaxeIntermission4Event : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            if (MainManager.instance.flags[555])
                MainManager.FadeIn();
            MainManager.FadeMusic(0.02f);
            yield return new WaitForSeconds(2);

            MainManager.instance.flags[916] = true;
            MainManager.instance.flags[938] = false;
            MainManager.instance.insideid = -1;

            MainManager.LoadMap((int)MainManager.Maps.WaspKingdomDrillRoom);
            yield return null;
            yield return null;

            yield return SetupPlayerHoaxe(new Vector3(-4.54f, 0, 1.62f), (int)NewAnimID.Hoaxe);
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.animstate = 106;
            hoaxe.emoticonoffset = new Vector3(0, 2.2f, -0.1f);
            MainManager.SetCamera(hoaxe.transform.position + Vector3.down, 1);
            MainManager.FadeOut(0.01f);

            yield return new WaitForSeconds(2);
            hoaxe.Emoticon(MainManager.Emoticons.DotsLong, 120);
            yield return new WaitForSeconds(2);

            hoaxe.animstate = 111;
            Sprite broomSprite = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("Hoaxe")[32];
            SpriteRenderer broom = MainManager.NewSpriteObject(hoaxe.transform.position + new Vector3(0, 0, -0.1f), MainManager.map.transform, broomSprite);
            yield return EventControl.thirdsec;
            MainManager.PlaySound("Toss3");
            float a = 0;
            float b = 15;
            Vector3 startPos = broom.transform.position;
            do
            {
                broom.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 90, a / b));
                broom.transform.position = Vector3.Lerp(startPos, startPos + new Vector3(-4, 0.1f, 0), a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            yield return EventControl.quartersec;
            hoaxe.animstate = (int)MainManager.Animations.Flustered;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[1], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.LockRigid(false);
            SetupHoaxeFlags();
            MainManager.ResetCamera();
            MainManager.ChangeMusic();
            yield return EventControl.tenthsec;
        }
    }

    public class HoaxeIntermission4EventEnd : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.FadeMusic(0.02f);
            EntityControl hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.animstate = 109;
            MainManager.map.transform.Find("bookHoaxe").gameObject.SetActive(false);
            MainManager.SetCamera(hoaxe.transform.position + new Vector3(0,-1,1), 0.05f);
            yield return EventControl.tenthsec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[5], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.FadeIn();
            yield return EventControl.sec;

            yield return null;
            MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>().sortingOrder = -30;
            yield return null;

            Sprite saplingSlide = Resources.LoadAll<Sprite>("Sprites/GUI/introslide")[3];
            SpriteRenderer[] sprites = new SpriteRenderer[2];
            Transform[] box = new Transform[2];

            sprites[0] = MainManager.NewSolidColor("back", Color.black, 0.01f, new Vector3(0f, 0f, 1f), new Vector2(0.5f, 0.5f));
            sprites[0].transform.parent = MainManager.GUICamera.transform;
            sprites[0].transform.localEulerAngles = Vector3.zero;
            sprites[0].transform.localPosition = new Vector3(0f, 0f, 1f);
            sprites[0].gameObject.layer = 5;
            sprites[0].sortingOrder = 5;

            yield return null;
            box[0] = MainManager.Create9Box(new Vector3(0f, 0f, 9f), new Vector2(17.5f, 10f), 1, -8, new Color(0.75f, 1f, 0.75f), false);
            box[1] = MainManager.Create9Box(new Vector3(0f, 0.85f, 8f), new Vector2(14.5f, 6.35f), 4, -2, new Color(1f, 0.75f, 0.2f), false);
            for (int i = 0; i < box.Length; i++)
            {
                box[i].transform.localScale = Vector3.one;
            }
            yield return null;

            sprites[1] = MainManager.NewUIObject("front", sprites[0].transform, new Vector3(0f, 0.85f), new Vector3(1f, 0.75f, 1f), saplingSlide, 1).GetComponent<SpriteRenderer>();
            sprites[1].color = new Color(0.75f, 0.75f, 0.75f);
            MainManager.ResetCamera(true);
            MainManager.ChangeMusic("Calm");

            yield return MainManager_Ext.LerpSpriteColor(sprites[0], 60, Color.clear);
            yield return EventControl.quartersec;

            for (int i = 0; i < 2; i++)
            {
                instance.StartCoroutine(MainManager.SetText("|hidespeed,1||boxstyle,-1||sort,2||bleep,2,1,1|" + MainManager.map.dialogues[6 + i], 0, new float?(14f), true, false, new Vector3(-7f, -6.15f, 10f), Vector3.zero, Vector2.one * 0.9f, null, null));
                yield return null;
                MainManager.instance.blinker.transform.localPosition = new Vector3(7.5f, -7.5f, -1f);
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                yield return null;
                if (i == 0)
                {
                    yield return MainManager_Ext.LerpSpriteColor(sprites[1], 60, Color.clear);
                    sprites[1].sprite = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("introslide_Crown")[0];

                    yield return EventControl.tenthsec;
                    yield return MainManager_Ext.LerpSpriteColor(sprites[1], 60, new Color(0.75f, 0.75f, 0.75f));
                }
            }
            MainManager.FadeMusic(0.007f);
            yield return MainManager_Ext.LerpSpriteColor(sprites[0], 60, Color.black);
            yield return EventControl.quartersec;

            MainManager.SetCamera(hoaxe.transform.position + new Vector3(0, -1, 1), 1);

            for(int i=0;i<box.Length;i++)
                UnityEngine.Object.Destroy(box[i].gameObject);
            UnityEngine.Object.Destroy(sprites[0].gameObject);

            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[8], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.FadeIn();
            yield return EventControl.sec;

            MainManager.LoadMap((int)MainManager.Maps.WaspKingdomJayde);
            yield return null;
            yield return null;

            EntityControl jayde = MainManager.GetEntity(3);
            jayde.transform.position = new Vector3(0, 0, -6.45f);
            jayde.alwaysactive = true;
            jayde.gameObject.SetActive(false);

            hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.transform.position = new Vector3(4f, 0, -0.4596f);
            hoaxe.animstate = 110;
            hoaxe.flip = true;

            Sprite bagSprite = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("Hoaxe")[65];
            SpriteRenderer bag = MainManager.NewSpriteObject(hoaxe.transform.position + new Vector3(0.6f, 0, -0.1f), MainManager.map.transform, bagSprite);

            MainManager.SetCamera(hoaxe.transform.position,MainManager.defaultcamangle, new Vector3(2, 2, -6.25f), 1);

            MainManager.FadeOut(0.02f);
            yield return EventControl.sec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[20], true, Vector3.zero, hoaxe.transform, hoaxe.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            jayde.gameObject.SetActive(true);

            jayde.MoveTowards(new Vector3(1.65f, 0, -1.63f));
            yield return new WaitUntil(() => !jayde.forcemove);

            hoaxe.Emoticon(MainManager.Emoticons.Exclamation, 45);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[21], true, Vector3.zero, jayde.transform, jayde.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.PlaySound("ItemGet0");
            hoaxe.flip = false;
            hoaxe.animstate = (int)MainManager.Animations.ItemGet;
            bag.transform.position = hoaxe.transform.position + new Vector3(0, 2.5f, -0.1f);
            yield return EventControl.sec;

            hoaxe.animstate =(int)MainManager.Animations.Idle;
            UnityEngine.Object.Destroy(bag.gameObject);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[22], true, Vector3.zero, jayde.transform, jayde.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxe.StartCoroutine(hoaxe.TempIgnoreColision(jayde.ccol, 60));
            hoaxe.animstate = 101;
            hoaxe.MoveTowards(new Vector3(-1, 0, -6f), 2, 101, 0);
            
            yield return EventControl.tenthsec;
            hoaxe.PlaySound("FastWoosh");
            jayde.StartCoroutine(jayde.SlowSpinStop(new Vector3(0, 10), 60));

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[23], true, Vector3.zero, jayde.transform, jayde.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.FadeIn();
            yield return EventControl.sec;


            MainManager.LoadMap((int)MainManager.Maps.FarGrasslandsLake);
            yield return null;
            yield return null;

            hoaxe = MainManager.instance.playerdata[0].entity;
            hoaxe.transform.position = new Vector3(-1.46f, 0, 50f);

            hoaxe.MoveTowards(new Vector3(-14f, 0, 28f), 1.5f,101,101);
            MainManager.SetCamera(hoaxe.transform.position, new Vector3(15,15,0),new Vector3(0,2.25f,-30),1);
            MainManager.FadeOut(0.02f);
            yield return EventControl.sec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[6], true, Vector3.zero, null, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.FadeIn();
            yield return EventControl.sec;

            MainManager.instance.playerdata[0].animid = 0;
            MainManager.instance.flags[11] = true;
            MainManager.player.basespeed = 5;
            MainManager.player.canpause = true;

            MainManager.instance.flags[937] = true;
            yield return EndIntermissionPostgame(instance, 118, (int)MainManager.Maps.SandCastleTreasureRoom);
        }
    }
}

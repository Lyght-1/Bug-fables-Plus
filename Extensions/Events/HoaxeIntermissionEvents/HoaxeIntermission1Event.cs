using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace BFPlus.Extensions.Events.HoaxeIntermissionEvents
{
    public class HoaxeIntermission1Event : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            if (MainManager.instance.flags[555])
                MainManager.FadeIn();
            MainManager.FadeMusic(0.02f);
            yield return EventControl.sec;
            yield return EventControl.sec;

            MainManager.instance.flags[916] = true;
            MainManager_Ext.noJump = true;
            MainManager.instance.insideid = -1;
            MainManager.LoadMap(233);
            yield return null;
            yield return null;

            List<Collider> colliders = new List<Collider>
            {
                MainManager.map.mainmesh.Find("MossyRoundRock (1)").gameObject.GetComponent<Collider>(),
                MainManager.map.mainmesh.Find("DeadTrunk2").gameObject.GetComponent<Collider>()
            };

            foreach (var col in colliders)
                col.enabled = false;

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.following = null;
                MainManager.map.chompy.transform.position = new Vector3(0,-30);
            }

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                UnityEngine.Object.Destroy(MainManager.instance.playerdata[i].entity.gameObject);
            }

            MainManager.ChangeParty(new int[1] { 0 }, true, true);
            MainManager.SetPlayers();
            yield return EventControl.tenthsec;
            EntityControl babyHoaxe = MainManager.instance.playerdata[0].entity;
            babyHoaxe.gameObject.SetActive(true);
            babyHoaxe.LockRigid(true);
            babyHoaxe.transform.position = new Vector3(0, -1000, 0);
            babyHoaxe.animid = (int)NewAnimID.BabyHoaxe;
            babyHoaxe.emoticonoffset = new Vector3(0, 1, -0.1f);

            EntityControl hoaxMom = EntityControl.CreateNewEntity("hoaxeMom", (int)NewAnimID.HoaxeMom,new Vector3(1.67f, 0, 5.20f));
            hoaxMom.animstate = 100;
            hoaxMom.transform.parent = MainManager.map.transform;

            MainManager.SetCamera(hoaxMom.transform,null, 1f);
            MainManager.FadeOut(0.01f);
            hoaxMom.MoveTowards(new Vector3(11.5f, -0.4f, 5f), 0.5f, 100, 105);

            yield return null;
            yield return new WaitUntil(() =>!hoaxMom.forcemove);

            hoaxMom.animstate = 105;
            for(int i = 0; i < 3; i++)
            {
                hoaxMom.flip = !hoaxMom.flip;
                yield return EventControl.sec;
            }
            hoaxMom.flip = true;

            hoaxMom.animstate = 106;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[1], true, Vector3.zero, hoaxMom.transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxMom.MoveTowards(new Vector3(14.1f, -0.4f, 6.4f), 0.5f, 100, 105);
            yield return null;
            yield return new WaitUntil(() => !hoaxMom.forcemove);

            hoaxMom.animstate = 101;
            yield return EventControl.halfsec;
            babyHoaxe.gameObject.SetActive(true);
            babyHoaxe.transform.position = new Vector3(15.1f, -0.4f, 6.4f);
            babyHoaxe.animstate = 101;
            babyHoaxe.flip = false;
            hoaxMom.animstate = 102;
            yield return EventControl.sec;

            hoaxMom.animstate = 103;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[2], true, Vector3.zero, hoaxMom.transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            hoaxMom.animstate = 102;
            yield return EventControl.sec;

            MainManager.PlaySound("OmegaEye", 1.5f);
            var eye = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/Eye")) as GameObject).transform;
            eye.transform.position = new Vector3(13.97f, 13.38f, 17.57f);
            eye.transform.localEulerAngles = new Vector3(322.3f, 179f, 179.89f);

            var light = eye.GetChild(2);
            Renderer[] componentsInChildren = light.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, componentsInChildren[i].material.color.a);
            }

            hoaxMom.animstate = 104;
            yield return EventControl.sec;
            UnityEngine.Object.Destroy(eye.gameObject);
            MainManager.SetCamera(babyHoaxe.transform.position, 0.02f);

            hoaxMom.animstate = 0;
            hoaxMom.Jump(10);
            hoaxMom.PlaySound("Jump");
            yield return null;
            yield return new WaitUntil(() => hoaxMom.onground);

            hoaxMom.animstate = (int)MainManager.Animations.Walk;
            hoaxMom.MoveTowards(new Vector3(0, 0, 4), 1.5f);
            yield return null;
            yield return new WaitUntil(() => !hoaxMom.forcemove);
            hoaxMom.gameObject.SetActive(false);

            yield return EventControl.sec;
            yield return EventControl.sec;

            for(int i = 0; i < 5; i++)
            {
                babyHoaxe.animstate = i % 2 == 0 ? 102 : 101;
                yield return EventControl.quartersec;
            }
            babyHoaxe.animstate = 102;
            babyHoaxe.StartCoroutine(babyHoaxe.ShakeSprite(0.2f, 60));
            yield return EventControl.sec;

            babyHoaxe.animstate = 0;
            GameObject blanket = UnityEngine.Object.Instantiate(MainManager_Ext.assetBundle.LoadAsset<GameObject>("HoaxeBlanket"),MainManager.map.transform);
            blanket.GetComponent<SpriteRenderer>().material = MainManager.spritemat;
            blanket.transform.position = babyHoaxe.transform.position + new Vector3(0, 0, 0.1f);
            yield return EventControl.halfsec;
            blanket.GetComponent<Animator>().enabled = false;

            babyHoaxe.LockRigid(true);

            MainManager.SetCamera(babyHoaxe.transform, null, 0.02f, new Vector3(0,1f,-4f));
            babyHoaxe.animstate = 1;
            yield return BattleControl_Ext.LerpPosition(60, babyHoaxe.transform.position, new Vector3(14.3f, -0.4f, 5.1f), babyHoaxe.transform);
            
            babyHoaxe.animstate = 0;
            for (int i = 0; i < 4; i++)
            {
                babyHoaxe.flip = !babyHoaxe.flip;
                yield return EventControl.sec;
            }

            babyHoaxe.PlaySound("Lost");
            babyHoaxe.Emoticon(MainManager.Emoticons.QuestionMark, 100);
            yield return new WaitUntil(() => babyHoaxe.emoticoncooldown <= 0f);

            babyHoaxe.PlaySound("Hungry", 1.2f, 1);
            babyHoaxe.StartCoroutine(babyHoaxe.ShakeSprite(0.05f, 30));
            yield return EventControl.sec;

            babyHoaxe.LockRigid(false);
            MainManager.player.basespeed = 2;
            MainManager.player.canpause = false;
            MainManager.instance.flags[11] = false; //can use beemrang
            MainManager.ResetCamera();

            foreach (var col in colliders)
                col.enabled = true;
            yield return EventControl.sec;
            //endEvent = false;
        }
    }

    public class HoaxeIntermission1EndEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while(MainManager.instance.message)
                yield return null;

            EntityControl babyHoaxe = MainManager.instance.playerdata[0].entity;

            MainManager.SetCamera(babyHoaxe.transform, null, 0.02f, new Vector3(0, 1f, -4f), new Vector3(10,15,0));

            GameObject mysteryBerry = MainManager.map.transform.Find("mysterryBerryHoaxe").gameObject;

            babyHoaxe.MoveTowards(new Vector3(34f, 0f, 11.39f));
            yield return null;
            yield return new WaitUntil(() => !babyHoaxe.forcemove);
            babyHoaxe.flip = false;

            babyHoaxe.animstate = 100;
            MainManager.PlaySound("Eat",-1, 1.2f, 1, true);

            yield return new WaitForSeconds(3);
            MainManager.StopSound("Eat");
            mysteryBerry.SetActive(false);
            babyHoaxe.animstate = 0;

            MainManager.PlaySound("Heal");
            MainManager.PlayParticle("Heart", babyHoaxe.transform.position + Vector3.up * 0.5f);
            yield return EventControl.sec;

            for (int i = 0; i < 4; i++)
            {
                babyHoaxe.flip = !babyHoaxe.flip;
                yield return EventControl.sec;
            }
            yield return EventControl.tenthsec;
            babyHoaxe.PlaySound("Lost");
            yield return new WaitForSeconds(2);

            MainManager.FadeIn();
            yield return new WaitForSeconds(2);

            MainManager.instance.flags[951] = true;

            MainManager_Ext.noJump = false;
            MainManager.instance.flags[11] = true;
            MainManager.player.basespeed = 5;
            MainManager.player.canpause = true;
            yield return EndIntermissionPostgame(instance, 26);
        }
    }
}

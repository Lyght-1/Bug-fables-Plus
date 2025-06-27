using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewDungeonsEvents.GiantLairPlayroomEvents
{
    public class PlayroomTrainEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }

            string text = MainManager.map.dialogues[3];

            text += "|prompt,map,0.7,";

            int numberOfStations = 2;

            //unlocked station 3
            if (MainManager.instance.flags[921])
            {
                numberOfStations++;
            }
            //unlocked station 4
            if (MainManager.instance.flags[926])
            {
                numberOfStations++;
            }

            text += numberOfStations + ",";

            for (int i = 0; i < numberOfStations; i++) 
            {
                text += "-11,";
            }

            int currentMap = (int)MainManager.map.mapid;

            int toSkip = 4 + currentMap - (int)NewMaps.GiantLairPlayroom1;

            for (int i = 0; i < numberOfStations; i++)
            {
                int station = 4 + i;

                if(toSkip != station)
                {
                    text += station + ",";
                }
            }

            text += "8|";
            Console.WriteLine(text);
            instance.StartCoroutine(MainManager.SetText(text, true, Vector3.zero, caller.transform,caller));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            int mapToLoad = -1;
            Vector3 playerPos = Vector3.zero;

            int option = MainManager.instance.option;
            if (option < numberOfStations - 1)
            {
                if (currentMap == (int)NewMaps.GiantLairPlayroom1)
                    option++;

                if (option >= 1 && currentMap == (int)NewMaps.GiantLairPlayroom2)
                    option++;

                if (option >= 2 && currentMap == (int)NewMaps.GiantLairPlayroom3)
                    option++;


                switch (option)
                {
                    //station 1
                    case 0:
                        mapToLoad = (int)NewMaps.GiantLairPlayroom1;
                        playerPos = new Vector3(-7.87f, 0f, 14.83f);
                        break;

                    //station 2
                    case 1:
                        mapToLoad = (int)NewMaps.GiantLairPlayroom2;
                        playerPos = new Vector3(11.94f, -0.1f, 18.43f);
                        break;

                    //station 3
                    case 2:
                        mapToLoad = (int)NewMaps.GiantLairPlayroom3;
                        playerPos = new Vector3(-18.31f, -0.1f, 47.42f);
                        break;

                    //station 4
                    case 3:
                        mapToLoad = (int)NewMaps.GiantLairPlayroomBoss;
                        playerPos = new Vector3(24.66f, 0.1f, 22.74f);
                        break;
                }

                bool behind = mapToLoad < currentMap || (mapToLoad == (int)NewMaps.GiantLairPlayroomBoss && currentMap == (int)NewMaps.GiantLairPlayroom1 && MainManager.instance.flags[927]);

                if (mapToLoad == (int)NewMaps.GiantLairPlayroom1 && currentMap == (int)NewMaps.GiantLairPlayroomBoss && MainManager.instance.flags[927])
                {
                    behind = false;
                }

                GameObject train = MainManager.map.transform.Find("Base").Find("ToyTrain").gameObject;

                if (behind)
                {
                    yield return FlipTrain(train.transform, 180);
                }
                Vector3 playerOffset = new Vector3(behind ? 2.8f : -2.8f, 0);

                Vector3 targetPos = train.transform.position + playerOffset;
                EntityControl[] party = MainManager.GetPartyEntities(true);
                if(MainManager.map.chompy != null)
                    party = party.AddToArray(MainManager.map.chompy);
                party[0].PlaySound("Jump");
                for (int i = 0; i < party.Length; i++)
                {
                    party[i].animstate = (int)MainManager.Animations.Jump;
                    party[i].LockRigid(true);
                    party[i].StartCoroutine(MainManager.ArcMovement(party[i].gameObject, GetTrainPartyPos(i, targetPos, behind), 5f, 30f));
                }

                yield return EventControl.halfsec;

                for (int i = 0; i < party.Length; i++)
                {
                    party[i].animstate = (int)MainManager.Animations.Idle;
                    party[i].transform.parent = train.transform;
                    party[i].flip = !behind;
                    party[i].backsprite = false;
                }


                train.GetComponent<Animator>().Play("Move");
                
                
                MainManager.PlaySound(MainManager_Ext.assetBundle.LoadAsset<AudioClip>("ToyTrainWhistle"));
                yield return EventControl.sec;

                targetPos = train.transform.position + (behind ? Vector3.left: Vector3.right) * 10;
                MainManager.PlaySound("ElevatorStart");
                MainManager.FadeIn(0.05f);
                yield return BattleControl_Ext.LerpPosition(180, train.transform.position, targetPos, train.transform);
                
                MainManager.LoadMap(mapToLoad);
                yield return EventControl.halfsec;
                train = MainManager.map.transform.Find("Base").Find("ToyTrain").gameObject;

                if (behind)
                    train.transform.localEulerAngles = new Vector3(180, 90, 90);

                Animator trainAnim = train.GetComponent<Animator>();
                trainAnim.Play("Move");

                targetPos = train.transform.position + playerOffset;
                party = MainManager.GetPartyEntities(true);


                if (MainManager.map.chompy != null)
                    party = party.AddToArray(MainManager.map.chompy);

                for (int i = 0; i < party.Length; i++)
                {
                    party[i].LockRigid(true);
                    party[i].transform.position = GetTrainPartyPos(i, targetPos, behind);
                    party[i].transform.parent = train.transform;
                    party[i].flip = !behind;
                    party[i].backsprite = false;
                }

                MainManager.ResetCamera(true);

                Vector3 startPos = train.transform.position;
                train.transform.position = train.transform.position + (behind ? Vector3.right : Vector3.left) * 10;

                MainManager.FadeOut(0.02f);
                MainManager.PlaySound("ElevatorEnd");
                yield return BattleControl_Ext.LerpPosition(180, train.transform.position, startPos, train.transform);
                trainAnim.Play("Idle");
                MainManager.PlaySound("WoodEnd");
                party[0].PlaySound("Jump");
                for (int i = 0; i < party.Length; i++)
                {
                    party[i].animstate = (int)MainManager.Animations.Jump;
                    party[i].LockRigid(true);
                    party[i].transform.parent = null;
                    party[i].StartCoroutine(MainManager.ArcMovement(party[i].gameObject, playerPos, 5f, 30f));
                }

                yield return EventControl.halfsec;
                for (int i = 0; i < party.Length; i++)
                {
                    party[i].LockRigid(false);
                    party[i].animstate = (int)MainManager.Animations.Idle;
                }

                if (behind)
                {
                    yield return FlipTrain(train.transform, 360);
                }
            }
        }

        Vector3 GetTrainPartyPos(int index, Vector3 targetPos, bool behind)
        {
            Vector3 baseOffset = new Vector3(-0.7f*index,0.5f,-0.5f + (index % 2 == 0 ? 0.2f : -0.2f));

            if (behind)
            {
                baseOffset = Vector3.Scale(baseOffset,new Vector3(1,0,1) * -1) + Vector3.up*0.5f;
            }
            return targetPos + baseOffset;

            //return  targetPos + new Vector3((behind ? 0.7f : -0.7f) * index, 0.5f, behind ? 0.5f : -0.5f + (index %2==0 ? 0.1f : -0.1f));
        }

        IEnumerator FlipTrain(Transform train, float targetXAngle)
        {
            Vector3 groundPos = train.transform.position;
            MainManager.PlaySound("AhoneynationBodySlamJump", 1.2f,1);
            yield return BattleControl_Ext.LerpPosition(15, groundPos, train.transform.position + Vector3.up * 3, train.transform);

            float a = 0;
            float b = 20;
            float startXAngle = train.transform.localEulerAngles.x;
            do
            {
                train.transform.localEulerAngles = new Vector3(Mathf.Lerp(startXAngle, targetXAngle, a / b), 90, 90);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            train.transform.localEulerAngles = new Vector3(targetXAngle, 90, 90);
            MainManager.PlaySound("Switch", 0.8f, 1);

            yield return BattleControl_Ext.LerpPosition(15, train.transform.position, groundPos, train.transform);
            MainManager.ShakeScreen(0.1f, 1);
            MainManager.PlaySound("Thud4");
            MainManager.PlayParticle("impactsmoke", train.transform.position);
        }
    }
}

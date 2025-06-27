using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events
{

    public class LeafbugVillageLiftEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            int targetMap = -1;
            Transform mainLift = null;
            Vector3 targetPos = Vector3.zero;

            if (MainManager.map.mapid == MainManager.Maps.Swamplands5)
            {
                mainLift = MainManager.map.transform.Find("LeafbugVillageWay(Clone)").Find("Lift");
                targetMap = (int)NewMaps.LeafbugVillage;
                targetPos = mainLift.position + Vector3.up * 5;
            }
            else
            {
                mainLift = MainManager.map.mainmesh.Find("Lift");
                targetMap = (int)MainManager.Maps.Swamplands5;
                targetPos = mainLift.position + Vector3.down * 5;
            }

            MainManager.PlaySound("WoodStart");
            yield return DoLift(mainLift, targetPos, true);

            MainManager.LoadMap(targetMap);
            yield return null;
            while (MainManager.map == null || MainManager.map.mainmesh == null)
            {
                yield return null;
            }

            if (MainManager.map.mapid == MainManager.Maps.Swamplands5)
            {
                mainLift = MainManager.map.transform.Find("LeafbugVillageWay(Clone)").Find("Lift");
                targetPos = mainLift.transform.position;
                mainLift.transform.position = mainLift.transform.position+Vector3.up*5;
            }
            else
            {
                mainLift = MainManager.map.mainmesh.Find("Lift");
                targetPos = mainLift.transform.position;
                mainLift.transform.position = mainLift.position + Vector3.down * 5;
            }

            yield return DoLift(mainLift, targetPos,false);

            MainManager.ShakeScreen(0.1f, 0.5f);
            MainManager.PlaySound("WoodEnd");
            MainManager.ResetCamera(true);
            yield return EventControl.tenthsec;
        }

        IEnumerator DoLift(Transform mainLift, Vector3 targetPos, bool start)
        {
            Transform plaftform = mainLift.Find("RoachLift");
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].entity.transform.parent = plaftform;
                MainManager.instance.playerdata[i].entity.LockRigid(true);
                MainManager.instance.playerdata[i].entity.ccol.enabled = false;
            }
            MainManager.player.transform.localPosition = new Vector3(0f, 0f, 0.65f);
            SpinAround s = plaftform.GetChild(0).gameObject.AddComponent<SpinAround>();
            s.itself = new Vector3(0f, 7f);
            MainManager.TeleportFollowers(MainManager.TPDir.Center, MainManager.player.transform.position);

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = MainManager.player.transform.position + new Vector3(0f, 0f, 0.35f);
                MainManager.map.chompy.transform.parent = plaftform;
            }
            MainManager.ResetCamera(true);

            if (start)
            {
                MainManager.ShakeScreen(0.2f, 0.75f);
                yield return EventControl.sec;
                MainManager.PlaySound("ElevatorStart");
            }

            Vector3 startPos = mainLift.transform.position;
            float a = 0f;
            float b = 100f;
            bool half = false;
            do
            {
                mainLift.transform.position = Vector3.Lerp(startPos, targetPos, a / b);
                if (!half && a / b > 0.5f)
                {
                    half = true;
                    if (start)
                    {
                        MainManager.FadeIn();
                    }
                    else
                    {
                        MainManager.FadeOut();
                    }
                }
                a += MainManager.framestep;
                yield return null;
            }
            while (a < b + 1f);

            for (int j = 0; j < MainManager.instance.playerdata.Length; j++)
            {
                MainManager.instance.playerdata[j].entity.transform.parent = null;
                MainManager.instance.playerdata[j].entity.LockRigid(false);
                MainManager.instance.playerdata[j].entity.ccol.enabled = true;
            }
            UnityEngine.Object.Destroy(s);
        }


    }
}

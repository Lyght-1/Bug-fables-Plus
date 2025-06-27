using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace BFPlus.Extensions.Events.HoaxeIntermissionEvents
{
    public class HoaxeIntermission7Event : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            bool calledFromCrown = MainManager.map.mapid == MainManager.Maps.BugariaMainPlaza;
            if (calledFromCrown)
            {
                MainManager.FadeIn();
                MainManager.FadeMusic(0.02f);
                yield return EventControl.sec;
                MainManager.instance.insideid = -1;
            }

            MainManager.instance.flags[948] = MainManager.map.mapid == MainManager.Maps.BugariaEndThrone;

            MainManager.instance.flags[916] = true;
            yield return EventControl.tenthsec;

            MainManager.LoadMap((int)MainManager.Maps.GiantLairSaplingPlains);
            yield return null;

            EntityControl tree = MainManager.GetEntity(9);
            MainManager.SetCamera(null, tree.transform.position, 1, new Vector3(0, 3f, -10f), new Vector3(0, 20, 0));
            MainManager.GetEntity(10).gameObject.SetActive(false);

            MainManager.ChangeMusic(NewMusic.HoaxeSadTheme.ToString());

            MainManager.FadeOut(0.01f);

            var temp = Resources.Load<GameObject>("prefabs/maps/GiantLairEntrance").transform.GetChild(1).gameObject;

            var particle = UnityEngine.Object.Instantiate(temp,MainManager.map.transform);
            particle.transform.position = tree.transform.position;
            particle.transform.localEulerAngles = Vector3.zero;

            MainManager.instance.flags[952] = true;

            if (!MainManager.instance.librarystuff[3, (int)NewAchievement.NothingButAHoaxe])
            {
                MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.NothingButAHoaxe);
            }

            yield return new WaitForSeconds(10);

            if (MainManager.instance.flags[63] && !MainManager.instance.flags[949])
            {
                MainManager.instance.flags[949] = true;
                MainManager.instance.flags[950] = true;
                //dropped medal anim

                EntityControl.CreateItem(tree.transform.position + new Vector3(0, 2, -1), 2, (int)Medal.EverlastingFlame, Vector3.zero, -1);
                yield return EventControl.sec;
            }

            ButtonSprite button = new GameObject().AddComponent<ButtonSprite>().SetUp(4, -1, "", new Vector3(7f, -3f, 10f), Vector3.one, 1, MainManager.GUICamera.transform);
            
            while (!MainManager.GetKey(4))
            {
                yield return null;
            }

            UnityEngine.Object.Destroy(button.gameObject);

            MainManager.FadeIn(0.1f);
            MainManager.FadeMusic(0.05f);
            yield return EventControl.sec;

            if (!calledFromCrown)
            {
                MainManager.instance.flags[947] = true;
            }

            yield return EndIntermissionPostgame(instance, 204, (int)MainManager.Maps.BugariaTheater);
        }
    }
}

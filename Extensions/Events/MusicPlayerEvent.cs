using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events
{
    public class MusicPlayerEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }

            if(MainManager.instance.inmusicrange != -1)
            {
                instance.StartCoroutine(MainManager.SetText(MainManager.commondialogue[207], null, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                yield break;
            }

            yield return EventControl.tenthsec;

            MainManager.FixSamira();
            instance.StartCoroutine(MainManager.SetText(MainManager.commondialogue[206], null, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            int play = -1;

            if (!MainManager.listcanceled)
            {
                play = MainManager.instance.samiramusics[MainManager.listoption + ((!MainManager.listvar.Contains(-1)) ? 0 : -1)][0];
            }
            else
            {
                MainManager_Ext.Instance.musicPlayer = false;
                MainManager.ChangeMusic(MainManager.map.music[MainManager.map.musicid]);

                if (MainManager.player.transform.Find("MusicSimple(Clone)") != null)
                {
                    UnityEngine.Object.Destroy(MainManager.player.transform.Find("MusicSimple(Clone)").gameObject);
                }
            }

            if (play >= 0)
            {
                AudioClip tclip = MainManager.music[0].clip;

                MainManager_Ext.Instance.musicPlayer = false;
                yield return MainManager_Ext.LoadNewMusicAsync(play);
                yield return null;
                if (tclip != null && tclip != MainManager.map.music[0])
                {
                    Resources.UnloadAsset(tclip);
                }

                MainManager_Ext.Instance.musicPlayer = true;
                MainManager_Ext.CreateMusicParticle();
            }
            yield return null;
        }
    }
}

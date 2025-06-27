using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Extensions.Events
{
    public class LeafbugCookingEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            EntityControl[] leafbugs = MainManager.GetEntities(new int[] {25,26,27});
            bool newrecipe = false;
            while (MainManager.instance.message)
            {
                yield return null;
            }

            instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[35], true, Vector3.zero, caller.transform, caller));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            if (MainManager.instance.option == 0)
            {
                MainManager.SaveCameraPosition(true);
                yield return EventControl.tenthsec;
                SpriteRenderer[] items = MainManager.map.mainmesh.Find("Campfire").GetComponentsInChildren<SpriteRenderer>();
                int[] ids = new int[] { -1, -1 };

                for (int i = 0; i < ids.Length; i++)
                {
                    MainManager.listcanceled = false;
                    instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[32], true, Vector3.zero, caller.transform, caller));
                    yield return null;

                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }

                    if (MainManager.listcanceled || MainManager.instance.flagvar[0] == -1)
                    {
                        if(i == 0)
                        {
                            yield return null;
                            yield break;
                        }
                    }
                    else
                    {
                        ids[i] = MainManager.instance.flagvar[0];
                        items[i].sprite = MainManager.itemsprites[0, ids[i]];
                        MainManager.instance.items[0].Remove(ids[i]);
                        yield return EventControl.halfsec;
                    }
                }
                instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[(ids[1] == -1) ? 34 : 33].Replace("@VAR1@", "|color,1|" + MainManager.itemdata[0, ids[0], 0] + "|color,0|").Replace("@VAR2@", (ids[1] > -1) ? ("|color,1|" + MainManager.itemdata[0, ids[1], 0] + "|color,0|") : "") + "|prompt,yesno,-11,-11|", true, Vector3.zero, caller.transform, caller));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                if (MainManager.instance.option == 1)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        if (ids[i] > -1)
                        {
                            MainManager.instance.items[0].Add(ids[i]);
                        }
                        items[i].sprite = null;
                    }
                }
                else
                {
                    MainManager.SetCamera(items[2].transform.position,0.04f);
                    int cookedItem = MainManager.MixIngredients(ids[0], ids[1]);
                    for (int l = 0; l < MainManager.librarylimit[2]; l++)
                    {
                        if (MainManager.libraryorder[2, l] == cookedItem)
                        {
                            newrecipe = !MainManager.instance.librarystuff[2, l];
                            MainManager.instance.librarystuff[2, l] = true;
                            break;
                        }
                    }
                    if (cookedItem == 87 || cookedItem == 32 || cookedItem == 86)
                    {
                        cookedItem = 8;
                    }

                    for (int i = 0; i < ids.Length; i++)
                    {
                        items[i].sprite = null;
                    }
                    bool mistake = cookedItem == 8;
                    MainManager.PlaySound("Cook" + (mistake ? 1 : 0), 5, Time.timeScale, 1f);
                    MainManager.PlaySound("Bubbling", 6, Time.timeScale, 1f);
                    GameObject stirParticles = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Stirr"), items[2].transform.position, Quaternion.Euler(-90f, 0f, 0f)) as GameObject;

                    yield return EventControl.tenthsec;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j< leafbugs.Length; j++)
                        {
                            leafbugs[j].PlaySound("Jump");
                            leafbugs[j].Jump();
                            yield return EventControl.tenthsec;
                        }
                        yield return EventControl.halfsec;
                    }

                    if (mistake)
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Mistake"), stirParticles.transform.position, Quaternion.Euler(-90f, 0f, 0f)) as GameObject, 1.5f);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Heart"), stirParticles.transform.position, Quaternion.Euler(-90f, 0f, 0f)) as GameObject, 1.5f);
                    }
                    UnityEngine.Object.Destroy(stirParticles.gameObject);
                    items[2].sprite = MainManager.itemsprites[0, cookedItem];
                    yield return EventControl.sec;
                    items[2].sprite = null;

                    MainManager.events.GiveItem(0, cookedItem);

                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }

                    for (int i = 0; i < MainManager.librarylimit[2]; i++)
                    {
                        if (MainManager.libraryorder[2, i] == MainManager.instance.flagvar[0])
                        {
                            newrecipe = !MainManager.instance.librarystuff[2, i];
                            MainManager.instance.librarystuff[2, i] = true;
                            break;
                        }
                    }

                    MainManager.ResetCamera();
                }

                for (int i = 0; i < items.Length; i++)
                {
                    items[i].sprite = null;
                }
            }

            if (newrecipe)
            {
                MainManager.UpdateJounal();
            }
            yield return EventControl.tenthsec;
        }
    }
}

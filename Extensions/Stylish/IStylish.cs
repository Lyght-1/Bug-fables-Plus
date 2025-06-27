using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Extensions.Stylish
{
    public interface IStylish
    {
        IEnumerator DoStylish(int actionid, int stylishID);
    }

    public class StylishUtils
    {
        public static bool CheckStylish(ref bool failedStylish,EntityControl entity, float time, float antiSpamFrames)
        {
            if (time < antiSpamFrames)
            {
                if (time > 3f)
                {
                    if (MainManager.GetKey(4, false))
                    {
                        failedStylish = true;
                        return false;
                    }
                }
            }
            else if (!failedStylish)
            {
                if(MainManager.BadgeIsEquipped((int)Medal.TimingTutor))
                    entity.Emoticon(MainManager.Emoticons.Exclamation);
                if (MainManager.GetKey(4, false))
                {
                    entity.Emoticon(MainManager.Emoticons.None);
                    return true;
                }
            }
            return false;
        }


        public static void ShowStylish(float pitch, EntityControl entity, float stylishIncrease = 0.1f)
        {
            MainManager.PlaySound("AtkSuccess", pitch, 1);
            MainManager.battle.StartCoroutine(BattleControl_Ext.Instance.ShowStylishMessage(entity));
            MainManager.battle.StartCoroutine(BattleControl_Ext.Instance.IncreaseStylishBar(stylishIncrease,entity));
        }
    }
}

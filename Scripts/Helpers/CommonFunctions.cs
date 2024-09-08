using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    class CommonFunctions
    {
        public static bool IsHebrewText(string text)
        {
            foreach (char c in text)
            {
                if (c >= 0x0590 && c <= 0x05FF)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetMoneyString() =>
              SettingsModel.Instance.Language == LanguageCode.HE
              ? new string(GameManager.Instance.MoneyAmount.ToString().Reverse().ToArray())
              : GameManager.Instance.MoneyAmount.ToString();


    }
}

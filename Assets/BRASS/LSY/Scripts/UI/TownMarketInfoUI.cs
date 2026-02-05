using UnityEngine;

namespace Team1
{
    public class TownMarketInfoUI : MonoBehaviour
    {
        public GameObject lPotion;
        public GameObject Core;


        public void Awake()
        {
            Core.SetActive(false);
            lPotion.SetActive(false);
        }

        public void LPotionInfo()
        {
            AllFalse();
            lPotion.SetActive(true);
        }
        public void CoreInfo()
        {
            AllFalse();
            Core.SetActive(true);
        }

        public void AllFalse()
        {
            Core.SetActive(false);
            lPotion.SetActive(false);
        }
    }
}
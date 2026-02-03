using UnityEngine;

namespace Team1
{
    public class MarketInfoUI : MonoBehaviour
    {
        public GameObject sPotion;
        public GameObject mPotion;
        public GameObject lPotion;

        public void Awake()
        {
            sPotion.SetActive(false);
            mPotion.SetActive(false);
            lPotion.SetActive(false);
        }

        public void SPotionInfo()
        {
            AllFalse();
            sPotion.SetActive(true);
        }
        public void MPotionInfo()
        {
            AllFalse();
            mPotion.SetActive(true);
        }
        public void LPotionInfo()
        {
            AllFalse();
            lPotion.SetActive(true);
        }

        public void AllFalse()
        {
            sPotion.SetActive(false);
            mPotion.SetActive(false);
            lPotion.SetActive(false);
        }

    }
}
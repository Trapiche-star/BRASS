using UnityEngine;

namespace Team1
{
    public class MarketInfoUI : MonoBehaviour
    {
        public GameObject sPotion;
        public GameObject mPotion;

        public void Awake()
        {
            sPotion.SetActive(false);
            mPotion.SetActive(false);
        }

        public void SPotionInfo()
        {
            sPotion.SetActive(true);
        }
        public void MPotionInfo()
        {
            mPotion.SetActive(true);
        }

    }
}
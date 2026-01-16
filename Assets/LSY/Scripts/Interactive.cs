/*using TMPro;
using UnityEngine;

namespace Team1
{
    /// <summary>
    /// 인터랙티브 오브젝트 관리하는 클래스들의 부모 추상 클래스
    /// </summary>
    public abstract class Interactive : MonoBehaviour
    {
        //추상 메서드
        #region abstract
        protected abstract void DoAction();
        #endregion

        #region Variables
        //참조
        protected BoxCollider collider;

        //인터랙티브 UI
        [Header("Interactive UI")]
        //크로스헤어
        public GameObject extraCross;

        //액션 UI
        public GameObject actionUI;
        public TextMeshProUGUI actionText;

        [SerializeField]
        protected string action = "Do Action";
        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            //참조
            collider = GetComponent<BoxCollider>();

            //Action UI가 널이면 Find 게임오브젝트로 참조 찾기
            *//*if(extraCross == null)
            {
                extraCross = GameObject.Find("ExtraCross");
                actionUI = GameObject.Find("ActionUI");
                actionText = GameObject.Find("ActionText").GetComponent<TextMeshProUGUI>();
            }*//*
        }

        protected virtual void OnMouseOver()
        {
            //일정거리 이상되면 UI 숨김
            if (PlayerCasting.distanceFromTarget > 2f)
            {
                HideActionUI();
                return;
            }

            ShowActionUI();

            //만약 Action 버튼을 누르면
            if (Input.GetButtonDown("Action"))
            {
                //충돌체 제거
                collider.enabled = false;

                //액션 UI 감추기
                HideActionUI();

                //"Do Action" - 인터랙트브 액션
                DoAction();
            }
        }

        protected virtual void OnMouseExit()
        {
            HideActionUI();
        }
        #endregion

        #region Custom Method
        protected virtual void ShowActionUI()
        {
            extraCross.SetActive(true);
            actionUI.SetActive(true);
            actionText.text = action;
        }

        protected virtual void HideActionUI()
        {
            extraCross.SetActive(false);
            actionUI.SetActive(false);
            actionText.text = "";
        }
        #endregion
    }
}*/
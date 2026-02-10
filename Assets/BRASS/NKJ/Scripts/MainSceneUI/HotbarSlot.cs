using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Linq;
using Team1;

public class HotbarSlot : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public Image iconImage;
    public Image cooldownImage;
    public TextMeshProUGUI costText;

    [Header("Slot Data")]
    private SlotContentType contentType = SlotContentType.Empty;
    private SkillData skillData;
    private ConsumableItem consumableItem;  // ✅ 추상 클래스 참조

    private bool isCooldown = false;

    private enum SlotContentType { Empty, Skill, Item }

    void Start()
    {
        ClearSlot();
    }

    // ✅ 드래그된 아이템을 받는 함수
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        // 1. 인벤토리 아이템인지 확인
        var itemDrag = droppedObject.GetComponent<InventoryItemDragHandler>();
        if (itemDrag != null && itemDrag.item != null)
        {
            SetItem(itemDrag.item);
            Debug.Log($"핫바에 아이템 등록: {itemDrag.item.ItemName}");
            return; // 등록 성공 시 종료
        }

        // 2. 스킬 슬롯인지 확인 (SkillSlot에 Drag 기능이 있다고 가정)
        var skillDrag = droppedObject.GetComponent<SkillSlot>();
        if (skillDrag != null && skillDrag.skillData != null)
        {
            SetSkill(skillDrag.skillData);
            Debug.Log($"핫바에 스킬 등록: {skillDrag.skillData.skillName}");
            return;
        }
    }

    // 스킬 설정
    public void SetSkill(SkillData data)
    {
        contentType = SlotContentType.Skill;
        skillData = data;
        consumableItem = null;

        iconImage.sprite = data.icon;
        iconImage.enabled = true;
        costText.text = data.mpCost.ToString();

        if (cooldownImage != null) cooldownImage.fillAmount = 0;
    }

    // 아이템 설정
    public void SetItem(ConsumableItem item)
    {
        contentType = SlotContentType.Item;
        consumableItem = item;
        skillData = null;

        iconImage.sprite = item.Icon;
        iconImage.enabled = true;

        UpdateItemCount();

        if (cooldownImage != null) cooldownImage.fillAmount = 0;
    }

    // 인벤토리 수량 업데이트
    private void UpdateItemCount()
    {
        if (consumableItem == null) return;

        var inventory = FindFirstObjectByType<Inventory>();
        if (inventory == null) return;

        var slot = inventory.Slots.FirstOrDefault(s => s.Item.ItemName == consumableItem.ItemName);

        if (slot != null && slot.Count > 0)
        {
            costText.text = slot.Count.ToString();
        }
        else
        {
            ClearSlot();
        }
    }

    void Update()
    {
        if (contentType == SlotContentType.Item)
        {
            UpdateItemCount();
        }
    }

    // 슬롯 사용
    public void UseSlot()
    {
        if (isCooldown) return;

        switch (contentType)
        {
            case SlotContentType.Skill:
                UseSkill();
                break;

            case SlotContentType.Item:
                UseItem();
                break;

            case SlotContentType.Empty:
                Debug.Log("빈 슬롯입니다.");
                break;
        }
    }

    private void UseSkill()
    {
        if (skillData == null) return;

        Debug.Log($"{skillData.skillName} 사용!");
        StartCoroutine(CooldownRoutine(skillData.cooldown));
    }

    private void UseItem()
    {
        if (consumableItem == null) return;

        var inventory = FindFirstObjectByType<Inventory>();
        if (inventory == null) return;

        var slot = inventory.Slots.FirstOrDefault(s => s.Item.ItemName == consumableItem.ItemName);

        if (slot == null || slot.Count <= 0)
        {
            Debug.Log($"{consumableItem.ItemName}이(가) 인벤토리에 없습니다!");
            ClearSlot();
            return;
        }

        // 아이템 사용 효과 발동
        consumableItem.Use(GameObject.FindGameObjectWithTag("Player"));
        Debug.Log($"✅ {consumableItem.ItemName} 사용!");

        // 인벤토리에서 제거
        slot.RemoveOne();
        if (slot.Count <= 0)
        {
            inventory.Slots.Remove(slot);
        }

        // ✅ OnInventoryChanged 이벤트 발생 (안전한 방식)
        //inventory.OnInventoryChanged?.Invoke();
        // 대체 코드:
        var inventoryUI = FindFirstObjectByType<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.SendMessage("Refresh", SendMessageOptions.DontRequireReceiver);
        }

        StartCoroutine(CooldownRoutine(0.5f));
    }

    private IEnumerator CooldownRoutine(float cooldownTime)
    {
        isCooldown = true;
        float timer = cooldownTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = timer / cooldownTime;
            }
            yield return null;
        }

        isCooldown = false;
        if (cooldownImage != null) cooldownImage.fillAmount = 0;
    }

    public void ClearSlot()
    {
        contentType = SlotContentType.Empty;
        skillData = null;
        consumableItem = null;

        iconImage.sprite = null;
        iconImage.enabled = false;
        costText.text = "";

        if (cooldownImage != null) cooldownImage.fillAmount = 0;
    }
}
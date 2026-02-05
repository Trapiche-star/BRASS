using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quest system test helper script (with defense code)
/// </summary>
public class QuestTester : MonoBehaviour
{
    [Header("Test Settings")]
    public int testQuestID = 1;
    public int progressAmount = 1;

    [Header("Test Shortcuts - Q + Number")]
    private bool qKeyHeld = false;

    void Update()
    {
        try
        {
            // Q키 상태 체크
            qKeyHeld = Keyboard.current != null && Keyboard.current.qKey.isPressed;

            // Test shortcuts: Q + 0/1/2/3/4
            if (qKeyHeld)
            {
                // Q + 0: Open detail popup
                if (Keyboard.current.digit0Key.wasPressedThisFrame)
                {
                    TestShowDetailPopup();
                }

                // Q + 1: Open accept popup
                if (Keyboard.current.digit1Key.wasPressedThisFrame)
                {
                    TestShowAcceptPopup();
                }

                // Q + 2: Update progress
                if (Keyboard.current.digit2Key.wasPressedThisFrame)
                {
                    TestUpdateProgress();
                }

                // Q + 3: Complete quest
                if (Keyboard.current.digit3Key.wasPressedThisFrame)
                {
                    TestCompleteQuest();
                }

                // Q + 4: Clear save data
                if (Keyboard.current.digit4Key.wasPressedThisFrame)
                {
                    TestClearSave();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] Update error: {e.Message}");
        }
    }

    [ContextMenu("Show Detail Popup")]
    public void TestShowDetailPopup()
    {
        try
        {
            if (QuestManager.Instance != null && QuestManager.Instance.questDetailPopup != null)
            {
                QuestManager.Instance.questDetailPopup.SetActive(true);
                Debug.Log($"[TEST] Detail popup opened");
            }
            else
            {
                Debug.LogError("[TEST] QuestManager.Instance or questDetailPopup is null!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] TestShowDetailPopup error: {e.Message}");
        }
    }

    [ContextMenu("Show Accept Popup")]
    public void TestShowAcceptPopup()
    {
        try
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.ShowAcceptPopup(testQuestID);
                Debug.Log($"[TEST] Quest {testQuestID} accept popup opened");
            }
            else
            {
                Debug.LogError("[TEST] QuestManager.Instance is null!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] TestShowAcceptPopup error: {e.Message}");
        }
    }

    [ContextMenu("Update Quest Progress")]
    public void TestUpdateProgress()
    {
        try
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.UpdateQuestProgress(testQuestID, progressAmount);
                Debug.Log($"[TEST] Quest {testQuestID} progress +{progressAmount}");
            }
            else
            {
                Debug.LogError("[TEST] QuestManager.Instance is null!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] TestUpdateProgress error: {e.Message}");
        }
    }

    [ContextMenu("Complete Quest")]
    public void TestCompleteQuest()
    {
        try
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.CompleteQuest(testQuestID);
                Debug.Log($"[TEST] Quest {testQuestID} completion attempted");
            }
            else
            {
                Debug.LogError("[TEST] QuestManager.Instance is null!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] TestCompleteQuest error: {e.Message}");
        }
    }

    [ContextMenu("Clear Save Data")]
    public void TestClearSave()
    {
        try
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.ClearSaveData();
                Debug.Log($"[TEST] Save data cleared");
            }
            else
            {
                Debug.LogError("[TEST] QuestManager.Instance is null!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] TestClearSave error: {e.Message}");
        }
    }

    // Simulate monster kill
    public void SimulateMonsterKill(string monsterName)
    {
        try
        {
            if (QuestManager.Instance == null)
            {
                Debug.LogError("[TEST] QuestManager.Instance is null!");
                return;
            }

            // Check all in-progress quests
            var inProgressQuests = QuestManager.Instance.GetInProgressQuests();

            foreach (var quest in inProgressQuests)
            {
                // If Kill type and target name matches
                if (quest.questType == QuestType.Kill && quest.targetName == monsterName)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.questID, 1);
                    Debug.Log($"[TEST] {monsterName} killed! Quest '{quest.questName}' progress updated");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] SimulateMonsterKill error: {e.Message}");
        }
    }

    // Simulate item collection
    public void SimulateItemCollect(string itemName, int amount = 1)
    {
        try
        {
            if (QuestManager.Instance == null)
            {
                Debug.LogError("[TEST] QuestManager.Instance is null!");
                return;
            }

            var inProgressQuests = QuestManager.Instance.GetInProgressQuests();

            foreach (var quest in inProgressQuests)
            {
                if (quest.questType == QuestType.Collect && quest.targetName == itemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.questID, amount);
                    Debug.Log($"[TEST] {itemName} x{amount} collected! Quest '{quest.questName}' progress updated");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuestTester] SimulateItemCollect error: {e.Message}");
        }
    }
}
// Save this file as: Assets/Editor/QuestDatabaseAutoUpdater.cs

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Automatically adds all QuestData ScriptableObjects to QuestManager's allQuestDB
/// </summary>
[InitializeOnLoad]
public class QuestDatabaseAutoUpdater
{
    static QuestDatabaseAutoUpdater()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        // Remove the update callback to run only once
        EditorApplication.update -= OnEditorUpdate;

        // Play 모드에서는 실행 안 함
        if (!Application.isPlaying)
        {
            // Wait a bit for Unity to finish loading
            EditorApplication.delayCall += UpdateQuestDatabase;
        }
    }

    [MenuItem("Tools/Quest System/Update Quest Database")]
    public static void UpdateQuestDatabase()
    {
        // Find all QuestData assets
        string[] guids = AssetDatabase.FindAssets("t:QuestData");
        QuestData[] allQuests = new QuestData[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            allQuests[i] = AssetDatabase.LoadAssetAtPath<QuestData>(path);
        }

        // Sort by questID
        allQuests = allQuests.OrderBy(q => q.questID).ToArray();

        Debug.Log($"[QuestDatabaseAutoUpdater] Found {allQuests.Length} QuestData assets");

        // Find all QuestManager instances in scenes (Unity 2023+ 호환)
#if UNITY_2023_1_OR_NEWER
        QuestManager[] managers = Object.FindObjectsByType<QuestManager>(FindObjectsSortMode.None);
#else
        QuestManager[] managers = Object.FindObjectsOfType<QuestManager>();
#endif

        if (managers.Length == 0)
        {
            Debug.LogWarning("[QuestDatabaseAutoUpdater] No QuestManager found in scene. Please add QuestManager to your scene first.");
            return;
        }

        foreach (var manager in managers)
        {
            // Update allQuestDB
            SerializedObject so = new SerializedObject(manager);
            SerializedProperty questDBProp = so.FindProperty("allQuestDB");

            questDBProp.ClearArray();

            for (int i = 0; i < allQuests.Length; i++)
            {
                questDBProp.InsertArrayElementAtIndex(i);
                questDBProp.GetArrayElementAtIndex(i).objectReferenceValue = allQuests[i];
            }

            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(manager);

            Debug.Log($"[QuestDatabaseAutoUpdater] Updated {manager.gameObject.name} with {allQuests.Length} quests");
        }

        // Save scene (Play 모드가 아닐 때만)
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }

        Debug.Log("[QuestDatabaseAutoUpdater] Quest database update complete!");
    }

    [MenuItem("Tools/Quest System/List All Quests")]
    public static void ListAllQuests()
    {
        string[] guids = AssetDatabase.FindAssets("t:QuestData");

        Debug.Log($"===== All Quest Data ({guids.Length}) =====");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            QuestData quest = AssetDatabase.LoadAssetAtPath<QuestData>(path);

            if (quest != null)
            {
                Debug.Log($"ID: {quest.questID} | Name: {quest.questName} | Path: {path}");
            }
        }

        Debug.Log("=====================================");
    }
}

/// <summary>
/// Automatically update quest database when QuestData is created/modified
/// </summary>
public class QuestDataPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Play 모드에서는 실행 안 함
        if (Application.isPlaying)
            return;

        bool questDataChanged = false;

        // Check if any QuestData was imported/deleted/moved
        foreach (string str in importedAssets)
        {
            if (str.Contains("QuestData") || str.EndsWith(".asset"))
            {
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(str);
                if (obj is QuestData)
                {
                    questDataChanged = true;
                    break;
                }
            }
        }

        if (!questDataChanged)
        {
            foreach (string str in deletedAssets)
            {
                if (str.Contains("QuestData"))
                {
                    questDataChanged = true;
                    break;
                }
            }
        }

        if (questDataChanged)
        {
            Debug.Log("[QuestDataPostProcessor] QuestData changed, updating database...");
            EditorApplication.delayCall += QuestDatabaseAutoUpdater.UpdateQuestDatabase;
        }
    }
}
#endif
using UnityEngine;
using System.IO; // 파일 입출력 필수

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // 어디서든 SaveManager.Instance로 접근 가능
    public GameData gameData = new GameData();

    private string savePath;

    private void Awake()
    {
        // 싱글톤 설정: 씬이 바뀌어도 파괴되지 않음
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "savefile.json");
            LoadGame(); // 게임 시작 시 자동 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 데이터를 파일로 저장
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("저장 완료: " + savePath);
    }

    // 파일에서 데이터를 읽어옴
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("데이터 로드 완료");
        }
        else
        {
            Debug.Log("저장된 파일이 없습니다. 새로 시작합니다.");
        }
    }
}
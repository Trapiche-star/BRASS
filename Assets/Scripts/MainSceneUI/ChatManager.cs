using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    public TMP_Text chatDisplay;
    public TMP_InputField inputField;

    // 메시지 데이터 구조
    struct ChatMessage { public string category; public string text; }
    List<ChatMessage> allMessages = new List<ChatMessage>();

    public void SendChat()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;
        AddMessage("General", inputField.text);
        inputField.text = "";
    }

    public void AddMessage(string category, string message)
    {
        allMessages.Add(new ChatMessage { category = category, text = message });
        RefreshChat("General"); // 기본적으로 전체 보기
    }

    public void RefreshChat(string filter)
    {
        chatDisplay.text = "";
        foreach (var msg in allMessages)
        {
            if (filter == "General" || msg.category == filter)
                chatDisplay.text += $"[{msg.category}] {msg.text}\n";
        }
    }
}
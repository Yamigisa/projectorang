using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace game
{
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button submitButton;
    
    [SerializeField] private GameObject buttonsObject;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject joinPanel;

    [SerializeField] private TMP_Text codeText;

    void OnEnable()
    {
        createButton.onClick.AddListener(CreateLobby);
        joinButton.onClick.AddListener(JoinLobby);
        submitButton.onClick.AddListener(SubmitCode);
    }

    void OnDisable()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
        submitButton.onClick.RemoveAllListeners();
    }

    private async void CreateLobby()
    {
        bool succeeded = await GameLobbyManager.instance.CreateLobby();
        if(succeeded)
        {
            buttonsObject.SetActive(false);
            lobbyPanel.SetActive(true);
        }
    }

    private void JoinLobby()
    {
        joinPanel.SetActive(true);
        buttonsObject.SetActive(false);
    }

    private async void SubmitCode()
    {
        string code = codeText.text;
        code = code.Substring(0, code.Length - 1);

        bool succeeded = await GameLobbyManager.instance.JoinLobby(code);
        if(succeeded)
        {
            joinPanel.SetActive(false);
            lobbyPanel.SetActive(true);
        }
    }
}
}
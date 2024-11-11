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
        [SerializeField] private Button backButton;
        
        [SerializeField] private GameObject buttonsObject;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject joinPanel;

        [SerializeField] private TMP_Text codeText;

        private void Start()
        {
            createButton.onClick.AddListener(CreateLobby);
            joinButton.onClick.AddListener(ActivateLobby);
            submitButton.onClick.AddListener(SubmitCode);
            backButton.onClick.AddListener(BackToMainMenu);
        }

        private void OnDisable()
        {
            createButton.onClick.RemoveAllListeners();
            joinButton.onClick.RemoveAllListeners();
            submitButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        private async void CreateLobby()
        {
            bool succeeded = await GameLobbyManager.instance.CreateLobby();
            if (succeeded)
            {
                ShowLobbyPanel();
            }
        }

        private void ShowLobbyPanel()
        {
            buttonsObject.SetActive(false);
            lobbyPanel.SetActive(true);
        }

        private async void SubmitCode()
        {
            string code = codeText.text;
            code = code.Substring(0, code.Length - 1);

            bool succeeded = await GameLobbyManager.instance.JoinLobby(code);
            if (succeeded)
            {
                ShowLobbyPanelAfterJoining();
            }
        }

        private void ShowLobbyPanelAfterJoining()
        {
            joinPanel.SetActive(false);
            lobbyPanel.SetActive(true);
        }

        private void ActivateLobby()
        {
            joinPanel.SetActive(true);
            buttonsObject.SetActive(false);
        }

        private void BackToMainMenu()
        {
            joinPanel.SetActive(false);
            buttonsObject.SetActive(true);
        }
    }
}

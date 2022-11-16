using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;



public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;  
    [SerializeField] private TextMeshProUGUI playersInGameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TMP_InputField lobbyCodeInput;

    private void Awake()
    {
        Cursor.visible = true;
    } 

    private void Update()
    {
       //playersInGameText.text = $"Players in game: {PlayerManager.instance.PlayersInGame}";
    }

    private void Start()
    {
        startHostButton.onClick.AddListener(OnHostClicked);

        startServerButton.onClick.AddListener(OnSeverClicked);

        startClientButton.onClick.AddListener(OnClientClicked);
    }

    private async void OnHostClicked()
    {
        await GameLobbyManager.Instance.CreateLobby();
        lobbyCodeText.text += $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";
    }

    private async void OnSeverClicked()
    {
        await GameLobbyManager.Instance.CreateLobby();
        lobbyCodeText.text += $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";
    }
    
    private async void OnClientClicked()
    {
        string code = lobbyCodeInput.text;
        code = code.Substring(0, code.Length -1);
        await GameLobbyManager.Instance.JoinLobby(code);
    }
}

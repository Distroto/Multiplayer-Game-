using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distroto.Manager; 
using UnityEngine;
using Distroto.Core;
using Distroto.Core.Singletons;

public class GameLobbyManager : Singleton<GameLobbyManager>
{
    public async Task<bool> CreateLobby()
    {
        Dictionary<string, string> playerData = new Dictionary<string, string>()
        {
            {"PlayerName", "HostPlayer"}
        };

        bool succeded = await LobbyManager.Instance.CreateLobby(maxPlayers:4, isPrivate:true, playerData);
        return succeded;
    }

    public async Task<bool> JoinLobby(string code)
    {
        Dictionary<string, string> playerData = new Dictionary<string, string>()
        {
            {"PlayerName", "JoinPlayer"}
        };

        bool succeded = await LobbyManager.Instance.JoinLobby(code, playerData);
        return succeded;

    }

    public string GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Distroto.Core.Singletons;

namespace Distroto.Manager
{
    public class LobbyManager : Distroto.Core.Singletons.Singleton<LobbyManager>
    {
        private Lobby _lobby;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _refreshLobbyCoroutine;

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode; 
        }

        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = isPrivate,
                Player = player,
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
            }
            catch(System.Exception)
            {
                return false;
            }
            
            Debug.Log(message: $"Lobby Created: {_lobby.Id}");
            
            _heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id, 6f));

            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
            
            return true;
        }

        private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSec)
        {
            while(true)
            {
                Debug.Log(message:"Heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSeconds(waitTimeSec);
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSec)
        {
            while(true)
            {
               Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
               yield return new WaitUntil(() => task.IsCompleted);
               Lobby newLobby = task.Result;
               if(newLobby.LastUpdated > _lobby.LastUpdated)
               {
                    _lobby = newLobby;
               }
               yield return new WaitForSeconds(waitTimeSec);
            }
        }
        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();

            foreach (KeyValuePair<string, string> entry in data)
            {
                playerData.Add(entry.Key, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,
                    entry.Value));
            }
            return playerData; 
        }

        public void OnApplicationQuit()
        {
            if(_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            } 
        }

        public async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData));

            options.Player = player;

            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            } 
            catch(System.Exception)
            {
                return false;
            }

            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
            return true;            
        }

    }
}
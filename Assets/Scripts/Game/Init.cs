using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;


public class Init : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();
        if(UnityServices.State == ServicesInitializationState.Initialized)
        {
            AuthenticationService.Instance.SignedIn += OnSignedIn;
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void OnSignedIn()
    {
        Debug.Log(message: $"Player Id:{AuthenticationService.Instance.PlayerId}");
        Debug.Log(message: $"TOken: {AuthenticationService.Instance.AccessToken}");
    }

}

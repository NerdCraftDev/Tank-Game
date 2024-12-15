using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject parentObj;
    public Canvas canvas;
    public Canvas lobbyCanvas;
    
    async void Awake() {
        await UnityServices.InitializeAsync();
        //await RelayService.Instance.CreateAllocationAsync(4);
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
            
            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); 

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async void CreateLobby() {
        string lobbyName = "new lobby";
        int maxPlayers = 4;
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        QueryLobbies();
        ToggleRoomGUI();
        Debug.Log("Lobby Created");
    }

    public async void JoinLobby(string id) {
        try
        {
            await LobbyService.Instance.JoinLobbyByIdAsync(id);
            Debug.Log($"Joined Lobby with ID {id}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        ToggleRoomGUI();
    }

    public async void LeaveLobby() {
        try {
            //string playerID = AuthenticationService.Instance.PlayerId;
            //await LobbyService.Instance.RemovePlayerAsync(lobbyID, playerID);
            ToggleRoomGUI();
            //Debug.Log($"Player with ID {playerID} left Lobby with ID {lobbyID}");
        }
        catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void QueryLobbies() {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(options);
            List<Lobby> lobbies = response.Results;

            foreach(Transform child in parentObj.transform) {
                Destroy(child.gameObject); 
            }
            int i = 1;
            foreach (Lobby lobby in lobbies) {
                GameObject buttonObj = Instantiate(buttonPrefab);
                buttonObj.transform.SetParent(parentObj.transform);
                buttonObj.transform.localScale = Vector3.one;
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1); 
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.anchoredPosition3D = new Vector3(parentObj.GetComponent<RectTransform>().sizeDelta.x/2, -i * 30, 0);
                buttonObj.GetComponent<TextMeshProUGUI>().text = lobby.Name;
                buttonObj.GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby.Id));
                i++;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void ToggleRoomGUI() {
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        lobbyCanvas.gameObject.SetActive(!lobbyCanvas.gameObject.activeSelf);
    }
}
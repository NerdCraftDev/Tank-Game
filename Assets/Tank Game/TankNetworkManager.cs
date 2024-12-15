using Mirror;
using Telepathy;
using UnityEngine;

public class TankNetworkManager : NetworkManager
{
    public GameObject tankPrefab; // Assign the tank prefab in the Unity editor

    public override void OnStartServer()
    {
        spawnPrefabs.Clear();
        spawnPrefabs.Add(tankPrefab);
    }

    public override void OnStartClient()
    {
        spawnPrefabs.Clear();
        spawnPrefabs.Add(tankPrefab);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        GameObject tank = Instantiate(tankPrefab);
        NetworkServer.AddPlayerForConnection(conn, tank);
    }
}
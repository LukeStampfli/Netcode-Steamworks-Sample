using System.Runtime.CompilerServices;
using Netcode.Transports;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class ConnectionHud : MonoBehaviour
{
    NetworkManager m_NetworkManager;

    SteamNetworkingTransport m_Transport;

    GUIStyle m_LabelTextStyle;
    
    string m_ConnectToId = "";

    public Vector2 DrawOffset = new Vector2(10, 10);

    public Color LabelColor = Color.black;

    void Awake()
    {
        // Only cache networking manager but not transport here because transport could change anytime.
        m_NetworkManager = GetComponent<NetworkManager>();
        m_LabelTextStyle = new GUIStyle(GUIStyle.none);
    }

    void OnGUI()
    {
        m_LabelTextStyle.normal.textColor = LabelColor;

        m_Transport = (SteamNetworkingTransport)m_NetworkManager.NetworkConfig.NetworkTransport;

        GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(400, 200)));

        if (IsRunning(m_NetworkManager))
        {
            DrawStatusGUI();
        }
        else
        {
            DrawConnectGUI();
        }

        GUILayout.EndArea();
    }

    void DrawConnectGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Connect to SteamID", m_LabelTextStyle);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        m_ConnectToId = GUILayout.TextField(m_ConnectToId);
        if (ulong.TryParse(m_ConnectToId, out ulong steamId))
        {
            m_Transport.ConnectToSteamID = steamId;
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Host (Server + Client)"))
        {
            m_NetworkManager.StartHost();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Server"))
        {
            m_NetworkManager.StartServer();
        }

        if (GUILayout.Button("Client"))
        {
            m_NetworkManager.StartClient();
        }

        GUILayout.EndHorizontal();
    }

    void DrawStatusGUI()
    {
        if (m_NetworkManager.IsServer)
        {
            var mode = m_NetworkManager.IsHost ? "Host" : "Server";
            GUILayout.Label($"{mode} steamid: {SteamUser.GetSteamID().m_SteamID}", m_LabelTextStyle);
        }
        else
        {
            if (m_NetworkManager.IsConnectedClient)
            {
                GUILayout.Label($"connected to: {m_Transport.ConnectToSteamID}", m_LabelTextStyle);
            }
        }

        if (GUILayout.Button("Shutdown"))
        {
            m_NetworkManager.Shutdown();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsRunning(NetworkManager networkManager) => networkManager.IsServer || networkManager.IsClient;
}
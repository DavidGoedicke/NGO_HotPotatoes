using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;

public class GameStartInterface : MonoBehaviour
{

    public Button StartAsServer;
    public Button StartAsClient;
    public Button StartAsHost;

    public TMP_InputField IP_Address;

    public GameObject ServerCameraPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartAsServer.onClick.AddListener(StartAsServer_); 
        StartAsClient.onClick.AddListener(StartAsClient_);
        StartAsHost.onClick.AddListener(StartAsHost_);
        NetworkManager.Singleton.OnClientDisconnectCallback += Disonnect;
    }

    private void  Disonnect(ulong client)
    {
     Application.Quit();
    }

    void StartAsServer_()
    {
        NetworkManager.Singleton.StartServer();
        var obj = Instantiate(ServerCameraPrefab,Vector3.up*35, Quaternion.Euler(90,0,0));
       
        selfDestruct();
    }
    
      void StartAsHost_()
        {
            NetworkManager.Singleton.StartHost();
         
            selfDestruct();
        }
    
    void StartAsClient_()
    {
       
       

        String tartger = "";
        if (IP_Address.text.Length == 0)
        {
            tartger = "127.0.0.1";
        }
        else
        {
            tartger = IP_Address.text;
        }
         Debug.Log("Trying to connect to:"+tartger);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(tartger,7777);
        
        NetworkManager.Singleton.StartClient();
        selfDestruct();
    }

    void selfDestruct()
    { 
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

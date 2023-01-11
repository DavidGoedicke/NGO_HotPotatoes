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

    public TMP_InputField IP_Address;

    public GameObject ServerCameraPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartAsServer.onClick.AddListener(StartAsServer_); 
        StartAsClient.onClick.AddListener(StartAsClient_); 
    }

    void StartAsServer_()
    {
        NetworkManager.Singleton.StartServer();
        var obj = Instantiate(ServerCameraPrefab,Vector3.zero, Quaternion.Euler(Vector3.down));
       
        selfDestruct();
    }
    
    void StartAsClient_()
    {
       
        Debug.Log("Trying to connect to:"+IP_Address.text);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(IP_Address.text,7777);
        
        NetworkManager.Singleton.StartClient();
        selfDestruct();
    }

    void selfDestruct()
    {
        Destroy(gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

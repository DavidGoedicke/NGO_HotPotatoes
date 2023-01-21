using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThrowAbleObject : NetworkBehaviour
{
    private Vector3 motion;

    public Vector3 Motion
    {
        get { return motion; } // get method
        set { motion = new Vector3(value.x, 0, value.z); } // set method
    }

    private Quaternion rot;

    NetworkObject myNetObj;

   // public delegate void  OnDespawn_Delegate;
   //    public OnDespawn_Delegate DespawnDelegate;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            myNetObj = GetComponent<NetworkObject>();
            myNetObj.RemoveOwnership();
            StartCoroutine( DestroyAfter(5));
            rot = Random.rotation;
        }

        if (IsLocalPlayer)
        {
            // Here wants to be somestuff that will happeln only onlthe local player thing
            
        }
    }

   
    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
     
        NetworkObject.Despawn(true);
       
    }

    public override void OnNetworkDespawn()
    {
        foreach (var localPlayerControlls in FindObjectsOfType<LocalPlayerControlls>())
        {
            localPlayerControlls.RemoteClickableDisconnect(NetworkObjectId);
          
        }
        
        base.OnNetworkDespawn();
        
    }


    /// <summary>
    /// THIS DOES NOT WORK. you need to have ownership somehow 
    /// </summary>
    /// <param name="cliendid"></param>
    [ServerRpc]
    public void ChangeOwnershipServerRPC(ulong cliendid)
    {
        myNetObj.ChangeOwnership(cliendid);
    }
    
   // Once I have owner ship I can control things and also relincquish controll!
    
    [ServerRpc]
    public void SetClickPositionServerRPC(Vector2 vec2)
    {
        Motion =  new Vector3( vec2.x,0,vec2.y )- transform.position;
     
    }

    [ServerRpc]
    public void ResetControllServerRPC()
    {
    
            
            myNetObj.RemoveOwnership();
        
    }



    // Update is called once per frame
    void Update()
    {
        if (IsServer) 
        {
    
            transform.position += motion * Time.deltaTime;
          
        }
    }
}


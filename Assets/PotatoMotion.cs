using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PotatoMotion : NetworkBehaviour
{
    private Vector3 motion;

    public Vector3 Motion
    {
        get { return motion; } // get method
        set { motion = new Vector3(value.x, 0, value.z); } // set method
    }

    private Quaternion rot;


    NetworkObject myNetObj;

    // Start is called before the first frame update
    void Start()
    {
        rot = Random.rotation;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            myNetObj = GetComponent<NetworkObject>();
            myNetObj.RemoveOwnership();
        }
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
            Debug.Log("Got an updated motion vector"+motion.magnitude.ToString());
            transform.position += motion * Time.deltaTime;
           // transform.Rotate(rot.eulerAngles * Time.deltaTime);
        }
    }
}
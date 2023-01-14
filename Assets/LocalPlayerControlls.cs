using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class LocalPlayerControlls : NetworkBehaviour
{
    public GameObject Throable;

    // Start is called before the first frame update
    private Vector3 startPos;

    private ThrowAbleObject lastClikedObject;

    enum clickReturn
    {
        DEFAULT,
        FLOOR,
        POTATO,
        NONE
    }

    clickReturn lastClickedItem = clickReturn.NONE;

    private Transform CameraTransform;

    void Start()
    {
        if (IsLocalPlayer)
        {
            CameraTransform = transform.GetComponentInChildren<Camera>().transform;
        }
        else
        {
            GetComponentInChildren<Camera>().enabled = false;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
           
           // FindObjectOfType<CinemachineTargetGroup>().AddMember(transform,1,5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;

        #region movment

        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.A))
        {
            pos.x -= 0.1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            pos.x += 0.1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            pos.z -= 0.1f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            pos.z += 0.1f;
        }


        CameraTransform.Translate(0, 0, Input.mouseScrollDelta.y);

        transform.position = pos;

        #endregion

        #region thorwing

        if (Input.GetMouseButtonDown(0))
        {
            lastClickedItem = GetHitPoint(out pos, out Transform netObj);
            startPos = pos;

            if (lastClickedItem == clickReturn.POTATO)
            {
                lastClikedObject = netObj.GetComponent<ThrowAbleObject>();


                if (lastClikedObject.IsOwnedByServer)
                {
                    AttemptToGrabObject(lastClikedObject.NetworkObjectId);
                    lastClickedItem = clickReturn.POTATO;
                }
                else
                {
                    lastClickedItem = clickReturn.NONE;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("mouseUp");
            var tmp = GetHitPoint(out pos, out Transform netObj);

            if (lastClickedItem == clickReturn.POTATO) // letting go of a potato
            {
                lastClikedObject.ResetControllServerRPC();
                lastClikedObject = null;
                lastClickedItem = clickReturn.NONE;
            }
            else if (tmp == clickReturn.FLOOR)
            {
                SpawnObject(startPos, pos - startPos);
                Debug.Log("Try To Spawn Object");
            }
        }


        if (lastClickedItem == clickReturn.POTATO && lastClikedObject != null &&  lastClikedObject.IsOwner)
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 6;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                lastClikedObject.SetClickPositionServerRPC(new Vector2(hit.point.x, hit.point.z));
            }
        }

        #endregion
    }

    private void SpawnObject(Vector3 start, Vector3 dir)
    {
        SpawnThrowableServerRPC(start, dir);
    }
    
    [ClientRpc]
    public void RemoteClickableDisconnectClientRPC(ulong clientID)
    {
        if (clientID == OwnerClientId)
        {
            lastClikedObject = null;
            lastClickedItem = clickReturn.NONE;
        }
        
    }
    [ServerRpc]
    private void SpawnThrowableServerRPC(Vector3 start, Vector3 dir)
    {
   //     Debug.Log("Try To Spawn Object");

        GameObject go = Instantiate(Throable
            , new Vector3(start.x,2,start.z) , Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<ThrowAbleObject>().Motion = dir;
    }

    private void AttemptToGrabObject(ulong NetObjID)
    {
        AttemptToGrabServerRPC(OwnerClientId, NetObjID);
    }

    [ServerRpc]
    private void AttemptToGrabServerRPC(ulong Clientid, ulong NetObjID)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[NetObjID].ChangeOwnership(Clientid);
    }


    private clickReturn GetHitPoint(out Vector3 point, out Transform netObj)
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            point = hit.point;

            netObj = hit.transform;
            // Debug.DrawLine(ray.origin, hit.point, Color.yellow,15);

            if (hit.transform.tag == "floor")
            {
                return clickReturn.FLOOR;
            }

            if (hit.transform.tag == "potato")
            {
                return clickReturn.POTATO;
            }

            return clickReturn.DEFAULT;
        }
        else
        {
            point = Vector3.zero;
            netObj = null;
            return clickReturn.NONE;
        }
    }
}
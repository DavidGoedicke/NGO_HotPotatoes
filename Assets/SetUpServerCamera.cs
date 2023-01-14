using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using Cinemachine;
using UnityEngine;

public class SetUpServerCamera : MonoBehaviour
{
   
   
    // Start is called before the first frame update
    void Start()
    {
        
        var tmp = new GameObject();
        tmp.AddComponent<CinemachineTargetGroup>();
        tmp.name = "TargetGroup";
        tmp.transform.SetParent(null);

        CinemachineVirtualCamera virtualCamera =  gameObject.AddComponent<CinemachineVirtualCamera>();
        
      virtualCamera.LookAt = tmp.transform;
      virtualCamera.Follow = tmp.transform;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraDistance : MonoBehaviour
{
CinemachineTargetGroup targetGroup;
    // Start is called before the first frame update
    void Start()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

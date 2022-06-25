using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Level2 : MonoBehaviour
{
    private NavMeshSurface nms;
    private void Awake()
    {
        nms = GetComponent<NavMeshSurface>();
    }
    void Start()
    {
        nms.BuildNavMesh();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

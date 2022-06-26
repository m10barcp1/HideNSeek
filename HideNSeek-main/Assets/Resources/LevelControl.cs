using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelControl : MonoBehaviour
{
    private NavMeshSurface nms;
    private void Awake()
    {
        nms = GetComponent<NavMeshSurface>();
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BakeNM(NavMeshData data)
    {
       
    }
}

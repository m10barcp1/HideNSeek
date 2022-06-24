using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootEffect : MonoBehaviour
{
    private SpriteRenderer footImg;
    public GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        footImg = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //player = GameObject.Find("HidePlayer");
    }

    // Update is called once per frame
    void Update()
    {
        //footImg.color = new Vector4(footImg.color.r, footImg.color.g, footImg.color.b,  2f/Vector3.Distance(transform.position, player.transform.position) * 255); 
    }
}

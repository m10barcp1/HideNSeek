using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingFootPrint : MonoBehaviour
{
    public static PoolingFootPrint instance;
    public static int numberOfFootPrint;

    public GameObject foot;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        numberOfFootPrint = 0;   
    }

    public void FootPrint(Vector3 indexPrint, Transform ParentObject)
    {
        if(numberOfFootPrint < transform.childCount)
        {
            int index = 0;
            for(int i = 0; i< transform.childCount; i++)
            {
                if(transform.GetChild(i).gameObject.activeSelf == false)
                {
                    index = i;
                    break;
                }
            }
            transform.GetChild(index).transform.position = indexPrint;
            transform.GetChild(index).gameObject.SetActive(true); 
            transform.GetChild(index).transform.rotation = ParentObject.transform.rotation;
            numberOfFootPrint++;
            StartCoroutine(ClearFootPrint(2f, transform.GetChild(index).gameObject));
        }
        else
        {
            GameObject footIns =  Instantiate(foot, transform);
            footIns.SetActive(false);
            FootPrint(indexPrint, ParentObject);
        }
    }
    IEnumerator ClearFootPrint(float seconds, GameObject footPrint)
    {
        yield return new WaitForSeconds(seconds);
        footPrint.SetActive(false);
        numberOfFootPrint--;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMM_MouseOver : MonoBehaviour
{
    public GameObject cmmstat;

    public void Start()
    {
        cmmstat.SetActive(false);
    } 

    public void OnMouseOver()
    {
        cmmstat.SetActive(true);
    }

    public void OnMouseExit()
    {
        cmmstat.SetActive(false);
    }
}

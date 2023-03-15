using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_List;

public class TestComparison : MonoBehaviour
{
    public bool listsEquate;
    public List<HurtboxOverlap> listOne;
    public List<HurtboxOverlap> listTwo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        listsEquate = listOne.OverlapEquals(listTwo);
    }
}

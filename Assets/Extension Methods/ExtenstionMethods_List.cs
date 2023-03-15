using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods_List
{
    public static class Extensions
    {
        //Finish this, make sure not to get fucked by reference types
        //You want this functionality to be like some value type shit
        public static bool OverlapEquals(this List<HurtboxOverlap> firstList, List<HurtboxOverlap> secondList)
        {
            Debug.Log("We running the right method?");
            //Obviously if the counts are different the lists are different
            if (firstList.Count != secondList.Count)
                return false;

            //Then we iterate through the list checking each member against each other
            //If we get here firstList.Count == secondList.Count so we can pick either to iterate over
            for (int i = 0; i < firstList.Count; i++)
            {
                if (firstList[i].clip == null)
                { Debug.LogError($"First Clip in position {i} is null"); return false; }

                if (secondList[i].clip == null)
                { Debug.LogError($"Second Clip in position {i} is null"); return false; }

                if (firstList[i].clip.name != secondList[i].clip.name)
                    return false;

                if (firstList[i].stateLength != secondList[i].stateLength)
                    return false;

                if (firstList[i].stateNormalizedTime != secondList[i].stateNormalizedTime)
                    return false;

                if (firstList[i].stateSpeed != secondList[i].stateSpeed)
                    return false;

                if (firstList[i].collider != secondList[i].collider)
                    return false;
            }

            //If we got here then the lists are equal
            return true;
        }
    }
}

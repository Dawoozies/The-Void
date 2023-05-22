using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OLD.Geometry;
namespace ExtensionMethods_List
{
    public static class Extensions
    {
        public static List<Circle> Copy(this List<Circle> list)
        {
            if (list == null || list.Count == 0)
                return null;
            List<Circle> copy = new List<Circle>();
            for (int i = 0; i < list.Count; i++)
            {
                copy.Add(list[i].Copy());
            }
            return copy;
        }
        public static List<Box> Copy(this List<Box> list)
        {
            if (list == null || list.Count == 0)
                return null;
            List<Box> copy = new List<Box>();
            for (int i = 0; i < list.Count; i++)
            {
                copy.Add(list[i].Copy());
            }
            return copy;
        }
        public static List<Area> Copy(this List<Area> list)
        {
            if (list == null || list.Count == 0)
                return null;
            List<Area> copy = new List<Area>();
            for (int i = 0; i < list.Count; i++)
            {
                copy.Add(list[i].Copy());
            }
            return copy;
        }
        //Finish this, make sure not to get fucked by reference types
        //You want this functionality to be like some value type shit
        public static bool OverlapEquals(this List<HurtboxOverlap> firstList, List<HurtboxOverlap> secondList)
        {
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

        //Use this method specifically for constructing a list which gets just the first instance of overlap
        //Instead of every frame an overlap exists
        public static void AddFirstInstance(this List<HurtboxOverlap> list, HurtboxOverlap overlap)
        {
            //Then nothing is in this list so we freely add the overlap
            if(list.Count <= 0)
            { list.Add(overlap); return; }

            //If we get here then list contains stuff and we need to check that the overlap isn't here but with just different normalized time
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].clip.name == overlap.clip.name && list[i].collider.name == overlap.collider.name)
                {
                    //Then same clip + same collider
                    //Then we just return without adding anything
                    if (list[i].stateNormalizedTime <= overlap.stateNormalizedTime)
                        return;
                }
            }

            //If we make it here then we got through the whole list without finding any other earlier instances
            list.Add(overlap);
        }
    }
}

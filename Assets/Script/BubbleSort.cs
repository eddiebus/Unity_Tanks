using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class BubbleSortObject<T>
{
    public T _obj;
    public float objValue;
    BubbleSortObject(T obj, float Value)
    {
        _obj = obj;
        objValue = Value;
    }
}

public class BubbleSort
{
    public static T[] Sort<T>(BubbleSortObject<T>[] objects)
    {
        List<T> resultList = new List<T>();
        if (objects.Length > 1)
        {
            BubbleSortObject<T>[] copyArray = new BubbleSortObject<T>[objects.Length];
            objects.CopyTo(copyArray, 0);
            for (int i = 0; i < copyArray.Length - 1; i++)
            {
                // Check for switch
                if (copyArray[i].objValue > copyArray[i + 1].objValue){
                    var _tempObj = copyArray[i];
                    copyArray[i] = copyArray[i+1];
                    copyArray[i + 1] = _tempObj;
                    i = -1; // Will become 0 on loop. 
                }
            }
        }

        return resultList.ToArray();
    }
}

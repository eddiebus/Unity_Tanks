using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements.Experimental;



public class BubbleSortObject<T>
{
    private T _obj;
    private float _objValue;


    public float objValue => _objValue;
    public T obj => _obj;

    public static bool operator >(BubbleSortObject<T> a, BubbleSortObject<T> b)
    {
        if (a.objValue > b.objValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool operator <(BubbleSortObject<T> a, BubbleSortObject<T> b)
    {
        return !(a > b);
    }

    public BubbleSortObject()
    {
    }

    public BubbleSortObject(T obj, float Value)
    {
        _obj = obj;
        _objValue = Value;
    }
}

public class BubbleSort
{
    public static BubbleSortObject<T>[] Sort<T>(BubbleSortObject<T>[] objects)
    {
        BubbleSortObject<T>[] copyArray = new BubbleSortObject<T>[objects.Length];
        objects.CopyTo(copyArray, 0);


        for (int i = 0; i < copyArray.Length - 1; i++)
        {
            // Check for switch
            if (copyArray[i] > copyArray[i + 1])
            {
                var _tempObj = copyArray[i];
                copyArray[i] = copyArray[i + 1];
                copyArray[i + 1] = _tempObj;
                i = -1; // Will become 0 on loop. 
            }
        }


        return copyArray;
    }
}

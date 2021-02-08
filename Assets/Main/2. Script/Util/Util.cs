using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static Transform FindChildrenObject(GameObject target,string objectName)
    {
        var childrenObj = target.GetComponentsInChildren<Transform>();
        for(int i=0; i<childrenObj.Length; i++)
        {
            if(childrenObj[i].name == objectName)
            {
                return childrenObj[i];
            }
        }
        return null;
    }
}

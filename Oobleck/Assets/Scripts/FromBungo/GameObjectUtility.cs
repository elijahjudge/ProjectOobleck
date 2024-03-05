using UnityEngine;

public static class GameObjectUtility
{
    public static GameObject GetRootGameObject(this GameObject obj, string excludeHolder = " exclude any parent with this ")
    {
        if (obj == null)
        {
            Debug.LogError("Object is null");
            return null;
        }

        Transform parent = obj.transform;
        while (parent.parent != null)
        {
            if (parent.parent.name == excludeHolder)
                return parent.gameObject;

            parent = parent.parent;
        }

        return parent.gameObject;
    }
}

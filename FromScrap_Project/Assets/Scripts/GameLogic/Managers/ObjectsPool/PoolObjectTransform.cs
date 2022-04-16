using System.Collections.Generic;
using System.Xml.Serialization;
using Packages.Utils.Extensions;
using UnityEngine;

public class PoolObjectTransform : MonoBehaviour
{
    [HideInInspector]public string ObjectName;
    [HideInInspector]public string InstanceID;
    [HideInInspector]public List<string> Links;

    [System.Serializable, XmlRoot("Settings")]
    public class PoolObjectInfo
    {
        [HideInInspector] public string ObjectName;
        
        [HideInInspector] public string InstanceID;
        [HideInInspector] public List<string> Links;
        
        [HideInInspector] public Bounds ObjectBounds;
        
        [HideInInspector] public Vector3 LocalPosition;
        [HideInInspector] public Quaternion LocalRotation;
        [HideInInspector] public Vector3 LocalScale;
    }
    
    protected void GetDefaultInfo(PoolObjectInfo info)
    {
        info.ObjectName =  ObjectName;
        info.ObjectBounds = gameObject.GetMaxBounds();
        info.InstanceID = InstanceID;
        var transformBuffer = transform;
        info.LocalPosition = transformBuffer.localPosition;
        info.LocalRotation = transformBuffer.localRotation;
        info.LocalScale = transformBuffer.localScale;
    }

    protected void AcceptTransformInfo(PoolObjectInfo settings)
    {
        ObjectName = settings.ObjectName;
        InstanceID = settings.InstanceID;
        Links = settings.Links;
        
        var transformBuffer = transform;
        transformBuffer.localPosition = settings.LocalPosition;
        transformBuffer.localRotation =  settings.LocalRotation;
        transformBuffer.localScale = settings.LocalScale;
    }
}

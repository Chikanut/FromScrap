using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FromScrap.Tools
{
    [Serializable]
    public class ObjectPool<T> where T : Component
    {
        public T Original;
        public Transform Parent;

        public List<T> Instances { get; } = new List<T>();

        public T GetNextObject()
        {
            return GetNextObject(Parent);
        }

        public T GetNextObject(Transform parent)
        {
            foreach (var icon in Instances)
            {
                if (icon.gameObject.activeSelf) continue;

                icon.gameObject.SetActive(true);
                icon.transform.SetParent(parent);

                return icon;
            }

            var newIcon = Object.Instantiate(Original, parent);
            newIcon.gameObject.SetActive(true);
            Instances.Add(newIcon);

            return newIcon;
        }
        
        public void ClearAll()
        {
            Instances.ForEach(o => o.gameObject.SetActive(false));
        }
    }
}
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


    [GenerateAuthoringComponent()]
    [Serializable]
    public struct LimitDOFJoint : IComponentData
    {
        public bool3 LockLinearAxes;
        public bool3 LockAngularAxes;
    }


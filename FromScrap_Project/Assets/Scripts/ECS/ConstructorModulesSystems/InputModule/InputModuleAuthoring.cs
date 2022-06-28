using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputModuleAuthoring : MonoBehaviour
{

}

public struct InputModuleComponent : IComponentData
{
   public Key TargetKey;
}

public struct InputSignalComponent : IComponentData
{
    
}

// public partial class InputModuleSystem : SystemBase
// {
//     
// }

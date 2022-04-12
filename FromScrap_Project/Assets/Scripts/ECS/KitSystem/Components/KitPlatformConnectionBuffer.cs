 using Unity.Entities;

 namespace Kits.Components
 {
     public enum KitType
     {
         Gun,
         Ram,
         Field,
         Companion,
         Modificator,
     }


     public struct KitPlatformConnectionBuffer : IBufferElementData
     {
         public KitType ConnectionType;
     }
 }

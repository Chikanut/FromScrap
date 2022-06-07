using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Serializable.ResourcesManager
{
   [Serializable]
   public class LevelAssetsEnvironmentPropAsset
   {
      public AssetReference AssetReference;
      public EnvironmentPropAssetType Type;
      public List<ResourceAssetTag> Tags;
   }

   [Serializable]
   public enum EnvironmentPropAssetType
   {
      Bush,
      Tree,
      Rock
   }

   [Serializable]
   public enum ResourceAssetTag
   {
      BigBush,
      SmallBush,
      TallTree,
      LittleTree,
      BigRock,
      SmallRock
   }

   [Serializable]
   public class GameResourcesManagerConfigData
   {
      public AssetReference UIPrefabAsset;
      public AssetReference CoreGameplayAsset;
      public AssetReference LevelAsset;
   }
}

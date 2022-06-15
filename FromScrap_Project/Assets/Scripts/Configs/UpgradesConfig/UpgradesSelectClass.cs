using System.Collections.Generic;
using System.Linq;
using Packages.Common.Storage.Config.Upgrades;

//You can use this static class to select some count of kits from config
public static class UpgradesSelectClass
	{
		public class InstallKitInfo
		{
			public int PlatformID;
			public int Level;
			public int KitIndex;
			public KitInfoData KitInfo;
		}

		public static List<InstallKitInfo> GetSuitableKits(int kitsCount,
			IUpgradesConfigController upgradesConfigController, CurrentCarInfoData data)
		{
			var kitsList = GetAllFreeSuitableKits(false, upgradesConfigController, data);
			kitsList.AddRange(GetAllUpgradeSuitableKits(upgradesConfigController, data));

			if (kitsList.Count < kitsCount)
			{
				kitsList.AddRange(GetAllFreeSuitableKits(true, upgradesConfigController, data));
			}

			var rng = new System.Random();
			kitsList = kitsList.OrderBy(a => rng.Next()).ToList();

			return kitsList;
		}

		public static List<InstallKitInfo> GetAllFreeSuitableKits(bool withDefault,
			IUpgradesConfigController upgradesConfigController, CurrentCarInfoData data)
		{
			var suitableKits = new List<InstallKitInfo>();

			foreach (var platform in data.platformInfos)
			{
				if (!platform.isFree) continue;

				var freeConnectors = platform.Connections;

				if (platform.canOccupy)
				{
					for (var i = 0; i < platform.ConnectedKits.Count; i++)
					{
						if (freeConnectors.Contains(platform.ConnectedKits[i].Type))
							freeConnectors.Remove(platform.ConnectedKits[i].Type);
					}
				}

				if (freeConnectors.Count == 0) continue;

				for (var i = 0; i < upgradesConfigController.GetUpgradesData.Kits.Count; i++)
				{
					var isOpened = upgradesConfigController.GetUpgradesData.Kits[i].IsOpened;
					var isDefault = upgradesConfigController.GetUpgradesData.Kits[i].Data.isDefault;

					if (isDefault && !withDefault) continue;
					if (!isOpened && !isDefault) continue;

					if (data.platformInfos.Any(p => p.ConnectedKitsIndexes.Any(k => k.Index == i))) continue;

					if (freeConnectors.Contains(upgradesConfigController.GetUpgradesData.Kits[i].Data.Type) &&
					    platform.ConnectedKitsIndexes.All(k => k.Index != i))
					{
						suitableKits.Add(new InstallKitInfo()
						{
							PlatformID = platform.ID,
							KitIndex = i,
							Level = 0,
							KitInfo = upgradesConfigController.GetUpgradesData.Kits[i].Data
						});
					}
				}
			}

			return suitableKits;
		}

		public static List<InstallKitInfo> GetAllUpgradeSuitableKits(IUpgradesConfigController upgradesConfigController,
			CurrentCarInfoData data)
		{
			var suitableKits = new List<InstallKitInfo>();

			foreach (var platform in data.platformInfos)
			{
				for (var i = 0; i < platform.ConnectedKits.Count; i++)
				{
					var kitInfo = upgradesConfigController.GetUpgradesData.Kits[platform.ConnectedKitsIndexes[i].Index]
						.Data;
					if (kitInfo.KitObjects.Count > platform.ConnectedKits[i].KitLevel + 1)
						suitableKits.Add(new InstallKitInfo()
						{
							PlatformID = platform.ID,
							Level = platform.ConnectedKits[i].KitLevel + 1,
							KitIndex = platform.ConnectedKitsIndexes[i].Index,
							KitInfo = kitInfo
						});
				}
			}

			return suitableKits;
		}
	}
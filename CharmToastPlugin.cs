using BepInEx;
using CharmCore.Api;
using UnityEngine;

namespace CharmToast
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("com.osmar.Pockets", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.osmar.CharmCore", BepInDependency.DependencyFlags.HardDependency)]
	public class CharmToastPlugin : BaseUnityPlugin
	{
		public const string PluginGuid = "com.osmar.CharmToast";
		public const string PluginName = "CharmToast";
		public const string PluginVersion = "1.0.0";

		internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; } = null!;

		private void Awake()
		{
			Logger = base.Logger;

			// Prevent the plugin from being deleted
			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;

			// Registering binds Enabled, WearPercent, CooldownSeconds and the Debug section (DebugMode) in this mod's config file.
			ToastCharm charm = new ToastCharm();
			CharmsApi.Register(charm, Config);
			this.gameObject.AddComponent<ButterSideUp>().Charm = charm;

			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
		}
	}
}

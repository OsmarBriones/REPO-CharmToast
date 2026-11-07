using BepInEx;
using CharmCore.Api;
using HarmonyLib;
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

		/// <summary>The Butter Side Up effect, reached by the Harmony patch.</summary>
		internal static ButterSideUp? Effect { get; private set; }

		private void Awake()
		{
			Logger = base.Logger;

			// Prevent the plugin from being deleted
			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;

			// Registering binds Enabled, WearPercent and the Debug section (DebugMode) in this mod's config file.
			ToastCharm charm = new ToastCharm();
			CharmsApi.Register(charm, Config);
			Effect = new ButterSideUp(charm);
			new Harmony(PluginGuid).PatchAll();

			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
		}
	}
}

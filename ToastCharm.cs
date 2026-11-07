using CharmCore.Api;

namespace CharmToast
{
	/// <summary>Butter Side Up: falls no longer throw you into a tumble.</summary>
	internal sealed class ToastCharm : Charm
	{
		internal const string PrefabName = "Valuable Museum Toast";

		internal ToastCharm()
			: base("Toast", "Toast - Butter Side Up", defaultWearPercent: 12f, defaultCooldownSeconds: null, PrefabName)
		{
			// Extra host-synced numbers for this power go here, for example:
			// Amount = AddSetting("Amount", 30f, 1f, 100f, "Description for the config file.");
		}
	}
}

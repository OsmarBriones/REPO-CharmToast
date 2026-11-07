using CharmCore.Api;

namespace CharmToast
{
	/// <summary>Butter Side Up: falls no longer throw you into a tumble.</summary>
	internal sealed class ToastCharm : Charm
	{
		internal const string PrefabName = "Valuable Museum Toast";

		private readonly CharmSetting blockFallDamage;

		internal ToastCharm()
			: base("Toast", "Toast - Butter Side Up", defaultWearPercent: 12f, defaultCooldownSeconds: null, PrefabName)
		{
			MinFallDistance = AddSetting(
				"MinFallDistance",
				8f,
				3f,
				50f,
				"Fall length in metres from which a saved landing costs wear. Falls the game would tumble you for (from 8 m) but that are shorter than this are saved for free, so values below 8 behave like 8.");
			blockFallDamage = AddSetting(
				"BlockFallDamage",
				1f,
				0f,
				1f,
				"1 = a saved fall also deals no fall damage. 0 = you do not tumble but still take the game's usual fall damage for the length of the fall.");
		}

		/// <summary>Fall length, in metres, from which a saved landing costs wear.</summary>
		internal CharmSetting MinFallDistance { get; }

		/// <summary>True when a saved fall also deals no damage (setting 1).</summary>
		internal bool BlockFallDamage => blockFallDamage.Value >= 0.5f;
	}
}

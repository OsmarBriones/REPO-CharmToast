using HarmonyLib;

namespace CharmToast.Patches
{
	/// <summary>
	/// Runs before <c>PlayerAvatar.Update</c>, whose last part reads the local player's fall distance and asks for the tumble
	/// (and arms the impact damage) at 8 m. The prefix lets Butter Side Up set that distance back first.
	/// </summary>
	[HarmonyPatch(typeof(PlayerAvatar), "Update")]
	internal static class PlayerAvatar_Update_Patch
	{
		[HarmonyPrefix]
		private static void Prefix(PlayerAvatar __instance)
		{
			CharmToastPlugin.Effect?.Process(__instance);
		}
	}
}

namespace CharmToast.Domain
{
	/// <summary>What the caller has to do after one frame of <see cref="FallTracker.Step"/>.</summary>
	internal struct FallStep
	{
		/// <summary>Set the game's fall distance back to 0 now, so the game does not start the tumble this frame.</summary>
		internal bool ZeroFallDistance;

		/// <summary>The fall that was being saved has just ended with a landing.</summary>
		internal bool Landed;

		/// <summary>The landing was a fall big enough to cost wear (one wear per landing).</summary>
		internal bool Wear;

		/// <summary>The damage the game's own fall tiers would have dealt for this fall (0 when none).</summary>
		internal int Damage;

		/// <summary>The whole fall length in metres, valid when <see cref="Landed"/> is true.</summary>
		internal float TotalFall;
	}

	/// <summary>
	/// Game-free rules of Butter Side Up. The game starts a tumble (and arms the impact damage) in the frame where the
	/// player's fall distance reaches 8 m. The charm sets that distance back to 0 before the game looks at it, so it has to
	/// remember the length of the fall itself: the metres already cleared plus the current stretch. It reports one landing
	/// per fall, not one event per frame. Kept free of Unity and game types so it is unit tested.
	/// </summary>
	internal sealed class FallTracker
	{
		/// <summary>The fall distance at which the game starts the tumble.</summary>
		internal const float GameThreshold = 8f;

		private bool saving;
		private float banked;
		private float lastDistance;

		/// <summary>True while a fall that the charm is holding back is in progress.</summary>
		internal bool IsSaving => saving;

		/// <summary>Forgets the fall in progress (charm switched off, level change).</summary>
		internal void Reset()
		{
			saving = false;
			banked = 0f;
			lastDistance = 0f;
		}

		/// <param name="fallDistance">The game's current fall distance of the local player.</param>
		/// <param name="grounded">True when the player stands on the ground.</param>
		/// <param name="fallReset">True when the game resets fall damage on purpose (for example a respawn teleport).</param>
		/// <param name="minFall">Falls shorter than this many metres are saved without wear.</param>
		internal FallStep Step(float fallDistance, bool grounded, bool fallReset, float minFall)
		{
			FallStep result = default;
			if (!saving)
			{
				if (fallDistance < GameThreshold)
				{
					return result;
				}

				saving = true;
				banked = 0f;
				lastDistance = 0f;
			}
			else if (grounded || fallReset)
			{
				float total = banked + lastDistance;
				bool teleported = fallReset && !grounded;
				Reset();
				if (teleported)
				{
					return result;
				}

				result.Landed = true;
				result.TotalFall = total;
				result.Wear = IsWorthWear(total, minFall);
				result.Damage = DamageForFall(total);
				return result;
			}

			if (fallDistance >= GameThreshold)
			{
				banked += fallDistance;
				lastDistance = 0f;
				result.ZeroFallDistance = true;
			}
			else
			{
				lastDistance = fallDistance;
			}

			return result;
		}

		/// <summary>True when a saved fall of <paramref name="totalFall"/> metres costs wear.</summary>
		internal static bool IsWorthWear(float totalFall, float minFall)
		{
			return totalFall >= minFall;
		}

		/// <summary>The damage the game deals for a fall of this length (its tiers: over 5 m 5, over 9 m 15, over 13 m 25, over 17 m 50, over 21 m 100).</summary>
		internal static int DamageForFall(float totalFall)
		{
			const float start = 5f;
			const float step = 4f;
			if (totalFall > start + step * 4f)
			{
				return 100;
			}

			if (totalFall > start + step * 3f)
			{
				return 50;
			}

			if (totalFall > start + step * 2f)
			{
				return 25;
			}

			if (totalFall > start + step)
			{
				return 15;
			}

			return totalFall > start ? 5 : 0;
		}
	}
}

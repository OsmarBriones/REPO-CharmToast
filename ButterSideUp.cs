using CharmToast.Domain;

namespace CharmToast
{
	/// <summary>
	/// Butter Side Up: falls no longer throw you into a tumble. <see cref="Process"/> is called by the prefix on
	/// <c>PlayerAvatar.Update</c> (see Patches/PlayerAvatar_Update_Patch.cs) on the owner's machine, right before the game's own
	/// fall logic runs. It sets the fall distance back to 0 once it reaches the game's tumble threshold, so the game never asks
	/// for the tumble, and counts one wear per landing. The detail lives in <see cref="FallTracker"/>.
	/// </summary>
	internal sealed class ButterSideUp
	{
		private readonly FallTracker tracker = new FallTracker();

		internal ButterSideUp(ToastCharm charm)
		{
			Charm = charm;
		}

		internal ToastCharm Charm { get; }

		internal void Process(PlayerAvatar avatar)
		{
			if (avatar == null || !avatar.isLocal)
			{
				return;
			}

			if (!Charm.IsActive)
			{
				tracker.Reset();
				return;
			}

			PlayerController controller = PlayerController.instance;
			if (controller == null || controller.CollisionController == null)
			{
				return;
			}

			PlayerCollisionController collision = controller.CollisionController;
			FallStep step = tracker.Step(collision.fallDistance, collision.Grounded, avatar.fallDamageResetState, Charm.MinFallDistance.Value);
			if (step.ZeroFallDistance)
			{
				collision.fallDistance = 0f;
				Charm.LogDebug("Fall of 8 m reached: tumble request prevented");
			}

			if (!step.Landed)
			{
				return;
			}

			Charm.LogDebug($"Landed after a fall of {step.TotalFall:0.#} m (wear: {step.Wear}, game damage would be {step.Damage})");
			if (step.Wear)
			{
				Charm.TryUse();
			}

			if (!Charm.BlockFallDamage && step.Damage > 0 && avatar.playerHealth != null)
			{
				// The game arms its fall damage only through the tumble, which is no longer requested, so the same amount is dealt here.
				avatar.playerHealth.Hurt(step.Damage, true);
			}
		}
	}
}

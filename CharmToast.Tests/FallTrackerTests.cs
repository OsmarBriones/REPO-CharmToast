using CharmToast.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmToast.Tests;

[TestClass]
public class FallTrackerTests
{
	[TestMethod]
	public void ShortFall_IsIgnored()
	{
		FallTracker tracker = new FallTracker();
		FallStep step = tracker.Step(5f, false, false, 8f);
		Assert.IsFalse(step.ZeroFallDistance);
		step = tracker.Step(0f, true, false, 8f);
		Assert.IsFalse(step.Landed);
		Assert.IsFalse(tracker.IsSaving);
	}

	[TestMethod]
	public void ReachingTheThreshold_ZeroesTheDistanceOnce()
	{
		FallTracker tracker = new FallTracker();
		Assert.IsTrue(tracker.Step(8.2f, false, false, 8f).ZeroFallDistance);
		Assert.IsFalse(tracker.Step(0.3f, false, false, 8f).ZeroFallDistance);
		Assert.IsTrue(tracker.IsSaving);
	}

	[TestMethod]
	public void Landing_ReportsOneWearPerFall_NotPerFrame()
	{
		FallTracker tracker = new FallTracker();
		int landings = 0;
		int wears = 0;
		float[] frames = { 4f, 8.1f, 0.4f, 2f, 5f, 8.3f, 1f, 3f };
		foreach (float distance in frames)
		{
			FallStep step = tracker.Step(distance, false, false, 8f);
			landings += step.Landed ? 1 : 0;
			wears += step.Wear ? 1 : 0;
		}

		FallStep landing = tracker.Step(0f, true, false, 8f);
		landings += landing.Landed ? 1 : 0;
		wears += landing.Wear ? 1 : 0;
		Assert.AreEqual(1, landings);
		Assert.AreEqual(1, wears);
		Assert.AreEqual(8.1f + 8.3f + 3f, landing.TotalFall, 0.001f);
		Assert.IsFalse(tracker.IsSaving);
	}

	[TestMethod]
	public void Landing_BelowMinimum_IsSavedWithoutWear()
	{
		FallTracker tracker = new FallTracker();
		tracker.Step(8.5f, false, false, 20f);
		FallStep landing = tracker.Step(0f, true, false, 20f);
		Assert.IsTrue(landing.Landed);
		Assert.IsFalse(landing.Wear);
	}

	[TestMethod]
	public void FallReset_EndsTheFallWithoutLanding()
	{
		FallTracker tracker = new FallTracker();
		tracker.Step(9f, false, false, 8f);
		FallStep step = tracker.Step(0f, false, true, 8f);
		Assert.IsFalse(step.Landed);
		Assert.IsFalse(step.Wear);
		Assert.IsFalse(tracker.IsSaving);
	}

	[TestMethod]
	public void Reset_ForgetsTheFall()
	{
		FallTracker tracker = new FallTracker();
		tracker.Step(9f, false, false, 8f);
		tracker.Reset();
		Assert.IsFalse(tracker.Step(0f, true, false, 8f).Landed);
	}

	[DataTestMethod]
	[DataRow(4f, 0)]
	[DataRow(5f, 0)]
	[DataRow(5.1f, 5)]
	[DataRow(8f, 5)]
	[DataRow(9.5f, 15)]
	[DataRow(13.5f, 25)]
	[DataRow(17.5f, 50)]
	[DataRow(21.5f, 100)]
	[DataRow(60f, 100)]
	public void DamageForFall_FollowsTheGameTiers(float fall, int expected)
	{
		Assert.AreEqual(expected, FallTracker.DamageForFall(fall));
	}

	[TestMethod]
	public void IsWorthWear_UsesTheMinimum()
	{
		Assert.IsTrue(FallTracker.IsWorthWear(8f, 8f));
		Assert.IsFalse(FallTracker.IsWorthWear(7.9f, 8f));
	}
}

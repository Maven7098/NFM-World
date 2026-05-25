using System;

namespace NFMWorldLibrary.Backend.AI
{
    public static class RewardManager
    {
        private static Action<float>? _onRewardApplied;

        public static void Initialize(Action<float> onRewardApplied)
        {
            _onRewardApplied = onRewardApplied;
        }
        // Per-frame reward calculation based on telemetry data
        public static float CalculateTicks(IInGameCar car, MadEngine mad, float roleFlag)
        {
            float reward = 0;
            byte CurrentLap = car.CurrentLap;
            ushort currentCheckpoint = car.CurrentCheckpoint;
            
            // Common Rewards and Penalties for all roles

            // Base reward: Speed
            reward += (float)mad.Speed * 0.1f;
            // Should Aerial Boosting and Hypergliding should be rewarded separately
            // or is the increased speed from them enough of a reward?
            // Should getting low on Power be a separate penalty, or is reduced speed enough of a penalty?
            // For now, we'll just let it affect the reward through reduced speed.

            // Penalty: Damage
            // However, we should be careful with this as it can lead to negative rewards which might destabilize training.
            // We can use a scaled penalty based on the hit magnitude to avoid excessively large negative rewards.
            reward -= (float)mad.Hitmag * 0.01f;
            // Penalty: Getting wasted
            // Should this be a Event-based reward (game should end if a car is wasted) or a Per-frame reward (negative reward upon car getting wasted)?
            // This is a harsh penalty to strongly discourage getting wasted, which is a critical failure state in the game.
            reward -= mad.Wasted ? 100f : 0f;

            if (roleFlag == 0f)
            {
                // Role: Racing
                // Incomplete
                // Reward: Checkpoints
                // If currentCheckpoint increments, give a reward.
                // This encourages the agent to progress through the track.
                // We can also consider giving a small reward for being close to the next checkpoint to encourage progress even if the agent hasn't reached it yet.
                reward += car.CurrentCheckpoint - currentCheckpoint > 0 ? 1f : 0f;
                
                // Incomplete
                // Small reward: Progress towards next checkpoint
                // This encourages the agent to move towards the next checkpoint even if it hasn't reached it yet
                // We can calculate progress as the distance to the next checkpoint compared to the distance at the
                // previous tick and reward the agent for reducing that distance.
                reward += 0.1f * (car.DistanceToNextCheckpoint - currentCheckpoint) / car.DistanceToNextCheckpoint;

                // Incomplete
                // Large Reward: Lap Completion
                // If CurrentLap increments, give a large reward.
                // In Checkpoint and Lap rewards, how can we check when the values incremented?
                // We can store the previous values and compare them each tick, or we can use events if the game provides them.
                reward += car.CurrentLap - CurrentLap > 0 ? 10f : 0f;
            }
            else if (roleFlag == 1f)
            {
                // Role: Wasting
                // Reward: Damage to Opponent
                // This encourages the agent to engage in combat and try to waste opponents.
                // Small Reward: Getting close to a target opponent
                // This encourages the agent to pursue opponents even if it can't successfully waste them yet.
                // We can calculate proximity to opponents using the relative position and reward the agent for being close
                // Large Reward: Successfully Wasting an Opponent
                // This is a critical success state for the Wasting role, so it should be rewarded
            }
            return reward;
        }

        // TODO: Implement Event-based reward for specific actions like finishing a race or wasting all opponents
        // which can trigger a significant reward and end the episode.

        public static void ApplyReward(float reward)
        {
            _onRewardApplied?.Invoke(reward);
        }
    }
}

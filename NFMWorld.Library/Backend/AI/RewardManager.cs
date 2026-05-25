using System;
using System.Runtime.CompilerServices;

namespace NFMWorldLibrary.Backend.AI
{
    public static class RewardManager
    {
        private static Action<float>? _onRewardApplied;

        private class PersistentState
        {
            public ushort LastCheckpoint;
            public byte LastLap;
            public bool WasWasted;
        }

        private static readonly ConditionalWeakTable<IInGameCar, PersistentState> _states = new();

        public static void Initialize(Action<float> onRewardApplied)
        {
            _onRewardApplied = onRewardApplied;
        }

        // Per-frame reward calculation based on telemetry data
        public static float CalculateTicks(IInGameCar car, MadEngine mad, float roleFlag)
        {
            var state = _states.GetOrCreateValue(car);
            float reward = 0;
            
            // Common Rewards and Penalties for all roles

            // Base reward: Speed
            reward += (float)mad.Speed * 0.1f;

            // Penalty: Damage
            // Scaled penalty based on hit magnitude
            reward -= (float)mad.Hitmag * 0.01f;

            // Penalty: Getting wasted
            // One-time large penalty when the car becomes wasted
            if (mad.Wasted && !state.WasWasted)
            {
                reward -= 100f;
            }
            state.WasWasted = mad.Wasted;

            if (roleFlag == 0f)
            {
                // Role: Racing
                
                // Reward: Checkpoints
                if (car.CurrentCheckpoint > state.LastCheckpoint)
                {
                    reward += 10f; // Increased reward for reaching checkpoint
                }
                else if (car.CurrentCheckpoint < state.LastCheckpoint && car.CurrentLap > state.LastLap)
                {
                    // This handles the lap wrap-around where checkpoint resets to 0
                    reward += 10f;
                }
                
                // Reward: Lap Completion
                if (car.CurrentLap > state.LastLap)
                {
                    reward += 100f; // Significant reward for completing a lap
                }

                state.LastCheckpoint = car.CurrentCheckpoint;
                state.LastLap = car.CurrentLap;

                // TODO: Implement "Distance to next checkpoint" reward shaping 
                // requires calculating distance from car to stage's next checkpoint object.
            }
            else if (roleFlag == 1f)
            {
                // Role: Wasting
                // Reward: Damage to Opponent (to be implemented)
                // Small Reward: Getting close to a target opponent (to be implemented)
                // Large Reward: Successfully Wasting an Opponent (to be implemented)
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

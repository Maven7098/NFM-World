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

        public static float Calculate(IInGameCar car, MadEngine mad, float roleFlag)
        {
            float reward = 0;
            
            // Base reward: Speed
            reward += (float)mad.Speed * 0.1f;

            // Penalty: Damage
            reward -= (float)mad.Hitmag * 0.01f;

            return reward;
        }

        public static void ApplyReward(float reward)
        {
            _onRewardApplied?.Invoke(reward);
        }
    }
}

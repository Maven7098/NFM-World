from gymnasium.envs.registration import register
from stable_baselines3 import PPO
from stable_baselines3.common.env_util import make_vec_env
import gymnasium as gym
import os

# Register the environment
# Pointing to nfm_env.py in the same directory
register(
    id='NFMRacer-v1',
    entry_point='nfm_env:NfmEnv',
)

def train():
    # Create the environment
    # Note: Ensure the C# game is running and ready to send UDP packets
    env = gym.make("NFMRacer-v1", ip="127.0.0.1", port=9000)

    # Instantiate the model
    # We use a slightly larger network (256x256) to handle the 57 features
    model = PPO(
        policy="MlpPolicy",
        env=env,
        n_steps=2048,
        batch_size=128,
        n_epochs=10,
        gamma=0.99,
        gae_lambda=0.95,
        clip_range=0.2,
        ent_coef=0.01, # Encourage exploration
        verbose=1,
        tensorboard_log="./ppo_nfm_tensorboard/"
    )

    print("Starting training... Make sure NFM-World is running in Training mode.")
    
    # Train the agent
    try:
        model.learn(total_timesteps=500000, progress_bar=True)
        model.save("PPO-NFMRacer-v1")
        print("Training complete. Model saved as PPO-NFMRacer-v1")
    except KeyboardInterrupt:
        print("Training interrupted. Saving current progress...")
        model.save("PPO-NFMRacer-v1-interrupted")

if __name__ == "__main__":
    train()

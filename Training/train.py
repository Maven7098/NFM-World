from gymnasium.envs.registration import register
from stable_baselines3 import PPO
from stable_baselines3.common.env_util import make_vec_env
from stable_baselines3.common.monitor import Monitor
import gymnasium as gym
import os

# Register the environment
register(
    id='NFMRacer-v1',
    entry_point='nfm_env:NfmEnv',
)

def train():
    # Create and wrap the environment
    env = gym.make("NFMRacer-v1", ip="127.0.0.1", port=9000)
    env = Monitor(env, "./ppo_nfm_tensorboard/") # Monitor logs episode stats

    model_path = "PPO-NFMRacer-v1"

    # Check if a saved model exists to resume training
    if os.path.exists(f"{model_path}.zip"):
        print(f"Loading existing model from {model_path}...")
        model = PPO.load(model_path, env=env)
    else:
        print("No existing model found. Creating a new one...")
        # Instantiate the model
        # Reduced n_steps from 2048 to 512 so it logs 4x more frequently
        model = PPO(
            policy="MlpPolicy",
            env=env,
            n_steps=512, 
            batch_size=64,
            n_epochs=10,
            gamma=0.99,
            gae_lambda=0.95,
            clip_range=0.2,
            ent_coef=0.01,
            verbose=1,
            tensorboard_log="./ppo_nfm_tensorboard/"
        )

    print("Starting training... Make sure NFM-World is running in Training mode.")
    
    # Train the agent
    try:
        model.learn(total_timesteps=500000, progress_bar=True, reset_num_timesteps=False)
        model.save(model_path)
        print(f"Training complete. Model saved as {model_path}")
    except KeyboardInterrupt:
        print("\nTraining interrupted. Saving current progress...")
        model.save(f"{model_path}-interrupted")

if __name__ == "__main__":
    train()

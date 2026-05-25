from gymnasium.envs.registration import register
from stable_baselines3 import PPO
from stable_baselines3.common.env_util import make_vec_env
import gymnasium as gym

# Register the environment
register(
    id='NFMRacer-v1',
    entry_point='training.nfm_env:NfmEnv',
)

# Create the environment
env = make_vec_env("NFMRacer-v1", n_envs=1)

# Instantiate the model
model = PPO(
    policy="MlpPolicy",
    env=env,
    n_steps=1024,
    batch_size=64,
    n_epochs=4,
    gamma=0.999,
    gae_lambda=0.98,
    ent_coef=0.01,
    verbose=1,
)

# Train the agent
model.learn(total_timesteps=100000)
model.save("PPO-NFMRacer-v1")

from gymnasium.envs.registration import register

register(
    id='NFMRacer-v1',
    entry_point='training.nfm_env:NfmEnv',
)

import socket
import gymnasium as gym
from gymnasium import spaces
import numpy as np
import struct

class NfmEnv(gym.Env):
    """
    Custom NFM-World Environment for RL Training.
    Communicates with C# game engine over UDP.
    """
    def __init__(self, ip="127.0.0.1", port=5005):
        super(NfmEnv, self).__init__()
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.socket.bind((ip, port))
        
        # 41 float features (as defined in C# TelemetryPacket struct)
        self.observation_space = spaces.Box(low=-np.inf, high=np.inf, shape=(41,), dtype=np.float32)
        
        # 5 bits for controls = 32 discrete actions
        self.action_space = spaces.Discrete(32)
        
        # Struct format: 41 floats (4 bytes each), Little Endian
        self.struct_format = '<41f'
        self.struct_size = struct.calcsize(self.struct_format)

    def step(self, action):
        # 1. Send action (5 bits packed into a single byte)
        self.socket.sendto(bytes([int(action)]), ("127.0.0.1", 5006))
        
        # 2. Receive telemetry
        data, addr = self.socket.recvfrom(self.struct_size)
        
        # 3. Unpack 41 floats
        obs = np.array(struct.unpack(self.struct_format, data), dtype=np.float32)
        
        # 4. Calculate Reward (Basic shaping)
        # TODO: Move reward calculation logic to C# Referee or here
        reward = obs[7] * 0.1  # Example: Reward based on Speed (index 7)
        
        # 5. Determine termination/truncation
        # TODO: Define these states based on Telemetry flags (e.g., Damage/Wasted)
        terminated = False
        truncated = False
        
        return obs, reward, terminated, truncated, {}

    def reset(self, seed=None, options=None):
        super().reset(seed=seed)
        # TODO: Send reset signal to C#
        return np.zeros(41, dtype=np.float32), {}

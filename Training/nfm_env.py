import socket
import gymnasium as gym
from gymnasium import spaces
import numpy as np
import struct
import time

class NfmEnv(gym.Env):
    """
    Custom NFM-World Environment for RL Training.
    Communicates with C# game engine over UDP.
    """
    def __init__(self, ip="127.0.0.1", port=9000):
        super(NfmEnv, self).__init__()
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.socket.bind((ip, port))
        self.socket.settimeout(5.0) # Timeout if game crashes
        self.last_addr = None
        
        # 56 features (as defined in C# TelemetryPacket struct)
        # Self(10) + Stats(24) + Nav(11) + Opp(7) + Meta(4) = 56
        self.observation_space = spaces.Box(low=-np.inf, high=np.inf, shape=(56,), dtype=np.float32)
        
        # 5 bits for controls = 32 discrete actions
        self.action_space = spaces.Discrete(32)
        
        # Struct format: 
        # Self: 10f
        # Stats: 3i (Swits), 3q (Acelf), i (Handb), q (Airs), i (Airc), i (Turn), 6q (Stats), 6i (Stats), q (DamMult), i (MaxMag)
        # Nav: 11f
        # Opp: 7f
        # Meta: 4f
        self.struct_format = '<10f 3i 3q i q i i 6q 6i q i 11f 7f 4f'
        self.struct_size = struct.calcsize(self.struct_format)

    def _unpack_telemetry(self, data):
        raw = struct.unpack(self.struct_format, data)
        obs = list(raw)
        
        # Convert fix64 (longs) to floats (divide by 2^32)
        # fix64 indices based on format string:
        # Acelf: 13, 14, 15
        # Airs: 17
        # Stats(6q): 20, 21, 22, 23, 24, 25
        # DamMult: 32
        fix64_indices = [13, 14, 15, 17, 20, 21, 22, 23, 24, 25, 32]
        for idx in fix64_indices:
            obs[idx] = obs[idx] / 4294967296.0
            
        return np.array(obs, dtype=np.float32)

    def step(self, action):
        # 1. Send action to last known C# address
        if self.last_addr:
            self.socket.sendto(bytes([int(action)]), self.last_addr)
        
        # 2. Receive latest telemetry (flush buffer to get fresh state)
        data = None
        try:
            while True:
                packet, addr = self.socket.recvfrom(self.struct_size + 1024)
                data = packet
                self.last_addr = addr
                # If we're getting packets faster than we can process, 
                # we only want the most recent one.
                if self.socket.gettimeout() is not None:
                    self.socket.setblocking(False)
        except BlockingIOError:
            self.socket.setblocking(True)
        except socket.timeout:
            print("Timed out waiting for C# telemetry...")
            return np.zeros(56), 0, True, False, {}

        if data is None:
             return np.zeros(56), 0, True, False, {}

        # 3. Unpack
        obs = self._unpack_telemetry(data)
        
        # 4. Get Reward from Meta State (index 55)
        reward = obs[55]
        
        # 5. Determine termination (index 54 is Damage, index 33 is MaxMag)
        # If Damage >= MaxMag, the car is wasted.
        terminated = obs[54] >= obs[33] and obs[33] > 0
        
        # If we got a massive negative reward, consider it a crash/reset
        if reward < -90:
            terminated = True

        return obs, reward, terminated, False, {}

    def reset(self, seed=None, options=None):
        super().reset(seed=seed)
        # Wait for the first packet after a reset
        print("Waiting for game to start/reset...")
        while True:
            try:
                data, addr = self.socket.recvfrom(self.struct_size + 1024)
                self.last_addr = addr
                obs = self._unpack_telemetry(data)
                return obs, {}
            except socket.timeout:
                continue

    def close(self):
        self.socket.close()

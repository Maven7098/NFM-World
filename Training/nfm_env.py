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
            # We want the MOST RECENT packet to avoid lag
            self.socket.setblocking(False)
            while True:
                try:
                    packet, addr = self.socket.recvfrom(self.struct_size + 1024)
                    data = packet
                    self.last_addr = addr
                except BlockingIOError:
                    break
            self.socket.setblocking(True)
            
            # If no data was available in the buffer, wait for one
            if data is None:
                data, addr = self.socket.recvfrom(self.struct_size + 1024)
                self.last_addr = addr

        except socket.timeout:
            print("Timed out waiting for C# telemetry...")
            return np.zeros(56), 0, True, False, {}

        # 3. Unpack
        obs = self._unpack_telemetry(data)
        
        # 4. Get Reward from Meta State (index 55)
        reward = float(obs[55])
        
        # 5. Determine termination
        # index 54 is Damage, index 33 is MaxMag
        is_wasted = obs[54] >= obs[33] and obs[33] > 0
        
        # Check for lap completion (index 41 is CheckpointDistance, but we can use Meta state or rewards)
        # If RewardManager sends a huge reward for finishing, we can detect it.
        # However, it's safer to check if the gamemode finished.
        # Let's assume for now that a huge reward or being wasted ends the episode.
        terminated = is_wasted or reward < -90 or reward > 400
        
        # Truncated is used for time limits, usually False unless we implement a step limit
        truncated = False 

        return obs, reward, terminated, truncated, {}

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
        # Send special 0xFE Shutdown Command to C#
        # Send it multiple times because UDP is unreliable
        if self.last_addr:
            for _ in range(10):
                try:
                    self.socket.sendto(bytes([0xFE]), self.last_addr)
                    time.sleep(0.01)
                except:
                    pass
            print("Sent shutdown signal to NFM-World.")
        self.socket.close()

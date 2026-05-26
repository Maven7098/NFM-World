# NFM-World: RL Architecture & Roadmap

This document summarizes the technical division between the C# Physics Engine and the Python AI Mind, specifically for the **Physics-Only Headless** training approach.

## 1. The Division of Labor

### C# (The Body & World)
*Location: `NFMWorld.Library`*
*   **Physics Loop:** Runs the 63 TPS (Ticks Per Second) simulation.
*   **Telemetry (Senses):** Maps internal physics variables (velocity, rotation, node distance) into a normalized 41-feature vector.
*   **Controls (Muscles):** Receives a byte/integer and maps it to car inputs (Gas, Brake, Steer).
*   **Referee:** Determines rewards, penalties, and episode termination (Done/Reset).
*   **Replays:** Handles the binary `.nfm` export for visual verification.

### Python (The Mind & Manager)
*Location: `training/`*
*   **Gymnasium Wrapper (`NfmEnv`):** Standardizes the C# bridge into a format ML libraries understand.
*   **The Brain (PPO):** The neural network that learns to associate telemetry with winning actions.
*   **Orchestrator:** Manages parallel instances (Vectorization) and curriculum stages.
*   **Telemetry Parser:** Unpacks UDP byte-streams into usable data structures.

---

## 2. Minimal Python Required (To Start)

Once the C# part is complete, you only need these **three components** in Python to begin training:

### A. The Socket Handler
A simple script using the `socket` library to:
1.  Open a UDP port (e.g., 5005).
2.  `recvfrom()` the C# telemetry blob.
3.  `sendto()` a control byte back to C#.

### B. The `NfmEnv` (Gym Interface)
A class that wraps the socket handler and provides:
*   `reset()`: Tells C# to move the car back to the start.
*   `step(action)`: Sends the action to C# and waits for the next telemetry/reward packet.
*   `observation_space`: Tells the AI that it sees 41 numbers.
*   `action_space`: Tells the AI it has 32 possible moves (5-bit input).

### C. The Training Script
A "Template" script (like your `nfmw_unit1.py`) that:
1.  Instantiates `NfmEnv`.
2.  Wraps it in a `DummyVecEnv`.
3.  Calls `model = PPO("MlpPolicy", env).learn(total_timesteps=...)`.

---

## 3. The "Loophole" Checklist (Reward Hacking Guards)
*   **Penalty for Time:** Small negative reward per frame to encourage speed.
*   **Progress Deltas:** Reward based on *change* in distance to the goal, not absolute distance.
*   **Lock-step Sync:** Ensure C# waits for Python before advancing the physics tick.

---

## 4. Next Steps
1.  **Research C#:** Deep dive into `NFMWorld.Library` to identify the best hooks for `PythonBridgeAi`.
2.  **Draft C#:** Implement the telemetry packing and control decoding.
3.  **Validate:** Run a headless console app that responds to a "Random Action" Python script.

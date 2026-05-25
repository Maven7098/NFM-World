# NFM-World Backend AI Documentation

## Overview
The Backend AI system enables high-performance Reinforcement Learning (RL) by creating a direct bridge between the C# physics engine and a Python-based RL training environment.

## Directory Structure (`NFMWorld.Library/Backend/AI/`)
- `BaseAI.cs`: Abstract base class for all AI implementations.
- `PythonBridgeAi.cs`: The core bridge class. Handles real-time UDP communication (Telemetry export, Action import) between C# and Python.
- `TelemetryMapper.cs`: Maps raw `IInGameCar` and `MadEngine` physics state to the standardized `TelemetryPacket` format.
- `RewardManager.cs`: The "Referee" logic. Calculates rewards per tick based on physics performance and role-based objectives.
- `TelemetryPacket.cs` (via `Telemetry` namespace): Defines the 41-feature observation vector.

## Data Flow
1. **Physics Tick**: `MadEngine.cs` updates the physics state.
2. **Referee**: `RewardManager.Calculate()` computes the current reward for the agent.
3. **Mapper**: `TelemetryMapper.Pack()` extracts physics primitives (`Speed`, `Airc`, etc.) into a 41-feature `TelemetryPacket`.
4. **Bridge**: `PythonBridgeAi` serializes the packet to bytes and sends via UDP to the Python `NfmEnv`.
5. **Action**: The Python model computes a control action, sends it back over UDP, and `PythonBridgeAi` applies it to the `Car.Control` object.

## Adding New AI Features
1. **Telemetry**: If you need new sensor data, add a field to `TelemetryPacket` in `Telemetry.cs`.
2. **Mapping**: Update `TelemetryMapper.cs` to include the new physics variable.
3. **Environment**: Update `training/nfm_env.py` to account for the new index in the observation space.

## Compilation Note
Ensure all new files include the `namespace NFMWorldLibrary.Backend.AI;` to maintain consistency with the library architecture.

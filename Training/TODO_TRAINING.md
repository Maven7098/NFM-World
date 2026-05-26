# NFM-World: AI Training Architecture

## Unified AI Model
We use a **Unified Reinforcement Learning Model** (based on PPO) to handle both Racing and Wasting behaviors.

### Core Principles
- **Goal-Conditioned Policy:** The model receives a `Role Flag` in its observation vector (0 for Racer, 1 for Waster).
- **Universal Physics:** By including `CarStats` (Airc, Moment, Push, etc.) in the input, a single model can drive any car and adapt its style to the vehicle's physical limits.
- **Shared Intention:** Racers learn to dodge Wasters by understanding their hunting patterns (since they share the same neural network weights).

## Implementation Roadmap
1. **UDP Bridge:** C# `PythonBridgeAi` communicates with a Python `Gymnasium` environment over a local UDP socket.
2. **Observation Space:** Includes Self State, Track State, Car Stats, Global State (nearest cars), and the Goal Flag.
3. **Reward System:** Role-specific rewards for checkpoint progress (Racer) and damage inflicted (Waster).
4. **Curriculum Training:** 4-phase training from solo racing to adversarial multi-agent chaos.

Reference Implementation Plan: `.gemini/tmp/gemini-cli/0fc5dd56-9694-4452-8ac5-7370a3c55510/plans/unified-ai-training.md`
# Implementation Plan: Unified AI Model with UDP Bridge

## Objective
Implement a high-performance AI training infrastructure for NFM-World using a **Unified Reinforcement Learning Model**. This model will be capable of both high-speed racing (exploiting physics like Aerial Boost) and aggressive wasting (targeting opponents) using a single neural network conditioned on a "Role" input.

## Architecture: The "Universal Brain"
Instead of separate models for every car and role, we will use a **Goal-Conditioned Policy**.
- **Input:** Observations + Car Stats + Role Flag (Racer/Waster).
- **Output:** Controls (Up, Down, Left, Right, Handbrake).

### Why Unified?
- **Shared Physics Intuition:** Both roles benefit from knowing how to handle the car's unique physics (Aerial Boost, momentum).
- **Competitive Awareness:** Racers learn to dodge wasters because they share the same behavioral understanding.
- **Dynamic Roles:** Allows middle-weight cars (Kool Kat) to switch between racing and hunting based on game state.

---

## Step 1: Infrastructure (The UDP Bridge)
Create a high-speed communication channel between the C# Game and the Python Training Script.

### Key Components:
- **`PythonBridgeAi.cs` (C#):**
    - Inherits from `BaseAi`.
    - Every `RunAi` tick:
        1. Packs the current car state and car stats into a byte array.
        2. Sends via UDP to `localhost:5005`.
        3. Waits (non-blocking) for a response with control inputs.
        4. Applies inputs to the `Control` object.
- **`nfm_env.py` (Python):**
    - A custom `Gymnasium` environment.
    - Manages the UDP socket.
    - Interfaces with `stable-baselines3` (PPO algorithm).

---

## Step 2: Observation & State Design
The AI needs the following data to "see" and "think":

| Category | Features | Rationale |
| :--- | :--- | :--- |
| **Self State** | Pos, Vel, Rot, Speed, Damage, Power, Mtouch | Basic driving and survival. |
| **Track State** | Dist to next 3 nodes, Next Checkpoint dir | Navigation. |
| **Car Stats** | Airc, Moment, Grip, Swits, Push | Universal handling across all cars. |
| **Global State** | Relative Pos/Vel of nearest 3 cars | Wasting/Dodging. |
| **Goal Flag** | 0: Racer, 1: Waster | Behavioral switching. |

---

## Step 3: Reward Function Design
We will use a weighted reward system based on the active role.

### Shared Rewards (The "Base"):
- `+ Speed`: Encourages movement.
- `- Damage`: Encourages survival.

### Role-Specific Rewards:
- **Racer (`Role=0`):** 
    - `+ Checkpoint Bonus`: Massive reward for finishing laps.
    - `+ AB Exploit`: Reward for velocity increases while airborne.
- **Waster (`Role=1`):**
    - `+ Damage Inflicted`: Reward when `othermad.Hitmag` increases.
    - `+ Proximity`: Small reward for staying near a target.
    - `+ Wasted Bonus`: Massive reward for destroying an opponent.

---

## Step 4: Training Curriculum
We will train the model in phases to ensure stable learning.

1.  **Phase 1: Solo Time Attack (Racer Focus)**
    - Train on `nfm1` with Radical One
    - Goal: Master steering and the Aerial Boost exploit
2.  **Phase 2: Class-Based Generalization**
    - Randomize cars within Class B (Lead Oxide, Kool Kat, Swrod of Justice)
    - AI learns to adjust inputs based on the `CarStats` vector
3.  **Phase 3: The Waster Introduction (Multi-Agent)**
    - Spawn 2 Racers and 2 Wasters
    - Switch some agents to `Role=1`
    - Wasters learn to hunt while Racers learn to dodge
4.  **Phase 4: Adversarial Fine-Tuning**
    - Train on complex tracks (4DV, Confusion)
    - Introduce the "Leader-Chasing" multiplier from `TODO.txt`
    - Train with Vanilla Class B first, then move onto Vanilla Class A
5.  **Phase 5: ELO Training**
    - Use ELO/ELO cars
        - Use a random class (from Class B to Class S)
            - Class S+ is not used, as I am only training Both AIs
            - Class S+ is only for pure racing or wasting games
    - Assign roles based on car type
        - Roles Can be based on stats or filename
        - Roles can also be decided within a .rad file or tag
    - The Anti-Racer penalty; wasters, even fast ones, should be prevented from racing
        - Only applicable to ELO cars, not Vanilla ones
6. **Phase 6: The MASHEEN Trial**
    - Back up Phase 5 training model (to prevent MASHEEN crashing the training model)
    - Introduce MASHEEN from the Vanilla training set into Class A
    - MASHEEN is isolated as a Class S car during Phase 4, to prevent degenerate behavior of other wasters (EL KING and DR Monstaa) during Class A training
7. **Phase 7: Custom ELO Trial**
    - Use this AI to balance the 4 custom ELO Cars:
        - Shadow Rider (B,R)
        - Tragdor (B,W)
        - iCE CREAM (B,W)
        - Dune Demon (A,W)
    - Two cars are also modified:
        - Tornado Shark (B,R): Now based on the Plymouth Barracuda 1966
        - Hot Pursuit (A,W): Now based on the Carbon Motors E7 Concept
    - Several cars are also renamed (performance is unaltered, but filenames are)
        - The Raven - Night Raven
        - Banshee - Gespenst
        - Prancer - Outrunner
        - Gojira - Kaiju
        - Get Lost - Infraction
        - Cross - Bounty Hunter
        - Titan Mk II - Titan

**Questions for each phase**
1. Phase 1: Should I filter the stats, or should I use everything?
    - Stats such as clrad can be used for damage calculation (or is it?)
    - At least the car polygons can be filtered out, as they are no longer factored in the damage calculation
2. Phase 5: I can hard-code roles in the car files, what is wrong with this approach?
    - This is to prevent fast wasters (like Quadra Turbo or Bounty Hunter) to race instead of waste, which violates community expectations
    - This is only for the ELO cars, vanilla cars (and other add-on cars) will instead get a float value for roles, allowing them to switch roles whenever necessary
3. Phase 6: Can this be used to prefer weighting certain cars depending on track type?
    - Read track .txt files, then consider NFM tracks (NFM1, NFM2, NFMM, NFMR) similar to it
    - What is the problem with this approach?
    
**Cars in each class (Vanilla Training Plan)**
- CLASS C (Not used in training)
    - Tornado Shark
    - Wow Caninaro
    - La Vita Crab
    - Nimi
- CLASS B
    - Formula 7
    - MAX Revenge
    - Lead Oxide
    - Kool Kat
    - Drifter X
    - Sword of Justice
- CLASS A
    - High Rider
    - EL KING
    - Mighty Eight
    - Radical One
    - DR Monstaa
- CLASS S (Not used in training until Phase 6)
    - M A S H E E N

---

## Verification & Testing
1.  **Latency Check:** Ensure the UDP round-trip is < 5ms to avoid physics desync.
2.  **Exploit Verification:** Compare AI lap times with Radical One against the developer's records.
3.  **Behavioral Audit:** Observe Wasters to ensure they prioritize the human player (1.2x multiplier) without ignoring bots.
4.  **Headless Performance:** Verify that Node #1 can run at 10x simulation speed without breaking the bridge.
5. **ELO Waster Behavior:** Designated wasters (such as those in the ELO channel) should ONLY waste

## CSV file format for Headless training (Ask it during Phase 3)
- Time Attack Files (Phase 2):
    - Car_Type (Which car was used in training? i.e. Radical One, Kool Kat)
    - Track_Type (Which stage was used in training? i.e. NFM1/2 - Contrary to Popular Belief or NFM2/16 - Four Dimensional Vertigo)
    - Track_Time (What is the total split in this session? - What should be the measurements - physics ticks, actual time, or so on?)
    - Lap_Time (What is the best lap in this session? - What should be the measurements - physics ticks, actual time, or so on?)
- Accumulate Files (Phase 3+):
    - Category (Vanilla or ELO)
    - Class (B, A, S)
    - Track_Type (Which stage was used in training?)
    - Car_Count (How many times this car was generated out of 100 trials, including duplicates)
    - Car_Win (How many times this car won, either by racing or by wasting)
    - Car_Lose (Only count cases where this car lost a race, not by being wasted)
    - Car_Wasted (How many times this car was wasted)
    - Car_RacerWeight (How much did the car learn during racing?)
    - Car_WasterWeight (How much did the car learn during wasting?)
    - Car_Alignment (Racer or Waster; fixed to 0 or 1 for ELO cars based on role)
- Trial Files (Phase 3+):
    - Category
    - Class
    - Track_Type
    - Car_Count (How many times this car appeared in this match?) - Duplicate AIs would be generated during training
    - Win_Car (Which car won?)
    - Win_Method (Racing or Wasting)
- Video file is sent every 500 plays, for every car perspective, for every phase

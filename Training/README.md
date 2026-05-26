# NFM-World: Reinforcement Learning Path

This document outlines the learning strategy for developing and improving the AI models in NFM-World, specifically mapped against the [Hugging Face Deep RL Course](https://huggingface.co/learn/deep-rl-course).

## Learning Roadmap

### 1. Introduction to Deep RL (Chapter 1)
*   **Status:** Essential Foundation.
*   **NFM-World Application:** Understanding the fundamental Markov Decision Process (MDP) loop between the C# game engine (Environment) and the Python `NfmEnv` (Agent).

### 2. Tabular Q-Learning (Chapter 2)
*   **Status:** Conceptual Only.
*   **NFM-World Application:** Low relevance for direct implementation. Since the game runs at 63 TPS with continuous coordinates, the state space is effectively infinite. A Q-Table cannot scale to this complexity.

### 3. Deep Q-Learning / DQN (Chapter 3)
*   **Status:** Understanding Foundation.
*   **NFM-World Application:** Provides the transition from tabular to neural-network-based function approximation. Important for understanding how a Neural Network (MLP) acts as a function approximator for continuous state inputs.

### 4. Policy Gradients (Chapter 4)
*   **Status:** Primary Focus.
*   **NFM-World Application:** Our model uses **PPO (Proximal Policy Optimization)**. This chapter explains how the model directly optimizes the probability of actions (Steer, Gas, Brake) based on the 41-feature input vector.

### 5. Actor-Critic Methods (Chapter 6)
*   **Status:** Critical for PPO Understanding.
*   **NFM-World Application:** PPO is an Actor-Critic algorithm. This chapter covers the "Critic" part—the component that learns to estimate the value of states, which is vital for effective reward optimization in NFM-World.

### 6. Proximal Policy Optimization (PPO) (Chapter 8)
*   **Status:** Final Implementation Focus.
*   **NFM-World Application:** Explains the core algorithm used in `train.py`. Focus on the clipping mechanism, which provides the stability needed for high-frequency (63 TPS) physics simulation training.

## Model Configuration Summary

| Feature | Specification |
| :--- | :--- |
| **Algorithm** | PPO (Actor-Critic) |
| **Policy Type** | `MlpPolicy` (Multi-Layer Perceptron) |
| **Input (Obs)** | 41-D Vector (Position, Speed, Damage, Stats, Nodes) |
| **Output (Action)** | 5-bit Discrete (32 combinations of inputs) |
| **Frequency** | 63 Ticks Per Second (TPS) |
| **Bridge** | UDP Socket (Python `Gymnasium` <-> C# `PythonBridgeAi`) |

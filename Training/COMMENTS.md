## Questions with Telemetry design (TelemetryMapper.cs and TelemetryPacket.cs)
- Does Telemetry should strictly be a 41D structure?
- I need to accommodate more information

# AI Nodes
- As we use reinforcement learning and find checkpoints, we can disregard AI nodes
- But the AI need to know the location of track pieces and the checkpoints
- I chose to keep the Checkpoint distance
- Checkpoint angle may not need to be retained; the AI can enter the checkpoint both at the front and at the back, just not the wall part
- For example, if there is a ramp nearby, the AI can stunt or AB (Aerial Boost) off them
- If there is a wall nearby, the AI should avoid the wall (because damage is a negative reinforcement)
- Additionally, the AI should know whether a ramp is facing their way or the opposite way

# Car Stats
- Is comprad and msquash needed?
- msquash at least appears to decide bad landing threshold
- Which may be needed for the AI to learn how to land properly when damaged.
- In vanilla NFM, comprad is used to determine the radius of the car's collision sphere
- which can affect how the car interacts with the environment and other cars, which actually affects damage calculation.
- Not sure if physics change in NFMW removed the need for comprad in damage calculation though.

# Opponent Awareness
- Should the opponent be a single nearest opponent or multiple opponents?
- Should there be a flag for what car is leading the race, what car has the most damage...
- Or at least tell the stats of the nearest opponent car to tell the AI what car is closest
- For example, as Radical One, one can get close to Mighty Eight or High Rider (lower moment)
- But one should not get close to EL KING or DR Monstaa (higher moment)
- Getting close to cars with higher moment can cause damage (negative reinforcement) and get wasted (even bigger negative reinforcement)

# Meta Stats
- Why is Damage included in Meta State instead of Self State?
- Is it because Damage is not a direct physical property of the car's current stats
- But rather a derived value that can affect the car's performance and decision-making?
- It represents the overall condition of the car and can influence how the AI should approach

## Reward Management (RewardManager.cs)
- Should reward be calculated straight from MadEngine, or should I use telemetry data?
- Should I teach the AI exactly how to drive, for example, should I explicitly have it learn Aerial Boost to see when the car is moving faster in the air, or is the speed reward sufficient?
    - Should Aerial Boosting and Hypergliding should be rewarded separately
    - or is the increased speed from them enough of a reward?
    - Should getting low on Power be a separate penalty, or is reduced speed enough of a penalty?
    - For now, we'll just let it affect the reward through reduced speed.
- What if the rewards converge to local maximum, without the AI discovering anything?
    - The airspeed of Radical One increases when it does backloops
    - It increases more when it does frontloops
    - It increases even more when it AB
    - It increases most when hypergliding (but it can't chain AB, plus risk of bad landing)
    - In this case, Radical One may discover backloops, and will not try any other stunt
    - It may also use hypergliding excessively when AB+bounce may be the fastest strategy
- Reward: Checkpoints
- If currentCheckpoint increments, give a reward.
- This encourages the agent to progress through the track.
- We can also consider giving a small reward for being close to the next checkpoint to encourage progress even if the agent hasn't reached it yet.
- Large Reward: Lap Completion
- If CurrentLap increments, give a large reward.
- In Checkpoint and Lap rewards, how can we check when the values incremented?
    - We can store the previous values and compare them each tick, or we can use events if the game provides them.

- Penalty: Damage
- However, we should be careful with this as it can lead to negative rewards which might destabilize training.
- We can use a scaled penalty based on the hit magnitude to avoid excessively large negative rewards.
- Penalty: Getting wasted
- Should this be a Event-based reward (game should end if a car is wasted) or a Per-frame reward (negative reward upon car getting wasted)?
- This is a harsh penalty to strongly discourage getting wasted, which is a critical failure state in the game.
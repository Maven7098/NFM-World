## TODO Priority:
# HIGH
- Generate win conditions for `go_race`
- Not relevant in Stages 0 to 3, but required during MARL training
    - Finish in first place
    - Waste the other cars
- Generate AI model for the game
    - STAGE 0: Implement the baseline to implement the AI model
        - C#-Python Bridge
        - C# Telemetry
        - Python AI tools
        - Headless Compilation
    - STAGE 1: Start with Radical One (STAGE 1~3 is for `go_tt`)
        - Continue until Radical One is able to beat this developer's lap time
        - Frankly I am not the best racer (around 880 ELO, lowest of all of AIM)
    - STAGE 2: Expand this with Class B Vanilla
        - Now the AI should be able to drive any Class B effectively, such as learning AB with Kool Kat, or learning ground racing with F7
    - STAGE 3: Expand this with Class A Vanilla
        - More specialized cars, such as Mighty Eight or EL KING, are introduced, giving the AI more flexibility
    - STAGE 4: Introduce Waster for Vanilla (start with `go_race`)
        - Train on Vanilla first as they have lower HP across the board, making survival more important
        - Start with Class B, then Class A
    - STAGE 5: Harder training with ELO AI
        - Less unwinnable permutations
    - STAGE 6: The M A S H E E N Trial
        - Introduce M A S H E E N in Vanilla Class A, and observe its behavior
        - M A S H E E N is not trained in stages 3 and 4 because of its `edge case` stats, which encourage degenerate behaviors, such as camping, to other wasters
# MEDIUM
- Implement the Freeplay Gameplay mode
    - Needs the AI to be implemented first
- Implement Replay Camera

## Generate Win Conditions:
- If you get wasted, you lose (reset stage)
- If you waste all the AI cars, you win (reset stage)
- If you cross the finish line first, you win (reset stage)
- If any AI car crosses the finish line first, you lose (reset stage)
    - Maybe necessary in order to allow to reset the game if the wasters win (racers get wasted)

## Generate AI for more advanced gameplay
# Part 0 (C#-Python bridge set up)
- How can I test the bridge?
    - I can start reading the code after Tuesday
# Part 1 (Racer Training)
- Train a Racer AI (radicalone)
    - Radical One (radicalone) was selected as it is a dedicated racer, and can Aerial Boost (AB - a physics exploit using vehicles with high airc values)
    - Input the other cars and train them respectively to allow the AI to acclimate to different cars
        - After Radical One, Prioritize Class B cars (Formula 7 is included in training set, High Rider is not)
    - Start with Introductory Stage and Contrary to Popular Belief
        - Then continue with the rest of the NFM1 stages
- As NFM-World does not yet support Linux GUI support, the training will be done headless and analysis on telemetry

# Part 2 (Waster Training)
- After training all racer AIs, introduce 2 Racer AIs (2 random racers) and 2 Waster AIs (2 random wasters)
    - Cars should match their classes
    - Racers will win a lot at first, but Wasters will soon learn to waste
    - MARL team approach to training the wasters
        - Point multiplier when chasing the race leader (1st: 4x, 2nd: 2x...)
        - Point divided when wasters hit each other
- After training Wasting AIs, repeat the process to every Vanilla waster
    - They can switch between racing and wasting at will
    - For example, they will try to 1v1 the player if they are the last car alive and they are not close to finishing the race, rather than keep racing

# Part 3 (Human Feedback & Testing)
- After sufficient phases, human player will enter the race (mustang)
    - As the developer assumes the AIs will be more competent than the player, give the human player an advantage by giving them a superior car (High Rider) than the AIs (Class B)
        - This is because the initial pair of cars trained, after Radical One, will be Class B
    - Point multiplier when chasing the player car (fixed to 1.2x)
    	- AI is frozen by this point to prevent degrading

# Final Part
- After training Vanilla AIs, train ELO AIs, with a human player for the testing
- Training should be done in every stage (nfm1, nfm2, nfmm, nfmrelit, elo)
    - Most of the training is done in Confusion, 4DV, L4D
        - With 20% of the training time spent on the remaining courses for fine-tuning
    - football and challenge categories are not meant to be used to train AI

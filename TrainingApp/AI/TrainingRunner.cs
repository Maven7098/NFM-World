using System;
using NFMWorldLibrary;
using NFMWorldLibrary.Backend;
using NFMWorldLibrary.Backend.Gamemodes;
using NFMWorldLibrary.Backend.AI;
using NFMWorldLibrary.Util;

namespace NFMWorld.MadEngine.AI
{
    public class TrainingRunner
    {
        public void StartTraining(string ipAddress, int port, string carName, string stagePath)
        {
            // 1. Initialize core backend
            BackendGameSparker.Load();

            // 2. Initialize the Python Bridge
            var pythonBridge = new PythonBridgeAi(ipAddress, port);

            // 3. Initialize your gamemode
            var parameters = new BaseGamemodeParameters()
            {
                PlayerCarIndex = 0,
                Players = [ new PlayerParameters() 
                { 
                    PlayerName = "AI_Agent",
                    CarName = carName,
                    Color = new Color3(255, 255, 255),
                    IsBot = false 
                } ]
            };

            var raceValues = BackendRaceValues.Create(stagePath);
            var gamemode = new TimeTrialGamemode(parameters, raceValues);

            // 4. Glue them together using the Adapter
            var adapter = new TimeTrialRunAdapter(gamemode);
            pythonBridge.Setup(adapter);

            // Register the RewardManager to send rewards through the pythonBridge
            RewardManager.Initialize(reward => pythonBridge.Send($"REWARD:{reward}"));

            gamemode.Reset();

            // 5. Start the high-speed training loop
            Console.WriteLine($"Starting training loop with Car: {carName}, Stage: {stagePath}...");
            var playerCar = gamemode.carsInRace[gamemode.playerCarIndex];
            
            while (true)
            {
                // 1. Invoke AI inference
                pythonBridge.RunAi(playerCar, gamemode.currentStage, gamemode.playerCarIndex);

                // 2. Tick the game logic
                gamemode.GameTick();
            }
        }
    }
}

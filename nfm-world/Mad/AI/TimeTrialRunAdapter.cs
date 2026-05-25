using System;
using NFMWorldLibrary.Backend.AI;
using NFMWorldLibrary.FixedMath;
using NFMWorldLibrary.Backend.Gamemodes;

namespace NFMWorld.Mad.AI
{
    public class TimeTrialRunAdapter : ITrackableRun
    {
        private readonly TimeTrialGamemode _gamemode;
        public event EventHandler<RunFinishedEventArgs>? RunFinished;

        public TimeTrialRunAdapter(TimeTrialGamemode gamemode)
        {
            _gamemode = gamemode;
            _gamemode.RaceFinished += (sender, data) =>
            {
                RunFinished?.Invoke(this, new RunFinishedEventArgs(true, data));
            };
        }

        public long GetCurrentTime() => 0; // TODO: Implement if needed
        public int GetCurrentLap() => _gamemode.carsInRace[_gamemode.playerCarIndex].currentLap;
        public int GetTotalLaps() => _gamemode.currentStage.nlaps;
        public fix64 GetTrackPosition() => (fix64)_gamemode.carsInRace[_gamemode.playerCarIndex].totalCheckpoint / (fix64)(_gamemode.currentStage.checkpoints.Count * _gamemode.currentStage.nlaps);
    }
}

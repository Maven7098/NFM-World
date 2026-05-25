using System;
using NFMWorldLibrary.FixedMath;

namespace NFMWorldLibrary.Backend.AI
{
    public class RunFinishedEventArgs : EventArgs
    {
        public bool Success { get; }
        public byte[] ResultData { get; }

        public RunFinishedEventArgs(bool success, byte[] resultData = null!)
        {
            Success = success;
            ResultData = resultData;
        }
    }

    public interface ITrackableRun
    {
        event EventHandler<RunFinishedEventArgs>? RunFinished;
        
        long GetCurrentTime();
        int GetCurrentLap();
        int GetTotalLaps();
        fix64 GetTrackPosition();
    }
}

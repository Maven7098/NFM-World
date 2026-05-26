using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace NFMWorldLibrary.Backend.AI
{
    public class PythonBridgeAi : BaseAi, IDisposable
    {
        private readonly UdpClient _udpClient = new();
        private readonly IPEndPoint _remoteEndPoint;
        private ITrackableRun? _runAdapter;
        public bool ResetRequested { get; set; }

        public PythonBridgeAi(string ipAddress, int port)
        {
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }

        public void Setup(ITrackableRun run)
        {
            _runAdapter = run;
            _runAdapter.RunFinished += OnRunFinished;
        }

        public void Send(string message)
        {
            // Note: Sending strings might crash the current Python script 
            // if it expects a fixed-size binary structure.
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(buffer, buffer.Length, _remoteEndPoint);
        }

        private void OnRunFinished(object? sender, RunFinishedEventArgs e)
        {
            // Instead of sending a string, we set a flag to reset or could pack a special terminal packet
            // For now, let's just mark that the run is done if we want to force a reset from C# side
            // or let the Python side detect termination via telemetry.
        }

        public override void RunAi(IInGameCar car, IStage stage, int currentCarIndex)
        {
            var mad = car.Mad;
            
            // 1. Calculate Reward and normalize by Physics Speedup
            float roleFlag = 0; 
            float rawReward = RewardManager.CalculateTicks(car, mad, roleFlag);
            float normalizedReward = rawReward * (float)Physics.PHYSICS_MULTIPLIER;

            // 2. Pack Telemetry
            float rank = (float)car.Placement / 10f; 
            var packet = TelemetryMapper.Pack(car, mad, stage, roleFlag, rank, normalizedReward);

            // 3. Serialize and Send
            byte[] buffer = StructToBytes(packet);
            _udpClient.Send(buffer, buffer.Length, _remoteEndPoint);

            // 4. Receive Controls & Commands (Non-blocking)
            while (_udpClient.Available > 0)
            {
                IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpClient.Receive(ref from);
                if (data.Length == 1)
                {
                    byte action = data[0];
                    if (action == 0xFF) // Special Reset Command
                    {
                        ResetRequested = true;
                    }
                    else
                    {
                        ApplyControls(car, action);
                    }
                }
            }
        }

        private void ApplyControls(IInGameCar car, byte action)
        {
            car.Control.Handb = (action & 1) != 0;
            car.Control.Down = (action & 2) != 0; // Using Down as Brake
            car.Control.Left = (action & 4) != 0;
            car.Control.Right = (action & 8) != 0;
            car.Control.Up = (action & 16) != 0;
            // bit 32 ignored for now as we don't have a 6th control
        }

        private static byte[] StructToBytes<T>(T str) where T : struct
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(str, ptr, false);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return arr;
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}

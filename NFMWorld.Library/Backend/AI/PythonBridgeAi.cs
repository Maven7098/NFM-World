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
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(buffer, buffer.Length, _remoteEndPoint);
        }

        private void OnRunFinished(object? sender, RunFinishedEventArgs e)
        {
            float completionReward = e.Success ? 500.0f : -100.0f;
            Send($"REWARD:{completionReward}");
        }

        public override void RunAi(IInGameCar car, int currentCarIndex)
        {
            var mad = car.Mad;
            
            // 1. Calculate Reward
            float roleFlag = 0; // Default to Racer for now
            float currentReward = RewardManager.Calculate(car, mad, roleFlag);

            // 2. Pack Telemetry
            float rank = (float)car.Placement / 10f; 
            var packet = TelemetryMapper.Pack(car, mad, roleFlag, rank, currentReward);

            // 3. Serialize and Send
            byte[] buffer = StructToBytes(packet);
            _udpClient.Send(buffer, buffer.Length, _remoteEndPoint);

            // 4. Receive Controls (Non-blocking)
            if (_udpClient.Available > 0)
            {
                IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpClient.Receive(ref from);
                if (data.Length > 0)
                {
                    byte action = data[0];
                    ApplyControls(car, action);
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

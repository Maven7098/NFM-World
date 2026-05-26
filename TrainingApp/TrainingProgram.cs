using System;
using System.Runtime.InteropServices;
using NFMWorldLibrary.Backend.AI;

namespace NFMWorld.MadEngine.AI
{
    class TrainingProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"TelemetryPacket Size: {Marshal.SizeOf<TelemetryPacket>()}");
            
            // Set your parameters here
            string ip = "127.0.0.1";
            int port = 9000;
            string car = "nfmm/radicalone";
            string stage = "nfm1/2_contrary";

            var runner = new TrainingRunner();
            runner.StartTraining(ip, port, car, stage);
        }
    }
}

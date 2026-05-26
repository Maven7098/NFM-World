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

            // Default parameters
            string ip = "127.0.0.1";
            int port = 9000;
            string car = args.Length > 0 ? args[0] : "nfmm/radicalone";
            string stage = args.Length > 1 ? args[1] : "nfm1/3_snakedance";

            Console.WriteLine($"Training Car: {car}");
            Console.WriteLine($"Training Stage: {stage}");

            var runner = new TrainingRunner();
            runner.StartTraining(ip, port, car, stage);
        }
    }
}

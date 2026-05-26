using NFMWorld.Training;

namespace NFMWorld.MadEngine.AI
{
    class TrainingProgram
    {
        static void Main(string[] args)
        {
            // Set your parameters here
            string ip = "127.0.0.1";
            int port = 9000;
            string car = "formula7";
            string stage = "road";

            var runner = new TrainingRunner();
            runner.StartTraining(ip, port, car, stage);
        }
    }
}

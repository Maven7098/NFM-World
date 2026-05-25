using System.Runtime.InteropServices;
using NFMWorldLibrary.FixedMath;

namespace NFMWorldLibrary.Backend.AI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    // Does it strictly have to be a 41D vector?
    // Can we add more dimensions to accommodate all car stats, since some stats are not currently included in the TelemetryPacket?
    public struct TelemetryPacket
    {
        // Self State (10)
        public float ForwardVel, RightVel, UpVel;
        public float YawRate, PitchRate, RollRate;
        public float Speed;
        public float PosX, PosY, PosZ;

        // Car Stats (23)
        // Is comprad and msquash needed?
        // msquash at least appears to decide bad landing threshold
        // Which may be needed for the AI to learn how to land properly when damaged.
        // In vanilla NFM, comprad is used to determine the radius of the car's collision sphere
        // which can affect how the car interacts with the environment and other cars.
        // This actually affects damage calculation.
        // Not sure if physics change in NFMW removed the need for comprad in damage calculation though.
        public Int3 Swits;
        public f64Vector3 Acelf;
        public int Handb;
        public fix64 Airs;
        public int Airc;
        public int Turn;
        public fix64 Grip;
        public fix64 Bounce;
        public fix64 Moment;
        public fix64 Comprad;
        public fix64 Push;
        public fix64 Revpush;
        public int Lift;
        public int Revlift;
        public int Powerloss;
        public int Flipy;
        public int Msquash;
        public int Clrad; 
        public fix64 Dammult;
        public int Maxmag;

        // Navigation (12)
        public float CheckpointRelX, CheckpointRelY, CheckpointRelZ;
        public float CheckpointAngle;
        public float CheckpointDistance;
        public float RampRelX, RampRelY, RampRelZ;
        public float RampOrientation; 
        public float RampDistance;
        public float GroundDistance;

        // Opponent Awareness (7)
        // Should the opponent be a single nearest opponent or multiple opponents?
        // For simplicity, we can start with a single nearest opponent.
        public float RelPosX, RelPosY, RelPosZ;
        public float RelVelX, RelVelY, RelVelZ;
        public float EnemyProximity;

        // Meta State (4)
        // Why is Damage included in Meta State instead of Self State?
        // Because Damage is not a direct physical property of the car's current state, but rather a derived value that can affect the car's performance and decision-making.
        // It represents the overall condition of the car and can influence how the AI should approach
        public float RoleFlag;
        public float Rank;
        public float Damage;
        public float CurrentReward;
    }
}

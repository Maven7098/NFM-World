using System.Runtime.InteropServices;

namespace NFMWorldLibrary.Backend.AI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TelemetryPacket
    {
        // Self State (7)
        public float PosX, PosY, PosZ;
        public float VelX, VelY, VelZ;
        public float Speed;

        // Car Stats (5)
        public float Airc, Moment, Grip, Swits, Push;

        // Navigation (5)
        public float AngleToNode, DistanceToNode;
        public float AngleToCheckpoint, DistanceToCheckpoint;
        public float GroundDistance;

        // Opponent Awareness (7)
        public float RelPosX, RelPosY, RelPosZ;
        public float RelVelX, RelVelY, RelVelZ;
        public float EnemyProximity;

        // Meta State (4)
        public float RoleFlag;
        public float Rank;
        public float Damage;
        public float CurrentReward;

        // Padding (13)
        public float Reserved1, Reserved2, Reserved3, Reserved4, Reserved5;
        public float Reserved6, Reserved7, Reserved8, Reserved9, Reserved10;
        public float Reserved11, Reserved12, Reserved13;
    }
}

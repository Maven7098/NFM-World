using NFMWorldLibrary.Backend.Gamemodes;
using NFMWorldLibrary.FixedMath;

namespace NFMWorldLibrary.Backend.AI
{
    public static class TelemetryMapper
    {
        public static TelemetryPacket Pack(IInGameCar car, MadEngine mad, float roleFlag, float rank, float currentReward)
        {
            var packet = new TelemetryPacket
            {
                // Self State
                PosX = (float)car.Position.X,
                PosY = (float)car.Position.Y,
                PosZ = (float)car.Position.Z,
                // Regarding Velocity, we can use the Scx, Scy, Scz values from MadEngine which represent the car's velocity in the world space.
                // What does Scx, Scy, Scz represent? They are the components of the car's velocity vector in the world coordinate system.
                VelX = (float)mad.Scx[0],
                VelY = (float)mad.Scy[0],
                VelZ = (float)mad.Scz[0],
                Speed = (float)mad.Speed,

                // Car Stats
                Swits = mad.Stat.Swits,
                Acelf = mad.Stat.Acelf,
                Handb = mad.Stat.Handb,
                Airs = mad.Stat.Airs,
                Airc = mad.Stat.Airc,
                Turn = mad.Stat.Turn,
                Grip = mad.Stat.Grip,
                Bounce = mad.Stat.Bounce,
                Moment = mad.Stat.Moment,
                Comprad = mad.Stat.Comprad,
                Push = mad.Stat.Push,
                Revpush = mad.Stat.Revpush,
                Lift = mad.Stat.Lift,
                Revlift = mad.Stat.Revlift,
                Powerloss = mad.Stat.Powerloss,
                Flipy = mad.Stat.Flipy,
                Msquash = mad.Stat.Msquash,
                Clrad = mad.Stat.Clrad,
                Dammult = mad.Stat.Dammult,
                Maxmag = mad.Stat.Maxmag,

                // Navigation (Basic implementation for now)
                // Navigation requires the ability to find nearest objects like ramps and walls
                // which is not currently implemented. For now, we can set these to zero or some default value.
                AngleToNode = 0f,
                DistanceToNode = 0f,
                AngleToCheckpoint = 0f,
                DistanceToCheckpoint = 0f,
                GroundDistance = (float)car.Position.Y,

                // Opponent Awareness (Placeholder)
                RelPosX = 0f, RelPosY = 0f, RelPosZ = 0f,
                RelVelX = 0f, RelVelY = 0f, RelVelZ = 0f,
                EnemyProximity = 0f,

                // Meta State
                RoleFlag = roleFlag,
                Rank = rank,
                Damage = (float)mad.Hitmag,
                CurrentReward = currentReward
            };

            return packet;
        }
    }
}

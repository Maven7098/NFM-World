using NFMWorldLibrary.Backend.Gamemodes;
using NFMWorldLibrary.FixedMath;

namespace NFMWorldLibrary.Backend.AI
{
    public static class TelemetryMapper
    {
        public static TelemetryPacket Pack(IInGameCar car, MadEngine mad, IStage stage, float roleFlag, float rank, float currentReward)
        {
            // --- Navigation Logic ---
            float cpRelX = 0, cpRelY = 0, cpRelZ = 0, cpAngle = 0, cpDist = 0;
            if (stage.checkpoints.Count > 0)
            {
                var nextCpIndex = car.CurrentCheckpoint % stage.checkpoints.Count;
                var cp = stage.checkpoints[nextCpIndex];
                
                // Absolute relative vector
                var dx = (float)(cp.Position.X - car.Position.X);
                var dy = (float)(cp.Position.Y - car.Position.Y);
                var dz = (float)(cp.Position.Z - car.Position.Z);
                
                cpDist = MathF.Sqrt(dx * dx + dy * dy + dz * dz);

                // Car-relative rotation (Yaw only for now)
                float carYawRel = (float)car.Rotation.Yaw.Radians;
                float cosRel = MathF.Cos(-carYawRel);
                float sinRel = MathF.Sin(-carYawRel);
                
                cpRelX = dx * cosRel - dz * sinRel;
                cpRelY = dy;
                cpRelZ = dx * sinRel + dz * cosRel;
                
                cpAngle = MathF.Atan2(cpRelX, cpRelZ);
            }

            float rampRelX = 0, rampRelY = 0, rampRelZ = 0, rampOri = 0, rampDist = float.MaxValue;
            foreach (var node in stage.nodes)
            {
                if (node.Kind == AiNodeKind.Ramp || node.Kind == AiNodeKind.FixRamp)
                {
                    var dx = (float)(node.Position.X - car.Position.X);
                    var dy = (float)(node.Position.Y - car.Position.Y);
                    var dz = (float)(node.Position.Z - car.Position.Z);
                    var d2 = dx * dx + dy * dy + dz * dz;
                    
                    if (d2 < rampDist)
                    {
                        rampDist = d2;
                        
                        float carYawRamp = (float)car.Rotation.Yaw.Radians;
                        float cosRamp = MathF.Cos(-carYawRamp);
                        float sinRamp = MathF.Sin(-carYawRamp);
                        
                        rampRelX = dx * cosRamp - dz * sinRamp;
                        rampRelY = dy;
                        rampRelZ = dx * sinRamp + dz * cosRamp;

                        // Orientation: Dot product of car forward and ramp forward
                        // (Assuming ramp forward is its Yaw)
                        float rampYaw = (float)node.Rotation.Yaw.Radians;
                        rampOri = MathF.Cos(rampYaw - carYawRamp); 
                    }
                }
            }
            if (rampDist == float.MaxValue) rampDist = 0;
            else rampDist = MathF.Sqrt(rampDist);

            // --- Velocity & Rotation Logic ---
            float carYaw = (float)car.Rotation.Yaw.Radians;
            float cos = MathF.Cos(-carYaw);
            float sin = MathF.Sin(-carYaw);

            // Calculate average world velocity from the 4 wheels
            float avgWorldX = 0, avgWorldY = 0, avgWorldZ = 0;
            for (int i = 0; i < 4; i++)
            {
                avgWorldX += (float)mad.Scx[i];
                avgWorldY += (float)mad.Scy[i];
                avgWorldZ += (float)mad.Scz[i];
            }
            avgWorldX /= 4f; avgWorldY /= 4f; avgWorldZ /= 4f;

            // Transform to Local Space (Forward/Right/Up)
            // Forward is +Z, Right is +X
            float localRight = avgWorldX * cos - avgWorldZ * sin;
            float localForward = avgWorldX * sin + avgWorldZ * cos;
            float localUp = avgWorldY;

            // Rotational rates (Delta rotation)
            // mad.Ucomp/Dcomp/Lcomp/Rcomp represent rotational forces/velocities
            float yawRate = (float)mad.Txz; // Spinning rate
            float pitchRate = (float)((float)mad.Pzy - (float)car.Rotation.Pitch.Degrees); // Rough approximation
            float rollRate = (float)((float)mad.Pxy - (float)car.Rotation.Roll.Degrees);

            var packet = new TelemetryPacket
            {
                // Self State
                ForwardVel = localForward,
                RightVel = localRight,
                UpVel = localUp,
                YawRate = yawRate,
                PitchRate = pitchRate,
                RollRate = rollRate,
                Speed = (float)mad.Speed,
                PosX = (float)car.Position.X,
                PosY = (float)car.Position.Y,
                PosZ = (float)car.Position.Z,

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

                // Navigation
                CheckpointRelX = cpRelX,
                CheckpointRelY = cpRelY,
                CheckpointRelZ = cpRelZ,
                CheckpointAngle = cpAngle,
                CheckpointDistance = cpDist,
                RampRelX = rampRelX,
                RampRelY = rampRelY,
                RampRelZ = rampRelZ,
                RampOrientation = rampOri,
                RampDistance = rampDist,
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

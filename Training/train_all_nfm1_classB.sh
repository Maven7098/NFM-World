#!/bin/bash

# Configuration
# High Rider is classified as a Class A car, thus it is excluded from this training batch
CARS=(
	"nfmm/formula7"
	"nfmm/maxrevenge"
	"nfmm/leadoxide"
	"nfmm/koolkat"
	"nfmm/drifter"
	"nfmm/policecops"
)
STAGES=(
    "nfm1/1_introductory"
    "nfm1/2_contrary"
    "nfm1/3_snakedance"
    "nfm1/4_grapefruit"
    "nfm1/5_heiscoming"
    "nfm1/6_paninaro"
    "nfm1/7_whenindanger"
    "nfm1/8_radical"
    "nfm1/9_beacharcade"
    "nfm1/10_confusion"
    "nfm1/11_madparty"
)

# Move to project root
cd "$(dirname "$0")/.."

for CAR in "${CARS[@]}"; do
for STAGE in "${STAGES[@]}"; do
    echo "=========================================================="
    echo "Starting Training for Car: $CAR | Stage: $STAGE"
    echo "=========================================================="

    # 1. Start C# Training App in the background
    # Redirect output to a log file or /dev/null to keep console clean
    dotnet run --project TrainingApp/TrainingApp.csproj "$CAR" "$STAGE" > "training_log_${STAGE//\//_}.txt" 2>&1 &
    CS_PID=$!

    # 2. Run Python Training
    # train.py will finish after the total_timesteps set in the script
    # Then it will call env.close(), which sends 0xFE to shutdown the C# app
    python3 Training/train.py

    # 3. Wait for C# app to finish gracefully
    wait $CS_PID

    echo "Finished $STAGE. Moving to next track..."
    echo ""
done
echo "All tracks completed!"
done


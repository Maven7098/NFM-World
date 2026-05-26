#!/bin/bash

# Configuration
# M A S H E E N is excluded due to its 'outlier' stats
# This can corrupt/disrupt training of other cars
CARS=(
	"nfmm/mustang"
	"nfmm/king"
	"nfmm/audir8"
	"nfmm/radicalone"
	"nfmm/drmonster"
)
STAGES=(
    "1_introductory.txt"
    "2_letthedream.txt"
    "3_arrested.txt"
    "4_twisted.txt"
    "5_centrifugal.txt"
    "6_stretch.txt"
    "7_garden.txt"
    "8_maximum.txt"
    "9_majestic.txt"
    "10_ghosts.txt"
    "11_rolling.txt"
    "12_santas.txt"
    "13_diggers.txt"
    "14_gunrun.txt"
    "15_dwm.txt"
    "16_4dv.txt"
    "17_madparty.txt"
)

# Move to project root
cd "$(dirname "$0")/.."

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

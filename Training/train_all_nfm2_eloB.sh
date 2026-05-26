#!/bin/bash

# Configuration
CARS=(
    "elo/b_br_drifterx.rad"
    "elo/b_br_hakosuka.rad"
    "elo/b_br_koolkat.rad"
    "elo/b_br_lavitacrab.rad"
    "elo/b_br_lmp.rad"
    "elo/b_br_nimi.rad"
    "elo/b_br_theraven.rad"
    "elo/b_br_tornadoshark.rad"
    "elo/b_bw_caninaro.rad"
    "elo/b_bw_fenrir.rad"
    "elo/b_bw_hound.rad"
    "elo/b_bw_ironclad.rad"
    "elo/b_bw_leadoxide.rad"
    "elo/b_bw_maxrevenge.rad"
    "elo/b_bw_quadraturbo.rad"
    "elo/b_bw_revonttr.rad"
    "elo/b_bw_soj.rad"
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

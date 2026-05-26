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


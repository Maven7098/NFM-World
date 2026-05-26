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
    "nfm2/1_introductory.txt"
    "nfm2/2_letthedream.txt"
    "nfm2/3_arrested.txt"
    "nfm2/4_twisted.txt"
    "nfm2/5_centrifugal.txt"
    "nfm2/6_stretch.txt"
    "nfm2/7_garden.txt"
    "nfm2/8_maximum.txt"
    "nfm2/9_majestic.txt"
    "nfm2/10_ghosts.txt"
    "nfm2/11_rolling.txt"
    "nfm2/12_santas.txt"
    "nfm2/13_diggers.txt"
    "nfm2/14_gunrun.txt"
    "nfm2/15_dwm.txt"
    "nfm2/16_4dv.txt"
    "nfm2/17_madparty.txt"
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
        dotnet TrainingApp/bin/Release/net10.0/TrainingApp.dll "$CAR" "$STAGE" > "training_log_${STAGE//\//_}.txt" 2>&1 &
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
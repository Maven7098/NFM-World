## How to use train_all files
# Build C# Training App once
echo "Building C# Training App in Release mode..."
dotnet build TrainingApp/TrainingApp.csproj -c Release

# Execute them
./train_all.sh
Currently the cars and stages are hardcoded, with the following combinations:
- Train only Radical One in NFM1 and NFM2 tracks
train_all.sh
- Train Vanilla Class A cars in NFM1 tracks (MASHEEN is excluded)
train_all_nfm1_classA.sh
- Train Vanilla Class B cars in NFM1 tracks (F7 is added, High Rider is excluded)
train_all_nfm1_classB.sh
- Train ELO Class A cars in NFM1 tracks
train_all_nfm1_eloA.sh
- Train ELO Class B cars in NFM1 tracks
train_all_nfm1_eloB.sh
- Train Vanilla Class A cars in NFM2 tracks (MASHEEN is excluded)
train_all_nfm2_classA.sh
- Train Vanilla Class B cars in NFM2 tracks (F7 is added, High Rider is excluded)
train_all_nfm2_classB.sh
- Train ELO Class A cars in NFM2 tracks
train_all_nfm2_eloA.sh
- Train ELO Class B cars in NFM2 tracks
train_all_nfm2_eloB.sh
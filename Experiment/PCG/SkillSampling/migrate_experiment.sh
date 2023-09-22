cd /workspace/GeneratedSample
for folder in pcg_*; do mv "$folder" "${folder/pcg_/skillsampling_}"; done
rm -rf ./*/*seed*.csv
#!/bin/bash

cuda=$1
num_run=$2
run_yaml=$3
build_dir=$4
json_path=$5

for run in $(seq $num_run); do
  folderName="${run_yaml:(-6):1}"
  result_dir=/app/data/MA/RL_GENERALIZATION/
  CUDA_VISIBLE_DEVICES=$cuda mlagents-learn $run_yaml --env=$build_dir --run-id=range${folderName}_run${run} --results-dir=$result_dir --num-envs=35 --no-graphics --env-args --skillPath $json_path
done
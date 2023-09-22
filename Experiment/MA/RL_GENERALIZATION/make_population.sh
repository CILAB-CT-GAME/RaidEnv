#!/bin/bash

num_population=$1
run_yaml=$2

build_dir=$3
json_path=$4
cuda=$5
for population in $(seq $num_population); do
  folderName="${json_path:(-6):1}"
  result_dir="/data/rl_generalization/population/"
  CUDA_VISIBLE_DEVICES=$cuda mlagents-learn $run_yaml --env=$build_dir --run-id=range${folderName}_population${population} --results-dir=$result_dir --num-envs=35 --no-graphics --env-args --skillPath $json_path
done
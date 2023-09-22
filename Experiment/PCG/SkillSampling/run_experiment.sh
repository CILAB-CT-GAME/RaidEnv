#!/bin/bash

mkdir exp_logs

exp_idx=0
num_gpus=$(nvidia-smi --list-gpus | wc -l)

for file in ./generated/*.yaml; do
	 echo $file

	 exp_name=$(echo $file | sed "s/.yaml//")
	 exp_name=$(echo $exp_name | sed "s/.\/generated\///")

   ((exp_idx++))
   allocated_gpu=$(($exp_idx % $num_gpus))

	 nohup docker run --rm -t --name skillsampling_$exp_name -v $(pwd):/config -v $(pwd)/../../../Project/Build/Linux:/game -v /mnt/nas/MMORPG/PCG/GeneratedSample:/workspace/results inchang/ct_game /bin/bash -c "chmod -R 755 /game && CUDA_VISIBLE_DEVICES=$allocated_gpu mlagents-learn /config/$file --env /game/MMORPG.x86_64 --no-graphics --num-envs 16 --run-id $exp_name" > ./exp_logs/$exp_name.log 2>&1 &
   sleep 5
done

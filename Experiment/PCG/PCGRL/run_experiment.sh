#!/bin/bash

mkdir exp_logs

exp_idx=0
num_gpus=$(nvidia-smi --list-gpus | wc -l)
custom_yaml_filename='.temporary.yaml'

for file in ./generated/*.yaml; do
	 echo $file

	 exp_name=$(echo $file | sed "s/.yaml//")
	 exp_name=$(echo $exp_name | sed "s/.\/generated\///")

  _file=$file


  if [ "$1" ]; then
    exp_seed="_$1"
    _file=$custom_yaml_filename
    cp $file $_file
    sed -i "s/$exp_name/$exp_name$exp_seed/" $_file

  else
    exp_seed=""
  fi

   ((exp_idx++))
   allocated_gpu=$(($exp_idx % $num_gpus))

	 nohup docker run --rm -t --gpus all --name $exp_name$exp_seed -v $(pwd):/config -v $(pwd)/../../../Project/Build/Linux:/game -v /mnt/nas/MMORPG/PCG/PCGRL:/workspace/results inchang/ct_game /bin/bash -c "chmod -R 755 /game && CUDA_VISIBLE_DEVICES=$allocated_gpu mlagents-learn /config/$_file --env /game/MMORPG.x86_64 --num-envs 16 --no-graphics --run-id $exp_name$exp_seed" > ./exp_logs/$exp_name$exp_seed.log 2>&1 &
   sleep 5
done

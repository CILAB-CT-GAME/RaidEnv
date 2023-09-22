docker run --rm -d --gpus all --name pcgtb -p 6006:6006 -v /mnt/nas/MMORPG/PCG/PCGRL:/workspace/results wilf93/ct_game /bin/bash -c "tensorboard --logdir /workspace/results --host 0.0.0.0"

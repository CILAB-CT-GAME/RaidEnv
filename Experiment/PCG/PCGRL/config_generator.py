from pprint import pprint
from tqdm import tqdm
import os
from os import path

PARAMETERS = {
    'winRate': {
        'target': [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7],
        'weight': 1.0
    },
}
PREFIX = 'pcg'
GENERATE_DIRNAME = 'generated'
with open("base_config.yaml.example", "r") as f:
    config_str = f.read()

PARAMETER_LIST = list()

os.makedirs(GENERATE_DIRNAME, exist_ok=True)


def recursive(config, depth):

    config_keys = list(PARAMETERS.keys())

    if len(config_keys) == 0:
        return list()

    cur_name = config_keys[0]
    cur_data = PARAMETERS[cur_name]

    del config[cur_name]

    ret = recursive(PARAMETERS, depth + 1)
    output = list()

    for data in cur_data['target']:
        output.append([(cur_name, data, cur_data['weight'])])

    total = list()
    for i in output:
        if len(ret) != 0:
            for j in ret:
                total.append(i + j)
        else:
            total = output

    return total
result = recursive(PARAMETERS, 0)

for params in tqdm(result):
    str_params_arr = list()
    str_params_lined = list()

    for param in params:
        str_param = f"{param[0]}-{param[1]}-{param[2]}"

        str_params_lined.append(f'  {PREFIX}_target_{param[0]}: {param[1]}\n')
        str_params_lined.append(f'  {PREFIX}_weight_{param[0]}: {param[2]}\n')

        str_params_arr.append(str_param)

    str_params_concated = '_'.join(str_params_arr)
    str_params_lined_concated = ''.join(str_params_lined)

    run_id = f'{PREFIX}_{str_params_concated}'
    filename = f'{run_id}.yaml'

    with open(path.join(GENERATE_DIRNAME, filename), 'w') as f:
        param_config_str = config_str.replace('[!EXPERIMENT_PARAMETERS!]', str_params_lined_concated)
        param_config_str = param_config_str.replace('[!RUN_ID!]', run_id)
        f.write(param_config_str)
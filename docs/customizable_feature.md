| Parameter Category      | Parameter Type  | Parameter Name       | Data Type | Range         | Description                                                     |
|------------------------|-----------------|----------------------|-----------|---------------|-----------------------------------------------------------------|
| Character Statistics    | Status          | Health Point         | integer   | [0, 1000]     | Maximum number that agent can be dealing with the damage      |
|                        |                 | Manna Point          | integer   | [0, 100]      | The resource that used when agent active a skill               |
|                        |                 | Spell Power          | integer   | [0, 100]      | Reference number used when calculating the skill damage        |
|                        |                 | Movement Speed       | float     | [1, 2]        | Move speed of the agent                                        |
|                        | Attack          | Power                | integer   | [0, 100]      | Reference number used when calculating the melee attack damage |
|                        |                 | Range                | integer   | [1, 10]       | Maximum distance of the melee attack can be reached            |
|                        |                 | Speed                | float     | [1, 2]        | Number of melee attacks can be activated per second            |
|                        | Defensive        | Armor                | integer   | [0, 100]      | Ratio that agent dealt reduced damage                           |
|                        |                 | Evasion              | integer   | [0, 100]      | Probability that agent can dodge melee attack or skill         |
|                        |                 | Parry                | float     | [0, 100]      | Probability that agent can block melee attack or skill         |
|                        | Primary         | Strength             | integer   | [1, 100]      | Measure of physical power and proficiency                      |
|                        |                 | Agility              | integer   | [1, 100]      | Measure of swiftness                                           |
|                        |                 | Intelligence         | integer   | [1, 100]      | Measure of mental power and magical prowess                    |
|                        | Secondary       | Critical             | integer   | [0, 100]      | Chance that agent ability can be more effective than normal    |
|                        |                 | Haste                | integer   | [0, 100]      | Determine the agent's action swiftness                         |
|                        |                 | Versatility          | integer   | [0, 100]      | Increase the agent's attack and defensive abilities effect     |
|                        |                 | Mastery              | integer   | [0, 100]      | Enhance some abilities effect to emphasize character class     |
| Skill                  | Information     | Name                 | string    | -             | Name given to each skill                                       |
|                        |                 | Trigger Type         | enum      | [0, 1]        | Determine the skill is active skill or passive skill           |
|                        |                 | Magic School         | enum      | [0, 9]        | Determine the visual effects of the skill (e.g. fire, frost)   |
|                        |                 | Hit Type             | enum      | [0, 1]        | Determine if the skill is a melee attack or skill              |
|                        |                 | Target Type          | enum      | [0, 2]        | Type of target which the skill can be used on                 |
|                        |                 | Projectile Speed     | float     | [0, 50]       | Speed of the projectile that reaches the target                |
|                        |                 | Affect on Ally       | bool      | True/False    | Determine if an agent can activate a skill on an ally           |
|                        |                 | Affect on Enemy      | bool      | True/False    | Determine if an agent can activate a skill on an enemy          |
|                        | Condition       | Cool Time            | float     | [0, 60]       | Amount of time required to activate the skill again            |
|                        |                 | Cast Time            | float     | [0, 2]        | Amount of time required to execute the skill after activation  |
|                        |                 | Cost                 | float     | [0, 100]      | Amount of Mana required to activate the skill                  |
|                        |                 | Range                | float     | [1, 20]       | Maximum distance of the skill can be reached                   |
|                        |                 | Charge               | integer   | [1, 3]        | Number of skill activations regardless of the Cool time        |
|                        |                 | Cast on moving       | bool      | True/False    | Determine if an agent can activate a skill during movement      |
|                        |                 | Cast on Casting      | bool      | True/False    | Determine if an agent can activate a skill during casting       |
|                        |                 | Cast on Channeling   | bool      | True/False    | Determine if an agent can activate a skill during executing     |
|                        | Coefficient    | Value                | float     | [0, 2]        | Scaling factor that determines the damage of the skill         |

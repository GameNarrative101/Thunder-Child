# Thunder Child
Thunder Child is an isometric, turn-based RPG mech fighter made in Unity. The player manages 3 resources: shield (or health); power, which is generated every turn; and heat, which is generated when power is spent. Remember to use your *Heat Sink* or you'll overheat and die! Otherwise, use weapons like the *Laser Minigun* or *Falconnet Cannon* to fight while you find, secure, and extract the Prometheus Core. [Miro Board documenting ideation and design](https://miro.com/app/board/uXjVJLD1NOA=/?share_link_id=679916103747)

https://github.com/user-attachments/assets/2e66aa6b-82da-4b08-81b5-43efca26f733


## 3-tier ability roll randomization
Missing feels bad! With the 3-tier roll, you can still have a bad turn (roll low), but not a useless one. But, since enemies don't miss either, this randomization methood puts a timer on combat. Kill your enemies first or die, no standstill. 

    public PowerRollTier Roll(int bonusModifier = 0) //Can receive a bonus modifier from actions
    {
        int roll1 = Random.Range(1, 11); // 1 to 10 inclusive
        int roll2 = Random.Range(1, 11);
        int total = roll1 + roll2 + rollBonus + bonusModifier;

        PowerRollTier tier;

        if (total <= 11) tier = PowerRollTier.Tier1;
        else if (total <= 16) tier = PowerRollTier.Tier2;
        else tier = PowerRollTier.Tier3;

        return tier;
    }

## Level completed
<img width="1478" height="828" alt="Success!" src="https://github.com/user-attachments/assets/5e5caaf2-7f3f-4b6b-b1dc-e2cf120a2146" />

## Death screen
<img width="1920" height="1078" alt="You died" src="https://github.com/user-attachments/assets/4cfde358-3984-470e-84a0-d8dbd750701c" />  

## Various Screenshots
<img width="1920" height="1082" alt="Laser Minigun" src="https://github.com/user-attachments/assets/555ffffb-b17f-4fa6-af92-9d645121c8b0" />
<img width="1924" height="1088" alt="Melee" src="https://github.com/user-attachments/assets/23ef7742-1edf-491b-9b46-a690b6626a1b" />
<img width="1923" height="1084" alt="Anti-Materiel Sniper Rifle" src="https://github.com/user-attachments/assets/fb165b95-13a8-47a9-b7fb-9a5aff74c9f4" />
<img width="1920" height="1083" alt="Explosion" src="https://github.com/user-attachments/assets/18d866bd-d73b-40ad-8746-4c211e33a0e5" />

## Miro Board for 

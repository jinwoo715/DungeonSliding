using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.Stat
{
    public class StatCopyAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;
        IStatReadOnly _ownerStat;
        IStatModifier _ownerStatModifier;
        public StatCopyAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            IStatReadOnly playerStat = _sensor.PlayerCombatant.StatReadOnly;

            int playerHP = playerStat.Get(ECreatureStatType.CurrentHP);
            int playerDamage = playerStat.Get(ECreatureStatType.Damage);

            int copyHPValue = Mathf.RoundToInt(playerHP * P1);
            int copyDamageValue = Mathf.RoundToInt(playerDamage * P2);

            int ownerHP = _ownerStat.Get(ECreatureStatType.CurrentHP);
            int ownerDamage = _ownerStat.Get(ECreatureStatType.Damage);

            int diffHP = copyHPValue - ownerHP;
            int diffDamage = copyDamageValue - ownerDamage;
            
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, diffHP));
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, diffDamage));

            yield break;
        }
        protected override void BindService()
        {
            BindService(ref _sensor);
            _ownerStat = _owner.StatReadOnly;
            _ownerStatModifier = _owner.StatModifier;
        }
    }
    //동족의 분노
    public class KindredRageAbility : EnemyAbilityBase
    {
        IStatModifier _ownerStatModifier;
        public KindredRageAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, P1));
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, P2));
            yield break;
        }
        
        protected override void BindService()
        {
            _ownerStatModifier = _owner.StatModifier;
        }
    }

   
    public class BerserkerAbility : EnemyAbilityBase
    {
        IStatModifier _statModifier;

        public BerserkerAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, P1));
            yield break;
        }

        protected override void BindService()
        {
            _statModifier = _owner.StatModifier;
        }
    }

    public class GrowthEnhancerAbility : EnemyAbilityBase
    {
        IStatModifier _statModifier;
        public GrowthEnhancerAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, P1));
            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, P2));

            yield break;
        }

        public override void ReleaseAbility()
        {
        }

        protected override void BindService()
        {
            _statModifier = _owner.StatModifier;
        }
    }

    public class IncitementAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;

        public IncitementAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            var enemies = _sensor.AllEnemyCombatants;

            foreach (var enemy in enemies)
            {
                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, P1));

                int maxHP = enemy.StatReadOnly.Get(ECreatureStatType.MaxHp);
                int addHP = Mathf.RoundToInt(maxHP * P2);

                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.MaxHp, EApplyStatType.Multiple, addHP));
            }

            yield break;
        }

        public override void ReleaseAbility()
        {
            var enemies = _sensor.AllEnemyCombatants;

            foreach (var enemy in enemies)
            {
                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, -P1));

                int currentHP = enemy.StatReadOnly.Get(ECreatureStatType.CurrentHP);
                int reduceHP = Mathf.RoundToInt(currentHP * P2);

                if (currentHP - reduceHP <= 0)
                    continue;

                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.MaxHp, EApplyStatType.Multiple, -reduceHP));
            }
        }

        protected override void BindService()
        {
            BindService(ref _sensor);
        }
    }
}

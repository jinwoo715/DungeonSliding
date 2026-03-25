using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.Enemy
{
    public class CopyPlayerStat : EnemyAbilityBase
    {
        ICombatantSensor _sensor;
        IStatReadOnly _ownerStat;
        IStatModifier _ownerStatModifier;
        IStatReadOnly _playerStat;
        public CopyPlayerStat(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            IStatReadOnly playerStat = _sensor.PlayerCombatant.StatReadOnly;

            int playerMaxHP = playerStat.Get(ECreatureStatType.MaxHp);
            int playerHP = playerStat.Get(ECreatureStatType.CurrentHP);
            int playerDamage = playerStat.Get(ECreatureStatType.Damage);

            
            float hpRatio = P1 * 0.01f;
            float damageRatio = P2 * 0.01f;

            int copyMax = Mathf.RoundToInt(playerMaxHP * hpRatio);
            int copyHPValue = Mathf.RoundToInt(playerHP * hpRatio);
            int copyDamageValue = Mathf.RoundToInt(playerDamage * damageRatio);

            int ownerMaxHP = _ownerStat.Get(ECreatureStatType.MaxHp);
            int ownerHP = _ownerStat.Get(ECreatureStatType.CurrentHP);
            int ownerDamage = _ownerStat.Get(ECreatureStatType.Damage);

            //현재가 40, 복사한 값이 20일 때,
            //현재가 20, 복사한 값이 40일 때,

            int maxDiff = copyMax - ownerMaxHP;
            int diffHP = copyHPValue - ownerHP;
            int diffDamage = copyDamageValue - ownerDamage;

            Debug.Log($"Copy HP : {copyHPValue}, Copy Dmg : {copyDamageValue}, Diff : {diffHP}");
            Debug.Log($"Current HP : {ownerHP}, Current Dmg : {ownerDamage}, Diff : {diffDamage}");

            //TODO 수정중
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.MaxHp, EApplyStatType.Add, maxDiff));
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, diffHP));
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, diffDamage));

            //SetMaxHP();
            //SetCurrentHP();
            //SetDamage();

            yield break;
        }

        private void SetMaxHP()
        {
            int diff = GetDiffValue(ECreatureStatType.MaxHp);

            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.MaxHp, EApplyStatType.Add, diff));
        }
        private void SetCurrentHP()
        {
            int diff = GetDiffValue(ECreatureStatType.CurrentHP);

            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, diff));
        }
        private void SetDamage()
        {
            int diff = GetDiffValue(ECreatureStatType.Damage);
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, diff));
        }

        private int GetDiffValue(ECreatureStatType type)
        {
            int playerCurrentHP = _playerStat.Get(type);
            int ownerCurrentHP = _ownerStat.Get(type);

            int diff = playerCurrentHP - ownerCurrentHP;
            return diff;
        }

        //플레이어의 최대 체력
        //플레이어의 현재 체력
        //플레이어의 공격력

        protected override void BindService()
        {
            BindService(ref _sensor);
            _ownerStat = _owner.StatReadOnly;
            _ownerStatModifier = _owner.StatModifier;
            _playerStat = _sensor.PlayerCombatant.StatReadOnly;
        }
    }

    public class EnhanceStat : EnemyAbilityBase
    {
        IStatModifier _ownerStatModifier;
        public EnhanceStat(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            Debug.Log(P1);
            Debug.Log(P2);
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.MaxHp, EApplyStatType.Add, P1));
            _ownerStatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, P2));

            yield break;
        }

        protected override void BindService()
        {
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
    public class Exaltation : EnemyAbilityBase
    {
        ICombatantSensor _sensor;

        public Exaltation(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            var enemies = _sensor.AllEnemyCombatants;

            float hpRatio = P2 * 0.01f;
            float damageRatio = P1 * 0.01f;

            foreach (var enemy in enemies)
            {
                if (enemy == _owner) continue;

                Debug.Log("Excute");

                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, damageRatio));

                int maxHP = enemy.StatReadOnly.Get(ECreatureStatType.MaxHp);
                int addHP = Mathf.RoundToInt(maxHP * P2);

                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.MaxHp, EApplyStatType.Multiple, hpRatio));
            }

            yield break;
        }

        public override void ReleaseAbility()
        {
            var enemies = _sensor.AllEnemyCombatants;

            float hpRatio = P2 * 0.01f;
            float damageRatio = P1 * 0.01f;

            foreach (var enemy in enemies)
            {

                //공격력 감소
                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, -damageRatio));
                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.MaxHp, EApplyStatType.Multiple, -hpRatio));

                //체력 감소 -> max는 그대로 ratio감소, 현재 체력은 현재 체력에 비례한 %감소
                int currentHP = enemy.StatReadOnly.Get(ECreatureStatType.CurrentHP);
                int reduceHP = Mathf.RoundToInt(currentHP * hpRatio);

                enemy.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, -reduceHP));
            }
        }

        protected override void BindService()
        {
            BindService(ref _sensor);
        }
    }
}

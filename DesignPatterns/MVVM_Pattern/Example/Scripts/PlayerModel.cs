using UnityModularScripts.DesignPatterns.MVVM_Pattern;

namespace TestScripts.Test_MVVM
{
    public class PlayerModel : BaseModel
    {
        private int _level = 1;
        private int _currentExp = 0;
        private int _maxExp => 10 + (_level * 2);  // 최대 경험치는 10 + 레벨 * 2

        public int Level
        {
            get => _level;
            set => SetField(ref _level, value);
        }

        public int CurrentExp
        {
            get => _currentExp;
            set => SetField(ref _currentExp, value);
        }

        public int MaxExp => _maxExp;

        public void GainExp(int amount)
        {
            CurrentExp += amount;
            if (CurrentExp >= MaxExp)
            {
                CurrentExp -= MaxExp;
                Level++;
            }
        }
    }
}
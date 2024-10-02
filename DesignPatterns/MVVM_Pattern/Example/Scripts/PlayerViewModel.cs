using UnityModularScripts.DesignPatterns.MVVM_Pattern;

namespace TestScripts.Test_MVVM
{
    public class PlayerViewModel : BaseViewModel
    {
        private readonly PlayerModel _playerModel;

        public PlayerViewModel(PlayerModel model)
        {
            _playerModel = model;
        }

        public int Level => _playerModel.Level;
        public int CurrentExp => _playerModel.CurrentExp;
        public int MaxExp => _playerModel.MaxExp;

        // 경험치 증가 메서드
        public void GainExp()
        {
            _playerModel.GainExp(3);  // 경험치 3씩 증가
            OnPropertyChanged(nameof(Level));  // 레벨 변경 알림
            OnPropertyChanged(nameof(CurrentExp));  // 현재 경험치 변경 알림
            OnPropertyChanged(nameof(MaxExp));  // 최대 경험치 변경 알림
            Log("Experience gained. Level: " + Level + ", CurrentExp: " + CurrentExp);
        }
    }
}
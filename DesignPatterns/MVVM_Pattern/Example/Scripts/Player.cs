using UnityEngine;

namespace TestScripts.Test_MVVM
{
    public class Player : MonoBehaviour
    {
        public PlayerView playerView;

        private void Start()
        {
            // PlayerModel 및 PlayerViewModel을 생성하고 연결
            var playerModel = new PlayerModel();
            var playerViewModel = new PlayerViewModel(playerModel);
            playerView.BindViewModel(playerViewModel);
        }
    }
}
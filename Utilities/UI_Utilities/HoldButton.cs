using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityModularScripts.Utilities.UI_Utilities
{
    /// <summary>
    /// HoldButton 클래스는 사용자가 버튼을 눌렀을 때 일정 시간 이후 지속적으로 이벤트를 발생시키는 기능을 제공합니다.
    /// 유니티의 기본 버튼과 유사하지만, 사용자가 버튼을 꾹 누르고 있을 때 주기적으로 이벤트가 발생하는 로직을 구현합니다.
    /// </summary>
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 첫 번째 클릭 후, 반복이 시작되기까지의 지연 시간을 설정합니다.
        /// </summary>
        [Range(0f, 3f)] [SerializeField] private float initialPressDelay = 0.5f; // 첫 클릭 후 반복 시작까지의 지연 시간

        /// <summary>
        /// 클릭이 반복되는 주기를 설정합니다. 즉, 클릭이 얼마나 자주 발생할지를 설정하는 간격입니다.
        /// </summary>
        [Range(0f, 3f)] [SerializeField] private float repeatClickInterval = 0.1f; // 반복 클릭 간격

        /// <summary>
        /// 사용자가 버튼을 꾹 누르고 있을 때 "홀드"로 인식되는 시간을 설정합니다. 이 시간이 지나면 홀드 이벤트가 발생합니다.
        /// </summary>
        [SerializeField] private float pressAndHoldThreshold = 0.5f; // 첫 클릭 후 '홀드'로 인식되는 시간

        /// <summary>
        /// 사용자가 버튼을 꾹 누르고 있을 때 반복적으로 발생하는 이벤트입니다.
        /// </summary>
        public event Action onHold; // 지속적인 클릭 이벤트

        /// <summary>
        /// 사용자가 버튼을 눌렀을 때 발생하는 이벤트입니다. 버튼이 처음 눌렸을 때 한 번만 발생합니다.
        /// </summary>
        public event Action onPressStart; // 첫 클릭 이벤트

        /// <summary>
        /// 사용자가 버튼에서 손을 뗐을 때 발생하는 이벤트입니다.
        /// </summary>
        public event Action onPressEnd; // 클릭 종료 이벤트

        private bool isHoldPressed; // 버튼이 꾹 눌려져 있는지 여부를 확인하는 플래그
        private DateTime pressStartTime; // 버튼이 처음 눌린 시간

        /// <summary>
        /// 사용자가 버튼을 처음 눌렀을 때 호출되는 메서드입니다.
        /// </summary>
        /// <param name="eventData">PointerEventData는 입력된 포인터의 세부 정보를 제공합니다.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            isHoldPressed = true;
            pressStartTime = DateTime.Now;

            onPressStart?.Invoke(); // 첫 클릭 이벤트 호출

            StartCoroutine(HoldRoutine()); // 버튼이 눌린 상태에서 반복 실행을 관리하는 코루틴 시작
        }

        /// <summary>
        /// 사용자가 버튼에서 손을 뗐을 때 호출되는 메서드입니다.
        /// </summary>
        /// <param name="eventData">PointerEventData는 입력된 포인터의 세부 정보를 제공합니다.</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            isHoldPressed = false;
            onPressEnd?.Invoke(); // 클릭 종료 이벤트 호출
        }

        /// <summary>
        /// 사용자가 버튼을 꾹 누르고 있을 때, 설정된 간격으로 이벤트를 발생시키는 코루틴입니다.
        /// </summary>
        /// <returns>코루틴으로 시간 간격에 따라 반복 작업을 수행합니다.</returns>
        private IEnumerator HoldRoutine()
        {
            // 첫 클릭 후 일정 시간이 지난 후에 반복 시작
            yield return new WaitForSeconds(initialPressDelay);

            while (isHoldPressed) // 버튼이 꾹 눌려져 있는 동안 실행
            {
                var elapsedSeconds = (DateTime.Now - pressStartTime).TotalSeconds;

                if (elapsedSeconds >= pressAndHoldThreshold)
                {
                    onHold?.Invoke(); // 홀드 이벤트 발생
                    yield return new WaitForSeconds(repeatClickInterval); // 반복 클릭 간격만큼 대기
                }
                else
                {
                    yield return null; // 대기 상태
                }
            }
        }
    }
}
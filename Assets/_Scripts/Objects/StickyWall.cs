using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyWall : MonoBehaviour
{
    private PlayerStateMachine _playerStateMachine = null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
            PlayerStateManager.Instance.ChangeState(PlayerState.OnStickyWall);

        if(collision.transform.TryGetComponent<PlayerStateMachine>(out _playerStateMachine))
            _playerStateMachine.isHittingStickyWall = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            if(PlayerStateManager.Instance.CurrentState == PlayerState.OnStickyWall)
                PlayerStateManager.Instance.ChangeState(PlayerState.Default);

        if(_playerStateMachine != null)
        {
            _playerStateMachine.isHittingStickyWall = false;
            _playerStateMachine = null;
        }
    }
}

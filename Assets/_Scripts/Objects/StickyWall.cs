using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyWall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
            PlayerStateManager.Instance.ChangeState(PlayerState.OnStickyWall);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            if(PlayerStateManager.Instance.CurrentState == PlayerState.OnStickyWall)
                PlayerStateManager.Instance.ChangeState(PlayerState.Default);
    }
}

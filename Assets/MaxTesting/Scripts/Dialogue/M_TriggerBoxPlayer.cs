using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_TriggerBoxPlayer : MonoBehaviour
{
    private Collider2D _collider2D = new Collider2D();
    private bool draw;
    [SerializeField] private LayerMask _layerMask;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            RaycastHit2D[] hit = Physics2D.LinecastAll(transform.position, col.transform.position);
            _collider2D = col;
            draw = true;
            if (hit[0] == GetComponent<Collider2D>())
            {
                for (int i = 1; i < hit.Length; i++)
                {
                    if (hit[i].collider == col)
                    {
                        col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = true;
                    }
                    else
                    {
                        col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider == col)
                    {
                        col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = true;
                    }
                    else
                    {
                        col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
                        return;
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Talkable")) return;
        RaycastHit2D[] hit = Physics2D.LinecastAll(transform.position, col.transform.position);
        _collider2D = col;
        draw = true;
        if (hit[0] == GetComponent<Collider2D>())
        {
            for (int i = 1; i < hit.Length; i++)
            {
                if (hit[i].collider == col)
                {
                    col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = true;
                }
                else
                {
                    col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider == col)
                {
                    col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = true;
                }
                else
                {
                    col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
                    return;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
            draw = false;
        }
    }


    private void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.DrawLine(transform.position, _collider2D.transform.position);
        }
        
    }
}

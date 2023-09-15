using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoxPlayer : MonoBehaviour
{
    private Collider2D _collider2D = new Collider2D();
    private bool draw;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, col.bounds.ClosestPoint(transform.position));
            _collider2D = col;
            draw = true;
            Debug.Log(hit.collider);
            if (hit.collider == col)
           {
               col.gameObject.GetComponent<DialogueTrigger>().PlayerInRange = true;
           }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, col.bounds.ClosestPoint(transform.position));
            _collider2D = col;
            draw = true;
            Debug.Log(hit.collider);
            if (hit.collider == col)
            {
                col.gameObject.GetComponent<DialogueTrigger>().PlayerInRange = true;
            }
            else
            {
                col.gameObject.GetComponent<DialogueTrigger>().PlayerInRange = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            col.gameObject.GetComponent<DialogueTrigger>().PlayerInRange = false;
            draw = false;
        }
    }


    private void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.DrawLine(transform.position, _collider2D.bounds.ClosestPoint(transform.position));
        }
        
    }
}

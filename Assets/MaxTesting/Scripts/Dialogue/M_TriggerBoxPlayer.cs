using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_TriggerBoxPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            RaycastHit2D[] hit = Physics2D.LinecastAll(transform.position, col.transform.position);
            List<GameObject> raySave = new List<GameObject>();
            foreach (RaycastHit2D hit2D in hit)
            {
               raySave.Add(hit2D.transform.gameObject); 
            }
            for (int i = 0; i < raySave.Count; i++)
            {
                if (raySave[i] == gameObject)
                {
                    raySave.RemoveAt(i);
                    i--;
                }
            }
            foreach (GameObject save in raySave)
            {
                print(save.name);
            }

            if (raySave[0].CompareTag("Talkable"))
            {
                col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = true;
            }
            else
            {
                col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            RaycastHit2D[] hit = Physics2D.LinecastAll(transform.position, col.transform.position);
            List<GameObject> raySave = new List<GameObject>();
            foreach (RaycastHit2D hit2D in hit)
            {
                raySave.Add(hit2D.transform.gameObject); 
            }
            for (int i = 0; i < raySave.Count; i++)
            {
                if (raySave[i] == gameObject)
                {
                    raySave.RemoveAt(i);
                    i--;
                }
            }
            foreach (GameObject save in raySave)
            {
                print(save.name);
            }

            if (raySave[0].CompareTag("Talkable"))
            {
                col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = true;
            }
            else
            {
                col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Talkable"))
        {
            col.gameObject.GetComponent<M_DialogueTrigger>().PlayerInRange = false;
        }
    }
}

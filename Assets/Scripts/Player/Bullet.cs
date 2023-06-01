using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Bullet : MonoBehaviour
{

    public GameObject explosionFx;

    private void Start()
    {
        StartCoroutine(DestroyOnSeconds());
    }

    //Check if the bullet have collision on the enemy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Explosion on hit
        GameObject hit;

        switch (collision.tag)
        {
            case "IBasic":
                //Instantiate explosion on collision
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                //In case of no shield enemy we take 1 enemy life point
                collision.gameObject.GetComponent<EnemiesAI>().currentHealth--;
                collision.gameObject.GetComponent<EnemiesAI>().enemiesAnim.SetTrigger("DamageOn");
                PlayerHealth.instance.playerPoints++;
                PlayerHealth.instance.playerShieldPoints++;
                Destroy(gameObject);
                break;
            case "IIMisile":
                //Instantiate explosion on collision
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                //In case of no shield enemy we take 1 enemy life point
                collision.gameObject.GetComponent<EnemiesAI>().currentHealth--;
                collision.gameObject.GetComponent<EnemiesAI>().enemiesAnim.SetTrigger("DamageOn");
                PlayerHealth.instance.playerPoints++;
                PlayerHealth.instance.playerShieldPoints++;
                Destroy(gameObject);
                break;
            case "IIShield":
                //Instantiate explosion on collision
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                //In case of shield enemy we take 1 enemy shield point
                collision.gameObject.GetComponent<EnemiesAI>().shield--;
                PlayerHealth.instance.playerPoints++;
                PlayerHealth.instance.playerShieldPoints++;
                //Then we check for shield points and activate the animation needed
                if (collision.gameObject.GetComponent<EnemiesAI>().shield > 0)
                {
                    collision.gameObject.GetComponent<EnemiesAI>().shieldObject.GetComponent<Animator>().SetTrigger("ShieldOnDamage");
                }
                else if(collision.gameObject.GetComponent<EnemiesAI>().shield == 0)
                {
                    collision.gameObject.GetComponent<EnemiesAI>().shieldObject.GetComponent<Animator>().SetTrigger("ShieldOff");
                }
                else
                {
                    //Once it has no more shield points start to take life points
                    collision.gameObject.GetComponent<EnemiesAI>().currentHealth--;
                    collision.gameObject.GetComponent<EnemiesAI>().enemiesAnim.SetTrigger("DamageOn");
                }
                Destroy(gameObject);
                break;
            case "IIIMisile":
                //Instantiate explosion on collision
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                //In case of no shield enemy we take 1 enemy life point
                collision.gameObject.GetComponent<EnemiesAI>().currentHealth--;
                collision.gameObject.GetComponent<EnemiesAI>().enemiesAnim.SetTrigger("DamageOn");
                PlayerHealth.instance.playerPoints++;
                PlayerHealth.instance.playerShieldPoints++;
                Destroy(gameObject);
                break;
            case "IIIShield":
                //Instantiate explosion on collision
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                //In case of shield enemy we take 1 enemy shield point
                collision.gameObject.GetComponent<EnemiesAI>().shield--;
                PlayerHealth.instance.playerPoints++;
                PlayerHealth.instance.playerShieldPoints++;
                //Then we check for shield points and activate the animation needed
                if (collision.gameObject.GetComponent<EnemiesAI>().shield > 0)
                {
                    collision.gameObject.GetComponent<EnemiesAI>().shieldObject.GetComponent<Animator>().SetTrigger("ShieldOnDamage");
                }
                else if (collision.gameObject.GetComponent<EnemiesAI>().shield == 0)
                {
                    collision.gameObject.GetComponent<EnemiesAI>().shieldObject.GetComponent<Animator>().SetTrigger("ShieldOff");
                }
                else
                {
                    //Once it has no more shield points start to take life points
                    collision.gameObject.GetComponent<EnemiesAI>().currentHealth--;
                    collision.gameObject.GetComponent<EnemiesAI>().enemiesAnim.SetTrigger("DamageOn");
                }
                Destroy(gameObject);
                break;
            case "Boss":
                //Instantiate explosion on collision
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                //In case of boss enemy we take 1 enemy shield point
                collision.gameObject.GetComponent<EnemiesAI>().shield--;
                PlayerHealth.instance.playerPoints++;
                PlayerHealth.instance.playerShieldPoints++;
                //Then we check for shield points and activate the animation needed
                if (collision.gameObject.GetComponent<EnemiesAI>().shield > 0)
                {
                    collision.gameObject.GetComponent<EnemiesAI>().shieldObject.GetComponent<Animator>().SetTrigger("ShieldOnDamage");
                }
                else if (collision.gameObject.GetComponent<EnemiesAI>().shield == 0)
                {
                    collision.gameObject.GetComponent<EnemiesAI>().shieldObject.GetComponent<Animator>().SetTrigger("ShieldOff");
                }
                else
                {
                    //Once it has no more shield points start to take life points
                    collision.gameObject.GetComponent<EnemiesAI>().currentHealth--;
                    collision.gameObject.GetComponent<EnemiesAI>().enemiesAnim.SetTrigger("DamageOn");
                }
                Destroy(gameObject);
                break;
            case "BulletIBasic":
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                Destroy(gameObject);
                break;
            case "Missile":
                hit = Instantiate(explosionFx, transform.position, transform.rotation);
                Destroy(hit, 1.6f);
                Destroy(gameObject);
                break;
        }

        //Destroy(gameObject);
    }

    private IEnumerator DestroyOnSeconds()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }





}
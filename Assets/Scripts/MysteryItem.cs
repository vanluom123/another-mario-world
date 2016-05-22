﻿using UnityEngine;
using System.Collections;

// created by Ho Hoang Tung
public class MysteryItem : MonoBehaviour {
    public enum Type { MYSTERY, BRICK, HIDE}
    public GameObject _itemPrefab;
    Animator _animator;

    private GameObject _item;

    public Type _type;
	// Use this for initialization
	void Start () {
        this._animator = GetComponent<Animator>();
        _animator.SetInteger("type", (int)_type);
        switch (_type)
        {
            case Type.MYSTERY:
                _animator.Play("Mystery Item");
                break;
            case Type.BRICK:
                _animator.Play("Mystery Brick");
                break;
            case Type.HIDE:
                _animator.Play("Mystery Hide");
                break;
        }
	    
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        // Kiểm tra bị player đội từ dưới  
        if (tag == "Player")
           checkHit(collision);

    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        string tag = collider.gameObject.tag;
        if (tag == "Enemy")
        {
            collider.gameObject.GetComponent<Enemy>()._aniamtor.SetInteger("status",(int) Enemy.eStatus.Hit);
        }
    }
    private void checkHit(Collision2D collision)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Normal") == false)
            return;
        // Nếu mario đang rơi thì không tính
        if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0)
            return;


        // Khoảng cách giữa vật mở item và player.
        // Dùng x và y để xác định hướng
        Vector3 distance = (this.transform.position - collision.gameObject.transform.position).normalized;

        // Nếu player đấm từ dưới lên thì mở item
        if (distance.y > 0)
        {
            //float top = collision.collider.bounds.max.y;
            float centerX = collision.collider.bounds.center.x;
            Collider2D thisCollider = this.GetComponent<Collider2D>();
            //if (top - thisCollider.bounds.min.y > 0.5)
            if (centerX > thisCollider.bounds.min.x && centerX < thisCollider.bounds.max.x)
            {
                // trigger hit làm nảy vật mở item lên
                this._animator.SetTrigger("hit");
                // Xác định player đội chệch về trái hay phải để xác định hướng chạy của item.
                if (distance.x < 0)
                {
                    (_itemPrefab.GetComponent<Item>() as Item)._speed.x = -Mathf.Abs((_itemPrefab.GetComponent<Item>() as Item)._speed.x);
                }
                else
                {
                    (_itemPrefab.GetComponent<Item>() as Item)._speed.x = Mathf.Abs((_itemPrefab.GetComponent<Item>() as Item)._speed.x);
                }
            }
        }
    }

}

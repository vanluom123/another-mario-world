﻿using UnityEngine;
using System.Collections;

// Created by Ho Hoang Tung
public class Troopa : Enemy {
    public enum eStatus { Normal, Shell, Hit, SpeedShell}
    private bool _flagHit;
    private bool _justSlide;

    private Vector3 _tempSpeed;
    protected override void Start()
    {

        base.Start();

        _imovement = new LinearMovement(_speed.x, _speed.y, _speed.z);
        //_hitbyplayer = new TroopaHitByPlayer();
        if ((_imovement as LinearMovement).Xspeed > 0)
            _aniamtor.SetBool("left", false);
        else
            _aniamtor.SetBool("left", true);
    }


    private const float _SHELLSTATCOUNTDOWN = 3.0f;
    private float _shell_stat_countdown;
    protected override void Update()
    {
        //if (_renderer.isVisible)
        //    _rigidBody2D.WakeUp();
        //else
        //{
        //    _rigidBody2D.Sleep();
        //}
        base.Update();

        if (_aniamtor.GetInteger("status") == (int)Troopa.eStatus.Shell)
        {
            _shell_stat_countdown -= Time.deltaTime;
            if (_shell_stat_countdown <= 0)
            {
                _shell_stat_countdown = _SHELLSTATCOUNTDOWN;
                _aniamtor.SetInteger("status", (int)Troopa.eStatus.Normal);
                this.SetSpeed(_tempSpeed);
            }
        }
        
    }

    void FixedUpdate()
    {
        InvokeRepeating("flagfalse", 1, 1);
    }

    private void flagfalse()
    {
        _flagHit = false;
        _justSlide = false;

    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Player")
        {
            int status = _aniamtor.GetInteger("status");
            switch (status)
            {
                case (int)eStatus.Normal:
                case (int)eStatus.SpeedShell:
                    if (_justSlide == false)
                        killPlayer(collision.gameObject);
                    break;
                case (int)eStatus.Shell:
                    collision.gameObject.GetComponent<MarioController>().kick();
                    speedSlide(collision.gameObject);
                    _justSlide = true;
                    break;
                case (int)eStatus.Hit:
                    return;
            }
            _flagHit = true;
        }
        else 
            base.OnCollisionEnter2D(collision);
    }

    public override void killPlayer(GameObject obj)
    {
        if (_justSlide == false)
            (obj.GetComponent<Mario>() as Mario).GotHit();

    }


    //protected override void OnTriggerEnter2D(Collider2D collider)
    //{
    //    int status = _aniamtor.GetInteger("status");
    //    switch (status)
    //    {
    //        case (int)eStatus.Normal:
    //            _aniamtor.SetInteger("status", (int)eStatus.Shell);
    //            break;
    //        case (int)eStatus.SpeedShell:
    //            _aniamtor.SetInteger("status", (int)eStatus.Shell);
    //            break;
    //        case (int)eStatus.Shell:
    //            return;
    //        case (int)eStatus.Hit:
    //            return;
    //    }
    //}

    protected override void checkHitByPlayer(Collider2D col)
    {
        if (this._aniamtor.GetInteger("status") == (int)eStatus.Shell)
        {
            speedSlide(col.gameObject);
        }
        else
            base.checkHitByPlayer(col);
        _isDie = false;
    }


    private void speedSlide(GameObject player)
    {
        Vector3 distance = this.transform.position - player.transform.position;
        _aniamtor.SetInteger("status", (int)Troopa.eStatus.SpeedShell);
        if (distance.x <= 0)
            this.SetSpeed(new Vector3(-0.3f, 0f, 0f));
        else
            this.SetSpeed(new Vector3(0.3f, 0f, 0f));
        _rigidBody2D.velocity = Vector2.zero;
    }
    public override void back()
    {
        base.back();
        if ((_imovement as LinearMovement).Xspeed > 0)
            _aniamtor.SetBool("left", false);
        else
            _aniamtor.SetBool("left", true);
    }

    // Nếu đụng vật khác thì đi ngược lại
    protected override void checkWithGround(Collision2D collision)
    {
        base.checkWithGround(collision);
        if (collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.gameObject.tag == "Brick")
        {
            float top = collision.collider.bounds.max.y;
            if (top - this.GetComponent<Collider2D>().bounds.min.y > 0.5)
                checkWithBrick(collision);
        }
    }

    private void checkWithBrick(Collision2D col)
    {
        if (_aniamtor.GetInteger("status") == (int)eStatus.SpeedShell)
            col.gameObject.GetComponent<Animator>().SetTrigger("smash");
    }

    protected override void checkWithEnemy(Collision2D collision)
    {
        eStatus status = (eStatus)_aniamtor.GetInteger("status");
        switch (status)
        {
            case eStatus.Normal:
                back();
                break;
            case eStatus.Shell:
                break;
            case eStatus.SpeedShell:
                // kill them all :v
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy._canHitByShell)
                    enemy.GetComponent<Animator>().SetInteger("status", (int)Enemy.eStatus.Hit);
                break;

        }
    }

    public override void hitByBullet(float dmg, Bullet.eType type)
    {
        if (_canHitByFire == false)
            return;
        // shell kháng các loại weapon, trừ boomerang
        if (this.GetComponent<Animator>().GetInteger("status") == (int)Troopa.eStatus.Shell &&
            type != Bullet.eType.boomerang)
            return;
        _hp -= dmg;
        if (_hp <= 0)
        {
            this.GetComponent<Animator>().SetInteger("status", (int)Enemy.eStatus.Hit);
        }
        else
        {
            this.GetComponent<Animator>().SetInteger("status", (int)Troopa.eStatus.Shell);
            _shell_stat_countdown = _SHELLSTATCOUNTDOWN;
            _tempSpeed = _speed;
            this.SetSpeed(Vector3.zero);

        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public Key left = "A";
    public Key right = "D";
    public Key up = "W";
    public Key down = "S";

    public Key interact = "E";
    public Key warp = KeyCode.Space;
    public Key upgrades = KeyCode.Tab;

    public Key fire = KeyCode.Mouse0;
    public Key special = KeyCode.Mouse1;

    public Key pause = KeyCode.Escape;

    private static Key[] keys;

    private void Start()
    {
        UpdateKeys();
    }

    public void UpdateKeys()
    {
        keys = new Key[]
        {
            left,
            right,
            up,
            down,
            interact,
            warp,
            upgrades,
            fire,
            special,
            pause
        };
    }

    public void Update()
    {
        foreach (Key k in keys)
        {
            k.UpdateKey();
        }

        if (upgrades.down)
            LevelUI.Instance.ToggleMenu(Menu.upgrades);

        if (pause.down && LevelUI.Instance.currMenu != Menu.upgrades)
            GameController.Instance.TogglePause();
    }
}

public class Key
{
    private KeyCode targetKey;
    private KeyCode alternateKey;
    public bool down;
    public bool held;
    public bool up;

    public Key(KeyCode targetKey)
    {
        this.targetKey = targetKey;
    }

    public Key(KeyCode targetKey, KeyCode alternateKey)
    {
        this.targetKey = targetKey;
        this.alternateKey = alternateKey;
    }

    public void UpdateKey()
    {
        down = Input.GetKeyDown(targetKey) || Input.GetKeyDown(alternateKey);
        held = Input.GetKey(targetKey) || Input.GetKey(alternateKey);
        up = Input.GetKeyUp(targetKey) || Input.GetKeyUp(alternateKey);
    }

    public string GetKey()
    {
        return targetKey.ToString();
    }

    public string GetAltKey()
    {
        return alternateKey.ToString();
    }

    public static implicit operator bool(Key obj)
    {
        return obj.down || obj.held || obj.up;
    }

    public static implicit operator Key(string key)
    {
        KeyCode newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);
        return new Key(newKey);
    }

    public static implicit operator Key(KeyCode key)
    {
        return new Key(key);
    }
}
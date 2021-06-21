using System;
using TouchScript.Behaviors;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class TouchComp : MonoBehaviour
{
    /// <summary>
    /// 设置能否点击
    /// </summary>
    public bool CanPress
    {
        get => _canPress;
        set
        {
            if (_curCollider)
            {
                _curCollider.enabled = value;
            }
            _canPress = value;
        }
    }
    private bool _canPress = false;

    /// <summary>
    /// 按下
    /// </summary>
    private event Action<object, System.EventArgs> _onPressedEvent;
    /// <summary>
    /// 释放
    /// </summary>
    private event Action<object, System.EventArgs> _onReleasedEvent;
    /// <summary>
    /// 长按
    /// </summary>
    private event Action<object, System.EventArgs> _onLongPressedEvent;
    /// <summary>
    /// 拖拽过程
    /// </summary>
    private event Action<object, System.EventArgs> _onTransformedEvent;
    /// <summary>
    /// 拖拽开始
    /// </summary>
    private event Action<object, System.EventArgs> _onTransformStartedEvent;
    /// <summary>
    /// 拖拽完毕
    /// </summary>
    private event Action<object, System.EventArgs> _onTransformCompletedEvent;
    private Collider _curCollider;

    private PressGesture _pressGesture;
    private ReleaseGesture _releaseGesture;
    private LongPressGesture _longPressGesture;
    private TransformGesture _transformGesture;
    private Transformer _transformer;


    private void Awake()
    {
        _curCollider = GetComponent<Collider>();
        if (_curCollider == null)
        {
            Debug.LogError("没找到碰撞体,请检查" + this.gameObject.name + "是否添加碰撞体");
        }
    }

    private void OnEnable()
    {
        if (_pressGesture != null)
        {
            _pressGesture.Pressed += _onPressed;
        }
        if (_releaseGesture != null)
        {
            _releaseGesture.Released += _onReleased;
        }
        if (_longPressGesture != null)
        {
            _longPressGesture.LongPressed += _onLongPressed;
        }
        if (_transformGesture != null)
        {
            _transformGesture.Transformed += _onTransformed;
            _transformGesture.TransformStarted += _onTransformStarted;
            _transformGesture.TransformCompleted += _onTransformCompleted;
        }
    }
    private void OnDisable()
    {
        if (_pressGesture != null)
        {
            _pressGesture.Pressed -= _onPressed;
        }
        if (_releaseGesture != null)
        {
            _releaseGesture.Released -= _onReleased;
        }
        if (_longPressGesture != null)
        {
            _longPressGesture.LongPressed -= _onLongPressed;
        }
        if (_transformGesture != null)
        {
            _transformGesture.Transformed -= _onTransformed;
            _transformGesture.TransformStarted -= _onTransformStarted;
            _transformGesture.TransformCompleted -= _onTransformCompleted;
        }
    }



    /// <summary>
    /// 按下
    /// </summary>
    public TouchComp AddPressed(Action<object, System.EventArgs> action)
    {
        _onPressedEvent += action;
        if (_pressGesture == null)
        {
            _pressGesture = gameObject.GetComponent<PressGesture>() ?? gameObject.AddComponent<PressGesture>();
            _pressGesture.Pressed += _onPressed;
        }


        return this;
    }
    /// <summary>
    /// 释放
    /// </summary>
    public TouchComp AddReleased(Action<object, System.EventArgs> action)
    {
        _onReleasedEvent += action;

        if (_releaseGesture == null)
        {
            _releaseGesture = gameObject.GetComponent<ReleaseGesture>() ?? gameObject.AddComponent<ReleaseGesture>();
            _releaseGesture.Released += _onReleased;
        }

        return this;
    }
    /// <summary>
    /// 长按
    /// </summary>
    public TouchComp AddLongPressed(Action<object, System.EventArgs> action)
    {
        _onLongPressedEvent += action;

        if (_longPressGesture == null)
        {
            _longPressGesture = gameObject.GetComponent<LongPressGesture>() ?? gameObject.AddComponent<LongPressGesture>();
            _longPressGesture.LongPressed += _onLongPressed;
        }

        return this;
    }
    /// <summary>
    /// 拖拽过程
    /// </summary>
    public TouchComp AddTransformed(Action<object, System.EventArgs> action)
    {
        _onTransformedEvent += action;

        if (_transformGesture == null)
        {
            _transformGesture = gameObject.GetComponent<TransformGesture>() ?? gameObject.AddComponent<TransformGesture>();
            _transformGesture.Transformed += _onTransformed;
            _transformGesture.TransformStarted += _onTransformStarted;
            _transformGesture.TransformCompleted += _onTransformCompleted;
        }
        if (_transformer == null)
        {
            _transformer = gameObject.GetComponent<Transformer>() ?? gameObject.AddComponent<Transformer>();
        }
        return this;
    }
    /// <summary>
    /// 拖拽开始
    /// </summary>
    public TouchComp AddTransformStarted(Action<object, System.EventArgs> action)
    {
        _onTransformStartedEvent += action;

        if (_transformGesture == null)
        {
            _transformGesture = gameObject.GetComponent<TransformGesture>() ?? gameObject.AddComponent<TransformGesture>();
            _transformGesture.Transformed += _onTransformed;
            _transformGesture.TransformStarted += _onTransformStarted;
            _transformGesture.TransformCompleted += _onTransformCompleted;
        }
        if (_transformer == null)
        {
            _transformer = gameObject.GetComponent<Transformer>() ?? gameObject.AddComponent<Transformer>();
        }
        return this;
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    public TouchComp AddTransformCompleted(Action<object, System.EventArgs> action)
    {
        _onTransformCompletedEvent += action;

        if (_transformGesture == null)
        {
            _transformGesture = gameObject.GetComponent<TransformGesture>() ?? gameObject.AddComponent<TransformGesture>();
            _transformGesture.Transformed += _onTransformed;
            _transformGesture.TransformStarted += _onTransformStarted;
            _transformGesture.TransformCompleted += _onTransformCompleted;
        }
        if (_transformer == null)
        {
            _transformer = gameObject.GetComponent<Transformer>() ?? gameObject.AddComponent<Transformer>();
        }
        return this;
    }

    /// <summary>
    /// 按下
    /// </summary>
    private void _onPressed(object sender, System.EventArgs e)
    {
        if (_onPressedEvent != null) _onPressedEvent(sender, e);
    }
    /// <summary>
    /// 释放
    /// </summary>
    private void _onReleased(object sender, System.EventArgs e)
    {
        if (_onReleasedEvent != null) _onReleasedEvent(sender, e);
    }
    /// <summary>
    /// 长按
    /// </summary>
    private void _onLongPressed(object sender, System.EventArgs e)
    {
        if (_onLongPressedEvent != null) _onLongPressedEvent(sender, e);
    }
    /// <summary>
    /// 拖拽过程
    /// </summary>
    private void _onTransformed(object sender, System.EventArgs e)
    {
        if (_onTransformedEvent != null) _onTransformedEvent(sender, e);
    }
    /// <summary>
    /// 拖拽开始
    /// </summary>
    private void _onTransformStarted(object sender, System.EventArgs e)
    {
        if (_onTransformStartedEvent != null) _onTransformStartedEvent(sender, e);
    }

    /// <summary>
    /// 拖拽完毕
    /// </summary>
    private void _onTransformCompleted(object sender, System.EventArgs e)
    {
        if (_onTransformCompletedEvent != null) _onTransformCompletedEvent(sender, e);
    }








}
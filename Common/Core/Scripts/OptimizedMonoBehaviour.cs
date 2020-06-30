using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 优化后的MonoBehaviour
/// </summary>
public class OptimizedMonoBehaviour : MonoBehaviour
{
    private Transform _transform;
    public Transform Transform
    {
        get
        {
            if (!_transform)
            {
                _transform = GetComponent<Transform>();
            }
            return _transform;
        }
    }

    private RectTransform _rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    private Animator _animator;
    public Animator Animator
    {
        get
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }

    private Image _image;
    public Image Image
    {
        get
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
            return _image;
        }
    }

    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (!_spriteRenderer)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _spriteRenderer;
        }
    }

    private Camera _camera;
    public Camera Camera
    {
        get
        {
            if (!_camera)
            {
                _camera = GetComponent<Camera>();
            }
            return _camera;
        }
    }

    private Camera _uiCamera;
    public Camera UICamera
    {
        get
        {
            if (!_uiCamera)
            {
                _uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            }
            return _uiCamera;
        }
    }


}

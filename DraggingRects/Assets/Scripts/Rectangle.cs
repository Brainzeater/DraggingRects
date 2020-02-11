using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle : MonoBehaviour
{
    // Компонент спрайта прямоугольника
    public SpriteRenderer sprite;

    void Awake()
    {
        // Генерация случайного цвета при инициализации
        sprite.color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));
    }
}
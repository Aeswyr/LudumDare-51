using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public void Init(float speed, Vector2 direction) {
        var rbody = gameObject.AddComponent<Rigidbody2D>();

        rbody.velocity = speed * direction;
        rbody.gravityScale = 0;

        if (TryGetComponent(out SpriteRenderer sprite)) {
            sprite.flipX = direction.x < 0;
        }
    }
}

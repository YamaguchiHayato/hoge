using UnityEngine;

namespace nsApp
{
    namespace nsActor
    {
        namespace nsPlayer
        {
            /// <summary>
            /// 弾の挙動を制御するクラス。
            /// </summary>
            [RequireComponent(typeof(Rigidbody))]
            [RequireComponent(typeof(SphereCollider))]
            public class Bullet : MonoBehaviour
            {
                [SerializeField] private float _speed = 20.0f;   //! 弾速。
                [SerializeField] private float _lifeTime = 3.0f; //! 消えるまでの秒数。
                private Rigidbody _rigidbody; //! 弾のRigidbody。


                /// <summary>
                /// Rigidbodyを初期化する。
                /// </summary>
                private void Awake()
                {
                    // Rigidbodyを取得し、重力を無効化してY軸方向の移動と回転を固定する。s
                    _rigidbody = GetComponent<Rigidbody>();
                    _rigidbody.useGravity = false;
                    _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |RigidbodyConstraints.FreezeRotation;
                }


                /// <summary>
                /// 弾の初期化を行う。弾の進行方向と無視するコライダーを設定する。
                /// </summary>
                /// <param name="direction"></param>
                /// <param name="ignoreColliders"></param>
                public void Init(Vector3 direction, Collider[] ignoreColliders)
                {
                    // 弾の進行方向を設定する。Y軸方向の移動は無効化する。
                    direction.y = 0.0f;
                    if (direction.sqrMagnitude < 0.001f)
                        direction = transform.forward;

                    // 弾の速度を設定し、無視するコライダーを登録する。
                    _rigidbody.linearVelocity = direction.normalized * _speed;
                    Collider bulletCollider = GetComponent<Collider>();

                    // 無視するコライダーを登録する。
                    foreach (Collider ignoreCollider in ignoreColliders)
                    {
                        // 無視するコライダーがnullでない場合のみ登録する。
                        if (ignoreCollider != null)
                            Physics.IgnoreCollision(bulletCollider, ignoreCollider);
                    }
                }


                /// <summary>
                /// 弾の寿命を減らし、寿命が尽きたら消す。
                /// </summary>
                private void Update()
                {
                    _lifeTime -= Time.deltaTime;
                    if (_lifeTime <= 0.0f)
                        Destroy(gameObject);
                }


                /// <summary>
                /// 弾が何かに衝突したら消す。
                /// </summary>
                /// <param name="collision"></param>
                private void OnCollisionEnter(Collision collision)
                {
                    // 衝突したオブジェクトが無視するコライダーでない場合のみ消す。
                    Destroy(gameObject);
                }
            }
        }
    }
}
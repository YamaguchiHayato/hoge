using UnityEngine;

namespace nsApp
{
    namespace nsActor
    {
        namespace nsPlayer
        {
            /// <summary>
            /// タンクの見た目を制御するインターフェース。
            /// </summary>
            public interface ITankView
            {
                Vector3 HullPosition { get; } //! 車体の位置。
                float HullYaw { get; }  //! 車体のヨー角。
                void ApplyHull(Vector3 position, float yaw);
                void ApplyTurret(float yaw);
                void SpawnBullet(Vector3 direction);
            }

            /// <summary>
            ///  タンクの見た目を制御するクラス。
            /// </summary>
            [RequireComponent(typeof(Rigidbody))]
            public class TankView : MonoBehaviour, ITankView
            {
                [Header("Hull")]
                [SerializeField] private Rigidbody _hull; //! 車体のRigidbody。
                [Header("Turret")]
                [SerializeField] private Transform _turret; //! 砲塔。
                [SerializeField] private Transform _muzzle; //! 弾の出現位置。
                [Header("Bullet")]
                [SerializeField] private Bullet _bulletPrefab; //! 発射する弾。
                public Vector3 HullPosition => _hull.position;       //! 車体の位置。
                public float HullYaw => _hull.rotation.eulerAngles.y; //! 車体のヨー角。


                /// <summary>
                /// 車体の位置とヨー角を反映する。
                /// </summary>
                /// <param name="position"></param>
                /// <param name="yaw"></param>
                public void ApplyHull(Vector3 position, float yaw)
                {
                    // 車体の位置とヨー角を設定する。
                    _hull.MovePosition(position);
                    _hull.MoveRotation(Quaternion.Euler(0.0f, yaw, 0.0f));
                }


                /// <summary>
                /// 砲塔のヨー角を反映する。    
                /// </summary>
                /// <param name="yaw"></param>
                public void ApplyTurret(float yaw)
                {
                    // 砲塔の回転を設定する。
                    _turret.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
                }


                /// <summary>
                /// 弾を発射する。
                /// </summary>
                /// <param name="direction"></param>
                public void SpawnBullet(Vector3 direction)
                {
                    // 弾を生成し、初期化する。
                    Bullet bullet = Instantiate(_bulletPrefab, _muzzle.position, _turret.rotation);
                    bullet.Init(direction, _hull.GetComponentsInChildren<Collider>());
                }
            }
        }
    }
}

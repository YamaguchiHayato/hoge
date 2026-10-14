using System;
using UnityEngine;


namespace nsApp
{
    namespace nsActor
    {
        namespace nsPlayer
        {
            /// <summary>
            /// このクラスだけで使う初期値。
            /// </summary>
            static class TankMoveDefault
            {
                public const float MoveSpeed = 5.0f;         //! 前進速度。
                public const float BackMoveSpeed = 3.0f;     //! 後退速度。
                public const float RotateSpeed = 100.0f;     //! 車体の回転速度。
                public const float AimSpeed = 720.0f;        //! 砲塔の回転速度。
                public const float ShotInterval = 0.4f;      //! 発射間隔。
                public const float MinDirectionSqr = 0.001f; //! 照準を無視する方向の長さ。
                public const float MinInput = 0.01f;         //! 入力を無視する大きさ。
                public const float Flat = 0.0f;              //! 水平にするための値。
            }


            /// <summary>
            /// タンク(戦車)の移動と砲塔の回転を制御するクラス。
            /// </summary>
            public class TankMove : MonoBehaviour
            {
                public event Action Fired; //! 発射したことを通知。

                public Vector3 Position => _position; //! 車体の座標。
                public float HullYaw => _hullYaw;     //! 車体の向き。
                public float TurretYaw => _turretYaw; //! 砲塔の向き。

                private readonly float _moveSpeed;     //! 前進速度。
                private readonly float _backMoveSpeed; //! 後退速度。
                private readonly float _rotateSpeed;   //! 回転速度。
                private readonly float _aimSpeed;      //! 砲塔の回転速度。
                private readonly float _shotInterval;  //! 発射間隔。

                private Vector3 _position;      //! 車体の座標。
                private float _hullYaw;         //! 車体の向き。
                private float _turretYaw;       //! 砲塔の向き。
                private float _targetTurretYaw; //! 砲塔の目標向き。
                private float _moveInput;       //! 前進・後退の入力値。
                private float _rotateInput;     //! 回転の入力値。
                private float _lastShotTime;    //! 最後に発射した時間。


                /// <summary>
                /// Unity がコンポーネントを作るときの初期化。
                /// </summary>
                public TankMove(): this(Vector3.zero, TankMoveDefault.Flat, TankMoveDefault.MoveSpeed, TankMoveDefault.BackMoveSpeed, TankMoveDefault.RotateSpeed, TankMoveDefault.AimSpeed, TankMoveDefault.ShotInterval){}


                /// <summary>
                /// タンク(戦車)モデルを初期化。
                /// </summary>
                public TankMove(Vector3 position, float hullYaw, float moveSpeed, float backMoveSpeed, float rotateSpeed, float aimSpeed, float shotInterval)
                {
                    _position = position;
                    _hullYaw = hullYaw;
                    _turretYaw = hullYaw;
                    _targetTurretYaw = hullYaw;
                    _moveSpeed = moveSpeed;
                    _backMoveSpeed = backMoveSpeed;
                    _rotateSpeed = rotateSpeed;
                    _aimSpeed = aimSpeed;
                    _shotInterval = shotInterval;
                }


                /// <summary>
                /// タンクの移動と砲塔の回転を設定する。
                /// </summary>
                public void SetDrive(float moveInput, float rotateInput)
                {
                    _moveInput = moveInput;
                    _rotateInput = rotateInput;
                }


                /// <summary>
                /// 砲塔の目標向きを設定する。
                /// </summary>
                public void SetAimDirection(Vector3 direction)
                {
                    // 砲塔の目標向きは水平面上での方向にする。
                    direction.y = TankMoveDefault.Flat;

                    // 方向の長さが小さい場合は無視する。
                    if (direction.sqrMagnitude < TankMoveDefault.MinDirectionSqr)
                        return;

                    // 砲塔の目標向きを設定する。
                    _targetTurretYaw = Quaternion.LookRotation(direction).eulerAngles.y;
                }


                /// <summary>
                /// 砲塔の回転をシミュレーションする。
                /// </summary>
                public void SimulateAim(float deltaTime)
                {
                    // 砲塔の回転を目標向きに近づける。
                    _turretYaw = Mathf.MoveTowardsAngle(_turretYaw, _targetTurretYaw, _aimSpeed * deltaTime);
                }


                /// <summary>
                /// タンクの移動をシミュレーションする。
                /// </summary>
                public void SimulateDrive(float deltaTime)
                {
                    // 車体の回転を入力に応じて行う。
                    if (Mathf.Abs(_rotateInput) >= TankMoveDefault.MinInput)
                        _hullYaw += _rotateInput * _rotateSpeed * deltaTime;

                    // 車体の前進・後退を入力に応じて行う。
                    if (Mathf.Abs(_moveInput) < TankMoveDefault.MinInput)
                        return;

                    // 前進・後退の速度を決定する。
                    float speed = _moveInput > TankMoveDefault.Flat ? _moveSpeed : _backMoveSpeed;
                    Vector3 forward = Quaternion.Euler(TankMoveDefault.Flat, _hullYaw, TankMoveDefault.Flat) * Vector3.forward;
                    _position += forward * (_moveInput * speed * deltaTime);
                }


                /// <summary>
                /// 発射可能かどうかをチェックし、可能であれば発射する。
                /// </summary>
                public bool TryFire(float time)
                {
                    // 発射間隔が経過していない場合は発射できない。
                    if (time < _lastShotTime + _shotInterval)
                        return false;

                    // 発射可能なので、発射したことを通知する。
                    _lastShotTime = time;
                    Fired?.Invoke();
                    return true;
                }


                /// <summary>
                /// 砲塔の前方向を取得する。
                /// </summary>
                public Vector3 GetTurretForward()
                {
                    // 砲塔の前方向を水平面上で取得する。
                    return Quaternion.Euler(TankMoveDefault.Flat, _turretYaw, TankMoveDefault.Flat) * Vector3.forward;
                }
            }
        }
    }
}
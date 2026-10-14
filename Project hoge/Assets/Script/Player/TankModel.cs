using System;
using UnityEngine;

namespace nsApp
{
    namespace nsActor
    {
        namespace nsPlayer
        {
            /// <summary>
            /// 戦車の移動、照準、発射判断を持つ。
            /// </summary>
            public class TankModel
            {
                /// <summary>
                /// 発射したときに通知する。
                /// </summary>
                public event Action Fired;

                /// <summary>
                /// 車体の位置。
                /// </summary>
                public Vector3 Position => _position;

                /// <summary>
                /// 車体のヨー角。
                /// </summary>
                public float HullYaw => _hullYaw;

                /// <summary>
                /// 砲塔のヨー角。
                /// </summary>
                public float TurretYaw => _turretYaw;

                private readonly float _moveSpeed;     //! 前進速度。
                private readonly float _backMoveSpeed; //! 後退速度。
                private readonly float _rotateSpeed;   //! 車体の回転速度。
                private readonly float _aimSpeed;      //! 砲塔の回転速度。
                private readonly float _shotInterval;  //! 次の弾までの待ち時間。

                private Vector3 _position;             //! 車体の位置。
                private float _hullYaw;                //! 車体のヨー角。
                private float _turretYaw;              //! 砲塔のヨー角。
                private float _targetTurretYaw;        //! 砲塔が向かうヨー角。
                private float _moveInput;              //! 前進・後退の入力値。
                private float _rotateInput;            //! 左右回転の入力値。
                private float _lastShotTime = -100.0f; //! 前回発射した時刻。

                /// <summary>
                /// 初期状態とパラメータを受け取る。
                /// </summary>
                public TankModel(Vector3 position, float hullYaw, float moveSpeed, float backMoveSpeed, float rotateSpeed, float aimSpeed,float shotInterval)
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
                /// 車体への入力を保存する。
                /// </summary>
                public void SetDrive(float moveInput, float rotateInput)
                {
                    // 入力値を保存する。前進・後退の入力値は-1.0～1.0の範囲で、回転の入力値も-1.0～1.0の範囲であることを想定している。
                    _moveInput = moveInput;
                    _rotateInput = rotateInput;
                }

                /// <summary>
                /// 砲塔が向かう水平方向を保存する。
                /// </summary>
                public void SetAimDirection(Vector3 direction)
                {
                    // 方向ベクトルのY成分を0にして水平方向のみに制限する。
                    direction.y = 0.0f;
                    if (direction.sqrMagnitude < 0.001f)
                        return;

                    // 方向ベクトルから砲塔の目標ヨー角を計算する。Quaternion.LookRotationを使って方向ベクトルから回転を求め、その回転のY軸の角度を取得する。
                    _targetTurretYaw = Quaternion.LookRotation(direction, Vector3.up).eulerAngles.y;
                }


                /// <summary>
                /// 砲塔を目標方向へ近づける。
                /// </summary>
                public void SimulateAim(float deltaTime)
                {
                    // Mathf.MoveTowardsAngleを使って、現在の砲塔のヨー角から目標のヨー角へ、一定の速度で近づける。
                    // deltaTimeを掛けることでフレームレートに依存しない動作にする。
                    _turretYaw = Mathf.MoveTowardsAngle(_turretYaw, _targetTurretYaw, _aimSpeed * deltaTime);
                }


                /// <summary>
                /// 車体を旋回させ、前後に進める。
                /// </summary>
                public void SimulateDrive(float deltaTime)
                {
                    // 回転入力がある場合は車体のヨー角を更新する。
                    // Mathf.Absで入力値の絶対値を取得し、0.01以上の場合のみ回転させる。
                    if (Mathf.Abs(_rotateInput) >= 0.01f)
                        _hullYaw += _rotateInput * _rotateSpeed * deltaTime;

                    // 前後の移動入力がある場合は車体の位置を更新する。
                    if (Mathf.Abs(_moveInput) < 0.01f)
                        return;

                    // 前進・後退の速度を決定する。
                    // 前進の場合は_moveSpeed、後退の場合は_backMoveSpeedを使用する。
                    float speed = _moveInput > 0.0f ? _moveSpeed : _backMoveSpeed;
                    Vector3 forward = Quaternion.Euler(0.0f, _hullYaw, 0.0f) * Vector3.forward;
                    _position += forward * (_moveInput * speed * deltaTime);
                }


                /// <summary>
                /// 待ち時間が過ぎていれば発射を通知する。
                /// </summary>
                public bool TryFire(float time)
                {
                    // 前回発射からの経過時間がショット間隔より短い場合は発射できない。
                    if (time < _lastShotTime + _shotInterval)
                        return false;

                    // 発射可能な場合は、前回発射時刻を更新し、Firedイベントを通知する。
                    _lastShotTime = time;
                    Fired?.Invoke();
                    return true;
                }


                /// <summary>
                /// 砲塔の水平な前方向を返す。
                /// </summary>
                public Vector3 GetTurretForward()
                {
                    // Quaternion.Eulerを使って、砲塔のヨー角から回転を作り、その回転をVector3.forwardに掛けることで、砲塔の前方向のベクトルを取得する。
                    return Quaternion.Euler(0.0f, _turretYaw, 0.0f) * Vector3.forward;
                }
            }
        }
    }
}
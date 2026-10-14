using UnityEngine;

namespace nsApp
{
    namespace nsActor
    {
        namespace nsPlayer
        {
            /// <summary>
            /// タンクの見た目と判断をつなぐクラス。
            /// </summary>
            public class TankPresenter : MonoBehaviour
            {
                [Header("Reference")]
                [SerializeField] private TankView _view;    //! 見た目。
                [SerializeField] private Camera _aimCamera; //! 画面を映しているカメラ。
                [Header("Move")]
                [SerializeField] private float _moveSpeed = 5.0f;     //! 前進速度。
                [SerializeField] private float _backMoveSpeed = 3.0f; //! 後退速度。
                [SerializeField] private float _rotateSpeed = 100.0f; //! 車体の回転速度。
                [Header("Aim")]
                [SerializeField] private float _aimSpeed = 720.0f; //! 砲塔の回転速度。
                [Header("Shot")]
                [SerializeField] private float _shotInterval = 0.4f; //! 次の弾までの待ち時間。
                private TankModel _model; //! 戦車の判断。


                /// <summary>
                /// 戦車の判断を初期化する。
                /// </summary>
                private void Awake()
                {
                    // 戦車の判断を初期化する。
                    _model = new TankModel(
                        _view.HullPosition,
                        _view.HullYaw,
                        _moveSpeed,
                        _backMoveSpeed,
                        _rotateSpeed,
                        _aimSpeed,
                        _shotInterval
                    );
                }


                /// <summary>
                /// 発射通知の購読を開始する。
                /// </summary>
                private void OnEnable()
                {
                    _model.Fired += OnFired;
                }


                /// <summary>
                /// 発射通知の購読を終了する。   
                /// </summary>
                private void OnDisable()
                {
                    _model.Fired -= OnFired;
                }



                /// <summary>
                /// 車体の移動と砲塔の回転を計算する。
                /// </summary>
                private void Update()
                {
                    // 入力を読み取って戦車の判断に反映する。
                    _model.SetDrive(ReadMoveInput(), ReadRotateInput());

                    // マウスが指している方向を読み取って戦車の判断に反映する。
                    _model.SetAimDirection(ReadAimDirection());

                    // 砲塔の回転を計算して View に反映する。
                    _model.SimulateAim(Time.deltaTime);

                    // 砲塔の回転結果を View に反映する。
                    _view.ApplyTurret(_model.TurretYaw);

                    // マウスの左クリックで弾を発射する。
                    if (Input.GetMouseButtonDown(0))
                        _model.TryFire(Time.time);
                }


                /// <summary>
                /// 車体の移動を計算する。
                /// </summary>
                private void FixedUpdate()
                {
                    // 車体の移動を計算して View に反映する。
                    _model.SimulateDrive(Time.fixedDeltaTime);
                    _view.ApplyHull(_model.Position, _model.HullYaw);
                }


                /// <summary>
                /// 弾を発射する。
                /// </summary>
                private void OnFired()
                {
                    // 弾を発射する。
                    _view.SpawnBullet(_model.GetTurretForward());
                }


                /// <summary>
                ///  前進・後退の入力を読む。
                /// </summary>
                /// <returns></returns>
                private float ReadMoveInput()
                {
                    // 前進・後退の入力を読み取る。
                    float moveInput = 0.0f;

                    // W キーで前進、S キーで後退。
                    if (Input.GetKey(KeyCode.W))
                        moveInput += 1.0f;
                    if (Input.GetKey(KeyCode.S))
                        moveInput -= 1.0f;

                    // 入力結果を返す。
                    return moveInput;
                }


                /// <summary>
                /// 車体の回転の入力を読む。
                /// </summary>
                /// <returns></returns>
                private float ReadRotateInput()
                {
                    float rotateInput = 0.0f;
                    if (Input.GetKey(KeyCode.A))
                        rotateInput -= 1.0f;
                    if (Input.GetKey(KeyCode.D))
                        rotateInput += 1.0f;
                    return rotateInput;
                }


                /// <summary>
                /// マウスが指している方向を読む。
                /// </summary>
                /// <returns></returns>
                private Vector3 ReadAimDirection()
                {
                    // マウスが指している方向を計算する。
                    if (_aimCamera == null)
                        return Vector3.zero;

                    // マウスの位置からレイを作成し、地面との交点を計算する。
                    Ray ray = _aimCamera.ScreenPointToRay(Input.mousePosition);
                    Plane ground = new Plane(Vector3.up, Vector3.zero);
                    if (!ground.Raycast(ray, out float distance))
                        return Vector3.zero;

                    // 交点から砲塔の前方向を計算する。
                    Vector3 hit = ray.GetPoint(distance);
                    Vector3 direction = hit - _view.HullPosition;
                    direction.y = 0.0f;
                    return direction;
                }
            }
        }
    }
}
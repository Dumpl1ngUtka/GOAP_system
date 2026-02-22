using UnityEngine;

public class ActiveRagdollBalance : MonoBehaviour
{
    [Header("Кости")]
    [Tooltip("Rigidbody физического таза (куда вешаем скрипт)")]
    public Rigidbody hipsRigidbody;
    [Tooltip("Transform таза из скрытого анимационного скелета")]
    public Transform targetHips; 

    [Header("Настройки баланса")]
    [Tooltip("Сила, с которой персонаж пытается выровняться вертикально")]
    public float uprightTorque = 5000f;
    
    [Tooltip("Поддерживающая сила (помогает ногам держать вес)")]
    public float liftForce = 300f;

    void Start()
    {
        if (hipsRigidbody == null)
            hipsRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (hipsRigidbody == null || targetHips == null) return;

        // 1. Удержание равновесия (Крутящий момент / Torque)
        // Вычисляем разницу между текущим вращением таза и тем, как он повернут в анимации
        Quaternion rotationDifference = targetHips.rotation * Quaternion.Inverse(hipsRigidbody.rotation);
        
        // Превращаем разницу (Quaternion) в угол и ось вращения
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
        
        // Нормализуем угол, чтобы вращение шло по кратчайшему пути
        if (angle > 180f) angle -= 360f;

        // Применяем крутящий момент, если есть отклонение
        if (angle != 0 && !float.IsNaN(axis.x))
        {
            // Умножаем на Mathf.Deg2Rad для корректного перевода в радианы
            Vector3 torque = axis * (angle * Mathf.Deg2Rad) * uprightTorque;
            hipsRigidbody.AddTorque(torque, ForceMode.Acceleration);
        }

        // 2. Искусственная поддержка веса (Lift)
        // Слегка тянем таз вверх, компенсируя часть гравитации. 
        // Без этого придется делать пружины ног (Spring) железобетонными, и пропадет эффект "желе".
        hipsRigidbody.AddForce(Vector3.up * liftForce, ForceMode.Force);
    }
}
using UnityEngine;
using UnityEngine.Serialization;

namespace ProceduralAnimations
{
    public class ActiveRagdollBuilder : MonoBehaviour
    {
        [Header("Скелеты")]
        [Tooltip("Корневая кость скрытого скелета (с Animator)")]
        public Transform _animatedSkeletonRoot;
    
        [Tooltip("Корневая кость физического скелета (видимого)")]
        public Transform _physicalSkeletonRoot;

        [Header("Настройки физики по умолчанию")]
        public float _springForce = 1000f;
        public float _damperForce = 50f;

        // Эта строчка добавляет кнопку в контекстное меню скрипта в Инспекторе
        [ContextMenu("Собрать Active Ragdoll")]
        public void BuildRagdoll()
        {
            if (_animatedSkeletonRoot == null || _physicalSkeletonRoot == null)
            {
                Debug.LogError("Ошибка: Назначьте оба корневых объекта скелетов в инспекторе!");
                return;
            }

            // Запускаем рекурсивный обход, начиная с корневых костей
            SetupBoneRecursive(_animatedSkeletonRoot, _physicalSkeletonRoot, true);
        
            Debug.Log("<color=green>Успех:</color> Active Ragdoll успешно собран и связан!");
        }

        private void SetupBoneRecursive(Transform animBone, Transform physBone, bool isRoot)
        {
            // 1. Проверка идентичности (по количеству детей)
            if (animBone.childCount != physBone.childCount)
            {
                Debug.LogError($"Ошибка структуры: У кости {animBone.name} ({animBone.childCount} детей), а у {physBone.name} ({physBone.childCount} детей). Скелеты не идентичны!");
                return;
            }

            // 2. Настраиваем Rigidbody
            Rigidbody rb = physBone.GetComponent<Rigidbody>();
            if (rb == null) 
            {
                rb = physBone.gameObject.AddComponent<Rigidbody>();
            }

            // Корневая кость (обычно Hips/Pelvis) не должна крепиться джоинтом к родителю, 
            // так как она является центром масс всего рэгдолла.
            if (!isRoot)
            {
                // 3. Удаляем стандартный CharacterJoint, если он был создан генератором Unity
                CharacterJoint oldJoint = physBone.GetComponent<CharacterJoint>();
                if (oldJoint != null)
                {
                    DestroyImmediate(oldJoint);
                }

                // 4. Добавляем и настраиваем ConfigurableJoint
                ConfigurableJoint joint = physBone.GetComponent<ConfigurableJoint>();
                if (joint == null)
                {
                    joint = physBone.gameObject.AddComponent<ConfigurableJoint>();
                }

                // Связываем с Rigidbody родительской кости
                Rigidbody parentRb = physBone.parent.GetComponent<Rigidbody>();
                if (parentRb != null)
                {
                    joint.connectedBody = parentRb;
                }

                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
            
                // Включаем проекцию, чтобы при сильных столкновениях кости не растягивались как резина
                joint.projectionMode = JointProjectionMode.PositionAndRotation;
                
                // Настраиваем моторы (Slerp Drive) для удержания позы
                joint.rotationDriveMode = RotationDriveMode.Slerp;
                JointDrive drive = new JointDrive
                {
                    positionSpring = _springForce,
                    positionDamper = _damperForce,
                    maximumForce = Mathf.Infinity
                };
                joint.slerpDrive = drive;

                // 5. Вешаем скрипт синхронизации (тот самый ActiveRagdollLimb из прошлого ответа)
                ActiveRagdollLimb limb = physBone.GetComponent<ActiveRagdollLimb>();
                if (limb == null)
                {
                    limb = physBone.gameObject.AddComponent<ActiveRagdollLimb>();
                }

                limb.Setup(joint, animBone);
            }

            // 6. Рекурсивно идем глубже по иерархии
            for (int i = 0; i < animBone.childCount; i++)
            {
                SetupBoneRecursive(animBone.GetChild(i), physBone.GetChild(i), false);
            }
        }
    }
}
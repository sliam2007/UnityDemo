using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityDemo.Presentation;

namespace UnityDemo.EditorTools
{
    [InitializeOnLoad]
    internal static class CreateCoolingFanOnce
    {
        private const string ScenePath = "Assets/Scenes/Simulator.unity";

        static CreateCoolingFanOnce()
        {
            EditorApplication.delayCall += CreateFan;
        }

        private static void CreateFan()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            Scene scene = SceneManager.GetActiveScene();

            if (scene.path != ScenePath)
            {
                scene = EditorSceneManager.OpenScene(
                    ScenePath,
                    OpenSceneMode.Single);
            }

            GameObject machine = GameObject.Find("Machine");

            if (machine == null ||
                machine.transform.Find("CoolingFan") != null)
            {
                return;
            }

            Material housingMaterial = LoadMaterial(
                "Assets/Materials/FrontPanelMetal.mat");
            Material bladeMaterial = LoadMaterial(
                "Assets/Materials/MachineBodyPaint.mat");
            Material metalMaterial = LoadMaterial(
                "Assets/Materials/RotorMetal.mat");

            GameObject fanObject = new GameObject("CoolingFan");
            fanObject.transform.SetParent(machine.transform, false);
            fanObject.transform.localPosition =
                new Vector3(0f, 1.54f, 0f);
            fanObject.AddComponent<CoolingFanView>();

            CreatePart(
                "FanHousing",
                PrimitiveType.Cylinder,
                fanObject.transform,
                new Vector3(0f, 0.04f, 0f),
                Quaternion.identity,
                new Vector3(0.8f, 0.04f, 0.8f),
                housingMaterial);

            Transform rotor = new GameObject("FanRotor").transform;
            rotor.SetParent(fanObject.transform, false);
            rotor.localPosition = new Vector3(0f, 0.105f, 0f);

            for (int index = 0; index < 5; index++)
            {
                float angle = index * 72f;
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);

                CreatePart(
                    $"FanBlade{index + 1}",
                    PrimitiveType.Sphere,
                    rotor,
                    rotation * new Vector3(0f, 0f, 0.18f),
                    rotation,
                    new Vector3(0.11f, 0.025f, 0.32f),
                    bladeMaterial);
            }

            CreatePart(
                "FanHub",
                PrimitiveType.Cylinder,
                rotor,
                new Vector3(0f, 0.015f, 0f),
                Quaternion.identity,
                new Vector3(0.18f, 0.035f, 0.18f),
                metalMaterial);

            Transform guard = new GameObject("FanGuard").transform;
            guard.SetParent(fanObject.transform, false);
            guard.localPosition = new Vector3(0f, 0.135f, 0f);

            const int segmentCount = 24;
            const float guardRadius = 0.42f;

            for (int index = 0; index < segmentCount; index++)
            {
                float angle = index * (360f / segmentCount);
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);

                CreatePart(
                    $"GuardSegment{index + 1:00}",
                    PrimitiveType.Cube,
                    guard,
                    rotation * new Vector3(0f, 0f, guardRadius),
                    rotation,
                    new Vector3(0.11f, 0.025f, 0.035f),
                    metalMaterial);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Created the roof cooling fan in the Simulator scene.");
        }

        private static Material LoadMaterial(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        }

        private static GameObject CreatePart(
            string partName,
            PrimitiveType primitiveType,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            Material material)
        {
            GameObject part = GameObject.CreatePrimitive(primitiveType);
            part.name = partName;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = localRotation;
            part.transform.localScale = localScale;

            Renderer partRenderer = part.GetComponent<Renderer>();
            partRenderer.sharedMaterial = material;

            Collider partCollider = part.GetComponent<Collider>();

            if (partCollider != null)
            {
                Object.DestroyImmediate(partCollider);
            }

            return part;
        }
    }
}

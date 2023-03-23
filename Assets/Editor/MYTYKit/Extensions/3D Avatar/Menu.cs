using System.Collections.Generic;
using MYTYKit.Components;
using MYTYKit.MotionAdapters;
using MYTYKit.MotionTemplates;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace MYTYKit
{
    public class Menu
    {
        const string AnimationControllerPath = "Assets/MYTYKit/Extensions/3D Avatar/Animation/DefaultAnimationController.controller";
        
          
        static AddAndRemoveRequest m_request;
        static readonly List<string> PackageList = new ()
        {
            "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRMShaders#v0.109.0",
            "https://github.com/vrm-c/UniVRM.git?path=/Assets/UniGLTF#v0.109.0",
            "https://github.com/vrm-c/UniVRM.git?path=/Assets/VRM10#v0.109.0",
        };

        
        [MenuItem("MYTY Kit/Extensions/Setup 3D Avatar Exporter to Scene", false, 100)]
        public static void CreateAvatarExporterPipeline()
        {
            var ikRoot = new GameObject("IKTargets");
            var lhTargetGo = new GameObject("LH");
            lhTargetGo.transform.parent = ikRoot.transform;
            var lhTarget = lhTargetGo.AddComponent<MYTYIKTarget>();
            var rhTargetGo = new GameObject("RH");
            rhTargetGo.transform.parent = ikRoot.transform;
            var rhTarget = rhTargetGo.AddComponent<MYTYIKTarget>();

            var mapper = Object.FindObjectOfType<MotionTemplateMapper>();
            if(mapper ==null) MotionTemplateEditor.CreateMotionTemplate();
            mapper = Object.FindObjectOfType<MotionTemplateMapper>();
           
            var avatarBuilderGo = new GameObject("AvatarBuilder");
            var avatarBuilder = avatarBuilderGo.AddComponent<HumanoidAvatarBuilder>();

            var avatarDescRootGo = new GameObject("Put all 3D models here");
            var avatarDesc = avatarDescRootGo.AddComponent<MYTYAvatarDesc>();
            var animator = avatarDescRootGo.AddComponent<Animator>();
            avatarDescRootGo.AddComponent<MYTYAvatarBinder>();
            var muscleSetting = avatarDescRootGo.AddComponent<MuscleSetting>();
            var driver = avatarDescRootGo.AddComponent<MYTY3DAvatarDriver>();

            avatarDesc.avatarBuilder = avatarBuilder;

            animator.runtimeAnimatorController =
                AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AnimationControllerPath);
            
            driver.muscleSetting = muscleSetting;
            driver.leftHandTarget = lhTarget;
            driver.rightHandTarget = rhTarget;
            
            driver.head = mapper.GetTemplate("Head") as AnchorTemplate;
            driver.blendShape = mapper.GetTemplate("BlendShape") as ParametricTemplate;
            driver.poseWorldPoints = mapper.GetTemplate("BodyPoints") as PointsTemplate;
            
        }

        [MenuItem("MYTY Kit/Extensions/Import UniVRM", false, 100)]
        static void ImportUniVRM()
        {
            m_request = Client.AddAndRemove(PackageList.ToArray(), null);
            EditorUtility.DisplayProgressBar("MYTY Kit","Installing packages",0.5f);
            EditorApplication.update += Progress;
        }
    
        static void Progress()
        {
            if (m_request.IsCompleted)
            {
                if (m_request.Status == StatusCode.Success)
                {
                    Debug.Log("Installation Done!");
                    EditorApplication.update -= Progress;
                    EditorUtility.ClearProgressBar();
                }
                else if (m_request.Status >= StatusCode.Failure)
                {
                    Debug.LogError(m_request.Error.message);
                    EditorApplication.update -= Progress;
                    EditorUtility.ClearProgressBar();
                }
            
            }
        }
    }

}
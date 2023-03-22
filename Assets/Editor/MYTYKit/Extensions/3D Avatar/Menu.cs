using MYTYKit.Components;
using MYTYKit.MotionAdapters;
using MYTYKit.MotionTemplates;
using UnityEditor;
using UnityEngine;

namespace MYTYKit
{
    public class Menu
    {
        const string AnimationControllerPath = "Assets/MYTYKit/Extensions/3D Avatar/Animation/DefaultAnimationController.controller"; 
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
    }
}
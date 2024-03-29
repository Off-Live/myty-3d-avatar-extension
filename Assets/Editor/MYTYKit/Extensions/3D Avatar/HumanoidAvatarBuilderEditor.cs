using MYTYKit.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MYTYKit
{
    [CustomEditor(typeof(HumanoidAvatarBuilder))]
    public class HumanoidAvatarBuilderEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var essentialBones = new VisualElement();
            
            container.Add(new PropertyField(serializedObject.FindProperty("avatarRoot")));
            essentialBones.Add(new Label("Essential bones"));
            essentialBones.Add(new PropertyField(serializedObject.FindProperty("hips")));
            essentialBones.Add(new PropertyField(serializedObject.FindProperty("spine")));
            essentialBones.Add(new PropertyField(serializedObject.FindProperty("head")));
            essentialBones.Add(new PropertyField(serializedObject.FindProperty("leftShoulder")));
            essentialBones.Add(new PropertyField(serializedObject.FindProperty("rightShoulder")));
            essentialBones.Add(new PropertyField(serializedObject.FindProperty("leftUpperLeg")));
            essentialBones.Add(new PropertyField(serializedObject.FindProperty("rightUpperLeg")));
            essentialBones.style.marginTop = 10;
            essentialBones.style.marginBottom = 10;

            var manualsetting = new VisualElement();
            manualsetting.Add(new Label("Manual setting"));
            var boneFolding = new Foldout();
            boneFolding.value = false;
            boneFolding.text = "Bones";
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("chest")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("upperChest")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("neck")));
            boneFolding.Add(new Label(" "));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("leftUpperArm")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("leftLowerArm")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("leftHand")));
            boneFolding.Add(new Label(" "));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("leftLowerLeg")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("leftFoot")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("leftToe")));
            boneFolding.Add(new Label(" "));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("rightUpperArm")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("rightLowerArm")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("rightHand")));
            boneFolding.Add(new Label(" "));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("rightLowerLeg")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("rightFoot")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("rightToe")));
            boneFolding.Add(new Label(" "));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("leftFingers")));
            boneFolding.Add(new PropertyField(serializedObject.FindProperty("rightFingers")));

            
            manualsetting.Add(boneFolding);
            
            container.Add(essentialBones);
            container.Add(manualsetting);

            var builder = (HumanoidAvatarBuilder)target;
            var btnAutoBody = new Button(()=>
            {
                builder.AutoBody();
                boneFolding.value = true;
            });
            
            
            var btnTpose = new Button(() =>
            {
                if (!builder.IsCachedPose())
                {
                    foreach (SceneView sceneView in SceneView.sceneViews)
                    {
                        sceneView.ShowNotification(new GUIContent("Please save pose first"));
                    }

                    return;
                }
                builder.TPose();
            });

            var btnSavePose = new Button(() =>
            {
                builder.SavePose();
                foreach (SceneView sceneView in SceneView.sceneViews)
                {
                    sceneView.ShowNotification(new GUIContent("Pose saved"));
                }

            });

            var btnRestorePose = new Button(() =>
            {
                if (!builder.IsCachedPose())
                {
                    foreach (SceneView sceneView in SceneView.sceneViews)
                    {
                        sceneView.ShowNotification(new GUIContent("No cached pose"));
                    }

                    return;
                }
                builder.RestorePose();
            });
            
            var btnAvatar = new Button(() =>
            {
                var avatar = builder.BuildAvatar();
                var path = string.Format(MYTYPath.AssetPath + "/{0}.ht", avatar.name.Replace(':', '_'));
                AssetDatabase.CreateAsset(avatar, path);
                builder.RestorePose();
                foreach (SceneView sceneView in SceneView.sceneViews)
                {
                    sceneView.ShowNotification(new GUIContent($"Avatar asset created at {path} "));
                }
            });
            
            btnAutoBody.text = "Auto Body";
            btnTpose.text = "T Pose";
            btnAvatar.text = "Build Avatar";
            btnSavePose.text = "Save Pose";
            btnRestorePose.text = "Restore Pose";
            
            container.Add(btnAutoBody);
            container.Add(btnSavePose);
            container.Add(btnTpose);
            container.Add(btnAvatar);
            container.Add(btnRestorePose);
            return container;
        }

    }
}
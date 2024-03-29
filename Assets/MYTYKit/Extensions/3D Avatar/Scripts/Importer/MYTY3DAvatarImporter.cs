using System;
using System.Collections.Generic;
using System.Linq;
using MYTYKit.MotionAdapters;
using MYTYKit.Components;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UniGLTF;
using VRMShaders;


namespace MYTYKit.AvatarImporter
{

    public class MYTY3DImporterException : Exception
    {
        public MYTY3DImporterException(string reason):base(reason)
        {
            
        }
    }
    [DisallowMultipleComponent]
    public class MYTY3DAvatarImporter : MonoBehaviour
    {
        public MYTY3DAvatarDriver driver;
        public Avatar avatar;
        
        List<RuntimeGltfInstance> m_instances = new();
        SkinnedMeshRenderer m_mainSmr;
        Transform m_rootBone;
        Transform m_avatarRoot;

        Dictionary<Transform, Transform> m_rootBoneMap = new();

        
        
        public void LoadMainbody(byte[] modelData, string jsonString, Action<GameObject> avatarLoaded)
        {
            if (driver == null)
            {
                throw new MYTY3DImporterException("The driver is not setup properly");
            }
       
            var jObj = JObject.Parse(jsonString);
            var mainBodyName = (string)jObj["mainBody"];
            var rootBoneName = (string)jObj["rootBone"];
            var avatarRootName = (string)jObj["avatarRoot"];
            transform.localScale = jObj["referenceScale"].ToObject<Vector3>();
            LoadGlb(modelData, avatarRootName, instance =>
            {
                m_mainSmr = instance.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(smr => smr.name == mainBodyName);
                if (m_mainSmr == null)
                {
                    instance.Dispose();
                    throw new MYTY3DImporterException($"The bodyname {mainBodyName} is not valid ");
                }

                m_rootBone = instance.GetComponentsInChildren<Transform>()
                    .FirstOrDefault(tf => tf.name == rootBoneName);
                if (m_rootBone == null)
                {
                    instance.Dispose();
                    throw new MYTY3DImporterException($"No root bone in mainbody");
                    
                }
                m_avatarRoot = instance.transform;
                m_avatarRoot.parent = driver.transform;
                m_instances.Add(instance);
            
                avatar = HumanoidAvatarBuilder.CreateAvatarFromJson(m_avatarRoot.gameObject, (JObject)jObj["avatar"]);
                driver.GetComponent<Animator>().avatar = avatar;
                driver.binder.SetupRootBody(m_rootBone);
                driver.DeserializeFromJObject((JObject)jObj["driver"]);
                driver.CheckAndSetupBlendShape(m_avatarRoot);
                driver.humanoidAvatarRoot = m_avatarRoot;
                driver.Initialize();
                avatarLoaded.Invoke(m_avatarRoot.gameObject);
            });
        }

        public void LoadTrait(byte[] bytes, string loadName, Action traitLoaded = null)
        {
            if (m_avatarRoot == null)
            {
                throw new MYTY3DImporterException("No mainbody is set");
                
            }
            LoadGlb(bytes, loadName, instance =>
            {
                instance.transform.parent = driver.transform;
                m_instances.Add(instance);
                var rootBone = FixAndGetRootBone(instance.transform);
                driver.binder.Bind(rootBone);
                driver.CheckAndSetupBlendShape(instance.transform);
                m_rootBoneMap[instance.transform] = rootBone;
                traitLoaded?.Invoke();
            });
           
           
        }

        public void UnloadTrait(string name)
        {
            var traitTf = m_instances.FirstOrDefault(instance=> instance.transform.name == name);
            if (traitTf == null) return;
            m_instances.Remove(traitTf);
            driver.binder.Unbind(m_rootBoneMap[traitTf.transform]);
            m_rootBoneMap.Remove(traitTf.transform);
            traitTf.Dispose();
        }

        async void LoadGlb(byte[] bytes, string loadName, Action<RuntimeGltfInstance> action)
        {
            
            using(var glbData = new GlbBinaryParser(bytes, loadName).Parse())
            using (var loader = new ImporterContext(glbData))
            {
#if UNITY_WEBGL
                var awaiter = new RuntimeOnlyNoThreadAwaitCaller();
#else
                var awaiter = new RuntimeOnlyAwaitCaller();
#endif
                var instance = await loader.LoadAsync(awaiter);
                instance.name = loadName;
                instance.EnableUpdateWhenOffscreen();
                instance.ShowMeshes();
                action(instance);
            }
        }
        
        Transform FixAndGetRootBone(Transform instance)
        {
            var rootBoneName = m_rootBone.name;
            var children = instance.GetComponentsInChildren<Transform>();
            var rootTf = children.First(tf => tf.name == rootBoneName);
            if (rootTf == null)
            {
                Debug.LogWarning("Cannot find root bone");
                return null;
            }
            instance.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach( smr => smr.rootBone = rootTf);
            return rootTf;
        }
    }
}

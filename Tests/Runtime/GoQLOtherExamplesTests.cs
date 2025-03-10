﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;


namespace Unity.SelectionGroups.Tests 
{
    internal class GoQLOtherExamplesTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            Assert.IsTrue(System.IO.File.Exists($"{TestScenePath}.unity"));
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode($"{TestScenePath}.unity", 
                new LoadSceneParameters(LoadSceneMode.Single));
        }
         
        [Test]
        public void RootGameObjects() {
            TestUtility.ExecuteGoQLAndVerify("/", 13,(Transform t) => null == t.parent);
        }
        
        [Test]
        public void FromQuadWildcardGetSecondChildWithAudioSource()
        {
            TestUtility.ExecuteGoQLAndVerify("Quad*/<t:AudioSource>[1]", 2,(Transform t) => {
                return null!=t.parent && t.parent.name.StartsWith("Quad") && null != t.GetComponent<AudioSource>();
            });
            
        }
        
        [Test]
        public void GameObjectsHavingTransformAndAudioSource()
        {
            TestUtility.ExecuteGoQLAndVerify("<t:Transform, t:AudioSource>", 7, 
                (Transform t) => null != t.GetComponent<AudioSource>()
            );            
        }
        
        [Test]
        public void FromRendererGetAudioWildcardThenGetRangedChildren()
        {            
            int startIndex = 0;
            int endIndex   = 3;
            
            List<Transform> results = TestUtility.ExecuteGoQLAndVerify($"<t:Renderer>/*Audio*/[{startIndex}:{endIndex}]", 
                3, (Transform t) => null!=t.parent && t.parent.name.Contains("Audio")
            );
            foreach (Transform t in results) {
                bool found = false;

                for (int i = startIndex; !found && i < endIndex; ++i) {
                    if (t.name.EndsWith($"Child ({i})"))
                        found = true;
                }
                Assert.IsTrue(found);
            }                     
            
        }
        
        [Test]
        public void FromCubeGetQuadThenGetLastAudioSource()
        {
            TestUtility.ExecuteGoQLAndVerify("Cube/Quad/<t:AudioSource>[-1]", 1,(Transform t) => {
                return null!=t.parent && t.parent.name == "Quad" && null != t.GetComponent<AudioSource>();
            });
        }
        
        [Test]
        public void SkinMaterial()
        {            
            TestUtility.ExecuteGoQLAndVerify("<m:Skin>", 3, (Transform t) => {
                MeshRenderer mr = t.GetComponent<MeshRenderer>();
                return null!=mr && mr.sharedMaterial.name == "Skin";
            });            
            
        }
        
        [Test]
        public void FromEnvGetMeshRenderer()
        {
            TestUtility.ExecuteGoQLAndVerify("/Env/**<t:MeshRenderer>", 7,
                (Transform t) => t.GetComponent<MeshRenderer>()!=null
            );
        }
        
        [Test]
        public void InnerWildcard()
        {
            TestUtility.ExecuteGoQLAndVerify("Env*ent", 5, (Transform t) => t.name.StartsWith("Env") && t.name.EndsWith("ent"));
        }
        

        [Test]
        public void ExclusionBeginningAndEndingWildcard()
        {
            TestUtility.ExecuteGoQLAndVerify("/!*Head*", 7, (Transform t) => !t.name.Contains("Head"));
        }

        [Test]
        public void ExclusionBeginningWildCardThenExclusionEndingWildcard()
        {
            TestUtility.ExecuteGoQLAndVerify("/!*ead/!C*", 14, (Transform t) => {
                Transform p = t.parent; 
                return null!=p && !p.name.EndsWith("ead") && !t.name.StartsWith("C");
            });
        }


        [Test]
        public void ExclusionThenWildcard()
        {
            TestUtility.ExecuteGoQLAndVerify("/Head/!Cube/*", 3, (Transform t) => {
                Transform p = t.parent; 
                return null!=p && p.name != "Cube";
            });
        }
        
        [Test]
        public void WildcardsWithExclusion()
        {
            TestUtility.ExecuteGoQLAndVerify("/Head*!*Unit", 4, (Transform t) => {
                return null == t.parent && t.name.StartsWith("Head") && !t.name.EndsWith("Unit");
            });
        }
        
        
        const string TestScenePath = "Packages/com.unity.selection-groups/Tests/Scenes/GoQLOtherExamplesTestScene";

    }

} //end namespace
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DOTS_Test
{
   public class TestUI : MonoBehaviour
   {
      [SerializeField] private TMP_InputField _objectsCount;
      [SerializeField] private Button _spawnObjects;
      [SerializeField] private Button _nextSceneButton;
      [SerializeField] private TestObjectsECS _spawner;

      private int count;

      void Start()
      {
         count = PlayerPrefs.HasKey(SceneManager.GetActiveScene().buildIndex.ToString() + "objectsCount")
            ? PlayerPrefs.GetInt(SceneManager.GetActiveScene().buildIndex.ToString() + "objectsCount")
            : 1;

         _objectsCount.text = count.ToString();
         _objectsCount.onEndEdit.AddListener(OnCountEdited);

         _spawnObjects.onClick.AddListener(OnSpawn);
         _nextSceneButton.onClick.AddListener(NextScene);

         _spawner.SpawnObjects(count);
         Application.targetFrameRate = 60;
      }

      private void NextScene()
      {
         var nextScene = SceneManager.GetActiveScene().buildIndex + 1;

         if (nextScene >= 2)
            nextScene = 0;

         DestroyAllEntities();
         SceneManager.LoadScene(nextScene);
      }

      void DestroyAllEntities()
      {
         // EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
         // entityManager.
         // entityManager.DestroyEntity(entityManager.UniversalQuery);

         var entitymanager = World.DefaultGameObjectInjectionWorld.EntityManager;
         entitymanager.DestroyEntity(entitymanager.UniversalQuery);
         ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);
         DefaultWorldInitialization.Initialize("Default World", false);
      }

      private void OnSpawn()
      {
         DestroyAllEntities();
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }

      private void OnCountEdited(string num)
      {
         count = int.Parse(num);
         PlayerPrefs.SetInt(SceneManager.GetActiveScene().buildIndex.ToString() + "objectsCount", count);
      }
   }
}

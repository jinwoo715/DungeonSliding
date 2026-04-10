using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JW.DungeonSliding
{
    public enum SceneType
    {
        LobbyScene,
        GameScene
    }

    public class SceneManagerEx
    {
        public void LoadScene(SceneType sceneType)
        {
            SceneManager.LoadScene(sceneType.ToString());

            //var op = SceneManager.LoadSceneAsync(sceneType.ToString(), LoadSceneMode.Additive);
            //op.

        }
    }
}

using System.Security.AccessControl;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using kengine.Game.Exceptions;
using SFML.Graphics;
using SFML.Window;
using SceneInfo = System.Tuple<int,string>;

namespace kengine.Game
{
    public class SceneManager
    {

        private static SceneManager _current;
        
        
        //Private fields
        private RenderTarget _renderTarget;
        private Window _mainWindow;
        private Dictionary<SceneInfo,GameScene> _sceneIndex;
        private GameScene _currentGameScene;
        private Stopwatch _deltaTimeCompute;
        private float _deltaTime = 0.0f;

        //Getter et setters
        public RenderTarget RenderTarget
        {
            get => _renderTarget;
            set => _renderTarget = value;
        }

        public Window EventEmitter
        {
            get => _mainWindow;
            set => _mainWindow = value;
        }

        public float DeltaTime => _deltaTime;


        public static SceneManager GetCurrent()
        {
            return _current ??= new SceneManager();
        }


        public SceneManager()
        {
            _sceneIndex = new Dictionary<SceneInfo,GameScene>();
            _deltaTimeCompute = new Stopwatch();
            _deltaTime = 0.0f;
        }

        //Scene indexing function

        public int IndexScene(GameScene scene, int sceneId)
        {
            return IndexScene(scene, "", sceneId);
        }
        
        public int IndexScene(GameScene scene, string sceneName = "", int sceneId = -1)
        {
            int id = sceneId < 0 ? _sceneIndex.Count : sceneId;
            string name = sceneName == "" ? "Scene " + _sceneIndex.Count : sceneName;
            
            _sceneIndex.Add(new Tuple<int,string>(id, name),scene);

            return id;
        }

        public int CreateAndIndexScene<T>(string sceneName, int sceneId = -1) where T : GameScene, new()
        {
            return IndexScene(new T(), sceneName, sceneId);
        }


        // Scene loading functions

        public bool LoadScene(int id, bool closePreviousScene = true)
        {
            GameScene sceneToLoad =  _sceneIndex.First(pair => pair.Key.Item1 == id ).Value;

            if (sceneToLoad == null) return false;
            
            if(closePreviousScene) _currentGameScene?.exit(_mainWindow);
            _currentGameScene = sceneToLoad;
            _currentGameScene.start(_mainWindow);
            
            return true;
        }

        public bool LoadScene(string sceneName, bool closePreviousScene = true)
        {
            GameScene sceneToLoad =  _sceneIndex.First(pair => pair.Key.Item2 == sceneName ).Value;

            if (sceneToLoad == null) return false;
            
            if(closePreviousScene) _currentGameScene?.exit(_mainWindow);
            
            _currentGameScene = sceneToLoad;
            _currentGameScene.start(_mainWindow);
            
            return true;
        }

        public void StartGameLoop()
        {
            if(RenderTarget == null) throw new NoRenderTargetException();

            while (_mainWindow.IsOpen)
            {
                _deltaTime = _deltaTimeCompute.ElapsedMilliseconds/1000.0f;

                _deltaTimeCompute.Restart();
                
                
                _renderTarget.Clear(Color.Black);
                
                _mainWindow.DispatchEvents();
                
                _currentGameScene.physicsUpdate();
                _currentGameScene.update();
                _currentGameScene.draw(RenderTarget);
                
                _mainWindow.Display();

            }
        }

    } 
}






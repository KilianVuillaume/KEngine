using SFML.Graphics;
using SFML.Window;

namespace kengine.Game
{
    abstract public class GameScene
    {


        public abstract void start(Window gameWindow = null);
    
        public abstract void draw(RenderTarget target);
        public abstract void update();
        public abstract void physicsUpdate();

        public abstract void exit(Window gameWindow = null);
    } 
}


using Entitas;

public class Controller {
    ISystems _systems;

    public Controller(Contexts c) {
        _systems = new GameSystems(c);
    }

    public void Initialize() {
       _systems.Initialize(); 
    }

    public void Execute() {
        _systems.Execute();
        _systems.Cleanup();
    }
}

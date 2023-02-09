using Entitas;
using Entitas.VisualDebugging.Unity;

public class Contexts {
    static Contexts _sharedInstance;
    public static Contexts sharedInstance {
        get { return _sharedInstance; }
        set { _sharedInstance = value; }
    }
    
#if UNITY_EDITOR
    public ContextObserver contextObserver;
#endif
    
    public Context state;
    public Contexts() {
        state = new Context("Context");
        sharedInstance = this;
    }
}

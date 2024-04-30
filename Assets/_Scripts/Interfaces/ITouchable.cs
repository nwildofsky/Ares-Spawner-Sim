// Interface for objects that want an interaction to happen when their collider is touched on
// Intended for use in touchscreen applications, but can be simulated with the mouse
public interface ITouchable
{
    public void HandleTouch();
}

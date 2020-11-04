namespace Core.Input.Controller {
    public interface IInputReader<TAction, TInput> where TAction : struct {
        void SetInputMap(KeyMapping<TAction, TInput> mapping);
        bool ReadInput(TAction input);
        bool ReadInputUp(TAction input);
        bool ReadInputDown(TAction input);
        float ReadAxis(TAction axialInput);
    }
}

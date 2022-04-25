namespace HyperMsg
{
    public interface IModule
    {
        void Install(IRuntime runtime);

        void Uninstall(IRuntime runtime);
    }
}

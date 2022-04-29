namespace HyperMsg
{
    public interface IModule
    {
        void Install(IContext runtime);

        void Uninstall(IContext runtime);
    }
}

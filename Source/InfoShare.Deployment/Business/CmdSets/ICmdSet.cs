namespace InfoShare.Deployment.Business.CmdSets
{
    public interface ICmdSet
    {
        void Run();
    }

    public interface ICmdSet<TResult>
    {
        TResult Run();
    }
}

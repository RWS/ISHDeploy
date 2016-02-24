namespace InfoShare.Deployment.Business.Operations
{
    public interface IOperation
    {
        void Run();
    }

    public interface IOperation<out TResult>
    {
        TResult Run();
    }
}

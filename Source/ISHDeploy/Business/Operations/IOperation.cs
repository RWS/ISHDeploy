
namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides the ability to run operation
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Runs current operation.
        /// </summary>
        void Run();
    }

    /// <summary>
    /// Provides the ability to run operation that returns result
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IOperation<out TResult>
    {
        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns></returns>
        TResult Run();
    }
}

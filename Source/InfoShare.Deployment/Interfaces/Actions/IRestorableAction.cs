namespace InfoShare.Deployment.Interfaces.Actions
{
    public interface IRestorableAction
    {
        void Backup();
        void Rollback();
    }
}

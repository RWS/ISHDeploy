namespace InfoShare.Deployment.Core
{
    public interface IRestorable
    {
        void Backup();
        void Rollback();
    }
}

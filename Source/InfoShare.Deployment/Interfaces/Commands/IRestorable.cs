namespace InfoShare.Deployment.Interfaces.Commands
{
    public interface IRestorable
    {
        void Backup();
        void Rollback();
    }
}

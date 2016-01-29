namespace Trisoft.Configuration.Automation.Core
{
    public interface IRestorable
    {
        void Backup();
        void Rollback();
    }
}

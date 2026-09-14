namespace Gestion_Financiera.Data
{
    using System.Data;

    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

using Microsoft.EntityFrameworkCore.Design;

namespace Redmanmale.TelegramToRss.DAL
{
    /// <summary>
    /// For design-time Update-Database method
    /// </summary>
    public class GeneralDbContextFactory : IDesignTimeDbContextFactory<GeneralDbContext>
    {
        public GeneralDbContext CreateDbContext(string[] args) => ConfigurationManager.CreatePgSqlDbContext();
    }
}

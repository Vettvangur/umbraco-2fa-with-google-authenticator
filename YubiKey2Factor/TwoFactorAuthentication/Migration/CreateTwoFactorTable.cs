using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace TwoFactorAuthentication.Migrations
{ 
    public class CreateTwoFactorTable : MigrationBase
    {
        public CreateTwoFactorTable(IMigrationContext context)
            : base(context)
        {
        }

        public override void Migrate()
        {
            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToArray();
            if (tables.InvariantContains(Constants.ProductName)) return;

            Create.Table(Constants.ProductName)
                .WithColumn("userId").AsInt32().NotNullable()
                .WithColumn("key").AsString()
                .WithColumn("value").AsString()
                .WithColumn("confirmed").AsBoolean().Do();
        }
    }
}
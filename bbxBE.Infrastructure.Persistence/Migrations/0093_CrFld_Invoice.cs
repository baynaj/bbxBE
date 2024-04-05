using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00093, "v01.00.03 -Invoice CashOnDelivery")]
    public class InitialTables_00093 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {
            Alter.Table("Invoice")
                .AddColumn("CashOnDelivery").AsBoolean().WithDefaultValue(false);

            Execute.Sql(string.Format(@"update Invoice set CashOnDelivery = (case when Imported = 1 and left(InvoiceNumber,1) = 'U' then 1 else 0 end)")); //ezen a ponton csak az ősfeltöltésben lehet utánvétes
        }
    }
}

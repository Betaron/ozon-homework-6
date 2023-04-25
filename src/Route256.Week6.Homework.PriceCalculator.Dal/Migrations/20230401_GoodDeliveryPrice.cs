using FluentMigrator;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Migrations;

[Migration(20230401, TransactionBehavior.None)]
public class GoodDeliveryPrice : Migration
{
    public override void Up()
    {
        Create.Table("goods_delivery_anomalies")
            .WithColumn("id").AsInt64().PrimaryKey("delivery_anomaly_pk").Identity()
            .WithColumn("good_id").AsInt64().NotNullable()
            .WithColumn("price").AsDecimal().NotNullable();

        Create.Index("goods_delivery_anomalies_good_id_index")
            .OnTable("goods_delivery_anomalies")
            .OnColumn("good_id");
    }

    public override void Down()
    {
        Delete.Table("goods_delivery_anomalies");
    }
}

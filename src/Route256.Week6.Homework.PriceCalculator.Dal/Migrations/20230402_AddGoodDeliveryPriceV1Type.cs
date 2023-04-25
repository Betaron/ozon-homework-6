using FluentMigrator;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Migrations;

[Migration(20230402, TransactionBehavior.None)]
public class AddGoodDeliveryPriceV1Type : Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'goods_delivery_prices_v1') THEN
            CREATE TYPE goods_delivery_prices_v1 as
            (
                  id      bigint
                , good_id bigint
                , price   numeric(19, 5)
            );
        END IF;
    END
$$;";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
DO $$
    BEGIN
        DROP TYPE IF EXISTS goods_delivery_prices_v1;
    END
$$;";

        Execute.Sql(sql);
    }
}
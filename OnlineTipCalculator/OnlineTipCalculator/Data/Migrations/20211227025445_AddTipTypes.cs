using Microsoft.EntityFrameworkCore.Migrations;
using OnlineTipCalculator.Models;

namespace OnlineTipCalculator.Data.Migrations
{
    public partial class AddTipTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = $@"CREATE TABLE {nameof(TipType)}
                            (
	                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
	                            TypeName TEXT NOT NULL,
	                            TypeValue TEXT NOT NULL
                            );
                    INSERT INTO TipType(TypeName,TypeValue)VALUES('{nameof(TipType.FastFood)}',{(int)TipType.FastFood});
                    INSERT INTO TipType(TypeName,TypeValue)VALUES('{nameof(TipType.SitDownRestaurant)}',{(int)TipType.SitDownRestaurant});
                    INSERT INTO TipType(TypeName,TypeValue)VALUES('{nameof(TipType.Bar)}',{(int)TipType.Bar});";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

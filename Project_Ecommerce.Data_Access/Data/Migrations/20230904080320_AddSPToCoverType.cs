using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Ecommerce.Data_Access.Migrations
{
    public partial class AddSPToCoverType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                                               //Save
            migrationBuilder.Sql(@" CREATE PROCEDURE CreateCoverType
                                    @name  varchar(50)
                                 As
                                 insert CoverTypes values(@name)");
                                                //Update
            migrationBuilder.Sql(@" CREATE PROCEDURE UpdateCoverType
                                    @id int,
                                    @name  varchar(50)
                                 As
                                 update CoverTypes set name=@name where id=@id");
                                                //Delete
            migrationBuilder.Sql(@" CREATE PROCEDURE DeleteCoverType
                                    @id int
                                 As
                                 delete CoverTypes where id=@id");
                                                //Display
            migrationBuilder.Sql(@" CREATE PROCEDURE GetCoverTypes
                                 As
                                 select * from CoverTypes");
                                                //Find Single 
            migrationBuilder.Sql(@" CREATE PROCEDURE GetCoverType
                                 @id int
                                 As
                                 select * from CoverTypes where id=@id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

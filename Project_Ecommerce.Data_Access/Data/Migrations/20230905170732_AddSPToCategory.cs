using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Ecommerce.Data_Access.Migrations
{
    public partial class AddSPToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@" CREATE PROCEDURE CreateCategory
                                    @name  varchar(50)
                                 As
                                 insert Categories values(@name)");
            //Update
            migrationBuilder.Sql(@" CREATE PROCEDURE UpdateCategory
                                    @id int,
                                    @name  varchar(50)
                                 As
                                 update Categories set name=@name where id=@id");
            //Delete
            migrationBuilder.Sql(@" CREATE PROCEDURE DeleteCategory
                                    @id int
                                 As
                                 delete Categories where id=@id");
            //Display
            migrationBuilder.Sql(@" CREATE PROCEDURE GetCategories
                                 As
                                 select * from Categories");
            //Find Single 
            migrationBuilder.Sql(@" CREATE PROCEDURE GetCategory
                                 @id int
                                 As
                                 select * from Categories where id=@id");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Borrowing", "BorrowingDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Borrowing", "DueDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Borrowing", "ReturnDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Borrowing", "LastExtensionDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Borrowing", "LastExtensionDate", c => c.DateTime());
            AlterColumn("dbo.Borrowing", "ReturnDate", c => c.DateTime());
            AlterColumn("dbo.Borrowing", "DueDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Borrowing", "BorrowingDate", c => c.DateTime(nullable: false));
        }
    }
}

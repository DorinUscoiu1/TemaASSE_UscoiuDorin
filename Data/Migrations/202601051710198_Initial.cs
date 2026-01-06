namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Author",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Book",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 1000),
                        ISBN = c.String(maxLength: 20),
                        TotalCopies = c.Int(nullable: false),
                        ReadingRoomOnlyCopies = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Borrowing",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReaderId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        BorrowingDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        TotalExtensionDays = c.Int(nullable: false),
                        LastExtensionDate = c.DateTime(),
                        InitialBorrowingDays = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Reader", t => t.ReaderId)
                .ForeignKey("dbo.Book", t => t.BookId)
                .Index(t => t.ReaderId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.LoanExtension",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BorrowingId = c.Int(nullable: false),
                        ExtensionDate = c.DateTime(nullable: false),
                        ExtensionDays = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Borrowing", t => t.BorrowingId, cascadeDelete: true)
                .Index(t => t.BorrowingId);
            
            CreateTable(
                "dbo.Reader",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        Address = c.String(nullable: false, maxLength: 255),
                        PhoneNumber = c.String(maxLength: 20),
                        Email = c.String(maxLength: 150),
                        RegistrationDate = c.DateTime(nullable: false),
                        IsStaff = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BookDomain",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        ParentDomainId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookDomain", t => t.ParentDomainId)
                .Index(t => t.ParentDomainId);
            
            CreateTable(
                "dbo.Edition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        Publisher = c.String(nullable: false, maxLength: 150),
                        Year = c.Int(nullable: false),
                        EditionNumber = c.Int(nullable: false),
                        PageCount = c.Int(nullable: false),
                        BookType = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.BookAuthor",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BookId, t.AuthorId })
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Author", t => t.AuthorId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.BookBookDomain",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        DomainId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BookId, t.DomainId })
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.BookDomain", t => t.DomainId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.DomainId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Edition", "BookId", "dbo.Book");
            DropForeignKey("dbo.BookBookDomain", "DomainId", "dbo.BookDomain");
            DropForeignKey("dbo.BookBookDomain", "BookId", "dbo.Book");
            DropForeignKey("dbo.BookDomain", "ParentDomainId", "dbo.BookDomain");
            DropForeignKey("dbo.Borrowing", "BookId", "dbo.Book");
            DropForeignKey("dbo.Borrowing", "ReaderId", "dbo.Reader");
            DropForeignKey("dbo.LoanExtension", "BorrowingId", "dbo.Borrowing");
            DropForeignKey("dbo.BookAuthor", "AuthorId", "dbo.Author");
            DropForeignKey("dbo.BookAuthor", "BookId", "dbo.Book");
            DropIndex("dbo.BookBookDomain", new[] { "DomainId" });
            DropIndex("dbo.BookBookDomain", new[] { "BookId" });
            DropIndex("dbo.BookAuthor", new[] { "AuthorId" });
            DropIndex("dbo.BookAuthor", new[] { "BookId" });
            DropIndex("dbo.Edition", new[] { "BookId" });
            DropIndex("dbo.BookDomain", new[] { "ParentDomainId" });
            DropIndex("dbo.LoanExtension", new[] { "BorrowingId" });
            DropIndex("dbo.Borrowing", new[] { "BookId" });
            DropIndex("dbo.Borrowing", new[] { "ReaderId" });
            DropTable("dbo.BookBookDomain");
            DropTable("dbo.BookAuthor");
            DropTable("dbo.Edition");
            DropTable("dbo.BookDomain");
            DropTable("dbo.Reader");
            DropTable("dbo.LoanExtension");
            DropTable("dbo.Borrowing");
            DropTable("dbo.Book");
            DropTable("dbo.Author");
        }
    }
}

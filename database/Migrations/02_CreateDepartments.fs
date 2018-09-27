namespace Migrations
open SimpleMigrations

[<Migration(2L, "Create Departments Table")>]
type CreateDepartments() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Departments(
      Id INT NOT NULL IDENTITY PRIMARY KEY,
      Name VARCHAR(16) NOT NULL UNIQUE,
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Departments")

namespace Migrations
open SimpleMigrations

[<Migration(3L, "Create User-Department Junction Table")>]
type CreateUserDepartments() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE UserDepartments (
      UserId INT,
      DepartmentId INT,
      Role INT,
      CONSTRAINT PK_UserDepartment PRIMARY KEY (UserId, DepartmentId),
      CONSTRAINT FK_User 
        FOREIGN KEY (UserId) REFERENCES Users (Id),
      CONSTRAINT FK_Department 
        FOREIGN KEY (DepartmentId) REFERENCES Departments (Id) 
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE UserDepartments")

namespace Migrations
open SimpleMigrations

[<Migration(1L, "Create Users Table")>]
type CreateUsers() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Users(
      Id INT NOT NULL IDENTITY PRIMARY KEY,
      Username VARCHAR(8) NOT NULL,
      PreferredName VARCHAR(128) NOT NULL,
      Department VARCHAR(128) NOT NULL,
      Expertise VARCHAR(256) NOT NULL,
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Users")

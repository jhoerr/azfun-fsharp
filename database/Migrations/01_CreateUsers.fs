namespace Migrations
open SimpleMigrations

[<Migration(1L, "Create Users Table")>]
type CreateUsers() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Users(
      Id INT NOT NULL IDENTITY PRIMARY KEY,
      Hash VARCHAR(128) NOT NULL,
      NetId VARCHAR(16) NOT NULL UNIQUE,
      Name VARCHAR(128) NOT NULL,
      LocationCode VARCHAR(4) NOT NULL,
      Location VARCHAR(256) NULL,
      CampusPhone VARCHAR(16) NULL,
      CampusEmail VARCHAR(32) NOT NULL,
      Position VARCHAR(128) NOT NULL,
      Expertise VARCHAR(256) NOT NULL,
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Users")

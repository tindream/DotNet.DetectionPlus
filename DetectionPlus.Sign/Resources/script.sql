

CREATE TABLE [Histroys](
"Id"  integer Primary Key AutoIncrement not null,
"Result"  bit,
"CreateOn"  datetime,
 unique(Id asc)
);
GO
Create index main.Histroys_id on Histroys (Id ASC);
GO



-- ----------------------------
-- Table structure for Users
-- Date: 2011-11-11
-- ----------------------------
CREATE TABLE [Users](
"Id"  integer Primary Key AutoIncrement not null,
"Name"  nvarchar(32),
"Pad"  nvarchar(32),
"Statu"  bit,
"UserType"  int,
"LoginOn"  datetime,
"CreateOn"  datetime,
 unique(Id asc)
);
GO
Create index main.Users_id on Users (Id ASC);
GO

-- ----------------------------
-- Table structure for Admins
-- Date: 2011-11-11
-- ----------------------------
CREATE TABLE [Admins](
"Id"  integer Primary Key AutoIncrement not null,
"Name"  nvarchar(32),
"Value"  nvarchar(32),
"DateTime"  datetime,
 unique(Id asc)
);
GO
Create index main.Admins_id on Admins (Id ASC);
GO
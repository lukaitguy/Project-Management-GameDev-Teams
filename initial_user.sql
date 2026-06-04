use ProjektniMenadzment

GO
/*Creating app db user for administrator role*/
INSERT INTO Korisnici(Id, Ime, Prezime, Email, BrojTelefona, IdentityUserId, DatumKreiranja)
VALUES (NEWID(), 'Admin', '', 'administrator@pmdb.com', '', '881ecb32-8773-4199-8627-05dc87d5a810', GETDATE())

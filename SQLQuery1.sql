select * from usuarios;

insert into [Usuarios](Email,NombreCompleto,Password,FechaRegistro,Estado,Restablecer,RolAcceso)
values('admin@gmail.com','Admin','123456',getdate(),'A',1,'Admin')


using AdaptadorPostgreSQL.Usuarios.Entidades;
using AdaptadorPostgreSQL.Usuarios.Mappers;
using Dominio.Usuarios.Modelo;
using Dominio.Usuarios.Puertos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorPostgreSQL.Usuarios.Adaptadores
{
    public class UsuarioRepositorioPostgreSQL : IUsuarioRepositorio
    {
        private readonly PostgreSQLDbContext _postgreSQLDbContext;
        private readonly MapToUsuarioEntity _mapToUsuarioEntity;
        private readonly MapToUserModelDominio _mapToUserModelDominio;

        public UsuarioRepositorioPostgreSQL(PostgreSQLDbContext dbContext)
        {
            _postgreSQLDbContext = dbContext;
            _mapToUsuarioEntity = new MapToUsuarioEntity();
            _mapToUserModelDominio = new MapToUserModelDominio();
        }

        public long AddUsuario(UsuarioModelo usuario)
        {
            UsuarioEntity usuarioEntity = _mapToUsuarioEntity.MapToUsusarioEntidad(usuario);
            _postgreSQLDbContext.Add(usuarioEntity);
            SaveChanges();
            return usuarioEntity.Id;
        }

        public UsuarioModelo ListUsuarioPorCorreo(string Correo)
        {
            UsuarioEntity usuarioEntity = _postgreSQLDbContext.Usuarios.FirstOrDefault(u=>u.Correo == Correo);
            if (usuarioEntity == null)
            {
                return new UsuarioModelo();
            }
            UsuarioModelo usuario = _mapToUserModelDominio.MapToUserDomainModel(usuarioEntity);

            return usuario;
        }

        public UsuarioModelo ListUsuarioPorId(long id)
        {

            UsuarioEntity usuarioEntity = _postgreSQLDbContext.Usuarios.Find(id);
            UsuarioModelo usuario = _mapToUserModelDominio.MapToUserDomainModel(usuarioEntity);

            return usuario;
        }

        //public void ActualizarUsuario(long id,UsuarioModelo usuarioActualizar)
        //{
        //    UsuarioEntity usuarioEntity = _mapToUsuarioEntity.MapToUsusarioEntidad(usuarioActualizar);
        //    var entry = _postgreSQLDbContext.Entry(usuarioEntity);
        //    entry.CurrentValues.SetValues(usuarioEntity);

        //    // Marcar como modificados solo los campos que cambiaron
        //    foreach (var propiedad in entry.Properties)
        //    {
        //        if (propiedad.IsModified)
        //        {
        //            entry.Property(propiedad.Metadata.Name).IsModified = true;
        //        }
        //    }

        //    SaveChanges();
        //}


        public void SaveChanges()
        {
            _postgreSQLDbContext.SaveChanges();
        }
    }
}

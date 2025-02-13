using Dominio.Servicios.ServicioValidaciones.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Servicios.ServicioValidaciones.Implementaciones
{
    public class PropertyValidator : IpropertyModelValidate
    {
        public bool ValidarPropiedad<T>(string nombrePropiedad)
        {
            if (string.IsNullOrEmpty(nombrePropiedad))
            {
                throw new ArgumentException($"La propiedad no puede ser nula o vacía.");
            }
            var propiedad = typeof(T).GetProperty(nombrePropiedad, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propiedad == null)
            {
                throw new ArgumentException($"La propiedad '{nombrePropiedad}' no existe en la entidad {typeof(T).Name}.");
            }

            return true; 
        }
    }
}

using AdaptadorAPI.Servicios.Contratos;
using Dominio.Entidades.Reviews.Puertos;
using Dominio.Servicios.ServicioValidaciones.Contratos;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdaptadorAPI.Servicios.Implementaciones
{
    public class ConvertirJsonElementToDiccionarioTextJson : IconverterJsonElementToDictionary
    {

        private readonly IpropertyModelValidate _ipropertyModelValidate;

        public ConvertirJsonElementToDiccionarioTextJson(IpropertyModelValidate ipropertyModelValidate)
        {
            _ipropertyModelValidate = ipropertyModelValidate;
        }
        public Dictionary<string, object> ConvertirJsonElementADiccionarioTipado<T>(Dictionary<string, JsonElement> cambiosJson)
        {
            var tipoEntidad = typeof(T);
            var cambios = new Dictionary<string, object>();

            foreach (var cambio in cambiosJson)
            {
                //Validamos existencia del campo en nuestro modelo
                if (!_ipropertyModelValidate.ValidarPropiedad<T>(cambio.Key))
                {
                    throw new ArgumentException($"El campo {cambio.Key} no existe en la entidad de Reviews.");
                }
                // Convertir JsonElement al tipo de la propiedad
                var propiedad = tipoEntidad.GetProperty(cambio.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var valor = ConvertirJsonElement(propiedad.PropertyType, cambio.Value);

                cambios.Add(cambio.Key, valor);
            }

            return cambios;
        }

        private object ConvertirJsonElement(Type tipoDestino, JsonElement elemento)
        {
            try
            {
                // Opciones de deserialización (consistentes con tu configuración global)
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                // Convertir el JsonElement a un objeto del tipo destino
                return elemento.Deserialize(tipoDestino, jsonOptions);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error al convertir valor: {ex.Message}");
            }
        }

    }
}

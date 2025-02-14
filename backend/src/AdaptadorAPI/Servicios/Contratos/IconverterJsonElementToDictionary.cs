using System.Text.Json;

namespace AdaptadorAPI.Servicios.Contratos
{
    public interface IconverterJsonElementToDictionary
    {
        Dictionary<string, object> ConvertirJsonElementADiccionarioTipado<T>(Dictionary<string, JsonElement> cambiosJson);
    }
}

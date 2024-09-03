using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Companies.Queries.GetCompanyList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Automatically register mappings from the assembly
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(t => typeof(IMapFrom<>).IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                .ToList();

            foreach (var @interface in interfaces)
            {
                var methodInfo = @interface.GetMethod("Mapping");
                var instance = Activator.CreateInstance(type);
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}


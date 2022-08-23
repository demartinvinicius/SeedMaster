using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nudes.SeedMaster.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nudes.SeedMaster.Seeder;

public class EfCoreHelpers
{
    public static void FillCleanableEntitiesQueue(DbContext context,Queue<IEntityType> queue)
    {
        ActualFillQueue(context, queue, true);
    }
    public static void FillSeedableQueue(DbContext context,Queue<IEntityType> queue)
    {
        ActualFillQueue(context,queue,false);
    }
    private static void ActualFillQueue(DbContext context,Queue<IEntityType> queue,bool useCleanAttribute)
    {
        IEnumerable<string> atributes = context.GetType().GetProperties().Where(x => x.GetCustomAttribute<EnableSeederAttribute>() != null && 
            (!useCleanAttribute || x.GetCustomAttribute<EnableSeederAttribute>().Clean)).Select(x => x.PropertyType.GenericTypeArguments.FirstOrDefault().Name);
        var entities = context.Model.GetEntityTypes().Where(y => atributes.Any(x => x == y.ClrType.Name));
        foreach (var entity in entities)
        {
            queue.Enqueue(entity);
        }
    }

    public static bool EntityHasData(DbContext context,IEntityType entity)
    {
        MethodInfo methodinfo = context.GetType().GetMethods().Single(p => p.Name == nameof(DbContext.Set) && !p.GetParameters().Any());
        var method = methodinfo.MakeGenericMethod(entity.ClrType).Invoke(context, null) as IEnumerable<object>;
        if (method.Any())
            return true;
        return false;
    }

}

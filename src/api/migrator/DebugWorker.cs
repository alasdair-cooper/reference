using System.Linq.Expressions;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Data.Types;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql.EntityFrameworkCore.PostgreSQL.Design.Internal;

namespace AlasdairCooper.Reference.Api.Migrator;

public class DebugWorker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ReferenceDbContext>();

        var dbSpecificDesignTimeServices = new NpgsqlDesignTimeServices();
        var designTimeServiceCollection = new ServiceCollection();
        
        designTimeServiceCollection.AddEntityFrameworkDesignTimeServices().AddDbContextDesignTimeServices(dbContext);

        dbSpecificDesignTimeServices.ConfigureDesignTimeServices(designTimeServiceCollection);
        
        var designTimeServiceProvider = designTimeServiceCollection.BuildServiceProvider();
        var scaffolder = designTimeServiceProvider.GetRequiredService<IMigrationsScaffolder>();
        var migration = scaffolder.ScaffoldMigration("Debug", "Migration.Debug");
    }
}

public class ComplexTypeParameterBindingFactory : IParameterBindingFactory
{
    public bool CanBind(Type parameterType, string parameterName)
    {
        return parameterType == typeof(Money);
    }

    public ParameterBinding Bind(IReadOnlyEntityType entityType, Type parameterType, string parameterName) => 
        Bind((IEntityType)entityType, parameterType);

    public ParameterBinding Bind(IMutableEntityType entityType, Type parameterType, string parameterName) => 
        Bind((IEntityType)entityType, parameterType);

    public ParameterBinding Bind(IConventionEntityType entityType, Type parameterType, string parameterName) => 
        Bind((IEntityType)entityType, parameterType);

    private static ParameterBinding Bind(IEntityType entityType, Type parameterType) => new ComplexTypeParameterBinding(parameterType);
}

public class ComplexTypeParameterBinding : ParameterBinding
{
    public ComplexTypeParameterBinding(Type parameterType, params IPropertyBase[]? consumedProperties) : base(parameterType, consumedProperties)
    {
    }

    public override Expression BindToParameter(ParameterBindingInfo bindingInfo)
    {
        var property = ConsumedProperties[0];

        return Expression.Call(bindingInfo.MaterializationContextExpression, MaterializationContext.GetValueBufferMethod)
            .CreateValueBufferReadValueExpression(property.ClrType, bindingInfo.GetValueBufferIndex(property), property);
    }

    public override ParameterBinding With(IPropertyBase[] consumedProperties)
        => new PropertyParameterBinding((IProperty)consumedProperties.Single());
} 
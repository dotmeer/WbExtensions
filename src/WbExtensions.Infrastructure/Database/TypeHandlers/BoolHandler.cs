using System.Data;
using Dapper;

namespace WbExtensions.Infrastructure.Database.TypeHandlers;

internal sealed class BoolHandler : SqlMapper.TypeHandler<bool>
{
    public override void SetValue(IDbDataParameter parameter, bool value)
    {
        parameter.Value = value ? 1 : 0;
    }

    public override bool Parse(object value)
    {
        return value is 1L;
    }
}
using System;
using System.Data;
using Dapper;

namespace WbExtensions.Infrastructure.Database.TypeHandlers;

internal sealed class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        var utc = value.ToUniversalTime();
        parameter.Value = utc.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
    }

    public override DateTime Parse(object value)
    {
        var unspecified = DateTime.Parse(value.ToString()!);

        return DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
    }
}
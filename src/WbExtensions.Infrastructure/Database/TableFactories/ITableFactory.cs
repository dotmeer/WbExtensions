namespace WbExtensions.Infrastructure.Database.TableFactories;

public interface ITableFactory<TModel>
{
    public void Migrate();
}
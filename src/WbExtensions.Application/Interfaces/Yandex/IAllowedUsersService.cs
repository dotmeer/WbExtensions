namespace WbExtensions.Application.Interfaces.Yandex;

public interface IAllowedUsersService
{
    bool IsUserAllowed(string email);
}
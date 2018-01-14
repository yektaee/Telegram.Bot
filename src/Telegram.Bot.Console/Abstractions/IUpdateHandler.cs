using System.Threading.Tasks;
using Telegram.Bot.Types;

// ReSharper disable once CheckNamespace
namespace Telegram.Bot.Console
{
    public interface IUpdateHandler
    {
        Task<bool> CanHandleUpdateAsync(Update update);

        Task HandleUpdateAsync(Update update, ITelegramBotClient bot);
    }
}

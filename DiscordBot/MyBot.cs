using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    class MyBot
    {
        // Instances 
        DiscordClient discord;
        CommandService commands;

        Random rand;

        string[] freshestMemes;
        string[] quotes;

        /// <summary>
        /// Constructs a new MyBot
        /// </summary>
        public MyBot()
        {
            // Creates a new random
            rand = new Random();

            // All the freshest memes
            freshestMemes = new string[]
            {
                "images/poem.jpg",
                "images/poem2.jpg",
                "images/daler.jpg"
            };

            quotes = System.IO.File.ReadAllLines("quotes.txt");

            // instanciates a new DiscordClient
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            // Sets the discord command prefix
            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = false;
            });

            commands = discord.GetService<CommandService>();

            RegisterMemeCommand();
            RegisterQuoteCommand();
            RegisterAddQuoteCommand();
            RegisterRefreshQuotesCommand();

            discord.ExecuteAndWait(async () =>
            {
                string apiKey = System.IO.File.ReadAllText("discord_api_key.txt");

                await discord.Connect(apiKey, TokenType.Bot);
            });
        }

        #region Registers

        /// <summary>
        /// Registers the Meme command
        /// </summary>
        private void RegisterMemeCommand()
        {
            commands.CreateCommand("meme")
                .Do(async (e) =>
                {
                    int randomMemeIndex = rand.Next(freshestMemes.Length);
                    string memeToPost = freshestMemes[randomMemeIndex];

                    await e.Channel.SendFile(memeToPost);
                });
        }

        /// <summary>
        /// Registers the Quote command
        /// </summary>
        private void RegisterQuoteCommand()
        {
            commands.CreateCommand("quote")
                .Do(async (e) =>
                {
                    int randomQuoteIndex = rand.Next(quotes.Length);
                    string quoteToPost = quotes[randomQuoteIndex];

                    await e.Channel.SendMessage(quoteToPost.TrimEnd(quoteToPost[quoteToPost.Length - 1]));
                });
        }

        /// <summary>
        /// Registers the AddQuote command
        /// </summary>
        private void RegisterAddQuoteCommand()
        {
            commands.CreateCommand("addquote")
                .Description("Adds a new quote to the database.")               
                .Parameter("Quote", ParameterType.Multiple)
                .Do(async (e) =>
                {
                    string message = e.Message.Text;

                    string splitMessage = message.Substring(10);

                    await e.Channel.SendMessage("Added quote: " + splitMessage);

                    //System.IO.File.WriteAllText("quotes.txt", splitMessage);

                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter("quotes.txt", true))
                    {
                        file.WriteLine(splitMessage + ",");
                    }
                });
        }

        /// <summary>
        /// Registers the RefreshQuotes command
        /// </summary>
        private void RegisterRefreshQuotesCommand()
        {
            commands.CreateCommand("refreshquotes")
                .Description("Refreshes the list of quotes, use this command after adding a quote.")
                .Do(async (e) =>
                {
                    quotes = System.IO.File.ReadAllLines("quotes.txt");
                    await e.Channel.SendMessage("Quotes have been refreshed!");
                });
        }
        #endregion

        /// <summary>
        /// Logs messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

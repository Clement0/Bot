using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace MoodBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 

        static private int mode = 0;
        static private string title = "";

        private string ask(int mode, string type, string message)
        {
            if (MessagesController.title.Equals(""))
            {
                MessagesController.mode = mode;
                return "De quel film voulez-vous avoir la " + type + "?";
            }
            MessagesController.mode = 0;
            return message;

        }

        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                string answer;
                Rootobject LUIS = await GetEntityFromLUIS(message.Text);
                if (LUIS.intents.Count() > 0)
                {
                    if (LUIS.intents[0].intent.Equals("SetAnswer")
                        && LUIS.entities.Count() == 1
                        && LUIS.entities[0].type.Equals("NoAnswer"))
                    {
                        mode = 0;
                    }
                    else if (mode == 1)
                    {
                        LUIS.intents[0].intent = "DisplayResume";
                    }
                    else if (mode == 2)
                    {
                        LUIS.intents[0].intent = "DisplayLength";
                    }
                    else if (mode == 3)
                    {
                        LUIS.intents[0].intent = "DisplayRate";
                    }

                    for (int i = 0; i < LUIS.entities.Count(); i++)
                    {
                        if (LUIS.entities[i].type.Equals("Title"))
                            title = LUIS.entities[i].entity;
                    }

                    switch (LUIS.intents[0].intent)
                    {
                        case "SetAnswer":
                            answer = "Que desirez-vous?";
                            break;

                        case "DisplayTitles":
                            answer = "les titres disponibles ce soir sont :  *listes*";
                            break;
                        case "DisplayResume":
                            answer = ask(1, "description", "Voici la description de " + title + " : *description*");
                            break;
                        case "DisplayLength":
                            answer = ask(2, "durée", title + " dure : *durée*");
                            break;

                        case "DisplayRate":
                            answer = ask(3, "note", title + " a reçu la note de: *note*");
                            break;

                        case "SetMood":

                            if (LUIS.entities.Count() == 0)
                                answer = "Dis moi en plus sur tes sentiments";
                            else
                                switch (LUIS.entities[0].type)
                                {
                                    case "SadFeeling":
                                        answer = "Tu as l'air triste. Voici un programme qui pourrait te convenir."
                                            + "*Afficher les film de type "
                                            + "Action "
                                            //+ "Comedie "
                                            + "Policier "
                                            + "Horreur "
                                            //+ "Documentaire "
                                            //+ "Drame "
                                            + "Romance "
                                            + "SF "
                                            //+ "Thriller "
                                            + "*"
                                            ;


                                        break;
                                    case "HappyFeeling":
                                        answer = "Tu as l'air heureux. Voici un programme qui pourrait te convenir."
                                        +"*Afficher les film de type "
                                        + "Action "
                                        + "Comedie "
                                        + "Policier "
                                        + "Horreur "
                                        + "Documentaire "
                                        //+ "Drame "
                                        + "Romance "
                                        + "SF "
                                        + "Thriller "
                                        + "*"
                                        ;
                                        break;
                                    case "TiredFeeling":
                                        answer = "Tu as l'air fatigué. Voici un programme qui pourrait te convenir."
                                        +"*Afficher les film de type "
                                        + "Action "
                                        + "Comedie "
                                        //+ "Policier "
                                        + "Horreur "
                                        //+ "Documentaire "
                                        + "Drame "
                                        + "Romance "
                                        + "SF "
                                        //+ "Thriller "
                                        + "*"
                                        ;
                                        break;
                                    case "AnxiousFeeling":
                                        answer = "Tu as l'air anxieux. Voici un programme qui pourrait te convenir."
                                        +"*Afficher les film de type "
                                        + "Action "
                                        + "Comedie "
                                        //+ "Policier "
                                        //+ "Horreur "
                                        //+ "Documentaire "
                                        + "Drame "
                                        + "Romance "
                                        + "SF "
                                        //+ "Thriller "
                                        + "*"
                                        ;
                                        break;
                                    case "EnergyFeeling":
                                        answer = "Tu as l'air en pleine forme. Voici un programme qui pourrait te convenir."
                                        +"*Afficher les film de type "
                                       + "Action "
                                       + "Comedie "
                                       + "Policier "
                                       + "Horreur "
                                       + "Documentaire "
                                       + "Drame "
                                       + "Romance "
                                       + "SF "
                                       + "Thriller "
                                       + "*"
                                       ;
                                        break;
                                    default:
                                        answer = "Et comment tu te sens par rapport à ça?";
                                        break;

                                }
                            break;
                        default:
                            answer = "Désolé je ne vous ai pas compris...";
                            break;
                    }
                }
                else
                {
                    answer = "Désolé je ne vous ai pas compris...";
                }

                return message.CreateReplyMessage(answer);



            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
                mode = 0;
                title = "";
            }
            else if (message.Type == "BotAddedToConversation")
            {
                return message.CreateReplyMessage($"Bonjour ! Comment s'est passé votre journée?");

            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
                mode = 0;
                title = "";
            }

            return null;
        }

       
        private static async Task<Rootobject> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            Rootobject Data = new Rootobject();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=6510dc6d-c79d-402d-994a-419164f25627&subscription-key=5455eb8082b9465282db6623b157d8a0&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<Rootobject>(JsonDataResponse);
                }
            }
            return Data;
        }


    }
}
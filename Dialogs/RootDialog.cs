using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using GreatWall.Dialogs;
using System.Collections.Generic;

namespace GreatWall
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;
        string strMessage;
        private string strWelcomeMessage = "[Medicine Bot]";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            
            await context.PostAsync(strWelcomeMessage);

            var message = context.MakeMessage();
            var actions = new List<CardAction>();

            actions.Add(new CardAction() { Title = "1. �Ǿ�ǰ ������ ã��", Value = "��Ī", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "2. �Ǿ�ǰ Ư¡���� ã��", Value = "Ư¡", Type = ActionTypes.ImBack });

            message.Attachments.Add(new HeroCard { Title = "�Ǿ�ǰ�� ã�� ����� �������ּ���.", Buttons = actions }.ToAttachment());

            await context.PostAsync(message);

            context.Wait(SendWelcomeMessageAsync);

        }

        public async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string strSelected = activity.Text.Trim();

            if(strSelected == "��Ī")
            {
                context.Call(new OrderDialog(strSelected), DialogResumeAfter);
            }
            else if (strSelected == "Ư¡")
            {
                context.Call(new OrderDialog(strSelected), DialogResumeAfter);
            }
            else
            {
                strMessage = "�ٽ� �������ּ���.";
                await context.PostAsync(strMessage);
                context.Wait(SendWelcomeMessageAsync);
            }
        }

        public async Task DialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                strMessage = await result;

                await this.MessageReceivedAsync(context, result);
            }
            catch(TooManyAttemptsException)
            {
                await context.PostAsync("Error occurred...");
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}
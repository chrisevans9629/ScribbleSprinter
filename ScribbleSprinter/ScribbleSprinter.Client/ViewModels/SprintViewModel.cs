﻿namespace ScribbleSprinter.Client.ViewModels
{
    public class SprintViewModel : ViewModelBase
    {
        public int Value { get; set; }
        public int TimeLimit = 5;
        private string text = "";
        System.Timers.Timer sprintTimer { get; } = new();
        System.Timers.Timer typingTimer { get; } = new();

        public Action<string>? OnSprintCompleted { get; set; }
        public Action? OnGameOver { get; set; }
        public string Text
        {
            get => text;
            set
            {
                if (value.Length > text.Length)
                {
                    typingTimer.Stop();
                    if (Value < TimeLimit)
                    {
                        typingTimer.Start();
                    }

                    if (!sprintTimer.Enabled)
                    {
                        sprintTimer.Start();
                    }
                }

                text = value;
            }
        }

        public override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                typingTimer.Interval = 1000;
                typingTimer.Elapsed += (s,e) => InvokeAsync(TypingTimer_Elapsed);

                sprintTimer.AutoReset = true;
                sprintTimer.Interval = 1000;
                sprintTimer.Elapsed += (s, e) => InvokeAsync(Countdown);
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        public Task Countdown()
        {
            Value++;
            if (Value >= TimeLimit)
            {
                typingTimer.Stop();
                sprintTimer.Stop();
                OnSprintCompleted?.Invoke(Text);
            }
            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task TypingTimer_Elapsed()
        {
            if (Value < TimeLimit)
            {
                typingTimer.Stop();
                sprintTimer.Stop();
                OnGameOver?.Invoke();
                StateHasChanged();
            }
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            typingTimer?.Dispose();
            sprintTimer?.Dispose();
            base.Dispose();
        }
    }
}
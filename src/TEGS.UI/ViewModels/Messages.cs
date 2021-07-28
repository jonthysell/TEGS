// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using GalaSoft.MvvmLight.Messaging;

namespace TEGS.UI.ViewModels
{
    public abstract class CallbackMessage<T> : MessageBase
    {
        private readonly Action<T> Callback;

        protected CallbackMessage(Action<T> callback = null) : base()
        {
            Callback = callback;
        }

        public void Process(T result)
        {
            Callback?.Invoke(result);
        }
    }

    public abstract class ShowViewModelMessage<T> : MessageBase where T : ViewModelBase
    {
        public readonly T ViewModel;

        protected ShowViewModelMessage(T viewmodel) : base()
        {
            ViewModel = viewmodel ?? throw new ArgumentNullException(nameof(viewmodel));
        }
    }

    public abstract class ShowAcceptRejectViewModelMessage<T> : ShowViewModelMessage<T> where T : AcceptRejectViewModelBase
    {
        private readonly Action<T> Callback;

        protected ShowAcceptRejectViewModelMessage(T viewmodel, Action<T> callback = null) : base(viewmodel)
        {
            Callback = callback;
        }

        public void Process()
        {
            Callback?.Invoke(ViewModel);
        }
    }

    public class ExceptionMessage : MessageBase
    {
        public readonly Exception Exception;

        public ExceptionMessage(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }

    [Flags]
    public enum FileType
    {
        None = 0,
        Graph = 1,
        SimulationOutput,
    }

    public class OpenFileMessage : CallbackMessage<string>
    {
        public readonly string Title;
        public readonly FileType FileType;

        public OpenFileMessage(string title, FileType fileType, Action<string> callback) : base(callback)
        {
            Title = title;
            FileType = fileType;
        }
    }

    public class SaveFileMessage : CallbackMessage<string>
    {
        public readonly string Title;
        public readonly FileType FileType;

        public SaveFileMessage(string title, FileType fileType, Action<string> callback) : base(callback)
        {
            Title = title;
            FileType = fileType;
        }
    }

    public class ShowGraphPropertiesMessage : ShowAcceptRejectViewModelMessage<GraphPropertiesViewModel>
    {
        public ShowGraphPropertiesMessage(ObservableGraph graph, Action<GraphPropertiesViewModel> callback = null) : base(new GraphPropertiesViewModel(graph), callback) { }
    }

    public class ShowGraphStateVariablesMessage : ShowAcceptRejectViewModelMessage<GraphStateVariablesViewModel>
    {
        public ShowGraphStateVariablesMessage(ObservableGraph graph, Action<GraphStateVariablesViewModel> callback = null) : base(new GraphStateVariablesViewModel(graph), callback) { }
    }
}

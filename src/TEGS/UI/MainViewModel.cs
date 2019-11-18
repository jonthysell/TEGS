// 
// MainViewModel.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if TEGSUI

using System;
using System.IO;
using System.Xml;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Command;

namespace TEGS.UI
{
    public class MainViewModel : ViewModelBase
    {
        public IDialogService DialogService
        {
            get
            {
                return SimpleIoc.Default.GetInstance<IDialogService>();
            }
        }

        public ObservableGraph CurrentGraph
        {
            get
            {
                return _graph;
            }
            set
            {
                _graph = value;
                RaisePropertyChanged(nameof(CurrentGraph));
                RaisePropertyChanged(nameof(IsGraphLoaded));
                SaveGraphAsAsync.RaiseCanExecuteChanged();
            }
        }
        private ObservableGraph _graph;

        public bool IsGraphLoaded
        {
            get
            {
                return (null != CurrentGraph);
            }
        }

        public RelayCommand NewGraphAsync
        {
            get
            {
                return _newGraphAsync ?? (_newGraphAsync = new RelayCommand(async () =>
                {
                    try
                    {
                        CurrentGraph = new ObservableGraph(new Graph());
                    }
                    catch (Exception ex)
                    {
                        await DialogService.ShowExceptionAsync(ex);
                    }
                }));
            }
        }
        private RelayCommand _newGraphAsync;

        public RelayCommand OpenGraphAsync
        {
            get
            {
                return _openGraphAsync ?? (_openGraphAsync = new RelayCommand(async () =>
                {
                    try
                    {
                        Stream inputStream = await DialogService.ShowPromptForInputStreamAsync(new string[] { ".xml" });
                        if (null != inputStream)
                        {
                            using (XmlReader xmlReader = XmlReader.Create(inputStream))
                            {
                                Graph graph = Graph.LoadXml(xmlReader);
                                CurrentGraph = new ObservableGraph(graph);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DialogService.ShowExceptionAsync(ex);
                    }
                }));
            }

        }
        private RelayCommand _openGraphAsync;

        public RelayCommand SaveGraphAsAsync
        {
            get
            {
                return _saveGraphAsAsync ?? (_saveGraphAsAsync = new RelayCommand(async () =>
                {
                    try
                    {
                        Stream outputStream = await DialogService.ShowPromptForOutputStreamAsync("TEGS Model", new string[] { ".xml" });
                        if (null !=  outputStream)
                        {
                            using (XmlWriter xmlWriter = XmlWriter.Create(outputStream))
                            {
                                CurrentGraph.Graph.SaveXml(xmlWriter);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DialogService.ShowExceptionAsync(ex);
                    }
                }, () =>
                {
                    return IsGraphLoaded;
                }));
            }
        }
        private RelayCommand _saveGraphAsAsync;
    }
}

#endif

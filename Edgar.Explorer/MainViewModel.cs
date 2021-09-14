using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using M5Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Edgar.Explorer
{
    public class MainViewModel : ViewModelBase
    {
        private const string ALL_FORM_TYPES = "All Form Types";

        private readonly EdgarClient _edgarClient = new();
        private readonly EdgarFilingClient _edgarFilingsClient = new();

        private EdgarFiling _selectedFiling;
        private string _selectedFormType;

        private bool _isbusy;

        private EdgarFilingLookup _edgarFilingsLookup;

        private IEnumerable<EdgarFiling> _filings;
        private IEnumerable<string> _formTypes;

        private string _edgarFilingText;

        public MainViewModel()
        {
            LoadCommand = new RelayCommand(() =>
            {
                _ = Task.Run(LoadAsync);
            });
        }

        public ICommand LoadCommand { get; }

        public int FilingsCount
        {
            get
            {
                return Filings?.Count() ?? 0;
            }
        }

        public IEnumerable<EdgarFiling> Filings
        {
            get { return _filings; }
            private set
            {
                if (_filings != value)
                {
                    _filings = value;

                    RaisePropertyChanged(() => Filings);
                    RaisePropertyChanged(() => FilingsCount);
                }
            }
        }

        public EdgarFiling SelectedFiling
        {
            get
            {
                return _selectedFiling;
            }

            set
            {
                if (_selectedFiling != value)
                {
                    _selectedFiling = value;
                    RaisePropertyChanged(() => SelectedFiling);

                    if (_selectedFiling == null)
                    {
                        EdgarFilingText = null;
                    }
                    else
                    {

                        _ = Task.Run(async () =>
                        {
                            var filing = _selectedFiling;

                            var text = await _edgarFilingsClient.GetEdgarFilingAsync(filing);

                            EdgarFilingText = text;
                        });
                    }

                }
            }
        }

        public IEnumerable<string> FormTypes
        {
            get { return _formTypes; }
            private set
            {
                if (_formTypes != value)
                {
                    _formTypes = value;
                    RaisePropertyChanged(() => FormTypes);
                }
            }
        }

        public string SelectedFormType
        {
            get { return _selectedFormType; }
            set
            {
                if (_selectedFormType != value)
                {
                    _selectedFormType = value;
                    RaisePropertyChanged(() => SelectedFormType);

                    _ = Task.Run(() =>
                    {
                        if (_selectedFormType == ALL_FORM_TYPES)
                        {
                            Filings = _edgarFilingsLookup.GetAllFilings();
                        }
                        else
                        {
                            Filings = _edgarFilingsLookup.GetFilingsByForm(_selectedFormType);
                        }
                    });
                }
            }
        }

        public bool AreControlsEnabled { get; private set; } = true;

        public bool IsBusy
        {
            get { return _isbusy; }
            set
            {
                _isbusy = value;
                RaisePropertyChanged(() => IsBusy);
                AreControlsEnabled = !_isbusy;
                RaisePropertyChanged(() => AreControlsEnabled);
            }
        }

        public string EdgarFilingText
        {
            get { return _edgarFilingText; }
            private set
            {
                if (_edgarFilingText != value)
                {
                    _edgarFilingText = value;
                    RaisePropertyChanged(() => EdgarFilingText);
                }
            }
        }

        private async Task LoadAsync()
        {
            IsBusy = true;

            try
            {
                if (_edgarFilingsLookup == null)
                {
                    _edgarFilingsLookup = await _edgarClient.GetEdgarFilingsAsync();
                    
                    var filings = _edgarFilingsLookup.GetAllFilings();
                    var formTypes = filings.Select(x => x.FormType).Distinct().OrderBy(x => x).ToList();
                    formTypes.Insert(0, ALL_FORM_TYPES);

                    FormTypes = formTypes;
                    SelectedFormType = ALL_FORM_TYPES;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
             {
                 base.RaisePropertyChanged(propertyExpression);
             }));
        }

    }
}
